using System;
using sc = System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace Eto.Drawing
{
	/// <summary>
	/// Color styles when parsing or converting the Color struct
	/// </summary>
	[Flags]
	public enum ColorStyles
	{
		/// <summary>
		/// Default style for Eto
		/// </summary>
		None = 0,
		/// <summary>
		/// Exclude the alpha component when parsing or when converting to hex, omit alpha if possible (it equals 1.0 / 255)
		/// </summary>
		ExcludeAlpha = 1 << 0,
		/// <summary>
		/// Specifies that the alpha component comes last when in hex (compatible with CSS format), otherwise alpha is first
		/// </summary>
		AlphaLast = 1 << 2,
		/// <summary>
		/// Use short hex version, if possible.
		/// </summary>
		/// <remarks>
		/// This is only used when converting to hex, and only if all component values support the short version
		/// </remarks>
		ShortHex = 1 << 3,
		/// <summary>
		/// All color styles
		/// </summary>
		All = ExcludeAlpha | AlphaLast | ShortHex
	}

	/// <summary>
	/// Represents a color with RGBA (Red, Green, Blue, and Alpha) components
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[sc.TypeConverter(typeof(ColorConverterInternal))]
	public struct Color : IEquatable<Color>, IComparable<Color>, IControlObjectSource
	{
		// static members for mapping color names from the Colors class
		static Dictionary<string, Color> colormap;
		static readonly object colormaplock = new object();
		internal const float Epsilon = 1f / 256f;

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
		public int Ab { get { return (int)((A * 255) + 0.5f); } set { A = value / 255f; } }

		/// <summary>
		/// Gets or sets the red component as a byte of a 32-bit color (0-255)
		/// </summary>
		/// <value>The red component</value>
		public int Rb { get { return (int)((R * 255) + 0.5f); } set { R = value / 255f; } }

		/// <summary>
		/// Gets or sets the green component as a byte of a 32-bit color (0-255)
		/// </summary>
		/// <value>The green component</value>
		public int Gb { get { return (int)((G * 255) + 0.5f); } set { G = value / 255f; } }

		/// <summary>
		/// Gets or sets the blue component as a byte of a 32-bit color (0-255)
		/// </summary>
		/// <value>The blue component</value>
		public int Bb { get { return (int)((B * 255) + 0.5f); } set { B = value / 255f; } }

		/// <summary>
		/// Gets the native color control object.
		/// </summary>
		/// <remarks>
		/// This can be null for platforms that do not have (or need) to store the native object.
		/// </remarks>
		/// <value>The native color control object.</value>
		public object ControlObject { get; private set; }

		/// <summary>
		/// The character to split up the string which will be converted
		/// </summary>
		static readonly char[] ColorSplitter = new [] { ',', ' ' };

		/// <summary>
		/// Creates a color from 8-bit ARGB components
		/// </summary>
		/// <returns>A new instance of the Color object with the specified components</returns>
		/// <param name="red">The red component (0-255)</param>
		/// <param name="green">The green component (0-255)</param>
		/// <param name="blue">The blue component (0-255)</param>
		/// <param name="alpha">The alpha component (0-255)</param>
		public static Color FromArgb(int red, int green, int blue, int alpha = 255)
		{
			return new Color(alpha: alpha / 255f, red: red / 255f, green: green / 255f, blue: blue / 255f);
		}

		/// <summary>
		/// Creates a Color from a 32-bit ARGB value
		/// </summary>
		/// <param name="argb">32-bit ARGB value with Alpha in the high byte</param>
		/// <returns>A new instance of the Color object with the specified color</returns>
		public static Color FromArgb(int argb)
		{
			return new Color(((argb >> 16) & 0xff) / 255f, ((argb >> 8) & 0xff) / 255f, (argb & 0xff) / 255f, ((argb >> 24) & 0xff) / 255f);
		}

		/// <summary>
		/// Creates a Color from a 24-bit RGB value
		/// </summary>
		/// <param name="argb">24-bit RGB value with Red in the high byte</param>
		/// <returns>A new instance of the Color object with the specified color</returns>
		public static Color FromRgb(int argb)
		{
			return new Color(((argb >> 16) & 0xff) / 255f, ((argb >> 8) & 0xff) / 255f, (argb & 0xff) / 255f);
		}

		/// <summary>
		/// Creates a Color with a specified value for the Red, Green, and Blue components
		/// </summary>
		/// <param name="val">Value for each RGB component</param>
		/// <param name="alpha">Alpha value</param>
		/// <returns>A new instance of the Color object with the specified grayscale color</returns>
		public static Color FromGrayscale(float val, float alpha = 1f)
		{
			return new Color(val, val, val, alpha);
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
		public static float Distance(Color value1, Color value2)
		{
			return (float)Math.Sqrt(Math.Pow(value1.R - value2.R, 2) + Math.Pow(value1.G - value2.G, 2) + Math.Pow(value1.B - value2.B, 2));
		}

		/// <summary>
		/// Blends the <paramref name="blendColor"/> onto the specified <paramref name="baseColor"/>. Uses alpha component of <paramref name="blendColor"/>
		/// to detemine the blending factor.
		/// </summary>
		/// <remarks>
		/// This computes the blended value of two colors.
		/// </remarks>
		/// <param name="baseColor">Base color</param>
		/// <param name="blendColor">Color to blend onto the base color</param>
		public static Color Blend(Color baseColor, Color blendColor)
		{
			return Blend(baseColor, blendColor, blendColor.A);
		}

		/// <summary>
		/// Blends the <paramref name="blendColor"/> onto the specified <paramref name="baseColor"/>. Ignores the alpha component and uses
		/// the provided blend factor.
		/// </summary>
		/// <remarks>
		/// This computes the blended value of two colors.
		/// </remarks>
		/// <param name="baseColor">Base color</param>
		/// <param name="blendColor">Color to blend onto the base color</param>
		/// <param name="blendFactor">Blend amount from 0 (<paramref name="baseColor"/>) to 1 (<paramref name="blendColor"/>).</param>
		public static Color Blend(Color baseColor, Color blendColor, float blendFactor)
		{
			if (blendFactor < Epsilon)
				return baseColor;

			if (Math.Abs(blendFactor - 1.0f) < Epsilon)
				return blendColor;

			var inv = 1.0f - blendFactor;
			baseColor.R = baseColor.R * inv + blendColor.R * blendFactor;
			baseColor.G = baseColor.G * inv + blendColor.G * blendFactor;
			baseColor.B = baseColor.B * inv + blendColor.B * blendFactor;
			return baseColor;
		}

		/// <summary>
		/// Initializes a new instance of the Color object with the specified red, green, blue, and alpha components
		/// </summary>
		/// <param name="red">Red component (0-1)</param>
		/// <param name="green">Green component (0-1)</param>
		/// <param name="blue">Blue component (0-1)</param>
		/// <param name="alpha">Alpha component (0-1)</param>
		public Color(float red, float green, float blue, float alpha = 1f)
			: this()
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
		public Color(Color color, float? alpha = null)
			: this()
		{
			R = color.R;
			G = color.G;
			B = color.B;
			A = alpha ?? color.A;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Eto.Drawing.Color"/> struct with the specified native control object.
		/// </summary>
		/// <remarks>
		/// Some platforms (e.g. macOS) have system color objects that change their RGB value automatically based on the display mode,
		/// such as light &amp; dark.
		/// </remarks>
		/// <param name="controlObject">Native control object representing this color.</param>
		/// <param name="red">Red component (0-1)</param>
		/// <param name="green">Green component (0-1)</param>
		/// <param name="blue">Blue component (0-1)</param>
		/// <param name="alpha">Alpha component (0-1)</param>
		public Color(object controlObject, float red, float green, float blue, float alpha)
			: this()
		{
			ControlObject = controlObject;
			R = red;
			G = green;
			B = blue;
			A = alpha;
		}

		/// <summary>
		/// Converts the specified string to a color
		/// </summary>
		/// <remarks>
		/// The string can be any of these formats
		///		- #AARRGGBB or #RRGGBB  (where ARGB are hex values)
		///		- 0xAARRGGBB or 0xRRGGBB
		///		- [named] (where [named] is a name of one of the properties in <see cref="Colors"/>)
		///		- [uint]  (where [uint] is a base-10 ARGB value)
		///		- [red], [green], [blue] (where each component is a value from 0-255, 0%-100%, or 0.0-1.0)
		///		- [alpha], [red], [green], [blue] (where each component is a value from 0-255, 0%-100%, or 0.0-1.0)
		///		
		/// If the string is null or empty, this will return <see cref="Colors.Transparent"/>
		/// </remarks>
		/// <param name="value">String value to parse</param>
		/// <param name="color">Color struct with the parsed value, or Transparent if value is invalid</param>
		/// <returns>True if the value was successfully parsed into a color, false otherwise</returns>
		public static bool TryParse(string value, out Color color) => TryParse(value, out color, ColorStyles.None);

		/// <summary>
		/// Converts the specified string to a color
		/// </summary>
		/// <remarks>
		/// The string can be any of these formats:
		///		- #AARRGGBB or 0xAARRGGBB (where ARGB are hex values)
		///		- #RRGGBBAA or 0xRRGGBBAA (when <paramref name="style" /> has <see cref="ColorStyles.AlphaLast" />)
		///		- #RRGGBB or 0xRRGGBB
		///		- [named] (where [named] is a name of one of the properties in <see cref="Colors"/>)
		///		- [uint]  (where [uint] is a base-10 ARGB value)
		///		- [uint]  (where [uint] is a base-10 RGBA value when <paramref name="style" /> has <see cref="ColorStyles.AlphaLast" />)
		///		- [red], [green], [blue] (where each component is a value from 0-255, 0%-100%, or 0.0-1.0)
		///		- [alpha], [red], [green], [blue] (where each component is a value from 0-255, 0%-100%, or 0.0-1.0)
		///		- [red], [green], [blue], [alpha] (when <paramref name="style" /> has <see cref="ColorStyles.AlphaLast" />)
		///		
		/// If the string is null or empty, this will return <see cref="Colors.Transparent"/>
		/// </remarks>
		/// <param name="value">String value to parse</param>
		/// <param name="color">Color struct with the parsed value, or Transparent if value is invalid</param>
		/// <param name="style">Styles for parsing</param>
		/// <returns>True if the value was successfully parsed into a color, false otherwise</returns>
		public static bool TryParse(string value, out Color color, ColorStyles style)
		{
			value = value.Trim();
			if (value.Length == 0)
			{
				color = Colors.Transparent;
				return true;
			}

			// hex values
			bool isHex = false;
			int pos = 0;
			var len = value.Length;

			if (len >= 4 && value[0] == '#')
			{
				isHex = true;
				pos = 1;
			}
			else if (len >= 6 && value[0] == '0' && (value[1] == 'x' || value[1] == 'X'))
			{
				isHex = true;
				pos = 2;
			}

			if (isHex)
			{
				len -= pos;
				if (!(len == 3 || len == 4 || len == 6 || len == 8))
				{
					goto invalid;
				}

				var isShort = len <= 4;
				var clen = isShort ? 1 : 2;
				var hasAlpha = len == 4 || len == 8;

				if (!int.TryParse(value.Substring(pos, clen), NumberStyles.HexNumber, null, out var c1))
					goto invalid;
				pos += clen;
				if (!int.TryParse(value.Substring(pos, clen), NumberStyles.HexNumber, null, out var c2))
					goto invalid;
				pos += clen;
				if (!int.TryParse(value.Substring(pos, clen), NumberStyles.HexNumber, null, out var c3))
					goto invalid;

				if (isShort)
				{
					c1 += c1 * 16;
					c2 += c2 * 16;
					c3 += c3 * 16;
				}

				int c4;
				if (hasAlpha)
				{
					pos += clen;
					if (!int.TryParse(value.Substring(pos, clen), NumberStyles.HexNumber, null, out c4))
						goto invalid;

					if (isShort)
						c4 += c4 * 16;
					if (style.HasFlag(ColorStyles.ExcludeAlpha))
					{
						if (style.HasFlag(ColorStyles.AlphaLast))
							color = Color.FromArgb(c1, c2, c3);
						else
							color = Color.FromArgb(c2, c3, c4);
					}
					else
					{
						if (style.HasFlag(ColorStyles.AlphaLast))
							color = Color.FromArgb(c1, c2, c3, c4);
						else
							color = Color.FromArgb(c2, c3, c4, c1);
					}
				}
				else
				{
					color = Color.FromArgb(c1, c2, c3);
				}

				return true;

			}

			// colormap
			if (colormap == null)
			{
				lock (colormaplock)
				{
					if (colormap == null)
					{
#if NETSTANDARD
						var props = from p in typeof(Colors).GetRuntimeProperties() where p.GetGetMethod().IsStatic && p.GetGetMethod().IsPublic select p;
#else
						var props = typeof (Colors).GetProperties (BindingFlags.Public | BindingFlags.Static);
#endif
						colormap = new Dictionary<string, Color>(StringComparer.OrdinalIgnoreCase);
						foreach (var val in props.Where(r => r.PropertyType == typeof(Color)))
						{
							var col = (Color)val.GetValue(null, null);
							colormap.Add(val.Name, col);
						}
					}
				}
			}
			if (colormap.TryGetValue(value, out color))
				return true;


			// component values

			bool alphaIsFloat = false;
			if (value.StartsWith("rgba"))
			{
				style |= ColorStyles.AlphaLast;
				value = value.Substring(4).TrimStart(' ', '(').TrimEnd(' ', ')');
				alphaIsFloat = true;
			}
			else if (value.StartsWith("rgb"))
			{
				value = value.Substring(3).TrimStart(' ', '(').TrimEnd(' ', ')');
			}
			int alphaIndex = style.HasFlag(ColorStyles.AlphaLast) ? 3 : 0; 


			string[] array = value.Split(ColorSplitter, StringSplitOptions.RemoveEmptyEntries);
			var array2 = new uint[array.Length];
			for (int i = 0; i < array2.Length; i++)
			{
				double fnum;
				bool isPercent = false;
				if (array[i].EndsWith("%"))
				{
					array[i] = array[i].TrimEnd('%');
					isPercent = true;
				}

				if (alphaIsFloat && i == alphaIndex)
				{
					if (!double.TryParse(array[i], out fnum))
						goto invalid;
					if (!isPercent)
						fnum *= 255;
				}
				else if (uint.TryParse(array[i], out var num))
				{
					fnum = num;
				}
				else if (double.TryParse(array[i], out fnum))
				{
					if (!isPercent)
						fnum *= 255;
				}
				else
				{
					goto invalid;
				}

				if (isPercent)
				{
					fnum = fnum * 255.0 / 100.0;
				}

				array2[i] = (uint)Math.Max(0, fnum);
			}
			if (array.Length == 1)
			{
				if (style.HasFlag(ColorStyles.AlphaLast))
				{
					var alpha = array2[0] & 255;
					var c = array2[0] >> 8;
					c += alpha << 24;
					array2[0] = c;
				}
				color = Color.FromArgb((int)array2[0]);
				if (style.HasFlag(ColorStyles.ExcludeAlpha))
					color.A = 1;
				return true;
			}

			for (int i = 0; i < array.Length; i++)
			{
				// ensure we're in range
				array2[i] = Math.Min(255, array2[i]);
			}

			if (array.Length == 3)
			{
				color = Color.FromArgb((int)array2[0], (int)array2[1], (int)array2[2]);
				return true;
			}
			if (array.Length == 4)
			{
				if (style.HasFlag(ColorStyles.ExcludeAlpha))
				{
					if (style.HasFlag(ColorStyles.AlphaLast))
						color = Color.FromArgb((int)array2[0], (int)array2[1], (int)array2[2]);
					else
						color = Color.FromArgb((int)array2[1], (int)array2[2], (int)array2[3]);
				}
				else
				{
					if (style.HasFlag(ColorStyles.AlphaLast))
						color = Color.FromArgb((int)array2[0], (int)array2[1], (int)array2[2], (int)array2[3]);
					else
						color = Color.FromArgb((int)array2[1], (int)array2[2], (int)array2[3], (int)array2[0]);
				}
				return true;
			}
		invalid:
			color = Colors.Transparent;
			return false;
		}

		/// <summary>
		/// Converts a string into a new instance of a Color
		/// </summary>
		/// <remarks>
		/// Use <see cref="TryParse(string, out Color, ColorStyles)"/> instead of try/catch.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">If the value is an invalid color</exception>
		/// <param name="value">Value to convert</param>
		public static Color Parse(string value)
		{
			Color color;
			if (TryParse(value, out color))
				return color;

			throw new ArgumentOutOfRangeException("value", value, string.Format(CultureInfo.CurrentCulture, "Cannot convert value to a color"));
		}

		/// <summary>
		/// Tests if the specified object has the same value as this Color
		/// </summary>
		/// <param name="obj">Color to compare with</param>
		/// <returns>True if the specified object is a Color and has the same ARGB components as this color, false otherwise</returns>
		public override bool Equals(object obj)
		{
			return obj is Color && this == (Color)obj;
		}

		/// <summary>
		/// Gets the hash code for this Color
		/// </summary>
		/// <returns>Hash code for the color</returns>
		public override int GetHashCode()
		{
			return R.GetHashCode() ^ G.GetHashCode() ^ B.GetHashCode() ^ A.GetHashCode();
		}

		/// <summary>
		/// Compares two Color structs for equality
		/// </summary>
		/// <param name="color1">The first Color struct to compare</param>
		/// <param name="color2">The second Color struct to compare</param>
		/// <returns>True if both the Color structs have the same values for all ARGB components</returns>
		public static bool operator ==(Color color1, Color color2)
		{
			return Math.Abs(color1.B - color2.B) < Epsilon
				&& Math.Abs(color1.R - color2.R) < Epsilon
				&& Math.Abs(color1.G - color2.G) < Epsilon
				&& Math.Abs(color1.A - color2.A) < Epsilon;
		}

		/// <summary>
		/// Compares two Color structs for inequality
		/// </summary>
		/// <param name="color1">The first Color struct to compare</param>
		/// <param name="color2">The second Color struct to compare</param>
		/// <returns>True if the Color structs have a differing value for any of the ARGB components</returns>
		public static bool operator !=(Color color1, Color color2)
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
		public void Invert()
		{
			R = 1f - R;
			G = 1f - G;
			B = 1f - B;
		}

		/// <summary>
		/// Converts this color to a 32-bit ARGB value.
		/// </summary>
		/// <returns>The 32-bit ARGB value that corresponds to this color</returns>
		public int ToArgb()
		{
			return (int)((uint)(B * byte.MaxValue) | (uint)(G * byte.MaxValue) << 8 | (uint)(R * byte.MaxValue) << 16 | (uint)(A * byte.MaxValue) << 24);
		}

		/// <summary>
		/// Converts this color to a 32-bit ARGB value given the specified <paramref name="style" />
		/// </summary>
		/// <returns>The 32-bit ARGB value that corresponds to this color</returns>
		public int ToArgb(ColorStyles style)
		{
			if (style.HasFlag(ColorStyles.ExcludeAlpha))
				return (int)((uint)(B * byte.MaxValue) | (uint)(G * byte.MaxValue) << 8 | (uint)(R * byte.MaxValue) << 16);

			if (style.HasFlag(ColorStyles.AlphaLast))
				return (int)((uint)(A * byte.MaxValue) | (uint)(B * byte.MaxValue) << 8 | (uint)(G * byte.MaxValue) << 16 | (uint)(R * byte.MaxValue) << 24);
			else
				return (int)((uint)(B * byte.MaxValue) | (uint)(G * byte.MaxValue) << 8 | (uint)(R * byte.MaxValue) << 16 | (uint)(A * byte.MaxValue) << 24);
		}

		/// <summary>
		/// Converts this color to a hex representation
		/// </summary>
		/// <remarks>
		/// This will either return a hex value with 8 digits (two per component), or 6 digits (two per RGB) if the <paramref name="includeAlpha"/> is set to false.
		/// </remarks>
		/// <param name="includeAlpha">True to include the alpha component, false to exclude it</param>
		/// <returns>A hex representation of this color, with 8 digits if <paramref name="includeAlpha"/> is true, or 6 digits if false</returns>
		public string ToHex(bool includeAlpha = true) => ToHex(includeAlpha ? ColorStyles.None : ColorStyles.ExcludeAlpha);

		/// <summary>
		/// Converts this color to a hex representation
		/// </summary>
		/// <remarks>
		/// This will either return a hex value with 8 digits (two per component), or 6 digits (two per RGB) depending on the specified <paramref name="style" />
		/// </remarks>
		/// <param name="style">Styles for converting to hex</param>
		/// <returns>A hex representation of this color, with 6 digits if <paramref name="style" /> has <see cref="ColorStyles.ExcludeAlpha" />, or 8 digits if not</returns>
		public string ToHex(ColorStyles style)
		{
			var r = Rb;
			var g = Gb;
			var b = Bb;
			var a = Ab;

			bool CanBeShort(int value) => value % 16 == value / 16;
			if (style.HasFlag(ColorStyles.ShortHex))
			{
				if (CanBeShort(r) && CanBeShort(g) && CanBeShort(b))
				{
					r /= 16;
					g /= 16;
					b /= 16;

					if (style.HasFlag(ColorStyles.ExcludeAlpha))
						return string.Format(CultureInfo.InvariantCulture, "#{0:X1}{1:X1}{2:X1}", r, g, b);
					
					if (CanBeShort(a))
					{
						a /= 16;
						if (style.HasFlag(ColorStyles.AlphaLast))
							return string.Format(CultureInfo.InvariantCulture, "#{0:X1}{1:X1}{2:X1}{3:X1}", r, g, b, a);
						else
							return string.Format(CultureInfo.InvariantCulture, "#{0:X1}{1:X1}{2:X1}{3:X1}", a, r, g, b);
					}
				}
			}

			if (style.HasFlag(ColorStyles.ExcludeAlpha))
				return string.Format(CultureInfo.InvariantCulture, "#{0:X2}{1:X2}{2:X2}", r, g, b);

			if (style.HasFlag(ColorStyles.AlphaLast))
				return string.Format(CultureInfo.InvariantCulture, "#{0:X2}{1:X2}{2:X2}{3:X2}", r, g, b, a);

			return string.Format(CultureInfo.InvariantCulture, "#{0:X2}{1:X2}{2:X2}{3:X2}", a, r, g, b);
		}

		/// <summary>
		/// Converts this object to a string
		/// </summary>
		/// <remarks>
		/// This just calls <see cref="ToHex(bool)"/>
		/// </remarks>
		/// <returns>A string representation of this object</returns>
		public override string ToString()
		{
			return ToHex();
		}

		/// <summary>
		/// Compares the specified color for equality
		/// </summary>
		/// <param name="other">Other color to determine equality</param>
		/// <returns>True if all components of the specified color are equal to this object</returns>
		public bool Equals(Color other)
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
		public static Color FromElementId(int id, int alpha = 255)
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

			return Color.FromArgb(red, green, blue, alpha);
		}

		/// <summary>
		/// Converts this color to an Element ID value
		/// </summary>
		/// <seealso cref="FromElementId"/>
		/// <returns>The element id value of this color</returns>
		public int ToElementId()
		{
			int result = (Rb & 0x7f) << 17
				| (Gb & 0x7f) << 10
				| (Bb & 0x7f) << 3
				| ((Rb & 0x80) == 0x80 ? 4 : 0)
				| ((Gb & 0x80) == 0x80 ? 2 : 0)
				| ((Bb & 0x80) == 0x80 ? 1 : 0);

			return result;
		}

		#endregion

		/// <summary>
		/// Compares two colors for sorting Colors in standard collections.
		/// </summary>
		/// <returns>A signed value indicating whether this instance is greater, less, or equal to other.</returns>
		/// <param name="other">The other color value to compare with.</param>
		int IComparable<Color>.CompareTo(Color other)
		{
			var result = A.CompareTo(other.A);
			if (result == 0)
			{
				result = R.CompareTo(other.R);
				if (result == 0)
				{
					result = G.CompareTo(other.G);
					if (result == 0)
					{
						result = B.CompareTo(other.B);
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Converts this RGB color to the HSB colorspace.
		/// </summary>
		/// <returns>A new ColorHSB structure.</returns>
		public ColorHSB ToHSB()
		{
			return new ColorHSB(this);
		}

		/// <summary>
		/// Converts this RGB color to the HSL colorspace.
		/// </summary>
		/// <returns>A new ColorHSL structure.</returns>
		public ColorHSL ToHSL()
		{
			return new ColorHSL(this);
		}

		/// <summary>
		/// Converts this RGB color to the CMYK colorspace.
		/// </summary>
		/// <returns>A new ColorCMYK structure.</returns>
		public ColorCMYK ToCMYK()
		{
			return new ColorCMYK(this);
		}
	}
}
