using System;
using System.IO;
using Eto.Drawing;
using System.Collections.Generic;

namespace Eto.Platform.GtkSharp.Drawing
{

	public class IconHandler : ImageHandler<Gtk.IconSet, Icon>, IIcon, IGtkPixbuf
	{
		Dictionary<Size, Gdk.Pixbuf> sizes = new Dictionary<Size, Gdk.Pixbuf>();
		
		public Gdk.Pixbuf Pixbuf { get; set; }

		#region IIcon Members
	
		public void Create(Stream stream)
		{
			Pixbuf = new Gdk.Pixbuf(stream);
			Control = new Gtk.IconSet(Pixbuf);
		}
		
		public void Create (string fileName)
		{
			Pixbuf = new Gdk.Pixbuf(fileName);
			Control = new Gtk.IconSet(Pixbuf);
		}

		#endregion
		
		public override Size Size {
			get {
				return new Size(Pixbuf.Width, Pixbuf.Height);
			}
		}
		
		public override void SetImage (Gtk.Image imageView, Gtk.IconSize? iconSize)
		{
			if (iconSize != null)
				imageView.SetFromIconSet(Control, iconSize.Value);
			else
				imageView.Pixbuf = Pixbuf;
		}
		
		public override void DrawImage (GraphicsHandler graphics, RectangleF source, RectangleF destination)
		{
			var context = graphics.Control;
			context.Save ();
			destination.X += (float)graphics.InverseOffset;
			destination.Y += (float)graphics.InverseOffset;
			context.Rectangle (destination.ToCairo ());
			double scalex = 1;
			double scaley = 1;
			if (source.Width != destination.Width || source.Height != destination.Height) {
				scalex = (double)destination.Width / (double)source.Width;
				scaley = (double)destination.Height / (double)source.Height;
				context.Scale (scalex, scaley);
			}
			Gdk.CairoHelper.SetSourcePixbuf (context, Pixbuf, (destination.Left / scalex) - source.Left, (destination.Top / scaley) - source.Top);
			var pattern = context.Source as Cairo.SurfacePattern;
			pattern.Filter = graphics.ImageInterpolation.ToCairo();
			context.Fill();
			context.Restore ();
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			if (disposing) {
				if (Pixbuf != null) { Pixbuf.Dispose(); Pixbuf = null; }
			}
		}
		
		public Gdk.Pixbuf GetPixbuf (Size maxSize)
		{
			Gdk.Pixbuf pixbuf = Pixbuf;
			if (pixbuf.Width > maxSize.Width && pixbuf.Height > maxSize.Height) {
				if (!sizes.TryGetValue (maxSize, out pixbuf)) {
					pixbuf = Pixbuf.ScaleSimple (maxSize.Width, maxSize.Height, Gdk.InterpType.Bilinear);
					sizes[maxSize] = pixbuf;
				}
			}
			
			return pixbuf;
		}
	}
}
