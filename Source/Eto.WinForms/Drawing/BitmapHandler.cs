using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Eto.Drawing;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using ImageManipulation;

namespace Eto.WinForms.Drawing
{
	/// <summary>
	/// Interface to get an image representation with the specified size
	/// </summary>
	public interface IWindowsImageSource
	{
		SD.Image GetImageWithSize(int? size);
	}

	/// <summary>
	/// Interface for all windows images
	/// </summary>
	/// <copyright>(c) 2012-2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public interface IWindowsImage : IWindowsImageSource
	{
		void DrawImage(GraphicsHandler graphics, RectangleF source, RectangleF destination);

		void DrawImage(GraphicsHandler graphics, float x, float y);

		void DrawImage(GraphicsHandler graphics, float x, float y, float width, float height);
	}

	/// <summary>
	/// Bitmap data handler.
	/// </summary>
	/// <copyright>(c) 2012-2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class BitmapDataHandler : BitmapData
	{
		public BitmapDataHandler(Image image, IntPtr data, int scanWidth, int bitsPerPixel, object controlObject)
			: base(image, data, scanWidth, bitsPerPixel, controlObject)
		{
		}

		public override int TranslateArgbToData(int argb)
		{
			return argb;
		}

		public override int TranslateDataToArgb(int bitmapData)
		{
			return bitmapData;
		}
	}

