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
		
		public override NSObject GetObjectValue (object dataItem)
		{
			if (Widget.Binding != null) {
				var val = Widget.Binding.GetValue (dataItem);
				return val is string ? new NSString((string)val) : null;
			}
			else
				return new NSString ();
		}
		
		public override void SetObjectValue (object dataItem, NSObject val)
		{
			if (Widget.Binding != null) {
				var str = val as NSString;
				if (str != null)
					Widget.Binding.SetValue (dataItem, (string)str);
				else
					Widget.Binding.SetValue (dataItem, null);
			}
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

