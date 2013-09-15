using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace Eto.Drawing
{
	/// <summary>
	/// Represents a color with RGBA (Red, Green, Blue, and Alpha) components
	/// </summary>
	[TypeConverter (typeof (ColorConverter))]
	public struct Color : IEquatable<Color>
	{
		// static members for mapping color names from the Colors class
		static Dictionary<string, Color> colormap;
		static object colormaplock = new object ();

		/// <summary>
		/// Gets or sets the alpha/opacity (0-1)
		/// </summary>
		public float A { get; set; }
		
		/// <summary>
		/// Gets or sets the red component (0-1)
		/// </summary>
		public float R { get; set; }

		/// <summary>
		/// Gets or sets the green (0-1)
		/// </summary>
		public float G { get; set; }

		/// <summary>
		/// Gets or sets the blue (0-1)
		/// </summary>
		public float B { get; set; }

		/// <summary>
		/// Gets or sets the alpha/opacity component as a byte of a 32-bit color (0-255)
		/// </summary>
		/// <value>The alpha component</value>
		public int Ab { get { return (int)((A * 255) + 0.5); } set { A = value / 255f; } }

		/// <summary>
		/// Gets or sets the red component as a byte of a 32-bit color (0-255)
		/// </summary>
		/// <value>The red component</value>
		public int Rb { get { return (int)((R * 255) + 0.5); } set { R = value / 255f; } }

		/// <summary>
		/// Gets or sets the green component as a byte of a 32-bit color (0-255)
		/// </summary>
		/// <value>The green component</value>
		public int Gb { get { return (int)((G * 255) + 0.5); } set { G = value / 255f; } }

		/// <summary>
		/// Gets or sets the blue component as a byte of a 32-bit color (0-255)
		/// </summary>
		/// <value>The blue component</value>
		public int Bb { get { return (int)((B * 255) + 0.5); } set { B = value / 255f; } }

		/// <summary>
		/// Creates a color from 8-bit ARGB components
		/// </summary>
		/// <returns>A new instance of the Color object with the specified components</returns>
		/// <param name="red">The red component (0-255)</param>
		/// <param name="green">The green component (0-255)</param>
		/// <param name="blue">The blue component (0-255)</param>
		/// <param name="alpha">The alpha component (0-255)</param>
		public static Color FromArgb (int red, int green, int blue, int alpha = 255)
		{
			return new Color(alpha: alpha / 255f, red: red / 255f, green: green / 255f, blue: blue / 255f);
		}

		/// <summary>
		/// Creates a Color from a 32-bit ARGB value
		/// </summary>
		/// <param name="argb">32-bit ARGB value with Alpha in the high byte</param>
		/// <returns>A new instance of the Color object with the specified color</returns>
		public static Color FromArgb (uint argb)
		{
			return new Color (((argb >> 16) & 0xff) / 255f, ((argb >> 8) & 0xff) / 255f, (argb & 0xff) / 255f, ((argb >> 24) & 0xff) / 255f);
		}

		/// <summary>
		/// Creates a Color with a specified value for the Red, Green, and Blue components
		/// </summary>
		/// <param name="val">Value for each RGB component</param>
		/// <param name="alpha">Alpha value</param>
		/// <returns>A new instance of the Color object with the specified grayscale color</returns>
		public static Color FromGrayscale (float val, float alpha = 1f)
		{
			return new Color (val, val, val, alpha);
		}

		/// <summary>
		/// Calculates the distance of the two colors in the RGB scale
		/// </summary>
		/// This is useful for comparing two different color values to determine if they are similar.
		/// 
		/// Typically though, <see cref="ColorHSL.Distance"/> gives the best result instead of using the RGB method.
		/// <param name="value1">First color to compare</param>
		/// <param name="value2">Second color to compare with</param>
		/// <returns>The overall distance/difference between the two colours. A lower value indicates a closer match</returns>
		public static float Distance (Color value1, Color value2)
		{
			return (float)Math.Sqrt (Math.Pow (value1.R - value2.R, 2) + Math.Pow (value1.G - value2.G, 2) + Math.Pow (value1.B - value2.B, 2));
		}

		/// <summary>
		/// Blends the <paramref name="blendColor"/> onto the specified <paramref name="baseColor"/>
		/// </summary>
		/// <remarks>
		/// This computes the blended value of two colors.
		/// </remarks>
		/// <param name="baseColor">Base color</param>
		/// <param name="blendColor">Color to blend onto the base color</param>
		public static Color Blend (Color baseColor, Color blendColor)
		{
			if (blendColor.A == 1.0)
				return blendColor;
			var inv = 1.0f - blendColor.A;
			baseColor.R = baseColor.R * inv + blendColor.R * blendColor.A;
			baseColor.G = baseColor.G * inv + blendColor.G * blendColor.A;
			baseColor.B = baseColor.B * inv + blendColor.B * blendColor.A;
			return baseColor;
		}

		/// <summary>
		/// Initializes a new instance of the Color object with the specified red, green, blue, and alpha components
		/// </summary>
		/// <param name="red">Red component (0-1)</param>
		/// <param name="green">Green component (0-1)</param>
		/// <param name="blue">Blue component (0-1)</param>
		/// <param name="alpha">Alpha component (0-1)</param>
		public Color (float red, float green, float blue, float alpha = 1f)
			: this ()
		{
			this.R = red;
			this.G = green;
			this.B = blue;
			this.A = alpha;
		}

		/// <summary>
		/// Initializes a new instance of the Color object as a copy of the specified <paramref name="color"/>
		/// </summary>
		/// <param name="color">Color to copy</param>
		/// <param name="alpha">Alpha to use for the new color, or null to use the alpha component from <paramref name="color"/></param>
		public Color (Color color, float? alpha = null)
			: this ()
		{
			R = color.R;
			G = color.G;
			B = color.B;
			A = alpha ?? color.A;
		}

		/// <summary>
		/// Converts the specified string to a color
		/// </summary>
		/// <remarks>
		/// The string can be any of these formats:
		///		- #AARRGGBB or #RRGGBB  (where ARGB are hex values)
		///		- 0xAARRGGBB or 0xRRGGBB
		///		- [named] (where [named] is a name of one of the properties in <see cref="Colors"/>)
		///		- [uint]  (where [uint] is a base-10 ARGB value)
		///		- [red], [green], [blue] (where each component is a value from 0-255)
		///		- [alpha], [red], [green], [blue]  (where each component is a value from 0-255)
		///		
		/// If the string is null or empty, this will return <see cref="Colors.Transparent"/>
		/// </remarks>
		/// <param name="value">String value to parse</param>
		/// <param name="color">Color struct with the parsed value, or Transparent if value is invalid</param>
		/// <param name="culture">Culture to use to parse the values</param>
		/// <returns>True if the value was successfully parsed into a color, false otherwise</returns>
		public static bool TryParse (string value, out Color color, CultureInfo culture = null)
		{
			culture = culture ?? CultureInfo.CurrentCulture;
			value = value.Trim ();
			if (value.Length == 0) {
				color = Colors.Transparent;
				return true;
			}

			string listSeparator = culture.TextInfo.ListSeparator;
			if (value.IndexOf (listSeparator) == -1) {
				bool isArgb = value[0] == '#';
				int num = (!isArgb) ? 0 : 1;
				bool ixHex = false;
				if (value.Length > num + 1 && value[num] == '0') {
					ixHex = (value[num + 1] == 'x' || value[num + 1] == 'X');
					if (ixHex) {
						num += 2;
					}
				}
				if (isArgb || ixHex) {
					value = value.Substring (num);
					uint num2;
					if (!uint.TryParse (value, NumberStyles.HexNumber, null, out num2)) {
						color = Colors.Transparent;
						return false;
					}

					if (value.Length < 6 || (value.Length == 6 && isArgb && ixHex)) {
						num2 &= 0xFFFFFF;
					}
					else {
						if (num2 >> 24 == 0) num2 |= 0xFF000000;
					}
					color = Color.FromArgb (num2);
					return true;
				}
				if (colormap == null) {
					lock (colormaplock) {
						if (colormap == null) {
							var props = typeof (Colors).GetProperties (BindingFlags.Public | BindingFlags.Static);
							colormap = new Dictionary<string, Color> (StringComparer.OrdinalIgnoreCase);
							foreach (var val in props.Where (r => r.PropertyType == typeof (Color))) {
								var col = (Color)val.GetValue (null, null);
								colormap.Add (val.Name, col);
							}
						}
					}
				}
				if (colormap.TryGetValue (value, out color))
					return true;
			}
			string[] array = value.Split (listSeparator.ToCharArray ());
			uint[] array2 = new uint[array.Length];
			for (int i = 0; i < array2.Length; i++) {
				uint num;
				if (!uint.TryParse (array[i], out num)) {
					color = Colors.Transparent;
					return false;
				}
				array2[i] = num;
			}
			switch (array.Length) {
			case 1:
				color = Color.FromArgb (array2[0]);
				return true;
			case 3:
				color = Color.FromArgb((int)array2[0], (int)array2[1], (int)array2[2]);
				return true;
			case 4:
				color = Color.FromArgb((int)array2[0], (int)array2[1], (int)array2[2], (int)array2[3]);
				return true;
			}
			color = Colors.Transparent;
			return false;
		}

		/// <summary>
		/// Converts a string into a new instance of a Color
		/// </summary>
		/// <remarks>
		/// Use <see cref="TryParse"/> instead of try/catch.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">If the value is an invalid color</exception>
		/// <param name="value">Value to convert</param>
		public static Color Parse (string value)
		{
			Color color;
			if (TryParse (value, out color))
				return color;
			throw new ArgumentOutOfRangeException ("value", value, "Cannot convert value to a color");
		}

		/// <summary>
		/// Tests if the specified object has the same value as this Color
		/// </summary>
		/// <param name="obj">Color to compare with</param>
		/// <returns>True if the specified object is a Color and has the same ARGB components as this color, false otherwise</returns>
		public override bool Equals (object obj)
		{
			return obj is Color && this == (Color)obj;
		}

		/// <summary>
		/// Gets the hash code for this Color
		/// </summary>
		/// <returns>Hash code for the color</returns>
		public override int GetHashCode ()
		{
			return R.GetHashCode () ^ G.GetHashCode () ^ B.GetHashCode () ^ A.GetHashCode ();
		}

		/// <summary>
		/// Compares two Color structs for equality
		/// </summary>
		/// <param name="color1">The first Color struct to compare</param>
		/// <param name="color2">The second Color struct to compare</param>
		/// <returns>True if both the Color structs have the same values for all ARGB components</returns>
		public static bool operator == (Color color1, Color color2)
		{
			return color1.B == color2.B && color1.R == color2.R && color1.G == color2.G && color1.A == color2.A;
		}

		/// <summary>
		/// Compares two Color structs for inequality
		/// </summary>
		/// <param name="color1">The first Color struct to compare</param>
		/// <param name="color2">The second Color struct to compare</param>
		/// <returns>True if the Color structs have a differing value for any of the ARGB components</returns>
		public static bool operator != (Color color1, Color color2)
		{
			return !(color1 == color2);
		}

		/// <summary>
		/// Inverts the RGB color values
		/// </summary>
		/// <remarks>
		/// This inverts the color components (other than the alpha component) by making them
		/// equal to the 1 minus the component's value.  This is useful for when you want to show
		/// a highlighted color but still show the variation in colors.
		/// </remarks>
		public void Invert ()
		{
			R = 1f - R;
			G = 1f - G;
			B = 1f - B;
		}

		/// <summary>
		/// Converts this color to a 32-bit ARGB value.
		/// </summary>
		/// <returns>The 32-bit ARGB value that corresponds to this color</returns>
		public uint ToArgb ()
		{
			return ((uint)(B * byte.MaxValue) | (uint)(G * byte.MaxValue) << 8 | (uint)(R * byte.MaxValue) << 16 | (uint)(A * byte.MaxValue) << 24);
		}

		/// <summary>
		/// Converts this color to a hex representation
		/// </summary>
		/// <remarks>
		/// This will either return a hex value with 8 digits (two per component), or 6 digits (two per RGB) if the <paramref name="includeAlpha"/> is set to false.
		/// </remarks>
		/// <param name="includeAlpha">True to include the alpha component, false to exclude it</param>
		/// <returns>A hex representation of this color, with 8 digits if <paramref name="includeAlpha"/> is true, or 6 digits if false</returns>
		public string ToHex (bool includeAlpha = true)
		{
			if (includeAlpha)
				return string.Format ("#{0:X2}{1:X2}{2:X2}{3:X2}", (byte)(A * byte.MaxValue), (byte)(R * byte.MaxValue), (byte)(G * byte.MaxValue), (byte)(B * byte.MaxValue));
			else
				return string.Format ("#{0:X2}{1:X2}{2:X2}", (byte)(R * byte.MaxValue), (byte)(G * byte.MaxValue), (byte)(B * byte.MaxValue));
		}

		/// <summary>
		/// Converts this object to a string
		/// </summary>
		/// <remarks>
		/// This just calls <see cref="ToHex"/>
		/// </remarks>
		/// <returns>A string representation of this object</returns>
		public override string ToString ()
		{
			return ToHex ();
		}

		/// <summary>
		/// Compares the specified color for equality
		/// </summary>
		/// <param name="other">Other color to determine equality</param>
		/// <returns>True if all components of the specified color are equal to this object</returns>
		public bool Equals (Color other)
		{
			return other == this;
		}

		#region Element Id Colors

		/// <summary>
		/// Creates a new color from an Element ID value
		/// </summary>
		/// <remarks>
		/// This algorithm is from the SVG# code base:
		/// The counter is used to generate IDs in the range [0,2^24-1]
		/// The 24 bits of the counter are interpreted as follows:
		/// [red 7 bits | green 7 bits | blue 7 bits |shuffle term 3 bits]
		/// The shuffle term is used to define how the remaining high
		/// bit is set on each color. The colors are generated in the
		/// range [0,127] (7 bits) instead of [0,255]. Then the shuffle term
		/// is used to adjust them into the range [0,255].
		/// This algorithm has the feature that consecutive ids generate
		/// visually distinct colors.
		/// </remarks>
		/// <returns>A new instance of a Color with the value from the element id</returns>
		/// <param name="id">Identifier.</param>
		/// <param name="alpha">Alpha.</param>
		public static Color FromElementId (int id, int alpha = 255)
		{
			int shuffleTerm = id & 7;
			
			int red = 0x7f & (id >> 17);
			int green = 0x7f & (id >> 10);
			int blue = 0x7f & (id >> 3);
			
			if ((shuffleTerm & 1) == 1)
				blue |= 0x80;
			
			if ((shuffleTerm & 2) == 2)
				green |= 0x80;
			
			if ((shuffleTerm & 4) == 4)
				red |= 0x80;
			
			return Color.FromArgb (red, green, blue, alpha);
		}

		/// <summary>
		/// Converts this color to an Element ID value
		/// </summary>
		/// <seealso cref="FromElementId"/>
		/// <returns>The element id value of this color</returns>
		public int ToElementId ()
		{
			int result = (this.Rb & 0x7f) << 17
				| (this.Gb & 0x7f) << 10
				| (this.Bb & 0x7f) << 3
				| ((this.Rb & 0x80) == 0x80 ? 4 : 0)
				| ((this.Gb & 0x80) == 0x80 ? 2 : 0)
				| ((this.Bb & 0x80) == 0x80 ? 1 : 0);
			
			return result;
		}

		#endregion

		#region Obsolete
		
		/// <summary>
		/// Obsolete. Do not use.
		/// </summary>
		[Obsolete("Use Colors.Black")]
		public static readonly Color Black = new Color (0f, 0f, 0f);
		
		/// <summary>
		/// Obsolete. Do not use.
		/// </summary>
		[Obsolete ("User Colors.White")]
		public static readonly Color White = new Color (1.0f, 1.0f, 1.0f);
		
		/// <summary>
		/// Obsolete. Do not use.
		/// </summary>
		[Obsolete ("User Colors.Gray")]
		public static readonly Color Gray = new Color (0x77 / 255f, 0x77 / 255f, 0x77 / 255f);
		
		/// <summary>
		/// Obsolete. Do not use.
		/// </summary>
		[Obsolete ("User Colors.DarkGray")]
		public static readonly Color LightGray = new Color (0xA8 / 255f, 0xA8 / 255f, 0xA8 / 255f);
		
		/// <summary>
		/// Obsolete. Do not use.
		/// </summary>
		[Obsolete ("User Colors.Red")]
		public static readonly Color Red = new Color (1f, 0, 0);
		
		/// <summary>
		/// Obsolete. Do not use.
		/// </summary>
		[Obsolete ("User Colors.Lime")]
		public static readonly Color Green = new Color (0, 1f, 0);
		
		/// <summary>
		/// Obsolete. Do not use.
		/// </summary>
		[Obsolete ("User Colors.Blue")]
		public static readonly Color Blue = new Color (0, 0, 1f);
		
		/// <summary>
		/// Obsolete. Do not use.
		/// </summary>
		[Obsolete ("User Colors.Transparent")]
		public static readonly Color Transparent = new Color (0f, 0f, 0f, 0f);
		
		/// <summary>
		/// An empty color with zero for all components
		/// </summary>
#pragma warning disable 618
		[Obsolete("Use nullable values instead of empty color structs")]
		public static readonly Color Empty = new Color { IsEmpty = true };
#pragma warning restore 618
		
		/// <summary>
		/// Obsolete, do not use.
		/// </summary>
		[Obsolete ("Use ColorCMYK.ToColor() or implicit conversion")]
		public Color (ColorCMYK cmyk)
		: this (cmyk.ToColor ())
		{
		}
		
		/// <summary>
		/// Obsolete, do not use.
		/// </summary>
		[Obsolete ("Use ColorHSL.ToColor() or implicit conversion")]
		public Color (ColorHSL hsl)
		: this (hsl.ToColor ())
		{
		}
		
		/// <summary>
		/// Obsolete, do not use.
		/// </summary>
		[Obsolete ("Use ColorHSB.ToColor() or implicit conversion")]
		public Color (ColorHSB hsb)
		: this (hsb.ToColor ())
		{
		}
		
		/// <summary>
		/// Obsolete. Do not use.
		/// </summary>
		[Obsolete ("Use nullable values instead")]
		public bool IsEmpty
		{
			get;
			private set;
		}
		
		/// <summary>
		/// Initializes a new instance of the Color object with the specified red, green, blue, and alpha components
		/// </summary>
		/// <param name="red">Red component (0-255)</param>
		/// <param name="green">Green component (0-255)</param>
		/// <param name="blue">Blue component (0-255)</param>
		/// <param name="alpha">Alpha component (0-255)</param>
		[Obsolete ("Use Color.FromArgb instead")]
		public Color (int red, int green, int blue, int alpha = 0xff)
		: this (red / 255f, green / 255f, blue / 255f, alpha / 255f)
		{
		}
		
		#endregion
	}
}
