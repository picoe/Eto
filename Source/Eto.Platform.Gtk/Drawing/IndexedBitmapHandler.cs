
#define GTK_2_6
using System;
using System.IO;
using System.Runtime.InteropServices;
using Eto.Drawing;
using System.Linq;

namespace Eto.Platform.GtkSharp.Drawing
{
	public class IndexedBitmapDataHandler : BitmapData
	{
		public IndexedBitmapDataHandler (IntPtr data, int scanWidth, object controlObject)
			: base(data, scanWidth, controlObject)
		{
		}

		public override uint TranslateArgbToData (uint argb)
		{
			return argb;
		}

		public override uint TranslateDataToArgb (uint bitmapData)
		{
			return bitmapData;
		}
	}

	public class IndexedBitmapHandler : ImageHandler<byte[], IndexedBitmap>, IIndexedBitmap
	{
		Size size;
		int rowStride;
		uint[] colors;

		public int RowStride {
			get { return rowStride; }
		}

		public override Size Size {
			get { return size; }
		}

		#region IIndexedBitmap Members

		public void Create (int width, int height, int bitsPerPixel)
		{
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
			return  new IndexedBitmapDataHandler (ptr, rowStride, null);
		}

		public void Unlock (BitmapData bitmapData)
		{
			IntPtr ptr = bitmapData.Data;
			Marshal.Copy (ptr, Control, 0, Control.Length);
			Marshal.FreeHGlobal (ptr);
		}

		public Palette Palette {
			get {
				Palette pal = new Palette (colors.Length);
				pal.AddRange (colors.Select (r => Color.FromArgb ((uint)r)));
				return pal;
			}
			set {
				if (value.Count != colors.Length)
					throw new ArgumentException ("Input palette must have the same colors as the output");
				for (int i=0; i<value.Count; i++) {
					colors [i] = value [i].ToArgb ();
				}
			}
		}

		#endregion

		private Gdk.RgbCmap GetPmap ()
		{
			return new Gdk.RgbCmap (colors);
		}

		public override void SetImage (Gtk.Image imageView)
		{
			using (var drawable = new Gdk.Pixmap(null, Size.Width, Size.Height, 24))
			using (var gc = new Gdk.GC(drawable)) {
				drawable.Colormap = new Gdk.Colormap (Gdk.Visual.System, true);
	
				
				drawable.DrawIndexedImage (gc, 0, 0, Size.Width, Size.Height, Gdk.RgbDither.None, Control, this.rowStride, GetPmap ());
				imageView.Pixmap = drawable;
				
			}
		}

		public override void DrawImage (GraphicsHandler graphics, Rectangle source, Rectangle destination)
		{
			// copy to a surface
			var surface = new Cairo.ImageSurface (Cairo.Format.Rgb24, source.Width, source.Height);
			unsafe {
				byte* destrow = (byte*)surface.DataPtr;
				fixed (byte* srcdata = this.Control) {
					byte* srcrow = srcdata + (source.Top * rowStride) + source.Left;
					for (int y = source.Top; y <= source.Bottom; y++) {
						byte* src = (byte*)srcrow;
						uint* dest = (uint*)destrow;
						for (int x = source.Left; x <= source.Right; x++) {
							*
							dest = colors [*src];
							src++;
							dest++;
						}
						
						srcrow += rowStride;
						destrow += surface.Stride;
					}
				}
			}
			
			var context = graphics.Control;
			context.Save ();
			context.Rectangle (Generator.ConvertC (destination));
			double scalex = 1;
			double scaley = 1;
			if (source.Width != destination.Width || source.Height != destination.Height) {
				scalex = (double)destination.Width / (double)source.Width;
				scaley = (double)destination.Height / (double)source.Height;
				context.Scale (scalex, scaley);
			}
			context.SetSourceSurface (surface, destination.Left, destination.Top);
			context.Fill ();
			context.Restore ();
			
			
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