	/// <summary>
	/// Bitmap handler.
	/// </summary>
	/// <copyright>(c) 2012-2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class BitmapHandler : WidgetHandler<SD.Bitmap, Bitmap>, Bitmap.IHandler, IWindowsImage
	{
		public BitmapHandler()
		{
		}

		public BitmapHandler(SD.Bitmap image)
		{
			Control = image;
		}

		public void Create(string fileName)
		{
			// We create a temp image from the file
			// because SD.Bitmap(filename) locks the file
			// until the image is disposed.
			// this is not the case in mono
			if (EtoEnvironment.Platform.IsWindows)
			{
				using (var temp = new SD.Bitmap(fileName))
					Control = new SD.Bitmap(temp);
			}
			else
				Control = new SD.Bitmap(fileName);
		}

		public void Create(Stream stream)
		{
			Control = new SD.Bitmap(stream);
		}

		public void Create(int width, int height, PixelFormat pixelFormat)
		{
			SD.Imaging.PixelFormat sdPixelFormat;
			switch (pixelFormat)
			{
				case PixelFormat.Format32bppRgb:
					sdPixelFormat = SD.Imaging.PixelFormat.Format32bppRgb;
					break;
				case PixelFormat.Format24bppRgb:
					sdPixelFormat = SD.Imaging.PixelFormat.Format24bppRgb;
					break;
			/*case PixelFormat.Format16bppRgb555:
					sdPixelFormat = SD.Imaging.PixelFormat.Format16bppRgb555;
					break;*/
				case PixelFormat.Format32bppRgba:
					sdPixelFormat = SD.Imaging.PixelFormat.Format32bppPArgb;
					break;
				default:
					throw new ArgumentOutOfRangeException("pixelFormat", pixelFormat, string.Format(CultureInfo.CurrentCulture, "Not supported"));
			}
			Control = new SD.Bitmap(width, height, sdPixelFormat);
		}

		public void Create(int width, int height, Graphics graphics)
		{
			Control = new SD.Bitmap(width, height, GraphicsHandler.GetControl(graphics));
		}

		public void Create(Image image, int width, int height, ImageInterpolation interpolation)
		{
			var source = image.ToSD();
			var pixelFormat = source.PixelFormat;
			if (
				pixelFormat == SD.Imaging.PixelFormat.Indexed
				|| pixelFormat == SD.Imaging.PixelFormat.Format1bppIndexed
				|| pixelFormat == SD.Imaging.PixelFormat.Format4bppIndexed
				|| pixelFormat == SD.Imaging.PixelFormat.Format8bppIndexed)
				pixelFormat = SD.Imaging.PixelFormat.Format32bppRgb;
			Control = new SD.Bitmap(width, height, pixelFormat);
			using (var graphics = SD.Graphics.FromImage(Control))
			{
				graphics.InterpolationMode = interpolation.ToSD();
				var rect = new SD.Rectangle(0, 0, width, height);
				graphics.FillRectangle(SD.Brushes.Transparent, rect);
				graphics.DrawImage(source, rect);
			}
		}

		public Size Size
		{
			get { return new Size(Control.Width, Control.Height); }
		}

		public BitmapData Lock()
		{
			SD.Imaging.BitmapData bd = Control.LockBits(new SD.Rectangle(0, 0, Control.Width, Control.Height), SD.Imaging.ImageLockMode.ReadWrite, Control.PixelFormat);
			return new BitmapDataHandler(Widget, bd.Scan0, bd.Stride, bd.PixelFormat.BitsPerPixel(), bd);
		}

		public void Unlock(BitmapData bitmapData)
		{
			Control.UnlockBits((SD.Imaging.BitmapData)bitmapData.ControlObject);
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
			if (format == ImageFormat.Gif)
			{
				var quantizer = new OctreeQuantizer(255, 8);
				var gif = quantizer.Quantize(Control);
				gif.Save(stream, format.ToSD());
			}
			else
				Control.Save(stream, format.ToSD());
		}

		public SD.Image GetImageWithSize(int? size)
		{
			if (size != null)
			{
				var max = Math.Max(Control.Width, Control.Height);
				var newsize = new SD.Size(size.Value * Control.Width / max, size.Value * Control.Height / max);
				return new SD.Bitmap(Control, newsize);
			}
			return Control;
		}

		public Bitmap Clone(Rectangle? rectangle = null)
		{
			SD.Bitmap copy;
			// copy data directly to avoid odd System.Drawing symantics of locking/sharing data.
			// this allows us to use the Bitmap.Lock() method after cloning. Using Clone() variations
			// will cause a GDI+ exception.
			var rect = rectangle ?? new Rectangle(Size);
			var srcbits = Control.LockBits(rect.ToSD(), SD.Imaging.ImageLockMode.ReadOnly, Control.PixelFormat);
			try
			{
				copy = new SD.Bitmap(rect.Width, rect.Height, Control.PixelFormat);
				var copybits = copy.LockBits(new Rectangle(rect.Size).ToSD(), SD.Imaging.ImageLockMode.WriteOnly, Control.PixelFormat);
				try
				{
					IntPtr srcptr = srcbits.Scan0;
					IntPtr copyptr = copybits.Scan0;
					var len = copybits.Stride;
					var temp = new byte[len]; // temp scanline buffer
					for (int y = 0; y < rect.Height; y++)
					{
						Marshal.Copy(srcptr, temp, 0, len);
						Marshal.Copy(temp, 0, copyptr, len);
						srcptr += srcbits.Stride;
						copyptr += copybits.Stride;
					}
				}
				finally
				{
					copy.UnlockBits(copybits);
				}
			}
			finally
			{
				Control.UnlockBits(srcbits);
			}
			if ((copy.PixelFormat & SD.Imaging.PixelFormat.Indexed) != 0)
				copy.Palette = Control.Palette;

			return new Bitmap(new BitmapHandler(copy));
		}

		public Color GetPixel(int x, int y)
		{
			return Control.GetPixel(x, y).ToEto();
		}

		public void DrawImage(GraphicsHandler graphics, RectangleF source, RectangleF destination)
		{
			graphics.Control.DrawImage(Control, destination.ToSD(), source.ToSD(), SD.GraphicsUnit.Pixel);
		}

		public void DrawImage(GraphicsHandler graphics, float x, float y)
		{
			graphics.Control.DrawImage(Control, x, y);
		}

		public void DrawImage(GraphicsHandler graphics, float x, float y, float width, float height)
		{
			graphics.Control.DrawImage(Control, x, y, width, height);
		}
	}
}
