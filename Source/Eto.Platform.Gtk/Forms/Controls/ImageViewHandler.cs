using System;
using Eto.Forms;
using Eto.Platform.GtkSharp.Drawing;
using Eto.Drawing;

namespace Eto.Platform.GtkSharp
{
	public class ImageViewHandler : GtkControl<Gtk.Image, ImageView>, IImageView
	{
		IImage image;
		
		public ImageViewHandler ()
		{
			Control = new Gtk.Image();
		}
		
		public Eto.Drawing.IImage Image {
			get {
				return image;
			}
			set {
				image = value;
				if (image == null) Control.Clear();
				else {
					var handler = Image.Handler as IImageHandler;
					if (handler != null)
					{
						handler.SetImage(Control);
					}
				}
			}
		}
	}
}

