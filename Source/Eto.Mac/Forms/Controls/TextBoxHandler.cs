using System;
using Eto.Forms;
using Eto.Mac.Forms.Controls;
using Eto.Drawing;
#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#endif

namespace Eto.Mac.Forms.Controls
{
	public interface ITextBoxWithMaxLength
	{
		int MaxLength { get; }
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
			if (Handler.MaxLength > 0)
			{
				int size = (int)value.Length;
				if (size > Handler.MaxLength)
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

	public class EtoTextField : NSTextField, IMacControl, ITextBoxWithMaxLength
	{
		public WeakReference WeakHandler { get; set; }

		IMacText TextHandler => WeakHandler.Target as IMacText;
		ITextBoxWithMaxLength MaxLengthHandler => WeakHandler.Target as ITextBoxWithMaxLength;

		public int MaxLength { get { return MaxLengthHandler?.MaxLength ?? 0; } }

		public EtoTextField()
		{
			Bezeled = true;
			Editable = true;
			Selectable = true;
			Formatter = new EtoFormatter { Handler = this };
			Cell.Scrollable = true;
			Cell.Wraps = false;
			Cell.UsesSingleLineMode = true;
		}

		[Export("textViewDidChangeSelection:")]
		public void TextViewDidChangeSelection(NSNotification notification)
		{
			if (TextHandler != null)
			{
				var textView = (NSTextView)notification.Object;
				TextHandler.SetLastSelection(textView.SelectedRange.ToEto());
			}
		}
	}


	public class TextBoxHandler : TextBoxHandler<TextBox, TextBox.ICallback>
	{
	}

	public class TextBoxHandler<TWidget, TCallback> : MacText<EtoTextField, TWidget, TCallback>, TextBox.IHandler, ITextBoxWithMaxLength
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
					SetCustomFieldEditor();
					AddMethod(selShouldChangeText, new Func<IntPtr, IntPtr, NSRange, IntPtr, bool>(TriggerShouldChangeText), "B@:{NSRange=QQ}@", CustomFieldEditor);
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

		static bool TriggerShouldChangeText(IntPtr sender, IntPtr sel, NSRange affectedCharRange, IntPtr replacementStringPtr)
		{
			var obj = Runtime.GetNSObject(sender);
			var handler = GetHandler(obj) as TextBoxHandler<TWidget, TCallback>;

			if (handler != null)
			{
				var replacementString = Messaging.GetNSObject<NSString>(replacementStringPtr);
				var args = new TextChangingEventArgs(replacementString, affectedCharRange.ToEto());
				handler.Callback.OnTextChanging(handler.Widget, args);
				if (args.Cancel)
					return false;
			}
			return Messaging.bool_objc_msgSendSuper_NSRange_IntPtr(obj.SuperHandle, sel, affectedCharRange, replacementStringPtr);
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
	}
}
