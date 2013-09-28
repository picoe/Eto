using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

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
		[Obsolete("Use None instead")]
		Normal = None,

		/// <summary>
		/// No extra font style applied
		/// </summary>
		None = 0,

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
	public interface IFont : IInstanceWidget
	{
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
		/// Creates a new font object with the specified <paramref name="typeface"/> and <paramref name="size"/>
		/// </summary>
		/// <param name="typeface">Typeface to specify the style (and family) of the font</param>
		/// <param name="size">Size of the font to create</param>
		void Create (FontTypeface typeface, float size);

		float XHeight { get; }
		
		float Ascent { get; }
		
		float Descent { get; }
		
		float LineHeight { get; }

		float Leading { get; }

		float Baseline { get; }

		/// <summary>
		/// Gets the size of the font in points
		/// </summary>
		float Size { get; }
		
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

		/// <summary>
		/// Creates a new instance of the Font class with a specified <paramref name="family"/>, <paramref name="size"/>, and <paramref name="style"/>
		/// </summary>
		/// <param name="family">Family of font to use</param>
		/// <param name="size">Size of the font, in points</param>
		/// <param name="style">Style of the font</param>
		/// <param name="generator">Generator to create the font for</param>
		public Font (string family, float size, FontStyle style = FontStyle.None, Generator generator = null)
			: base(generator, typeof(IFont))
		{
			Handler.Create (new FontFamily(family), size, style);
		}

		/// <summary>
		/// Creates a new instance of the Font class with a specified <paramref name="family"/>, <paramref name="size"/>, and <paramref name="style"/>
		/// </summary>
		/// <param name="generator">Generator to create the font for</param>
		/// <param name="family">Family of font to use</param>
		/// <param name="size">Size of the font, in points</param>
		/// <param name="style">Style of the font</param>
		public Font(FontFamily family, float size, FontStyle style = FontStyle.None, Generator generator = null)
			: base(generator, typeof(IFont))
		{
			Handler.Create(family, size, style);
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
		public Font(SystemFont systemFont, float? size = null, Generator generator = null)
			: base(generator, typeof(IFont))
		{
			Handler.Create(systemFont, size);
		}


		/// <summary>
		/// Initializes a new instance of the Font class with the specified <paramref name="typeface"/> and <paramref name="size"/>
		/// </summary>
		/// <param name="typeface">Typeface of the font to create</param>
		/// <param name="size">Size of the font in points</param>
		/// <param name="generator">Generator to create the font handler</param>
		public Font (FontTypeface typeface, float size, Generator generator = null)
			: base (generator, typeof (IFont))
		{
			Handler.Create (typeface, size);
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
		
		public float XHeight
		{
			get { return Handler.XHeight; }
		}
		
		public float Ascent
		{
			get { return Handler.Ascent; }
		}
		
		public float Descent
		{
			get { return Handler.Descent; }
		}
		
		public float LineHeight
		{
			get { return Handler.LineHeight; }
		}

		public float Leading
		{
			get { return Handler.Leading; }
		}

		public float Baseline
		{
			get { return Handler.Baseline; }
		}

		/// <summary>
		/// Gets the size, in points, of this font
		/// </summary>
		public float Size
		{
			get { return Handler.Size; }
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
			return string.Format(CultureInfo.InvariantCulture, "Family={0}, Typeface={1}, Size={2}pt, Style={3}", Family, Typeface, Size, FontStyle);
		}

		public override bool Equals(object obj)
		{
			var font = obj as Font;

			return font != null
				&& object.ReferenceEquals(this.Generator, font.Generator)
				&& this.Family.Equals(font.Family)
				&& this.Size.Equals (font.Size)
				&& this.FontStyle.Equals(font.FontStyle);
		}

		public override int GetHashCode()
		{
			return this.FamilyName.GetHashCode () ^ this.Generator.GetHashCode () ^ this.Size.GetHashCode () ^ this.FontStyle.GetHashCode ();
		}
	}
}
