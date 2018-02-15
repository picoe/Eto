using Eto.Drawing;
using System.Collections.Generic;
using sd = System.Drawing;
using System.Globalization;

namespace Eto.WinForms.Drawing
{
	public class FontFamilyHandler : WidgetHandler<sd.FontFamily, FontFamily>, FontFamily.IHandler
	{
		public string Name { get; set; }

		public string LocalizedName
		{
			get { return Control.Name; }
		}

		static IEnumerable<sd.FontStyle> Styles
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
			get
			{
				foreach (var style in Styles)
				{
					if (Control.IsStyleAvailable(style))
						yield return new FontTypeface(Widget, new FontTypefaceHandler(style));
				}
			}
		}

		public FontFamilyHandler()
		{
		}

		public FontFamilyHandler(sd.FontFamily windowsFamily)
		{
			this.Control = windowsFamily;
			Name = Control.GetName(0);
		}

		public void Create(string familyName)
		{
			Name = familyName;
			switch (familyName.ToUpperInvariant())
			{
				case FontFamilies.MonospaceFamilyName:
					Control = sd.FontFamily.GenericMonospace;
					break;
				case FontFamilies.SansFamilyName:
					Control = sd.FontFamily.GenericSansSerif;
					break;
				case FontFamilies.SerifFamilyName:
					Control = sd.FontFamily.GenericSerif;
					break;
				case FontFamilies.CursiveFamilyName:
					Control = new sd.FontFamily("Comic Sans MS");
					break;
				case FontFamilies.FantasyFamilyName:
					Control = new sd.FontFamily("Gabriola");
					break;
				default:
					Control = new sd.FontFamily(familyName);
					Name = Control.GetName(0);
					break;
			}
		}
	}
}
