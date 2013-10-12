using System;
using System.IO;
using System.Linq;
using Eto.Drawing;
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using sd = System.Drawing;
using MonoMac.ImageIO;

namespace Eto.Platform.Mac.Drawing
{
	/// <summary>
	/// Bitmap data handler.
	/// </summary>
	/// <copyright>(c) 2012-2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class BitmapDataHandler : BitmapData
	{
		public BitmapDataHandler(Bitmap bitmap, IntPtr data, int scanWidth, int bitsPerPixel, object controlObject)
			: base(bitmap, data, scanWidth, bitsPerPixel, controlObject)
		{
		}

		public static uint ArgbToData(uint argb)
		{
			return (argb & 0xFF00FF00) | ((argb & 0xFF) << 16) | ((argb & 0xFF0000) >> 16);
		}

		public static uint DataToArgb(uint bitmapData)
		{
			return (bitmapData & 0xFF00FF00) | ((bitmapData & 0xFF) << 16) | ((bitmapData & 0xFF0000) >> 16);
		}

		public override uint TranslateArgbToData(uint argb)
		{
			return (argb & 0xFF00FF00) | ((argb & 0xFF) << 16) | ((argb & 0xFF0000) >> 16);
		}

		public override uint TranslateDataToArgb(uint bitmapData)
		{
			return (bitmapData & 0xFF00FF00) | ((bitmapData & 0xFF) << 16) | ((bitmapData & 0xFF0000) >> 16);
		}

		public override bool Flipped { get { return false; } }
	}

