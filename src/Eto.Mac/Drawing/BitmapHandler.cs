using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Eto.Drawing;
using Eto.Mac.Forms;
using Eto.Shared.Drawing;
using System.Collections.Generic;
using Eto.Forms;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreImage;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
using MonoMac.CoreImage;
#if Mac64
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#if SDCOMPAT
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
#endif
#endif

namespace Eto.Mac.Drawing
{
	/// <summary>
	/// Bitmap data handler.
	/// </summary>
	/// <copyright>(c) 2012-2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class BitmapDataHandler : BaseBitmapData
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
		protected NSBitmapImageRep bmprep;
		bool alpha = true;

		public BitmapHandler()
		{
		}

		public BitmapHandler(NSImage image)
		{
			Control = image;
			rep = Control.BestRepresentationForDevice(null);
			bmprep = rep as NSBitmapImageRep;
		}

		public void Create(string fileName)
		{
			if (!File.Exists(fileName))
				throw new FileNotFoundException("Icon not found", fileName);
			Control = new NSImage(fileName);
			rep = Control.BestRepresentationForDevice(null);
			bmprep = rep as NSBitmapImageRep;
			Control.Size = new CGSize(rep.PixelsWide, rep.PixelsHigh);
		}

		public void Create(Stream stream)
		{
			Control = new NSImage(NSData.FromStream(stream));
			rep = Control.BestRepresentationForDevice(null);
			bmprep = rep as NSBitmapImageRep;
			Control.Size = new CGSize(rep.PixelsWide, rep.PixelsHigh);
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
					throw new ArgumentOutOfRangeException("pixelFormat", pixelFormat, string.Format(CultureInfo.CurrentCulture, "Not supported"));
			}
		}

		public void Create(int width, int height, Graphics graphics)
		{
			Create(width, height, PixelFormat.Format32bppRgba);
		}

		public void Create(Image image, int width, int height, ImageInterpolation interpolation)
		{
			var source = image.ToNS();
			Control = source.Resize(new CGSize(width, height), interpolation);
		}

		public override NSImage GetImage()
		{
			return Control;
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
			var sourceRect = new CGRect(source.X, (float)Control.Size.Height - source.Y - source.Height, source.Width, source.Height);
			var destRect = destination.ToNS();
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
				var rect = new CGRect(CGPoint.Empty, Control.Size);
				var image = new NSImage();
				var cgimage = Control.AsCGImage(ref rect, null, null).WithImageInRect(rectangle.Value.ToNS());
				image.AddRepresentation(new NSBitmapImageRep(cgimage));
				return new Bitmap(new BitmapHandler(image));
			}
		}

		public Color GetPixel(int x, int y)
		{
			EnsureRep();
			if (bmprep == null)
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Cannot get pixel data for this type of bitmap ({0})", rep?.GetType()));

			return bmprep.ColorAt(x, y).ToEto(false);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (rep != null)
				{
					rep.Dispose();
					rep = null;
				}
			}
			base.Dispose(disposing);
		}

		public BitmapData Lock()
		{
			EnsureRep();
			return bmprep == null ? null : new BitmapDataHandler(Widget, bmprep.BitmapData, (int)bmprep.BytesPerRow, (int)bmprep.BitsPerPixel, Control);
		}

		public void Unlock(BitmapData bitmapData)
		{
		}

		public NSBitmapImageRep GetBitmapImageRep()
		{
			EnsureRep();
			return bmprep;
		}


		protected void EnsureRep()
		{
			if (rep == null)
				rep = Control.BestRepresentationForDevice(null);
			if (bmprep != null)
				return;

			if (rep is IconFrameHandler.LazyImageRep lazyRep)
			{
				bmprep = lazyRep.Rep;
			}
			else
			{
				bmprep = rep as NSBitmapImageRep ?? Control.BestRepresentationForDevice(null) as NSBitmapImageRep;
			}

			if (bmprep == null)
			{
				Control.LockFocus();
				bmprep = new NSBitmapImageRep(new CGRect(CGPoint.Empty, Control.Size));
				//bmprep.Type.ColorSpaceName = Control.Representations()[0].ColorSpaceName;
				Control.UnlockFocus();
			}
		}
	}
}
