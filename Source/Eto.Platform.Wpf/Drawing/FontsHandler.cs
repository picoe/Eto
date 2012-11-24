using Eto.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swm = System.Windows.Media;

namespace Eto.Platform.Wpf.Drawing
{
	public class FontsHandler : WidgetHandler<Widget>, IFonts
	{
		public IEnumerable<FontFamily> AvailableFontFamilies
		{
			get { return swm.Fonts.SystemFontFamilies.Select (r => new FontFamily(Generator, new FontFamilyHandler(r))); ; }
		}

		public FontFamily GetFontFamily (string familyName)
		{
			return new FontFamily(Generator, new FontFamilyHandler (new swm.FontFamily(familyName)));
		}

		public IFontFamily GetSystemFontFamily (string systemFamilyName)
		{
			switch (systemFamilyName) {
			case FontFamilies.MonospaceFamilyName:
				return new FontFamilyHandler (new swm.FontFamily ("Courier New"));
			case FontFamilies.SansFamilyName:
				return new FontFamilyHandler (new swm.FontFamily ("Verdana"));
			case FontFamilies.SerifFamilyName:
				return new FontFamilyHandler (new swm.FontFamily ("Times New Roman"));
			default:
				throw new NotSupportedException ();
			}
		}
	}
}
