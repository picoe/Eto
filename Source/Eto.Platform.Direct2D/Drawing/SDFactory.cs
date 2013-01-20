using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using s = SharpDX;
using sd = SharpDX.Direct2D1;
using sw = SharpDX.DirectWrite;

namespace Eto.Platform.Direct2D.Drawing
{
    static class SDFactory
    {
        static sd.Factory instance;
        
        public static sd.Factory Instance
        {
            get
            {
                if (instance == null)
                    instance = new sd.Factory();

                return instance;
            }
        }
    }
}
