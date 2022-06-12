using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;
using s = SharpDX;
using sd = SharpDX.Direct2D1;
using sw = SharpDX.WIC;
using System.IO;
using Eto.Shared.Drawing;

namespace Eto.Direct2D.Drawing
{
	public class WicBitmapData : BaseBitmapData
	{
		public WicBitmapData(Image image, sw.BitmapLock bitmapLock, int bitsPerPixel, bool premultipliedAlpha)
			: base(image, bitmapLock.Data.DataPointer, bitmapLock.Data.Pitch, bitsPerPixel, bitmapLock, premultipliedAlpha)
		{
		}

		public override int TranslateArgbToData(int argb)
		{
			if (PremultipliedAlpha)
			{
				var a = (uint)(byte)(argb >> 24);
				var r = (uint)(byte)(argb >> 16);
				var g = (uint)(byte)(argb >> 8);
				var b = (uint)(byte)(argb);
				r = r * a / 255;
				g = g * a / 255;
				b = b * a / 255;
				return unchecked((int)((a << 24) | (r << 16) | (g << 8) | (b)));
			}
			return argb;
		}

		public override int TranslateDataToArgb(int bitmapData)
		{
			if (PremultipliedAlpha)
			{
				var a = (uint)(byte)(bitmapData >> 24);
				var r = (uint)(byte)(bitmapData >> 16);
				var g = (uint)(byte)(bitmapData >> 8);
				var b = (uint)(byte)(bitmapData);
				if (a > 0)
				{
					b = b * 255 / a;
					g = g * 255 / a;
					r = r * 255 / a;
				}
				return unchecked((int)((a << 24) | (r << 16) | (g << 8) | (b)));
			}
			return bitmapData;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				((sw.BitmapLock)ControlObject).Dispose();
			}
			base.Dispose(disposing);
		}
	}

	public class BitmapHandler : ImageHandler<Bitmap>, Bitmap.IHandler
    {
		public BitmapHandler()
		{
		}

		public BitmapHandler(sw.Bitmap control)
		{
			Control = control;
		}
		
        public BitmapData Lock()
        {
			var data = Control.Lock(sw.BitmapLockFlags.Write);
			var bpp = Control.PixelFormat == PixelFormat.Format24bppRgb.ToWic() ? 24 : 32;
			return new WicBitmapData(Widget, data, bpp, IsPremultiplied);
        }

        public void Unlock(BitmapData bitmapData)
        {
			Reset();
        }
	}
}
