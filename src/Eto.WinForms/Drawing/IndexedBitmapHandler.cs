using System;
using System.Globalization;
using System.Linq;
using Eto.Drawing;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Shared.Drawing;

namespace Eto.WinForms.Drawing
{

	public class IndexedBitmapDataHandler : BaseBitmapData
	{
		public IndexedBitmapDataHandler(Image image, IntPtr data, int scanWidth, int bitsPerPixel, object controlObject)
			: base (image, data, scanWidth, bitsPerPixel, controlObject)
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

	public class IndexedBitmapHandler : WidgetHandler<SD.Bitmap, IndexedBitmap>, IndexedBitmap.IHandler, IWindowsImage
	{
		public void Create(int width, int height, int bitsPerPixel)
		{
			SD.Imaging.PixelFormat sdPixelFormat;
			switch (bitsPerPixel)
			{
				case 8:
					sdPixelFormat = SD.Imaging.PixelFormat.Format8bppIndexed;
					break;
				case 4:
					sdPixelFormat = SD.Imaging.PixelFormat.Format4bppIndexed;
					break;
				case 1:
					sdPixelFormat = SD.Imaging.PixelFormat.Format1bppIndexed;
					break;
				default:
					throw new ArgumentOutOfRangeException("bitsPerPixel", bitsPerPixel, string.Format(CultureInfo.CurrentCulture, "Not supported"));
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
			return new BitmapDataHandler(Widget, bd.Scan0, bd.Stride, bd.PixelFormat.BitsPerPixel(), bd);
		}

		public void Unlock(BitmapData bitmapData)
		{
			Control.UnlockBits((SD.Imaging.BitmapData)bitmapData.ControlObject);
		}

		public Palette Palette
		{
			get
			{
				SD.Imaging.ColorPalette cp = Control.Palette;
				return new Palette(cp.Entries.Select(r => r.ToEto ()).ToList ());
			}
			set
			{
				SD.Imaging.ColorPalette cp = Control.Palette;
				if (value.Count != cp.Entries.Length)
					throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Input palette must have the same colors as the output"));
				for (int i=0; i<value.Count; i++)
				{
					cp.Entries[i] = value[i].ToSD ();
				}
				Control.Palette = cp;
			}
		}


		public SD.Image GetImageWithSize(int? size)
		{
			return Control;
		}

		public SD.Image GetImageWithSize(Size? size)
		{
			return Control;
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
