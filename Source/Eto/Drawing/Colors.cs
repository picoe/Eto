using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eto.Drawing
{
	/// <summary>
	/// List of common colors
	/// </summary>
	public static class Colors
	{
		/// <summary>Gets a color with an ARGB value of #</summary>
		public static Color Transparent { get { return Color.FromArgb (0, 0, 0, 0); } }

		// Red colors
		/// <summary>Gets a color with an ARGB value of #FFCD5C5C</summary>
		public static Color IndianRed { get { return Color.FromArgb(0xFFCD5C5C); } } // 205  92  92
		/// <summary>Gets a color with an ARGB value of #FFF08080</summary>
		public static Color LightCoral { get { return Color.FromArgb (0xFFF08080); } } // 240 128 128
		/// <summary>Gets a color with an ARGB value of #FFFA8072</summary>
		public static Color Salmon { get { return Color.FromArgb (0xFFFA8072); } } // 250 128 114
		/// <summary>Gets a color with an ARGB value of #FFE9967A</summary>
		public static Color DarkSalmon { get { return Color.FromArgb (0xFFE9967A); } } // 233 150 122
		/// <summary>Gets a color with an ARGB value of #FFFFA07A</summary>
		public static Color LightSalmon { get { return Color.FromArgb (0xFFFFA07A); } } // 255 160 122
		/// <summary>Gets a color with an ARGB value of #FFFF0000</summary>
		public static Color Red { get { return Color.FromArgb (0xFFFF0000); } } // 255   0   0
		/// <summary>Gets a color with an ARGB value of #FFDC143C</summary>
		public static Color Crimson { get { return Color.FromArgb (0xFFDC143C); } } // 220  20  60
		/// <summary>Gets a color with an ARGB value of #FFB22222</summary>
		public static Color FireBrick { get { return Color.FromArgb (0xFFB22222); } } // 178  34  34
		/// <summary>Gets a color with an ARGB value of #FF8B0000</summary>
		public static Color DarkRed { get { return Color.FromArgb (0xFF8B0000); } } // 139   0   0
		
		// Pink colors
		/// <summary>Gets a color with an ARGB value of #FFFFC0CB</summary>
		public static Color Pink { get { return Color.FromArgb (0xFFFFC0CB); } } // 255 192 203
		/// <summary>Gets a color with an ARGB value of #FFFFB6C1</summary>
		public static Color LightPink { get { return Color.FromArgb (0xFFFFB6C1); } } // 255 182 193
		/// <summary>Gets a color with an ARGB value of #FFFF69B4</summary>
		public static Color HotPink { get { return Color.FromArgb (0xFFFF69B4); } } // 255 105 180
		/// <summary>Gets a color with an ARGB value of #FFFF1493</summary>
		public static Color DeepPink { get { return Color.FromArgb (0xFFFF1493); } } // 255  20 147
		/// <summary>Gets a color with an ARGB value of #FFC71585</summary>
		public static Color MediumVioletRed { get { return Color.FromArgb (0xFFC71585); } } // 199  21 133
		/// <summary>Gets a color with an ARGB value of #FFDB7093</summary>
		public static Color PaleVioletRed { get { return Color.FromArgb (0xFFDB7093); } } // 219 112 147
		
		// Orange colors
		/// <summary>Gets a color with an ARGB value of #FFFF7F50</summary>
		public static Color Coral { get { return Color.FromArgb (0xFFFF7F50); } } // 255 127  80
		/// <summary>Gets a color with an ARGB value of #FFFF6347</summary>
		public static Color Tomato { get { return Color.FromArgb (0xFFFF6347); } } // 255  99  71
		/// <summary>Gets a color with an ARGB value of #FFFF4500</summary>
		public static Color OrangeRed { get { return Color.FromArgb (0xFFFF4500); } } // 255  69   0
		/// <summary>Gets a color with an ARGB value of #FFFF8C00</summary>
		public static Color DarkOrange { get { return Color.FromArgb (0xFFFF8C00); } } // 255 140   0
		/// <summary>Gets a color with an ARGB value of #FFFFA500</summary>
		public static Color Orange { get { return Color.FromArgb (0xFFFFA500); } } // 255 165   0

		// Yellow colors
		/// <summary>Gets a color with an ARGB value of #FFFFD700</summary>
		public static Color Gold { get { return Color.FromArgb (0xFFFFD700); } } // 255 215   0
		/// <summary>Gets a color with an ARGB value of #FFFFFF00</summary>
		public static Color Yellow { get { return Color.FromArgb (0xFFFFFF00); } } // 255 255   0
		/// <summary>Gets a color with an ARGB value of #FFFFFFE0</summary>
		public static Color LightYellow { get { return Color.FromArgb (0xFFFFFFE0); } } // 255 255 224
		/// <summary>Gets a color with an ARGB value of #FFFFFACD</summary>
		public static Color LemonChiffon { get { return Color.FromArgb (0xFFFFFACD); } } // 255 250 205
		/// <summary>Gets a color with an ARGB value of #FFFAFAD2</summary>
		public static Color LightGoldenrodYellow { get { return Color.FromArgb (0xFFFAFAD2); } } // 250 250 210
		/// <summary>Gets a color with an ARGB value of #FFFFEFD5</summary>
		public static Color PapayaWhip { get { return Color.FromArgb (0xFFFFEFD5); } } // 255 239 213
		/// <summary>Gets a color with an ARGB value of #FFFFE4B5</summary>
		public static Color Moccasin { get { return Color.FromArgb (0xFFFFE4B5); } } // 255 228 181
		/// <summary>Gets a color with an ARGB value of #FFFFDAB9</summary>
		public static Color PeachPuff { get { return Color.FromArgb (0xFFFFDAB9); } } // 255 218 185
		/// <summary>Gets a color with an ARGB value of #FFEEE8AA</summary>
		public static Color PaleGoldenrod { get { return Color.FromArgb (0xFFEEE8AA); } } // 238 232 170
		/// <summary>Gets a color with an ARGB value of #FFF0E68C</summary>
		public static Color Khaki { get { return Color.FromArgb (0xFFF0E68C); } } // 240 230 140
		/// <summary>Gets a color with an ARGB value of #FFBDB76B</summary>
		public static Color DarkKhaki { get { return Color.FromArgb (0xFFBDB76B); } } // 189 183 107

		// Purple colors
		/// <summary>Gets a color with an ARGB value of #FFE6E6FA</summary>
		public static Color Lavender { get { return Color.FromArgb (0xFFE6E6FA); } } // 230 230 250
		/// <summary>Gets a color with an ARGB value of #FFD8BFD8</summary>
		public static Color Thistle { get { return Color.FromArgb (0xFFD8BFD8); } } // 216 191 216
		/// <summary>Gets a color with an ARGB value of #FFDDA0DD</summary>
		public static Color Plum { get { return Color.FromArgb (0xFFDDA0DD); } } // 221 160 221
		/// <summary>Gets a color with an ARGB value of #FFEE82EE</summary>
		public static Color Violet { get { return Color.FromArgb (0xFFEE82EE); } } // 238 130 238
		/// <summary>Gets a color with an ARGB value of #FFDA70D6</summary>
		public static Color Orchid { get { return Color.FromArgb (0xFFDA70D6); } } // 218 112 214
		/// <summary>Gets a color with an ARGB value of #FFFF00FF</summary>
		public static Color Fuchsia { get { return Color.FromArgb (0xFFFF00FF); } } // 255   0 255
		/// <summary>Gets a color with an ARGB value of #FFFF00FF</summary>
		public static Color Magenta { get { return Color.FromArgb (0xFFFF00FF); } } // 255   0 255
		/// <summary>Gets a color with an ARGB value of #FFBA55D3</summary>
		public static Color MediumOrchid { get { return Color.FromArgb (0xFFBA55D3); } } // 186  85 211
		/// <summary>Gets a color with an ARGB value of #FF9370DB</summary>
		public static Color MediumPurple { get { return Color.FromArgb (0xFF9370DB); } } // 147 112 219
		/// <summary>Gets a color with an ARGB value of #FF8A2BE2</summary>
		public static Color BlueViolet { get { return Color.FromArgb (0xFF8A2BE2); } } // 138  43 226
		/// <summary>Gets a color with an ARGB value of #FF9400D3</summary>
		public static Color DarkViolet { get { return Color.FromArgb (0xFF9400D3); } } // 148   0 211
		/// <summary>Gets a color with an ARGB value of #FF9932CC</summary>
		public static Color DarkOrchid { get { return Color.FromArgb (0xFF9932CC); } } // 153  50 204
		/// <summary>Gets a color with an ARGB value of #FF8B008B</summary>
		public static Color DarkMagenta { get { return Color.FromArgb (0xFF8B008B); } } // 139   0 139
		/// <summary>Gets a color with an ARGB value of #FF800080</summary>
		public static Color Purple { get { return Color.FromArgb (0xFF800080); } } // 128   0 128
		/// <summary>Gets a color with an ARGB value of #FF4B0082</summary>
		public static Color Indigo { get { return Color.FromArgb (0xFF4B0082); } } // 75   0 130
		/// <summary>Gets a color with an ARGB value of #FF483D8B</summary>
		public static Color DarkSlateBlue { get { return Color.FromArgb (0xFF483D8B); } } //  72  61 139
		/// <summary>Gets a color with an ARGB value of #FF6A5ACD</summary>
		public static Color SlateBlue { get { return Color.FromArgb (0xFF6A5ACD); } } // 106  90 205
		/// <summary>Gets a color with an ARGB value of #FF7B68EE</summary>
		public static Color MediumSlateBlue { get { return Color.FromArgb (0xFF7B68EE); } } // 123 104 238
		
		// Green colors
		/// <summary>Gets a color with an ARGB value of #FFADFF2F</summary>
		public static Color GreenYellow { get { return Color.FromArgb (0xFFADFF2F); } } // 173 255  47
		/// <summary>Gets a color with an ARGB value of #FF7FFF00</summary>
		public static Color Chartreuse { get { return Color.FromArgb (0xFF7FFF00); } } // 127 255   0
		/// <summary>Gets a color with an ARGB value of #FF7CFC00</summary>
		public static Color LawnGreen { get { return Color.FromArgb (0xFF7CFC00); } } // 124 252   0
		/// <summary>Gets a color with an ARGB value of #FF00FF00</summary>
		public static Color Lime { get { return Color.FromArgb (0xFF00FF00); } } //  0 255   0
		/// <summary>Gets a color with an ARGB value of #FF32CD32</summary>
		public static Color LimeGreen { get { return Color.FromArgb (0xFF32CD32); } } // 50 205  50
		/// <summary>Gets a color with an ARGB value of #FF98FB98</summary>
		public static Color PaleGreen { get { return Color.FromArgb (0xFF98FB98); } } // 152 251 152
		/// <summary>Gets a color with an ARGB value of #FF90EE90</summary>
		public static Color LightGreen { get { return Color.FromArgb (0xFF90EE90); } } // 144 238 144
		/// <summary>Gets a color with an ARGB value of #FF00FA9A</summary>
		public static Color MediumSpringGreen { get { return Color.FromArgb (0xFF00FA9A); } } //  0 250 154
		/// <summary>Gets a color with an ARGB value of #FF00FF7F</summary>
		public static Color SpringGreen { get { return Color.FromArgb (0xFF00FF7F); } } // 0 255 127
		/// <summary>Gets a color with an ARGB value of #FF3CB371</summary>
		public static Color MediumSeaGreen { get { return Color.FromArgb (0xFF3CB371); } } //  60 179 113
		/// <summary>Gets a color with an ARGB value of #FF2E8B57</summary>
		public static Color SeaGreen { get { return Color.FromArgb (0xFF2E8B57); } } // 46 139  87
		/// <summary>Gets a color with an ARGB value of #FF228B22</summary>
		public static Color ForestGreen { get { return Color.FromArgb (0xFF228B22); } } //  34 139  34
		/// <summary>Gets a color with an ARGB value of #FF008000</summary>
		public static Color Green { get { return Color.FromArgb (0xFF008000); } } //   0 128   0
		/// <summary>Gets a color with an ARGB value of #FF006400</summary>
		public static Color DarkGreen { get { return Color.FromArgb (0xFF006400); } } //   0 100   0
		/// <summary>Gets a color with an ARGB value of #FF9ACD32</summary>
		public static Color YellowGreen { get { return Color.FromArgb (0xFF9ACD32); } } // 154 205  50
		/// <summary>Gets a color with an ARGB value of #FF6B8E23</summary>
		public static Color OliveDrab { get { return Color.FromArgb (0xFF6B8E23); } } // 107 142  35
		/// <summary>Gets a color with an ARGB value of #FF808000</summary>
		public static Color Olive { get { return Color.FromArgb (0xFF808000); } } // 128 128   0
		/// <summary>Gets a color with an ARGB value of #FF556B2F</summary>
		public static Color DarkOliveGreen { get { return Color.FromArgb (0xFF556B2F); } } //  85 107  47
		/// <summary>Gets a color with an ARGB value of #FF66CDAA</summary>
		public static Color MediumAquamarine { get { return Color.FromArgb (0xFF66CDAA); } } // 102 205 170
		/// <summary>Gets a color with an ARGB value of #FF8FBC8F</summary>
		public static Color DarkSeaGreen { get { return Color.FromArgb (0xFF8FBC8F); } } // 143 188 143
		/// <summary>Gets a color with an ARGB value of #FF20B2AA</summary>
		public static Color LightSeaGreen { get { return Color.FromArgb (0xFF20B2AA); } } //  32 178 170
		/// <summary>Gets a color with an ARGB value of #FF008B8B</summary>
		public static Color DarkCyan { get { return Color.FromArgb (0xFF008B8B); } } //   0 139 139
		/// <summary>Gets a color with an ARGB value of #FF008080</summary>
		public static Color Teal { get { return Color.FromArgb (0xFF008080); } } //   0 128 128

		// Blue/Cyan colors
		/// <summary>Gets a color with an ARGB value of #FF00FFFF</summary>
		public static Color Aqua { get { return Color.FromArgb (0xFF00FFFF); } } //   0 255 255
		/// <summary>Gets a color with an ARGB value of #FF00FFFF</summary>
		public static Color Cyan { get { return Color.FromArgb (0xFF00FFFF); } } //   0 255 255
		/// <summary>Gets a color with an ARGB value of #FFE0FFFF</summary>
		public static Color LightCyan { get { return Color.FromArgb (0xFFE0FFFF); } } // 224 255 255
		/// <summary>Gets a color with an ARGB value of #FFAFEEEE</summary>
		public static Color PaleTurquoise { get { return Color.FromArgb (0xFFAFEEEE); } } // 175 238 238
		/// <summary>Gets a color with an ARGB value of #FF7FFFD4</summary>
		public static Color Aquamarine { get { return Color.FromArgb (0xFF7FFFD4); } } // 127 255 212
		/// <summary>Gets a color with an ARGB value of #FF40E0D0</summary>
		public static Color Turquoise { get { return Color.FromArgb (0xFF40E0D0); } } //  64 224 208
		/// <summary>Gets a color with an ARGB value of #FF48D1CC</summary>
		public static Color MediumTurquoise { get { return Color.FromArgb (0xFF48D1CC); } } //  72 209 204
		/// <summary>Gets a color with an ARGB value of #FF00CED1</summary>
		public static Color DarkTurquoise { get { return Color.FromArgb (0xFF00CED1); } } //   0 206 209
		/// <summary>Gets a color with an ARGB value of #FF5F9EA0</summary>
		public static Color CadetBlue { get { return Color.FromArgb (0xFF5F9EA0); } } //  95 158 160
		/// <summary>Gets a color with an ARGB value of #FF4682B4</summary>
		public static Color SteelBlue { get { return Color.FromArgb (0xFF4682B4); } } //  70 130 180
		/// <summary>Gets a color with an ARGB value of #FFB0C4DE</summary>
		public static Color LightSteelBlue { get { return Color.FromArgb (0xFFB0C4DE); } } // 176 196 222
		/// <summary>Gets a color with an ARGB value of #FFB0E0E6</summary>
		public static Color PowderBlue { get { return Color.FromArgb (0xFFB0E0E6); } } // 176 224 230
		/// <summary>Gets a color with an ARGB value of #FFADD8E6</summary>
		public static Color LightBlue { get { return Color.FromArgb (0xFFADD8E6); } } // 173 216 230
		/// <summary>Gets a color with an ARGB value of #FF87CEEB</summary>
		public static Color SkyBlue { get { return Color.FromArgb (0xFF87CEEB); } } // 135 206 235
		/// <summary>Gets a color with an ARGB value of #FF87CEFA</summary>
		public static Color LightSkyBlue { get { return Color.FromArgb (0xFF87CEFA); } } // 135 206 250
		/// <summary>Gets a color with an ARGB value of #FF00BFFF</summary>
		public static Color DeepSkyBlue { get { return Color.FromArgb (0xFF00BFFF); } } //   0 191 255
		/// <summary>Gets a color with an ARGB value of #FF1E90FF</summary>
		public static Color DodgerBlue { get { return Color.FromArgb (0xFF1E90FF); } } //  30 144 255
		/// <summary>Gets a color with an ARGB value of #FF6495ED</summary>
		public static Color CornflowerBlue { get { return Color.FromArgb (0xFF6495ED); } } // 100 149 237
		/// <summary>Gets a color with an ARGB value of #FF4169E1</summary>
		public static Color RoyalBlue { get { return Color.FromArgb (0xFF4169E1); } } //  65 105 225
		/// <summary>Gets a color with an ARGB value of #FF0000FF</summary>
		public static Color Blue { get { return Color.FromArgb (0xFF0000FF); } } //   0   0 255
		/// <summary>Gets a color with an ARGB value of #FF0000CD</summary>
		public static Color MediumBlue { get { return Color.FromArgb (0xFF0000CD); } } //   0   0 205
		/// <summary>Gets a color with an ARGB value of #FF00008B</summary>
		public static Color DarkBlue { get { return Color.FromArgb (0xFF00008B); } } //   0   0 139
		/// <summary>Gets a color with an ARGB value of #FF000080</summary>
		public static Color Navy { get { return Color.FromArgb (0xFF000080); } } //   0   0 128
		/// <summary>Gets a color with an ARGB value of #FF191970</summary>
		public static Color MidnightBlue { get { return Color.FromArgb (0xFF191970); } } //  25  25 112

		// Brown colors
		/// <summary>Gets a color with an ARGB value of #FFFFF8DC</summary>
		public static Color Cornsilk { get { return Color.FromArgb (0xFFFFF8DC); } } // 255 248 220
		/// <summary>Gets a color with an ARGB value of #FFFFEBCD</summary>
		public static Color BlanchedAlmond { get { return Color.FromArgb (0xFFFFEBCD); } } // 255 235 205
		/// <summary>Gets a color with an ARGB value of #FFFFE4C4</summary>
		public static Color Bisque { get { return Color.FromArgb (0xFFFFE4C4); } } // 255 228 196
		/// <summary>Gets a color with an ARGB value of #FFFFDEAD</summary>
		public static Color NavajoWhite { get { return Color.FromArgb (0xFFFFDEAD); } } // 255 222 173
		/// <summary>Gets a color with an ARGB value of #FFF5DEB3</summary>
		public static Color Wheat { get { return Color.FromArgb (0xFFF5DEB3); } } // 245 222 179
		/// <summary>Gets a color with an ARGB value of #FFDEB887</summary>
		public static Color BurlyWood { get { return Color.FromArgb (0xFFDEB887); } } // 222 184 135
		/// <summary>Gets a color with an ARGB value of #FFD2B48C</summary>
		public static Color Tan { get { return Color.FromArgb (0xFFD2B48C); } } // 210 180 140
		/// <summary>Gets a color with an ARGB value of #FFBC8F8F</summary>
		public static Color RosyBrown { get { return Color.FromArgb (0xFFBC8F8F); } } // 188 143 143
		/// <summary>Gets a color with an ARGB value of #FFF4A460</summary>
		public static Color SandyBrown { get { return Color.FromArgb (0xFFF4A460); } } // 244 164  96
		/// <summary>Gets a color with an ARGB value of #FFDAA520</summary>
		public static Color Goldenrod { get { return Color.FromArgb (0xFFDAA520); } } // 218 165  32
		/// <summary>Gets a color with an ARGB value of #FFB8860B</summary>
		public static Color DarkGoldenrod { get { return Color.FromArgb (0xFFB8860B); } } // 184 134  11
		/// <summary>Gets a color with an ARGB value of #FFCD853F</summary>
		public static Color Peru { get { return Color.FromArgb (0xFFCD853F); } } // 205 133  63
		/// <summary>Gets a color with an ARGB value of #FFD2691E</summary>
		public static Color Chocolate { get { return Color.FromArgb (0xFFD2691E); } } // 210 105  30
		/// <summary>Gets a color with an ARGB value of #FF8B4513</summary>
		public static Color SaddleBrown { get { return Color.FromArgb (0xFF8B4513); } } // 139  69  19
		/// <summary>Gets a color with an ARGB value of #FFA0522D</summary>
		public static Color Sienna { get { return Color.FromArgb (0xFFA0522D); } } // 160  82  45
		/// <summary>Gets a color with an ARGB value of #FFA52A2A</summary>
		public static Color Brown { get { return Color.FromArgb (0xFFA52A2A); } } // 165  42  42
		/// <summary>Gets a color with an ARGB value of #FF800000</summary>
		public static Color Maroon { get { return Color.FromArgb (0xFF800000); } } // 128   0   0
		
		// White colors
		/// <summary>Gets a color with an ARGB value of #FFFFFFFF</summary>
		public static Color White { get { return Color.FromArgb (0xFFFFFFFF); } } // 255 255 255
		/// <summary>Gets a color with an ARGB value of #FFFFFAFA</summary>
		public static Color Snow { get { return Color.FromArgb (0xFFFFFAFA); } } // 255 250 250
		/// <summary>Gets a color with an ARGB value of #FFF0FFF0</summary>
		public static Color Honeydew { get { return Color.FromArgb (0xFFF0FFF0); } } // 240 255 240
		/// <summary>Gets a color with an ARGB value of #FFF5FFFA</summary>
		public static Color MintCream { get { return Color.FromArgb (0xFFF5FFFA); } } // 245 255 250
		/// <summary>Gets a color with an ARGB value of #FFF0FFFF</summary>
		public static Color Azure { get { return Color.FromArgb (0xFFF0FFFF); } } // 240 255 255
		/// <summary>Gets a color with an ARGB value of #FFF0F8FF</summary>
		public static Color AliceBlue { get { return Color.FromArgb (0xFFF0F8FF); } } // 240 248 255
		/// <summary>Gets a color with an ARGB value of #FFF8F8FF</summary>
		public static Color GhostWhite { get { return Color.FromArgb (0xFFF8F8FF); } } // 248 248 255
		/// <summary>Gets a color with an ARGB value of #FFF5F5F5</summary>
		public static Color WhiteSmoke { get { return Color.FromArgb (0xFFF5F5F5); } } // 245 245 245
		/// <summary>Gets a color with an ARGB value of #FFFFF5EE</summary>
		public static Color Seashell { get { return Color.FromArgb (0xFFFFF5EE); } } // 255 245 238
		/// <summary>Gets a color with an ARGB value of #FFF5F5DC</summary>
		public static Color Beige { get { return Color.FromArgb (0xFFF5F5DC); } } // 245 245 220
		/// <summary>Gets a color with an ARGB value of #FFFDF5E6</summary>
		public static Color OldLace { get { return Color.FromArgb (0xFFFDF5E6); } } // 253 245 230
		/// <summary>Gets a color with an ARGB value of #FFFFFAF0</summary>
		public static Color FloralWhite { get { return Color.FromArgb (0xFFFFFAF0); } } // 255 250 240
		/// <summary>Gets a color with an ARGB value of #FFFFFFF0</summary>
		public static Color Ivory { get { return Color.FromArgb (0xFFFFFFF0); } } // 255 255 240
		/// <summary>Gets a color with an ARGB value of #FFFAEBD7</summary>
		public static Color AntiqueWhite { get { return Color.FromArgb (0xFFFAEBD7); } } // 250 235 215
		/// <summary>Gets a color with an ARGB value of #FFFAF0E6</summary>
		public static Color Linen { get { return Color.FromArgb (0xFFFAF0E6); } } // 250 240 230
		/// <summary>Gets a color with an ARGB value of #FFFFF0F5</summary>
		public static Color LavenderBlush { get { return Color.FromArgb (0xFFFFF0F5); } } // 255 240 245
		/// <summary>Gets a color with an ARGB value of #FFFFE4E1</summary>
		public static Color MistyRose { get { return Color.FromArgb (0xFFFFE4E1); } } // 255 228 225

		// Gray colors
		/// <summary>Gets a color with an ARGB value of #FFDCDCDC</summary>
		public static Color Gainsboro { get { return Color.FromArgb (0xFFDCDCDC); } } // 220 220 220
		/// <summary>Gets a color with an ARGB value of #FFD3D3D3</summary>
		public static Color LightGrey { get { return Color.FromArgb (0xFFD3D3D3); } } // 211 211 211
		/// <summary>Gets a color with an ARGB value of #FFC0C0C0</summary>
		public static Color Silver { get { return Color.FromArgb (0xFFC0C0C0); } } // 192 192 192
		/// <summary>Gets a color with an ARGB value of #FFA9A9A9</summary>
		public static Color DarkGray { get { return Color.FromArgb (0xFFA9A9A9); } } // 169 169 169
		/// <summary>Gets a color with an ARGB value of #FF808080</summary>
		public static Color Gray { get { return Color.FromArgb (0xFF808080); } } // 128 128 128
		/// <summary>Gets a color with an ARGB value of #FF696969</summary>
		public static Color DimGray { get { return Color.FromArgb (0xFF696969); } } // 105 105 105
		/// <summary>Gets a color with an ARGB value of #FF778899</summary>
		public static Color LightSlateGray { get { return Color.FromArgb (0xFF778899); } } // 119 136 153
		/// <summary>Gets a color with an ARGB value of #FF708090</summary>
		public static Color SlateGray { get { return Color.FromArgb (0xFF708090); } } // 112 128 144
		/// <summary>Gets a color with an ARGB value of #FF2F4F4F</summary>
		public static Color DarkSlateGray { get { return Color.FromArgb (0xFF2F4F4F); } } //  47  79  79
		/// <summary>Gets a color with an ARGB value of #FF000000</summary>
		public static Color Black { get { return Color.FromArgb (0xFF000000); } } //   0   0   0
	}
}
