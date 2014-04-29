using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;
using s = SharpDX;
using sd = SharpDX.Direct2D1;
using sw = SharpDX.DirectWrite;

namespace Eto.Direct2D.Drawing
{
    public class FontTypefaceHandler : WidgetHandler<sw.FontFace, FontTypeface>, IFontTypeface
    {
		public sw.Font Font { get; private set; }

		public FontTypefaceHandler(sw.Font font)
		{
			Font = font;
			Control = new sw.FontFace(font);
		}

		public FontStyle FontStyle
		{
			get { return Font.ToEtoStyle(); }
		}

		public string Name
		{
			get { return Font.FaceNames.GetString(0); }
		}
    }
}
