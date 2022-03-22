using Eto.Drawing;
using System.Globalization;
using System.Linq;
using swm = System.Windows.Media;
using sw = System.Windows;
using swd = System.Windows.Documents;
using Eto.Wpf.CustomControls.FontDialog;
using System.Collections.Generic;
using Eto.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System;
using Eto.Shared.Drawing;

namespace Eto.Wpf.Drawing
{
	public class FontTypefaceHandler : WidgetHandler<swm.Typeface, FontTypeface>, FontTypeface.IHandler
	{
		string _name;
		string _localizedName;

		~FontTypefaceHandler()
		{
			Dispose(false);
		}

		public FontTypefaceHandler(swm.Typeface type)
		{
			this.Control = type;
		}

		public FontTypefaceHandler()
		{
		}

		public FontTypefaceHandler(swd.TextSelection range, sw.Controls.RichTextBox control)
		{
			var family = range.GetPropertyValue(swd.TextElement.FontFamilyProperty) as swm.FontFamily ?? swd.TextElement.GetFontFamily(control);
			var style = range.GetPropertyValue(swd.TextElement.FontStyleProperty) as sw.FontStyle? ?? swd.TextElement.GetFontStyle(control);
			var weight = range.GetPropertyValue(swd.TextElement.FontWeightProperty) as sw.FontWeight? ?? swd.TextElement.GetFontWeight(control);
			var stretch = range.GetPropertyValue(swd.TextElement.FontStretchProperty) as sw.FontStretch? ?? swd.TextElement.GetFontStretch(control);
			Control = new swm.Typeface(family, style, weight, stretch);
		}

		public void Apply(swd.TextRange range)
		{
			range.ApplyPropertyValue(swd.TextElement.FontFamilyProperty, Control.FontFamily);
			range.ApplyPropertyValue(swd.TextElement.FontStyleProperty, Control.Style);
			range.ApplyPropertyValue(swd.TextElement.FontStretchProperty, Control.Stretch);
			range.ApplyPropertyValue(swd.TextElement.FontWeightProperty, Control.Weight);
		}

		public string Name => _name ?? (_name = NameDictionaryExtensions.GetEnglishName(Control.FaceNames));

		public string LocalizedName => _localizedName ?? (_localizedName = NameDictionaryExtensions.GetDisplayName(Control.FaceNames));

		public FontStyle FontStyle => WpfConversions.Convert(Control.Style, Control.Weight);


		public bool IsSymbol => Control.TryGetGlyphTypeface(out var glyph) && glyph.Symbol;

		public FontFamily Family { get; private set; }

		public bool HasCharacterRanges(IEnumerable<Range<int>> ranges)
		{
			if (Control.TryGetGlyphTypeface(out var glyph))
			{
				foreach (var range in ranges)
				{
					for (int i = range.Start; i <= range.End; i++)
					{
						if (!glyph.CharacterToGlyphMap.ContainsKey(i)) return false;
					}
				}
			}

			return true;
		}
		
		internal static string CreateTempDirectoryForFonts()
		{
			var dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
			if (!Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
			}
			return dir;
		}

		string tempFontDirectory;

		public void Create(Stream stream)
		{
			tempFontDirectory = CreateTempDirectoryForFonts();
			// assume everything is an otf so WPF picks it up..  We should detect the format somehow. :/
			var fileName = Path.Combine(tempFontDirectory, Guid.NewGuid().ToString() + ".otf");

			// Ideally we'd load this from memory somehow like you can when you compile resources
			// however those type of resources can't be added dynamically as far as I can tell.
			// However, apparently there's a huge memory leak when doing so anyway, 
			// so we create a file instead.
			// https://stackoverflow.com/questions/31452443/wpf-textblock-memory-leak-when-using-font
			var fileStream = File.Create(fileName);
			stream.CopyTo(fileStream);
			fileStream.Close();

			Create(fileName);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (tempFontDirectory != null && Directory.Exists(tempFontDirectory))
			{
				try
				{
					Directory.Delete(tempFontDirectory);
				}
				catch
				{
				}
				tempFontDirectory = null;
			}
		}

		public void Create(string fileName)
		{
			var fontInfo = OpenTypeFontInfo.FromFile(fileName).Single();
			string familyName = null;
			string faceName = null;
			if (fontInfo != null)
			{
				familyName = fontInfo.TypographicFamilyName ?? fontInfo.FamilyName;
				faceName = fontInfo.TypographicSubFamilyName ?? fontInfo.SubFamilyName;
			}
			else
			{
				// do we need this??
				// get the font family name using System.Drawing.. 
				var fontCollection = new System.Drawing.Text.PrivateFontCollection();
				fontCollection.AddFontFile(fileName);
				var families = fontCollection.Families;
				if (families.Length == 0)
					throw new ArgumentOutOfRangeException(nameof(fileName), "Could not load font from file");
				familyName = families[0].Name;
			}

			// is there no way to use fonts other than from a file system?!? ugh.
			var path = Path.GetDirectoryName(fileName);
			var name = $"file:///{path.Replace("\\", "/")}/#{familyName}";
			var wpffamily = new swm.FontFamily(name);
			var familyHandler = new FontFamilyHandler(wpffamily);
			familyHandler.SetTypefaces(new[] { Widget });
			Family = new FontFamily(familyHandler);
			if (faceName != null)
				Control = wpffamily.GetTypefaces().First(r => NameDictionaryExtensions.GetEnglishName(r.FaceNames) == faceName);
			else
				Control = wpffamily.GetTypefaces().First();
		}

		public void Create(FontFamily family)
		{
			Family = family;
		}
	}
}
