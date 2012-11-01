using System;

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
		/// </summary>
		Italic = 1 << 1
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
		/// Gets a value indicating that this font has a bold style
		/// </summary>
		bool Bold { get; }

		/// <summary>
		/// Gets a value indicating that this font has an italic style
		/// </summary>
		bool Italic { get; }

		/// <summary>
		/// Gets the size of the font in points
		/// </summary>
		float Size { get; }
	}
	
	/// <summary>
	/// Defines a format for text
	/// </summary>
	public class Font : InstanceWidget
	{
		IFont inner;

		/// <summary>
		/// Creates a new instance of the Font class with a specified <paramref name="family"/>, <paramref name="size"/>, and <paramref name="style"/>
		/// </summary>
		/// <param name="family">Family of font to use</param>
		/// <param name="size">Size of the font, in points</param>
		/// <param name="style">Style of the font</param>
		public Font(FontFamily family, float size, FontStyle style = FontStyle.Normal)
			: this(Generator.Current, family, size, style)
		{
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
			inner.Create(family, size, style);
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
		public Font(SystemFont systemFont, float? size = null)
			: this(Generator.Current, systemFont, size)
		{
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
		}
		
		/// <summary>
		/// Gets the size, in points, of this font
		/// </summary>
		public float Size
		{
			get { return inner.Size; }
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
	}
}
