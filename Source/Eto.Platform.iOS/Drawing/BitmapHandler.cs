using System;
using System.IO;
using System.Linq;
using Eto.Drawing;
using SD = System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.CoreGraphics;

namespace Eto.Platform.iOS.Drawing
{
	public class BitmapDataHandler : BitmapData
	{
		public BitmapDataHandler (IntPtr data,int scanWidth,object controlObject)
			: base(data, scanWidth, controlObject)
		{
		}

		public override uint TranslateArgbToData(uint argb)
		{
			return argb; //(argb & 0xFF00FF00) | ((argb & 0xFF) << 16) | ((argb & 0xFF0000) >> 16);
		}

		public override uint TranslateDataToArgb(uint bitmapData)
		{
			return bitmapData; //(bitmapData & 0xFF00FF00) | ((bitmapData & 0xFF) << 16) | ((bitmapData & 0xFF0000) >> 16);
		}

		public override bool Flipped {
			get {
				return false;
			}
		}
	}

	public class BitmapHandler : ImageHandler<UIImage, Bitmap>, IBitmap
	{
		int bytesPerRow;
		CGDataProvider provider;
		CGImage cgimage;
		
		public NSMutableData Data { get; private set; }
		
		public void Create (string fileName)
		{
			Control = new UIImage (fileName);
		}

		public void Create (Stream stream)
		{
			Control = new UIImage (NSData.FromStream (stream));
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
					bytesPerRow = bytesPerPixel * width;
					Data = new NSMutableData((uint)(bytesPerRow*height));
				
					provider = new CGDataProvider (Data.MutableBytes, (int)Data.Length, false);
					cgimage = new CGImage (width, height, bitsPerComponent, bitsPerPixel, bytesPerRow, CGColorSpace.CreateDeviceRGB (), CGBitmapFlags.ByteOrder32Little | CGBitmapFlags.PremultipliedFirst, provider, null, true, CGColorRenderingIntent.Default);
					Control = UIImage.FromImage(cgimage);
				
					break;
				}
			case PixelFormat.Format24bppRgb:
				{
					int numComponents = 3;
					int bitsPerComponent = 8;
					int bitsPerPixel = numComponents * bitsPerComponent;
					int bytesPerPixel = bitsPerPixel / 8;
					bytesPerRow = bytesPerPixel * width;
					Data = new NSMutableData((uint)(bytesPerRow*height));
				
					provider = new CGDataProvider (Data.MutableBytes, (int)Data.Length, false);
					cgimage = new CGImage (width, height, bitsPerComponent, bitsPerPixel, bytesPerRow, CGColorSpace.CreateDeviceRGB (), CGBitmapFlags.ByteOrder32Little | CGBitmapFlags.PremultipliedFirst, provider, null, true, CGColorRenderingIntent.Default);
					Control = UIImage.FromImage(cgimage);
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
			return new BitmapDataHandler (Data.MutableBytes, bytesPerRow, Control);
		}

		public void Unlock (BitmapData bitmapData)
		{
			// don't need to do anythin
		}

		public void Save (Stream stream, ImageFormat format)
		{
			NSData data = null;
			switch (format) {
			case ImageFormat.Jpeg:
				data = Control.AsJPEG();
				break;
			case ImageFormat.Png:
				data = Control.AsPNG();
				break;
			case ImageFormat.Bitmap:
			case ImageFormat.Gif:
			case ImageFormat.Tiff:
			default:
				throw new NotSupportedException ();
			}
			data.AsStream().CopyTo(stream);
			data.Dispose();
		}

		public override Size Size {
			get { return Generator.ConvertF (Control.Size); }
		}
		
		public override void DrawImage (GraphicsHandler graphics, int x, int y)
		{
			var nsimage = this.Control;
			var destRect = graphics.TranslateView(Generator.ConvertF(new Rectangle(x, y, (int)nsimage.Size.Width, (int)nsimage.Size.Height)), false);
			nsimage.Draw(destRect, CGBlendMode.Normal, 1);
		}

		/*
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
			var sourceRect = Generator.ConvertF(source); 
			//var sourceRect = graphics.Translate(Generator.ConvertF(source), nsimage.Size.Height);
			SD.RectangleF destRect = graphics.TranslateView(Generator.ConvertF(destination), false);
			if (source.TopLeft != Point.Empty || sourceRect.Size != nsimage.Size)
			{
				graphics.Context.SaveState();
				//graphics.Context.ClipToRect(destRect);
				graphics.Context.TranslateCTM(0, nsimage.Size.Height);
				graphics.Context.ScaleCTM(nsimage.Size.Width / destRect.Width, -(nsimage.Size.Height / destRect.Height));
				graphics.Context.DrawImage(new SD.RectangleF(SD.PointF.Empty, destRect.Size), nsimage.CGImage);
				//nsimage.CGImage(destRect, CGBlendMode.Normal, 1);

				graphics.Context.RestoreState();
				
				//var imgportion = nsimage..CGImage.WithImageInRect(sourceRect);
				/*graphics.Context.SaveState();
				if (graphics.Flipped) {
					graphics.Context.TranslateCTM(0, destRect.Bottom);
					graphics.Context.ScaleCTM(1.0F, -1.0F);
				}*/
				//var context = graphics.ControlObject as CGContext;
				//Console.WriteLine("drawing source:{0} dest:{1}", source, destRect);
				//graphics.Context.DrawImage(destRect, imgportion);
				
				//nsimage = UIImage.FromImage(imgportion);
				//nsimage.Draw(destRect, CGBlendMode.Normal, 1);
				
				//imgportion.Dispose();
				//nsimage.Dispose();
				//graphics.Context.RestoreState();
			}
			else 
			{
				//graphics.Context.DrawImage(destRect, nsimage.CGImage);
				//Console.WriteLine("drawing full image");	
				nsimage.Draw(destRect, CGBlendMode.Normal, 1);
			}
		}
		
		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			if (disposing)
			{
				if (Data != null)
				{
					Data.Dispose();
					Data = null;
				}
			}
		}

	}
}
