using System;
using System.IO;
using System.Linq;
using Eto.Drawing;
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using SD = System.Drawing;

namespace Eto.Platform.Mac.Drawing
{
	public class BitmapDataHandler : BitmapData
	{
		public BitmapDataHandler (IntPtr data, int scanWidth, object controlObject)
			: base(data, scanWidth, controlObject)
		{
		}

		public static uint ArgbToData (uint argb)
		{
			return (argb & 0xFF00FF00) | ((argb & 0xFF) << 16) | ((argb & 0xFF0000) >> 16);
		}
		
		public static uint DataToArgb (uint bitmapData)
		{
			return (bitmapData & 0xFF00FF00) | ((bitmapData & 0xFF) << 16) | ((bitmapData & 0xFF0000) >> 16);
		}
		
		public override uint TranslateArgbToData (uint argb)
		{
			return (argb & 0xFF00FF00) | ((argb & 0xFF) << 16) | ((argb & 0xFF0000) >> 16);
		}

		public override uint TranslateDataToArgb (uint bitmapData)
		{
			return (bitmapData & 0xFF00FF00) | ((bitmapData & 0xFF) << 16) | ((bitmapData & 0xFF0000) >> 16);
		}

		public override bool Flipped {
			get {
				return false;
			}
		}
	}

	public class BitmapHandler : ImageHandler<NSImage, Bitmap>, IBitmap
	{
		NSImageRep rep;
		NSBitmapImageRep bmprep;
		bool alpha = true;
		
		public BitmapHandler ()
		{
		}
		
		public BitmapHandler (NSImage image)
		{
			Control = image;
		}

		public void Create (string fileName)
		{
			if (!File.Exists (fileName))
				throw new FileNotFoundException ("Icon not found", fileName);
			Control = new NSImage (fileName);
			rep = Control.BestRepresentationForDevice(null);
			bmprep = rep as NSBitmapImageRep;
			Control.Size = new SD.SizeF(rep.PixelsWide, rep.PixelsHigh);
		}

		public void Create (Stream stream)
		{
			Control = new NSImage (NSData.FromStream (stream));
			rep = Control.BestRepresentationForDevice(null);
			bmprep = rep as NSBitmapImageRep;
			Control.Size = new SD.SizeF(rep.PixelsWide, rep.PixelsHigh);
		}

		public void Create (int width, int height, PixelFormat pixelFormat)
		{			
			switch (pixelFormat) {
			case PixelFormat.Format32bppRgb:
				{
					alpha = false;
					int numComponents = 4;
					int bitsPerComponent = 8;
					int bitsPerPixel = numComponents * bitsPerComponent;
					int bytesPerPixel = bitsPerPixel / 8;
					int bytesPerRow = bytesPerPixel * width;

					rep = bmprep = new NSBitmapImageRep (IntPtr.Zero, width, height, bitsPerComponent, 3, false, false, NSColorSpace.DeviceRGB, bytesPerRow, bitsPerPixel);
					Control = new NSImage ();
					Control.AddRepresentation (rep);
				
					//var provider = new CGDataProvider (data.Bytes, (int)data.Length);
					//var cgImage = new CGImage (width, height, bitsPerComponent, bitsPerPixel, bytesPerRow, CGColorSpace.CreateDeviceRGB (), CGBitmapFlags.ByteOrder32Little | CGBitmapFlags.PremultipliedFirst, provider, null, true, CGColorRenderingIntent.Default);
					//Control = new NSImage (cgImage, new System.Drawing.SizeF (width, height));
				
					break;
				}
			case PixelFormat.Format24bppRgb:
				{
					alpha = false;
					int numComponents = 3;
					int bitsPerComponent = 8;
					int bitsPerPixel = numComponents * bitsPerComponent;
					int bytesPerPixel = bitsPerPixel / 8;
					int bytesPerRow = bytesPerPixel * width;
				
					rep = bmprep = new NSBitmapImageRep (IntPtr.Zero, width, height, bitsPerComponent, numComponents, false, false, NSColorSpace.DeviceRGB, bytesPerRow, bitsPerPixel);
					Control = new NSImage ();
					Control.AddRepresentation (rep);

					//var provider = new CGDataProvider (data.ClassHandle);
					//var cgImage = new CGImage (width, height, bitsPerComponent, bitsPerPixel, bytesPerRow, CGColorSpace.CreateDeviceRGB (), CGBitmapFlags.ByteOrder32Little | CGBitmapFlags.PremultipliedFirst, provider, null, true, CGColorRenderingIntent.Default);
					//Control = new NSImage (cgImage, new System.Drawing.SizeF (width, height));
					break;
				}
			case PixelFormat.Format32bppRgba: {
					alpha = true;
					int numComponents = 4;
					int bitsPerComponent = 8;
					int bitsPerPixel = numComponents * bitsPerComponent;
					int bytesPerPixel = bitsPerPixel / 8;
					int bytesPerRow = bytesPerPixel * width;

					rep = bmprep = new NSBitmapImageRep (IntPtr.Zero, width, height, bitsPerComponent, numComponents, true, false, NSColorSpace.DeviceRGB, bytesPerRow, bitsPerPixel);
					Control = new NSImage ();
					Control.AddRepresentation (rep);

					//var provider = new CGDataProvider (data.Bytes, (int)data.Length);
					//var cgImage = new CGImage (width, height, bitsPerComponent, bitsPerPixel, bytesPerRow, CGColorSpace.CreateDeviceRGB (), CGBitmapFlags.ByteOrder32Little | CGBitmapFlags.PremultipliedFirst, provider, null, true, CGColorRenderingIntent.Default);
					//Control = new NSImage (cgImage, new System.Drawing.SizeF (width, height));

					break;
				}
			/*case PixelFormat.Format16bppRgb555:
					control = new Gdk.Pixbuf(Gdk.Colorspace.Rgb, false, 5, width, height);
					break;*/
			default:
				throw new ArgumentOutOfRangeException ("pixelFormat", pixelFormat, "Not supported");
			}
		}

