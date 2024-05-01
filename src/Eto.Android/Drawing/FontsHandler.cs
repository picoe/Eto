using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using ag = Android.Graphics;
using System.Xml.Linq;
namespace Eto.Android.Drawing
{
	public class FontsHandler : WidgetHandler<Widget>, Fonts.IHandler
	{
		private IEnumerable<FontFamily> availableFontFamilies;

		public IEnumerable<FontFamily> AvailableFontFamilies
		{
			get
			{
				if (availableFontFamilies == null)
					availableFontFamilies = LoadFamilies();

				return availableFontFamilies;
			}
		}

		private IEnumerable<FontFamily> LoadFamilies()
		{
			if (TryLoadFamiliesPreL(out var familiesPreL))
				return familiesPreL;

			if (TryLoadFamiliesPostL(out familiesPreL))
				return familiesPreL;

			return Enumerable.Empty<FontFamily>();
		}

		private bool TryLoadFamiliesPreL(out IEnumerable<FontFamily> families)
		{
			try
			{
				families = XDocument.Load("/system/etc/system_fonts.xml")
					.Element("familyset")
					?.Elements("family")
					?.Elements("nameset")
					?.Elements("name")
					?.Select(e => e.Value)
					?.Distinct()
					?.Select(n => new FontFamily(new FontFamilyHandler(n)))
					?.ToArray();

				return families != null;
			}
			catch
			{
				families = null;
				return false;
			}
		}

		private bool TryLoadFamiliesPostL(out IEnumerable<FontFamily> families)
		{
			families = null;

			try
			{
				var FamilySet = XDocument.Load("/system/etc/fonts.xml")
					.Element("familyset");

				if (FamilySet == null)
					return false;

				var Families = FamilySet.Elements("family")
					.Attributes("name");
				
				var Aliases = FamilySet.Elements("alias")
					.Attributes("name");

				families = Families
					.Concat(Aliases)
					.Select(a => a.Value)
					.Distinct()
					.Select(n => new FontFamily(new FontFamilyHandler(n)))
					.ToArray();

				return true;
			}
			catch
			{
				families = null;
				return false;
			}
		}

		public bool FontFamilyAvailable(string fontFamily)
		{
			return AvailableFontFamilies.Any(f => String.Equals(f.Name, fontFamily, StringComparison.OrdinalIgnoreCase));
		}
	}
}
