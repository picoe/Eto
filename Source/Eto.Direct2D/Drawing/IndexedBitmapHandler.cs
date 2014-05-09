using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;
using s = SharpDX;
using sd = SharpDX.Direct2D1;
using sw = SharpDX.WIC;
using System.IO;

namespace Eto.Direct2D.Drawing
{
	public class IndexedBitmapHandler : ImageHandler<IndexedBitmap>, IndexedBitmap.IHandler
    {
		public static Guid GetFormat(int bitsPerPixel)
		{
			switch (bitsPerPixel)
			{
				case 8:
					return sw.PixelFormat.Format8bppIndexed;
				case 4:
					return sw.PixelFormat.Format4bppIndexed;
				case 2:
					return sw.PixelFormat.Format2bppIndexed;
				case 1:
					return sw.PixelFormat.Format1bppIndexed;
				default:
					throw new NotSupportedException();
			}
		}

		protected override sd.Bitmap CreateDrawableBitmap(sd.RenderTarget target)
		{
			using (var converter = new sw.FormatConverter(SDFactory.WicImagingFactory))
			{
				converter.Initialize(Control, sw.PixelFormat.Format32bppPBGRA);
				var bmp = new sw.Bitmap(SDFactory.WicImagingFactory, converter, sw.BitmapCreateCacheOption.CacheOnLoad);
				return sd.Bitmap.FromWicBitmap(target, bmp);
			}
		}

		public void Create(int width, int height, int bitsPerPixel)
		{
			Control = new sw.Bitmap(SDFactory.WicImagingFactory, width, height, GetFormat(bitsPerPixel), sw.BitmapCreateCacheOption.CacheOnLoad);
		}

		public BitmapData Lock()
        {
			var data = Control.Lock(sw.BitmapLockFlags.Write);
			return new WicBitmapData(Widget, data, Widget.BitsPerPixel);
        }

        public void Unlock(BitmapData bitmapData)
        {
			Reset();
        }

		public void Resize(int width, int height)
		{
			throw new NotImplementedException();
		}

		Palette palette;
		public Palette Palette
		{
			get { return palette; }
			set
			{
				palette = value;
				var pal = new sw.Palette(SDFactory.WicImagingFactory);
				// for some reason, Red/Blue are swapped here when displaying the bitmap
				pal.Initialize(palette.Select(r => new s.Color4(r.B, r.G, r.R, r.A)).ToArray());
				Control.Palette = pal;
			}
		}
	}
}