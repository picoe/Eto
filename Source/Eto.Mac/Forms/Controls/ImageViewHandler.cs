using System;
using MonoMac.AppKit;
using Eto.Forms;
using Eto.Drawing;
using Eto.Mac.Drawing;

namespace Eto.Mac.Forms.Controls
{
	public class ImageViewHandler : MacControl<NSImageView, ImageView, ImageView.ICallback>, IImageView
	{
		Image image;
		
		public class EtoImageView : NSImageView, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public object Handler
			{ 
				get { return WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); } 
			}
		}
		
		public ImageViewHandler ()
		{
			Control = new EtoImageView { Handler = this, ImageScaling = NSImageScale.ProportionallyUpOrDown };
		}

		protected override SizeF GetNaturalSize (SizeF availableSize)
		{
			return image == null ? Size.Empty : image.Size;
		}
		
		public Image Image {
			get {
				return image;
			}
			set {
				image = value;
				Control.Image = image == null ? null : ((IImageSource)value.Handler).GetImage();
			}
		}
	}
}

