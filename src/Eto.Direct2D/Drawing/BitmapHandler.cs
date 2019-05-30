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
		public WicBitmapData(Image image, sw.BitmapLock bitmapLock, int bitsPerPixel)
			: base(image, bitmapLock.Data.DataPointer, bitmapLock.Data.Pitch, bitsPerPixel, bitmapLock)
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
			return new WicBitmapData(Widget, data, bpp);
        }

        public void Unlock(BitmapData bitmapData)
        {
			Reset();
        }
	}
}
