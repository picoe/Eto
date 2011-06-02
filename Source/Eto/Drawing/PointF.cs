using System;

namespace Eto.Drawing
{
	public struct PointF
	{
		private float x;
		private float y;
 
		public static readonly PointF Empty = new PointF(0, 0);

		public static PointF Add(PointF point, SizeF size)
		{
			return new PointF(point.X + size.Width, point.Y + size.Height);
		}
		
		public PointF(float x, float y)
		{
			this.x = x;
			this.y = y;
		}

		public float X
		{
			get { return x; }
			set { x = value; }
		}

		public float Y
		{
			get { return y; }
			set { y = value; }
		}

		public static PointF operator - (PointF point1, PointF point2)
		{
			return new PointF(point1.x - point2.x, point1.y - point2.y);
		}

		public static PointF operator + (PointF point, Size size)
		{
			return new PointF(point.x + size.Width, point.y + size.Height);
		}

		public static bool operator==(PointF p1, PointF p2)
		{
			return p1.x == p2.x && p1.y == p2.y;
		}

		public static bool operator!=(PointF p1, PointF p2)
		{
			return p1.x != p2.x || p1.y != p2.y;
		}
		
		public static PointF operator *(PointF point, SizeF size)
		{
			PointF result = point;
			result.x *= size.Width;
			result.y *= size.Height;
			return result;
		}

		public static PointF operator /(PointF point, SizeF size)
		{
			PointF result = point;
			result.x /= size.Width;
			result.y /= size.Height;
			return result;
		}
		
		public static implicit operator PointF(Point point)
		{
			return new PointF(point.X, point.Y);
		}


		public override bool Equals(object obj)
		{
			if (!(obj is PointF)) return false;
			PointF p = (PointF)obj;
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
