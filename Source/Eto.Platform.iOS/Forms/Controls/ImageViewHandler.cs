using System;
using MonoTouch.UIKit;
using Eto.Forms;
using Eto.Drawing;
using Eto.Platform.iOS.Drawing;

namespace Eto.Platform.iOS.Forms.Controls
{
	public class ImageViewHandler : iosControl<UIImageView, ImageView>, IImageView
	{
		Image image;

		public ImageViewHandler ()
		{
		}

		public override UIImageView CreateControl ()
		{
			return new UIImageView();
		}

		public Eto.Drawing.Image Image {
			get { return image; }
			set {
				image = value;
				Control.Image = value.ToUIImage ();
			}
		}
	}
}

