#define GTK_2_6
using System;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using Eto.Drawing;
using Eto.Shared.Drawing;

namespace Eto.GtkSharp.Drawing
{
	public class IndexedBitmapDataHandler : BaseBitmapData
	{
		public IndexedBitmapDataHandler (Image image, IntPtr data, int scanWidth, int bitsPerPixel, object controlObject)
			: base(image, data, scanWidth, bitsPerPixel, controlObject)
		{
		}

		public override int TranslateArgbToData (int argb)
		{
			return argb;
		}

		public override int TranslateDataToArgb (int bitmapData)
		{
			return bitmapData;
		}
	}

	public class IndexedBitmapHandler : ImageHandler<byte[], IndexedBitmap>, IndexedBitmap.IHandler
	{
		Size size;
		int rowStride;
		int bitsPerPixel;
		uint[] colors;

		public int RowStride {
			get { return rowStride; }
		}

		public override Size Size {
			get { return size; }
		}

		public void Create (int width, int height, int bitsPerPixel)
		{
			this.bitsPerPixel = bitsPerPixel;
			rowStride = width * bitsPerPixel / 8;
			int colorCount = (int)Math.Pow (2, bitsPerPixel);
			colors = new uint[colorCount];
			for (int i=0; i<colorCount; i++) {
				colors [i] = 0xffffffff;
			}

			size = new Size (width, height);
			Control = new byte[height * rowStride];
		}

		public void Resize (int width, int height)
		{
			throw new NotSupportedException ("Cannot resize an indexed image");
		}

		public BitmapData Lock ()
		{
			IntPtr ptr = Marshal.AllocHGlobal (Control.Length);
			Marshal.Copy (Control, 0, ptr, Control.Length);
			return  new IndexedBitmapDataHandler (Widget, ptr, rowStride, bitsPerPixel, null);
		}

		public void Unlock (BitmapData bitmapData)
		{
			IntPtr ptr = bitmapData.Data;
			Marshal.Copy (ptr, Control, 0, Control.Length);
			Marshal.FreeHGlobal (ptr);
		}

		public Palette Palette {
			get {
				return new Palette (colors.Select (r => Color.FromArgb(unchecked((int)r))).ToList ());
			}
			set {
				if (value.Count != colors.Length)
					throw new ArgumentException (string.Format(CultureInfo.CurrentCulture, "Input palette must have the same colors as the output"));
				for (int i=0; i<value.Count; i++) {
					colors [i] = (uint)value [i].ToArgb ();
				}
			}
		}

#if GTK2
		Gdk.RgbCmap GetPmap ()
		{
			return new Gdk.RgbCmap (colors);
		}

		public override void SetImage (Gtk.Image imageView, Gtk.IconSize? iconSize)
		{
			using (var drawable = new Gdk.Pixmap(null, Size.Width, Size.Height, 24))
			using (var gc = new Gdk.GC(drawable)) {
				drawable.Colormap = new Gdk.Colormap (Gdk.Visual.System, true);
	
				
				drawable.DrawIndexedImage (gc, 0, 0, Size.Width, Size.Height, Gdk.RgbDither.None, Control, rowStride, GetPmap ());

				if (iconSize != null) {
					var iconSet = new Gtk.IconSet(Gdk.Pixbuf.FromDrawable (drawable, Gdk.Colormap.System, 0, 0, 0, 0, size.Width, size.Height));
					imageView.SetFromIconSet(iconSet, iconSize.Value);
				}
				else
					imageView.Pixmap = drawable;
				
			}
		}
#else
		public override void SetImage (Gtk.Image imageView, Gtk.IconSize? iconSize)
		{
		}
#endif

		public override void DrawImage (GraphicsHandler graphics, RectangleF source, RectangleF destination)
		{
			// copy to a surface
			using (var surface = new Cairo.ImageSurface (Cairo.Format.Rgb24, (int)source.Width, (int)source.Height))
			{
				unsafe
				{
					var destrow = (byte*)surface.DataPtr;
					fixed (byte* srcdata = Control)
					{
						byte* srcrow = srcdata + ((int)source.Top * rowStride) + (int)source.Left;
						for (int y = (int)source.Top; y < (int)source.Bottom; y++)
						{
							var src = srcrow;
							var dest = (uint*)destrow;
							for (int x = (int)source.Left; x < (int)source.Right; x++)
							{
								*
							dest = colors[*src];
								src++;
								dest++;
							}

							srcrow += rowStride;
							destrow += surface.Stride;
						}
					}
				}

				var context = graphics.Control;
				context.Save();
				context.Rectangle(destination.ToCairo());
				double scalex = 1;
				double scaley = 1;
				if (Math.Abs(source.Width - destination.Width) > 0.5f || Math.Abs(source.Height - destination.Height) > 0.5f)
				{
					scalex = (double)destination.Width / (double)source.Width;
					scaley = (double)destination.Height / (double)source.Height;
					context.Scale(scalex, scaley);
				}
				context.SetSourceSurface(surface, (int)destination.Left, (int)destination.Top);
				context.Fill();
				context.Restore();
			}


			/*
			if (graphics == null || graphics.Control == null || graphics.GC == null) 
				throw new Exception("WHAA?");
			using (var drawable = new Gdk.Pixmap(graphics.Control, source.Right+1, source.Bottom+1))
			using (var gc = new Gdk.GC(drawable))
			{
				if (drawable.Colormap == null) 
					drawable.Colormap = graphics.Control.Colormap;
				drawable.DrawIndexedImage(gc, 0, 0, source.Right+1, source.Bottom+1, Gdk.RgbDither.None, Control, this.rowStride, GetPmap());
				if (source.Width != destination.Width || source.Height != destination.Height)
				{
					// scale da shit
					Gdk.Pixbuf pb = new Gdk.Pixbuf(Gdk.Colorspace.Rgb, true, 8, source.Width, source.Height);
					pb.GetFromDrawable(drawable, drawable.Colormap, source.X, source.Y, 0, 0, source.Width, source.Height);
	
					Gdk.Pixbuf pbDest = new Gdk.Pixbuf(Gdk.Colorspace.Rgb, true, 8, destination.Width, destination.Height);
					pb.Scale(pbDest, 0, 0, destination.Width, destination.Height, 0, 0, (double)destination.Width / (double)source.Width,
						(double)destination.Height / (double)source.Height, Gdk.InterpType.Bilinear);
					pb.Dispose();
	
	
					graphics.Control.DrawPixbuf(graphics.GC, pbDest, 0, 0, destination.X, destination.Y, destination.Width, destination.Height, Gdk.RgbDither.None, 0, 0);
					pbDest.Dispose();
				}
				else
				{
					// no scaling necessary!
					graphics.Control.DrawDrawable(graphics.GC, drawable, source.X, source.Y, destination.X, destination.Y, destination.Width, destination.Height);
				}
				
			}*/
		}
	}
}
