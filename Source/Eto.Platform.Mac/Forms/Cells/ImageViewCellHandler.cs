using System;
using MonoMac.AppKit;
using Eto.Forms;
using Eto.Drawing;
using Eto.Platform.Mac.Drawing;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class ImageViewCellHandler : CellHandler<NSImageCell, ImageViewCell>, IImageViewCell
	{
		public class EtoImageCell : NSImageCell, IMacControl
		{
			public object Handler { get; set; }
			
			public EtoImageCell ()
			{
			}
			
			public EtoImageCell (IntPtr handle) : base(handle)
			{
			}
			
			[Export("copyWithZone:")]
			NSObject CopyWithZone (IntPtr zone)
			{
				var ptr = Messaging.IntPtr_objc_msgSendSuper_IntPtr (SuperHandle, MacCommon.selCopyWithZone.Handle, zone);
				return new EtoImageCell (ptr) { Handler = this.Handler };
			}
		}
		
		public ImageViewCellHandler ()
		{
			Control = new EtoImageCell { Handler = this };
		}
		
		public override NSObject GetObjectValue (object dataItem)
		{
			if (Widget.Binding != null) {
				var img = Widget.Binding.GetValue (dataItem) as Image;
				if (img != null) {
					var imgHandler = ((IImageSource)img.Handler);
					return imgHandler.GetImage ();
				}
			}
			return new NSImage();
		}

		public override void SetObjectValue (object dataItem, NSObject val)
		{
		}
		
		public override float GetPreferredSize (object value, System.Drawing.SizeF cellSize)
		{
			var img = value as Image;
			if (img != null) {
				return cellSize.Height / (float)img.Size.Height * (float)img.Size.Width;
			}
			return 16;
		}
	}
}

