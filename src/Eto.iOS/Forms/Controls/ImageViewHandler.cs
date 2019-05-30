using System;
using UIKit;
using Eto.Forms;
using Eto.Drawing;
using Eto.iOS.Drawing;

namespace Eto.iOS.Forms.Controls
{
	public class ImageViewHandler : IosView<UIImageView, ImageView, ImageView.ICallback>, ImageView.IHandler
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

