using System;

namespace Eto.Drawing
{
	public struct ColorHSL
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
		
		public readonly static ColorHSL Empty = new ColorHSL ();
		
		public static float Distance (ColorHSL value1, ColorHSL value2)
		{
			return (float)Math.Sqrt (Math.Pow ((value1.H - value2.H) / 360f, 2) + Math.Pow (value1.S - value2.S, 2) + Math.Pow (value1.L - value2.L, 2));
		}
		
		public ColorHSL (float hue, float saturation, float luminance, float alpha = 1f)
			: this()
		{
			H = hue;
			S = saturation;
			L = luminance;
			A = alpha;
		}
 
		public ColorHSL (Color color)
			: this()
		{
			float h = 0, s = 0, l = 0;

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
			
			this.H = h;
			this.S = s;
			this.L = l;
			this.A = color.A;
		}
		
		public override bool Equals (object obj)
		{
			return obj is ColorHSL && this == (ColorHSL)obj;
		}

		public override int GetHashCode ()
		{
			return H.GetHashCode () ^ S.GetHashCode () ^ L.GetHashCode () ^ A.GetHashCode ();
		}

		public static bool operator == (ColorHSL x, ColorHSL y)
		{
			return x.H == y.H && x.S == y.S && x.L == y.L && x.A == y.A;
		}

		public static bool operator != (ColorHSL x, ColorHSL y)
		{
			return !(x == y);
		}
		
	}
}

