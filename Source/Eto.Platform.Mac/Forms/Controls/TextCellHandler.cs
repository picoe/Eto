using System;
using MonoMac.AppKit;
using Eto.Forms;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class TextCellHandler : CellHandler<NSTextFieldCell, TextCell>, ITextCell
	{
		public class EtoTextFieldCell : NSTextFieldCell, IMacControl
		{
			public object Handler { get; set; }
			
			public EtoTextFieldCell ()
			{
			}
			
			public EtoTextFieldCell (IntPtr handle) : base(handle)
			{
			}
			
			[Export("copyWithZone:")]
			NSObject CopyWithZone (IntPtr zone)
			{
				var ptr = Messaging.IntPtr_objc_msgSendSuper_IntPtr (SuperHandle, MacCommon.selCopyWithZone.Handle, zone);
				return new EtoTextFieldCell (ptr) { Handler = this.Handler };
			}
		}
		
		public TextCellHandler ()
		{
			Control = new EtoTextFieldCell { Handler = this };
		}
		
		public override NSObject GetObjectValue (object val)
		{
			if (val == null)
				return new NSString ();
			return base.GetObjectValue (val);
		}
		
		public override object SetObjectValue (NSObject val)
		{
			var str = val as NSString;
			if (str != null)
				return (string)str;
			return null;
		}
		
		public override float GetPreferredSize (object value, System.Drawing.SizeF cellSize)
		{
			var font = Control.Font ?? NSFont.SystemFontOfSize (NSFont.SystemFontSize);
			var str = new NSString (Convert.ToString (value));
			var attrs = NSDictionary.FromObjectAndKey (font, NSAttributedString.FontAttributeName);
			return str.StringSize (attrs).Width + 4; // for border
			
		}
	}
}

