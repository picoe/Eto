using System;
using System.Collections.Generic;
using FontCacheKey = System.Tuple<Eto.Drawing.SystemFont, float?, Eto.Drawing.FontDecoration>;

namespace Eto.Drawing
{
	/// <summary>
	/// Methods to get information about current fonts installed the running system
	/// </summary>
	public static class SystemFonts
	{
		static readonly object cacheKey = new object();

		static Font GetFont(SystemFont systemFont, float? size, FontDecoration decoration, Generator generator)
		{
			var cache = generator.Cache<FontCacheKey, Font>(cacheKey);
			Font font;
			lock (cache)
			{
				var key = new FontCacheKey(systemFont, size, decoration);
				if (!cache.TryGetValue(key, out font))
				{
					font = new Font(systemFont, size, decoration, generator);
					cache.Add(key, font);
				}
			}
			return font;
		}
		// Cached
		/// <summary>
		/// Gets a cached font
		/// </summary>
		/// <param name="systemFont">System font to get</param>
		/// <param name="size">Size in points of the font</param>
		/// <param name="decoration">Decorations to apply to the font</param>
		/// <param name="generator">Generator to create the font</param>
		public static Font Cached(SystemFont systemFont, float? size = null, FontDecoration decoration = FontDecoration.None, Generator generator = null)
		{
			return GetFont(systemFont, size, decoration, generator);
		}

		/// <summary>
		/// Clears the font cache
		/// </summary>
		/// <remarks>
		/// This is useful if you are using the <see cref="Cached(SystemFont,float?,FontDecoration,Generator)"/> method to cache fonts and want to clear it
		/// to conserve memory or resources.
		/// </remarks>
		/// <param name="generator">Generator to clear the font cache for</param>
		public static void ClearCache(Generator generator = null)
		{
			var cache = generator.Cache<FontCacheKey, Font>(cacheKey);
			lock (cache)
			{
				cache.Clear();
			}
		}

		public static Font Bold(float? size = null, FontDecoration decoration = FontDecoration.None, Generator generator = null)
		{
			return GetFont(SystemFont.Bold, size, decoration, generator);
		}

		public static Font Default(float? size = null, FontDecoration decoration = FontDecoration.None, Generator generator = null)
		{
			return GetFont(SystemFont.Default, size, decoration, generator);
		}

		public static Font Label(float? size = null, FontDecoration decoration = FontDecoration.None, Generator generator = null)
		{
			return GetFont(SystemFont.Label, size, decoration, generator);
		}

		public static Font Menu(float? size = null, FontDecoration decoration = FontDecoration.None, Generator generator = null)
		{
			return GetFont(SystemFont.Menu, size, decoration, generator);
		}

		public static Font MenuBar(float? size = null, FontDecoration decoration = FontDecoration.None, Generator generator = null)
		{
			return GetFont(SystemFont.MenuBar, size, decoration, generator);
		}

		public static Font Message(float? size = null, FontDecoration decoration = FontDecoration.None, Generator generator = null)
		{
			return GetFont(SystemFont.Message, size, decoration, generator);
		}

		public static Font Palette(float? size = null, FontDecoration decoration = FontDecoration.None, Generator generator = null)
		{
			return GetFont(SystemFont.Palette, size, decoration, generator);
		}

		public static Font StatusBar(float? size = null, FontDecoration decoration = FontDecoration.None, Generator generator = null)
		{
			return GetFont(SystemFont.StatusBar, size, decoration, generator);
		}

		public static Font TitleBar(float? size = null, FontDecoration decoration = FontDecoration.None, Generator generator = null)
		{
			return GetFont(SystemFont.TitleBar, size, decoration, generator);
		}

		public static Font ToolTip(float? size = null, FontDecoration decoration = FontDecoration.None, Generator generator = null)
		{
			return GetFont(SystemFont.ToolTip, size, decoration, generator);
		}

	}
}

