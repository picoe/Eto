using System;

namespace Eto.Drawing
{
	/// <summary>
	/// Colour representation in HSB
	/// </summary>
	public struct ColorHSB
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
		
		public readonly static ColorHSB Empty = new ColorHSB ();
		
		public static float Distance (ColorHSB value1, ColorHSB value2)
		{
			return (float)Math.Sqrt (Math.Pow ((value1.H - value2.H) / 360f, 2) + Math.Pow (value1.S - value2.S, 2) + Math.Pow (value1.B - value2.B, 2));
		}

		public ColorHSB (float hue, float saturation, float brightness, float alpha = 1f)
			: this()
		{
			H = hue;
			S = saturation;
			B = brightness;
			A = alpha;
		}
 
		public ColorHSB (Color color)
			: this()
		{
			float max = Math.Max (color.R, Math.Max (color.G, color.B));
			float min = Math.Min (color.R, Math.Min (color.G, color.B));

			float h = 0f;
			if (max == color.R && color.G >= color.B) {
				h = 60 * (color.G - color.B) / (max - min);
			} else if (max == color.R && color.G < color.B) {
				h = 60 * (color.G - color.B) / (max - min) + 360;
			} else if (max == color.G) {
				h = 60 * (color.B - color.R) / (max - min) + 120;
			} else if (max == color.B) {
				h = 60 * (color.R - color.G) / (max - min) + 240;
			}

			float s = (max == 0) ? 0f : (1.0f - (min / max));
			
			this.H = h;
			this.S = s;
			this.B = max;
			this.A = color.A;
		}
		
		public override bool Equals (object obj)
		{
			return obj is ColorHSB && this == (ColorHSB)obj;
		}

		public override int GetHashCode ()
		{
			return H.GetHashCode () ^ S.GetHashCode () ^ B.GetHashCode () ^ A.GetHashCode ();
		}

		public Color ToColor ()
		{
			float r = 0;
			float g = 0;
			float b = 0;

			if (this.S == 0) {
				r = g = b = 0;
			}
			else {
				// the color wheel consists of 6 sectors. Figure out which sector

				// you're in.

				float sectorPos = this.H / 60.0f;
				int sectorNumber = (int)(Math.Floor (sectorPos));
				// get the fractional part of the sector

				float fractionalSector = sectorPos - sectorNumber;

				// calculate values for the three axes of the color.

				float p = this.B * (1.0f - this.S);
				float q = this.B * (1.0f - (this.S * fractionalSector));
				float t = this.B * (1.0f - (this.S * (1 - fractionalSector)));

				// assign the fractional colors to r, g, and b based on the sector

				// the angle is in.

				switch (sectorNumber) {
				case 0:
					r = this.B;
					g = t;
					b = p;
					break;
				case 1:
					r = q;
					g = this.B;
					b = p;
					break;
				case 2:
					r = p;
					g = this.B;
					b = t;
					break;
				case 3:
					r = p;
					g = q;
					b = this.B;
					break;
				case 4:
					r = t;
					g = p;
					b = this.B;
					break;
				case 5:
					r = this.B;
					g = p;
					b = q;
					break;
				}
			}

			return new Color (r, g, b, this.A);
		}

		public static bool operator == (ColorHSB x, ColorHSB y)
		{
			return x.H == y.H && x.S == y.S && x.B == y.B && x.A == y.A;
		}

		public static bool operator != (ColorHSB x, ColorHSB y)
		{
			return !(x == y);
		}

		public static implicit operator Color (ColorHSB hsb)
		{
			return hsb.ToColor ();
		}
		
	}
}

