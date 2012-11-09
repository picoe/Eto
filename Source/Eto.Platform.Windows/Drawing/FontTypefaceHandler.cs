using Eto.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sd = System.Drawing;

namespace Eto.Platform.Windows.Drawing
{
	public class FontTypefaceHandler : WidgetHandler<sd.FontStyle, FontTypeface>, IFontTypeface
	{
		public FontTypefaceHandler (sd.FontStyle style)
		{
			this.Control = style;
			Name = this.FontStyle.ToString ().Replace (',', ' ');
		}

		public string Name { get; set; }

		public FontStyle FontStyle
		{
			get { return Generator.Convert (Control); }
		}
	}
}
