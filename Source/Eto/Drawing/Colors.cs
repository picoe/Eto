
namespace Eto.Drawing
{
	/// <summary>
	/// List of common colors
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public static class Colors
	{
		/// <summary>Gets a color with an ARGB value of #</summary>
		public static Color Transparent { get { return Color.FromArgb (0, 0, 0, 0); } }

		// Red colors
		/// <summary>Gets a color with an ARGB value of #FFCD5C5C</summary>
		public static Color IndianRed { get { return Color.FromRgb(0xCD5C5C); } } // 205  92  92
		/// <summary>Gets a color with an ARGB value of #FFF08080</summary>
		public static Color LightCoral { get { return Color.FromRgb(0xF08080); } } // 240 128 128
		/// <summary>Gets a color with an ARGB value of #FFFA8072</summary>
		public static Color Salmon { get { return Color.FromRgb(0xFA8072); } } // 250 128 114
		/// <summary>Gets a color with an ARGB value of #FFE9967A</summary>
		public static Color DarkSalmon { get { return Color.FromRgb(0xE9967A); } } // 233 150 122
		/// <summary>Gets a color with an ARGB value of #FFFFA07A</summary>
		public static Color LightSalmon { get { return Color.FromRgb(0xFFA07A); } } // 255 160 122
		/// <summary>Gets a color with an ARGB value of #FFFF0000</summary>
		public static Color Red { get { return Color.FromRgb(0xFF0000); } } // 255   0   0
		/// <summary>Gets a color with an ARGB value of #FFDC143C</summary>
		public static Color Crimson { get { return Color.FromRgb(0xDC143C); } } // 220  20  60
		/// <summary>Gets a color with an ARGB value of #FFB22222</summary>
		public static Color Firebrick { get { return Color.FromRgb(0xB22222); } } // 178  34  34
		/// <summary>Gets a color with an ARGB value of #FF8B0000</summary>
		public static Color DarkRed { get { return Color.FromRgb(0x8B0000); } } // 139   0   0
		
		// Pink colors
		/// <summary>Gets a color with an ARGB value of #FFFFC0CB</summary>
		public static Color Pink { get { return Color.FromRgb(0xFFC0CB); } } // 255 192 203
		/// <summary>Gets a color with an ARGB value of #FFFFB6C1</summary>
		public static Color LightPink { get { return Color.FromRgb(0xFFB6C1); } } // 255 182 193
		/// <summary>Gets a color with an ARGB value of #FFFF69B4</summary>
		public static Color HotPink { get { return Color.FromRgb(0xFF69B4); } } // 255 105 180
		/// <summary>Gets a color with an ARGB value of #FFFF1493</summary>
		public static Color DeepPink { get { return Color.FromRgb(0xFF1493); } } // 255  20 147
		/// <summary>Gets a color with an ARGB value of #FFC71585</summary>
		public static Color MediumVioletRed { get { return Color.FromRgb(0xC71585); } } // 199  21 133
		/// <summary>Gets a color with an ARGB value of #FFDB7093</summary>
		public static Color PaleVioletRed { get { return Color.FromRgb(0xDB7093); } } // 219 112 147
		
		// Orange colors
		/// <summary>Gets a color with an ARGB value of #FFFF7F50</summary>
		public static Color Coral { get { return Color.FromRgb(0xFF7F50); } } // 255 127  80
		/// <summary>Gets a color with an ARGB value of #FFFF6347</summary>
		public static Color Tomato { get { return Color.FromRgb(0xFF6347); } } // 255  99  71
		/// <summary>Gets a color with an ARGB value of #FFFF4500</summary>
		public static Color OrangeRed { get { return Color.FromRgb(0xFF4500); } } // 255  69   0
		/// <summary>Gets a color with an ARGB value of #FFFF8C00</summary>
		public static Color DarkOrange { get { return Color.FromRgb(0xFF8C00); } } // 255 140   0
		/// <summary>Gets a color with an ARGB value of #FFFFA500</summary>
		public static Color Orange { get { return Color.FromRgb(0xFFA500); } } // 255 165   0

		// Yellow colors
		/// <summary>Gets a color with an ARGB value of #FFFFD700</summary>
		public static Color Gold { get { return Color.FromRgb(0xFFD700); } } // 255 215   0
		/// <summary>Gets a color with an ARGB value of #FFFFFF00</summary>
		public static Color Yellow { get { return Color.FromRgb(0xFFFF00); } } // 255 255   0
		/// <summary>Gets a color with an ARGB value of #FFFFFFE0</summary>
		public static Color LightYellow { get { return Color.FromRgb(0xFFFFE0); } } // 255 255 224
		/// <summary>Gets a color with an ARGB value of #FFFFFACD</summary>
		public static Color LemonChiffon { get { return Color.FromRgb(0xFFFACD); } } // 255 250 205
		/// <summary>Gets a color with an ARGB value of #FFFAFAD2</summary>
		public static Color LightGoldenrodYellow { get { return Color.FromRgb(0xFAFAD2); } } // 250 250 210
		/// <summary>Gets a color with an ARGB value of #FFFFEFD5</summary>
		public static Color PapayaWhip { get { return Color.FromRgb(0xFFEFD5); } } // 255 239 213
		/// <summary>Gets a color with an ARGB value of #FFFFE4B5</summary>
		public static Color Moccasin { get { return Color.FromRgb(0xFFE4B5); } } // 255 228 181
		/// <summary>Gets a color with an ARGB value of #FFFFDAB9</summary>
		public static Color PeachPuff { get { return Color.FromRgb(0xFFDAB9); } } // 255 218 185
		/// <summary>Gets a color with an ARGB value of #FFEEE8AA</summary>
		public static Color PaleGoldenrod { get { return Color.FromRgb(0xEEE8AA); } } // 238 232 170
		/// <summary>Gets a color with an ARGB value of #FFF0E68C</summary>
		public static Color Khaki { get { return Color.FromRgb(0xF0E68C); } } // 240 230 140
		/// <summary>Gets a color with an ARGB value of #FFBDB76B</summary>
		public static Color DarkKhaki { get { return Color.FromRgb(0xBDB76B); } } // 189 183 107

		// Purple colors
		/// <summary>Gets a color with an ARGB value of #FFE6E6FA</summary>
		public static Color Lavender { get { return Color.FromRgb(0xE6E6FA); } } // 230 230 250
		/// <summary>Gets a color with an ARGB value of #FFD8BFD8</summary>
		public static Color Thistle { get { return Color.FromRgb(0xD8BFD8); } } // 216 191 216
		/// <summary>Gets a color with an ARGB value of #FFDDA0DD</summary>
		public static Color Plum { get { return Color.FromRgb(0xDDA0DD); } } // 221 160 221
		/// <summary>Gets a color with an ARGB value of #FFEE82EE</summary>
		public static Color Violet { get { return Color.FromRgb(0xEE82EE); } } // 238 130 238
		/// <summary>Gets a color with an ARGB value of #FFDA70D6</summary>
		public static Color Orchid { get { return Color.FromRgb(0xDA70D6); } } // 218 112 214
		/// <summary>Gets a color with an ARGB value of #FFFF00FF</summary>
		public static Color Fuchsia { get { return Color.FromRgb(0xFF00FF); } } // 255   0 255
		/// <summary>Gets a color with an ARGB value of #FFFF00FF</summary>
		public static Color Magenta { get { return Color.FromRgb(0xFF00FF); } } // 255   0 255
		/// <summary>Gets a color with an ARGB value of #FFBA55D3</summary>
		public static Color MediumOrchid { get { return Color.FromRgb(0xBA55D3); } } // 186  85 211
		/// <summary>Gets a color with an ARGB value of #FF9370DB</summary>
		public static Color MediumPurple { get { return Color.FromRgb(0x9370DB); } } // 147 112 219
		/// <summary>Gets a color with an ARGB value of #FF8A2BE2</summary>
		public static Color BlueViolet { get { return Color.FromRgb(0x8A2BE2); } } // 138  43 226
		/// <summary>Gets a color with an ARGB value of #FF9400D3</summary>
		public static Color DarkViolet { get { return Color.FromRgb(0x9400D3); } } // 148   0 211
		/// <summary>Gets a color with an ARGB value of #FF9932CC</summary>
		public static Color DarkOrchid { get { return Color.FromRgb(0x9932CC); } } // 153  50 204
		/// <summary>Gets a color with an ARGB value of #FF8B008B</summary>
		public static Color DarkMagenta { get { return Color.FromRgb(0x8B008B); } } // 139   0 139
		/// <summary>Gets a color with an ARGB value of #FF800080</summary>
		public static Color Purple { get { return Color.FromRgb(0x800080); } } // 128   0 128
		/// <summary>Gets a color with an ARGB value of #FF4B0082</summary>
		public static Color Indigo { get { return Color.FromRgb(0x4B0082); } } // 75   0 130
		/// <summary>Gets a color with an ARGB value of #FF483D8B</summary>
		public static Color DarkSlateBlue { get { return Color.FromRgb(0x483D8B); } } //  72  61 139
		/// <summary>Gets a color with an ARGB value of #FF6A5ACD</summary>
		public static Color SlateBlue { get { return Color.FromRgb(0x6A5ACD); } } // 106  90 205
		/// <summary>Gets a color with an ARGB value of #FF7B68EE</summary>
		public static Color MediumSlateBlue { get { return Color.FromRgb(0x7B68EE); } } // 123 104 238
		
		// Green colors
		/// <summary>Gets a color with an ARGB value of #FFADFF2F</summary>
		public static Color GreenYellow { get { return Color.FromRgb(0xADFF2F); } } // 173 255  47
		/// <summary>Gets a color with an ARGB value of #FF7FFF00</summary>
		public static Color Chartreuse { get { return Color.FromRgb(0x7FFF00); } } // 127 255   0
		/// <summary>Gets a color with an ARGB value of #FF7CFC00</summary>
		public static Color LawnGreen { get { return Color.FromRgb(0x7CFC00); } } // 124 252   0
		/// <summary>Gets a color with an ARGB value of #FF00FF00</summary>
		public static Color Lime { get { return Color.FromRgb(0x00FF00); } } //  0 255   0
		/// <summary>Gets a color with an ARGB value of #FF32CD32</summary>
		public static Color LimeGreen { get { return Color.FromRgb(0x32CD32); } } // 50 205  50
		/// <summary>Gets a color with an ARGB value of #FF98FB98</summary>
		public static Color PaleGreen { get { return Color.FromRgb(0x98FB98); } } // 152 251 152
		/// <summary>Gets a color with an ARGB value of #FF90EE90</summary>
		public static Color LightGreen { get { return Color.FromRgb(0x90EE90); } } // 144 238 144
		/// <summary>Gets a color with an ARGB value of #FF00FA9A</summary>
		public static Color MediumSpringGreen { get { return Color.FromRgb(0x00FA9A); } } //  0 250 154
		/// <summary>Gets a color with an ARGB value of #FF00FF7F</summary>
		public static Color SpringGreen { get { return Color.FromRgb(0x00FF7F); } } // 0 255 127
		/// <summary>Gets a color with an ARGB value of #FF3CB371</summary>
		public static Color MediumSeaGreen { get { return Color.FromRgb(0x3CB371); } } //  60 179 113
		/// <summary>Gets a color with an ARGB value of #FF2E8B57</summary>
		public static Color SeaGreen { get { return Color.FromRgb(0x2E8B57); } } // 46 139  87
		/// <summary>Gets a color with an ARGB value of #FF228B22</summary>
		public static Color ForestGreen { get { return Color.FromRgb(0x228B22); } } //  34 139  34
		/// <summary>Gets a color with an ARGB value of #FF008000</summary>
		public static Color Green { get { return Color.FromRgb(0x008000); } } //   0 128   0
		/// <summary>Gets a color with an ARGB value of #FF006400</summary>
		public static Color DarkGreen { get { return Color.FromRgb(0x006400); } } //   0 100   0
		/// <summary>Gets a color with an ARGB value of #FF9ACD32</summary>
		public static Color YellowGreen { get { return Color.FromRgb(0x9ACD32); } } // 154 205  50
		/// <summary>Gets a color with an ARGB value of #FF6B8E23</summary>
		public static Color OliveDrab { get { return Color.FromRgb(0x6B8E23); } } // 107 142  35
		/// <summary>Gets a color with an ARGB value of #FF808000</summary>
		public static Color Olive { get { return Color.FromRgb(0x808000); } } // 128 128   0
		/// <summary>Gets a color with an ARGB value of #FF556B2F</summary>
		public static Color DarkOliveGreen { get { return Color.FromRgb(0x556B2F); } } //  85 107  47
		/// <summary>Gets a color with an ARGB value of #FF66CDAA</summary>
		public static Color MediumAquamarine { get { return Color.FromRgb(0x66CDAA); } } // 102 205 170
		/// <summary>Gets a color with an ARGB value of #FF8FBC8F</summary>
		public static Color DarkSeaGreen { get { return Color.FromRgb(0x8FBC8F); } } // 143 188 143
		/// <summary>Gets a color with an ARGB value of #FF20B2AA</summary>
		public static Color LightSeaGreen { get { return Color.FromRgb(0x20B2AA); } } //  32 178 170
		/// <summary>Gets a color with an ARGB value of #FF008B8B</summary>
		public static Color DarkCyan { get { return Color.FromRgb(0x008B8B); } } //   0 139 139
		/// <summary>Gets a color with an ARGB value of #FF008080</summary>
		public static Color Teal { get { return Color.FromRgb(0x008080); } } //   0 128 128

		// Blue/Cyan colors
		/// <summary>Gets a color with an ARGB value of #FF00FFFF</summary>
		public static Color Aqua { get { return Color.FromRgb(0x00FFFF); } } //   0 255 255
		/// <summary>Gets a color with an ARGB value of #FF00FFFF</summary>
		public static Color Cyan { get { return Color.FromRgb(0x00FFFF); } } //   0 255 255
		/// <summary>Gets a color with an ARGB value of #FFE0FFFF</summary>
		public static Color LightCyan { get { return Color.FromRgb(0xE0FFFF); } } // 224 255 255
		/// <summary>Gets a color with an ARGB value of #FFAFEEEE</summary>
		public static Color PaleTurquoise { get { return Color.FromRgb(0xAFEEEE); } } // 175 238 238
		/// <summary>Gets a color with an ARGB value of #FF7FFFD4</summary>
		public static Color Aquamarine { get { return Color.FromRgb(0x7FFFD4); } } // 127 255 212
		/// <summary>Gets a color with an ARGB value of #FF40E0D0</summary>
		public static Color Turquoise { get { return Color.FromRgb(0x40E0D0); } } //  64 224 208
		/// <summary>Gets a color with an ARGB value of #FF48D1CC</summary>
		public static Color MediumTurquoise { get { return Color.FromRgb(0x48D1CC); } } //  72 209 204
		/// <summary>Gets a color with an ARGB value of #FF00CED1</summary>
		public static Color DarkTurquoise { get { return Color.FromRgb(0x00CED1); } } //   0 206 209
		/// <summary>Gets a color with an ARGB value of #FF5F9EA0</summary>
		public static Color CadetBlue { get { return Color.FromRgb(0x5F9EA0); } } //  95 158 160
		/// <summary>Gets a color with an ARGB value of #FF4682B4</summary>
		public static Color SteelBlue { get { return Color.FromRgb(0x4682B4); } } //  70 130 180
		/// <summary>Gets a color with an ARGB value of #FFB0C4DE</summary>
		public static Color LightSteelBlue { get { return Color.FromRgb(0xB0C4DE); } } // 176 196 222
		/// <summary>Gets a color with an ARGB value of #FFB0E0E6</summary>
		public static Color PowderBlue { get { return Color.FromRgb(0xB0E0E6); } } // 176 224 230
		/// <summary>Gets a color with an ARGB value of #FFADD8E6</summary>
		public static Color LightBlue { get { return Color.FromRgb(0xADD8E6); } } // 173 216 230
		/// <summary>Gets a color with an ARGB value of #FF87CEEB</summary>
		public static Color SkyBlue { get { return Color.FromRgb(0x87CEEB); } } // 135 206 235
		/// <summary>Gets a color with an ARGB value of #FF87CEFA</summary>
		public static Color LightSkyBlue { get { return Color.FromRgb(0x87CEFA); } } // 135 206 250
		/// <summary>Gets a color with an ARGB value of #FF00BFFF</summary>
		public static Color DeepSkyBlue { get { return Color.FromRgb(0x00BFFF); } } //   0 191 255
		/// <summary>Gets a color with an ARGB value of #FF1E90FF</summary>
		public static Color DodgerBlue { get { return Color.FromRgb(0x1E90FF); } } //  30 144 255
		/// <summary>Gets a color with an ARGB value of #FF6495ED</summary>
		public static Color CornflowerBlue { get { return Color.FromRgb(0x6495ED); } } // 100 149 237
		/// <summary>Gets a color with an ARGB value of #FF4169E1</summary>
		public static Color RoyalBlue { get { return Color.FromRgb(0x4169E1); } } //  65 105 225
		/// <summary>Gets a color with an ARGB value of #FF0000FF</summary>
		public static Color Blue { get { return Color.FromRgb(0x0000FF); } } //   0   0 255
		/// <summary>Gets a color with an ARGB value of #FF0000CD</summary>
		public static Color MediumBlue { get { return Color.FromRgb(0x0000CD); } } //   0   0 205
		/// <summary>Gets a color with an ARGB value of #FF00008B</summary>
		public static Color DarkBlue { get { return Color.FromRgb(0x00008B); } } //   0   0 139
		/// <summary>Gets a color with an ARGB value of #FF000080</summary>
		public static Color Navy { get { return Color.FromRgb(0x000080); } } //   0   0 128
		/// <summary>Gets a color with an ARGB value of #FF191970</summary>
		public static Color MidnightBlue { get { return Color.FromRgb(0x191970); } } //  25  25 112

		// Brown colors
		/// <summary>Gets a color with an ARGB value of #FFFFF8DC</summary>
		public static Color Cornsilk { get { return Color.FromRgb(0xFFF8DC); } } // 255 248 220
		/// <summary>Gets a color with an ARGB value of #FFFFEBCD</summary>
		public static Color BlanchedAlmond { get { return Color.FromRgb(0xFFEBCD); } } // 255 235 205
		/// <summary>Gets a color with an ARGB value of #FFFFE4C4</summary>
		public static Color Bisque { get { return Color.FromRgb(0xFFE4C4); } } // 255 228 196
		/// <summary>Gets a color with an ARGB value of #FFFFDEAD</summary>
		public static Color NavajoWhite { get { return Color.FromRgb(0xFFDEAD); } } // 255 222 173
		/// <summary>Gets a color with an ARGB value of #FFF5DEB3</summary>
		public static Color Wheat { get { return Color.FromRgb(0xF5DEB3); } } // 245 222 179
		/// <summary>Gets a color with an ARGB value of #FFDEB887</summary>
		public static Color BurlyWood { get { return Color.FromRgb(0xDEB887); } } // 222 184 135
		/// <summary>Gets a color with an ARGB value of #FFD2B48C</summary>
		public static Color Tan { get { return Color.FromRgb(0xD2B48C); } } // 210 180 140
		/// <summary>Gets a color with an ARGB value of #FFBC8F8F</summary>
		public static Color RosyBrown { get { return Color.FromRgb(0xBC8F8F); } } // 188 143 143
		/// <summary>Gets a color with an ARGB value of #FFF4A460</summary>
		public static Color SandyBrown { get { return Color.FromRgb(0xF4A460); } } // 244 164  96
		/// <summary>Gets a color with an ARGB value of #FFDAA520</summary>
		public static Color Goldenrod { get { return Color.FromRgb(0xDAA520); } } // 218 165  32
		/// <summary>Gets a color with an ARGB value of #FFB8860B</summary>
		public static Color DarkGoldenrod { get { return Color.FromRgb(0xB8860B); } } // 184 134  11
		/// <summary>Gets a color with an ARGB value of #FFCD853F</summary>
		public static Color Peru { get { return Color.FromRgb(0xCD853F); } } // 205 133  63
		/// <summary>Gets a color with an ARGB value of #FFD2691E</summary>
		public static Color Chocolate { get { return Color.FromRgb(0xD2691E); } } // 210 105  30
		/// <summary>Gets a color with an ARGB value of #FF8B4513</summary>
		public static Color SaddleBrown { get { return Color.FromRgb(0x8B4513); } } // 139  69  19
		/// <summary>Gets a color with an ARGB value of #FFA0522D</summary>
		public static Color Sienna { get { return Color.FromRgb(0xA0522D); } } // 160  82  45
		/// <summary>Gets a color with an ARGB value of #FFA52A2A</summary>
		public static Color Brown { get { return Color.FromRgb(0xA52A2A); } } // 165  42  42
		/// <summary>Gets a color with an ARGB value of #FF800000</summary>
		public static Color Maroon { get { return Color.FromRgb(0x800000); } } // 128   0   0
		
		// White colors
		/// <summary>Gets a color with an ARGB value of #FFFFFFFF</summary>
		public static Color White { get { return Color.FromRgb(0xFFFFFF); } } // 255 255 255
		/// <summary>Gets a color with an ARGB value of #FFFFFAFA</summary>
		public static Color Snow { get { return Color.FromRgb(0xFFFAFA); } } // 255 250 250
		/// <summary>Gets a color with an ARGB value of #FFF0FFF0</summary>
		public static Color Honeydew { get { return Color.FromRgb(0xF0FFF0); } } // 240 255 240
		/// <summary>Gets a color with an ARGB value of #FFF5FFFA</summary>
		public static Color MintCream { get { return Color.FromRgb(0xF5FFFA); } } // 245 255 250
		/// <summary>Gets a color with an ARGB value of #FFF0FFFF</summary>
		public static Color Azure { get { return Color.FromRgb(0xF0FFFF); } } // 240 255 255
		/// <summary>Gets a color with an ARGB value of #FFF0F8FF</summary>
		public static Color AliceBlue { get { return Color.FromRgb(0xF0F8FF); } } // 240 248 255
		/// <summary>Gets a color with an ARGB value of #FFF8F8FF</summary>
		public static Color GhostWhite { get { return Color.FromRgb(0xF8F8FF); } } // 248 248 255
		/// <summary>Gets a color with an ARGB value of #FFF5F5F5</summary>
		public static Color WhiteSmoke { get { return Color.FromRgb(0xF5F5F5); } } // 245 245 245
		/// <summary>Gets a color with an ARGB value of #FFFFF5EE</summary>
		public static Color Seashell { get { return Color.FromRgb(0xFFF5EE); } } // 255 245 238
		/// <summary>Gets a color with an ARGB value of #FFF5F5DC</summary>
		public static Color Beige { get { return Color.FromRgb(0xF5F5DC); } } // 245 245 220
		/// <summary>Gets a color with an ARGB value of #FFFDF5E6</summary>
		public static Color OldLace { get { return Color.FromRgb(0xFDF5E6); } } // 253 245 230
		/// <summary>Gets a color with an ARGB value of #FFFFFAF0</summary>
		public static Color FloralWhite { get { return Color.FromRgb(0xFFFAF0); } } // 255 250 240
		/// <summary>Gets a color with an ARGB value of #FFFFFFF0</summary>
		public static Color Ivory { get { return Color.FromRgb(0xFFFFF0); } } // 255 255 240
		/// <summary>Gets a color with an ARGB value of #FFFAEBD7</summary>
		public static Color AntiqueWhite { get { return Color.FromRgb(0xFAEBD7); } } // 250 235 215
		/// <summary>Gets a color with an ARGB value of #FFFAF0E6</summary>
		public static Color Linen { get { return Color.FromRgb(0xFAF0E6); } } // 250 240 230
		/// <summary>Gets a color with an ARGB value of #FFFFF0F5</summary>
		public static Color LavenderBlush { get { return Color.FromRgb(0xFFF0F5); } } // 255 240 245
		/// <summary>Gets a color with an ARGB value of #FFFFE4E1</summary>
		public static Color MistyRose { get { return Color.FromRgb(0xFFE4E1); } } // 255 228 225

		// Gray colors
		/// <summary>Gets a color with an ARGB value of #FFDCDCDC</summary>
		public static Color Gainsboro { get { return Color.FromRgb(0xDCDCDC); } } // 220 220 220
		/// <summary>Gets a color with an ARGB value of #FFD3D3D3</summary>
		public static Color LightGrey { get { return Color.FromRgb(0xD3D3D3); } } // 211 211 211
		/// <summary>Gets a color with an ARGB value of #FFC0C0C0</summary>
		public static Color Silver { get { return Color.FromRgb(0xC0C0C0); } } // 192 192 192
		/// <summary>Gets a color with an ARGB value of #FFA9A9A9</summary>
		public static Color DarkGray { get { return Color.FromRgb(0xA9A9A9); } } // 169 169 169
		/// <summary>Gets a color with an ARGB value of #FF808080</summary>
		public static Color Gray { get { return Color.FromRgb(0x808080); } } // 128 128 128
		/// <summary>Gets a color with an ARGB value of #FF696969</summary>
		public static Color DimGray { get { return Color.FromRgb(0x696969); } } // 105 105 105
		/// <summary>Gets a color with an ARGB value of #FF778899</summary>
		public static Color LightSlateGray { get { return Color.FromRgb(0x778899); } } // 119 136 153
		/// <summary>Gets a color with an ARGB value of #FF708090</summary>
		public static Color SlateGray { get { return Color.FromRgb(0x708090); } } // 112 128 144
		/// <summary>Gets a color with an ARGB value of #FF2F4F4F</summary>
		public static Color DarkSlateGray { get { return Color.FromRgb(0x2F4F4F); } } //  47  79  79
		/// <summary>Gets a color with an ARGB value of #FF000000</summary>
		public static Color Black { get { return Color.FromRgb(0x000000); } } //   0   0   0
	}
}
