using System;
using System.Collections.Generic;
using ActualFontCache =
    System.Collections.Generic.Dictionary<
        Eto.Drawing.FontKey,
        Eto.Drawing.Font>;
using System.Text;

namespace Eto.Drawing
{
	/// <summary>
	/// Enumeration of the standard Font Families for a <see cref="Font"/>
	/// </summary>
	public enum FontFamily
	{
		/// <summary>
		/// Font family with each character having the same width
		/// </summary>
		Monospace,

		/// <summary>
		/// Font family with no serifs (e.g. Arial, Helvetica, etc)
		/// </summary>
		Sans,

		/// <summary>
		/// Font family with serifs (e.g. Times New Roman, etc)
		/// </summary>
		Serif
	}
	
	/// <summary>
	/// Enumeration of the different system fonts for a <see cref="Font"/>
	/// </summary>
	/// <remarks>
	/// This is useful when you want to use a font that is the same as standard UI elements.
	/// </remarks>
	public enum SystemFont
	{
		/// <summary>
		/// Default system font
		/// </summary>
		Default,

		/// <summary>
		/// Default system font in BOLD
		/// </summary>
		Bold,

		/// <summary>
		/// Default label font
		/// </summary>
		Label
#if DESKTOP
		,

		/// <summary>
		/// Default title bar font (window title)
		/// </summary>
		TitleBar,

		/// <summary>
		/// Default tool top font
		/// </summary>
		ToolTip,

		/// <summary>
		/// Default menu bar font
		/// </summary>
		MenuBar,

		/// <summary>
		/// Default font for items in a menu
		/// </summary>
		Menu,

		/// <summary>
		/// Default font for message boxes
		/// </summary>
		Message,

		/// <summary>
		/// Default font for palette dialogs
		/// </summary>
		Palette,

		/// <summary>
		/// Default font for status bars
		/// </summary>
		StatusBar
#endif
	}
	
	/// <summary>
	/// Syles for a <see cref="Font"/>
	/// </summary>
	[Flags]
	public enum FontStyle
	{
		/// <summary>
		/// Normal font style that is neither bold or italic
		/// </summary>
		Normal = 0,

		/// <summary>
		/// Bold font style
		/// </summary>
		Bold = 1 << 0,

		/// <summary>
		/// Italic font style
		Italic = 1 << 1,
        Underline = 1 << 2,
        Strikeout = 1 << 3,
		/// </summary>
	}
	
	/// <summary>
	/// Platform handler for the <see cref="Font"/> class
	/// </summary>
	public interface IFont : IInstanceWidget
	{
        void Create(string fontFamily, float sizeInPoints, FontStyle style);
		/// <summary>
		/// Creates a new font object
		/// </summary>
		/// <param name="family">Type of font family</param>
		/// <param name="size">Size of the font (in points)</param>
		/// <param name="style">Style of the font</param>
        void Create(FontFamily family, float size, FontStyle style);

		/// <summary>
		/// Creates a new font object with the specified <paramref name="systemFont"/> and optional size
		/// </summary>
		/// <param name="systemFont">System font to create</param>
		/// <param name="size">Size of font to use, or null to use the system font's default size</param>
		void Create(SystemFont systemFont, float? size);

		/// <summary>
		/// Gets a value indicating that this font has a bold style
		/// </summary>
        bool Bold { get; }

		/// <summary>
		/// Gets a value indicating that this font has an italic style
		/// </summary>
        bool Italic { get; }
        bool Underline { get; }
        bool Strikeout { get; }
        float ExHeightInPixels { get; }
        float AscentInPixels { get; }
        float DescentInPixels { get; }
        float HeightInPixels { get; }
        float SizeInPixels { get; }
        float SizeInPoints { get; }
        string FontFamily { get; }
        IFont Clone();

        void Create(string fontFamily, float sizeInPoints);

        void Create();
    }

	/// <summary>
	/// Defines a format for text
	/// </summary>
    public class Font : InstanceWidget, ICloneable
    {
        IFont inner;

		/// <summary>
		/// Creates a new instance of the Font class with a specified <paramref name="family"/>, <paramref name="size"/>, and <paramref name="style"/>
		/// </summary>
		/// <param name="family">Family of font to use</param>
		/// <param name="size">Size of the font, in points</param>
		/// <param name="style">Style of the font</param>
        public string FontFamily 
        { 
            get { return inner.FontFamily; }            
        }

		/// <summary>
		/// Creates a new instance of the Font class with a specified <paramref name="family"/>, <paramref name="size"/>, and <paramref name="style"/>
		/// </summary>
		/// <param name="generator">Generator to create the font for</param>
		/// <param name="family">Family of font to use</param>
		/// <param name="size">Size of the font, in points</param>
		/// <param name="style">Style of the font</param>
		public Font (Generator generator, FontFamily family, float size, FontStyle style = FontStyle.Normal)
			: base(generator, typeof(IFont))
        {
            inner = (IFont)Handler;
            inner.Create(fontFamily, sizeInPoints, style);
            if (ControlObject == null)
                throw new InvalidOperationException();
        }

