using System;
using Eto.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace Eto.GtkSharp.Drawing
{
	public class FontFamilyHandler : WidgetHandler<Pango.FontFamily, FontFamily>, FontFamily.IHandler
	{
		public string Name { get; set; }

		public IEnumerable<FontTypeface> Typefaces
		{
			get { return Control.Faces.Select (r => new FontTypeface(Widget, new FontTypefaceHandler(r))); }
		}

		public FontFamilyHandler ()
		{
		}
		
		public FontFamilyHandler (Pango.FontFamily pangoFamily)
		{
			Control = pangoFamily;
			Name = Control.Name;
		}

		public void Create (string familyName)
		{
			Name = familyName;
			switch (familyName.ToUpperInvariant()) {
			case FontFamilies.MonospaceFamilyName:
				Control = GetFontFamily("monospace", "FreeMono", "Courier");
				break;
			case FontFamilies.SansFamilyName:
				Control = GetFontFamily("sans", "FreeSans");
				break;
			case FontFamilies.SerifFamilyName:
				Control = GetFontFamily("serif", "FreeSerif");
				break;
			case FontFamilies.CursiveFamilyName:
				// from http://www.codestyle.org/css/font-family/sampler-Cursive.shtml#cursive-linux
				Control = GetFontFamily("URW Chancery L", "Comic Sans MS", "Purisa", "Vemana2000", "Domestic Manners", "serif");
				break;
			case FontFamilies.FantasyFamilyName:
				// from http://www.codestyle.org/css/font-family/sampler-Fantasy.shtml#fantasy-linux
				Control = GetFontFamily("Impact", "Penguin Attack", "Balker", "Marked Fool", "Junkyard", "Linux Biolinum", "serif");
				break;
			default:
				Control = GetFontFamily(familyName);
				if (Control == null)
					throw new ArgumentOutOfRangeException("familyName", familyName, "Font Family specified is not supported by this system");
				Name = Control.Name;
				break;
			}
		}

		static Pango.FontFamily GetFontFamily(params string [] familyNames)
		{
			foreach (var familyName in familyNames)
			{
				var family = GetFontFamily (familyName);
				if (family != null)
					return family;
			}
			return null;
		}

		public static Pango.FontFamily GetFontFamily(string familyName)
		{
			return FontsHandler.Context.Families.FirstOrDefault(r => string.Equals(r.Name, familyName, StringComparison.InvariantCultureIgnoreCase));
		}
	}
}

