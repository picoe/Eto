using System;
using System.Collections.Generic;
using BrushKey = System.Tuple<System.UInt32, Eto.Generator>;

namespace Eto.Drawing
{
	/// <summary>
	/// List of brushes with common colors and brush cache for solid color brushes
	/// </summary>
	/// <copyright>(c) 2012 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public static class Brushes
	{
		static object cacheKey = new object();

		static IBrush GetBrush (Color color, Generator generator)
		{
			var cache = (generator ?? Generator.Current).Cache<BrushKey, IBrush>(cacheKey);
			IBrush brush;
			lock (cache) {
				var key = new BrushKey (color.ToArgb (), generator);
				if (!cache.TryGetValue (key, out brush)) {
					brush = SolidBrush.Create (color, generator);
					cache.Add (key, brush);
				}
			}
			return brush;
		}

		// Cached
		/// <summary>
		/// Gets a cached solid brush with the specified color
		/// </summary>
		/// <param name="color">Color of the cached solid brush to get</param>
		/// <param name="generator">Generator to get the brush for</param>
		static IBrush Cached (Color color, Generator generator = null) { return GetBrush (color, generator); }

		/// <summary>
		/// Clears the brush cache
		/// </summary>
		/// <remarks>
		/// This is useful if you are using the <see cref="Cached"/> method to cache brushes and want to clear it
		/// to conserve memory or resources.
		/// </remarks>
		/// <param name="generator">Generator to clear the brush cache for</param>
		static void ClearCache (Generator generator = null)
		{
			var cache = (generator ?? Generator.Current).Cache<BrushKey, IBrush>(cacheKey);
			lock (cache) {
				cache.Clear ();
			}
		}

		// Red colors
		/// <summary>Gets a color with an ARGB value of #FFCD5C5C</summary>
		public static IBrush IndianRed (Generator generator = null) { return GetBrush (Colors.IndianRed, generator); }
		/// <summary>Gets a color with an ARGB value of #FFF08080</summary>
		public static IBrush LightCoral(Generator generator = null) { return GetBrush (Colors.LightCoral, generator); }
		/// <summary>Gets a color with an ARGB value of #FFFA8072</summary>
		public static IBrush Salmon(Generator generator = null) { return GetBrush (Colors.Salmon, generator); }
		/// <summary>Gets a color with an ARGB value of #FFE9967A</summary>
		public static IBrush DarkSalmon(Generator generator = null) { return GetBrush (Colors.DarkSalmon, generator); }
		/// <summary>Gets a color with an ARGB value of #FFFFA07A</summary>
		public static IBrush LightSalmon(Generator generator = null) { return GetBrush (Colors.LightSalmon, generator); }
		/// <summary>Gets a color with an ARGB value of #FFFF0000</summary>
		public static IBrush Red(Generator generator = null) { return GetBrush (Colors.Red, generator); }
		/// <summary>Gets a color with an ARGB value of #FFDC143C</summary>
		public static IBrush Crimson(Generator generator = null) { return GetBrush (Colors.Crimson, generator); }
		/// <summary>Gets a color with an ARGB value of #FFB22222</summary>
		public static IBrush FireBrick(Generator generator = null) { return GetBrush (Colors.FireBrick, generator); }
		/// <summary>Gets a color with an ARGB value of #FF8B0000</summary>
		public static IBrush DarkRed(Generator generator = null) { return GetBrush (Colors.DarkRed, generator); }
		
		// Pink colors
		/// <summary>Gets a color with an ARGB value of #FFFFC0CB</summary>
		public static IBrush Pink(Generator generator = null) { return GetBrush (Colors.Pink, generator); }
		/// <summary>Gets a color with an ARGB value of #FFFFB6C1</summary>
		public static IBrush LightPink(Generator generator = null) { return GetBrush (Colors.LightPink, generator); }
		/// <summary>Gets a color with an ARGB value of #FFFF69B4</summary>
		public static IBrush HotPink(Generator generator = null) { return GetBrush (Colors.HotPink, generator); }
		/// <summary>Gets a color with an ARGB value of #FFFF1493</summary>
		public static IBrush DeepPink(Generator generator = null) { return GetBrush (Colors.DeepPink, generator); }
		/// <summary>Gets a color with an ARGB value of #FFC71585</summary>
		public static IBrush MediumVioletRed(Generator generator = null) { return GetBrush (Colors.MediumVioletRed, generator); }
		/// <summary>Gets a color with an ARGB value of #FFDB7093</summary>
		public static IBrush PaleVioletRed(Generator generator = null) { return GetBrush (Colors.PaleVioletRed, generator); }
		
		// Orange colors
		/// <summary>Gets a color with an ARGB value of #FFFF7F50</summary>
		public static IBrush Coral(Generator generator = null) { return GetBrush (Colors.Coral, generator); }
		/// <summary>Gets a color with an ARGB value of #FFFF6347</summary>
		public static IBrush Tomato(Generator generator = null) { return GetBrush (Colors.Tomato, generator); }
		/// <summary>Gets a color with an ARGB value of #FFFF4500</summary>
		public static IBrush OrangeRed(Generator generator = null) { return GetBrush (Colors.OrangeRed, generator); }
		/// <summary>Gets a color with an ARGB value of #FFFF8C00</summary>
		public static IBrush DarkOrange(Generator generator = null) { return GetBrush (Colors.DarkOrange, generator); }
		/// <summary>Gets a color with an ARGB value of #FFFFA500</summary>
		public static IBrush Orange(Generator generator = null) { return GetBrush (Colors.Orange, generator); }
		
		// Yellow colors
		/// <summary>Gets a color with an ARGB value of #FFFFD700</summary>
		public static IBrush Gold(Generator generator = null) { return GetBrush (Colors.Gold, generator); }
		/// <summary>Gets a color with an ARGB value of #FFFFFF00</summary>
		public static IBrush Yellow(Generator generator = null) { return GetBrush (Colors.Yellow, generator); }
		/// <summary>Gets a color with an ARGB value of #FFFFFFE0</summary>
		public static IBrush LightYellow(Generator generator = null) { return GetBrush (Colors.LightYellow, generator); }
		/// <summary>Gets a color with an ARGB value of #FFFFFACD</summary>
		public static IBrush LemonChiffon(Generator generator = null) { return GetBrush (Colors.LemonChiffon, generator); }
		/// <summary>Gets a color with an ARGB value of #FFFAFAD2</summary>
		public static IBrush LightGoldenrodYellow(Generator generator = null) { return GetBrush (Colors.LightGoldenrodYellow, generator); }
		/// <summary>Gets a color with an ARGB value of #FFFFEFD5</summary>
		public static IBrush PapayaWhip(Generator generator = null) { return GetBrush (Colors.PapayaWhip, generator); }
		/// <summary>Gets a color with an ARGB value of #FFFFE4B5</summary>
		public static IBrush Moccasin(Generator generator = null) { return GetBrush (Colors.Moccasin, generator); }
		/// <summary>Gets a color with an ARGB value of #FFFFDAB9</summary>
		public static IBrush PeachPuff(Generator generator = null) { return GetBrush (Colors.PeachPuff, generator); }
		/// <summary>Gets a color with an ARGB value of #FFEEE8AA</summary>
		public static IBrush PaleGoldenrod(Generator generator = null) { return GetBrush (Colors.PaleGoldenrod, generator); }
		/// <summary>Gets a color with an ARGB value of #FFF0E68C</summary>
		public static IBrush Khaki(Generator generator = null) { return GetBrush (Colors.Khaki, generator); }
		/// <summary>Gets a color with an ARGB value of #FFBDB76B</summary>
		public static IBrush DarkKhaki(Generator generator = null) { return GetBrush (Colors.DarkKhaki, generator); }
		
		// Purple colors
		/// <summary>Gets a color with an ARGB value of #FFE6E6FA</summary>
		public static IBrush Lavender(Generator generator = null) { return GetBrush (Colors.Lavender, generator); }
		/// <summary>Gets a color with an ARGB value of #FFD8BFD8</summary>
		public static IBrush Thistle(Generator generator = null) { return GetBrush (Colors.Thistle, generator); }
		/// <summary>Gets a color with an ARGB value of #FFDDA0DD</summary>
		public static IBrush Plum(Generator generator = null) { return GetBrush (Colors.Plum, generator); }
		/// <summary>Gets a color with an ARGB value of #FFEE82EE</summary>
		public static IBrush Violet(Generator generator = null) { return GetBrush (Colors.Violet, generator); }
		/// <summary>Gets a color with an ARGB value of #FFDA70D6</summary>
		public static IBrush Orchid(Generator generator = null) { return GetBrush (Colors.Orchid, generator); }
		/// <summary>Gets a color with an ARGB value of #FFFF00FF</summary>
		public static IBrush Fuchsia(Generator generator = null) { return GetBrush (Colors.Fuchsia, generator); }
		/// <summary>Gets a color with an ARGB value of #FFFF00FF</summary>
		public static IBrush Magenta(Generator generator = null) { return GetBrush (Colors.Magenta, generator); }
		/// <summary>Gets a color with an ARGB value of #FFBA55D3</summary>
		public static IBrush MediumOrchid(Generator generator = null) { return GetBrush (Colors.MediumOrchid, generator); }
		/// <summary>Gets a color with an ARGB value of #FF9370DB</summary>
		public static IBrush MediumPurple(Generator generator = null) { return GetBrush (Colors.MediumPurple, generator); }
		/// <summary>Gets a color with an ARGB value of #FF8A2BE2</summary>
		public static IBrush BlueViolet(Generator generator = null) { return GetBrush (Colors.BlueViolet, generator); }
		/// <summary>Gets a color with an ARGB value of #FF9400D3</summary>
		public static IBrush DarkViolet(Generator generator = null) { return GetBrush (Colors.DarkViolet, generator); }
		/// <summary>Gets a color with an ARGB value of #FF9932CC</summary>
		public static IBrush DarkOrchid(Generator generator = null) { return GetBrush (Colors.DarkOrchid, generator); }
		/// <summary>Gets a color with an ARGB value of #FF8B008B</summary>
		public static IBrush DarkMagenta(Generator generator = null) { return GetBrush (Colors.DarkMagenta, generator); }
		/// <summary>Gets a color with an ARGB value of #FF800080</summary>
		public static IBrush Purple(Generator generator = null) { return GetBrush (Colors.Purple, generator); }
		/// <summary>Gets a color with an ARGB value of #FF4B0082</summary>
		public static IBrush Indigo(Generator generator = null) { return GetBrush (Colors.Indigo, generator); }
		/// <summary>Gets a color with an ARGB value of #FF483D8B</summary>
		public static IBrush DarkSlateBlue(Generator generator = null) { return GetBrush (Colors.DarkSlateBlue, generator); }
		/// <summary>Gets a color with an ARGB value of #FF6A5ACD</summary>
		public static IBrush SlateBlue(Generator generator = null) { return GetBrush (Colors.SlateBlue, generator); }
		/// <summary>Gets a color with an ARGB value of #FF7B68EE</summary>
		public static IBrush MediumSlateBlue(Generator generator = null) { return GetBrush (Colors.MediumSlateBlue, generator); }
		
		// Green colors
		/// <summary>Gets a color with an ARGB value of #FFADFF2F</summary>
		public static IBrush GreenYellow(Generator generator = null) { return GetBrush (Colors.GreenYellow, generator); }
		/// <summary>Gets a color with an ARGB value of #FF7FFF00</summary>
		public static IBrush Chartreuse(Generator generator = null) { return GetBrush (Colors.Chartreuse, generator); }
		/// <summary>Gets a color with an ARGB value of #FF7CFC00</summary>
		public static IBrush LawnGreen(Generator generator = null) { return GetBrush (Colors.LawnGreen, generator); }
		/// <summary>Gets a color with an ARGB value of #FF00FF00</summary>
		public static IBrush Lime(Generator generator = null) { return GetBrush (Colors.Lime, generator); }
		/// <summary>Gets a color with an ARGB value of #FF32CD32</summary>
		public static IBrush LimeGreen(Generator generator = null) { return GetBrush (Colors.LimeGreen, generator); }
		/// <summary>Gets a color with an ARGB value of #FF98FB98</summary>
		public static IBrush PaleGreen(Generator generator = null) { return GetBrush (Colors.PaleGreen, generator); }
		/// <summary>Gets a color with an ARGB value of #FF90EE90</summary>
		public static IBrush LightGreen(Generator generator = null) { return GetBrush (Colors.LightGreen, generator); }
		/// <summary>Gets a color with an ARGB value of #FF00FA9A</summary>
		public static IBrush MediumSpringGreen(Generator generator = null) { return GetBrush (Colors.MediumSpringGreen, generator); }
		/// <summary>Gets a color with an ARGB value of #FF00FF7F</summary>
		public static IBrush SpringGreen(Generator generator = null) { return GetBrush (Colors.SpringGreen, generator); }
		/// <summary>Gets a color with an ARGB value of #FF3CB371</summary>
		public static IBrush MediumSeaGreen(Generator generator = null) { return GetBrush (Colors.MediumSeaGreen, generator); }
		/// <summary>Gets a color with an ARGB value of #FF2E8B57</summary>
		public static IBrush SeaGreen(Generator generator = null) { return GetBrush (Colors.SeaGreen, generator); }
		/// <summary>Gets a color with an ARGB value of #FF228B22</summary>
		public static IBrush ForestGreen(Generator generator = null) { return GetBrush (Colors.ForestGreen, generator); }
		/// <summary>Gets a color with an ARGB value of #FF008000</summary>
		public static IBrush Green(Generator generator = null) { return GetBrush (Colors.Green, generator); }
		/// <summary>Gets a color with an ARGB value of #FF006400</summary>
		public static IBrush DarkGreen(Generator generator = null) { return GetBrush (Colors.DarkGreen, generator); }
		/// <summary>Gets a color with an ARGB value of #FF9ACD32</summary>
		public static IBrush YellowGreen(Generator generator = null) { return GetBrush (Colors.YellowGreen, generator); }
		/// <summary>Gets a color with an ARGB value of #FF6B8E23</summary>
		public static IBrush OliveDrab(Generator generator = null) { return GetBrush (Colors.OliveDrab, generator); }
		/// <summary>Gets a color with an ARGB value of #FF808000</summary>
		public static IBrush Olive(Generator generator = null) { return GetBrush (Colors.Olive, generator); }
		/// <summary>Gets a color with an ARGB value of #FF556B2F</summary>
		public static IBrush DarkOliveGreen(Generator generator = null) { return GetBrush (Colors.DarkOliveGreen, generator); }
		/// <summary>Gets a color with an ARGB value of #FF66CDAA</summary>
		public static IBrush MediumAquamarine(Generator generator = null) { return GetBrush (Colors.MediumAquamarine, generator); }
		/// <summary>Gets a color with an ARGB value of #FF8FBC8F</summary>
		public static IBrush DarkSeaGreen(Generator generator = null) { return GetBrush (Colors.DarkSeaGreen, generator); }
		/// <summary>Gets a color with an ARGB value of #FF20B2AA</summary>
		public static IBrush LightSeaGreen(Generator generator = null) { return GetBrush (Colors.LightSeaGreen, generator); }
		/// <summary>Gets a color with an ARGB value of #FF008B8B</summary>
		public static IBrush DarkCyan(Generator generator = null) { return GetBrush (Colors.DarkCyan, generator); }
		/// <summary>Gets a color with an ARGB value of #FF008080</summary>
		public static IBrush Teal(Generator generator = null) { return GetBrush (Colors.Teal, generator); }
		
		// Blue/Cyan colors
		/// <summary>Gets a color with an ARGB value of #FF00FFFF</summary>
		public static IBrush Aqua(Generator generator = null) { return GetBrush (Colors.Aqua, generator); }
		/// <summary>Gets a color with an ARGB value of #FF00FFFF</summary>
		public static IBrush Cyan(Generator generator = null) { return GetBrush (Colors.Cyan, generator); }
		/// <summary>Gets a color with an ARGB value of #FFE0FFFF</summary>
		public static IBrush LightCyan(Generator generator = null) { return GetBrush (Colors.LightCyan, generator); }
		/// <summary>Gets a color with an ARGB value of #FFAFEEEE</summary>
		public static IBrush PaleTurquoise(Generator generator = null) { return GetBrush (Colors.PaleTurquoise, generator); }
		/// <summary>Gets a color with an ARGB value of #FF7FFFD4</summary>
		public static IBrush Aquamarine(Generator generator = null) { return GetBrush (Colors.Aquamarine, generator); }
		/// <summary>Gets a color with an ARGB value of #FF40E0D0</summary>
		public static IBrush Turquoise(Generator generator = null) { return GetBrush (Colors.Turquoise, generator); }
		/// <summary>Gets a color with an ARGB value of #FF48D1CC</summary>
		public static IBrush MediumTurquoise(Generator generator = null) { return GetBrush (Colors.MediumTurquoise, generator); }
		/// <summary>Gets a color with an ARGB value of #FF00CED1</summary>
		public static IBrush DarkTurquoise(Generator generator = null) { return GetBrush (Colors.DarkTurquoise, generator); }
		/// <summary>Gets a color with an ARGB value of #FF5F9EA0</summary>
		public static IBrush CadetBlue(Generator generator = null) { return GetBrush (Colors.CadetBlue, generator); }
		/// <summary>Gets a color with an ARGB value of #FF4682B4</summary>
		public static IBrush SteelBlue(Generator generator = null) { return GetBrush (Colors.SteelBlue, generator); }
		/// <summary>Gets a color with an ARGB value of #FFB0C4DE</summary>
		public static IBrush LightSteelBlue(Generator generator = null) { return GetBrush (Colors.LightSteelBlue, generator); }
		/// <summary>Gets a color with an ARGB value of #FFB0E0E6</summary>
		public static IBrush PowderBlue(Generator generator = null) { return GetBrush (Colors.PowderBlue, generator); }
		/// <summary>Gets a color with an ARGB value of #FFADD8E6</summary>
		public static IBrush LightBlue(Generator generator = null) { return GetBrush (Colors.LightBlue, generator); }
		/// <summary>Gets a color with an ARGB value of #FF87CEEB</summary>
		public static IBrush SkyBlue(Generator generator = null) { return GetBrush (Colors.SkyBlue, generator); }
		/// <summary>Gets a color with an ARGB value of #FF87CEFA</summary>
		public static IBrush LightSkyBlue(Generator generator = null) { return GetBrush (Colors.LightSkyBlue, generator); }
		/// <summary>Gets a color with an ARGB value of #FF00BFFF</summary>
		public static IBrush DeepSkyBlue(Generator generator = null) { return GetBrush (Colors.DeepSkyBlue, generator); }
		/// <summary>Gets a color with an ARGB value of #FF1E90FF</summary>
		public static IBrush DodgerBlue(Generator generator = null) { return GetBrush (Colors.DodgerBlue, generator); }
		/// <summary>Gets a color with an ARGB value of #FF6495ED</summary>
		public static IBrush CornflowerBlue(Generator generator = null) { return GetBrush (Colors.CornflowerBlue, generator); }
		/// <summary>Gets a color with an ARGB value of #FF4169E1</summary>
		public static IBrush RoyalBlue(Generator generator = null) { return GetBrush (Colors.RoyalBlue, generator); }
		/// <summary>Gets a color with an ARGB value of #FF0000FF</summary>
		public static IBrush Blue(Generator generator = null) { return GetBrush (Colors.Blue, generator); }
		/// <summary>Gets a color with an ARGB value of #FF0000CD</summary>
		public static IBrush MediumBlue(Generator generator = null) { return GetBrush (Colors.MediumBlue, generator); }
		/// <summary>Gets a color with an ARGB value of #FF00008B</summary>
		public static IBrush DarkBlue(Generator generator = null) { return GetBrush (Colors.DarkBlue, generator); }
		/// <summary>Gets a color with an ARGB value of #FF000080</summary>
		public static IBrush Navy(Generator generator = null) { return GetBrush (Colors.Navy, generator); }
		/// <summary>Gets a color with an ARGB value of #FF191970</summary>
		public static IBrush MidnightBlue(Generator generator = null) { return GetBrush (Colors.MidnightBlue, generator); }
		
		// Brown colors
		/// <summary>Gets a color with an ARGB value of #FFFFF8DC</summary>
		public static IBrush Cornsilk(Generator generator = null) { return GetBrush (Colors.Cornsilk, generator); }
		/// <summary>Gets a color with an ARGB value of #FFFFEBCD</summary>
		public static IBrush BlanchedAlmond(Generator generator = null) { return GetBrush (Colors.BlanchedAlmond, generator); }
		/// <summary>Gets a color with an ARGB value of #FFFFE4C4</summary>
		public static IBrush Bisque(Generator generator = null) { return GetBrush (Colors.Bisque, generator); }
		/// <summary>Gets a color with an ARGB value of #FFFFDEAD</summary>
		public static IBrush NavajoWhite(Generator generator = null) { return GetBrush (Colors.NavajoWhite, generator); }
		/// <summary>Gets a color with an ARGB value of #FFF5DEB3</summary>
		public static IBrush Wheat(Generator generator = null) { return GetBrush (Colors.Wheat, generator); }
		/// <summary>Gets a color with an ARGB value of #FFDEB887</summary>
		public static IBrush BurlyWood(Generator generator = null) { return GetBrush (Colors.BurlyWood, generator); }
		/// <summary>Gets a color with an ARGB value of #FFD2B48C</summary>
		public static IBrush Tan(Generator generator = null) { return GetBrush (Colors.Tan, generator); }
		/// <summary>Gets a color with an ARGB value of #FFBC8F8F</summary>
		public static IBrush RosyBrown(Generator generator = null) { return GetBrush (Colors.RosyBrown, generator); }
		/// <summary>Gets a color with an ARGB value of #FFF4A460</summary>
		public static IBrush SandyBrown(Generator generator = null) { return GetBrush (Colors.SandyBrown, generator); }
		/// <summary>Gets a color with an ARGB value of #FFDAA520</summary>
		public static IBrush Goldenrod(Generator generator = null) { return GetBrush (Colors.Goldenrod, generator); }
		/// <summary>Gets a color with an ARGB value of #FFB8860B</summary>
		public static IBrush DarkGoldenrod(Generator generator = null) { return GetBrush (Colors.DarkGoldenrod, generator); }
		/// <summary>Gets a color with an ARGB value of #FFCD853F</summary>
		public static IBrush Peru(Generator generator = null) { return GetBrush (Colors.Peru, generator); }
		/// <summary>Gets a color with an ARGB value of #FFD2691E</summary>
		public static IBrush Chocolate(Generator generator = null) { return GetBrush (Colors.Chocolate, generator); }
		/// <summary>Gets a color with an ARGB value of #FF8B4513</summary>
		public static IBrush SaddleBrown(Generator generator = null) { return GetBrush (Colors.SaddleBrown, generator); }
		/// <summary>Gets a color with an ARGB value of #FFA0522D</summary>
		public static IBrush Sienna(Generator generator = null) { return GetBrush (Colors.Sienna, generator); }
		/// <summary>Gets a color with an ARGB value of #FFA52A2A</summary>
		public static IBrush Brown(Generator generator = null) { return GetBrush (Colors.Brown, generator); }
		/// <summary>Gets a color with an ARGB value of #FF800000</summary>
		public static IBrush Maroon(Generator generator = null) { return GetBrush (Colors.Maroon, generator); }
		
		// White colors
		/// <summary>Gets a color with an ARGB value of #FFFFFFFF</summary>
		public static IBrush White(Generator generator = null) { return GetBrush (Colors.White, generator); }
		/// <summary>Gets a color with an ARGB value of #FFFFFAFA</summary>
		public static IBrush Snow(Generator generator = null) { return GetBrush (Colors.Snow, generator); }
		/// <summary>Gets a color with an ARGB value of #FFF0FFF0</summary>
		public static IBrush Honeydew(Generator generator = null) { return GetBrush (Colors.Honeydew, generator); }
		/// <summary>Gets a color with an ARGB value of #FFF5FFFA</summary>
		public static IBrush MintCream(Generator generator = null) { return GetBrush (Colors.MintCream, generator); }
		/// <summary>Gets a color with an ARGB value of #FFF0FFFF</summary>
		public static IBrush Azure(Generator generator = null) { return GetBrush (Colors.Azure, generator); }
		/// <summary>Gets a color with an ARGB value of #FFF0F8FF</summary>
		public static IBrush AliceBlue(Generator generator = null) { return GetBrush (Colors.AliceBlue, generator); }
		/// <summary>Gets a color with an ARGB value of #FFF8F8FF</summary>
		public static IBrush GhostWhite(Generator generator = null) { return GetBrush (Colors.GhostWhite, generator); }
		/// <summary>Gets a color with an ARGB value of #FFF5F5F5</summary>
		public static IBrush WhiteSmoke(Generator generator = null) { return GetBrush (Colors.WhiteSmoke, generator); }
		/// <summary>Gets a color with an ARGB value of #FFFFF5EE</summary>
		public static IBrush Seashell(Generator generator = null) { return GetBrush (Colors.Seashell, generator); }
		/// <summary>Gets a color with an ARGB value of #FFF5F5DC</summary>
		public static IBrush Beige(Generator generator = null) { return GetBrush (Colors.Beige, generator); }
		/// <summary>Gets a color with an ARGB value of #FFFDF5E6</summary>
		public static IBrush OldLace(Generator generator = null) { return GetBrush (Colors.OldLace, generator); }
		/// <summary>Gets a color with an ARGB value of #FFFFFAF0</summary>
		public static IBrush FloralWhite(Generator generator = null) { return GetBrush (Colors.FloralWhite, generator); }
		/// <summary>Gets a color with an ARGB value of #FFFFFFF0</summary>
		public static IBrush Ivory(Generator generator = null) { return GetBrush (Colors.Ivory, generator); }
		/// <summary>Gets a color with an ARGB value of #FFFAEBD7</summary>
		public static IBrush AntiqueWhite(Generator generator = null) { return GetBrush (Colors.AntiqueWhite, generator); }
		/// <summary>Gets a color with an ARGB value of #FFFAF0E6</summary>
		public static IBrush Linen(Generator generator = null) { return GetBrush (Colors.Linen, generator); }
		/// <summary>Gets a color with an ARGB value of #FFFFF0F5</summary>
		public static IBrush LavenderBlush(Generator generator = null) { return GetBrush (Colors.LavenderBlush, generator); }
		/// <summary>Gets a color with an ARGB value of #FFFFE4E1</summary>
		public static IBrush MistyRose(Generator generator = null) { return GetBrush (Colors.MistyRose, generator); }
		
		// Gray colors
		/// <summary>Gets a color with an ARGB value of #FFDCDCDC</summary>
		public static IBrush Gainsboro(Generator generator = null) { return GetBrush (Colors.Gainsboro, generator); }
		/// <summary>Gets a color with an ARGB value of #FFD3D3D3</summary>
		public static IBrush LightGrey(Generator generator = null) { return GetBrush (Colors.LightGrey, generator); }
		/// <summary>Gets a color with an ARGB value of #FFC0C0C0</summary>
		public static IBrush Silver(Generator generator = null) { return GetBrush (Colors.Silver, generator); }
		/// <summary>Gets a color with an ARGB value of #FFA9A9A9</summary>
		public static IBrush DarkGray(Generator generator = null) { return GetBrush (Colors.DarkGray, generator); }
		/// <summary>Gets a color with an ARGB value of #FF808080</summary>
		public static IBrush Gray(Generator generator = null) { return GetBrush (Colors.Gray, generator); }
		/// <summary>Gets a color with an ARGB value of #FF696969</summary>
		public static IBrush DimGray(Generator generator = null) { return GetBrush (Colors.DimGray, generator); }
		/// <summary>Gets a color with an ARGB value of #FF778899</summary>
		public static IBrush LightSlateGray(Generator generator = null) { return GetBrush (Colors.LightSlateGray, generator); }
		/// <summary>Gets a color with an ARGB value of #FF708090</summary>
		public static IBrush SlateGray(Generator generator = null) { return GetBrush (Colors.SlateGray, generator); }
		/// <summary>Gets a color with an ARGB value of #FF2F4F4F</summary>
		public static IBrush DarkSlateGray(Generator generator = null) { return GetBrush (Colors.DarkSlateGray, generator); }
		/// <summary>Gets a color with an ARGB value of #FF000000</summary>
		public static IBrush Black(Generator generator = null) { return GetBrush (Colors.Black, generator); }
	}
}