        public Font(
            FontFamily family, 
            float size, 
            FontStyle style = FontStyle.Normal, 
            Generator g = null)
            : base(g ?? Generator.Current, typeof(IFont))
		/// </remarks>
		/// <param name="systemFont">Type of system font to create</param>
		/// <param name="size">Optional size of the font, in points. If not specified, the default size of the system font is used</param>
        {
            inner = (IFont)Handler;
            inner.Create(family, size, style);
            if (ControlObject == null)
                throw new InvalidOperationException();
        }

		/// <summary>
		/// Creates a new instance of the Font class with a specified <paramref name="systemFont"/> and optional custom <paramref name="size"/>
		/// </summary>
		/// <remarks>
		/// The system fonts are the same fonts that the standard UI of each platform use for particular areas
		/// given the <see cref="SystemFont"/> enumeration.
		/// </remarks>
		/// <param name="generator">Generator to create the font for</param>
		/// <param name="systemFont">Type of system font to create</param>
		/// <param name="size">Optional size of the font, in points. If not specified, the default size of the system font is used</param>
		public Font (Generator generator, SystemFont systemFont, float? size = null)
			: base(generator, typeof(IFont))
        {
            inner = (IFont)Handler;
            inner.Create(systemFont, size);
            if (ControlObject == null)
                throw new InvalidOperationException();
        }

