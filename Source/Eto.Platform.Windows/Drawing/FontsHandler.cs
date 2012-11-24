using Eto.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sd = System.Drawing;

namespace Eto.Platform.Windows.Drawing
{
	public class FontsHandler : WidgetHandler<Widget>, IFonts
	{
		public IEnumerable<FontFamily> AvailableFontFamilies
		{
			get {
				return sd.FontFamily.Families.Select (r => new FontFamily(Generator, new FontFamilyHandler(r)));
			}
		}

		public FontFamily GetFontFamily (string familyName)
		{
			return new FontFamily (Generator, new FontFamilyHandler (new sd.FontFamily (familyName)));
		}

		public IFontFamily GetSystemFontFamily (string systemFamilyName)
		{
			switch (systemFamilyName) {
			case FontFamilies.MonospaceFamilyName: return new FontFamilyHandler (sd.FontFamily.GenericMonospace);
			case FontFamilies.SansFamilyName: return new FontFamilyHandler (sd.FontFamily.GenericSansSerif);
			case FontFamilies.SerifFamilyName: return new FontFamilyHandler (sd.FontFamily.GenericSerif);
			default:
				throw new NotSupportedException ();
			}
		}
	}
}
