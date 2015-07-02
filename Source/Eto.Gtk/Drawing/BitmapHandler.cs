using System;
using System.IO;
using Eto.Drawing;
using System.Collections.Generic;

namespace Eto.GtkSharp.Drawing
{
	/// <summary>
	/// Bitmap data handler.
	/// </summary>
	/// <copyright>(c) 2012-2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class BitmapDataHandler : BitmapData
	{
		public BitmapDataHandler(Image image, IntPtr data, int scanWidth, int bitsPerPixel, object controlObject)
			: base(image, data, scanWidth, bitsPerPixel, controlObject)
		{
		}

		public override int TranslateArgbToData(int argb)
		{
			return unchecked((int)(((uint)argb & 0xFF00FF00) | (((uint)argb & 0xFF) << 16) | (((uint)argb & 0xFF0000) >> 16)));
		}

		public override int TranslateDataToArgb(int bitmapData)
		{
			return unchecked((int)(((uint)bitmapData & 0xFF00FF00) | (((uint)bitmapData & 0xFF) << 16) | (((uint)bitmapData & 0xFF0000) >> 16)));
		}
	}

	/// <summary>
	/// Bitmap handler.
	/// </summary>
	/// <copyright>(c) 2012-2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class BitmapHandler : ImageHandler<Gdk.Pixbuf, Bitmap>, Bitmap.IHandler, IGtkPixbuf
	{
		readonly Dictionary<Size, Gdk.Pixbuf> sizes = new Dictionary<Size, Gdk.Pixbuf>();

		public bool Alpha { get; set; }

		public BitmapHandler()
		{
		}

		public BitmapHandler(Gdk.Pixbuf pixbuf)
		{
			this.Control = pixbuf;
		}

		public void Create(string fileName)
		{
			Control = new Gdk.Pixbuf(fileName);
		}

		public void Create(Stream stream)
		{
			Control = new Gdk.Pixbuf(stream);
		}

		public void Create(int width, int height, PixelFormat pixelFormat)
		{
			switch (pixelFormat)
			{
				case PixelFormat.Format32bppRgba:
					Control = new Gdk.Pixbuf(Gdk.Colorspace.Rgb, true, 8, width, height);
					Control.Fill(0);
					Alpha = true;
					break;
				case PixelFormat.Format32bppRgb:
					Control = new Gdk.Pixbuf(Gdk.Colorspace.Rgb, true, 8, width, height);
					Control.Fill(0x000000FF);
					break;
				case PixelFormat.Format24bppRgb:
					Control = new Gdk.Pixbuf(Gdk.Colorspace.Rgb, false, 8, width, height);
					break;
			/*case PixelFormat.Format16bppRgb555:
						control = new Gdk.Pixbuf(Gdk.Colorspace.Rgb, false, 5, width, height);
						break;*/
				default:
					throw new NotSupportedException();
			}
		}

		public void Create(int width, int height, Graphics graphics)
		{
			Create(width, height, PixelFormat.Format32bppRgba);
		}

		public void Create(Image image, int width, int height, ImageInterpolation interpolation)
		{
			var pixbuf = image.ToGdk();
			Control = pixbuf.ScaleSimple(width, height, interpolation.ToGdk());
		}

		public BitmapData Lock()
		{
			return new BitmapDataHandler(Widget, Control.Pixels, Control.Rowstride, Control.HasAlpha ? 32 : 24, null);
		}

		public void Unlock(BitmapData bitmapData)
		{
			sizes.Clear();
		}

		public void Save(string fileName, ImageFormat format)
		{
			using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
			{
				Save(stream, format);
			}
		}

		public void Save(Stream stream, ImageFormat format)
		{
			string fileName = Guid.NewGuid().ToString();
			Control.Save(fileName, format.ToGdk());
			Stream fileStream = File.OpenRead(fileName);
			var buffer = new byte[4096];

			int size = fileStream.Read(buffer, 0, buffer.Length);
			while (size > 0)
			{
				stream.Write(buffer, 0, size);
				size = fileStream.Read(buffer, 0, buffer.Length);
			}
			fileStream.Close();
			File.Delete(fileName);
		}

		public override Size Size
		{
			get { return new Size(Control.Width, Control.Height); }
		}

		public override void SetImage(Gtk.Image imageView, Gtk.IconSize? iconSize)
		{
			if (iconSize != null)
				imageView.SetFromIconSet(new Gtk.IconSet(Control), iconSize.Value);
			else
				imageView.Pixbuf = Control;
		}

		public override void DrawImage(GraphicsHandler graphics, RectangleF source, RectangleF destination)
		{
			var context = graphics.Control;
			context.Save();
			destination.X += (float)graphics.InverseOffset;
			destination.Y += (float)graphics.InverseOffset;
			context.Rectangle(destination.ToCairo());
			double scalex = 1;
			double scaley = 1;
			if (Math.Abs(source.Width - destination.Width) > 0.5f || Math.Abs(source.Height - destination.Height) > 0.5f)
			{
				scalex = (double)destination.Width / (double)source.Width;
				scaley = (double)destination.Height / (double)source.Height;
				context.Scale(scalex, scaley);
			}
			Gdk.CairoHelper.SetSourcePixbuf(context, Control, (destination.Left / scalex) - source.Left, (destination.Top / scaley) - source.Top);
			var pattern = (Cairo.SurfacePattern)context.GetSource();
			pattern.Filter = graphics.ImageInterpolation.ToCairo();
			context.Fill();
			context.Restore();

			if (EtoEnvironment.Platform.IsMac)
				pattern.Dispose();

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

		public Gdk.Pixbuf Pixbuf
		{
			get
			{
				return Control;
			}
		}

		public Gdk.Pixbuf GetPixbuf(Size maxSize, Gdk.InterpType interpolation = Gdk.InterpType.Bilinear)
		{
			Gdk.Pixbuf pixbuf = Control;
			if (pixbuf.Width > maxSize.Width && pixbuf.Height > maxSize.Height)
			{
				if (!sizes.TryGetValue(maxSize, out pixbuf))
				{
					pixbuf = Control.ScaleSimple(maxSize.Width, maxSize.Height, interpolation);
					sizes[maxSize] = pixbuf;
				}
			}

			return pixbuf;
		}

		public Bitmap Clone(Rectangle? rectangle = null)
		{
			if (rectangle == null)
				return new Bitmap(new BitmapHandler(Control.Copy()));
			else
			{
				var rect = rectangle.Value;
				PixelFormat format;
				if (Control.BitsPerSample == 24)
					format = PixelFormat.Format24bppRgb;
				else if (Control.HasAlpha)
					format = PixelFormat.Format32bppRgba;
				else
					format = PixelFormat.Format32bppRgb;
				var bmp = new Bitmap(rect.Width, rect.Height, format);
				Control.CopyArea(rect.X, rect.Y, rect.Width, rect.Height, bmp.ToGdk(), 0, 0);
				return bmp;
			}
		}

		public Color GetPixel(int x, int y)
		{
			using (var data = Lock())
			{
				unsafe
				{
					var srcrow = (byte*)data.Data;
					srcrow += y * data.ScanWidth;
					srcrow += x * data.BytesPerPixel;
					if (data.BytesPerPixel == 4)
					{
						return Color.FromArgb(data.TranslateDataToArgb(*(int*)srcrow));
					}
					if (data.BytesPerPixel == 3)
					{
						var b = *(srcrow++);
						var g = *(srcrow++);
						var r = *(srcrow++);
						return Color.FromArgb(r, g, b);
					}
					throw new NotSupportedException();
				}
			}
		}
	}
}
