using System;
using MonoMac.AppKit;
using Eto.Forms;
using MonoMac.Foundation;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class TextCellHandler : CellHandler<NSTextFieldCell, TextCell>, ITextCell
	{
		public TextCellHandler ()
		{
			Control = new NSTextFieldCell ();
		}
		
		public override NSObject GetObjectValue (object val)
		{
			if (val == null) return new NSString();
			return base.GetObjectValue (val);
		}
		
		public override object SetObjectValue (NSObject val)
		{
			var str = val as NSString;
			if (str != null) return (string)str;
			return null;
		}
		
		public override float GetPreferredSize (object value, System.Drawing.SizeF cellSize)
		{
			var font = Control.Font ?? NSFont.SystemFontOfSize(NSFont.SystemFontSize);
			var str = new NSString(Convert.ToString (value));
			var attrs =  NSDictionary.FromObjectAndKey(font, NSAttributedString.FontAttributeName);
			return str.StringSize(attrs).Width + 4; // for border
			
		}
	}
}

