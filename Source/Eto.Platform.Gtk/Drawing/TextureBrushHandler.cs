using System;
using Eto.Drawing;

namespace Eto.Platform.GtkSharp.Drawing
{
	public class TextureBrushHandler : BrushHandler, ITextureBrushHandler, ITextureBrush
	{
		Image image;
		Gdk.Pixbuf pixbuf;

		public void Create (Image image)
		{
			this.image = image;
		}

		public Image Image
		{
			get { return image; }
		}

		public IMatrix Transform
		{
			get; set;
		}

		public override void Apply (GraphicsHandler graphics)
		{
			if (pixbuf == null)
				pixbuf = Image.ToGdk ();

			if (Transform != null)
				graphics.Control.Transform (Transform.ToCairo ());

			Gdk.CairoHelper.SetSourcePixbuf (graphics.Control, pixbuf, 0, 0);
			var pattern = graphics.Control.Source as Cairo.SurfacePattern;
			if (pattern != null) pattern.Extend = Cairo.Extend.Repeat;

		}
	}
}

