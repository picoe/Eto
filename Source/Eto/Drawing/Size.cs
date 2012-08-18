using System;
using System.ComponentModel;

namespace Eto.Drawing
{
	[TypeConverter(typeof(SizeConverter))]
	public struct Size : IEquatable<Size>
	{
		public int Width { get; set; }

		public int Height { get; set; }
		
		public static readonly Size Empty = new Size (0, 0);

		public static Size Round (SizeF size)
		{
			return new Size ((int)Math.Round (size.Width), (int)Math.Round (size.Height));
		}

		public static Size Truncate (SizeF size)
		{
			return new Size ((int)size.Width, (int)size.Height);
		}
		
		public static Size Min (Size size1, Size size2)
		{
			return new Size (Math.Min (size1.Width, size2.Width), Math.Min (size1.Height, size2.Height));
		}

		public static Size Max (Size size1, Size size2)
		{
			return new Size (Math.Max (size1.Width, size2.Width), Math.Max (size1.Height, size2.Height));
		}

		public static Size Abs (Size size)
		{
			return new Size (Math.Abs (size.Width), Math.Abs (size.Height));
		}
		
		public Size (int width, int height)
			: this()
		{
			Width = width;
			Height = height;
		}
		
		public Size (Point point)
			: this(point.X, point.Y)
		{
		}
		
		public bool Contains (Point point)
		{
			return Contains (point.X, point.Y);
		}
		
		public bool Contains (int x, int y)
		{
			if (Width == 0 || Height == 0)
				return false;
			return (x >= 0 && x <= Width && y >= 0 && y <= Height);
		}
		
		public bool IsEmpty {
			get { return Width == 0 || Height == 0; }
		}

		public static Size operator * (Size size1, Size size2)
		{
			Size result = size1;
			result.Width = size1.Width * size2.Width;
			result.Height = size1.Height * size2.Height;
			return result;
		}

		public static Size operator * (Size size1, int multiplier)
		{
			Size result = size1;
			result.Width = size1.Width * multiplier;
			result.Height = size1.Height * multiplier;
			return result;
		}
		
		public static Size operator / (Size size1, Size size2)
		{
			Size result = size1;
			result.Width = size1.Width / size2.Width;
			result.Height = size1.Height / size2.Height;
			return result;
		}

		public static Size operator / (Size size1, int divider)
		{
			Size result = size1;
			result.Width = size1.Width / divider;
			result.Height = size1.Height / divider;
			return result;
		}
		
		public static Size operator + (Size size1, Size size2)
		{
			Size result = size1;
			result.Width = size1.Width + size2.Width;
			result.Height = size1.Height + size2.Height;
			return result;
		}

		public static Size operator - (Size size1, Size size2)
		{
			Size result = size1;
			result.Width = size1.Width - size2.Width;
			result.Height = size1.Height - size2.Height;
			return result;
		}

		public static Size operator - (Size size1, int amount)
		{
			return new Size (size1.Width - amount, size1.Height - amount);
		}

		public static Size operator + (Size size1, int amount)
		{
			return new Size (size1.Width + amount, size1.Height + amount);
		}
		
		public static bool operator == (Size size1, Size size2)
		{
			return (size1.Width == size2.Width && size1.Height == size2.Height);
		}

		public static bool operator != (Size size1, Size size2)
		{
			return (size1.Width != size2.Width || size1.Height != size2.Height);
		}

		public override bool Equals (object obj)
		{
			if (!(obj is Size))
				return false;
			Size other = (Size)obj;
			return (Width == other.Width && Height == other.Height);
		}

		public override int GetHashCode ()
		{
			return Width ^ Height;
		}

		public override string ToString ()
		{
			return String.Format ("Width={0} Height={1}", Width, Height);
		}

		public bool Equals (Size other)
		{
			return (Width == other.Width && Height == other.Height);
		}
	}
}
