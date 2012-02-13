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
	}
}

