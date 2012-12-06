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
		Dictionary<Size, Gdk.Pixbuf> sizes = new Dictionary<Size, Gdk.Pixbuf> ();

		public bool Alpha { get; set; }
		
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
				Control.Fill(0);
				Alpha = true;
				break;
			case PixelFormat.Format32bppRgb:
				Control = new Gdk.Pixbuf (Gdk.Colorspace.Rgb, true, 8, width, height);
				Control.Fill(0x000000FF);
				break;
			case PixelFormat.Format24bppRgb:
				Control = new Gdk.Pixbuf (Gdk.Colorspace.Rgb, false, 8, width, height);
				break;
			/*case PixelFormat.Format16bppRgb555:
					control = new Gdk.Pixbuf(Gdk.Colorspace.Rgb, false, 5, width, height);
					break;*/
			default:
				throw new NotSupportedException ();
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
			Control.Save (fileName, format.ToGdk ());
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

		public override void SetImage (Gtk.Image imageView)
		{
			imageView.Pixbuf = Control;
			/*
			Gdk.Pixmap pix;
			Gdk.Pixmap mask;
			Control.RenderPixmapAndMask (out pix, out mask, 0);
			imageView.Pixmap = pix;
			imageView.Mask = mask;
			 * */
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
			Gdk.CairoHelper.SetSourcePixbuf (context, Control, (destination.Left / scalex) - source.Left, (destination.Top / scaley) - source.Top);
			var pattern = context.Source as Cairo.SurfacePattern;
			pattern.Filter = graphics.ImageInterpolation.ToCairo ();
			context.Fill ();
			context.Restore ();
			
			/*
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
			 *
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
			/*if (pb != Control)
				pb.Dispose ();*/
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
					sizes [maxSize] = pixbuf;
				}
			}
			
			return pixbuf;
		}
		#endregion

        #region IBitmap Members


        public void Create(int width, int height, Graphics graphics)
        {
            throw new NotImplementedException();
        }

        public void Create(Size size, PixelFormat pixelFormat)
        {
            throw new NotImplementedException();
        }

        public IBitmap Clone()
        {
            throw new NotImplementedException();
        }

        public Color GetPixel(int x, int y)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
