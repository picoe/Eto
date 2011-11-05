using System;

namespace Eto.Drawing
{
	public struct Color
	{
		public static readonly Color Black = new Color (0, 0, 0);
		public static readonly Color White = new Color (1.0f, 1.0f, 1.0f);
		public static readonly Color Gray = new Color (0x77 / 255f, 0x77 / 255f, 0x77 / 255f);
		public static readonly Color LightGray = new Color (0xA8 / 255f, 0xA8 / 255f, 0xA8 / 255f);
		public static readonly Color Red = new Color (1f, 0, 0);
		public static readonly Color Green = new Color (0, 1f, 0);
		public static readonly Color Blue = new Color (0, 0, 1f);
		public static readonly Color Transparent = new Color (0, 0, 0, 0);
		public static readonly Color Empty = new Color ();

		public static Color FromArgb (uint argb)
		{
			return new Color (((argb >> 16) & 0xff) / 255f, ((argb >> 8) & 0xff) / 255f, (argb & 0xff) / 255f, ((argb >> 24) & 0xff) / 255f);
		}

		public static Color FromGrayscale (float val, float alpha = 1f)
		{
			return new Color (val, val, val, alpha);
		}

		public static float Distance (Color value1, Color value2)
		{
			return (float)Math.Sqrt (Math.Pow (value1.R - value2.R, 2) + Math.Pow (value1.G - value2.G, 2) + Math.Pow (value1.B - value2.B, 2));
		}
		
		public Color (float red, float green, float blue, float alpha = 1f)
			: this()
		{
			this.R = red;
			this.G = green;
			this.B = blue;
			this.A = alpha;
		}

		public Color (int red, int green, int blue, int alpha = 0xff)
			: this (red / 255f, green / 255f, blue / 255f, alpha / 255f)
		{
		}
		
		public Color (ColorCMYK cmyk)
			: this()
		{
			R = (1 - cmyk.C) * (1 - cmyk.K);
			G = (1 - cmyk.M) * (1 - cmyk.K);
			B = (1 - cmyk.Y) * (1 - cmyk.K);
			A = cmyk.A;
		}
		
		public Color (ColorHSL hsl)
			: this()
		{
			if (hsl.S == 0) {
				// achromatic color (gray scale)
				R = G = B = hsl.L;
			} else {
				float q = (hsl.L < 0.5f) ? (hsl.L * (1f + hsl.S)) : (hsl.L + hsl.S - (hsl.L * hsl.S));
				float p = (2f * hsl.L) - q;

				float Hk = hsl.H / 360f;
				float[] T = new float[3];
				T [0] = Hk + (1f / 3f);    // Tr
				T [1] = Hk;                // Tb
				T [2] = Hk - (1f / 3f);    // Tg

				for (int i=0; i<3; i++) {
					if (T [i] < 0)
						T [i] += 1f;
					if (T [i] > 1)
						T [i] -= 1f;

					if ((T [i] * 6f) < 1) {
						T [i] = p + ((q - p) * 6f * T [i]);
					} else if ((T [i] * 2f) < 1) { //(1.0/6.0)<=T[i] && T[i]<0.5
						T [i] = q;
					} else if ((T [i] * 3f) < 2) { // 0.5<=T[i] && T[i]<(2.0/3.0)
						T [i] = p + (q - p) * ((2f / 3f) - T [i]) * 6f;
					} else
						T [i] = p;
				}

				R = T [0];
				G = T [1];
				B = T [2];
			}
			A = hsl.A;
		}
		
		public Color (ColorHSB hsb)
			: this()
		{
			float r = 0;
			float g = 0;
			float b = 0;

			if (hsb.S == 0) {
				r = g = b = 0;
			} else {
				// the color wheel consists of 6 sectors. Figure out which sector

				// you're in.

				float sectorPos = hsb.H / 60.0f;
				int sectorNumber = (int)(Math.Floor (sectorPos));
				// get the fractional part of the sector

				float fractionalSector = sectorPos - sectorNumber;

				// calculate values for the three axes of the color.

				float p = hsb.B * (1.0f - hsb.S);
				float q = hsb.B * (1.0f - (hsb.S * fractionalSector));
				float t = hsb.B * (1.0f - (hsb.S * (1 - fractionalSector)));

				// assign the fractional colors to r, g, and b based on the sector

				// the angle is in.

				switch (sectorNumber) {
				case 0:
					r = hsb.B;
					g = t;
					b = p;
					break;
				case 1:
					r = q;
					g = hsb.B;
					b = p;
					break;
				case 2:
					r = p;
					g = hsb.B;
					b = t;
					break;
				case 3:
					r = p;
					g = q;
					b = hsb.B;
					break;
				case 4:
					r = t;
					g = p;
					b = hsb.B;
					break;
				case 5:
					r = hsb.B;
					g = p;
					b = q;
					break;
				}
			}
			
			this.R = r;
			this.G = g;
			this.B = b;
			this.A = hsb.A;
		}
		
		/// <summary>
		/// Gets or sets the red (0-1)
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
		/// Gets or sets the alpha (0-1)
		/// </summary>
		public float A { get; set; }

		public override bool Equals (object obj)
		{
			return obj is Color && this == (Color)obj;
		}

		public override int GetHashCode ()
		{
			return R.GetHashCode () ^ G.GetHashCode () ^ B.GetHashCode () ^ A.GetHashCode ();
		}

		public static bool operator == (Color x, Color y)
		{
			return x.B == y.B && x.R == y.R && x.G == y.G && x.A == y.A;
		}

		public static bool operator != (Color x, Color y)
		{
			return !(x == y);
		}

		public void Invert ()
		{
			R = 1f - R;
			G = 1f - G;
			B = 1f - B;
		}

		public uint ToArgb ()
		{
			return ((uint)(B * 255) | (uint)(G * 255) << 8 | (uint)(R * 255) << 16 | (uint)(A * 255) << 24);
		}
	}
}
