using System;
using Eto.Forms;
using MonoMac.AppKit;
using MonoMac.Foundation;
using Eto.Platform.Mac.Forms.Controls;
using MonoMac.ObjCRuntime;

namespace Eto.Platform.Mac
{
	public class MyFormatter : NSFormatter
	{
		public TextBoxHandler Handler { get; set; }
		
		public override string StringFor (NSObject value)
		{
			if (value == null)
				return string.Empty;
			var str = value as NSString;
			//if (str != null && value.Handle != IntPtr.Zero)
			return str.ToString ();
		}

		[Export("getObjectValue:forString:errorDescription:")]
		public bool GetObjectValue (ref IntPtr obj, IntPtr value, ref IntPtr error)
		{
			obj = value;
			return true;
		}
		
		[Export("isPartialStringValid:proposedSelectedRange:originalString:originalSelectedRange:errorDescription:")]
		public bool IsPartialStringValid (ref NSString value, IntPtr proposedSelRange, NSString origString, NSRange origSelRange, ref IntPtr error)
		{
			if (Handler.MaxLength >= 0) {
				int size = value.Length;
				if (size > Handler.MaxLength) {
					return false;
				}
			}
			return true;
		}
		
		[Export("attributedStringForObjectValue:withDefaultAttributes:")]
		public NSAttributedString AttributedStringForObjectValue (IntPtr anObject, NSDictionary attributes)
		{
			return null;
		}
	}
	
	public class TextBoxHandler : MacText<NSTextField, TextBox>, ITextBox
	{
		
		class EtoTextField : NSTextField, IMacControl
		{
			object IMacControl.Handler { get { return Handler; } }

			public TextBoxHandler Handler { get; set; }
			
			public override bool PerformKeyEquivalent (NSEvent theEvent)
			{
				if (Handler.Widget.HasFocus) {
					MacEventView.KeyDown (Handler.Widget, theEvent);
					return false;
					/*return base.PerformKeyEquivalent (theEvent);
					else
						return false;*/
				} else
					return false;
			}
			
		}
		
		public override bool HasFocus {
			get {
				return ((IMacWindow)Widget.ParentWindow.Handler).FieldEditorObject == Control;
			}
		}
		
		public TextBoxHandler ()
		{
			Control = new EtoTextField{ Handler = this };
			Control.Bezeled = true;
			Control.Editable = true;
			Control.Selectable = true;
			
			Control.Formatter = new MyFormatter{ Handler = this };
			//Control.BezelStyle = NSTextFieldBezelStyle.Square;
			//Control.Bordered = true;
			MaxLength = -1;
		}
		
		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case TextArea.TextChangedEvent:
				Control.Changed += delegate {
					Widget.OnTextChanged (EventArgs.Empty);
				};
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
		}
		
		public bool ReadOnly {
			get { return !Control.Editable; }
			set { Control.Editable = !value; }
		}
		
		public int MaxLength {
			get;
			set;
		}
	}
}
