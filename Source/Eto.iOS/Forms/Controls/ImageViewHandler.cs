using System;
using MonoTouch.UIKit;
using Eto.Forms;
using Eto.Drawing;
using Eto.iOS.Drawing;

namespace Eto.iOS.Forms.Controls
{
	public class ImageViewHandler : IosControl<UIImageView, ImageView>, IImageView
	{
		Image image;

		public ImageViewHandler ()
		{
			Control = new UIImageView();
			Control.ContentMode = UIViewContentMode.ScaleAspectFit;
		}

		public Eto.Drawing.Image Image {
			get { return image; }
			set {
				image = value;
				Control.Image = value.ToUI ();
			}
		}
	}
}

