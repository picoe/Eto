using Eto.Drawing;
using System.Collections.Generic;
using sd = System.Drawing;
using System.Globalization;
using System.Linq;
using System.IO;
using System;
using Eto.Shared.Drawing;

namespace Eto.WinForms.Drawing
{
	public class FontFamilyHandler : WidgetHandler<sd.FontFamily, FontFamily>, FontFamily.IHandler
	{
		string _name;
		public string Name => _name ?? (_name = Control.GetName(0));

		public string LocalizedName => Control.Name;

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

		FontTypeface[] _typefaces;

		public IEnumerable<FontTypeface> Typefaces => _typefaces ?? (_typefaces = GetTypefaces().ToArray());

		IEnumerable<FontTypeface> GetTypefaces()
		{
			if (FontsHandler.UseTypographicFonts)
			{
				var searchName = Name;
				switch (Name.ToUpperInvariant())
			{
				case FontFamilies.MonospaceFamilyName:
					searchName = sd.FontFamily.GenericMonospace.Name;
					break;
				case FontFamilies.SansFamilyName:
					searchName = sd.FontFamily.GenericSansSerif.Name;
					break;
				case FontFamilies.SerifFamilyName:
					searchName = sd.FontFamily.GenericSerif.Name;
					break;
				case FontFamilies.CursiveFamilyName:
					searchName = "Comic Sans MS";
					break;
				case FontFamilies.FantasyFamilyName:
					searchName = "Gabriola";
					break;
			}

				var family = Eto.Drawing.Fonts.AvailableFontFamilies.FirstOrDefault(r => r.Name == searchName);
				if (family == null)
					yield break;
					
				foreach (var typeface in family.Typefaces)
				{
					yield return typeface;
				}
				yield break;
			}
			foreach (var style in Styles)
			{
				if (Control.IsStyleAvailable(style))
					yield return new FontTypeface(Widget, new FontTypefaceHandler(Control, style));
			}
		}

		internal void SetTypefaces(FontTypeface[] typefaces)
		{
			_typefaces = typefaces;
		}

		public FontFamilyHandler()
		{
		}

		public FontFamilyHandler(sd.FontFamily windowsFamily, string name = null)
		{
			Control = windowsFamily;
			_name = name;
			if (_name == null && FontsHandler.UseTypographicFonts)
			{
				_name = FontsHandler.FindFontFamilyName(Control);
			}
		}

		public void Create(string familyName)
		{
			_name = familyName;
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
					_name = Control.GetName(0);
					break;
			}
		}

		static string[] defaultVariationNames = new[] { "Bold", "Italic", "Regular" };

		internal static FontFamily CreateFamily(string familyName, IEnumerable<OpenTypeFontInfo> typefaceInfos)
		{
			var familyHandler = new FontFamilyHandler(null, familyName);
			var family = new FontFamily(familyHandler);
			var typefaces = new List<FontTypeface>();

			void AddTypeface(OpenTypeFontInfo typefaceInfo, string sdfamilyName, string variationName)
			{
				try
				{
					var sdfamily = new sd.FontFamily(sdfamilyName);
					if (!familyHandler.HasControl)
						familyHandler.Control = sdfamily;
					var typefaceHandler = new FontTypefaceHandler(sdfamily, typefaceInfo, variationName);
					var typeface = new FontTypeface(family, typefaceHandler);
					typefaces.Add(typeface);
				}
				catch (ArgumentException)
				{
				}
			}

			foreach (var typefaceInfo in typefaceInfos)
			{
				if (typefaceInfo.VariationSubFamilyNames?.Length > 0)
				{
					// variable font, combine with variations
					var gdinames = typefaceInfo.VariationSubFamilyNames.Select(r => GetGdiCompatibleName(typefaceInfo, r)).ToList();
					foreach (var name in gdinames)
					{
						AddTypeface(typefaceInfo, name.familyName, name.typefaceName);
					}
				}
				else
				{
					AddTypeface(typefaceInfo, typefaceInfo.FamilyName, null);
				}
			}
			if (typefaces.Count == 0)
				return null;

			familyHandler.SetTypefaces(typefaces.ToArray());
			return family;
		}

		private static (string familyName, string typefaceName) GetGdiCompatibleName(OpenTypeFontInfo typefaceInfo, string variationName)
		{
			var sdfamilyName = typefaceInfo.TypographicFamilyName ?? typefaceInfo.FamilyName;

			// turn variation names to GDI+ compatible name
			var subfamilies = variationName.Split(' ')
				.OrderBy(VariationSorter).ToList();

			var subfamilyName = string.Join(" ", subfamilies.Where(r => Array.IndexOf(defaultVariationNames, r) == -1));
			var typefaceName = string.Join(" ", subfamilies);

			if (!string.IsNullOrEmpty(subfamilyName))
				sdfamilyName += " " + subfamilyName;

			return (sdfamilyName, typefaceName);
		}

		private static int VariationSorter(string arg)
		{
			// GDI+ orders the variations in a consistent way it seems..
			if (arg.IndexOf("regular", StringComparison.OrdinalIgnoreCase) >= 0)
				return -1;
			if (arg.IndexOf("bold", StringComparison.OrdinalIgnoreCase) >= 0)
				return 2;
			if (arg.IndexOf("light", StringComparison.OrdinalIgnoreCase) >= 0)
				return 3;
			if (arg.IndexOf("condensed", StringComparison.OrdinalIgnoreCase) >= 0)
				return 4;
			return 0;
		}

		internal void SetFontCollection(sd.Text.PrivateFontCollection fontCollection) => _fontCollection = fontCollection;

		sd.Text.PrivateFontCollection _fontCollection;

		public void CreateFromFiles(IEnumerable<string> fileNames)
		{
			_fontCollection = new sd.Text.PrivateFontCollection();
			var infos = new List<OpenTypeFontInfo>();
			foreach (var fileName in fileNames)
			{
				_fontCollection.AddFontFile(fileName);
				infos.AddRange(OpenTypeFontInfo.FromFile(fileName));
			}

			// map to typographic family/typefaces
			MapToTypographic(infos);
		}

		private void MapToTypographic(List<OpenTypeFontInfo> infos)
		{
			var typefaces = new List<FontTypeface>();
			var families = _fontCollection.Families;
			for (int i = 0; i < infos.Count; i++)
			{
				var info = infos[i];
				var currentName = info.TypographicFamilyName ?? info.FamilyName;
				if (_name == null)
					_name = currentName;
				else if (_name != currentName)
					throw new InvalidOperationException($"Family name of the supplied font files do not match. '{_name}' and '{currentName}'");

				var family = families.FirstOrDefault(r => r.Name == info.FamilyName);
				if (family == null)
					family = families.First();

				var typefaceHandler = new FontTypefaceHandler(family, info);
				var typeface = new FontTypeface(Widget, typefaceHandler);
				typefaces.Add(typeface);
			}
			_typefaces = typefaces.ToArray();
			Control = _fontCollection.Families[0];
		}

		public unsafe void CreateFromStreams(IEnumerable<Stream> streams)
		{
			_fontCollection = new sd.Text.PrivateFontCollection();
			var infos = new List<OpenTypeFontInfo>();
			foreach (var stream in streams)
			{
				using (var ms = new MemoryStream())
				{
					stream.CopyTo(ms);

					var bytes = ms.ToArray();
					fixed (byte* ptr = bytes)
					{
						_fontCollection.AddMemoryFont((IntPtr)ptr, bytes.Length);
					}
					ms.Position = 0;
					infos.AddRange(OpenTypeFontInfo.FromStream(ms));
				}
			}
			MapToTypographic(infos);
		}
	}
}
