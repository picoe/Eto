using Eto.Drawing;
using System.Globalization;
using System.Linq;
using swm = System.Windows.Media;
using sw = System.Windows;
using swd = System.Windows.Documents;
using Eto.Wpf.CustomControls.FontDialog;
using System.Collections.Generic;
using Eto.Forms;

namespace Eto.Wpf.Drawing
{
	public class FontTypefaceHandler : WidgetHandler<swm.Typeface, FontTypeface>, FontTypeface.IHandler
	{
		string name;
		string localizedName;

		public FontTypefaceHandler (swm.Typeface type)
		{
			this.Control = type;
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

		public string Name => name ?? (name = NameDictionaryExtensions.GetEnglishName(Control.FaceNames));

		public string LocalizedName => localizedName ?? (localizedName = NameDictionaryExtensions.GetDisplayName(Control.FaceNames));

		public FontStyle FontStyle => WpfConversions.Convert (Control.Style, Control.Weight);


		public bool IsSymbol => Control.TryGetGlyphTypeface(out var glyph) && glyph.Symbol;

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

	}
}
