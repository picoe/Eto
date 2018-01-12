using System;

namespace Eto.Drawing
{
	/// <summary>
	/// Color representation in HSB color model
	/// </summary>
	/// <remarks>
	/// This allows you to manage a color in the HSB (otherwise known as HSV) cylindrical model.
	/// 
	/// This is a helper class to handle HSB colors. Whenever a color is used it must be
	/// converted to a <see cref="Color"/> struct first, either by using <see cref="ColorHSB.ToColor"/>
	/// or the implicit conversion.
	/// </remarks>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public struct ColorHSB : IEquatable<ColorHSB>
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
		/// Gets or sets the brightness (0-1)
		/// </summary>
		public float B { get; set; }

		/// <summary>
		/// Calculates the 'distance' of two HSB colors
		/// </summary>
		/// <remarks>
		/// This is useful for comparing two different color values to determine if they are similar.
		/// 
		/// Typically though, <see cref="ColorHSL.Distance"/> gives the best result instead of this method.
		/// </remarks>
		/// <param name="value1">First color to compare</param>
		/// <param name="value2">Second color to compare</param>
		/// <returns>The overall distance/difference between the two colours. A lower value indicates a closer match</returns>
		public static float Distance(ColorHSB value1, ColorHSB value2)
		{
			return (float)Math.Sqrt(Math.Pow((value1.H - value2.H) / 360f, 2) + Math.Pow(value1.S - value2.S, 2) + Math.Pow(value1.B - value2.B, 2));
		}

		/// <summary>
		/// Initializes a new instance of the ColorHSB class
		/// </summary>
		/// <param name="hue">Hue component (0-360)</param>
		/// <param name="saturation">Saturation component (0-1)</param>
		/// <param name="brightness">Brightness component (0-1)</param>
		/// <param name="alpha">Alpha component (0-1)</param>
		public ColorHSB(float hue, float saturation, float brightness, float alpha = 1f)
			: this()
		{
			H = hue;
			S = saturation;
			B = brightness;
			A = alpha;
		}

		/// <summary>
		/// Initializes a new instance of the ColorHSB class with the same color values as <paramref name="color"/>
		/// </summary>
		/// <param name="color">RBG Color value to convert to HSB</param>
		public ColorHSB(Color color)
			: this()
		{
			float max = Math.Max(color.R, Math.Max(color.G, color.B));
			float min = Math.Min(color.R, Math.Min(color.G, color.B));

			float delta = max - min;
			float h;
			float s;
			if (delta <= float.Epsilon)
			{
				h = 0f;
				s = 0f;
			}
			else
			{
				if (Math.Abs(max - color.R) < Color.Epsilon && color.G >= color.B)
				{
					h = 60 * (color.G - color.B) / delta;
				}
				else if (Math.Abs(max - color.R) < Color.Epsilon && color.G < color.B)
				{
					h = 60 * (color.G - color.B) / delta + 360;
				}
				else if (Math.Abs(max - color.G) < Color.Epsilon)
				{
					h = 60 * (color.B - color.R) / delta + 120;
				}
				else if (Math.Abs(max - color.B) < Color.Epsilon)
				{
					h = 60 * (color.R - color.G) / delta + 240;
				}
				else
				{
					h = 0f;
				}

				s = (Math.Abs(max) < Color.Epsilon) ? 0f : (1.0f - (min / max));
			}
			
			this.H = h;
			this.S = s;
			this.B = max;
			this.A = color.A;
		}

		/// <summary>
		/// Converts this instance to an equivalent RGB <see cref="Color"/>
		/// </summary>
		/// <returns>A new instance of a <see cref="Color"/> with an equivalent color</returns>
		public Color ToColor()
		{
			float r = 0;
			float g = 0;
			float b = 0;

			if (S <= 0)
			{
				r = g = b = B; // grayscale
			}
			else
			{
				// the color wheel consists of 6 sectors. Figure out which sector

				// you're in.

				float sectorPos = H / 60.0f;
				int sectorNumber = (int)(Math.Floor(sectorPos));
				// get the fractional part of the sector

				float fractionalSector = sectorPos - sectorNumber;

				// calculate values for the three axes of the color.

				float p = B * (1.0f - S);
				float q = B * (1.0f - (S * fractionalSector));
				float t = B * (1.0f - (S * (1 - fractionalSector)));

				// assign the fractional colors to r, g, and b based on the sector

				// the angle is in.

				switch (sectorNumber)
				{
					case 0:
						r = B;
						g = t;
						b = p;
						break;
					case 1:
						r = q;
						g = B;
						b = p;
						break;
					case 2:
						r = p;
						g = B;
						b = t;
						break;
					case 3:
						r = p;
						g = q;
						b = B;
						break;
					case 4:
						r = t;
						g = p;
						b = B;
						break;
					case 5:
						r = B;
						g = p;
						b = q;
						break;
				}
			}

			return new Color(r, g, b, A);
		}

		/// <summary>
		/// Compares two instances of the <see cref="ColorHSB"/> for equality
		/// </summary>
		/// <param name="color1">First color to compare</param>
		/// <param name="color2">Secont color to compare</param>
		/// <returns>True if both instances are equal, false otherwise</returns>
		public static bool operator ==(ColorHSB color1, ColorHSB color2)
		{
			return color1.H == color2.H && color1.S == color2.S && color1.B == color2.B && color1.A == color2.A;
		}

		/// <summary>
		/// Compares two instances of the <see cref="ColorHSB"/> for inequality
		/// </summary>
		/// <param name="color1">First color to compare</param>
		/// <param name="color2">Secont color to compare</param>
		/// <returns>True if the instances are not equal, false if they are equal</returns>
		public static bool operator !=(ColorHSB color1, ColorHSB color2)
		{
			return !(color1 == color2);
		}

		/// <summary>
		/// Implicitly converts from a ColorHSB to a <see cref="Color"/>
		/// </summary>
		/// <param name="hsb">HSB color instance to convert</param>
		/// <returns>A new instance of a <see cref="Color"/> that represents the <paramref name="hsb"/> value</returns>
		public static implicit operator Color(ColorHSB hsb)
		{
			return hsb.ToColor();
		}

		/// <summary>
		/// Implicitly converts from a <see cref="Color"/> to a ColorHSB
		/// </summary>
		/// <param name="color">RGB color value to convert</param>
		/// <returns>A new instance of a ColorHSB that represents the RGB <paramref name="color"/> value</returns>
		public static implicit operator ColorHSB(Color color)
		{
			return new ColorHSB(color);
		}

		/// <summary>
		/// Compares an object to determine equality with this instance
		/// </summary>
		/// <param name="obj">Object to compare</param>
		/// <returns>True if the object is equal to this instance's value, false otherwise</returns>
		public override bool Equals(object obj)
		{
			return obj is ColorHSB && this == (ColorHSB)obj;
		}

		/// <summary>
		/// Gets the hash code for this object
		/// </summary>
		/// <returns>Hash code to use for this object</returns>
		public override int GetHashCode()
		{
			return H.GetHashCode() ^ S.GetHashCode() ^ B.GetHashCode() ^ A.GetHashCode();
		}

		/// <summary>
		/// Compares a ColorHSB for equality
		/// </summary>
		/// <param name="other">Other instance to compare with</param>
		/// <returns>True if <paramref name="other"/> is equal to this instance's value, false otherwise</returns>
		public bool Equals(ColorHSB other)
		{
			return other == this;
		}
	}
}

