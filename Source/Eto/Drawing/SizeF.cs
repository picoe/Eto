using System;
using System.ComponentModel;

namespace Eto.Drawing
{
	[TypeConverter(typeof(SizeFConverter))]
	public struct SizeF : IEquatable<SizeF>
	{
		public float Width { get; set; }

		public float Height { get; set; }

		public static readonly SizeF Empty = new SizeF (0, 0);


		public static SizeF Min (SizeF size1, SizeF size2)
		{
			return new SizeF (Math.Min (size1.Width, size2.Width), Math.Min (size1.Height, size2.Height));
		}

		public static SizeF Max (SizeF size1, SizeF size2)
		{
			return new SizeF (Math.Max (size1.Width, size2.Width), Math.Max (size1.Height, size2.Height));
		}

		public static SizeF Abs (SizeF size)
		{
			return new SizeF(Math.Abs (size.Width), Math.Abs (size.Height));
		}


		public SizeF (float width, float height)
			: this()
		{
			this.Width = width;
			this.Height = height;
		}

		public SizeF (PointF point)
			: this(point.X, point.Y)
		{
		}
		
		public bool Contains (PointF point)
		{
			return Contains (point.X, point.Y);
		}
		
		public bool Contains (float x, float y)
		{
			if (Width == 0 || Height == 0)
				return false;
			return (x >= 0 && x <= Width && y >= 0 && y <= Height);
		}
		
		public bool IsEmpty {
			get { return Width == 0 || Height == 0; }
		}
		
		public static SizeF operator * (SizeF size1, SizeF size2)
		{
			SizeF result = size1;
			result.Width = size1.Width * size2.Width;
			result.Height = size1.Height * size2.Height;
			return result;
		}

		public static SizeF operator * (SizeF size1, float multiplier)
		{
			SizeF result = size1;
			result.Width = size1.Width * multiplier;
			result.Height = size1.Height * multiplier;
			return result;
		}
		
		public static SizeF operator / (SizeF size1, SizeF size2)
		{
			SizeF result = size1;
			result.Width = size1.Width / size2.Width;
			result.Height = size1.Height / size2.Height;
			return result;
		}

		public static SizeF operator / (SizeF size1, float divider)
		{
			SizeF result = size1;
			result.Width = size1.Width / divider;
			result.Height = size1.Height / divider;
			return result;
		}
		
		public static SizeF operator + (SizeF size1, SizeF size2)
		{
			SizeF result = size1;
			result.Width = size1.Width + size2.Width;
			result.Height = size1.Height + size2.Height;
			return result;
		}

		public static SizeF operator - (SizeF size1, float amount)
		{
			return new SizeF (size1.Width - amount, size1.Height - amount);
		}

		public static SizeF operator + (SizeF size1, float amount)
		{
			return new SizeF (size1.Width + amount, size1.Height + amount);
		}

		public static bool operator == (SizeF size1, SizeF size2)
		{
			return (size1.Width == size2.Width && size1.Height == size2.Height);
		}

		public static bool operator != (SizeF size1, SizeF size2)
		{
			return (size1.Width != size2.Width || size1.Height != size2.Height);
		}

		public static implicit operator SizeF (Size size)
		{
			return new SizeF (size.Width, size.Height);
		}

		public override bool Equals (object obj)
		{
			return obj is SizeF && (SizeF)obj == this;
		}

		public override int GetHashCode ()
		{
			return Width.GetHashCode () ^ Height.GetHashCode ();
		}

		public override string ToString ()
		{
			return String.Format ("Width={0} Height={1}", Width, Height);
		}

		public bool Equals (SizeF other)
		{
			return other == this;
		}
	}
}
