using System;
using System.IO;
using System.Linq;
using Eto.Drawing;
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using sd = System.Drawing;
using Eto.Mac.Forms;
#if Mac64
using CGFloat = System.Double;
using NSInteger = System.Int64;
using NSUInteger = System.UInt64;
#else
using NSSize = System.Drawing.SizeF;
using NSRect = System.Drawing.RectangleF;
using NSPoint = System.Drawing.PointF;
using CGFloat = System.Single;
using NSInteger = System.Int32;
using NSUInteger = System.UInt32;
#endif

namespace Eto.Mac.Drawing
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

		public static int ArgbToData(int argb)
		{
			return unchecked((int)(((uint)argb & 0xFF00FF00) | (((uint)argb & 0xFF) << 16) | (((uint)argb & 0xFF0000) >> 16)));
		}

		public static int DataToArgb(int bitmapData)
		{
			return unchecked((int)(((uint)bitmapData & 0xFF00FF00) | (((uint)bitmapData & 0xFF) << 16) | (((uint)bitmapData & 0xFF0000) >> 16)));
		}

		public override int TranslateArgbToData(int argb)
		{
			return unchecked((int)(((uint)argb & 0xFF00FF00) | (((uint)argb & 0xFF) << 16) | (((uint)argb & 0xFF0000) >> 16)));
		}

		public override int TranslateDataToArgb(int bitmapData)
		{
			return unchecked((int)(((uint)bitmapData & 0xFF00FF00) | (((uint)bitmapData & 0xFF) << 16) | (((uint)bitmapData & 0xFF0000) >> 16)));
		}

		public override bool Flipped { get { return false; } }
	}

	/// <summary>
	/// Bitmap handler.
	/// </summary>
	/// <copyright>(c) 2012-2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class BitmapHandler : ImageHandler<NSImage, Bitmap>, Bitmap.IHandler
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
			Control.Size = new NSSize(rep.PixelsWide, rep.PixelsHigh);
		}

		public void Create(Stream stream)
		{
			Control = new NSImage(NSData.FromStream(stream));
			rep = Control.BestRepresentationForDevice(null);
			bmprep = rep as NSBitmapImageRep;
			Control.Size = new NSSize(rep.PixelsWide, rep.PixelsHigh);
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
			Control = source.Resize(new NSSize(width, height), interpolation);
		}

		public override NSImage GetImage()
		{
			return Control;
		}

		public BitmapData Lock()
		{
			return bmprep == null ? null : new BitmapDataHandler(Widget, bmprep.BitmapData, (int)bmprep.BytesPerRow, (int)bmprep.BitsPerPixel, Control);
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
				img = bmprep != null ? bmprep.CGImage : Control.CGImage;
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
			var sourceRect = new NSRect(source.X, (float)Control.Size.Height - source.Y - source.Height, source.Width, source.Height);
			var destRect = graphics.TranslateView(destination.ToNS(), true, true);
			if (alpha)
				Control.Draw(destRect, sourceRect, NSCompositingOperation.SourceOver, 1, true, null);
			else
				Control.Draw(destRect, sourceRect, NSCompositingOperation.Copy, 1, true, null);
		}

		public Bitmap Clone(Rectangle? rectangle = null)
		{
			if (rectangle == null)
				return new Bitmap(new BitmapHandler((NSImage)Control.Copy()));
			else
			{
				var rect = new NSRect(new NSPoint(), Control.Size);
				var temp = Control.AsCGImage (ref rect, null, null).WithImageInRect (rectangle.Value.ToSDRectangleF());
				var image = new NSImage (temp, new NSSize(temp.Width, temp.Height));
				return new Bitmap(new BitmapHandler(image));
			}
		}

		public Color GetPixel(int x, int y)
		{
			if (bmprep == null)
				throw new InvalidOperationException(string.Format("Cannot get pixel data for this type of bitmap ({0})", rep.GetType()));

			return bmprep.ColorAt(x, y).ToEto();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (rep != null)
				{
					rep.SafeDispose();
					rep = null;
				}
			}
			base.Dispose(disposing);
		}
	}
}