	/// <summary>
	/// Bitmap handler.
	/// </summary>
	/// <copyright>(c) 2012-2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class BitmapHandler : ImageHandler<NSImage, Bitmap>, IBitmap
	{
		NSImageRep rep;
		NSBitmapImageRep bmprep;
		bool alpha = true;

		public BitmapHandler()
		{
		}

		public BitmapHandler(NSImage image)
		{
			Control = image;
		}

		public void Create(string fileName)
		{
			if (!File.Exists(fileName))
				throw new FileNotFoundException("Icon not found", fileName);
			Control = new NSImage(fileName);
			rep = Control.BestRepresentationForDevice(null);
			bmprep = rep as NSBitmapImageRep;
			Control.Size = new sd.SizeF(rep.PixelsWide, rep.PixelsHigh);
		}

		public void Create(Stream stream)
		{
			Control = new NSImage(NSData.FromStream(stream));
			rep = Control.BestRepresentationForDevice(null);
			bmprep = rep as NSBitmapImageRep;
			Control.Size = new sd.SizeF(rep.PixelsWide, rep.PixelsHigh);
		}

		public void Create(int width, int height, PixelFormat pixelFormat)
		{			
			switch (pixelFormat)
			{
				case PixelFormat.Format32bppRgb:
					{
						alpha = false;
						const int numComponents = 4;
						const int bitsPerComponent = 8;
						const int bitsPerPixel = numComponents * bitsPerComponent;
						const int bytesPerPixel = bitsPerPixel / 8;
						int bytesPerRow = bytesPerPixel * width;

						rep = bmprep = new NSBitmapImageRep(IntPtr.Zero, width, height, bitsPerComponent, 3, false, false, NSColorSpace.DeviceRGB, bytesPerRow, bitsPerPixel);
						Control = new NSImage();
						Control.AddRepresentation(rep);
						break;
					}
				case PixelFormat.Format24bppRgb:
					{
						alpha = false;
						const int numComponents = 3;
						const int bitsPerComponent = 8;
						const int bitsPerPixel = numComponents * bitsPerComponent;
						const int bytesPerPixel = bitsPerPixel / 8;
						int bytesPerRow = bytesPerPixel * width;
				
						rep = bmprep = new NSBitmapImageRep(IntPtr.Zero, width, height, bitsPerComponent, numComponents, false, false, NSColorSpace.DeviceRGB, bytesPerRow, bitsPerPixel);
						Control = new NSImage();
						Control.AddRepresentation(rep);
						break;
					}
				case PixelFormat.Format32bppRgba:
					{
						alpha = true;
						const int numComponents = 4;
						const int bitsPerComponent = 8;
						const int bitsPerPixel = numComponents * bitsPerComponent;
						const int bytesPerPixel = bitsPerPixel / 8;
						int bytesPerRow = bytesPerPixel * width;

						rep = bmprep = new NSBitmapImageRep(IntPtr.Zero, width, height, bitsPerComponent, numComponents, true, false, NSColorSpace.DeviceRGB, bytesPerRow, bitsPerPixel);
						Control = new NSImage();
						Control.AddRepresentation(rep);
						break;
					}
			/*case PixelFormat.Format16bppRgb555:
					control = new Gdk.Pixbuf(Gdk.Colorspace.Rgb, false, 5, width, height);
					break;*/
				default:
					throw new ArgumentOutOfRangeException("pixelFormat", pixelFormat, "Not supported");
			}
		}

		public void Create(int width, int height, Graphics graphics)
		{
			Create(width, height, PixelFormat.Format32bppRgba);
		}

		public void Create(Image image, int width, int height, ImageInterpolation interpolation)
		{
			var source = image.ToNS();
			Control = source.Resize(new sd.Size(width, height), interpolation);
		}

		public override NSImage GetImage()
		{
			return Control;
		}

		public BitmapData Lock()
		{
			if (bmprep != null)
				return new BitmapDataHandler(Widget, bmprep.BitmapData, bmprep.BytesPerRow, bmprep.BitsPerPixel, Control);
			else
				return null;
		}

		public void Unlock(BitmapData bitmapData)
		{
		}

		public void Save(Stream stream, ImageFormat format)
		{
			NSBitmapImageFileType type;
			switch (format)
			{
				case ImageFormat.Bitmap:
					type = NSBitmapImageFileType.Bmp;
					break;
				case ImageFormat.Gif:
					type = NSBitmapImageFileType.Gif;
					break;
				case ImageFormat.Jpeg:
					type = NSBitmapImageFileType.Jpeg;
					break;
				case ImageFormat.Png:
					type = NSBitmapImageFileType.Png;
					break;
				case ImageFormat.Tiff:
					type = NSBitmapImageFileType.Tiff;
					break;
				default:
					throw new NotSupportedException();
			}
			var reps = Control.Representations();
			if (reps == null)
				throw new InvalidDataException();
			var newrep = reps.OfType<NSBitmapImageRep>().FirstOrDefault();
			if (newrep == null)
			{
				CGImage img;
				if (this.bmprep != null)
					img = this.bmprep.CGImage;
				else
					img = Control.CGImage;
				newrep = new NSBitmapImageRep(img);
			}
			var data = newrep.RepresentationUsingTypeProperties(type, new NSDictionary());
			var datastream = data.AsStream();
			datastream.CopyTo(stream);
			stream.Flush();
			datastream.Dispose();
		}

		public override Size Size
		{
			get
			{
				/*
				NSImageRep rep = this.rep;
				if (rep == null)
					rep = Control.BestRepresentationForDevice (null);
				if (rep != null)
					return new Size(rep.PixelsWide, rep.PixelsHigh);
				else
				*/
				return Control.Size.ToEtoSize();
			}
		}

		public override void DrawImage(GraphicsHandler graphics, RectangleF source, RectangleF destination)
		{
			var sourceRect = graphics.Translate(source.ToSD(), Control.Size.Height);
			var destRect = graphics.TranslateView(destination.ToSD(), true, true);
			graphics.FlipDrawing();
			destRect.Y = graphics.ViewHeight - destRect.Y - destRect.Height;
			if (alpha)
				Control.Draw(destRect, sourceRect, NSCompositingOperation.SourceOver, 1);
			else
				Control.Draw(destRect, sourceRect, NSCompositingOperation.Copy, 1);
		}

		public Bitmap Clone(Rectangle? rectangle = null)
		{
			if (rectangle == null)
				return new Bitmap(Generator, new BitmapHandler((NSImage)Control.Copy()));
			else
			{
				var rect = rectangle.Value;
				PixelFormat format;
				if (bmprep != null && bmprep.BitsPerPixel == 24)
					format = PixelFormat.Format24bppRgb;
				else if (alpha || (bmprep != null && bmprep.HasAlpha))
					format = PixelFormat.Format32bppRgba;
				else
					format = PixelFormat.Format32bppRgb;

				var bmp = new Bitmap(rect.Width, rect.Height, format, Generator);
				using (var graphics = new Graphics (Generator, bmp))
				{
					graphics.DrawImage(Widget, rect, new Rectangle(rect.Size));
				}
				return bmp;
			}
		}

		public Color GetPixel(int x, int y)
		{
			if (bmprep == null)
				throw new InvalidOperationException(string.Format("Cannot get pixel data for this type of bitmap ({0})", rep.GetType()));

			return bmprep.ColorAt(x, y).ToEto();
		}
	}
}
