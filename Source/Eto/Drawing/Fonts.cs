using System;
using System.Collections.Generic;
using FontCacheKey = System.Tuple<Eto.Drawing.FontFamily, float, Eto.Drawing.FontStyle, Eto.Drawing.FontDecoration>;

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
		/// Gets a value indicating whether the specified font family is available in the system
		/// </summary>
		/// <remarks>
		/// This is used to allow for (relatively) efficient lookup of a font name when the user
		/// specifies a comma-separated list of families when creating a <see cref="FontFamily"/>
		/// or <see cref="Font"/>.
		/// </remarks>
		/// <returns><c>true</c>, if family available was available, <c>false</c> otherwise.</returns>
		/// <param name="fontFamily">Font family to determine if it is available</param>
		bool FontFamilyAvailable (string fontFamily);
	}

	/// <summary>
	/// Methods to get information about current fonts installed the running system
	/// </summary>
	public static class Fonts
	{
		static readonly object cacheKey = new object();
		
		static Font GetFont (FontFamily family, float size, FontStyle style, FontDecoration decoration, Generator generator)
		{
			var cache = generator.Cache<FontCacheKey, Font>(cacheKey);
			Font font;
			lock (cache) {
				var key = new FontCacheKey (family, size, style, decoration);
				if (!cache.TryGetValue (key, out font)) {
					font = new Font (family, size, style, decoration, generator);
					cache.Add (key, font);
				}
			}
			return font;
		}
		
		// Cached
		/// <summary>
		/// Gets a cached font
		/// </summary>
		/// <param name="familyName">Family name of the font</param>
		/// <param name="size">Size in points of the font</param>
		/// <param name="style">Style of the font</param>
		/// <param name="decoration">Decorations to apply to the font</param>
		/// <param name="generator">Generator to create the font</param>
		public static Font Cached (string familyName, float size, FontStyle style = FontStyle.None, FontDecoration decoration = FontDecoration.None, Generator generator = null)
		{
			return GetFont (new FontFamily(generator, familyName), size, style, decoration, generator);
		}

		/// <summary>
		/// Gets a cached font
		/// </summary>
		/// <param name="family">Family of the font</param>
		/// <param name="size">Size in points of the font</param>
		/// <param name="style">Style of the font</param>
		/// <param name="decoration">Decorations to apply to the font</param>
		/// <param name="generator">Generator to create the font</param>
		public static Font Cached (FontFamily family, float size, FontStyle style = FontStyle.None, FontDecoration decoration = FontDecoration.None, Generator generator = null)
		{
			return GetFont (family, size, style, decoration, generator);
		}

		/// <summary>
		/// Clears the font cache
		/// </summary>
		/// <remarks>
		/// This is useful if you are using the <see cref="Cached(FontFamily,float,FontStyle,FontDecoration,Generator)"/> method to cache fonts and want to clear it
		/// to conserve memory or resources.
		/// </remarks>
		/// <param name="generator">Generator to clear the font cache for</param>
		public static void ClearCache (Generator generator = null)
		{
			var cache = generator.Cache<FontCacheKey, Font>(cacheKey);
			lock (cache) {
				cache.Clear ();
			}
		}

		/// <summary>
		/// Gets a font with the <see cref="FontFamilies.Monospace"/> family and the specified size and style
		/// </summary>
		/// <param name="size">Size of the font</param>
		/// <param name="style">Style of the font</param>
		/// <param name="decoration">Decorations to apply to the font</param>
		/// <param name="generator">Generator to get the font</param>
		public static Font Monospace (float size, FontStyle style = FontStyle.None, FontDecoration decoration = FontDecoration.None, Generator generator = null)
		{
			return GetFont (FontFamilies.Monospace (generator), size, style, decoration, generator);
		}

		/// <summary>
		/// Gets a font with the <see cref="FontFamilies.Sans"/> family and the specified size and style
		/// </summary>
		/// <param name="size">Size of the font</param>
		/// <param name="style">Style of the font</param>
		/// <param name="decoration">Decorations to apply to the font</param>
		/// <param name="generator">Generator to get the font</param>
		public static Font Sans (float size, FontStyle style = FontStyle.None, FontDecoration decoration = FontDecoration.None, Generator generator = null)
		{
			return GetFont (FontFamilies.Sans (generator), size, style, decoration, generator);
		}

		/// <summary>
		/// Gets a font with the <see cref="FontFamilies.Serif"/> family and the specified size and style
		/// </summary>
		/// <param name="size">Size of the font</param>
		/// <param name="style">Style of the font</param>
		/// <param name="decoration">Decorations to apply to the font</param>
		/// <param name="generator">Generator to get the font</param>
		public static Font Serif (float size, FontStyle style = FontStyle.None, FontDecoration decoration = FontDecoration.None, Generator generator = null)
		{
			return GetFont (FontFamilies.Serif (generator), size, style, decoration, generator);
		}

		/// <summary>
		/// Gets a font with the <see cref="FontFamilies.Cursive"/> family and the specified size and style
		/// </summary>
		/// <param name="size">Size of the font</param>
		/// <param name="style">Style of the font</param>
		/// <param name="decoration">Decorations to apply to the font</param>
		/// <param name="generator">Generator to get the font</param>
		public static Font Cursive (float size, FontStyle style = FontStyle.None, FontDecoration decoration = FontDecoration.None, Generator generator = null)
		{
			return GetFont (FontFamilies.Cursive (generator), size, style, decoration, generator);
		}

		/// <summary>
		/// Gets a font with the <see cref="FontFamilies.Fantasy"/> family and the specified size and style
		/// </summary>
		/// <param name="size">Size of the font</param>
		/// <param name="style">Style of the font</param>
		/// <param name="decoration">Decorations to apply to the font</param>
		/// <param name="generator">Generator to get the font</param>
		public static Font Fantasy (float size, FontStyle style = FontStyle.None, FontDecoration decoration = FontDecoration.None, Generator generator = null)
		{
			return GetFont (FontFamilies.Fantasy (generator), size, style, decoration, generator);
		}


		/// <summary>
		/// Gets an enumeration of available font families in the current system
		/// </summary>
		/// <param name="generator">Generator to get the font families for</param>
		/// <returns>An enumeration of font family objects that this system supports</returns>
		public static IEnumerable<FontFamily> AvailableFontFamilies (Generator generator = null)
		{
			var fonts = generator.CreateShared<IFonts>();
			return fonts.AvailableFontFamilies;
		}
	}
}

