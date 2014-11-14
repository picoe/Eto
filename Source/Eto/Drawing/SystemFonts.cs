using System;
using System.Collections.Generic;
using FontCacheKey = System.Tuple<Eto.Drawing.SystemFont, float?, Eto.Drawing.FontDecoration>;

namespace Eto.Drawing
{
	/// <summary>
	/// Methods to get information about current fonts installed the running system
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public static class SystemFonts
	{
		static readonly object cacheKey = new object();

		static Font GetFont(SystemFont systemFont, float? size, FontDecoration decoration)
		{
			var cache = Platform.Instance.Cache<FontCacheKey, Font>(cacheKey);
			Font font;
			lock (cache)
			{
				var key = new FontCacheKey(systemFont, size, decoration);
				if (!cache.TryGetValue(key, out font))
				{
					font = new Font(systemFont, size, decoration);
					cache.Add(key, font);
				}
			}
			return font;
		}

		/// <summary>
		/// Gets a cached font
		/// </summary>
		/// <param name="systemFont">System font to get</param>
		/// <param name="size">Size in points of the font</param>
		/// <param name="decoration">Decorations to apply to the font</param>
		public static Font Cached(SystemFont systemFont, float? size = null, FontDecoration decoration = FontDecoration.None)
		{
			return GetFont(systemFont, size, decoration);
		}

		/// <summary>
		/// Clears the font cache
		/// </summary>
		/// <remarks>
		/// This is useful if you are using the <see cref="Cached(SystemFont,float?,FontDecoration,Generator)"/> method to cache fonts and want to clear it
		/// to conserve memory or resources.
		/// </remarks>
		public static void ClearCache()
		{
			var cache = Platform.Instance.Cache<FontCacheKey, Font>(cacheKey);
			lock (cache)
			{
				cache.Clear();
			}
		}

		/// <summary>
		/// Gets the system bold font with optional specified <paramref name="size"/> and <paramref name="decoration"/>.
		/// </summary>
		/// <param name="size">Size for the font, or null for the default system font size.</param>
		/// <param name="decoration">Decorations to add to the font.</param>
		public static Font Bold(float? size = null, FontDecoration decoration = FontDecoration.None)
		{
			return GetFont(SystemFont.Bold, size, decoration);
		}

		/// <summary>
		/// Gets the system default font with optional specified <paramref name="size"/> and <paramref name="decoration"/>.
		/// </summary>
		/// <param name="size">Size for the font, or null for the default system font size.</param>
		/// <param name="decoration">Decorations to add to the font.</param>
		public static Font Default(float? size = null, FontDecoration decoration = FontDecoration.None)
		{
			return GetFont(SystemFont.Default, size, decoration);
		}

		/// <summary>
		/// Gets the system label font with optional specified <paramref name="size"/> and <paramref name="decoration"/>.
		/// </summary>
		/// <param name="size">Size for the font, or null for the default system font size.</param>
		/// <param name="decoration">Decorations to add to the font.</param>
		public static Font Label(float? size = null, FontDecoration decoration = FontDecoration.None)
		{
			return GetFont(SystemFont.Label, size, decoration);
		}

		/// <summary>
		/// Gets the system menu font with optional specified <paramref name="size"/> and <paramref name="decoration"/>.
		/// </summary>
		/// <param name="size">Size for the font, or null for the default system font size.</param>
		/// <param name="decoration">Decorations to add to the font.</param>
		public static Font Menu(float? size = null, FontDecoration decoration = FontDecoration.None)
		{
			return GetFont(SystemFont.Menu, size, decoration);
		}

		/// <summary>
		/// Gets the system menu bar font with optional specified <paramref name="size"/> and <paramref name="decoration"/>.
		/// </summary>
		/// <param name="size">Size for the font, or null for the default system font size.</param>
		/// <param name="decoration">Decorations to add to the font.</param>
		public static Font MenuBar(float? size = null, FontDecoration decoration = FontDecoration.None)
		{
			return GetFont(SystemFont.MenuBar, size, decoration);
		}

		/// <summary>
		/// Gets the system message box font with optional specified <paramref name="size"/> and <paramref name="decoration"/>.
		/// </summary>
		/// <param name="size">Size for the font, or null for the default system font size.</param>
		/// <param name="decoration">Decorations to add to the font.</param>
		public static Font Message(float? size = null, FontDecoration decoration = FontDecoration.None)
		{
			return GetFont(SystemFont.Message, size, decoration);
		}

		/// <summary>
		/// Gets the system palette font with optional specified <paramref name="size"/> and <paramref name="decoration"/>.
		/// </summary>
		/// <param name="size">Size for the font, or null for the default system font size.</param>
		/// <param name="decoration">Decorations to add to the font.</param>
		public static Font Palette(float? size = null, FontDecoration decoration = FontDecoration.None)
		{
			return GetFont(SystemFont.Palette, size, decoration);
		}

		/// <summary>
		/// Gets the system status bar font with optional specified <paramref name="size"/> and <paramref name="decoration"/>.
		/// </summary>
		/// <param name="size">Size for the font, or null for the default system font size.</param>
		/// <param name="decoration">Decorations to add to the font.</param>
		public static Font StatusBar(float? size = null, FontDecoration decoration = FontDecoration.None)
		{
			return GetFont(SystemFont.StatusBar, size, decoration);
		}

		/// <summary>
		/// Gets the system title bar font with optional specified <paramref name="size"/> and <paramref name="decoration"/>.
		/// </summary>
		/// <param name="size">Size for the font, or null for the default system font size.</param>
		/// <param name="decoration">Decorations to add to the font.</param>
		public static Font TitleBar(float? size = null, FontDecoration decoration = FontDecoration.None)
		{
			return GetFont(SystemFont.TitleBar, size, decoration);
		}

		/// <summary>
		/// Gets the system tooltip font with optional specified <paramref name="size"/> and <paramref name="decoration"/>.
		/// </summary>
		/// <param name="size">Size for the font, or null for the default system font size.</param>
		/// <param name="decoration">Decorations to add to the font.</param>
		public static Font ToolTip(float? size = null, FontDecoration decoration = FontDecoration.None)
		{
			return GetFont(SystemFont.ToolTip, size, decoration);
		}

		#pragma warning disable 612,618

		/// <summary>
		/// Gets a cached font
		/// </summary>
		/// <param name="systemFont">System font to get</param>
		/// <param name="size">Size in points of the font</param>
		/// <param name="decoration">Decorations to apply to the font</param>
		/// <param name="generator">Generator to create the font</param>
		[Obsolete("Use variation without generator instead")]
		public static Font Cached(SystemFont systemFont, float? size, FontDecoration decoration, Generator generator)
		{
			return GetFont(systemFont, size, decoration);
		}

		/// <summary>
		/// Clears the font cache
		/// </summary>
		/// <remarks>
		/// This is useful if you are using the <see cref="Cached(SystemFont,float?,FontDecoration,Generator)"/> method to cache fonts and want to clear it
		/// to conserve memory or resources.
		/// </remarks>
		/// <param name="generator">Generator to clear the font cache for</param>
		[Obsolete("Use variation without generator instead")]
		public static void ClearCache(Generator generator)
		{
			var cache = generator.Cache<FontCacheKey, Font>(cacheKey);
			lock (cache)
			{
				cache.Clear();
			}
		}

		/// <summary>
		/// Bold the specified size, decoration and generator.
		/// </summary>
		/// <param name="size">Size.</param>
		/// <param name="decoration">Decoration.</param>
		/// <param name="generator">Generator.</param>
		[Obsolete("Use variation without generator instead")]
		public static Font Bold(float? size, FontDecoration decoration, Generator generator)
		{
			return GetFont(SystemFont.Bold, size, decoration);
		}

		/// <summary>
		/// Default the specified size, decoration and generator.
		/// </summary>
		/// <param name="size">Size.</param>
		/// <param name="decoration">Decoration.</param>
		/// <param name="generator">Generator.</param>
		[Obsolete("Use variation without generator instead")]
		public static Font Default(float? size, FontDecoration decoration, Generator generator)
		{
			return GetFont(SystemFont.Default, size, decoration);
		}

		/// <summary>
		/// Label the specified size, decoration and generator.
		/// </summary>
		/// <param name="size">Size.</param>
		/// <param name="decoration">Decoration.</param>
		/// <param name="generator">Generator.</param>
		[Obsolete("Use variation without generator instead")]
		public static Font Label(float? size, FontDecoration decoration, Generator generator)
		{
			return GetFont(SystemFont.Label, size, decoration);
		}

		/// <summary>
		/// Menu the specified size, decoration and generator.
		/// </summary>
		/// <param name="size">Size.</param>
		/// <param name="decoration">Decoration.</param>
		/// <param name="generator">Generator.</param>
		[Obsolete("Use variation without generator instead")]
		public static Font Menu(float? size, FontDecoration decoration, Generator generator)
		{
			return GetFont(SystemFont.Menu, size, decoration);
		}

		/// <summary>
		/// Menus the bar.
		/// </summary>
		/// <returns>The bar.</returns>
		/// <param name="size">Size.</param>
		/// <param name="decoration">Decoration.</param>
		/// <param name="generator">Generator.</param>
		[Obsolete("Use variation without generator instead")]
		public static Font MenuBar(float? size, FontDecoration decoration, Generator generator)
		{
			return GetFont(SystemFont.MenuBar, size, decoration);
		}

		/// <summary>
		/// Message the specified size, decoration and generator.
		/// </summary>
		/// <param name="size">Size.</param>
		/// <param name="decoration">Decoration.</param>
		/// <param name="generator">Generator.</param>
		[Obsolete("Use variation without generator instead")]
		public static Font Message(float? size, FontDecoration decoration, Generator generator)
		{
			return GetFont(SystemFont.Message, size, decoration);
		}

		/// <summary>
		/// Palette the specified size, decoration and generator.
		/// </summary>
		/// <param name="size">Size.</param>
		/// <param name="decoration">Decoration.</param>
		/// <param name="generator">Generator.</param>
		[Obsolete("Use variation without generator instead")]
		public static Font Palette(float? size, FontDecoration decoration, Generator generator)
		{
			return GetFont(SystemFont.Palette, size, decoration);
		}

		/// <summary>
		/// Statuses the bar.
		/// </summary>
		/// <returns>The bar.</returns>
		/// <param name="size">Size.</param>
		/// <param name="decoration">Decoration.</param>
		/// <param name="generator">Generator.</param>
		[Obsolete("Use variation without generator instead")]
		public static Font StatusBar(float? size, FontDecoration decoration, Generator generator)
		{
			return GetFont(SystemFont.StatusBar, size, decoration);
		}

		/// <summary>
		/// Titles the bar.
		/// </summary>
		/// <returns>The bar.</returns>
		/// <param name="size">Size.</param>
		/// <param name="decoration">Decoration.</param>
		/// <param name="generator">Generator.</param>
		[Obsolete("Use variation without generator instead")]
		public static Font TitleBar(float? size, FontDecoration decoration, Generator generator)
		{
			return GetFont(SystemFont.TitleBar, size, decoration);
		}

		/// <summary>
		/// Tools the tip.
		/// </summary>
		/// <returns>The tip.</returns>
		/// <param name="size">Size.</param>
		/// <param name="decoration">Decoration.</param>
		/// <param name="generator">Generator.</param>
		[Obsolete("Use variation without generator instead")]
		public static Font ToolTip(float? size, FontDecoration decoration, Generator generator)
		{
			return GetFont(SystemFont.ToolTip, size, decoration);
		}

		#pragma warning restore 612,618
	}
}

