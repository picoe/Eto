using System;
using System.IO;
using Eto.Drawing;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using System.Linq;

namespace Eto.Platform.Windows.Drawing
{

	public class IndexedBitmapDataHandler : BitmapData
	{
		public IndexedBitmapDataHandler(IntPtr data, int scanWidth, object controlObject)
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

	public class IndexedBitmapHandler : WidgetHandler<SD.Bitmap, IndexedBitmap>, IIndexedBitmap
	{

		public IndexedBitmapHandler()
		{
		}


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
					throw new ArgumentOutOfRangeException("bitsPerPixel", bitsPerPixel, "Not supported");
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

		public Palette Palette
		{
			get
			{
				SD.Imaging.ColorPalette cp = Control.Palette;
				var pal = new Palette(cp.Entries.Length);
				pal.AddRange (cp.Entries.Select(r => Generator.Convert (r)));
				
				return pal;
			}
			set
			{
				SD.Imaging.ColorPalette cp = Control.Palette;
				if (value.Count != cp.Entries.Length) throw new ArgumentException("Input palette must have the same colors as the output");
				for (int i=0; i<value.Count; i++)
				{
					cp.Entries[i] = Generator.Convert(value[i]);
				}
				Control.Palette = cp;
			}
		}

	}
}
