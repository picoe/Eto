using System;
using System.IO;
using Eto.Drawing;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using ImageManipulation;

namespace Eto.Platform.Windows.Drawing
{
	/// <summary>
	/// Interface for all windows images
	/// </summary>
	/// <copyright>(c) 2012-2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public interface IWindowsImage
	{
		SD.Image GetImageWithSize (int? size);

		void DrawImage (GraphicsHandler graphics, RectangleF source, RectangleF destination);

		void DrawImage (GraphicsHandler graphics, float x, float y);

		void DrawImage (GraphicsHandler graphics, float x, float y, float width, float height);
	}

	/// <summary>
	/// Bitmap data handler.
	/// </summary>
	/// <copyright>(c) 2012-2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class BitmapDataHandler : BitmapData
	{
		public BitmapDataHandler(Image image, IntPtr data, int scanWidth, int bitsPerPixel, object controlObject)
			: base (image, data, scanWidth, bitsPerPixel, controlObject)
		{
		}

		public override uint TranslateArgbToData(uint argb)
		{
			return argb;
		}

		public override uint TranslateDataToArgb(uint bitmapData)
		{
			return bitmapData;
		}
	}

	/// <summary>
	/// Bitmap handler.
	/// </summary>
	/// <copyright>(c) 2012-2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class BitmapHandler : WidgetHandler<SD.Bitmap, Bitmap>, IBitmap, IWindowsImage
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
					throw new ArgumentOutOfRangeException("pixelFormat", pixelFormat, "Not supported");
			}
			Control = new SD.Bitmap(width, height, sdPixelFormat);
		}

		public void Create(int width, int height, Graphics graphics)
		{
			this.Control = new SD.Bitmap(width, height, GraphicsHandler.GetControl (graphics));
		}

		public void Create (Image image, int width, int height, ImageInterpolation interpolation)
		{
			var source = image.ToSD ();
			Control = new SD.Bitmap (width, height, source.PixelFormat);
			using (var graphics = SD.Graphics.FromImage(Control)) {
				graphics.InterpolationMode = interpolation.ToSD ();
				var rect = new SD.Rectangle (0, 0, width, height);
				graphics.FillRectangle (SD.Brushes.Transparent, rect);
				graphics.DrawImage (source, rect);
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
		
		public void Save(Stream stream, ImageFormat format)
		{
			if (format == ImageFormat.Gif)
			{
				var quantizer = new OctreeQuantizer (255, 8);
				var gif = quantizer.Quantize(Control);
				gif.Save(stream, format.ToSD ());
			}
			else  Control.Save(stream, format.ToSD ());
		}

		public SD.Image GetImageWithSize (int? size)
		{
			if (size != null) {
				var max = Math.Max(Control.Width, Control.Height);
				var newsize = new SD.Size (size.Value * Control.Width / max, size.Value * Control.Height / max);
				return new SD.Bitmap (Control, newsize);
			}
			else
				return Control;
		}

		public Bitmap Clone(Rectangle? rectangle = null)
		{
			SD.Bitmap copy;
			if (rectangle == null)
				copy = (SD.Bitmap)this.Control.Clone ();
			else
				copy = this.Control.Clone (rectangle.Value.ToSD(), this.Control.PixelFormat);

			return new Bitmap (Generator, new BitmapHandler (copy));
		}

		public Color GetPixel(int x, int y)
		{
			return this.Control.GetPixel(x, y).ToEto();
		}

		public void DrawImage (GraphicsHandler graphics, RectangleF source, RectangleF destination)
		{
			graphics.Control.DrawImage (Control, destination.ToSD (), source.ToSD (), SD.GraphicsUnit.Pixel);
		}

		public void DrawImage (GraphicsHandler graphics, float x, float y)
		{
			graphics.Control.DrawImage (Control, x, y);
		}

		public void DrawImage (GraphicsHandler graphics, float x, float y, float width, float height)
		{
			graphics.Control.DrawImage (Control, x, y, width, height);
		}
	}
}
