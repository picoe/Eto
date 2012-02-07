using System;
using MonoMac.AppKit;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.Mac
{
	public class ImageViewHandler : MacControl<NSImageView, ImageView>, IImageView
	{
		IImage image;
		
		public ImageViewHandler ()
		{
			Control = new NSImageView ();
		}
		
		protected override Size GetNaturalSize ()
		{
			if (image != null)
				return image.Size;
			else
				return Size.Empty;
		}
		
		public IImage Image {
			get {
				return image;
			}
			set {
				image = value;
				if (image != null)
					Control.Image = (NSImage)value.ControlObject;
				else
					Control.Image = null;
			}
		}
	}
}

