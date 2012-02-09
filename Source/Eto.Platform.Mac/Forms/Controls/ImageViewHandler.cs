using System;
using MonoMac.AppKit;
using Eto.Forms;
using Eto.Drawing;
using Eto.Platform.Mac.Drawing;

namespace Eto.Platform.Mac
{
	public class ImageViewHandler : MacControl<NSImageView, ImageView>, IImageView
	{
		IImage image;
		
		public class EtoImageView : NSImageView, IMacControl
		{
			public object Handler { get; set; }
		}
		
		public ImageViewHandler ()
		{
			Control = new EtoImageView { Handler = this };
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
					Control.Image = ((IImageSource)value.Handler).GetImage ();
				else
					Control.Image = null;
			}
		}
	}
}

