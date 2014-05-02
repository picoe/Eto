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
		public BitmapDataHandler(Image image, IntPtr data, int scanWidth, int bitsPerPixel, object controlObject)
			: base(image, data, scanWidth, bitsPerPixel, controlObject)
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

		public override bool Flipped
		{
			get
			{
				return false;
			}
		}
	}

	public class BitmapHandler : ImageHandler<UIImage, Bitmap>, IBitmap
	{
		CGDataProvider provider;
		CGImage cgimage;

		public NSMutableData Data { get; private set; }

		public void Create(string fileName)
		{
			Control = new UIImage(fileName);
		}

		public void Create(Stream stream)
		{
			Control = new UIImage(NSData.FromStream(stream));
		}

		internal void Create(UIImage image)
		{
			Control = image;
		}

		public void Create(int width, int height, PixelFormat pixelFormat)
		{			
			switch (pixelFormat)
			{
				case PixelFormat.Format32bppRgba:
					{
						const int numComponents = 4;
						const int bitsPerComponent = 8;
						const int bitsPerPixel = numComponents * bitsPerComponent;
						const int bytesPerPixel = bitsPerPixel / 8;
						int bytesPerRow = bytesPerPixel * width;

						Data = NSMutableData.FromLength(bytesPerRow * height);

						provider = new CGDataProvider(Data.MutableBytes, (int)Data.Length, false);
						cgimage = new CGImage(width, height, bitsPerComponent, bitsPerPixel, bytesPerRow, CGColorSpace.CreateDeviceRGB(), CGBitmapFlags.ByteOrder32Little | CGBitmapFlags.PremultipliedFirst, provider, null, true, CGColorRenderingIntent.Default);
						Control = UIImage.FromImage(cgimage);
				
						break;
					}
				case PixelFormat.Format32bppRgb:
					{
						const int numComponents = 4;
						const int bitsPerComponent = 8;
						const int bitsPerPixel = numComponents * bitsPerComponent;
						const int bytesPerPixel = bitsPerPixel / 8;
						int bytesPerRow = bytesPerPixel * width;
						Data = NSMutableData.FromLength(bytesPerRow * height);
						//Data = new NSMutableData ((uint)(bytesPerRow * height));
				
						provider = new CGDataProvider(Data.MutableBytes, (int)Data.Length, false);
						cgimage = new CGImage(width, height, bitsPerComponent, bitsPerPixel, bytesPerRow, CGColorSpace.CreateDeviceRGB(), CGBitmapFlags.ByteOrder32Little | CGBitmapFlags.NoneSkipFirst, provider, null, true, CGColorRenderingIntent.Default);
						Control = UIImage.FromImage(cgimage);
				
						break;
					}
				case PixelFormat.Format24bppRgb:
					{
						const int numComponents = 3;
						const int bitsPerComponent = 8;
						const int bitsPerPixel = numComponents * bitsPerComponent;
						const int bytesPerPixel = bitsPerPixel / 8;
						int bytesPerRow = bytesPerPixel * width;
						Data = new NSMutableData((uint)(bytesPerRow * height));
				
						provider = new CGDataProvider(Data.MutableBytes, (int)Data.Length, false);
						cgimage = new CGImage(width, height, bitsPerComponent, bitsPerPixel, bytesPerRow, CGColorSpace.CreateDeviceRGB(), CGBitmapFlags.ByteOrder32Little | CGBitmapFlags.PremultipliedFirst, provider, null, true, CGColorRenderingIntent.Default);
						Control = UIImage.FromImage(cgimage);
						break;
					}
				default:
					throw new ArgumentOutOfRangeException("pixelFormat", pixelFormat, "Not supported");
			}
		}

		public void Create(Image image, int width, int height, ImageInterpolation interpolation)
		{
			var source = image.ToUI();
			// todo: use interpolation
			Control = source.Scale(new SD.SizeF(width, height));
		}

		public void Create(int width, int height, Graphics graphics)
		{
			Create(width, height, PixelFormat.Format32bppRgba);
		}

		public BitmapData Lock()
		{
			cgimage = cgimage ?? Control.CGImage;
			return new BitmapDataHandler(Widget, Data.MutableBytes, cgimage.BytesPerRow, cgimage.BitsPerPixel, Control);
		}

		public void Unlock(BitmapData bitmapData)
		{
			// don't need to do anythin
		}

		public void Save(Stream stream, ImageFormat format)
		{
			NSData data;
			switch (format)
			{
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
					throw new NotSupportedException();
			}
			data.AsStream().CopyTo(stream);
			data.Dispose();
		}

		public override Size Size
		{
			get { return Control.Size.ToEtoSize(); }
		}

		public override void DrawImage(GraphicsHandler graphics, float x, float y)
		{
			var nsimage = Control;
			var destRect = graphics.TranslateView(new SD.RectangleF(x, y, (int)nsimage.Size.Width, (int)nsimage.Size.Height), false);
			nsimage.Draw(destRect, CGBlendMode.Normal, 1);
		}

		public override void DrawImage(GraphicsHandler graphics, RectangleF source, RectangleF destination)
		{
			var sourceRect = source.ToSD();
			var imgsize = Control.Size;
			SD.RectangleF destRect = graphics.TranslateView(destination.ToSD(), false);
			if (source.TopLeft != Point.Empty || sourceRect.Size != imgsize)
			{
				graphics.Control.TranslateCTM(destRect.X - sourceRect.X, imgsize.Height + (destRect.Y - sourceRect.Y));
				graphics.Control.ScaleCTM(imgsize.Width / sourceRect.Width, -(imgsize.Height / sourceRect.Height));
				graphics.Control.DrawImage(new SD.RectangleF(SD.PointF.Empty, destRect.Size), Control.CGImage);
			}
			else
			{
				Control.Draw(destRect, CGBlendMode.Normal, 1);
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				if (Data != null)
				{
					Data.Dispose();
					Data = null;
				}
			}
		}

		public override UIImage GetUIImage()
		{
			return Control;
		}

		public Bitmap Clone(Rectangle? rectangle = null)
		{
			if (rectangle == null)
				return new Bitmap(Generator, new BitmapHandler { Control = (UIImage)Control.Copy() });
			else
			{
				var rect = rectangle.Value;
				cgimage = cgimage ?? Control.CGImage;
				PixelFormat format;
				if (cgimage.BitsPerPixel == 24)
					format = PixelFormat.Format24bppRgb;
				else if (cgimage.AlphaInfo == CGImageAlphaInfo.None)
					format = PixelFormat.Format32bppRgb;
				else
					format = PixelFormat.Format32bppRgba;
				
				var bmp = new Bitmap(rect.Width, rect.Height, format, Generator);
				using (var graphics = new Graphics (bmp))
				{
					graphics.DrawImage(Widget, rect, new Rectangle(rect.Size));
				}
				return bmp;
			}
		}

		public Color GetPixel(int x, int y)
		{
			using (var data = Lock ())
			{
				unsafe
				{
					var srcrow = (byte*)data.Data;
					srcrow += y * data.ScanWidth;
					srcrow += x * data.BytesPerPixel;
					if (data.BytesPerPixel == 4)
					{
						return Color.FromArgb(data.TranslateDataToArgb(*(uint*)srcrow));
					}
					else if (data.BytesPerPixel == 3)
					{
						var b = *(srcrow ++);
						var g = *(srcrow ++);
						var r = *(srcrow ++);
						return Color.FromArgb(r, g, b);
					}
					else
						throw new NotSupportedException();
				}
			}
		}
	}
}
