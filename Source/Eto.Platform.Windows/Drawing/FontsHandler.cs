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
		HashSet<string> availableFontFamilies;

		public IEnumerable<FontFamily> AvailableFontFamilies
		{
			get {
				return sd.FontFamily.Families.Select (r => new FontFamily(Generator, new FontFamilyHandler(r)));
			}
		}

		public bool FontFamilyAvailable (string fontFamily)
		{
			if (availableFontFamilies == null) {
				availableFontFamilies = new HashSet<string> (StringComparer.InvariantCultureIgnoreCase);
				foreach (var family in sd.FontFamily.Families) {
					availableFontFamilies.Add (family.Name);
				}
			}
			return availableFontFamilies.Contains (fontFamily);
		}
	}
}
