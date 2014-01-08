using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;
using s = SharpDX;
using sd = SharpDX.Direct2D1;
using sw = SharpDX.WIC;
using System.IO;

namespace Eto.Platform.Direct2D.Drawing
{
	public class WicBitmapData : BitmapData
	{
		public WicBitmapData(Image image, sw.BitmapLock bitmapLock, int bitsPerPixel)
			: base(image, bitmapLock.Data.DataPointer, bitmapLock.Data.Pitch, bitsPerPixel, bitmapLock)
		{
		}

		public override uint TranslateArgbToData(uint argb)
		{
			return (argb & 0xFFFF) << 8 | (argb & 0xFF000000) >> 16 | (argb & 0xFF000000);
		}

		public override uint TranslateDataToArgb(uint bitmapData)
		{
			return (bitmapData & 0xFFFF00) >> 8 | (bitmapData & 0xFF) << 16 | (bitmapData & 0xFF000000);
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

	public class BitmapHandler : ImageHandler<Bitmap>, IBitmap
    {
        public BitmapData Lock()
        {
			var data = Control.Lock(sw.BitmapLockFlags.Write);
			return new WicBitmapData(Widget, data, 32);
        }

        public void Unlock(BitmapData bitmapData)
        {
        }
	}
}