		/// <summary>
		/// Gets the size, in points, of this font
		/// </summary>
		public float Size
		{
			get { return inner.Size; }

        public Font(IFont inner) :
            base(Generator.Current, inner)
        {
            this.inner = inner;
            if (ControlObject == null)
                throw new InvalidOperationException();
        }

        public Font() :
            base(Generator.Current, typeof(IFont))
        {
            inner = (IFont)Handler;
            inner.Create();
            if (ControlObject == null)
                throw new InvalidOperationException();
        }

        public Font(string fontFamily, float sizeInPoints) :
            base(Generator.Current, typeof(IFont))
        {
            this.inner = (IFont)Handler;
            inner.Create(fontFamily, sizeInPoints);

            if (ControlObject == null)
                throw new InvalidOperationException();
        }
        
        public float ExHeightInPixels
        {
            get { return inner.ExHeightInPixels; }
        }

        public float AscentInPixels
        {
            get { return inner.AscentInPixels; }
        }

        public float DescentInPixels
        {
            get { return inner.DescentInPixels; }
        }

        public float HeightInPixels
        {
            get { return inner.HeightInPixels; }
        }

        public float SizeInPoints
        {
            get { return inner.SizeInPoints; }
        }

        public float SizeInPixels
        {
            get { return inner.SizeInPixels; }            
        }

		/// <summary>
		/// Gets a value indicating that this font has a bold style
		/// </summary>
        public bool Bold
        {
            get { return inner.Bold; }            
        }

		/// <summary>
		/// Gets a value indicating that this font has an italic style
		/// </summary>
        public bool Italic
        {
            get { return inner.Italic; }
        }

        public bool Underline
        {
            get { return inner.Underline; }
        }

        public bool Strikeout
        {
            get { return inner.Strikeout; }
        }


        public FontStyle FontStyle
        {
            get
            {
                var result =
                    0 |
                    (this.Bold ? FontStyle.Bold : 0) |
                    (this.Underline ? FontStyle.Underline : 0) |
                    (this.Italic ? FontStyle.Italic : 0) |
                    (this.Strikeout? FontStyle.Strikeout : 0);

                return result;
            }
        }

        public Font Clone()
        {
            var result =
                new Font(
                    this.FontFamily, this.SizeInPoints, this.FontStyle);

            return result;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        #region ToString

        // If true, ToString() returns just the
        // font family.
        public bool UseSimplifiedToString { get; set; }

        public override string ToString()
        {
            return
                UseSimplifiedToString
                ?
                    string.Format(
                        "{0}{1}{2}{3}",
                        FontFamily ?? "",
                        this.Bold ? " Bold" : "",
                        this.Italic ? " Italic" : "",
                        this.Underline ? " Underline" : "")
                : string.Format(
                    "{0} {1}pt{2}{3}{4}",
                    FontFamily ?? "",
                    SizeInPoints,
                    Bold ? " Bold" : "",
                    Italic ? " Italic" : "",
                    Underline ? " Underline" : "");
        }

        #endregion


        #region Equals

        public override bool Equals(object obj)
        {
            var n = obj as Font;

            return
                n != null &&
                string.Equals(this.FontFamily, n.FontFamily) &&
                this.SizeInPixels.Equals(n.SizeInPixels) &&
                this.Bold == n.Bold &&
                this.Italic == n.Italic &&
                this.Underline == n.Underline
                // Do not include the color in the equality check. 
                //&& this.Color.Equals(n.Color)
                ;
        }

        public override int GetHashCode()
        {
            int result =
                (this.FontFamily != null
                 ? this.FontFamily.GetHashCode()
                 : 0)^
                this.SizeInPixels.GetHashCode() ^
                this.Bold.GetHashCode() ^
                this.Italic.GetHashCode() ^
                this.Underline.GetHashCode()
                // ^ this.Color.GetHashCode()
                ;

            return result;
        }

        #endregion

        #region Static Methods

        private static Dictionary<string, string> TranslatedFontNames =
            new Dictionary<string, string>()
            {
#if Windows
                { "serif", System.Drawing.FontFamily.GenericSerif.Name},
#endif
                { "sans-serif", "Arial"},
                { "cursive", "Comic Sans MS"},
                { "fantasy", "Impact"},
#if Windows
                { "monospace", System.Drawing.FontFamily.GenericMonospace.Name},
#endif
            };

        public static string TranslateFontFamily(
            string fontFamily)
        {
            var result = fontFamily;

            if (!TranslatedFontNames.TryGetValue(
                fontFamily,
                out result))
                result = fontFamily;

            return result;
        }

        public static Font ComputeFont(
            string fontFamily,
            Drawing.FontStyle fontStyle,
            float fontSizePixels,
            ActualFontCache actualFontCache)
        {
            Font result = null;

            fontFamily = 
                string.IsNullOrEmpty(fontFamily) 
                ? "serif"
                : fontFamily;

            // Loop over a possible comma separated list of
            // font families
            var fontFamilies =
                fontFamily.Split(
                    new char[] { ',' },
                    StringSplitOptions.RemoveEmptyEntries);

            foreach (
                var singleFontFamily
                in fontFamilies)
            {
                var translatedFontFamily =
                    Eto.Drawing.Font.TranslateFontFamily(
                        singleFontFamily);

                var fontKey =
                    new FontKey()
                    {
                        FontFamily =
                            translatedFontFamily,

                        FontSizePixels =
                            fontSizePixels,

                        FontStyle =
                            fontStyle,
                    };

                if (actualFontCache == null ||
                    !actualFontCache.TryGetValue(
                        fontKey,
                        out result))
                {
                    result =
                        Eto.Drawing.FontCache.GetFont(
                            translatedFontFamily,
                            fontSizePixels,
                            fontStyle);

                    if (result != null &&
                        actualFontCache != null)
                    {
                        actualFontCache[fontKey]
                            = result;
                    }
                }

                if (result != null)
                    break;
            }

            /// Always use a default font so that
            /// This function never returns null
            if (result == null)
                result =
                    Eto.Drawing.FontCache.GetFont(
                        Eto.Drawing.Font.TranslateFontFamily(
                            "serif"),
                        Font.DefaultFontSizePixels,
                        Eto.Drawing.FontStyle.Normal);

            return result;
        }

        #endregion

        #region Constants

        /// <summary>
        /// Default font size in points. Change this value to modify the default font size.
        /// </summary>
        public const float DefaultFontSizePoints = 12f;

        public const float DefaultFontSizePixels =
            DefaultFontSizePoints *
            Eto.Drawing.Constants.PointsToPixels;

        #endregion
    }

    public class FontKey
    {
        public string FontFamily { get; set; }
        public float FontSizePixels { get; set; }
        public FontStyle FontStyle { get; set; }

        public override int GetHashCode()
        {
            return
                (this.FontFamily != null
                 ? this.FontFamily.GetHashCode()
                 : 0) ^
                this.FontSizePixels.GetHashCode() ^
                this.FontStyle.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var o = obj as FontKey;

            var result =
                o != null &&
                SafeEquals(this.FontFamily, o.FontFamily) &&
                this.FontSizePixels.Equals(o.FontSizePixels) &&
                this.FontStyle.Equals(o.FontStyle);

            return result;
        }

        private static bool SafeEquals(object a, object b)
        {
            bool result =
                a != null
                ? a.Equals(b)
                : b == null;   // two null objects are equal

            return result;
        }

        public override string ToString()
        {
            return string.Format(
                "{0} {1}px {2}",
                FontFamily ?? "",
                FontSizePixels,
                FontStyle);
        }
    }

    /// <summary>
    /// Currently used by the browser.
    /// </summary>
    public static class FontCache
    {
        private static Dictionary<FontKey, Font> fonts =
            new Dictionary<FontKey, Font>();

        public static Font GetFont(
            string fontFamily,
            float fontSizePixels,
            FontStyle fontStyle)
        {
            Font result = null;

            var key = new FontKey()
            {
                FontFamily = fontFamily,
                FontSizePixels = fontSizePixels,
                FontStyle = fontStyle
            };

            if (//Dbg.Assert(
                fonts != null //) 
                && !fonts.TryGetValue(key, out result))
            {
                result =
                    new Font(
                    fontFamily,
                    fontSizePixels / Constants.PointsToPixels, // convert to points
                    (FontStyle)(int)fontStyle);

                fonts[key] = result;
            }

            return result;
        }
    }
}
