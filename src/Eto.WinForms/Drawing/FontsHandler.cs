using Eto.Shared.Drawing;
using Microsoft.Win32;
namespace Eto.WinForms.Drawing
{
	public class FontsHandler : WidgetHandler<Widget>, Fonts.IHandler
	{
		HashSet<string> availableFontFamilies;
		static FontFamily[] _families;

		/// <summary>
		/// Setting to use typographic family/sub-family names vs. the old style which uses FontStyle as the typeface names.
		/// Note this is disabled for now as it doesn't properly deal with variable fonts or localized names.
		/// </summary>
		public static bool UseTypographicFonts { get; set; } = false;

		public IEnumerable<FontFamily> AvailableFontFamilies => _families ?? (_families = GetFontFamilies().ToArray());
		
		public static IEnumerable<FontFamily> GetFontFamilies()
		{
			if (UseTypographicFonts)
				return GetTypographicFontFamilies();
			else
				return GetNormalFontFamilies();
		}
		
		public static IEnumerable<FontFamily> GetNormalFontFamilies()
		{
			return sd.FontFamily.Families.Select(r => new FontFamily(new FontFamilyHandler(r)));
		}

		internal static string FindFontFamilyName(sd.FontFamily sdfamily)
		{
			_families = _families ?? (_families = GetFontFamilies().ToArray());
			var familyName = sdfamily.Name;
			foreach (var family in _families.Select(r => r.Handler).OfType<FontFamilyHandler>())
			{
				if (!familyName.StartsWith(family.Name, StringComparison.OrdinalIgnoreCase))
					continue;
					
				foreach (var typeface in family.Typefaces.Select(r => r.Handler).OfType<FontTypefaceHandler>())
				{
					if (typeface.SDFontFamily.Name == familyName)
					{
						return family.Name;
					}
				}
			}
			return null;
		}

		internal static string FindFontTypefaceName(sd.FontFamily sdfamily, sd.FontStyle fontStyle)
		{
			_families = _families ?? (_families = GetFontFamilies().ToArray());
			var familyName = sdfamily.Name;
			foreach (var family in _families.Select(r => r.Handler).OfType<FontFamilyHandler>())
			{
				if (!familyName.StartsWith(family.Name, StringComparison.OrdinalIgnoreCase))
					continue;

				foreach (var typeface in family.Typefaces.Select(r => r.Handler).OfType<FontTypefaceHandler>())
				{
					if (typeface.SDFontFamily.Name == familyName && typeface.Control == fontStyle)
					{
						return typeface.Name;
					}
				}
			}
			return null;
		}

		public static IEnumerable<FontFamily> GetTypographicFontFamilies()
		{
			var fontInfos = GetInstalledFontFiles().SelectMany(OpenTypeFontInfo.FromFile);
			var familyInfos = fontInfos.Where(r => r != null).GroupBy(r => r.TypographicFamilyName ?? r.FamilyName);
			foreach (var familyInfo in familyInfos)
			{
				var family = FontFamilyHandler.CreateFamily(familyInfo.Key, familyInfo);
				if (family == null)
					continue;
				yield return family;
			}
		}

		public static IEnumerable<string> GetInstalledFontFiles()
		{
			var fontsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);

			using (var installedFonts = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows NT\CurrentVersion\Fonts"))
			{
				if (installedFonts != null)
				{
					foreach (var installedFont in installedFonts.GetValueNames())
					{
						var filename = installedFonts.GetValue(installedFont) as string;
						if (string.IsNullOrWhiteSpace(filename))
							continue;
						if (!Path.IsPathRooted(filename))
							filename = Path.Combine(fontsFolder, filename);
						if (File.Exists(filename))
							yield return filename;
					}
				}
			}
		}


		public bool FontFamilyAvailable(string fontFamily)
		{
			if (availableFontFamilies == null)
			{
				availableFontFamilies = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
				foreach (var family in AvailableFontFamilies)
				{
					availableFontFamilies.Add(family.Name);
				}
			}
			return availableFontFamilies.Contains(fontFamily);
		}
	}
}
