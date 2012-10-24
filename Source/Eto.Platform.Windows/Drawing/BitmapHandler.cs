using System;
using System.IO;
using Eto.Drawing;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using ImageManipulation;

namespace Eto.Platform.Windows.Drawing
{
	public interface IWindowsImage
	{
		SD.Image GetImageWithSize (int? size);
	}


	public class BitmapDataHandler : BitmapData
	{
		public BitmapDataHandler(IntPtr data, int scanWidth, object controlObject)
			: base(data, scanWidth, controlObject)
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

		public Size Size
		{
			get { return new Size(Control.Width, Control.Height); }
		}

		public void Resize(int width, int height)
		{
			Control = new SD.Bitmap(Control, new SD.Size(width, height));
		}

		public BitmapData Lock()
		{
			SD.Imaging.BitmapData bd = Control.LockBits(new SD.Rectangle(0, 0, Control.Width, Control.Height), SD.Imaging.ImageLockMode.ReadWrite, Control.PixelFormat);
			return new BitmapDataHandler(bd.Scan0, bd.Stride, bd);
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
				var yummygif = quantizer.Quantize(Control);
				yummygif.Save(stream, format.ToSD ());
			}
			else  Control.Save(stream, format.ToSD ());
		}


		public SD.Image GetImageWithSize (int? size)
		{
			return Control;
		}
	}
}
