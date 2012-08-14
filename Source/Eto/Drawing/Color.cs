using System;
using System.ComponentModel;
using System.Globalization;

namespace Eto.Drawing
{
	/// <summary>
	/// Represents a color with RGBA (Red, Green, Blue, and Alpha) components
	/// </summary>
	[TypeConverter (typeof (ColorConverter))]
	public struct Color
	{
		[Obsolete("Use Colors.Black")]
		public static readonly Color Black = new Color (0, 0, 0);

		[Obsolete("User Colors.White")]
		public static readonly Color White = new Color (1.0f, 1.0f, 1.0f);

		[Obsolete ("User Colors.Gray")]
		public static readonly Color Gray = new Color (0x77 / 255f, 0x77 / 255f, 0x77 / 255f);

		[Obsolete ("User Colors.DarkGray")]
		public static readonly Color LightGray = new Color (0xA8 / 255f, 0xA8 / 255f, 0xA8 / 255f);

		[Obsolete ("User Colors.Red")]
		public static readonly Color Red = new Color (1f, 0, 0);

		[Obsolete ("User Colors.Lime")]
		public static readonly Color Green = new Color (0, 1f, 0);

		[Obsolete ("User Colors.Blue")]
		public static readonly Color Blue = new Color (0, 0, 1f);

		[Obsolete ("User Colors.Transparent")]
		public static readonly Color Transparent = new Color (0, 0, 0, 0);

		/// <summary>
		/// An empty color with zero for all components
		/// </summary>
		#pragma warning disable 618
		[Obsolete("Use nullable values instead of empty color structs")]
		public static readonly Color Empty = new Color { IsEmpty = true };
		#pragma warning restore 618

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
			: this ()
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
				color = new Color (array2[0], array2[1], array2[2]);
				return true;
			case 4:
				color = new Color (array2[0], array2[1], array2[2], array2[3]);
				return true;
			}
			color = Colors.Transparent;
			return false;
		}

		[Obsolete ("Use ColorCMYK.ToColor() or implicit conversion")]
		public Color (ColorCMYK cmyk)
			: this ()
		{
			R = (1 - cmyk.C) * (1 - cmyk.K);
			G = (1 - cmyk.M) * (1 - cmyk.K);
			B = (1 - cmyk.Y) * (1 - cmyk.K);
			A = cmyk.A;
		}

		[Obsolete ("Use ColorHSL.ToColor() or implicit conversion")]
		public Color (ColorHSL hsl)
			: this ()
		{
			if (hsl.S == 0) {
				// achromatic color (gray scale)
				R = G = B = hsl.L;
			}
			else {
				float q = (hsl.L < 0.5f) ? (hsl.L * (1f + hsl.S)) : (hsl.L + hsl.S - (hsl.L * hsl.S));
				float p = (2f * hsl.L) - q;

				float Hk = hsl.H / 360f;
				float[] T = new float[3];
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

				R = T[0];
				G = T[1];
				B = T[2];
			}
			A = hsl.A;
		}

		[Obsolete ("Use ColorHSB.ToColor() or implicit conversion")]
		public Color (ColorHSB hsb)
			: this ()
		{
			float r = 0;
			float g = 0;
			float b = 0;

			if (hsb.S == 0) {
				r = g = b = 0;
			}
			else {
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

		[Obsolete("Use nullable values instead")]
		public bool IsEmpty
		{
			get;
			private set;
		}

		public void Invert ()
		{
			R = 1f - R;
			G = 1f - G;
			B = 1f - B;
		}

		public uint ToArgb ()
		{
			return ((uint)(B * byte.MaxValue) | (uint)(G * byte.MaxValue) << 8 | (uint)(R * byte.MaxValue) << 16 | (uint)(A * byte.MaxValue) << 24);
		}

		public string ToHex ()
		{
			return string.Format ("#{0:X2}{1:X2}{2:X2}{3:X2}", (byte)(A * byte.MaxValue), (byte)(R * byte.MaxValue), (byte)(G * byte.MaxValue), (byte)(B * byte.MaxValue));
		}

		public override string ToString ()
		{
			return ToHex ();
		}
	}
}
