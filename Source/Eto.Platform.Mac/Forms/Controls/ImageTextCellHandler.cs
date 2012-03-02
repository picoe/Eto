using System;
using MonoMac.AppKit;
using Eto.Forms;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
using Eto.Drawing;
using Eto.Platform.Mac.Drawing;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class ImageTextCellHandler : CellHandler<NSTextFieldCell, ImageTextCell>, IImageTextCell
	{
		public class EtoCell : MacImageListItemCell, IMacControl
		{
			public object Handler { get; set; }
			
			public EtoCell ()
			{
			}
			
			public EtoCell (IntPtr handle) : base(handle)
			{
			}
			
			[Export("copyWithZone:")]
			NSObject CopyWithZone (IntPtr zone)
			{
				var ptr = Messaging.IntPtr_objc_msgSendSuper_IntPtr (SuperHandle, MacCommon.selCopyWithZone.Handle, zone);
				return new EtoCell (ptr) { Handler = this.Handler };
			}
		}
		
		public ImageTextCellHandler ()
		{
			Control = new EtoCell { Handler = this };
		}
		
		public override NSObject GetObjectValue (object dataItem)
		{
			var result = new MacImageData();
			if (Widget.TextBinding != null) {
				result.Text = (NSString)Convert.ToString (Widget.TextBinding.GetValue (dataItem));
			}
			if (Widget.ImageBinding != null) {
				var image = Widget.ImageBinding.GetValue (dataItem) as Image;
				result.Image = image != null ? ((IImageSource)image.Handler).GetImage () : null;
			}
			else result.Image = new NSImage();
			return result;
		}
		
		public override void SetObjectValue (object dataItem, NSObject val)
		{
			if (Widget.TextBinding != null) {
				var str = val as NSString;
				if (str != null)
					Widget.TextBinding.SetValue (dataItem, (string)str);
				else
					Widget.TextBinding.SetValue (dataItem, null);
			}
		}
		
		public override float GetPreferredSize (object value, System.Drawing.SizeF cellSize)
		{
			var font = Control.Font ?? NSFont.SystemFontOfSize (NSFont.SystemFontSize);
			var str = new NSString (Convert.ToString (value));
			var attrs = NSDictionary.FromObjectAndKey (font, NSAttributedString.FontAttributeName);
			return str.StringSize (attrs).Width + 4 + 16 + MacImageListItemCell.IMAGE_PADDING * 2; // for border + image
			
		}
	}
}

