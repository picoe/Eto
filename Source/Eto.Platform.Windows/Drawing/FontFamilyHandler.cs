using Eto.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sd = System.Drawing;

namespace Eto.Platform.Windows.Drawing
{
	public class FontFamilyHandler : WidgetHandler<sd.FontFamily, FontFamily>, IFontFamily
	{
		public string Name
		{
			get { return Control.Name; }
		}

		IEnumerable<sd.FontStyle> Styles
		{
			get
			{
				yield return sd.FontStyle.Regular;
				yield return sd.FontStyle.Bold;
				yield return sd.FontStyle.Italic;
				yield return sd.FontStyle.Bold | sd.FontStyle.Italic;
			}
		}

		public IEnumerable<FontTypeface> Typefaces
		{
			get {
				foreach (var style in Styles) {
					if (Control.IsStyleAvailable (style))
						yield return new FontTypeface (Widget, new FontTypefaceHandler (style));
				}
			}
		}

		public FontFamilyHandler ()
		{
		}

		public FontFamilyHandler (sd.FontFamily windowsFamily)
		{
			this.Control = windowsFamily;
		}

		public void Create (string familyName)
		{
			this.Control = new sd.FontFamily (familyName);
		}
	}
}
