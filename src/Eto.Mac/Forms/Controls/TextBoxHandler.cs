using System;
using Eto.Forms;
using Eto.Mac.Forms.Controls;
using Eto.Drawing;

namespace Eto.Mac.Forms.Controls
{
	public interface ITextBoxWithMaxLength
	{
		int MaxLength { get; }
	}

	interface IMacTextBoxHandler
	{
		TextBox.ICallback Callback { get; }

		TextBox Widget { get; }
	}

	public class EtoFormatter : NSFormatter
	{
		WeakReference handler;

		public ITextBoxWithMaxLength Handler { get { return (ITextBoxWithMaxLength)handler.Target; } set { handler = new WeakReference(value); } }

		public override string StringFor(NSObject value)
		{
			return value as NSString;
		}

		[Export("getObjectValue:forString:errorDescription:")]
		public bool GetObjectValue(ref NSObject obj, NSString value, ref IntPtr error)
		{
			obj = value;
			return true;
		}

		[Export("isPartialStringValid:proposedSelectedRange:originalString:originalSelectedRange:errorDescription:")]
		public bool IsPartialStringValid(ref NSString value, IntPtr proposedSelRange, NSString origString, NSRange origSelRange, ref IntPtr error)
		{
			var h = Handler;
			if (h == null)
				return true;
			if (h.MaxLength > 0)
			{
				int size = (int)value.Length;
				if (size > h.MaxLength)
				{
					return false;
				}
			}
			return true;
		}

		[Export("attributedStringForObjectValue:withDefaultAttributes:")]
		public NSAttributedString AttributedStringForObjectValue(IntPtr anObject, NSDictionary attributes)
		{
			return null;
		}
	}
	
	public interface IColorizeCell
	{
		Color? Color { get; set; }
	}
	
	public class EtoTextFieldCell : NSTextFieldCell, IColorizeCell
	{
		ColorizeView colorize;

		public EtoTextFieldCell()
		{
			StringValue = string.Empty;
		}

		public Color? Color 
		{ 
			get => colorize?.Color;
			set => ColorizeView.Create(ref colorize, value);
		}

		public override void DrawInteriorWithFrame(CGRect cellFrame, NSView inView)
		{
			colorize?.End();
			base.DrawInteriorWithFrame(cellFrame, inView);
		}
		public override void DrawWithFrame(CGRect cellFrame, NSView inView)
		{
			colorize?.Begin(cellFrame, inView);
			base.DrawWithFrame(cellFrame, inView);
		}
	}

	public class EtoTextField : NSTextField, IMacControl, ITextBoxWithMaxLength
	{
		public WeakReference WeakHandler { get; set; }

		IMacViewHandler Handler => WeakHandler.Target as IMacViewHandler;
		IMacText TextHandler => WeakHandler.Target as IMacText;
		ITextBoxWithMaxLength MaxLengthHandler => WeakHandler.Target as ITextBoxWithMaxLength;

		public int MaxLength { get { return MaxLengthHandler?.MaxLength ?? 0; } }
		
		public EtoTextField(IntPtr handle)
			: base(handle)	
		{
		}

		EtoFormatter formatter;

		public EtoTextField()
		{
			Cell = new EtoTextFieldCell();
			Bezeled = true;
			Editable = true;
			Selectable = true;
			Formatter = formatter = new EtoFormatter { Handler = this };
			Cell.Scrollable = true;
			Cell.Wraps = false;
			Cell.UsesSingleLineMode = true;
		}

		[Export("textViewDidChangeSelection:")]
		public void TextViewDidChangeSelection(NSNotification notification)
		{
			var h = TextHandler;
			if (h != null)
			{
				var textView = (NSTextView)notification.Object;
				h.SetLastSelection(textView.SelectedRange.ToEto());
			}
		}

		public override void MouseDown(NSEvent theEvent)
		{
			var h = TextHandler;
			if (h != null && h.AutoSelectMode == AutoSelectMode.Always && CurrentEditor?.SelectedRange.Length == 0)
			{
				CurrentEditor?.SelectAll(this);
			}
			
			var handler = Handler;
			if (handler == null)
				return;
			var args = MacConversions.GetMouseEvent(handler, theEvent, false);
			if (theEvent.ClickCount >= 2)
				handler.Callback.OnMouseDoubleClick(handler.Widget, args);
			
			if (!args.Handled)
			{
				handler.Callback.OnMouseDown(handler.Widget, args);
			}
			if (!args.Handled)
			{
				handler.SuppressMouseEvents++;
				base.MouseDown(theEvent);
				handler.SuppressMouseEvents--;
				
				// some controls use event loops until mouse up, so we need to trigger the mouse up here.
				handler.TriggerMouseCallback();
			}
		}
	}


	public class TextBoxHandler : TextBoxHandler<TextBox, TextBox.ICallback>
	{
	}

	public class TextBoxHandler<TWidget, TCallback> : MacText<EtoTextField, TWidget, TCallback>, TextBox.IHandler, ITextBoxWithMaxLength, IMacTextBoxHandler
		where TWidget: TextBox
		where TCallback: TextBox.ICallback
	{
		protected override void Initialize()
		{
			MaxLength = 0;
			base.Initialize();
		}

		protected override EtoTextField CreateControl()
		{
			return new EtoTextField();
		}

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			var size = base.GetNaturalSize(availableSize);
			size.Width = Math.Max(100, size.Height);
			return size;
		}

		static readonly IntPtr selShouldChangeText = Selector.GetHandle("shouldChangeTextInRange:replacementString:");

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TextControl.TextChangedEvent:
					Control.Changed += HandleTextChanged;
					break;
				case TextBox.TextChangingEvent:
					// handled by custom field editor
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		static void HandleTextChanged (object sender, EventArgs e)
		{
			var h = GetHandler(sender) as TextBoxHandler<TWidget, TCallback>;
			h.Callback.OnTextChanged(h.Widget, EventArgs.Empty);
		}

		public bool ReadOnly
		{
			get { return !Control.Editable; }
			set { Control.Editable = !value; }
		}

		public int MaxLength
		{
			get;
			set;
		}

		public string PlaceholderText
		{
			get { return ((NSTextFieldCell)Control.Cell).PlaceholderString; }
			set { ((NSTextFieldCell)Control.Cell).PlaceholderString = value ?? string.Empty; }
		}

		TextBox.ICallback IMacTextBoxHandler.Callback => Callback;

		TextBox IMacTextBoxHandler.Widget => Widget;

		protected override bool TextChanging(string oldText, string newText)
		{
			var args = new TextChangingEventArgs(oldText, newText, false);
			Callback.OnTextChanging(Widget, args);
			return args.Cancel;
		}
	}
}
