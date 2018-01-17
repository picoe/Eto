using System;
using System.Collections.Generic;
using BrushKey = System.Tuple<System.Int32>;

namespace Eto.Drawing
{
	/// <summary>
	/// List of brushes with common colors and brush cache for solid color brushes
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public static class Brushes
	{
		static readonly object cacheKey = new object();

		static SolidBrush GetBrush (Color color)
		{
			var cache = Platform.Instance.Cache<BrushKey, SolidBrush>(cacheKey);
			SolidBrush brush;
			lock (cache) {
				var key = new BrushKey (color.ToArgb ());
				if (!cache.TryGetValue (key, out brush)) {
					brush = new SolidBrush (color);
					cache.Add (key, brush);
				}
			}
			return brush;
		}

		/// <summary>
		/// Gets a cached solid brush with the specified color
		/// </summary>
		/// <param name="color">Color of the cached solid brush to get</param>
		public static SolidBrush Cached(Color color) { return GetBrush(color); }

		/// <summary>
		/// Clears the brush cache
		/// </summary>
		/// <remarks>
		/// This is useful if you are using the <see cref="Cached(Color)"/> method to cache brushes and want to clear it
		/// to conserve memory or resources.
		/// </remarks>
		public static void ClearCache()
		{
			var cache = Platform.Instance.Cache<BrushKey, SolidBrush>(cacheKey);
			lock (cache) {
				cache.Clear ();
			}
		}

		/// <summary>Gets a solid brush with an ARGB value of #00000000</summary>
		public static SolidBrush Transparent { get { return GetBrush(Colors.Transparent); } }

		// Red colors
		/// <summary>Gets a solid brush with a color ARGB value of #FFCD5C5C</summary>
		public static SolidBrush IndianRed { get { return GetBrush(Colors.IndianRed); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFF08080</summary>
		public static SolidBrush LightCoral { get { return GetBrush(Colors.LightCoral); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFFA8072</summary>
		public static SolidBrush Salmon { get { return GetBrush(Colors.Salmon); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFE9967A</summary>
		public static SolidBrush DarkSalmon { get { return GetBrush(Colors.DarkSalmon); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFFFA07A</summary>
		public static SolidBrush LightSalmon { get { return GetBrush(Colors.LightSalmon); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFFF0000</summary>
		public static SolidBrush Red { get { return GetBrush(Colors.Red); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFDC143C</summary>
		public static SolidBrush Crimson { get { return GetBrush(Colors.Crimson); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFB22222</summary>
		public static SolidBrush Firebrick { get { return GetBrush(Colors.Firebrick); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF8B0000</summary>
		public static SolidBrush DarkRed { get { return GetBrush(Colors.DarkRed); } }
		
		// Pink colors
		/// <summary>Gets a solid brush with a color ARGB value of #FFFFC0CB</summary>
		public static SolidBrush Pink { get { return GetBrush(Colors.Pink); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFFFB6C1</summary>
		public static SolidBrush LightPink { get { return GetBrush(Colors.LightPink); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFFF69B4</summary>
		public static SolidBrush HotPink { get { return GetBrush(Colors.HotPink); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFFF1493</summary>
		public static SolidBrush DeepPink { get { return GetBrush(Colors.DeepPink); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFC71585</summary>
		public static SolidBrush MediumVioletRed { get { return GetBrush(Colors.MediumVioletRed); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFDB7093</summary>
		public static SolidBrush PaleVioletRed { get { return GetBrush(Colors.PaleVioletRed); } }
		
		// Orange colors
		/// <summary>Gets a solid brush with a color ARGB value of #FFFF7F50</summary>
		public static SolidBrush Coral { get { return GetBrush(Colors.Coral); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFFF6347</summary>
		public static SolidBrush Tomato { get { return GetBrush(Colors.Tomato); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFFF4500</summary>
		public static SolidBrush OrangeRed { get { return GetBrush(Colors.OrangeRed); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFFF8C00</summary>
		public static SolidBrush DarkOrange { get { return GetBrush(Colors.DarkOrange); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFFFA500</summary>
		public static SolidBrush Orange { get { return GetBrush(Colors.Orange); } }
		
		// Yellow colors
		/// <summary>Gets a solid brush with a color ARGB value of #FFFFD700</summary>
		public static SolidBrush Gold { get { return GetBrush(Colors.Gold); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFFFFF00</summary>
		public static SolidBrush Yellow { get { return GetBrush(Colors.Yellow); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFFFFFE0</summary>
		public static SolidBrush LightYellow { get { return GetBrush(Colors.LightYellow); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFFFFACD</summary>
		public static SolidBrush LemonChiffon { get { return GetBrush(Colors.LemonChiffon); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFFAFAD2</summary>
		public static SolidBrush LightGoldenrodYellow { get { return GetBrush(Colors.LightGoldenrodYellow); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFFFEFD5</summary>
		public static SolidBrush PapayaWhip { get { return GetBrush(Colors.PapayaWhip); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFFFE4B5</summary>
		public static SolidBrush Moccasin { get { return GetBrush(Colors.Moccasin); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFFFDAB9</summary>
		public static SolidBrush PeachPuff { get { return GetBrush(Colors.PeachPuff); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFEEE8AA</summary>
		public static SolidBrush PaleGoldenrod { get { return GetBrush(Colors.PaleGoldenrod); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFF0E68C</summary>
		public static SolidBrush Khaki { get { return GetBrush(Colors.Khaki); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFBDB76B</summary>
		public static SolidBrush DarkKhaki { get { return GetBrush(Colors.DarkKhaki); } }
		
		// Purple colors
		/// <summary>Gets a solid brush with a color ARGB value of #FFE6E6FA</summary>
		public static SolidBrush Lavender { get { return GetBrush(Colors.Lavender); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFD8BFD8</summary>
		public static SolidBrush Thistle { get { return GetBrush(Colors.Thistle); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFDDA0DD</summary>
		public static SolidBrush Plum { get { return GetBrush(Colors.Plum); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFEE82EE</summary>
		public static SolidBrush Violet { get { return GetBrush(Colors.Violet); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFDA70D6</summary>
		public static SolidBrush Orchid { get { return GetBrush(Colors.Orchid); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFFF00FF</summary>
		public static SolidBrush Fuchsia { get { return GetBrush(Colors.Fuchsia); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFFF00FF</summary>
		public static SolidBrush Magenta { get { return GetBrush(Colors.Magenta); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFBA55D3</summary>
		public static SolidBrush MediumOrchid { get { return GetBrush(Colors.MediumOrchid); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF9370DB</summary>
		public static SolidBrush MediumPurple { get { return GetBrush(Colors.MediumPurple); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF8A2BE2</summary>
		public static SolidBrush BlueViolet { get { return GetBrush(Colors.BlueViolet); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF9400D3</summary>
		public static SolidBrush DarkViolet { get { return GetBrush(Colors.DarkViolet); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF9932CC</summary>
		public static SolidBrush DarkOrchid { get { return GetBrush(Colors.DarkOrchid); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF8B008B</summary>
		public static SolidBrush DarkMagenta { get { return GetBrush(Colors.DarkMagenta); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF800080</summary>
		public static SolidBrush Purple { get { return GetBrush(Colors.Purple); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF4B0082</summary>
		public static SolidBrush Indigo { get { return GetBrush(Colors.Indigo); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF483D8B</summary>
		public static SolidBrush DarkSlateBlue { get { return GetBrush(Colors.DarkSlateBlue); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF6A5ACD</summary>
		public static SolidBrush SlateBlue { get { return GetBrush(Colors.SlateBlue); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF7B68EE</summary>
		public static SolidBrush MediumSlateBlue { get { return GetBrush(Colors.MediumSlateBlue); } }
		
		// Green colors
		/// <summary>Gets a solid brush with a color ARGB value of #FFADFF2F</summary>
		public static SolidBrush GreenYellow { get { return GetBrush(Colors.GreenYellow); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF7FFF00</summary>
		public static SolidBrush Chartreuse { get { return GetBrush(Colors.Chartreuse); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF7CFC00</summary>
		public static SolidBrush LawnGreen { get { return GetBrush(Colors.LawnGreen); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF00FF00</summary>
		public static SolidBrush Lime { get { return GetBrush(Colors.Lime); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF32CD32</summary>
		public static SolidBrush LimeGreen { get { return GetBrush(Colors.LimeGreen); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF98FB98</summary>
		public static SolidBrush PaleGreen { get { return GetBrush(Colors.PaleGreen); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF90EE90</summary>
		public static SolidBrush LightGreen { get { return GetBrush(Colors.LightGreen); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF00FA9A</summary>
		public static SolidBrush MediumSpringGreen { get { return GetBrush(Colors.MediumSpringGreen); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF00FF7F</summary>
		public static SolidBrush SpringGreen { get { return GetBrush(Colors.SpringGreen); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF3CB371</summary>
		public static SolidBrush MediumSeaGreen { get { return GetBrush(Colors.MediumSeaGreen); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF2E8B57</summary>
		public static SolidBrush SeaGreen { get { return GetBrush(Colors.SeaGreen); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF228B22</summary>
		public static SolidBrush ForestGreen { get { return GetBrush(Colors.ForestGreen); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF008000</summary>
		public static SolidBrush Green { get { return GetBrush(Colors.Green); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF006400</summary>
		public static SolidBrush DarkGreen { get { return GetBrush(Colors.DarkGreen); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF9ACD32</summary>
		public static SolidBrush YellowGreen { get { return GetBrush(Colors.YellowGreen); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF6B8E23</summary>
		public static SolidBrush OliveDrab { get { return GetBrush(Colors.OliveDrab); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF808000</summary>
		public static SolidBrush Olive { get { return GetBrush(Colors.Olive); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF556B2F</summary>
		public static SolidBrush DarkOliveGreen { get { return GetBrush(Colors.DarkOliveGreen); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF66CDAA</summary>
		public static SolidBrush MediumAquamarine { get { return GetBrush(Colors.MediumAquamarine); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF8FBC8F</summary>
		public static SolidBrush DarkSeaGreen { get { return GetBrush(Colors.DarkSeaGreen); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF20B2AA</summary>
		public static SolidBrush LightSeaGreen { get { return GetBrush(Colors.LightSeaGreen); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF008B8B</summary>
		public static SolidBrush DarkCyan { get { return GetBrush(Colors.DarkCyan); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF008080</summary>
		public static SolidBrush Teal { get { return GetBrush(Colors.Teal); } }
		
		// Blue/Cyan colors
		/// <summary>Gets a solid brush with a color ARGB value of #FF00FFFF</summary>
		public static SolidBrush Aqua { get { return GetBrush(Colors.Aqua); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF00FFFF</summary>
		public static SolidBrush Cyan { get { return GetBrush(Colors.Cyan); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFE0FFFF</summary>
		public static SolidBrush LightCyan { get { return GetBrush(Colors.LightCyan); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFAFEEEE</summary>
		public static SolidBrush PaleTurquoise { get { return GetBrush(Colors.PaleTurquoise); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF7FFFD4</summary>
		public static SolidBrush Aquamarine { get { return GetBrush(Colors.Aquamarine); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF40E0D0</summary>
		public static SolidBrush Turquoise { get { return GetBrush(Colors.Turquoise); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF48D1CC</summary>
		public static SolidBrush MediumTurquoise { get { return GetBrush(Colors.MediumTurquoise); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF00CED1</summary>
		public static SolidBrush DarkTurquoise { get { return GetBrush(Colors.DarkTurquoise); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF5F9EA0</summary>
		public static SolidBrush CadetBlue { get { return GetBrush(Colors.CadetBlue); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF4682B4</summary>
		public static SolidBrush SteelBlue { get { return GetBrush(Colors.SteelBlue); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFB0C4DE</summary>
		public static SolidBrush LightSteelBlue { get { return GetBrush(Colors.LightSteelBlue); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFB0E0E6</summary>
		public static SolidBrush PowderBlue { get { return GetBrush(Colors.PowderBlue); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFADD8E6</summary>
		public static SolidBrush LightBlue { get { return GetBrush(Colors.LightBlue); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF87CEEB</summary>
		public static SolidBrush SkyBlue { get { return GetBrush(Colors.SkyBlue); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF87CEFA</summary>
		public static SolidBrush LightSkyBlue { get { return GetBrush(Colors.LightSkyBlue); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF00BFFF</summary>
		public static SolidBrush DeepSkyBlue { get { return GetBrush(Colors.DeepSkyBlue); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF1E90FF</summary>
		public static SolidBrush DodgerBlue { get { return GetBrush(Colors.DodgerBlue); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF6495ED</summary>
		public static SolidBrush CornflowerBlue { get { return GetBrush(Colors.CornflowerBlue); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF4169E1</summary>
		public static SolidBrush RoyalBlue { get { return GetBrush(Colors.RoyalBlue); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF0000FF</summary>
		public static SolidBrush Blue { get { return GetBrush(Colors.Blue); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF0000CD</summary>
		public static SolidBrush MediumBlue { get { return GetBrush(Colors.MediumBlue); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF00008B</summary>
		public static SolidBrush DarkBlue { get { return GetBrush(Colors.DarkBlue); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF000080</summary>
		public static SolidBrush Navy { get { return GetBrush(Colors.Navy); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF191970</summary>
		public static SolidBrush MidnightBlue { get { return GetBrush(Colors.MidnightBlue); } }
		
		// Brown colors
		/// <summary>Gets a solid brush with a color ARGB value of #FFFFF8DC</summary>
		public static SolidBrush Cornsilk { get { return GetBrush(Colors.Cornsilk); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFFFEBCD</summary>
		public static SolidBrush BlanchedAlmond { get { return GetBrush(Colors.BlanchedAlmond); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFFFE4C4</summary>
		public static SolidBrush Bisque { get { return GetBrush(Colors.Bisque); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFFFDEAD</summary>
		public static SolidBrush NavajoWhite { get { return GetBrush(Colors.NavajoWhite); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFF5DEB3</summary>
		public static SolidBrush Wheat { get { return GetBrush(Colors.Wheat); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFDEB887</summary>
		public static SolidBrush BurlyWood { get { return GetBrush(Colors.BurlyWood); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFD2B48C</summary>
		public static SolidBrush Tan { get { return GetBrush(Colors.Tan); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFBC8F8F</summary>
		public static SolidBrush RosyBrown { get { return GetBrush(Colors.RosyBrown); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFF4A460</summary>
		public static SolidBrush SandyBrown { get { return GetBrush(Colors.SandyBrown); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFDAA520</summary>
		public static SolidBrush Goldenrod { get { return GetBrush(Colors.Goldenrod); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFB8860B</summary>
		public static SolidBrush DarkGoldenrod { get { return GetBrush(Colors.DarkGoldenrod); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFCD853F</summary>
		public static SolidBrush Peru { get { return GetBrush(Colors.Peru); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFD2691E</summary>
		public static SolidBrush Chocolate { get { return GetBrush(Colors.Chocolate); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF8B4513</summary>
		public static SolidBrush SaddleBrown { get { return GetBrush(Colors.SaddleBrown); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFA0522D</summary>
		public static SolidBrush Sienna { get { return GetBrush(Colors.Sienna); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFA52A2A</summary>
		public static SolidBrush Brown { get { return GetBrush(Colors.Brown); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF800000</summary>
		public static SolidBrush Maroon { get { return GetBrush(Colors.Maroon); } }
		
		// White colors
		/// <summary>Gets a solid brush with a color ARGB value of #FFFFFFFF</summary>
		public static SolidBrush White { get { return GetBrush(Colors.White); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFFFFAFA</summary>
		public static SolidBrush Snow { get { return GetBrush(Colors.Snow); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFF0FFF0</summary>
		public static SolidBrush Honeydew { get { return GetBrush(Colors.Honeydew); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFF5FFFA</summary>
		public static SolidBrush MintCream { get { return GetBrush(Colors.MintCream); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFF0FFFF</summary>
		public static SolidBrush Azure { get { return GetBrush(Colors.Azure); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFF0F8FF</summary>
		public static SolidBrush AliceBlue { get { return GetBrush(Colors.AliceBlue); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFF8F8FF</summary>
		public static SolidBrush GhostWhite { get { return GetBrush(Colors.GhostWhite); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFF5F5F5</summary>
		public static SolidBrush WhiteSmoke { get { return GetBrush(Colors.WhiteSmoke); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFFFF5EE</summary>
		public static SolidBrush Seashell { get { return GetBrush(Colors.Seashell); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFF5F5DC</summary>
		public static SolidBrush Beige { get { return GetBrush(Colors.Beige); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFFDF5E6</summary>
		public static SolidBrush OldLace { get { return GetBrush(Colors.OldLace); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFFFFAF0</summary>
		public static SolidBrush FloralWhite { get { return GetBrush(Colors.FloralWhite); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFFFFFF0</summary>
		public static SolidBrush Ivory { get { return GetBrush(Colors.Ivory); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFFAEBD7</summary>
		public static SolidBrush AntiqueWhite { get { return GetBrush(Colors.AntiqueWhite); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFFAF0E6</summary>
		public static SolidBrush Linen { get { return GetBrush(Colors.Linen); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFFFF0F5</summary>
		public static SolidBrush LavenderBlush { get { return GetBrush(Colors.LavenderBlush); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFFFE4E1</summary>
		public static SolidBrush MistyRose { get { return GetBrush(Colors.MistyRose); } }
		
		// Gray colors
		/// <summary>Gets a solid brush with a color ARGB value of #FFDCDCDC</summary>
		public static SolidBrush Gainsboro { get { return GetBrush(Colors.Gainsboro); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFD3D3D3</summary>
		public static SolidBrush LightGrey { get { return GetBrush(Colors.LightGrey); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFC0C0C0</summary>
		public static SolidBrush Silver { get { return GetBrush(Colors.Silver); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FFA9A9A9</summary>
		public static SolidBrush DarkGray { get { return GetBrush(Colors.DarkGray); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF808080</summary>
		public static SolidBrush Gray { get { return GetBrush(Colors.Gray); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF696969</summary>
		public static SolidBrush DimGray { get { return GetBrush(Colors.DimGray); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF778899</summary>
		public static SolidBrush LightSlateGray { get { return GetBrush(Colors.LightSlateGray); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF708090</summary>
		public static SolidBrush SlateGray { get { return GetBrush(Colors.SlateGray); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF2F4F4F</summary>
		public static SolidBrush DarkSlateGray { get { return GetBrush(Colors.DarkSlateGray); } }
		/// <summary>Gets a solid brush with a color ARGB value of #FF000000</summary>
		public static SolidBrush Black { get { return GetBrush(Colors.Black); } }
	}
}