using System.Windows;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Media;
namespace Eto.Wpf.CustomControls.FontDialog
{
    class FontFamilyListItem : TextBlock, IComparable
    {
        readonly string _displayName;

        public FontFamilyListItem(swm.FontFamily fontFamily)
        {
            _displayName = GetDisplayName(fontFamily);

            this.FontFamily = fontFamily;
            this.Text = _displayName;
            this.ToolTip = _displayName;

            // In the case of symbol font, apply the default message font to the text so it can be read.
            if (IsSymbolFont(fontFamily))
            {
                var range = new TextRange(ContentStart, ContentEnd);
                range.ApplyPropertyValue(TextBlock.FontFamilyProperty, sw.SystemFonts.MessageFontFamily);
            }
        }

        public override string ToString()
        {
            return _displayName;
        }

        int IComparable.CompareTo(object obj)
        {
            return string.Compare(_displayName, obj.ToString(), true, CultureInfo.CurrentCulture);
        }

        internal static bool IsSymbolFont(swm.FontFamily fontFamily)
        {
            foreach (swm.Typeface typeface in fontFamily.GetTypefaces())
            {
                swm.GlyphTypeface face;
                if (typeface.TryGetGlyphTypeface(out face))
                {
                    return face.Symbol;
                }
            }
            return false;
        }

        internal static string GetDisplayName(swm.FontFamily family)
        {
            return NameDictionaryExtensions.GetDisplayName(family.FamilyNames);
        }
    }
}
