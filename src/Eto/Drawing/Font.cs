using System;
using System.Globalization;
using sc = System.ComponentModel;

namespace Eto.Drawing
{
	/// <summary>
	/// Enumeration of the different system fonts for a <see cref="Font"/>
	/// </summary>
	/// <remarks>
	/// This is useful when you want to use a font that is the same as standard UI elements.
	/// </remarks>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
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
		Label,
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
		StatusBar,
		/// <summary>
		/// Default font for text that a user can typically change
		/// </summary>
		/// <remarks>
		/// On macOS, the system font isn't normally a font that the user would select or use, other than for user interface elements.
		/// </remarks>
		User
	}

	/// <summary>
	/// Syles for a <see cref="Font"/>
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[Flags]
	public enum FontStyle
	{
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
	}

	/// <summary>
	/// Decorations for a <see cref="Font"/>
	/// </summary>
	/// <remarks>
	/// These specify the different decorations to apply to a font, and are not related to the style.
	/// </remarks>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[Flags]
	public enum FontDecoration
	{
		/// <summary>
		/// No decorations
		/// </summary>
		None = 0,
		/// <summary>
		/// Underline font decoration
		/// </summary>
		Underline = 1 << 0,
		/// <summary>
		/// Strikethrough font decoration
		/// </summary>
		Strikethrough = 1 << 1,
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
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[sc.TypeConverter(typeof(FontConverter))]
	[Handler(typeof(Font.IHandler))]
	public class Font : Widget
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Creates a new instance of the Font class with a specified <paramref name="family"/>, <paramref name="size"/>, and <paramref name="style"/>
		/// </summary>
		/// <param name="family">Family of font to use</param>
		/// <param name="size">Size of the font, in points</param>
		/// <param name="style">Style of the font</param>
		/// <param name="decoration">Decorations to apply to the font</param>
		public Font(string family, float size, FontStyle style = FontStyle.None, FontDecoration decoration = FontDecoration.None)
		{
			Handler.Create(new FontFamily(family), size, style, decoration);
			Initialize();
		}

		/// <summary>
		/// Creates a new instance of the Font class with a specified <paramref name="family"/>, <paramref name="size"/>, and <paramref name="style"/>
		/// </summary>
		/// <param name="family">Family of font to use</param>
		/// <param name="size">Size of the font, in points</param>
		/// <param name="style">Style of the font</param>
		/// <param name="decoration">Decorations to apply to the font</param>
		public Font(FontFamily family, float size, FontStyle style = FontStyle.None, FontDecoration decoration = FontDecoration.None)
		{
			Handler.Create(family, size, style, decoration);
			Initialize();
		}

		/// <summary>
		/// Creates a new instance of the Font class with a specified <paramref name="systemFont"/> and optional custom <paramref name="size"/>
		/// </summary>
		/// <remarks>
		/// The system fonts are the same fonts that the standard UI of each platform use for particular areas
		/// given the <see cref="SystemFont"/> enumeration.
		/// </remarks>
		/// <param name="systemFont">Type of system font to create</param>
		/// <param name="size">Optional size of the font, in points. If not specified, the default size of the system font is used</param>
		/// <param name="decoration">Decorations to apply to the font</param>
		public Font(SystemFont systemFont, float? size = null, FontDecoration decoration = FontDecoration.None)
		{
			Handler.Create(systemFont, size, decoration);
			Initialize();
		}

		/// <summary>
		/// Initializes a new instance of the Font class with the specified <paramref name="typeface"/> and <paramref name="size"/>
		/// </summary>
		/// <param name="typeface">Typeface of the font to create</param>
		/// <param name="size">Size of the font in points</param>
		/// <param name="decoration">Decorations to apply to the font</param>
		public Font(FontTypeface typeface, float size, FontDecoration decoration = FontDecoration.None)
		{
			Handler.Create(typeface, size, decoration);
			Initialize();
		}

		/// <summary>
		/// Initializes a new instance of the Font class with the specified font <paramref name="handler"/>
		/// </summary>
		/// <remarks>
		/// Not intended to be used directly, this is used by each platform to pass back a font instance with a specific handler
		/// </remarks>
		/// <param name="handler">Handler for the font</param>
		public Font(IHandler handler)
			: base(handler)
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
		/// Gets the decorations applied to the font
		/// </summary>
		/// <remarks>
		/// Decorations can be applied to any typeface/style of font.
		/// </remarks>
		/// <value>The font decoration.</value>
		public FontDecoration FontDecoration
		{
			get { return Handler.FontDecoration; }
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

		/// <summary>
		/// Gets the height of the lower case 'x' character
		/// </summary>
		/// <value>The height of the x character</value>
		public float XHeight
		{
			get { return Handler.XHeight; }
		}

		/// <summary>
		/// Gets the top y co-ordinate from the baseline to the tallest character ascent
		/// </summary>
		/// <value>The tallest ascent of the font</value>
		public float Ascent
		{
			get { return Handler.Ascent; }
		}

		/// <summary>
		/// Gets the bottom y co-ordinate from the baseline to the longest character descent
		/// </summary>
		/// <value>The longest descent of the font</value>
		public float Descent
		{
			get { return Handler.Descent; }
		}

		/// <summary>
		/// Gets the height of a single line of the font
		/// </summary>
		/// <value>The height of a single line</value>
		public float LineHeight
		{
			get { return Handler.LineHeight; }
		}

		/// <summary>
		/// Gets the leading space between each line
		/// </summary>
		/// <value>The leading.</value>
		public float Leading
		{
			get { return Handler.Leading; }
		}

		/// <summary>
		/// Gets the offset of the baseline from the drawing point
		/// </summary>
		/// <value>The baseline offset from the drawing point</value>
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
			get { return FontStyle.HasFlag(FontStyle.Bold); }
		}

		/// <summary>
		/// Gets a value indicating that this font has an italic style
		/// </summary>
		public bool Italic
		{
			get { return FontStyle.HasFlag(FontStyle.Italic); }
		}

		/// <summary>
		/// Gets a value indicating that this font has an underline decoration
		/// </summary>
		public bool Underline
		{
			get { return FontDecoration.HasFlag(FontDecoration.Underline); }
		}

		/// <summary>
		/// Gets a value indicating that this font has a strikethrough decoration
		/// </summary>
		public bool Strikethrough
		{
			get { return FontDecoration.HasFlag(FontDecoration.Strikethrough); }
		}

		/// <summary>
		/// Measures the specified string to get its size in logical pixels.
		/// </summary>
		/// <remarks>
		/// This is equivalent to <see cref="Graphics.MeasureString"/>.  When you have a Graphics object, it is recommended
		/// to use that to measure the string if available, as it may be more efficient and take into account the current graphics state.
		/// </remarks>
		/// <seealso cref="Graphics.MeasureString"/>
		/// <returns>The size of the text in logical pixels if drawn using Graphics.DrawText.</returns>
		/// <param name="text">Text string to measure.</param>
		public SizeF MeasureString(string text)
		{
			return Handler.MeasureString(text);
		}

		/// <summary>
		/// Gets a string representation of the font object
		/// </summary>
		/// <returns>String representation of the font object</returns>
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Family={0}, Typeface={1}, Size={2}pt, Style={3}", Family, Typeface, Size, FontStyle);
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="Eto.Drawing.Font"/>.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="Eto.Drawing.Font"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current <see cref="Eto.Drawing.Font"/>;
		/// otherwise, <c>false</c>.</returns>
		public override bool Equals(object obj)
		{
			var font = obj as Font;

			return font != null
				&& object.ReferenceEquals(Platform, font.Platform)
				&& Family.Equals(font.Family)
				&& Size.Equals(font.Size)
				&& FontStyle.Equals(font.FontStyle);
		}

		/// <summary>
		/// Serves as a hash function for a <see cref="Eto.Drawing.Font"/> object.
		/// </summary>
		/// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.</returns>
		public override int GetHashCode()
		{
			return FamilyName.GetHashCode() ^ Platform.GetHashCode() ^ Size.GetHashCode() ^ FontStyle.GetHashCode();
		}

		#region Handler

		/// <summary>
		/// Platform handler for the <see cref="Font"/> class
		/// </summary>
		[AutoInitialize(false)]
		public new interface IHandler : Widget.IHandler
		{
			/// <summary>
			/// Creates a new font object
			/// </summary>
			/// <param name="family">Type of font family</param>
			/// <param name="size">Size of the font (in points)</param>
			/// <param name="style">Style of the font</param>
			/// <param name="decoration">Decorations to apply to the font</param>
			void Create(FontFamily family, float size, FontStyle style, FontDecoration decoration);

			/// <summary>
			/// Creates a new font object with the specified <paramref name="systemFont"/> and optional size
			/// </summary>
			/// <param name="systemFont">System font to create</param>
			/// <param name="size">Size of font to use, or null to use the system font's default size</param>
			/// <param name="decoration">Decorations to apply to the font</param>
			void Create(SystemFont systemFont, float? size, FontDecoration decoration);

			/// <summary>
			/// Creates a new font object with the specified <paramref name="typeface"/> and <paramref name="size"/>
			/// </summary>
			/// <param name="typeface">Typeface to specify the style (and family) of the font</param>
			/// <param name="size">Size of the font to create</param>
			/// <param name="decoration">Decorations to apply to the font</param>
			void Create(FontTypeface typeface, float size, FontDecoration decoration);

			/// <summary>
			/// Gets the height of the lower case 'x' character
			/// </summary>
			/// <value>The height of the x character</value>
			float XHeight { get; }

			/// <summary>
			/// Gets the top y co-ordinate from the baseline to the tallest character ascent
			/// </summary>
			/// <value>The tallest ascent of the font</value>
			float Ascent { get; }

			/// <summary>
			/// Gets the bottom y co-ordinate from the baseline to the longest character descent
			/// </summary>
			/// <value>The longest descent of the font</value>
			float Descent { get; }

			/// <summary>
			/// Gets the height of a single line of the font
			/// </summary>
			/// <value>The height of a single line</value>
			float LineHeight { get; }

			/// <summary>
			/// Gets the leading space between each line
			/// </summary>
			/// <value>The leading.</value>
			float Leading { get; }

			/// <summary>
			/// Gets the offset of the baseline from the drawing point
			/// </summary>
			/// <value>The baseline offset from the drawing point</value>
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
			/// This does not necessarily represent all of the style properties of the font. 
			/// Each <see cref="Typeface"/> has its own style relative to the font family.  This is meerely a
			/// convenience to get the common properties of a font's typeface style
			/// </remarks>
			FontStyle FontStyle { get; }

			/// <summary>
			/// Gets the decorations applied to the font
			/// </summary>
			/// <remarks>
			/// Decorations can be applied to any typeface/style of font.
			/// </remarks>
			/// <value>The font decoration.</value>
			FontDecoration FontDecoration { get; }

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

			/// <summary>
			/// Measures the specified string to get its size in logical pixels.
			/// </summary>
			/// <remarks>
			/// This is equivalent to <see cref="Graphics.MeasureString"/>.  When you have a Graphics object, it is recommended
			/// to use that to measure the string if available, as it may be more efficient and take into account the current graphics state.
			/// </remarks>
			/// <seealso cref="Graphics.MeasureString"/>
			/// <returns>The size of the text in logical pixels if drawn using Graphics.DrawText.</returns>
			/// <param name="text">Text string to measure.</param>
			SizeF MeasureString(string text);
		}

		#endregion
	}
}
