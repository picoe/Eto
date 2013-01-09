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
		public string Name { get; set; }

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
			Name = Control.Name;
		}

		public void Create (string familyName)
		{
			Name = familyName;
			switch (familyName.ToLowerInvariant()) {
			case FontFamilies.MonospaceFamilyName:
				this.Control = sd.FontFamily.GenericMonospace;
				break;
			case FontFamilies.SansFamilyName:
				this.Control = sd.FontFamily.GenericSansSerif;
				break;
			case FontFamilies.SerifFamilyName:
				this.Control = sd.FontFamily.GenericSerif;
				break;
			case FontFamilies.CursiveFamilyName:
				this.Control = new sd.FontFamily ("Comic Sans MS");
				break;
			case FontFamilies.FantasyFamilyName:
				this.Control = new sd.FontFamily ("Gabriola");
				break;
			default:
				this.Control = new sd.FontFamily (familyName);
				Name = Control.Name;
				break;
			}
		}
	}
}
