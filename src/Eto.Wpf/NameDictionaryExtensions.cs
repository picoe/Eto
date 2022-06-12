using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Media;
using System.Windows.Markup;

namespace Eto.Wpf.CustomControls.FontDialog
{
    static class NameDictionaryExtensions
    {
		public static string GetEnglishName(this LanguageSpecificStringDictionary nameDictionary)
		{
			return GetName(nameDictionary, "en-us");
		}

		public static string GetEnglishName(this IDictionary<CultureInfo, string> nameDictionary)
		{
			return GetName(nameDictionary, CultureInfo.GetCultureInfo("en-us"));
		}

		public static string GetDisplayName(this LanguageSpecificStringDictionary nameDictionary)
		{
			return GetName(nameDictionary, CultureInfo.CurrentUICulture.IetfLanguageTag);
		}

		public static string GetDisplayName(this IDictionary<CultureInfo, string> nameDictionary)
		{
			return GetName(nameDictionary, CultureInfo.CurrentUICulture);
		}


		public static string GetName(this IDictionary<CultureInfo, string> nameDictionary, CultureInfo culture)
		{
			if (nameDictionary.TryGetValue(culture, out var name))
			{
				return name;
			}

			// No exact match; return the name for the most closely related language.
			int bestRelatedness = -1;
			string bestName = string.Empty;

			foreach (KeyValuePair<CultureInfo, string> pair in nameDictionary)
			{
				int relatedness = GetRelatedness(pair.Key, culture);
				if (relatedness > bestRelatedness)
				{
					bestRelatedness = relatedness;
					bestName = pair.Value;
				}
			}

			return bestName;
		}

		public static string GetName(this LanguageSpecificStringDictionary nameDictionary, string ietfLanguageTag)
        {
            // Look up the display name based on the UI culture, which is the same culture
            // used for resource loading.
            var userLanguage = XmlLanguage.GetLanguage(ietfLanguageTag);

            // Look for an exact match.
            string name;
            if (nameDictionary.TryGetValue(userLanguage, out name))
            {
                return name;
            }

            // No exact match; return the name for the most closely related language.
            int bestRelatedness = -1;
            string bestName = string.Empty;

            foreach (KeyValuePair<XmlLanguage, string> pair in nameDictionary)
            {
                int relatedness = GetRelatedness(pair.Key, userLanguage);
                if (relatedness > bestRelatedness)
                {
                    bestRelatedness = relatedness;
                    bestName = pair.Value;
                }
            }

            return bestName;
        }

        static int GetRelatedness(XmlLanguage keyLang, XmlLanguage userLang)
        {
			CultureInfo keyCulture = CultureInfo.GetCultureInfoByIetfLanguageTag(keyLang.IetfLanguageTag);
			CultureInfo userCulture = CultureInfo.GetCultureInfoByIetfLanguageTag(userLang.IetfLanguageTag);
			return GetRelatedness(keyCulture, userCulture);
        }

		static int GetRelatedness(CultureInfo keyCulture, CultureInfo userCulture)
		{
			try
			{
				// Get equivalent cultures.
				if (!userCulture.IsNeutralCulture)
				{
					userCulture = userCulture.Parent;
				}

				// If the key is a prefix or parent of the user language it's a good match.
				if (IsPrefixOf(keyCulture.IetfLanguageTag, userCulture.IetfLanguageTag) || userCulture.Equals(keyCulture))
				{
					return 2;
				}

				// If the key and user language share a common prefix or parent neutral culture, it's a reasonable match.
				if (IsPrefixOf(TrimSuffix(userCulture.IetfLanguageTag), keyCulture.IetfLanguageTag) || userCulture.Equals(keyCulture.Parent))
				{
					return 1;
				}
			}
			catch (ArgumentException)
			{
				// Language tag with no corresponding CultureInfo.
			}

			// They're unrelated languages.
			return 0;
		}

		static string TrimSuffix(string tag)
		{
			int i = tag.LastIndexOf('-');
			return i > 0 ? tag.Substring(0, i) : tag;
		}

        static bool IsPrefixOf(string prefix, string tag)
        {
            return prefix.Length < tag.Length &&
                tag[prefix.Length] == '-' &&
                string.CompareOrdinal(prefix, 0, tag, 0, prefix.Length) == 0;
        }
    }
}
