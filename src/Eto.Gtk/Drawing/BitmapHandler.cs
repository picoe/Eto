using System;
using System.IO;
using Eto.Drawing;
using System.Collections.Generic;
using Eto.Shared.Drawing;

namespace Eto.GtkSharp.Drawing
{
	/// <summary>
	/// Bitmap data handler.
	/// </summary>
	/// <copyright>(c) 2012-2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class BitmapDataHandler : BaseBitmapData
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

		public Cairo.ImageSurface Surface { get; set; }

		public bool Alpha { get; set; }

		bool _isAlphaDirty;
		bool _needsAlphaFixup;

		public BitmapHandler()
		{
		}

#if GTK2
		public BitmapHandler(Gdk.Image image)
		{
			Create(image.Width, image.Height, image.BitsPerPixel == 32 ? PixelFormat.Format32bppRgb : PixelFormat.Format24bppRgb);
			Control.GetFromImage(image, image.Colormap ?? Gdk.Colormap.System, 0, 0, 0, 0, image.Width, image.Height);
		}
#endif

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
					_needsAlphaFixup = true;
					break;
				case PixelFormat.Format24bppRgb:
					Control = new Gdk.Pixbuf(Gdk.Colorspace.Rgb, false, 8, width, height);
					Control.Fill(0);
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
			EnsureData();
			return InnerLock();
		}

		BitmapData InnerLock()
		{
			return new BitmapDataHandler(Widget, Control.Pixels, Control.Rowstride, Control.HasAlpha ? 32 : 24, null);
		}

		public void Unlock(BitmapData bitmapData)
		{
			sizes.Clear();
			SetAlphaDirty();
		}

		public void Save(string fileName, ImageFormat format)
		{
			EnsureData();
			using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
			{
				Save(stream, format);
			}
		}

		public void Save(Stream stream, ImageFormat format)
		{
			EnsureData();
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
			EnsureData();
			if (iconSize != null)
				imageView.SetFromIconSet(new Gtk.IconSet(Control), iconSize.Value);
			else
				imageView.Pixbuf = Control;
		}

		public void SetAlphaDirty() => _isAlphaDirty = _needsAlphaFixup;

		public override void DrawImage(GraphicsHandler graphics, RectangleF source, RectangleF destination)
		{
			FixupAlpha();
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
			Cairo.SurfacePattern pattern;
			if (Surface != null)
			{
				// we got a surface (by drawing on the bitmap using Graphics), so use that directly.
				pattern = new Cairo.SurfacePattern(Surface);
				var m = new Cairo.Matrix();
				m.InitTranslate(source.Left - (destination.Left / scalex), source.Top - (destination.Top / scaley));
				pattern.Matrix = m;
				context.SetSource(pattern);
			}
			else
			{
				Gdk.CairoHelper.SetSourcePixbuf(context, Control, (destination.Left / scalex) - source.Left, (destination.Top / scaley) - source.Top);
				pattern = (Cairo.SurfacePattern)context.GetSource();
			}

			pattern.Filter = graphics.ImageInterpolation.ToCairo();
			context.Fill();
			context.Restore();
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
				EnsureData();
				FixupAlpha();
				return Control;
			}
		}

		public Gdk.Pixbuf GetPixbuf(Size maxSize, Gdk.InterpType interpolation = Gdk.InterpType.Bilinear, bool shrink = false)
		{
			EnsureData();
			FixupAlpha();
			Gdk.Pixbuf pixbuf = Control;
			if (pixbuf.Width > maxSize.Width && pixbuf.Height > maxSize.Height
				|| (shrink && (maxSize.Width < pixbuf.Width || maxSize.Height < pixbuf.Height)))
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
			EnsureData();
			FixupAlpha();
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
						return Color.FromRgb(data.TranslateDataToArgb(*(int*)srcrow));
					}
					throw new NotSupportedException();
				}
			}
		}
		
		public unsafe void FixupAlpha()
		{
			if (_isAlphaDirty)
			{
				if (Surface != null)
				{
					var size = Size;
					var ptr = (byte*)Surface.DataPtr;
					ptr += 3; // alpha is the last byte of four
					for (int y = 0; y < size.Height; y++)
					{
						var rowptr = (byte*)ptr;
						for (int x = 0; x < size.Width; x++)
						{
							*rowptr = 255;
							rowptr += 4;
						}
						ptr += Surface.Stride;
					}
				}
				else
				{
					using (var bd = Lock())
					{
						var size = Size;
						var ptr = (byte*)bd.Data;
						ptr += 3; // alpha is the last byte of four
						for (int y = 0; y < size.Height; y++)
						{
							var rowptr = (byte*)ptr;
							for (int x = 0; x < size.Width; x++)
							{
								*rowptr = 255;
								rowptr += 4;
							}
							ptr += bd.ScanWidth;
						}
					}
				}
				_isAlphaDirty = false;
			}
		}

		unsafe void EnsureData()
		{
			if (Surface == null)
				return;
			using (var bd = InnerLock())
			{
				var size = Size;
				var srcrow = (byte*)Surface.DataPtr;
				if (bd.BytesPerPixel == 4 && _isAlphaDirty)
				{
					var destrow = (byte*)bd.Data;
					for (int y = 0; y < size.Height; y++)
					{
						var src = (int*)srcrow;
						var dest = (int*)destrow;
						for (int x = 0; x < size.Width; x++)
						{
							var argb = bd.TranslateArgbToData(*src);
							argb = unchecked(argb | (int)0xFF000000);
							*dest = argb;
							src++;
							dest++;
						}
						srcrow += Surface.Stride;
						destrow += bd.ScanWidth;
					}
					_isAlphaDirty = false;
				}
				if (bd.BytesPerPixel == 4)
				{
					var destrow = (byte*)bd.Data;
					for (int y = 0; y < size.Height; y++)
					{
						var src = (int*)srcrow;
						var dest = (int*)destrow;
						for (int x = 0; x < size.Width; x++)
						{
							*dest = bd.TranslateArgbToData(*src);
							src++;
							dest++;
						}
						srcrow += Surface.Stride;
						destrow += bd.ScanWidth;
					}
				}
				else if (bd.BytesPerPixel == 3)
				{
					var destrow = (byte*)bd.Data;
					for (int y = 0; y < size.Height; y++)
					{
						var src = (int*)srcrow;
						var dest = (byte*)destrow;
						for (int x = 0; x < size.Width; x++)
						{
							var data = bd.TranslateArgbToData(*src);
							*(dest++) = (byte)data;
							data >>= 8;
							*(dest++) = (byte)data;
							data >>= 8;
							*(dest++) = (byte)data;
							src++;
						}
						srcrow += Surface.Stride;
						destrow += bd.ScanWidth;
					}
				}
				else
				{
					for (int y = 0; y < size.Height; y++)
					{
						var src = (int*)srcrow;
						for (int x = 0; x < size.Width; x++)
						{
							bd.SetPixel(x, y, Color.FromArgb(*src));
							src++;
						}
						srcrow += Surface.Stride;
					}
				}
			}
			((IDisposable)Surface).Dispose();
			Surface = null;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (Surface != null)
				{
					((IDisposable)Surface).Dispose();
					Surface = null;
				}
			}
			base.Dispose(disposing);
		}
	}
}
