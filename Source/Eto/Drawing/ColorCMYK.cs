using System;

namespace Eto.Drawing
{
	/// <summary>
	/// Represents a color in the CMYK color model.
	/// </summary>
	/// <remarks>
	/// This is a helper class to handle CMYK colors. Whenever a color is used it must be
	/// converted to a <see cref="Color"/> struct first, either by using <see cref="ColorCMYK.ToColor"/>
	/// or the implicit conversion.
	/// </remarks>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public struct ColorCMYK : IEquatable<ColorCMYK>
	{
		/// <summary>
		/// Cyan component
		/// </summary>
		public float C { get; set; }
		
		/// <summary>
		/// Magenta component
		/// </summary>
		public float M { get; set; }
		
		/// <summary>
		/// Yellow component
		/// </summary>
		public float Y { get; set; }
		
		/// <summary>
		/// Key (black) component
		/// </summary>
		public float K { get; set; }
		
		/// <summary>
		/// Alpha component
		/// </summary>
		public float A { get; set; }
		
		/// <summary>
		/// Calculates the 'distance' of two CMYK colors
		/// </summary>
		/// <remarks>
		/// This is useful for comparing two different color values to determine if they are similar.
		/// 
		/// Typically though, <see cref="ColorHSL.Distance"/> gives the best result instead of this method.
		/// </remarks>
		/// <param name="value1">First color to compare</param>
		/// <param name="value2">Second color to compare</param>
		/// <returns>The overall distance/difference between the two colours. A lower value indicates a closer match</returns>
		public static float Distance (ColorCMYK value1, ColorCMYK value2)
		{
			return (float)Math.Sqrt (Math.Pow ((value1.C - value2.C), 2) + Math.Pow (value1.M - value2.M, 2) + Math.Pow (value1.Y - value2.Y, 2) + Math.Pow (value1.K - value2.K, 2));
		}
		
		/// <summary>
		/// Initializes a new instance of the ColorCMYK class
		/// </summary>
		/// <param name="cyan">Cyan component</param>
		/// <param name="magenta">Magenta component</param>
		/// <param name="yellow">Yellow component</param>
		/// <param name="black">Key/black component</param>
		/// <param name="alpha">Alpha component</param>
		public ColorCMYK (float cyan, float magenta, float yellow, float black, float alpha = 1f)
			: this()
		{
			C = cyan;
			M = magenta;
			Y = yellow;
			K = black;
			A = alpha;
		}
 
		/// <summary>
		/// Initializes a new instance of the ColorCMYK with the specified RGB <see cref="Color"/>
		/// </summary>
		/// <param name="color">Color to convert from</param>
		public ColorCMYK (Color color)
			: this()
		{
			float c = 1f - color.R;
			float m = 1f - color.G;
			float y = 1f - color.B;

			float k = Math.Min (c, Math.Min (m, y));

			if (k == 1.0) {
				C = 0;
				M = 0;
				Y = 0;
				K = 1f;
			} else {
				C = (c - k) / (1f - k);
				M = (m - k) / (1f - k);
				Y = (y - k) / (1f - k);
				K = k;
			}
			this.A = color.A;
		}

		/// <summary>
		/// Gets this object as an ARGB color value
		/// </summary>
		public Color ToColor ()
		{
			return new Color(
				(1 - C) * (1 - K),
				(1 - M) * (1 - K),
				(1 - Y) * (1 - K),
				A
			);
		}

		/// <summary>
		/// Compares two colors for equality
		/// </summary>
		/// <param name="color1">First color to compare</param>
		/// <param name="color2">Second color to compare</param>
		/// <returns>true if the two colors are equal, false otherwise</returns>
		public static bool operator == (ColorCMYK color1, ColorCMYK color2)
		{
			return color1.C == color2.C && color1.M == color2.M && color1.Y == color2.Y && color1.K == color2.K && color1.A == color2.A;
		}

		/// <summary>
		/// Compares two colors for inequality
		/// </summary>
		/// <param name="color1">First color to compare</param>
		/// <param name="color2">Second color to compare</param>
		/// <returns>true if the two colors are not equal, false otherwise</returns>
		public static bool operator != (ColorCMYK color1, ColorCMYK color2)
		{
			return !(color1 == color2);
		}

		/// <summary>
		/// Converts this instance to an ARGB color value
		/// </summary>
		/// <param name="cmyk">cmyk value to convert</param>
		/// <returns>A new instance of the Color class with the converted value</returns>
		public static implicit operator Color (ColorCMYK cmyk)
		{
			return cmyk.ToColor ();
		}

		/// <summary>
		/// Converts this an ARGB color value to a CMYK value
		/// </summary>
		/// <param name="color">RGB value to convert</param>
		/// <returns>A new instance of the ColorCMYK class with the converted value</returns>
		public static implicit operator ColorCMYK (Color color)
		{
			return new ColorCMYK (color);
		}

		/// <summary>
		/// Returns a value indicating that this is equal to the specified object
		/// </summary>
		/// <param name="obj">object to compare with</param>
		/// <returns>true if the colours are equal, false otherwise</returns>
		public override bool Equals (object obj)
		{
			return obj is ColorCMYK && this == (ColorCMYK)obj;
		}

		/// <summary>
		/// Gets the hash code for this object
		/// </summary>
		public override int GetHashCode ()
		{
			return C.GetHashCode () ^ M.GetHashCode () ^ Y.GetHashCode () ^ K.GetHashCode () ^ A.GetHashCode ();
		}

		/// <summary>
		/// Returns a value indicating that this is equal to the specified color
		/// </summary>
		/// <param name="other">ColorCMYK to compare with</param>
		/// <returns>True if the colours are equal, false otherwise</returns>
		public bool Equals (ColorCMYK other)
		{
			return other == this;
		}
	}
}

