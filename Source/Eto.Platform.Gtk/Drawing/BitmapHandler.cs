using System;
using System.IO;
using Eto.Drawing;
using System.Collections.Generic;

namespace Eto.Platform.GtkSharp.Drawing
{
	public class BitmapDataHandler : BitmapData
	{
		public BitmapDataHandler (IntPtr data, int scanWidth, object controlObject)
			: base(data, scanWidth, controlObject)
		{
		}

		public override uint TranslateArgbToData (uint argb)
		{
			return (argb & 0xFF00FF00) | ((argb & 0xFF) << 16) | ((argb & 0xFF0000) >> 16);
		}

		public override uint TranslateDataToArgb (uint bitmapData)
		{
			return (bitmapData & 0xFF00FF00) | ((bitmapData & 0xFF) << 16) | ((bitmapData & 0xFF0000) >> 16);
		}
	}

	public class BitmapHandler : ImageHandler<Gdk.Pixbuf, Bitmap>, IBitmap, IGtkPixbuf
	{
		Dictionary<Size, Gdk.Pixbuf> sizes = new Dictionary<Size, Gdk.Pixbuf>();
		bool alpha;
		
		public BitmapHandler ()
		{
		}

		public BitmapHandler (Gdk.Pixbuf pixbuf)
		{
			this.Control = pixbuf;
		}
		
		public void Create (string fileName)
		{
			Control = new Gdk.Pixbuf (fileName);
		}

		public void Create (Stream stream)
		{
			Control = new Gdk.Pixbuf (stream);
		}

		public void Create (int width, int height, PixelFormat pixelFormat)
		{
			switch (pixelFormat) {
				case PixelFormat.Format32bppRgba:
				Control = new Gdk.Pixbuf (Gdk.Colorspace.Rgb, true, 8, width, height);
				alpha = true;
					break;
			case PixelFormat.Format32bppRgb:
				Control = new Gdk.Pixbuf (Gdk.Colorspace.Rgb, true, 8, width, height);
				break;
			case PixelFormat.Format24bppRgb:
				Control = new Gdk.Pixbuf (Gdk.Colorspace.Rgb, false, 8, width, height);
				break;
			/*case PixelFormat.Format16bppRgb555:
					control = new Gdk.Pixbuf(Gdk.Colorspace.Rgb, false, 5, width, height);
					break;*/
			default:
				throw new ArgumentOutOfRangeException ("pixelFormat", pixelFormat, "Not supported");
			}
		}

		public void Resize (int width, int height)
		{
			Control = Control.ScaleSimple (width, height, Gdk.InterpType.Bilinear);
			sizes.Clear ();
		}

		public BitmapData Lock ()
		{
			return new BitmapDataHandler (Control.Pixels, Control.Rowstride, null);
		}

		public void Unlock (BitmapData bitmapData)
		{
			sizes.Clear ();
		}

		public void Save (Stream stream, ImageFormat format)
		{
			string fileName = Guid.NewGuid ().ToString ();
			Control.Save (fileName, Generator.Convert (format));
			Stream fileStream = File.OpenRead (fileName);
			byte[] buffer = new byte[4096];

			int size = fileStream.Read (buffer, 0, buffer.Length);
			while (size > 0) {
				stream.Write (buffer, 0, size);
				size = fileStream.Read (buffer, 0, buffer.Length);
			}
			fileStream.Close ();
			File.Delete (fileName);
		}

		public override Size Size {
			get { return new Size (Control.Width, Control.Height); }
		}

		public override void DrawImage (GraphicsHandler graphics, int x, int y)
		{
			graphics.Control.DrawPixbuf (graphics.GC, Control, 0, 0, x, y, Control.Width, Control.Height, Gdk.RgbDither.None, 0, 0);
		}

		public override void DrawImage (GraphicsHandler graphics, int x, int y, int width, int height)
		{
			if (width != Control.Width || height != Control.Height) {
				Gdk.Pixbuf pbDest = Control.ScaleSimple (width, height, Gdk.InterpType.Bilinear);
				pbDest.RenderToDrawable (graphics.Control, graphics.GC, 0, 0, x, y, pbDest.Width, pbDest.Height, Gdk.RgbDither.None, 0, 0);
				pbDest.Dispose ();
			} else
				Control.RenderToDrawable (graphics.Control, graphics.GC, 0, 0, x, y, width, height, Gdk.RgbDither.None, 0, 0);
		}
		
		public override void SetImage (Gtk.Image imageView)
		{
			imageView.Pixbuf = Control;
		}

		public override void DrawImage (GraphicsHandler graphics, Rectangle source, Rectangle destination)
		{
			Gdk.Pixbuf pb = Control;
			

			if (source.Width != destination.Width || source.Height != destination.Height) {
				Gdk.Pixbuf pbDest = new Gdk.Pixbuf (Gdk.Colorspace.Rgb, true, 8, destination.Width, destination.Height);
				double scalex = (double)(destination.Width) / (double)(source.Width);
				double scaley = (double)(destination.Height) / (double)(source.Height);
				pb.Scale (pbDest, 0, 0, destination.Width, destination.Height, -(source.X * scalex), -(source.Y * scaley),
					scalex, scaley, Gdk.InterpType.Bilinear);
				source.Location = new Point (0, 0);
				pb = pbDest;
			}
			/*
			 */
			pb.RenderToDrawable (graphics.Control, graphics.GC, source.X, source.Y, destination.X, destination.Y, destination.Width, destination.Height, Gdk.RgbDither.None, 0, 0);
			/*
			 *
			Gdk.Pixmap pm, mask;
			pb.RenderPixmapAndMask(out pm, out mask, 0);
			graphics.Drawable.DrawDrawable(graphics.GC, pm, source.X, source.Y, destination.X, destination.Y, destination.Width, destination.Height);
			pm.Dispose();
			mask.Dispose();
			/*
			 *
			graphics.Drawable.DrawPixbuf(null, pb, source.X, source.Y, destination.X, destination.Y, destination.Width, destination.Height, Gdk.RgbDither.None, 0, 0);
			/*
			 */
			if (pb != Control)
				pb.Dispose ();
		}

		#region IGtkPixbuf implementation
		public Gdk.Pixbuf Pixbuf {
			get {
				return Control;
			}
		}
		
		public Gdk.Pixbuf GetPixbuf (Size maxSize)
		{
			Gdk.Pixbuf pixbuf = Control;
			if (pixbuf.Width > maxSize.Width && pixbuf.Height > maxSize.Height) {
				if (!sizes.TryGetValue (maxSize, out pixbuf)) {
					pixbuf = Control.ScaleSimple (maxSize.Width, maxSize.Height, Gdk.InterpType.Bilinear);
					sizes[maxSize] = pixbuf;
				}
			}
			
			return pixbuf;
		}
		#endregion
	}
}
