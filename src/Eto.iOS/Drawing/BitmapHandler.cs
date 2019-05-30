using System;
using System.IO;
using System.Linq;
using Eto.Drawing;
using SD = System.Drawing;
using UIKit;
using Foundation;
using CoreGraphics;
using Eto.Shared.Drawing;

namespace Eto.iOS.Drawing
{
	public class BitmapDataHandler : BaseBitmapData
	{
		public BitmapDataHandler(Image image, IntPtr data, int scanWidth, int bitsPerPixel, object controlObject)
			: base(image, data, scanWidth, bitsPerPixel, controlObject)
		{
		}

		public override int TranslateArgbToData(int argb)
		{
			return argb; //(argb & 0xFF00FF00) | ((argb & 0xFF) << 16) | ((argb & 0xFF0000) >> 16);
		}

		public override int TranslateDataToArgb(int bitmapData)
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

	public class BitmapHandler : ImageHandler<UIImage, Bitmap>, Bitmap.IHandler
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

		public BitmapHandler()
		{
		}

		public BitmapHandler(UIImage image)
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
						Control = UIImage.FromImage(cgimage, 0f, UIImageOrientation.Up);
				
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
						Control = UIImage.FromImage(cgimage, 0f, UIImageOrientation.Up);
				
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
						Control = UIImage.FromImage(cgimage, 0f, UIImageOrientation.Up);
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
			Control = source.Scale(new CGSize(width, height), 0f);
		}

		public void Create(int width, int height, Graphics graphics)
		{
			Create(width, height, PixelFormat.Format32bppRgba);
		}

		public BitmapData Lock()
		{
			cgimage = cgimage ?? Control.CGImage;
			if (Data == null)
			{
				Data = (NSMutableData)cgimage.DataProvider.CopyData().MutableCopy();
				provider = new CGDataProvider(Data.MutableBytes, (int)Data.Length, false);
				cgimage = new CGImage((int)cgimage.Width, (int)cgimage.Height, (int)cgimage.BitsPerComponent, (int)cgimage.BitsPerPixel, (int)cgimage.BytesPerRow, cgimage.ColorSpace, cgimage.BitmapInfo, provider, null, cgimage.ShouldInterpolate, cgimage.RenderingIntent);
				Control = UIImage.FromImage(cgimage);
			}
			return new BitmapDataHandler(Widget, Data.MutableBytes, (int)cgimage.BytesPerRow, (int)cgimage.BitsPerPixel, Control);
		}

		public void Unlock(BitmapData bitmapData)
		{
			// don't need to do anythin
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
			var destRect = new CGRect(x, y, (int)nsimage.Size.Width, (int)nsimage.Size.Height);
			nsimage.Draw(destRect, CGBlendMode.Normal, 1);
		}

		public override void DrawImage(GraphicsHandler graphics, RectangleF source, RectangleF destination)
		{
			var destRect = destination;
			var drawRect = GetDrawRect(ref source, ref destRect, Control.Size.ToEto());
			graphics.Control.ClipToRect(destRect.ToNS()); // first apply the clip since destination is in view coordinates.
			Control.Draw(drawRect.ToNS(), CGBlendMode.Normal, 1);
		}

		private static RectangleF GetDrawRect(ref RectangleF source, ref RectangleF destRect, SizeF imageSize)
		{
			var scale = destRect.Size / source.Size;
			var scaledImageSize = imageSize * scale;
			// We want the source rectangle's location to coincide with the destination rectangle's location.
			// However the source image is drawn scaled.
			// The relevant equation is:
			// source.Location * scale + offset = destination.Location, which gives:
			var offset = (destRect.Location - (source.Location * scale));
			var drawRect = new RectangleF(offset, scaledImageSize);
			return drawRect;
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

		public override UIImage GetUIImage(int? maxSize = null)
		{
			var size = Size;
			var imgSize = Math.Max(size.Width, size.Height);
			if (maxSize != null && imgSize > maxSize.Value)
			{
				size = (Size)(size * ((float)maxSize.Value / (float)imgSize));
				var img = new Bitmap(Widget, size.Width, size.Height);
				return img.ToUI();
			}
			return Control;
		}

		public Bitmap Clone(Rectangle? rectangle = null)
		{
			if (rectangle == null)
				return new Bitmap(new BitmapHandler { Control = new UIImage(Control.CGImage.Clone()) });
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
				
				var bmp = new Bitmap(rect.Width, rect.Height, format);
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
						return Color.FromArgb(data.TranslateDataToArgb(*(int*)srcrow));
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
