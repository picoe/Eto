using System;
using System.ComponentModel;

namespace Eto.Drawing
{
	[TypeConverter(typeof(PointFConverter))]
	public struct PointF : IEquatable<PointF>
	{
		float x;
		float y;
		public static readonly PointF Empty = new PointF (0, 0);

		public static double Distance (Point point1, Point point2)
		{
			return Math.Sqrt (Math.Abs (point1.X - point2.X) + Math.Abs (point1.Y - point2.Y));
		}
		
		public static PointF Add (PointF point, SizeF size)
		{
			return new PointF (point.X + size.Width, point.Y + size.Height);
		}
		
		public static PointF Min (PointF point1, PointF point2)
		{
			return new PointF (Math.Min (point1.X, point2.X), Math.Min (point1.Y, point2.Y));
		}

		public static PointF Max (PointF point1, PointF point2)
		{
			return new PointF (Math.Max (point1.X, point2.X), Math.Max (point1.Y, point2.Y));
		}
		
		public static PointF Abs (PointF point)
		{
			return new PointF (Math.Abs (point.X), Math.Abs (point.Y));
		}
		
		public PointF (float x, float y)
		{
			this.x = x;
			this.y = y;
		}

		public float X {
			get { return x; }
			set { x = value; }
		}

		public float Y {
			get { return y; }
			set { y = value; }
		}

		public bool IsEmpty {
			get { return x == 0 && y == 0; }
		}
		
		public static SizeF operator - (PointF point1, PointF point2)
		{
			return new SizeF (point1.x - point2.x, point1.y - point2.y);
		}

		public static SizeF operator + (PointF point1, PointF point2)
		{
			return new SizeF (point1.x + point2.x, point1.y + point2.y);
		}
		
		public static PointF operator + (PointF point, Size size)
		{
			return new PointF (point.x + size.Width, point.y + size.Height);
		}

		public static PointF operator - (PointF point, Size size)
		{
			return new PointF (point.x - size.Width, point.y - size.Height);
		}

		public static PointF operator + (PointF point, float size)
		{
			return new PointF (point.x + size, point.y + size);
		}

		public static PointF operator - (PointF point, float size)
		{
			return new PointF (point.x - size, point.y - size);
		}

		public static bool operator == (PointF p1, PointF p2)
		{
			return p1.x == p2.x && p1.y == p2.y;
		}

		public static bool operator!= (PointF p1, PointF p2)
		{
			return p1.x != p2.x || p1.y != p2.y;
		}
		
		public static PointF operator * (PointF point, SizeF size)
		{
			PointF result = point;
			result.x *= size.Width;
			result.y *= size.Height;
			return result;
		}

		public static PointF operator / (PointF point, SizeF size)
		{
			PointF result = point;
			result.x /= size.Width;
			result.y /= size.Height;
			return result;
		}
		
		public static PointF operator * (PointF point, float size)
		{
			var result = point;
			result.x *= size;
			result.y *= size;
			return result;
		}

		public static PointF operator / (PointF point, float size)
		{
			var result = point;
			result.x /= size;
			result.y /= size;
			return result;
		}
		
		public static implicit operator PointF (Point point)
		{
			return new PointF (point.X, point.Y);
		}

		public override bool Equals (object obj)
		{
			return obj is PointF && (PointF)obj == this;
		}

		public override int GetHashCode ()
		{
			return base.GetHashCode ();
		}

		public override string ToString ()
		{
			return String.Format ("X={0} Y={1}", x, y);
		}

		public bool Equals (PointF other)
		{
			return other == this;
		}
	}
}
