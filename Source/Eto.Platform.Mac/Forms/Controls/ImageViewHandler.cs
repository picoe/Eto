using System;
using MonoMac.AppKit;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.Mac
{
	public class ImageViewHandler : MacControl<NSImageView, ImageView>, IImageView
	{
		IImage image;
		
		public ImageViewHandler()
		{
			Control = new NSImageView();
		}
		
		#region IImageView implementation
		
		public IImage Image {
			get {
				return image;
			}
			set {
				image = value;
				Control.Image = (NSImage)value.ControlObject;
			}
		}
		
		#endregion
}
}

