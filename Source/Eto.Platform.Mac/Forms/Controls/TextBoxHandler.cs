using System;
using Eto.Forms;
using MonoMac.AppKit;
using MonoMac.Foundation;
using Eto.Platform.Mac.Forms.Controls;
using MonoMac.ObjCRuntime;
using Eto.Drawing;

namespace Eto.Platform.Mac.Forms.Controls
{
	public interface ITextBoxWithMaxLength
	{
		int MaxLength { get; set; }
	}

	public class EtoFormatter : NSFormatter
	{
		WeakReference handler;

		public ITextBoxWithMaxLength Handler { get { return (ITextBoxWithMaxLength)handler.Target; } set { handler = new WeakReference(value); } }

		public override string StringFor(NSObject value)
		{
			if (value == null)
				return string.Empty;
			var str = value as NSString;
			//if (str != null && value.Handle != IntPtr.Zero)
			return str.ToString();
		}

		[Export("getObjectValue:forString:errorDescription:")]
		public bool GetObjectValue(ref IntPtr obj, IntPtr value, ref IntPtr error)
		{
			obj = value;
			return true;
		}

		[Export("isPartialStringValid:proposedSelectedRange:originalString:originalSelectedRange:errorDescription:")]
		public bool IsPartialStringValid(ref NSString value, IntPtr proposedSelRange, NSString origString, NSRange origSelRange, ref IntPtr error)
		{
			if (Handler.MaxLength >= 0)
			{
				int size = value.Length;
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

	public class EtoTextField : NSTextField, IMacControl
	{
		public WeakReference WeakHandler { get; set; }

		public TextBoxHandler Handler
		{ 
			get { return (TextBoxHandler)WeakHandler.Target; }
			set { WeakHandler = new WeakReference(value); } 
		}
	}

	public class TextBoxHandler : MacText<EtoTextField, TextBox>, ITextBox, ITextBoxWithMaxLength
	{

		public override bool HasFocus
		{
			get
			{
				if (Widget.ParentWindow == null)
					return false;
				return ((IMacWindow)Widget.ParentWindow.Handler).FieldEditorObject == Control;
			}
		}

		public override EtoTextField CreateControl()
		{
			return new EtoTextField
			{
				Handler = this,
				Bezeled = true,
				Editable = true,
				Selectable = true,
				Formatter = new EtoFormatter{ Handler = this }
			};
		}

		protected override void Initialize()
		{
			base.Initialize();
			Control.Cell.Scrollable = true;
			Control.Cell.Wraps = false;
			Control.Cell.UsesSingleLineMode = true;

			MaxLength = -1;
		}

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			var size = base.GetNaturalSize(availableSize);
			size.Width = Math.Max(100, size.Height);
			return size;
		}

		public override void AttachEvent(string handler)
		{
			switch (handler)
			{
				case TextArea.TextChangedEvent:
					Control.Changed += HandleTextChanged;
					break;
				default:
					base.AttachEvent(handler);
					break;
			}
		}

		static void HandleTextChanged (object sender, EventArgs e)
		{
			var h = GetHandler(sender) as TextBoxHandler;
			h.Widget.OnTextChanged(EventArgs.Empty);
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

		public void SelectAll()
		{
			Control.SelectText(Control);
		}
	}
}
