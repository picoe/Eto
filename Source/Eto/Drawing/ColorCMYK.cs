using System;

namespace Eto.Drawing
{
	public struct ColorCMYK
	{
		public float C { get; set; }
		
		public float M { get; set; }
		
		public float Y { get; set; }
		
		public float K { get; set; }
		
		public float A { get; set; }
		
		public readonly static ColorCMYK Empty = new ColorCMYK ();
		
		public static float Distance (ColorCMYK value1, ColorCMYK value2)
		{
			return (float)Math.Sqrt (Math.Pow ((value1.C - value2.C), 2) + Math.Pow (value1.M - value2.M, 2) + Math.Pow (value1.Y - value2.Y, 2) + Math.Pow (value1.K - value2.K, 2));
		}
		
		public ColorCMYK (float cyan, float magenta, float yellow, float black, float alpha = 1f)
			: this()
		{
			C = cyan;
			M = magenta;
			Y = yellow;
			K = black;
			A = alpha;
		}
 
		public ColorCMYK (Color color)
			: this()
		{
			float c = 1f - color.R;
			float m = 1f - color.G;
			float y = 1f - color.B;

			float k = (float)Math.Min (c, Math.Min (m, y));

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
		
		public override bool Equals (object obj)
		{
			return obj is ColorCMYK && this == (ColorCMYK)obj;
		}

		public override int GetHashCode ()
		{
			return C.GetHashCode () ^ M.GetHashCode () ^ Y.GetHashCode () ^ K.GetHashCode () ^ A.GetHashCode ();
		}

		public static bool operator == (ColorCMYK x, ColorCMYK y)
		{
			return x.C == y.C && x.M == y.M && x.Y == y.Y && x.K == y.K && x.A == y.A;
		}

		public static bool operator != (ColorCMYK x, ColorCMYK y)
		{
			return !(x == y);
		}
		
	}
}

