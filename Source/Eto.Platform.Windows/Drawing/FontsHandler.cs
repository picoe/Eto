using Eto.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sd = System.Drawing;

namespace Eto.Platform.Windows.Drawing
{
	public class FontsHandler : IFonts
	{
		public IEnumerable<FontFamily> AvailableFontFamilies
		{
			get {
				return sd.FontFamily.Families.Select (r => new FontFamily(Generator.Current, new FontFamilyHandler(r)));
			}
		}

		public Widget Widget { get; set; }

		public void Initialize ()
		{
		}

		public FontFamily GetFontFamily (string familyName)
		{
			return new FontFamily (Generator.Current, new FontFamilyHandler (new sd.FontFamily (familyName)));
		}

		public FontFamily GetSystemFontFamily (string systemFamilyName)
		{
			switch (systemFamilyName) {
			case FontFamilies.MonospaceFamilyName: return new FontFamily (Generator.Current, new FontFamilyHandler (sd.FontFamily.GenericMonospace));
			case FontFamilies.SansFamilyName: return new FontFamily (Generator.Current, new FontFamilyHandler (sd.FontFamily.GenericSansSerif));
			case FontFamilies.SerifFamilyName: return new FontFamily (Generator.Current, new FontFamilyHandler (sd.FontFamily.GenericSerif));
			default:
				throw new NotSupportedException ();
			}
		}
	}
}
