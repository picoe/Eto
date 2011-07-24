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
		public BitmapDataHandler (IntPtr data,int scanWidth,object controlObject)
			: base(data, scanWidth, controlObject)
		{
		}

		public override uint TranslateArgbToData(uint argb)
		{
			return (argb & 0xFF00FF00) | ((argb & 0xFF) << 16) | ((argb & 0xFF0000) >> 16);
		}

		public override uint TranslateDataToArgb(uint bitmapData)
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
		NSBitmapImageRep rep;
		
		public BitmapHandler()
		{
		}
		
		public BitmapHandler(NSImage image)
		{
			Control = image;
		}

		public void Create (string fileName)
		{
			Control = new NSImage (fileName);
		}

		public void Create (Stream stream)
		{
			Control = new NSImage (NSData.FromStream (stream));
		}

		public void Create (int width, int height, PixelFormat pixelFormat)
		{			
			switch (pixelFormat) {
			case PixelFormat.Format32bppRgb:
				{
					int numComponents = 4;
					int bitsPerComponent = 8;
					int bitsPerPixel = numComponents * bitsPerComponent;
					int bytesPerPixel = bitsPerPixel / 8;
					int bytesPerRow = bytesPerPixel * width;
				
					rep = new NSBitmapImageRep(IntPtr.Zero, width, height, bitsPerComponent, numComponents, true, false, NSColorSpace.DeviceRGB, bytesPerRow, bitsPerPixel);
					Control = new NSImage();
					Control.AddRepresentation(rep);
				
					//var provider = new CGDataProvider (data.Bytes, (int)data.Length);
					//var cgImage = new CGImage (width, height, bitsPerComponent, bitsPerPixel, bytesPerRow, CGColorSpace.CreateDeviceRGB (), CGBitmapFlags.ByteOrder32Little | CGBitmapFlags.PremultipliedFirst, provider, null, true, CGColorRenderingIntent.Default);
					//Control = new NSImage (cgImage, new System.Drawing.SizeF (width, height));
				
					break;
				}
			case PixelFormat.Format24bppRgb:
				{
					int numComponents = 3;
					int bitsPerComponent = 8;
					int bitsPerPixel = numComponents * bitsPerComponent;
					int bytesPerPixel = bitsPerPixel / 8;
					int bytesPerRow = bytesPerPixel * width;
				
					rep = new NSBitmapImageRep(IntPtr.Zero, width, height, bitsPerComponent, numComponents, false, false, NSColorSpace.DeviceRGB, bytesPerRow, bitsPerPixel);
					Control = new NSImage();
					Control.AddRepresentation(rep);

					//var provider = new CGDataProvider (data.ClassHandle);
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

		public BitmapData Lock ()
		{
			//Control.LockFocus();
			return new BitmapDataHandler (rep.BitmapData, rep.BytesPerRow, Control);
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
			var reps = Control.Representations();
			if (reps == null) throw new InvalidDataException();
			var newrep = reps.OfType<NSBitmapImageRep>().FirstOrDefault();
			if (newrep == null) {
				NSData tiff;
				if (this.rep != null) tiff = this.rep.TiffRepresentation;
				else tiff = Control.AsTiff();
				newrep = new NSBitmapImageRep(tiff);
			}
			var data = newrep.RepresentationUsingTypeProperties (type, new NSDictionary());
			var datastream = data.AsStream ();
			datastream.CopyTo (stream);
			stream.Flush();
			datastream.Dispose ();
		}

		public override Size Size {
			get { return Generator.ConvertF (Control.Size); }
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
		
		public override void DrawImage (GraphicsHandler graphics, Rectangle source, Rectangle destination)
		{
			var nsimage = this.Control;
			var sourceRect = graphics.Translate(Generator.ConvertF(source), nsimage.Size.Height);
			var destRect = graphics.TranslateView(Generator.ConvertF(destination), false);
			nsimage.Draw(destRect, sourceRect, NSCompositingOperation.Copy, 1);
		}

	}
}
