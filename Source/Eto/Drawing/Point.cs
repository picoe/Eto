using System;
using System.ComponentModel;

namespace Eto.Drawing
{
	[TypeConverter(typeof(PointConverter))]
	public struct Point
	{
		int x;
		int y;
		
		public static readonly Point Empty  = new Point(0, 0);
		
		public static double Distance(Point point1, Point point2)
		{
			return Math.Sqrt(Math.Abs(point1.X - point2.X) + Math.Abs (point1.Y - point2.Y));
		}
		
		public static Point Truncate(PointF point)
		{
			return new Point((int)point.X, (int)point.Y);
		}

		public static Point Round(PointF point)
		{
			return new Point((int)Math.Round(point.X), (int)Math.Round(point.Y));
		}
		
		public static Point Add(Point point, Size size)
		{
			return new Point(point.X + size.Width, point.Y + size.Height);
		}
		
		public static Point Min (Point point1, Point point2)
		{
			return new Point(Math.Min (point1.X, point2.X), Math.Min (point1.Y, point2.Y));
		}

		public static Point Max (Point point1, Point point2)
		{
			return new Point(Math.Max (point1.X, point2.X), Math.Max (point1.Y, point2.Y));
		}
		
		public static Point Abs (Point point)
		{
			return new Point(Math.Abs (point.X), Math.Abs (point.Y));
		}
		
		public Point(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public Point (Size size)
		{
			this.x = size.Width;
			this.y = size.Height;
		}
		
		public Point(PointF point)
		{
			this.x = (int)point.X;
			this.y = (int)point.Y;
		}
		
		public int X
		{
			get { return x; }
			set { x = value; }
		}

		public int Y
		{
			get { return y; }
			set { y = value; }
		}
		
		public void Restrict(Rectangle rectangle)
		{
			if (x < rectangle.Left) x = rectangle.Left;
			if (x > rectangle.Right) x = rectangle.Right;
			if (y < rectangle.Top) y = rectangle.Top;
			if (y > rectangle.Bottom) y = rectangle.Bottom;
		}
		
		public void Add(int x, int y)
		{
			this.x += x;
			this.y += y;
		}

		public void Add(Point val)
		{
			this.Add (val.X, val.Y);
		}
		
		public static Point operator - (Point point1, Point point2)
		{
			return new Point(point1.x - point2.x, point1.y - point2.y);
		}

		public static Point operator + (Point point1, Point point2)
		{
			return new Point(point1.x + point2.x, point1.y + point2.y);
		}

		public static Point operator + (Point point, Size size)
		{
			return new Point(point.x + size.Width, point.y + size.Height);
		}

		public static Point operator - (Point point, Size size)
		{
			return new Point(point.x - size.Width, point.y - size.Height);
		}

		public static Point operator + (Point point, int size)
		{
			return new Point (point.x + size, point.y + size);
		}

		public static Point operator - (Point point, int size)
		{
			return new Point (point.x - size, point.y - size);
		}
		
		public static bool operator==(Point p1, Point p2)
		{
			return p1.x == p2.x && p1.y == p2.y;
		}

		public static bool operator!=(Point p1, Point p2)
		{
			return p1.x != p2.x || p1.y != p2.y;
		}

		public static Point operator *(Point point, Size size)
		{
			var result = point;
			result.x *= size.Width;
			result.y *= size.Height;
			return result;
		}

		public static Point operator /(Point point, Size size)
		{
			var result = point;
			result.x /= size.Width;
			result.y /= size.Height;
			return result;
		}

		public static Point operator *(Point point, int size)
		{
			var result = point;
			result.x *= size;
			result.y *= size;
			return result;
		}

		public static Point operator /(Point point, int size)
		{
			var result = point;
			result.x /= size;
			result.y /= size;
			return result;
		}
		
		public override bool Equals(object obj)
		{
			if (!(obj is Point)) return false;
			Point p = (Point)obj;
			return (x == p.x && y == p.y);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}

		public override string ToString()
		{
			return String.Format("X={0} Y={1}", x, y);
		}

	}
}
