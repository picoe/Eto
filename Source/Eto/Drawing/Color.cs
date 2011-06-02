using System;

namespace Eto.Drawing
{
	public struct Color
	{
		public static readonly Color Black = Color.FromArgb (0, 0, 0);
		public static readonly Color White = Color.FromArgb (0xFF, 0xFF, 0xFF);
		public static readonly Color Gray = Color.FromArgb (0x77, 0x77, 0x77);
		public static readonly Color LightGray = Color.FromArgb (0xA8, 0xA8, 0xA8);
		public static readonly Color Red = Color.FromArgb (0xFF, 0, 0);
		public static readonly Color Green = Color.FromArgb (0, 0xFF, 0);
		public static readonly Color Blue = Color.FromArgb (0, 0, 0xFF);

		public static Color FromArgb (uint argb)
		{
			return new Color ((byte)((argb >> 16) & 0xff), (byte)((argb >> 8) & 0xff), (byte)(argb & 0xff), (byte)((argb >> 24) & 0xff));
		}

		public static Color FromArgb (int r, int g, int b)
		{
			return new Color ((byte)r, (byte)g, (byte)b);
		}

		public static Color FromArgb (byte r, byte g, byte b)
		{
			return new Color (r, g, b);
		}

		public static Color FromArgb (byte r, byte g, byte b, byte a)
		{
			return new Color (r, g, b, a);
		}

		public Color (byte r, byte g, byte b)
			: this()
		{
			this.R = r;
			this.G = g;
			this.B = b;
			this.A = 0xff;
		}

		public Color (byte r, byte g, byte b, byte a)
			: this()
		{
			this.R = r;
			this.G = g;
			this.B = b;
			this.A = a;
		}

		public byte R { get; set; }

		public byte G { get; set; }

		public byte B { get; set; }

		public byte A { get; set; }

		public override bool Equals (object obj)
		{
			return obj is Color && this == (Color)obj;
		}

		public override int GetHashCode ()
		{
			return R.GetHashCode () ^ G.GetHashCode() ^ B.GetHashCode() ^ A.GetHashCode();
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
			R = (byte)(255 - R);
			G = (byte)(255 - G);
			B = (byte)(255 - B);
		}

		public uint ToArgb ()
		{
			return ((uint)B | (uint)G << 8 | (uint)R << 16 | (uint)A << 24);
		}
	}
}
