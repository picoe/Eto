using System;

namespace Eto.Drawing
{
	/// <summary>
	/// Color representation in the HSL color model
	/// </summary>
	/// <remarks>
	/// This allows you to manage a color in the HSL cylindrical model.
	/// 
	/// This is a helper class to handle HSL colors. Whenever a color is used it must be
	/// converted to a <see cref="Color"/> struct first, either by using <see cref="ColorHSL.ToColor"/>
	/// or the implicit conversion.
	/// </remarks>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public struct ColorHSL : IEquatable<ColorHSL>
	{
		/// <summary>
		/// Gets or sets the alpha (0-1)
		/// </summary>
		public float A { get; set; }
		
		/// <summary>
		/// Gets or sets the hue (0-360)
		/// </summary>
		public float H { get; set; }
		
		/// <summary>
		/// Gets or sets the saturation (0-1)
		/// </summary>
		public float S { get; set; }
		
		/// <summary>
		/// Gets or sets the luminance (0-1)
		/// </summary>
		public float L { get; set; }
		
		/// <summary>
		/// Calculates the 'distance' of two HSL colors
		/// </summary>
		/// <remarks>
		/// This is useful for comparing two different color values to determine if they are similar.
		/// 
		/// The HSL comparison algorithm, while not essentially accurate, gives a good representation of like-colours
		/// to the human eye. This method of calculating distance is preferred over the other methods (RGB, CMYK, HSB)
		/// </remarks>
		/// <param name="value1">First color to compare</param>
		/// <param name="value2">Second color to compare</param>
		/// <returns>The overall distance/difference between the two colours. A lower value indicates a closer match</returns>
		public static float Distance (ColorHSL value1, ColorHSL value2)
		{
			return (float)Math.Sqrt (Math.Pow ((value1.H - value2.H) / 360f, 2) + Math.Pow (value1.S - value2.S, 2) + Math.Pow (value1.L - value2.L, 2));
		}
		
		/// <summary>
		/// Initializes a new instance of the ColorHSL class
		/// </summary>
		/// <param name="hue">Hue component (0-360)</param>
		/// <param name="saturation">Saturation component (0-1)</param>
		/// <param name="luminance">Luminace component (0-1)</param>
		/// <param name="alpha">Alpha component (0-1)</param>
		public ColorHSL (float hue, float saturation, float luminance, float alpha = 1f)
			: this()
		{
			H = hue;
			S = saturation;
			L = luminance;
			A = alpha;
		}
 
		/// <summary>
		/// Initializes a new instance of the ColorHSL class with converted HSL values from a <see cref="Color"/>
		/// </summary>
		/// <param name="color">RGB color to convert to HSL</param>
		public ColorHSL (Color color)
			: this()
		{
			float h = 0;
			float s = 0;
			float l;

			// normalize red, green, blue values

			float r = color.R;
			float g = color.G;
			float b = color.B;

			float max = Math.Max (r, Math.Max (g, b));
			float min = Math.Min (r, Math.Min (g, b));

			// hue

			if (max == min) {
				h = 0; // undefined

			} else if (max == r && g >= b) {
				h = 60f * (g - b) / (max - min);
			} else if (max == r && g < b) {
				h = 60f * (g - b) / (max - min) + 360f;
			} else if (max == g) {
				h = 60f * (b - r) / (max - min) + 120f;
			} else if (max == b) {
				h = 60f * (r - g) / (max - min) + 240f;
			}

			// luminance

			l = (max + min) / 2f;

			// saturation

			if (l == 0 || max == min) {
				s = 0;
			} else if (0 < l && l <= 0.5) {
				s = (max - min) / (max + min);
			} else if (l > 0.5) {
				s = (max - min) / (2 - (max + min)); //(max-min > 0)?

			}
			
			H = h;
			S = s;
			L = l;
			A = color.A;
		}

		/// <summary>
		/// Converts this HSL color to a RGB <see cref="Color"/> value
		/// </summary>
		/// <returns>A new instance of an RGB <see cref="Color"/> converted from HSL</returns>
		public Color ToColor ()
		{

			if (S == 0) {
				// achromatic color (gray scale)
				return new Color(L, L, L, A);
			}
			else {
				float q = (L < 0.5f) ? (L * (1f + S)) : (L + S - (L * S));
				float p = (2f * L) - q;

				float Hk = H / 360f;
				var T = new float[3];
				T[0] = Hk + (1f / 3f);    // Tr
				T[1] = Hk;                // Tb
				T[2] = Hk - (1f / 3f);    // Tg

				for (int i = 0; i < 3; i++) {
					if (T[i] < 0)
						T[i] += 1f;
					if (T[i] > 1)
						T[i] -= 1f;

					if ((T[i] * 6f) < 1) {
						T[i] = p + ((q - p) * 6f * T[i]);
					}
					else if ((T[i] * 2f) < 1) { //(1.0/6.0)<=T[i] && T[i]<0.5
						T[i] = q;
					}
					else if ((T[i] * 3f) < 2) { // 0.5<=T[i] && T[i]<(2.0/3.0)
						T[i] = p + (q - p) * ((2f / 3f) - T[i]) * 6f;
					}
					else
						T[i] = p;
				}

				return new Color (T[0], T[1], T[2], A);
			}
		}
		
		/// <summary>
		/// Compares two <see cref="ColorHSL"/> objects for equality
		/// </summary>
		/// <param name="color1">First color to compare</param>
		/// <param name="color2">Second color to compare</param>
		/// <returns>True if the objects are equal, false otherwise</returns>
		public static bool operator == (ColorHSL color1, ColorHSL color2)
		{
			return color1.H == color2.H && color1.S == color2.S && color1.L == color2.L && color1.A == color2.A;
		}

		/// <summary>
		/// Compares two <see cref="ColorHSL"/> objects for equality
		/// </summary>
		/// <param name="color1">First color to compare</param>
		/// <param name="color2">Second color to compare</param>
		/// <returns>True if the objects are equal, false otherwise</returns>
		public static bool operator != (ColorHSL color1, ColorHSL color2)
		{
			return !(color1 == color2);
		}

		/// <summary>
		/// Implicitly converts a <see cref="ColorHSL"/> to an RGB <see cref="Color"/>
		/// </summary>
		/// <param name="hsl">HSL Color to convert</param>
		/// <returns>An RGB color converted from the specified <paramref name="hsl"/> color</returns>
		public static implicit operator Color (ColorHSL hsl)
		{
			return hsl.ToColor ();
		}

		/// <summary>
		/// Implicitly converts from a <see cref="Color"/> to a ColorHSL
		/// </summary>
		/// <param name="color">RGB color value to convert</param>
		/// <returns>A new instance of a ColorHSL that represents the RGB <paramref name="color"/> value</returns>
		public static implicit operator ColorHSL (Color color)
		{
			return new ColorHSL (color);
		}

		/// <summary>
		/// Compares the given object for equality with this object
		/// </summary>
		/// <param name="obj">Object to compare with</param>
		/// <returns>True if the object is equal to this instance, false otherwise</returns>
		public override bool Equals (object obj)
		{
			return obj is ColorHSL && this == (ColorHSL)obj;
		}

		/// <summary>
		/// Gets the hash code for this object
		/// </summary>
		/// <returns>Hash code for this object</returns>
		public override int GetHashCode ()
		{
			return H.GetHashCode () ^ S.GetHashCode () ^ L.GetHashCode () ^ A.GetHashCode ();
		}

		/// <summary>
		/// Compares the given object for equality with this object
		/// </summary>
		/// <param name="other">Object to compare with</param>
		/// <returns>True if the object is equal to this instance, false otherwise</returns>
		public bool Equals (ColorHSL other)
		{
			return other == this;
		}
	}
}

