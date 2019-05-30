using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Eto.Drawing;

namespace Eto.GtkSharp.Drawing
{
	public class FontFamilyHandler : WidgetHandler<Pango.FontFamily, FontFamily>, FontFamily.IHandler
	{
		public string Name { get; set; }

		public string LocalizedName => Control?.Name ?? Name;

		public IEnumerable<FontTypeface> Typefaces
		{
			get { return Control.Faces.Where(r => r != null).Select(r => new FontTypeface(Widget, new FontTypefaceHandler(r))); }
		}

		public FontFamilyHandler()
		{
		}

		public FontFamilyHandler(Pango.FontFamily pangoFamily)
		{
			Control = pangoFamily;
			Name = Control.Name;
		}

		public static Pango.FontFamily FindCorrectedFamily(string familyName)
		{
			var family = GetFontFamily(familyName);
			if (family == null)
			{
				if (EtoEnvironment.Platform.IsMac && familyName == ".AppleSystemUIFont")
				{
					// Hack to map to .SF NS Text on macOS.
					// currently only works in Gtk2 as the system font isn't mapped.
					family = GetFontFamily(".SF NS Text");
					if (family != null)
						return family;
				}

				// HACK: Sometimes font description is not actually valid?
				familyName = familyName.TrimStart('.');
				family = GetFontFamily(familyName);
				if (family == null)
				{
					var idx = familyName.LastIndexOf(' ');
					while (family == null && idx > 0)
					{
						familyName = familyName.Substring(0, idx);
						family = GetFontFamily(familyName);
						idx = familyName.LastIndexOf(' ');
					}
				}
			}
			return family;
		}

		public void Create(string familyName)
		{
			Name = familyName;
			switch (familyName.ToUpperInvariant())
			{
				case FontFamilies.MonospaceFamilyName:
					Control = GetFontFamily("monospace", "FreeMono", "Courier New", "Courier");
					break;
				case FontFamilies.SansFamilyName:
					Control = GetFontFamily("sans", "FreeSans", "Arial", "Helvetica");
					break;
				case FontFamilies.SerifFamilyName:
					Control = GetFontFamily("serif", "FreeSerif", "Times New Roman", "Times");
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
					Control = FindCorrectedFamily(familyName);
					if (Control == null)
						throw new ArgumentOutOfRangeException(nameof(familyName), familyName, "Font Family specified is not supported by this system");
					Name = Control.Name;
					break;
			}
		}

		static Pango.FontFamily GetFontFamily(params string[] familyNames)
		{
			foreach (var familyName in familyNames)
			{
				var family = GetFontFamily(familyName);
				if (family != null)
					return family;
			}
			return null;
		}

		public static Pango.FontFamily GetFontFamily(string familyName)
		{
			if (string.IsNullOrEmpty(familyName))
				return null;
			return FontsHandler.Context.Families.FirstOrDefault(r => string.Equals(r.Name, familyName, StringComparison.InvariantCultureIgnoreCase));
		}
	}
}

