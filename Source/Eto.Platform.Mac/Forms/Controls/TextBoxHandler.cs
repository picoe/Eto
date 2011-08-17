using System;
using Eto.Forms;
using MonoMac.AppKit;
using MonoMac.Foundation;

namespace Eto.Platform.Mac
{
	public class MyFormatter : NSFormatter
	{
		public TextBoxHandler Handler { get; set; }
		
		public override string StringFor (NSObject value)
		{
			if (value == null) return string.Empty;
			var str = value as NSString;
			//if (str != null && value.Handle != IntPtr.Zero)
				return str.ToString ();
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
			if (Handler.MaxLength >= 0) {
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
	
	public class TextBoxHandler : MacText<NSTextField, TextBox>, ITextBox
	{
		
		class MyDelegate : NSTextFieldDelegate
		{
			public TextBoxHandler Handler { get; set;}
			
			public override void Changed (NSNotification notification)
			{
				Handler.Widget.OnTextChanged(EventArgs.Empty);
			}
		}
		
		public TextBoxHandler()
		{
			Control = new NSTextField();
			Control.Bezeled = true;
			Control.Editable = true;
			Control.Delegate = new MyDelegate{ Handler = this };
			/*Control.Changed += delegate {
				Widget.OnTextChanged(EventArgs.Empty);
			};*/
			
			Control.Formatter = new MyFormatter{ Handler = this };
			//Control.BezelStyle = NSTextFieldBezelStyle.Square;
			//Control.Bordered = true;
			MaxLength = -1;
		}
		
		public bool ReadOnly
		{
			get { return Control.Enabled; }
			set { Control.Enabled = value; }
		}
		
		public int MaxLength {
			get; set;
		}
	}
}
