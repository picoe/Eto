using System;

namespace Eto.Drawing
{
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

		public static bool operator == (ColorHSB x, ColorHSB y)
		{
			return x.H == y.H && x.S == y.S && x.B == y.B && x.A == y.A;
		}

		public static bool operator != (ColorHSB x, ColorHSB y)
		{
			return !(x == y);
		}
		
	}
}

