using System;
using System.Collections.Generic;
using PenKey = System.Tuple<System.Int32, float, Eto.Drawing.DashStyle>;

namespace Eto.Drawing
{
	/// <summary>
	/// List of pens with common colors and pen cache for pens with a specified color/thickness
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public static class Pens
	{
		static readonly object cacheKey = new object();

		static Pen GetPen(Color color, float thickness = 1f, DashStyle dashStyle = null)
		{
			var cache = Platform.Instance.Cache<PenKey, Pen>(cacheKey);
			Pen pen;
			lock (cache)
			{
				var key = new PenKey(color.ToArgb(), thickness, dashStyle);
				if (!cache.TryGetValue(key, out pen))
				{
					pen = new Pen(color, thickness);
					if (dashStyle != null)
						pen.DashStyle = dashStyle;
					cache.Add(key, pen);
				}
			}
			return pen;
		}

		/// <summary>
		/// Gets a cached pen with the specified <paramref name="color"/> and <paramref name="thickness"/>
		/// </summary>
		/// <param name="color">Color of the cached pen to get</param>
		/// <param name="thickness">Thickness of the cached pen to get</param>
		/// <param name="dashStyle">Dash Style for the pen</param>
		public static Pen Cached(Color color, float thickness = 1f, DashStyle dashStyle = null)
		{
			return GetPen(color, thickness, dashStyle);
		}

		/// <summary>
		/// Clears the pen cache
		/// </summary>
		/// <remarks>
		/// This is useful if you are using the <see cref="Cached(Color,float,DashStyle)"/> method to cache pens and want to clear it
		/// to conserve memory or resources.
		/// </remarks>
		public static void ClearCache()
		{
			var cache = Platform.Instance.Cache<PenKey, Pen>(cacheKey);
			lock (cache)
			{
				cache.Clear();
			}
		}

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #00000000</summary>
		public static Pen Transparent { get { return GetPen(Colors.Transparent); } }
		
		// Red colors
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFCD5C5C</summary>
		public static Pen IndianRed { get { return GetPen(Colors.IndianRed); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFF08080</summary>
		public static Pen LightCoral { get { return GetPen(Colors.LightCoral); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFA8072</summary>
		public static Pen Salmon { get { return GetPen(Colors.Salmon); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFE9967A</summary>
		public static Pen DarkSalmon { get { return GetPen(Colors.DarkSalmon); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFFA07A</summary>
		public static Pen LightSalmon { get { return GetPen(Colors.LightSalmon); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFF0000</summary>
		public static Pen Red { get { return GetPen(Colors.Red); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFDC143C</summary>
		public static Pen Crimson { get { return GetPen(Colors.Crimson); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFB22222</summary>
		public static Pen Firebrick { get { return GetPen(Colors.Firebrick); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF8B0000</summary>
		public static Pen DarkRed { get { return GetPen(Colors.DarkRed); } }
		
		// Pink colors
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFFC0CB</summary>
		public static Pen Pink { get { return GetPen(Colors.Pink); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFFB6C1</summary>
		public static Pen LightPink { get { return GetPen(Colors.LightPink); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFF69B4</summary>
		public static Pen HotPink { get { return GetPen(Colors.HotPink); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFF1493</summary>
		public static Pen DeepPink { get { return GetPen(Colors.DeepPink); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFC71585</summary>
		public static Pen MediumVioletRed { get { return GetPen(Colors.MediumVioletRed); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFDB7093</summary>
		public static Pen PaleVioletRed { get { return GetPen(Colors.PaleVioletRed); } }
		
		// Orange colors
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFF7F50</summary>
		public static Pen Coral { get { return GetPen(Colors.Coral); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFF6347</summary>
		public static Pen Tomato { get { return GetPen(Colors.Tomato); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFF4500</summary>
		public static Pen OrangeRed { get { return GetPen(Colors.OrangeRed); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFF8C00</summary>
		public static Pen DarkOrange { get { return GetPen(Colors.DarkOrange); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFFA500</summary>
		public static Pen Orange { get { return GetPen(Colors.Orange); } }
		
		// Yellow colors
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFFD700</summary>
		public static Pen Gold { get { return GetPen(Colors.Gold); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFFFF00</summary>
		public static Pen Yellow { get { return GetPen(Colors.Yellow); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFFFFE0</summary>
		public static Pen LightYellow { get { return GetPen(Colors.LightYellow); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFFFACD</summary>
		public static Pen LemonChiffon { get { return GetPen(Colors.LemonChiffon); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFAFAD2</summary>
		public static Pen LightGoldenrodYellow { get { return GetPen(Colors.LightGoldenrodYellow); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFFEFD5</summary>
		public static Pen PapayaWhip { get { return GetPen(Colors.PapayaWhip); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFFE4B5</summary>
		public static Pen Moccasin { get { return GetPen(Colors.Moccasin); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFFDAB9</summary>
		public static Pen PeachPuff { get { return GetPen(Colors.PeachPuff); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFEEE8AA</summary>
		public static Pen PaleGoldenrod { get { return GetPen(Colors.PaleGoldenrod); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFF0E68C</summary>
		public static Pen Khaki { get { return GetPen(Colors.Khaki); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFBDB76B</summary>
		public static Pen DarkKhaki { get { return GetPen(Colors.DarkKhaki); } }
		
		// Purple colors
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFE6E6FA</summary>
		public static Pen Lavender { get { return GetPen(Colors.Lavender); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFD8BFD8</summary>
		public static Pen Thistle { get { return GetPen(Colors.Thistle); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFDDA0DD</summary>
		public static Pen Plum { get { return GetPen(Colors.Plum); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFEE82EE</summary>
		public static Pen Violet { get { return GetPen(Colors.Violet); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFDA70D6</summary>
		public static Pen Orchid { get { return GetPen(Colors.Orchid); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFF00FF</summary>
		public static Pen Fuchsia { get { return GetPen(Colors.Fuchsia); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFF00FF</summary>
		public static Pen Magenta { get { return GetPen(Colors.Magenta); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFBA55D3</summary>
		public static Pen MediumOrchid { get { return GetPen(Colors.MediumOrchid); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF9370DB</summary>
		public static Pen MediumPurple { get { return GetPen(Colors.MediumPurple); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF8A2BE2</summary>
		public static Pen BlueViolet { get { return GetPen(Colors.BlueViolet); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF9400D3</summary>
		public static Pen DarkViolet { get { return GetPen(Colors.DarkViolet); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF9932CC</summary>
		public static Pen DarkOrchid { get { return GetPen(Colors.DarkOrchid); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF8B008B</summary>
		public static Pen DarkMagenta { get { return GetPen(Colors.DarkMagenta); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF800080</summary>
		public static Pen Purple { get { return GetPen(Colors.Purple); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF4B0082</summary>
		public static Pen Indigo { get { return GetPen(Colors.Indigo); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF483D8B</summary>
		public static Pen DarkSlateBlue { get { return GetPen(Colors.DarkSlateBlue); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF6A5ACD</summary>
		public static Pen SlateBlue { get { return GetPen(Colors.SlateBlue); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF7B68EE</summary>
		public static Pen MediumSlateBlue { get { return GetPen(Colors.MediumSlateBlue); } }
		
		// Green colors
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFADFF2F</summary>
		public static Pen GreenYellow { get { return GetPen(Colors.GreenYellow); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF7FFF00</summary>
		public static Pen Chartreuse { get { return GetPen(Colors.Chartreuse); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF7CFC00</summary>
		public static Pen LawnGreen { get { return GetPen(Colors.LawnGreen); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF00FF00</summary>
		public static Pen Lime { get { return GetPen(Colors.Lime); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF32CD32</summary>
		public static Pen LimeGreen { get { return GetPen(Colors.LimeGreen); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF98FB98</summary>
		public static Pen PaleGreen { get { return GetPen(Colors.PaleGreen); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF90EE90</summary>
		public static Pen LightGreen { get { return GetPen(Colors.LightGreen); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF00FA9A</summary>
		public static Pen MediumSpringGreen { get { return GetPen(Colors.MediumSpringGreen); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF00FF7F</summary>
		public static Pen SpringGreen { get { return GetPen(Colors.SpringGreen); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF3CB371</summary>
		public static Pen MediumSeaGreen { get { return GetPen(Colors.MediumSeaGreen); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF2E8B57</summary>
		public static Pen SeaGreen { get { return GetPen(Colors.SeaGreen); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF228B22</summary>
		public static Pen ForestGreen { get { return GetPen(Colors.ForestGreen); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF008000</summary>
		public static Pen Green { get { return GetPen(Colors.Green); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF006400</summary>
		public static Pen DarkGreen { get { return GetPen(Colors.DarkGreen); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF9ACD32</summary>
		public static Pen YellowGreen { get { return GetPen(Colors.YellowGreen); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF6B8E23</summary>
		public static Pen OliveDrab { get { return GetPen(Colors.OliveDrab); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF808000</summary>
		public static Pen Olive { get { return GetPen(Colors.Olive); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF556B2F</summary>
		public static Pen DarkOliveGreen { get { return GetPen(Colors.DarkOliveGreen); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF66CDAA</summary>
		public static Pen MediumAquamarine { get { return GetPen(Colors.MediumAquamarine); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF8FBC8F</summary>
		public static Pen DarkSeaGreen { get { return GetPen(Colors.DarkSeaGreen); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF20B2AA</summary>
		public static Pen LightSeaGreen { get { return GetPen(Colors.LightSeaGreen); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF008B8B</summary>
		public static Pen DarkCyan { get { return GetPen(Colors.DarkCyan); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF008080</summary>
		public static Pen Teal { get { return GetPen(Colors.Teal); } }
		
		// Blue/Cyan colors
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF00FFFF</summary>
		public static Pen Aqua { get { return GetPen(Colors.Aqua); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF00FFFF</summary>
		public static Pen Cyan { get { return GetPen(Colors.Cyan); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFE0FFFF</summary>
		public static Pen LightCyan { get { return GetPen(Colors.LightCyan); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFAFEEEE</summary>
		public static Pen PaleTurquoise { get { return GetPen(Colors.PaleTurquoise); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF7FFFD4</summary>
		public static Pen Aquamarine { get { return GetPen(Colors.Aquamarine); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF40E0D0</summary>
		public static Pen Turquoise { get { return GetPen(Colors.Turquoise); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF48D1CC</summary>
		public static Pen MediumTurquoise { get { return GetPen(Colors.MediumTurquoise); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF00CED1</summary>
		public static Pen DarkTurquoise { get { return GetPen(Colors.DarkTurquoise); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF5F9EA0</summary>
		public static Pen CadetBlue { get { return GetPen(Colors.CadetBlue); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF4682B4</summary>
		public static Pen SteelBlue { get { return GetPen(Colors.SteelBlue); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFB0C4DE</summary>
		public static Pen LightSteelBlue { get { return GetPen(Colors.LightSteelBlue); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFB0E0E6</summary>
		public static Pen PowderBlue { get { return GetPen(Colors.PowderBlue); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFADD8E6</summary>
		public static Pen LightBlue { get { return GetPen(Colors.LightBlue); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF87CEEB</summary>
		public static Pen SkyBlue { get { return GetPen(Colors.SkyBlue); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF87CEFA</summary>
		public static Pen LightSkyBlue { get { return GetPen(Colors.LightSkyBlue); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF00BFFF</summary>
		public static Pen DeepSkyBlue { get { return GetPen(Colors.DeepSkyBlue); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF1E90FF</summary>
		public static Pen DodgerBlue { get { return GetPen(Colors.DodgerBlue); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF6495ED</summary>
		public static Pen CornflowerBlue { get { return GetPen(Colors.CornflowerBlue); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF4169E1</summary>
		public static Pen RoyalBlue { get { return GetPen(Colors.RoyalBlue); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF0000FF</summary>
		public static Pen Blue { get { return GetPen(Colors.Blue); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF0000CD</summary>
		public static Pen MediumBlue { get { return GetPen(Colors.MediumBlue); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF00008B</summary>
		public static Pen DarkBlue { get { return GetPen(Colors.DarkBlue); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF000080</summary>
		public static Pen Navy { get { return GetPen(Colors.Navy); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF191970</summary>
		public static Pen MidnightBlue { get { return GetPen(Colors.MidnightBlue); } }
		
		// Brown colors
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFFF8DC</summary>
		public static Pen Cornsilk { get { return GetPen(Colors.Cornsilk); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFFEBCD</summary>
		public static Pen BlanchedAlmond { get { return GetPen(Colors.BlanchedAlmond); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFFE4C4</summary>
		public static Pen Bisque { get { return GetPen(Colors.Bisque); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFFDEAD</summary>
		public static Pen NavajoWhite { get { return GetPen(Colors.NavajoWhite); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFF5DEB3</summary>
		public static Pen Wheat { get { return GetPen(Colors.Wheat); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFDEB887</summary>
		public static Pen BurlyWood { get { return GetPen(Colors.BurlyWood); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFD2B48C</summary>
		public static Pen Tan { get { return GetPen(Colors.Tan); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFBC8F8F</summary>
		public static Pen RosyBrown { get { return GetPen(Colors.RosyBrown); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFF4A460</summary>
		public static Pen SandyBrown { get { return GetPen(Colors.SandyBrown); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFDAA520</summary>
		public static Pen Goldenrod { get { return GetPen(Colors.Goldenrod); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFB8860B</summary>
		public static Pen DarkGoldenrod { get { return GetPen(Colors.DarkGoldenrod); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFCD853F</summary>
		public static Pen Peru { get { return GetPen(Colors.Peru); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFD2691E</summary>
		public static Pen Chocolate { get { return GetPen(Colors.Chocolate); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF8B4513</summary>
		public static Pen SaddleBrown { get { return GetPen(Colors.SaddleBrown); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFA0522D</summary>
		public static Pen Sienna { get { return GetPen(Colors.Sienna); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFA52A2A</summary>
		public static Pen Brown { get { return GetPen(Colors.Brown); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF800000</summary>
		public static Pen Maroon { get { return GetPen(Colors.Maroon); } }
		
		// White colors
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFFFFFF</summary>
		public static Pen White { get { return GetPen(Colors.White); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFFFAFA</summary>
		public static Pen Snow { get { return GetPen(Colors.Snow); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFF0FFF0</summary>
		public static Pen Honeydew { get { return GetPen(Colors.Honeydew); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFF5FFFA</summary>
		public static Pen MintCream { get { return GetPen(Colors.MintCream); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFF0FFFF</summary>
		public static Pen Azure { get { return GetPen(Colors.Azure); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFF0F8FF</summary>
		public static Pen AliceBlue { get { return GetPen(Colors.AliceBlue); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFF8F8FF</summary>
		public static Pen GhostWhite { get { return GetPen(Colors.GhostWhite); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFF5F5F5</summary>
		public static Pen WhiteSmoke { get { return GetPen(Colors.WhiteSmoke); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFFF5EE</summary>
		public static Pen Seashell { get { return GetPen(Colors.Seashell); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFF5F5DC</summary>
		public static Pen Beige { get { return GetPen(Colors.Beige); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFDF5E6</summary>
		public static Pen OldLace { get { return GetPen(Colors.OldLace); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFFFAF0</summary>
		public static Pen FloralWhite { get { return GetPen(Colors.FloralWhite); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFFFFF0</summary>
		public static Pen Ivory { get { return GetPen(Colors.Ivory); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFAEBD7</summary>
		public static Pen AntiqueWhite { get { return GetPen(Colors.AntiqueWhite); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFAF0E6</summary>
		public static Pen Linen { get { return GetPen(Colors.Linen); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFFF0F5</summary>
		public static Pen LavenderBlush { get { return GetPen(Colors.LavenderBlush); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFFFE4E1</summary>
		public static Pen MistyRose { get { return GetPen(Colors.MistyRose); } }
		
		// Gray colors
		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFDCDCDC</summary>
		public static Pen Gainsboro { get { return GetPen(Colors.Gainsboro); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFD3D3D3</summary>
		public static Pen LightGrey { get { return GetPen(Colors.LightGrey); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFC0C0C0</summary>
		public static Pen Silver { get { return GetPen(Colors.Silver); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FFA9A9A9</summary>
		public static Pen DarkGray { get { return GetPen(Colors.DarkGray); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF808080</summary>
		public static Pen Gray { get { return GetPen(Colors.Gray); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF696969</summary>
		public static Pen DimGray { get { return GetPen(Colors.DimGray); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF778899</summary>
		public static Pen LightSlateGray { get { return GetPen(Colors.LightSlateGray); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF708090</summary>
		public static Pen SlateGray { get { return GetPen(Colors.SlateGray); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF2F4F4F</summary>
		public static Pen DarkSlateGray { get { return GetPen(Colors.DarkSlateGray); } }

		/// <summary>Gets a pen with a thickness of 1 and ARGB color value of #FF000000</summary>
		public static Pen Black { get { return GetPen(Colors.Black); } }
	}
}

