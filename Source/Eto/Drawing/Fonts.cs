using System;
using System.Collections.Generic;
using FontCacheKey = System.Tuple<Eto.Drawing.FontFamily, float, Eto.Drawing.FontStyle, Eto.Drawing.FontDecoration>;

namespace Eto.Drawing
{
	/// <summary>
	/// Methods to get information about current fonts installed the running system
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public static class Fonts
	{
		static readonly object cacheKey = new object();
		
		static Font GetFont (FontFamily family, float size, FontStyle style, FontDecoration decoration)
		{
			var cache = Platform.Instance.Cache<FontCacheKey, Font>(cacheKey);
			Font font;
			lock (cache) {
				var key = new FontCacheKey (family, size, style, decoration);
				if (!cache.TryGetValue (key, out font)) {
					font = new Font (family, size, style, decoration);
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
		public static Font Cached (string familyName, float size, FontStyle style = FontStyle.None, FontDecoration decoration = FontDecoration.None)
		{
			return GetFont (new FontFamily(familyName), size, style, decoration);
		}

		/// <summary>
		/// Gets a cached font
		/// </summary>
		/// <param name="family">Family of the font</param>
		/// <param name="size">Size in points of the font</param>
		/// <param name="style">Style of the font</param>
		/// <param name="decoration">Decorations to apply to the font</param>
		public static Font Cached (FontFamily family, float size, FontStyle style = FontStyle.None, FontDecoration decoration = FontDecoration.None)
		{
			return GetFont (family, size, style, decoration);
		}

		/// <summary>
		/// Clears the font cache
		/// </summary>
		/// <remarks>
		/// This is useful if you are using the <see cref="Cached(FontFamily,float,FontStyle,FontDecoration)"/> method to cache fonts and want to clear it
		/// to conserve memory or resources.
		/// </remarks>
		public static void ClearCache ()
		{
			var cache = Platform.Instance.Cache<FontCacheKey, Font>(cacheKey);
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
		public static Font Monospace (float size, FontStyle style = FontStyle.None, FontDecoration decoration = FontDecoration.None)
		{
			return GetFont (FontFamilies.Monospace, size, style, decoration);
		}

		/// <summary>
		/// Gets a font with the <see cref="FontFamilies.Sans"/> family and the specified size and style
		/// </summary>
		/// <param name="size">Size of the font</param>
		/// <param name="style">Style of the font</param>
		/// <param name="decoration">Decorations to apply to the font</param>
		public static Font Sans (float size, FontStyle style = FontStyle.None, FontDecoration decoration = FontDecoration.None)
		{
			return GetFont (FontFamilies.Sans, size, style, decoration);
		}

		/// <summary>
		/// Gets a font with the <see cref="FontFamilies.Serif"/> family and the specified size and style
		/// </summary>
		/// <param name="size">Size of the font</param>
		/// <param name="style">Style of the font</param>
		/// <param name="decoration">Decorations to apply to the font</param>
		public static Font Serif (float size, FontStyle style = FontStyle.None, FontDecoration decoration = FontDecoration.None)
		{
			return GetFont (FontFamilies.Serif, size, style, decoration);
		}

		/// <summary>
		/// Gets a font with the <see cref="FontFamilies.Cursive"/> family and the specified size and style
		/// </summary>
		/// <param name="size">Size of the font</param>
		/// <param name="style">Style of the font</param>
		/// <param name="decoration">Decorations to apply to the font</param>
		public static Font Cursive (float size, FontStyle style = FontStyle.None, FontDecoration decoration = FontDecoration.None)
		{
			return GetFont (FontFamilies.Cursive, size, style, decoration);
		}

		/// <summary>
		/// Gets a font with the <see cref="FontFamilies.Fantasy"/> family and the specified size and style
		/// </summary>
		/// <param name="size">Size of the font</param>
		/// <param name="style">Style of the font</param>
		/// <param name="decoration">Decorations to apply to the font</param>
		public static Font Fantasy (float size, FontStyle style = FontStyle.None, FontDecoration decoration = FontDecoration.None)
		{
			return GetFont (FontFamilies.Fantasy, size, style, decoration);
		}


		/// <summary>
		/// Gets an enumeration of available font families in the current system
		/// </summary>
		/// <returns>An enumeration of font family objects that this system supports</returns>
		public static IEnumerable<FontFamily> AvailableFontFamilies
		{
			get
			{
				var fonts = Platform.Instance.CreateShared<IHandler>();
				return fonts.AvailableFontFamilies;
			}
		}

		/// <summary>
		/// Platform handler interface for the <see cref="Fonts"/> class
		/// </summary>
		public interface IHandler
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
	}
}