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
		
		public override NSObject GetObjectValue (object val)
		{
			if (val == null)
				return null;
			var result = new MacImageData();
			var objVal = val as object[];
			if (objVal != null && objVal.Length >= 2) {
				var image = objVal[0] as Image;
				result.Image = image != null ? ((IImageSource)image.Handler).GetImage () : null;
				result.Text = (NSString)Convert.ToString (objVal[1]);
			}
			else {
				var listItem = val as IImageListItem;
				if (listItem != null) {
					var image = listItem.Image as Image;
					result.Image = image != null ? ((IImageSource)image.Handler).GetImage () : null;
					result.Text = (NSString)(listItem.Text ?? string.Empty);
				}
			}
			return result;
		}
		
		public override object SetObjectValue (NSObject val)
		{
			var data = val as MacImageData;
			if (data != null) {
				// grr.
			}
			return null;
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

