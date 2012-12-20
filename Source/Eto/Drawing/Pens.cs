using System;
using System.Collections.Generic;
using PenKey = System.Tuple<System.UInt32, float, Eto.Generator>;

namespace Eto.Drawing
{
	/// <summary>
	/// List of pens with common colors and pen cache for pens with a specified color/thickness
	/// </summary>
	/// <copyright>(c) 2012 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public static class Pens
	{
		static object cacheKey = new object ();

		static IPen GetPen (Generator generator, Color color, float thickness = 1f)
		{
			var cache = generator.Cache<PenKey, IPen> (cacheKey);
			IPen pen;
			lock (cache) {
				var key = new PenKey (color.ToArgb (), thickness, generator);
				if (!cache.TryGetValue (key, out pen)) {
					pen = Pen.Create (color, thickness, generator);
					cache.Add (key, pen);
				}
			}
			return pen;
		}

		/// <summary>
		/// Gets a cached pen with the specified <paramref name="color"/> and <paramref name="thickness"/>
		/// </summary>
		/// <param name="color">Color of the cached pen to get</param>
		/// <param name="thickness">Thickness of the cached pen to get</param>
		/// <param name="generator">Generator to get the cached pen for</param>
		public static IPen Cached (Color color, float thickness = 1f, Generator generator = null)
		{
			return GetPen (generator, color, thickness);
		}

		/// <summary>
		/// Clears the pen cache
		/// </summary>
		/// <remarks>
		/// This is useful if you are using the <see cref="Cached"/> method to cache pens and want to clear it
		/// to conserve memory or resources.
		/// </remarks>
		/// <param name="generator">Generator to clear the pen cache for</param>
		public static void ClearCache (Generator generator = null)
		{
			var cache = generator.Cache<PenKey, IPen> (cacheKey);
			lock (cache) {
				cache.Clear ();
			}
		}

		// Red colors
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFCD5C5C</summary>
		public static IPen IndianRed (Generator generator = null)
		{
			return GetPen (generator, Colors.IndianRed);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFF08080</summary>
		public static IPen LightCoral (Generator generator = null)
		{
			return GetPen (generator, Colors.LightCoral);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFA8072</summary>
		public static IPen Salmon (Generator generator = null)
		{
			return GetPen (generator, Colors.Salmon);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFE9967A</summary>
		public static IPen DarkSalmon (Generator generator = null)
		{
			return GetPen (generator, Colors.DarkSalmon);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFFA07A</summary>
		public static IPen LightSalmon (Generator generator = null)
		{
			return GetPen (generator, Colors.LightSalmon);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFF0000</summary>
		public static IPen Red (Generator generator = null)
		{
			return GetPen (generator, Colors.Red);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFDC143C</summary>
		public static IPen Crimson (Generator generator = null)
		{
			return GetPen (generator, Colors.Crimson);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFB22222</summary>
		public static IPen FireBrick (Generator generator = null)
		{
			return GetPen (generator, Colors.FireBrick);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF8B0000</summary>
		public static IPen DarkRed (Generator generator = null)
		{
			return GetPen (generator, Colors.DarkRed);
		}

		// Pink colors
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFFC0CB</summary>
		public static IPen Pink (Generator generator = null)
		{
			return GetPen (generator, Colors.Pink);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFFB6C1</summary>
		public static IPen LightPink (Generator generator = null)
		{
			return GetPen (generator, Colors.LightPink);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFF69B4</summary>
		public static IPen HotPink (Generator generator = null)
		{
			return GetPen (generator, Colors.HotPink);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFF1493</summary>
		public static IPen DeepPink (Generator generator = null)
		{
			return GetPen (generator, Colors.DeepPink);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFC71585</summary>
		public static IPen MediumVioletRed (Generator generator = null)
		{
			return GetPen (generator, Colors.MediumVioletRed);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFDB7093</summary>
		public static IPen PaleVioletRed (Generator generator = null)
		{
			return GetPen (generator, Colors.PaleVioletRed);
		}

		// Orange colors
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFF7F50</summary>
		public static IPen Coral (Generator generator = null)
		{
			return GetPen (generator, Colors.Coral);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFF6347</summary>
		public static IPen Tomato (Generator generator = null)
		{
			return GetPen (generator, Colors.Tomato);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFF4500</summary>
		public static IPen OrangeRed (Generator generator = null)
		{
			return GetPen (generator, Colors.OrangeRed);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFF8C00</summary>
		public static IPen DarkOrange (Generator generator = null)
		{
			return GetPen (generator, Colors.DarkOrange);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFFA500</summary>
		public static IPen Orange (Generator generator = null)
		{
			return GetPen (generator, Colors.Orange);
		}

		// Yellow colors
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFFD700</summary>
		public static IPen Gold (Generator generator = null)
		{
			return GetPen (generator, Colors.Gold);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFFFF00</summary>
		public static IPen Yellow (Generator generator = null)
		{
			return GetPen (generator, Colors.Yellow);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFFFFE0</summary>
		public static IPen LightYellow (Generator generator = null)
		{
			return GetPen (generator, Colors.LightYellow);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFFFACD</summary>
		public static IPen LemonChiffon (Generator generator = null)
		{
			return GetPen (generator, Colors.LemonChiffon);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFAFAD2</summary>
		public static IPen LightGoldenrodYellow (Generator generator = null)
		{
			return GetPen (generator, Colors.LightGoldenrodYellow);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFFEFD5</summary>
		public static IPen PapayaWhip (Generator generator = null)
		{
			return GetPen (generator, Colors.PapayaWhip);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFFE4B5</summary>
		public static IPen Moccasin (Generator generator = null)
		{
			return GetPen (generator, Colors.Moccasin);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFFDAB9</summary>
		public static IPen PeachPuff (Generator generator = null)
		{
			return GetPen (generator, Colors.PeachPuff);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFEEE8AA</summary>
		public static IPen PaleGoldenrod (Generator generator = null)
		{
			return GetPen (generator, Colors.PaleGoldenrod);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFF0E68C</summary>
		public static IPen Khaki (Generator generator = null)
		{
			return GetPen (generator, Colors.Khaki);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFBDB76B</summary>
		public static IPen DarkKhaki (Generator generator = null)
		{
			return GetPen (generator, Colors.DarkKhaki);
		}

		// Purple colors
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFE6E6FA</summary>
		public static IPen Lavender (Generator generator = null)
		{
			return GetPen (generator, Colors.Lavender);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFD8BFD8</summary>
		public static IPen Thistle (Generator generator = null)
		{
			return GetPen (generator, Colors.Thistle);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFDDA0DD</summary>
		public static IPen Plum (Generator generator = null)
		{
			return GetPen (generator, Colors.Plum);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFEE82EE</summary>
		public static IPen Violet (Generator generator = null)
		{
			return GetPen (generator, Colors.Violet);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFDA70D6</summary>
		public static IPen Orchid (Generator generator = null)
		{
			return GetPen (generator, Colors.Orchid);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFF00FF</summary>
		public static IPen Fuchsia (Generator generator = null)
		{
			return GetPen (generator, Colors.Fuchsia);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFF00FF</summary>
		public static IPen Magenta (Generator generator = null)
		{
			return GetPen (generator, Colors.Magenta);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFBA55D3</summary>
		public static IPen MediumOrchid (Generator generator = null)
		{
			return GetPen (generator, Colors.MediumOrchid);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF9370DB</summary>
		public static IPen MediumPurple (Generator generator = null)
		{
			return GetPen (generator, Colors.MediumPurple);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF8A2BE2</summary>
		public static IPen BlueViolet (Generator generator = null)
		{
			return GetPen (generator, Colors.BlueViolet);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF9400D3</summary>
		public static IPen DarkViolet (Generator generator = null)
		{
			return GetPen (generator, Colors.DarkViolet);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF9932CC</summary>
		public static IPen DarkOrchid (Generator generator = null)
		{
			return GetPen (generator, Colors.DarkOrchid);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF8B008B</summary>
		public static IPen DarkMagenta (Generator generator = null)
		{
			return GetPen (generator, Colors.DarkMagenta);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF800080</summary>
		public static IPen Purple (Generator generator = null)
		{
			return GetPen (generator, Colors.Purple);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF4B0082</summary>
		public static IPen Indigo (Generator generator = null)
		{
			return GetPen (generator, Colors.Indigo);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF483D8B</summary>
		public static IPen DarkSlateBlue (Generator generator = null)
		{
			return GetPen (generator, Colors.DarkSlateBlue);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF6A5ACD</summary>
		public static IPen SlateBlue (Generator generator = null)
		{
			return GetPen (generator, Colors.SlateBlue);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF7B68EE</summary>
		public static IPen MediumSlateBlue (Generator generator = null)
		{
			return GetPen (generator, Colors.MediumSlateBlue);
		}

		// Green colors
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFADFF2F</summary>
		public static IPen GreenYellow (Generator generator = null)
		{
			return GetPen (generator, Colors.GreenYellow);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF7FFF00</summary>
		public static IPen Chartreuse (Generator generator = null)
		{
			return GetPen (generator, Colors.Chartreuse);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF7CFC00</summary>
		public static IPen LawnGreen (Generator generator = null)
		{
			return GetPen (generator, Colors.LawnGreen);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF00FF00</summary>
		public static IPen Lime (Generator generator = null)
		{
			return GetPen (generator, Colors.Lime);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF32CD32</summary>
		public static IPen LimeGreen (Generator generator = null)
		{
			return GetPen (generator, Colors.LimeGreen);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF98FB98</summary>
		public static IPen PaleGreen (Generator generator = null)
		{
			return GetPen (generator, Colors.PaleGreen);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF90EE90</summary>
		public static IPen LightGreen (Generator generator = null)
		{
			return GetPen (generator, Colors.LightGreen);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF00FA9A</summary>
		public static IPen MediumSpringGreen (Generator generator = null)
		{
			return GetPen (generator, Colors.MediumSpringGreen);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF00FF7F</summary>
		public static IPen SpringGreen (Generator generator = null)
		{
			return GetPen (generator, Colors.SpringGreen);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF3CB371</summary>
		public static IPen MediumSeaGreen (Generator generator = null)
		{
			return GetPen (generator, Colors.MediumSeaGreen);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF2E8B57</summary>
		public static IPen SeaGreen (Generator generator = null)
		{
			return GetPen (generator, Colors.SeaGreen);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF228B22</summary>
		public static IPen ForestGreen (Generator generator = null)
		{
			return GetPen (generator, Colors.ForestGreen);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF008000</summary>
		public static IPen Green (Generator generator = null)
		{
			return GetPen (generator, Colors.Green);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF006400</summary>
		public static IPen DarkGreen (Generator generator = null)
		{
			return GetPen (generator, Colors.DarkGreen);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF9ACD32</summary>
		public static IPen YellowGreen (Generator generator = null)
		{
			return GetPen (generator, Colors.YellowGreen);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF6B8E23</summary>
		public static IPen OliveDrab (Generator generator = null)
		{
			return GetPen (generator, Colors.OliveDrab);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF808000</summary>
		public static IPen Olive (Generator generator = null)
		{
			return GetPen (generator, Colors.Olive);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF556B2F</summary>
		public static IPen DarkOliveGreen (Generator generator = null)
		{
			return GetPen (generator, Colors.DarkOliveGreen);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF66CDAA</summary>
		public static IPen MediumAquamarine (Generator generator = null)
		{
			return GetPen (generator, Colors.MediumAquamarine);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF8FBC8F</summary>
		public static IPen DarkSeaGreen (Generator generator = null)
		{
			return GetPen (generator, Colors.DarkSeaGreen);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF20B2AA</summary>
		public static IPen LightSeaGreen (Generator generator = null)
		{
			return GetPen (generator, Colors.LightSeaGreen);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF008B8B</summary>
		public static IPen DarkCyan (Generator generator = null)
		{
			return GetPen (generator, Colors.DarkCyan);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF008080</summary>
		public static IPen Teal (Generator generator = null)
		{
			return GetPen (generator, Colors.Teal);
		}

		// Blue/Cyan colors
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF00FFFF</summary>
		public static IPen Aqua (Generator generator = null)
		{
			return GetPen (generator, Colors.Aqua);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF00FFFF</summary>
		public static IPen Cyan (Generator generator = null)
		{
			return GetPen (generator, Colors.Cyan);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFE0FFFF</summary>
		public static IPen LightCyan (Generator generator = null)
		{
			return GetPen (generator, Colors.LightCyan);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFAFEEEE</summary>
		public static IPen PaleTurquoise (Generator generator = null)
		{
			return GetPen (generator, Colors.PaleTurquoise);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF7FFFD4</summary>
		public static IPen Aquamarine (Generator generator = null)
		{
			return GetPen (generator, Colors.Aquamarine);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF40E0D0</summary>
		public static IPen Turquoise (Generator generator = null)
		{
			return GetPen (generator, Colors.Turquoise);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF48D1CC</summary>
		public static IPen MediumTurquoise (Generator generator = null)
		{
			return GetPen (generator, Colors.MediumTurquoise);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF00CED1</summary>
		public static IPen DarkTurquoise (Generator generator = null)
		{
			return GetPen (generator, Colors.DarkTurquoise);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF5F9EA0</summary>
		public static IPen CadetBlue (Generator generator = null)
		{
			return GetPen (generator, Colors.CadetBlue);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF4682B4</summary>
		public static IPen SteelBlue (Generator generator = null)
		{
			return GetPen (generator, Colors.SteelBlue);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFB0C4DE</summary>
		public static IPen LightSteelBlue (Generator generator = null)
		{
			return GetPen (generator, Colors.LightSteelBlue);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFB0E0E6</summary>
		public static IPen PowderBlue (Generator generator = null)
		{
			return GetPen (generator, Colors.PowderBlue);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFADD8E6</summary>
		public static IPen LightBlue (Generator generator = null)
		{
			return GetPen (generator, Colors.LightBlue);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF87CEEB</summary>
		public static IPen SkyBlue (Generator generator = null)
		{
			return GetPen (generator, Colors.SkyBlue);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF87CEFA</summary>
		public static IPen LightSkyBlue (Generator generator = null)
		{
			return GetPen (generator, Colors.LightSkyBlue);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF00BFFF</summary>
		public static IPen DeepSkyBlue (Generator generator = null)
		{
			return GetPen (generator, Colors.DeepSkyBlue);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF1E90FF</summary>
		public static IPen DodgerBlue (Generator generator = null)
		{
			return GetPen (generator, Colors.DodgerBlue);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF6495ED</summary>
		public static IPen CornflowerBlue (Generator generator = null)
		{
			return GetPen (generator, Colors.CornflowerBlue);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF4169E1</summary>
		public static IPen RoyalBlue (Generator generator = null)
		{
			return GetPen (generator, Colors.RoyalBlue);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF0000FF</summary>
		public static IPen Blue (Generator generator = null)
		{
			return GetPen (generator, Colors.Blue);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF0000CD</summary>
		public static IPen MediumBlue (Generator generator = null)
		{
			return GetPen (generator, Colors.MediumBlue);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF00008B</summary>
		public static IPen DarkBlue (Generator generator = null)
		{
			return GetPen (generator, Colors.DarkBlue);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF000080</summary>
		public static IPen Navy (Generator generator = null)
		{
			return GetPen (generator, Colors.Navy);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF191970</summary>
		public static IPen MidnightBlue (Generator generator = null)
		{
			return GetPen (generator, Colors.MidnightBlue);
		}

		// Brown colors
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFFF8DC</summary>
		public static IPen Cornsilk (Generator generator = null)
		{
			return GetPen (generator, Colors.Cornsilk);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFFEBCD</summary>
		public static IPen BlanchedAlmond (Generator generator = null)
		{
			return GetPen (generator, Colors.BlanchedAlmond);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFFE4C4</summary>
		public static IPen Bisque (Generator generator = null)
		{
			return GetPen (generator, Colors.Bisque);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFFDEAD</summary>
		public static IPen NavajoWhite (Generator generator = null)
		{
			return GetPen (generator, Colors.NavajoWhite);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFF5DEB3</summary>
		public static IPen Wheat (Generator generator = null)
		{
			return GetPen (generator, Colors.Wheat);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFDEB887</summary>
		public static IPen BurlyWood (Generator generator = null)
		{
			return GetPen (generator, Colors.BurlyWood);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFD2B48C</summary>
		public static IPen Tan (Generator generator = null)
		{
			return GetPen (generator, Colors.Tan);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFBC8F8F</summary>
		public static IPen RosyBrown (Generator generator = null)
		{
			return GetPen (generator, Colors.RosyBrown);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFF4A460</summary>
		public static IPen SandyBrown (Generator generator = null)
		{
			return GetPen (generator, Colors.SandyBrown);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFDAA520</summary>
		public static IPen Goldenrod (Generator generator = null)
		{
			return GetPen (generator, Colors.Goldenrod);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFB8860B</summary>
		public static IPen DarkGoldenrod (Generator generator = null)
		{
			return GetPen (generator, Colors.DarkGoldenrod);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFCD853F</summary>
		public static IPen Peru (Generator generator = null)
		{
			return GetPen (generator, Colors.Peru);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFD2691E</summary>
		public static IPen Chocolate (Generator generator = null)
		{
			return GetPen (generator, Colors.Chocolate);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF8B4513</summary>
		public static IPen SaddleBrown (Generator generator = null)
		{
			return GetPen (generator, Colors.SaddleBrown);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFA0522D</summary>
		public static IPen Sienna (Generator generator = null)
		{
			return GetPen (generator, Colors.Sienna);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFA52A2A</summary>
		public static IPen Brown (Generator generator = null)
		{
			return GetPen (generator, Colors.Brown);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF800000</summary>
		public static IPen Maroon (Generator generator = null)
		{
			return GetPen (generator, Colors.Maroon);
		}

		// White colors
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFFFFFF</summary>
		public static IPen White (Generator generator = null)
		{
			return GetPen (generator, Colors.White);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFFFAFA</summary>
		public static IPen Snow (Generator generator = null)
		{
			return GetPen (generator, Colors.Snow);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFF0FFF0</summary>
		public static IPen Honeydew (Generator generator = null)
		{
			return GetPen (generator, Colors.Honeydew);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFF5FFFA</summary>
		public static IPen MintCream (Generator generator = null)
		{
			return GetPen (generator, Colors.MintCream);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFF0FFFF</summary>
		public static IPen Azure (Generator generator = null)
		{
			return GetPen (generator, Colors.Azure);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFF0F8FF</summary>
		public static IPen AliceBlue (Generator generator = null)
		{
			return GetPen (generator, Colors.AliceBlue);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFF8F8FF</summary>
		public static IPen GhostWhite (Generator generator = null)
		{
			return GetPen (generator, Colors.GhostWhite);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFF5F5F5</summary>
		public static IPen WhiteSmoke (Generator generator = null)
		{
			return GetPen (generator, Colors.WhiteSmoke);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFFF5EE</summary>
		public static IPen Seashell (Generator generator = null)
		{
			return GetPen (generator, Colors.Seashell);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFF5F5DC</summary>
		public static IPen Beige (Generator generator = null)
		{
			return GetPen (generator, Colors.Beige);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFDF5E6</summary>
		public static IPen OldLace (Generator generator = null)
		{
			return GetPen (generator, Colors.OldLace);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFFFAF0</summary>
		public static IPen FloralWhite (Generator generator = null)
		{
			return GetPen (generator, Colors.FloralWhite);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFFFFF0</summary>
		public static IPen Ivory (Generator generator = null)
		{
			return GetPen (generator, Colors.Ivory);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFAEBD7</summary>
		public static IPen AntiqueWhite (Generator generator = null)
		{
			return GetPen (generator, Colors.AntiqueWhite);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFAF0E6</summary>
		public static IPen Linen (Generator generator = null)
		{
			return GetPen (generator, Colors.Linen);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFFF0F5</summary>
		public static IPen LavenderBlush (Generator generator = null)
		{
			return GetPen (generator, Colors.LavenderBlush);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFFE4E1</summary>
		public static IPen MistyRose (Generator generator = null)
		{
			return GetPen (generator, Colors.MistyRose);
		}

		// Gray colors
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFDCDCDC</summary>
		public static IPen Gainsboro (Generator generator = null)
		{
			return GetPen (generator, Colors.Gainsboro);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFD3D3D3</summary>
		public static IPen LightGrey (Generator generator = null)
		{
			return GetPen (generator, Colors.LightGrey);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFC0C0C0</summary>
		public static IPen Silver (Generator generator = null)
		{
			return GetPen (generator, Colors.Silver);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFA9A9A9</summary>
		public static IPen DarkGray (Generator generator = null)
		{
			return GetPen (generator, Colors.DarkGray);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF808080</summary>
		public static IPen Gray (Generator generator = null)
		{
			return GetPen (generator, Colors.Gray);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF696969</summary>
		public static IPen DimGray (Generator generator = null)
		{
			return GetPen (generator, Colors.DimGray);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF778899</summary>
		public static IPen LightSlateGray (Generator generator = null)
		{
			return GetPen (generator, Colors.LightSlateGray);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF708090</summary>
		public static IPen SlateGray (Generator generator = null)
		{
			return GetPen (generator, Colors.SlateGray);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF2F4F4F</summary>
		public static IPen DarkSlateGray (Generator generator = null)
		{
			return GetPen (generator, Colors.DarkSlateGray);
		}
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF000000</summary>
		public static IPen Black (Generator generator = null)
		{
			return GetPen (generator, Colors.Black);
		}
	}
}

