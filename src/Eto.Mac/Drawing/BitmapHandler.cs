using Eto.Mac.Forms;
using Eto.Shared.Drawing;
namespace Eto.Mac.Drawing
{
	/// <summary>
	/// Bitmap data handler.
	/// </summary>
	/// <copyright>(c) 2012-2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class BitmapDataHandler : BaseBitmapData
	{
		public BitmapDataHandler(Bitmap bitmap, IntPtr data, int scanWidth, int bitsPerPixel, object controlObject, bool isPremultiplied)
			: base(bitmap, data, scanWidth, bitsPerPixel, controlObject, isPremultiplied)
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
			var a = (uint)(byte)(argb >> 24);
			var r = (uint)(byte)(argb >> 16);
			var g = (uint)(byte)(argb >> 8);
			var b = (uint)(byte)(argb);
			if (PremultipliedAlpha)
			{
				r = r * a / 255;
				g = g * a / 255;
				b = b * a / 255;
			}
			return unchecked((int)((a << 24) | (b << 16) | (g << 8) | (r)));
		}

		public override int TranslateDataToArgb(int bitmapData)
		{
			var a = (uint)(byte)(bitmapData >> 24);
			var b = (uint)(byte)(bitmapData >> 16);
			var g = (uint)(byte)(bitmapData >> 8);
			var r = (uint)(byte)(bitmapData);
			if (a > 0 && PremultipliedAlpha)
			{
				b = b * 255 / a;
				g = g * 255 / a;
				r = r * 255 / a;
			}
			return unchecked((int)((a << 24) | (r << 16) | (g << 8) | (b)));
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
			rep = GetBestRepresentation();
			bmprep = rep as NSBitmapImageRep;
			alpha = rep.HasAlpha;
		}

		public void Create(string fileName)
		{
			if (!File.Exists(fileName))
				throw new FileNotFoundException("Icon not found", fileName);
			Control = new NSImage(fileName);
			rep = GetBestRepresentation();
			bmprep = rep as NSBitmapImageRep;
			Control.Size = new CGSize(rep.PixelsWide, rep.PixelsHigh);
			alpha = rep.HasAlpha;
		}

		public void Create(Stream stream)
		{
			Control = new NSImage(NSData.FromStream(stream));
			rep = GetBestRepresentation();
			bmprep = rep as NSBitmapImageRep;
			Control.Size = new CGSize(rep.PixelsWide, rep.PixelsHigh);
			alpha = rep.HasAlpha;
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

		public override Size Size => Control.Size.ToEtoSize();

		public override void DrawImage(GraphicsHandler graphics, RectangleF source, RectangleF destination)
		{
			if (Control.Template)
			{
				DrawTemplateImage(graphics, source, destination);
				return;
			}
			
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

			// don't convert colorspace here otherwise we get incorrect data.. why?
			var nscolor = bmprep.ColorAt(x, y);
			if (nscolor.ComponentCount >= 3)
			{
				nscolor.GetRgba(out var red, out var green, out var blue, out var alpha);
				return new Color(nscolor, (float)red, (float)green, (float)blue, (float)alpha);
			}
			
			return nscolor.ToEto();
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
			if (bmprep == null)
				return null;
			
			bool isPremultiplied = alpha && !bmprep.BitmapFormat.HasFlag(NSBitmapFormat.AlphaNonpremultiplied);
			return new BitmapDataHandler(Widget, bmprep.BitmapData, (int)bmprep.BytesPerRow, (int)bmprep.BitsPerPixel, Control, isPremultiplied);
		}

		public void Unlock(BitmapData bitmapData)
		{
		}

		public NSBitmapImageRep GetBitmapImageRep()
		{
			EnsureRep();
			return bmprep;
		}
		
		NSImageRep GetBestRepresentation()
		{
			// Control.BestRepresentationForDevice() is deprecated
			return Control.BestRepresentation(new CGRect(CGPoint.Empty, Control.Size), null, null);
		}


		protected void EnsureRep()
		{
			if (rep == null)
				rep = GetBestRepresentation();

			// on Big Sur, rep is usually going to be a proxy, so let's find the concrete NSBitmapImageRep class the slow way..

			if (bmprep != null)
				return;

			if (rep is IconFrameHandler.LazyImageRep lazyRep)
			{
				bmprep = lazyRep.Rep;
			}
			else
			{
				bmprep = rep as NSBitmapImageRep ?? GetBestRepresentation() as NSBitmapImageRep;
			}

			if (bmprep != null)
				return;

			// go through concrete representations as we might have a proxy (Big Sur)
			// this is fixed with MonoMac, but not Xamarin.Mac.
			var representations = Control.Representations();
			for (int i = 0; i < representations.Length; i++)
			{
				NSImageRep rep = representations[i];
				if (rep is NSBitmapImageRep brep)
				{
					bmprep = brep;
					return;
				}
			}

			// create a new bitmap rep and copy the contents
			var size = Size;
			int numComponents = rep.HasAlpha ? 4 : 3;
			int bitsPerComponent = 8;
			int bitsPerPixel = numComponents * bitsPerComponent;
			int bytesPerPixel = bitsPerPixel / 8;
			int bytesPerRow = bytesPerPixel * size.Width;
			bmprep = new NSBitmapImageRep(IntPtr.Zero, size.Width, size.Height, bitsPerComponent, numComponents, rep.HasAlpha, false, rep.ColorSpaceName, bytesPerRow, bitsPerPixel);
			var graphicsContext = NSGraphicsContext.FromBitmap(bmprep);
			NSGraphicsContext.GlobalSaveGraphicsState();
			NSGraphicsContext.CurrentContext = graphicsContext;
			Control.Draw(CGPoint.Empty, new CGRect(CGPoint.Empty, size.ToNS()), NSCompositingOperation.Copy, 1);
			NSGraphicsContext.GlobalRestoreGraphicsState();
			
			// remove all existing representations
			for (int i = 0; i < representations.Length; i++)
			{
				NSImageRep rep = representations[i];
				Control.RemoveRepresentation(rep);
			}

			// add the new one back
			Control.AddRepresentation(bmprep);
		}
	}
}