		public void Resize (int width, int height)
		{
			//control = control.ScaleSimple (width, height, Gdk.InterpType.Bilinear);
		}
		
		public override NSImage GetImage ()
		{
			return Control;
		}

		public BitmapData Lock ()
		{
			//Control.LockFocus();
			if (bmprep != null)
				return new BitmapDataHandler (bmprep.BitmapData, bmprep.BytesPerRow, Control);
			else
				return null;
		}

		public void Unlock (BitmapData bitmapData)
		{
			//Control.UnlockFocus();
			// don't need to do anythin
		}

		public void Save (Stream stream, ImageFormat format)
		{
			NSBitmapImageFileType type;
			switch (format) {
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
				throw new NotSupportedException ();
			}
			var reps = Control.Representations ();
			if (reps == null)
				throw new InvalidDataException ();
			var newrep = reps.OfType<NSBitmapImageRep> ().FirstOrDefault ();
			if (newrep == null) {
				NSData tiff;
				if (this.bmprep != null)
					tiff = this.bmprep.TiffRepresentation;
				else
					tiff = Control.AsTiff ();
				newrep = new NSBitmapImageRep (tiff);
			}
			var data = newrep.RepresentationUsingTypeProperties (type, new NSDictionary ());
			var datastream = data.AsStream ();
			datastream.CopyTo (stream);
			stream.Flush ();
			datastream.Dispose ();
		}

		public override Size Size {
			get { 
				NSImageRep rep = this.rep;
				if (rep == null)
					rep = Control.BestRepresentationForDevice (null);
				if (rep != null)
					return new Size(rep.PixelsWide, rep.PixelsHigh);
				else
					return Control.Size.ToEtoSize();
			}
		}
		
		/*
		public override void DrawImage (GraphicsHandler graphics, int x, int y)
		{
			var nsimage = this.Control;
			var sourceRect = graphics.Translate(new SD.RectangleF(0, 0, nsimage.Size.Width, nsimage.Size.Height), nsimage.Size.Height);
			var destRect = graphics.TranslateView(new SD.RectangleF(x, y, nsimage.Size.Width, nsimage.Size.Height), false);
			nsimage.Draw(destRect, sourceRect, NSCompositingOperation.SourceOver, 1);
		}
		
		public override void DrawImage (GraphicsHandler graphics, int x, int y, int width, int height)
		{
			var nsimage = this.Control;
			var sourceRect = graphics.Translate(new SD.RectangleF(0, 0, nsimage.Size.Width, nsimage.Size.Height), nsimage.Size.Height);
			var destRect = graphics.TranslateView(new SD.RectangleF(x, y, width, height), false);
			nsimage.Draw(destRect, sourceRect, NSCompositingOperation.SourceOver, 1);
		}
		*/
		
		public override void DrawImage (GraphicsHandler graphics, RectangleF source, RectangleF destination)
		{
			var nsimage = this.Control;
			var sourceRect = graphics.Translate (source.ToSD (), nsimage.Size.Height);
			var destRect = graphics.TranslateView (destination.ToSD (), true, true);
			if (alpha)
				nsimage.Draw (destRect, sourceRect, NSCompositingOperation.SourceOver, 1);
			else
				nsimage.Draw (destRect, sourceRect, NSCompositingOperation.Copy, 1);
		}
	}
}
