using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using s = SharpDX;
using sd = SharpDX.Direct2D1;
using sw = SharpDX.DirectWrite;

namespace Eto.Direct2D.Drawing
{
    public static class SDFactory
    {
        static sd.Factory d2D1Factory;        
        public static sd.Factory D2D1Factory { get { return d2D1Factory ?? (d2D1Factory = new sd.Factory()); } }

		static s.WIC.ImagingFactory wicImagingFactory;
		public static s.WIC.ImagingFactory WicImagingFactory { get { return wicImagingFactory ?? (wicImagingFactory = new s.WIC.ImagingFactory()); } }

		static sw.Factory directWriteFactory;
		public static sw.Factory DirectWriteFactory { get { return directWriteFactory ?? (directWriteFactory = new sw.Factory()); } }
    }
}
