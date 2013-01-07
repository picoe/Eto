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
		/// </summary>
		Italic = 1 << 1,

        /// <summary>
        /// Underline font style
        /// </summary>
        Underline = 1 << 2,

        /// <summary>
        /// Strikeout font style
        /// </summary>
        Strikeout = 1 << 3,

	}
	
	/// <summary>
	/// Platform handler for the <see cref="Font"/> class
	/// </summary>
	public interface IFont : 
        IInstanceWidget
	{
        void Create();

		/// <summary>
		/// Creates a new font object
		/// </summary>
		/// <param name="family">Type of font family</param>
		/// <param name="sizeInPoints">Size of the font (in points)</param>
		/// <param name="style">Style of the font</param>
        void Create(FontFamily family, float sizeInPoints, FontStyle style);

		/// <summary>
		/// Creates a new font object with the specified <paramref name="systemFont"/> and optional size
		/// </summary>
		/// <param name="systemFont">System font to create</param>
		/// <param name="sizeInPoints">Size of font to use, or null to use the system font's default size</param>
		void Create(SystemFont systemFont, float? sizeInPoints);

		/// <summary>
		/// Creates a new font object with the specified <paramref name="typeface"/> and <paramref name="sizeInPoints"/>
		/// </summary>
		/// <param name="typeface">Typeface to specify the style (and family) of the font</param>
		/// <param name="sizeInPoints">Size of the font to create</param>
		void Create (FontTypeface typeface, float sizeInPoints);

        float XHeightInPixels { get; }
        
        float AscentInPixels { get; }
        
        float DescentInPixels { get; }
        
        float LineHeightInPixels { get; }

        /// <summary>
        /// Gets the size of the font in pixels
        /// </summary>
        float SizeInPixels { get; }

        /// <summary>
        /// Gets the size of the font in points
        /// </summary>
        float SizeInPoints { get; }
                
		/// <summary>
		/// Gets the name of the family of this font
		/// </summary>
		string FamilyName { get; }

		/// <summary>
		/// Gets the style flags for this font
		/// </summary>
		/// <remarks>
		/// This does not represent all of the style properties of the font. Each <see cref="Typeface"/>
		/// has its own style relative to the font family.
		/// </remarks>
		FontStyle FontStyle { get; }

		/// <summary>
		/// Gets the family information for this font
		/// </summary>
		/// <remarks>
		/// This should always return an instance that represents the family of this font
		/// </remarks>
		FontFamily Family { get; }

		/// <summary>
		/// Gets the typeface information for this font
		/// </summary>
		/// <remarks>
		/// This should always return an instance that represents the typeface of this font
		/// </remarks>
		FontTypeface Typeface { get; }
    }

	/// <summary>
	/// Defines a format for text
	/// </summary>
	/// <remarks>
	/// A font is typically defined with a specified font family, with a given typeface.  Each typeface has certain characteristics
	/// that define the variation of the font family, for example Bold, or Italic.
	/// 
	/// You can get a list of <see cref="FontFamily"/> objects available in the current system using
	/// <see cref="Fonts.AvailableFontFamilies"/>, which can then be used to create an instance of a font.
	/// </remarks>
    public class Font : InstanceWidget
    {
		new IFont Handler { get { return (IFont)base.Handler; } }


        public Font(IFont inner) :
            base(Generator.Current, inner)
        {
        }

        public Font() :
            base(Generator.Current, typeof(IFont))
        {
            Handler.Create();
        }

		/// <summary>
		/// Creates a new instance of the Font class with a specified <paramref name="family"/>, <paramref name="size"/>, and <paramref name="style"/>
		/// </summary>
		/// <param name="family">Family of font to use</param>
        /// <param name="sizeInPoints">Size of the font, in points</param>
		/// <param name="style">Style of the font</param>
        public Font(
            string fontFamily,
            float sizeInPoints,
            FontStyle style = FontStyle.Normal,
            Generator generator = null)
            : this(FontFamily.CreateWebFontFamily(fontFamily), sizeInPoints, style, generator)
        {
        }

		/// <summary>
		/// Creates a new instance of the Font class with a specified <paramref name="family"/>, <paramref name="sizeInPoints"/>, and <paramref name="style"/>
		/// </summary>
		/// <param name="generator">Generator to create the font for</param>
		/// <param name="family">Family of font to use</param>
		/// <param name="sizeInPoints">Size of the font, in points</param>
		/// <param name="style">Style of the font</param>
		public Font (
            Generator generator, 
            FontFamily family, 
            float sizeInPoints, 
            FontStyle style = FontStyle.Normal)
			: base(generator, typeof(IFont))
        {
			Handler.Create(family, sizeInPoints, style);
        }

        public Font(
            SystemFont systemFont,
            float? sizeInPoints = null,
            Generator generator = null)
            : base(generator ?? Generator.Current, typeof(IFont))
        {
            Handler.Create(systemFont, sizeInPoints);
        }


        public Font(
            FontFamily family, 
            float sizeInPoints, 
            FontStyle style = FontStyle.Normal, 
            Generator g = null)
            : base(g ?? Generator.Current, typeof(IFont))
		/// </remarks>
		/// <param name="systemFont">Type of system font to create</param>
        /// <param name="sizeInPoints">Optional size of the font, in points. If not specified, the default size of the system font is used</param>
        {
            Handler.Create(family, sizeInPoints, style);
        }

		/// <summary>
		/// Creates a new instance of the Font class with a specified <paramref name="systemFont"/> and optional custom <paramref name="sizeInPoints"/>
		/// </summary>
		/// <remarks>
		/// The system fonts are the same fonts that the standard UI of each platform use for particular areas
		/// given the <see cref="SystemFont"/> enumeration.
		/// </remarks>
		/// <param name="generator">Generator to create the font for</param>
		/// <param name="systemFont">Type of system font to create</param>
		/// <param name="sizeInPoints">Optional size of the font, in points. If not specified, the default size of the system font is used</param>
		public Font (Generator generator, SystemFont systemFont, float? sizeInPoints = null)
			: base(generator, typeof(IFont))
        {
			Handler.Create(systemFont, sizeInPoints);
		}

		/// <summary>
		/// Initializes a new instance of the Font class with the specified <paramref name="familyName"/>, <paramref name="sizeInPoints"/>, and <paramref name="style"/>
		/// </summary>
		/// <param name="generator">Generator to create the font handler</param>
		/// <param name="familyName">Name of the font family to create</param>
		/// <param name="sizeInPoints">Size of the font (in points)</param>
		/// <param name="style">Style to apply to the font (if available for that family)</param>
		public Font (Generator generator, string familyName, float sizeInPoints, FontStyle style = FontStyle.Normal)
			: base(generator, typeof(IFont))
		{
			Handler.Create(new FontFamily(generator, familyName), sizeInPoints, style);
		}

		/// <summary>
		/// Initializes a new instance of the Font class with the specified <paramref name="typeface"/> and <paramref name="sizeInPoints"/>
		/// </summary>
		/// <param name="typeface">Typeface of the font to create</param>
		/// <param name="sizeInPoints">Size of the font in points</param>
		public Font (FontTypeface typeface, float sizeInPoints)
			: this (null, typeface, sizeInPoints)
		{
		}

		/// <summary>
		/// Initializes a new instance of the Font class with the specified <paramref name="typeface"/> and <paramref name="sizeInPoints"/>
		/// </summary>
		/// <param name="generator">Generator to create the font handler</param>
		/// <param name="typeface">Typeface of the font to create</param>
		/// <param name="sizeInPoints">Size of the font in points</param>
		public Font (Generator generator, FontTypeface typeface, float sizeInPoints)
			: base (generator, typeof (IFont))
		{
			Handler.Create (typeface, sizeInPoints);
		}

		/// <summary>
		/// Initializes a new instance of the Font class with the specified font <paramref name="handler"/>
		/// </summary>
		/// <remarks>
		/// Not intended to be used directly, this is used by each platform to pass back a font instance with a specific handler
		/// </remarks>
		/// <param name="generator">Generator of the handler</param>
		/// <param name="handler">Handler for the font</param>
		public Font (Generator generator, IFont handler)
			: base (generator, handler, true)
		{
		}

		/// <summary>
		/// Gets the name of the family of this font
		/// </summary>
		public string FamilyName
		{
			get { return Handler.FamilyName; }
		}

		/// <summary>
		/// Gets the style flags for this font
		/// </summary>
		/// <remarks>
		/// This does not represent all of the style properties of the font. Each <see cref="Typeface"/>
		/// has its own style relative to the font family.
		/// </remarks>
		public FontStyle FontStyle
		{
			get { return Handler.FontStyle; }
		}

		/// <summary>
		/// Gets the family information for this font
		/// </summary>
		public FontFamily Family
		{
			get { return Handler.Family; }
		}

		/// <summary>
		/// Gets the typeface information for this font
		/// </summary>
		public FontTypeface Typeface
		{
			get { return Handler.Typeface; }
        }
        
        public float XHeightInPixels
        {
            get { return Handler.XHeightInPixels; }
        }

        public float AscentInPixels
        {
            get { return Handler.AscentInPixels; }
        }

        public float DescentInPixels
        {
            get { return Handler.DescentInPixels; }
        }

        public float HeightInPixels
        {
            get { return Handler.LineHeightInPixels; }
        }

        /// <summary>
        /// Gets the size, in points, of this font
        /// </summary>
        public float SizeInPoints
        {
            get { return Handler.SizeInPoints; }
        }

        public float SizeInPixels
        {
            get { return Handler.SizeInPixels; }            
        }

		/// <summary>
		/// Gets a value indicating that this font has a bold style
		/// </summary>
        public bool Bold
        {
			get { return FontStyle.HasFlag (FontStyle.Bold); }
        }

		/// <summary>
		/// Gets a value indicating that this font has an italic style
		/// </summary>
        public bool Italic
        {
			get { return FontStyle.HasFlag (FontStyle.Italic); }
		}

        /// <summary>
        /// Gets a value indicating that this font has an underline style
        /// </summary>
        public bool Underline
        {
            get { return FontStyle.HasFlag(FontStyle.Underline); }
        }

        /// <summary>
        /// Gets a value indicating that this font has a strikeout style
        /// </summary>
        public bool Strikeout
        {
            get { return FontStyle.HasFlag(FontStyle.Strikeout); }
        }

        /// <summary>
        /// Gets a string representation of the font object
        /// </summary>
        /// <returns>String representation of the font object</returns>
        public override string ToString()
        {
            return string.Format("Family={0}, Typeface={1}, Size={2}pt, Style={3}", Family, Typeface, SizeInPoints, FontStyle);
        }

        #region Equals

        public override bool Equals(object obj)
        {
            var n = obj as Font;

            return
                n != null &&
                // this allows fonts from different generators
                // to be used within a single app
                object.ReferenceEquals(this.Generator, n.Generator) &&
                this.Family.Equals(n.Family) &&
                this.SizeInPixels.Equals(n.SizeInPixels) &&
                this.FontStyle.Equals(n.FontStyle);
        }

        public override int GetHashCode()
        {
            int result =
                (this.Family != null
                 ? this.Family.GetHashCode()
                 : 0)^
                (this.Generator != null
                 ? this.Generator.GetHashCode()
                 : 0) ^
                this.SizeInPixels.GetHashCode() ^
                this.FontStyle.GetHashCode();

            return result;
        }

        #endregion

        #region Static Methods

        private static Dictionary<string, string> TranslatedFontNames =
            new Dictionary<string, string>()
            {
#if Windows
                //{ "serif", System.Drawing.FontFamily.GenericSerif.Name},
#endif
                { "sans-serif", "Arial"},
                { "cursive", "Comic Sans MS"},
                { "fantasy", "Impact"},
#if Windows
                //{ "monospace", System.Drawing.FontFamily.GenericMonospace.Name},
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
            FontStyle fontStyle,
            float fontSizePixels,
            ActualFontCache actualFontCache,
            Generator generator = null)
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
                    Font.TranslateFontFamily(
                        singleFontFamily);

                var fontKey =
                    new FontKey()
                    {
                        Generator =
                            generator,

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
                        FontCache.GetFont(
                            translatedFontFamily,
                            fontSizePixels,
                            fontStyle,
                            generator);

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
                    FontCache.GetFont(
                        Font.TranslateFontFamily(
                            "serif"),
                        Font.DefaultFontSizePixels,
                        FontStyle.Normal,
                        generator);

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
            Constants.PointsToPixels;

        #endregion
    }
}
