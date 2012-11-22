using System;
using System.Collections.Generic;

namespace Eto.Drawing
{
	/// <summary>
	/// Platform handler interface for the <see cref="Fonts"/> class
	/// </summary>
	public interface IFonts : IWidget
	{
		/// <summary>
		/// Gets an enumeration of available font families in the current system
		/// </summary>
		IEnumerable<FontFamily> AvailableFontFamilies { get; }

		/// <summary>
		/// Gets a system font family, based on one of the constants in the <see cref="FontFamilies"/>
		/// </summary>
		/// <remarks>
		/// This differs from creating a font family using its <see cref="FontFamily(string)"/> constructor 
		/// in that this will create an appropriate monospace, sans-serif, or serif font given the current platform.
		/// 
		/// Each platform may return a different font family based on the specified <paramref name="systemFontFamily"/>
		/// </remarks>
		/// <param name="systemFontFamily">A system font family to get</param>
		/// <returns>A new instance of a system font family</returns>
		FontFamily GetSystemFontFamily (string systemFontFamily);
	}

	/// <summary>
	/// Methods to get information about current fonts installed the running system
	/// </summary>
	public static class Fonts
	{
		/// <summary>
		/// Gets an enumeration of available font families in the current system
		/// </summary>
		/// <param name="generator">Generator to get the font families for</param>
		/// <returns>An enumeration of font family objects that this system supports</returns>
		public static IEnumerable<FontFamily> AvailableFontFamilies (Generator generator = null)
		{
			generator = generator ?? Generator.Current;
			var fonts = generator.CreateHandler<IFonts>();
			return fonts.AvailableFontFamilies;
		}

		/// <summary>
		/// Gets a system font family, based on one of the constants in the <see cref="FontFamilies"/>
		/// </summary>
		/// <remarks>
		/// This differs from creating a font family using its <see cref="FontFamily(string)"/> constructor 
		/// in that this will create an appropriate monospace, sans-serif, or serif font given the current platform.
		/// 
		/// Each platform may return a different font family based on the specified <paramref name="systemFontFamily"/>
		/// </remarks>
		/// <param name="systemFontFamily">A system font family to get</param>
		/// <param name="generator">Generator to get the system font family for</param>
		/// <returns>A new instance of a system font family</returns>
		public static FontFamily GetSystemFontFamily (string systemFontFamily, Generator generator = null)
		{
			generator = generator ?? Generator.Current;
			var fonts = generator.CreateHandler<IFonts>();
			return fonts.GetSystemFontFamily(systemFontFamily);
		}
	}
}

