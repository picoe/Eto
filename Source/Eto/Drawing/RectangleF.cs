using System;
using System.ComponentModel;

namespace Eto.Drawing
{
	[TypeConverter(typeof(RectangleFConverter))]
	public struct RectangleF
	{
		float x;
		float y;
		float width;
		float height;
		
		public static implicit operator RectangleF(Rectangle rect)
		{
			return new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
		}		
		
		/* COMMON */
		
		public static readonly RectangleF Empty = new RectangleF(0, 0, 0, 0);

		public void Restrict(PointF point, SizeF size)
		{
			Restrict(new RectangleF(point, size));
		}
		
		public void Restrict(RectangleF rectangle)
		{
			if (Left < rectangle.Left) Left = rectangle.Left;
			if (Top < rectangle.Top) Top = rectangle.Top;
			if (Right > rectangle.Right) Right = rectangle.Right;
			if (Bottom > rectangle.Bottom) Bottom = rectangle.Bottom;
		}
		
		public void Normalize()
		{
			if (width < 0)
			{
				float old = x;
				x = x+width;
				width = old-x+1;
			}
			if (height < 0)
			{
				float old = y;
				y = y+height;
				height = old-y+1;
			}
		}


		public RectangleF(PointF start, PointF end)
		{
			this.x = start.X;
			this.y = start.Y;
			this.width = (end.X >= start.X) ? end.X - start.X + 1 : end.X - start.X;
			this.height = (end.Y >= start.Y) ? end.Y - start.Y + 1: end.Y - start.Y;
		}

		public RectangleF(PointF point, SizeF size)
		{
			this.x = point.X;
			this.y = point.Y;
			this.width = size.Width;
			this.height = size.Height;
		}

		public RectangleF(SizeF size)
		{
			this.x = 0;
			this.y = 0;
			this.width = size.Width;
			this.height = size.Height;
		}
		
		public RectangleF(float x, float y, float width, float height)
		{
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
		}

		public void Offset(float x, float y)
		{
			this.x += x;
			this.y += y;
		}
		public void Offset(SizeF size)
		{
			this.x += size.Width;
			this.y += size.Height;
		}
		public bool Contains(PointF point)
		{
			return Contains(point.X, point.Y);
		}
		
		public bool Contains(float x, float y)
		{
			if (Width == 0 || height == 0) return false;
			return (x >= Left && x <= Right && y >= Top && y <= Bottom);
		}

		public bool Intersects (RectangleF rect)
		{
			return rect.X < this.X + this.Width && this.X < rect.X + rect.Width && rect.Y < this.Y + this.Height && this.Y < rect.Y + rect.Height;
		}

		public static RectangleF Union(RectangleF rect1, RectangleF rect2)
		{
			RectangleF rect = rect1;
			if (rect2.Left < rect.Left) rect.Left = rect2.Left;
			if (rect2.Top < rect.Top) rect.Top = rect2.Top;
			if (rect2.Right > rect.Right) rect.Right = rect2.Right;
			if (rect2.Bottom > rect.Bottom) rect.Bottom = rect2.Bottom;
			return rect;
		}

		public bool IsZero
		{
			get { return x == 0 && y == 0 && width==0 && height==0; }
		}
		
		public bool IsEmpty
		{
			get { return width==0 || height==0; }
		}

		public PointF Location
		{
			get { return new PointF(x, y); }
			set { x = value.X; y = value.Y; }
		}
		
		public PointF EndLocation
		{
			get
			{
				float xx = (width > 0) ? x+width-1 : x+width;
				float yy = (height > 0) ? y+height-1 : y+height;
				return new PointF(xx, yy);
			}
			set
			{
				width = (value.X >= x) ? (value.X - x) + 1 : value.X - x;
				height = (value.Y >= y) ? (value.Y - y) + 1 : value.Y - y;
			}
		}

		public SizeF Size
		{
			get { return new SizeF(width, height); }
			set { width = value.Width; height = value.Height; }
		}
		
		public PointF TopLeft
		{
			get { return new PointF(Left, Top); }
			set { Top = value.Y; Left = value.X; }
		}

		public PointF TopRight
		{
			get { return new PointF(Right, Top); }
			set { Top = value.Y; Right = value.X; }
		}
		
		public PointF BottomRight
		{
			get { return new PointF(Right, Bottom); }
			set { Bottom = value.Y; Right = value.X; }
		}

		public PointF BottomLeft
		{
			get { return new PointF(Left, Bottom); }
			set { Bottom = value.Y; Left = value.X; }
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

		public float Width
		{
			get { return width; }
			set { width = value; }
		}

		public float Height
		{
			get { return height; }
			set { height = value; }
		}

		#region Positional methods
		public float Top
		{
			get { return (height >= 0) ? y : y+height; }
			set
			{
				if (height >= 0) { 
					height += y-value; y = value;
					if (height < 0) height = 0;
				}
				else { height = value-y; }
			}
		}

		public float Left
		{
			get { return (width >= 0) ? x : x+width; }
			set
			{
				if (width >= 0) { 
					width += x-value; x = value; 
					if (width < 0) width = 0;
				}
				else { width = value-x; }
			}
		}

		public float Bottom
		{
			get { return (height > 0) ? y + height-1 : y; }
			set
			{
				if (height >= 0) {
					height = value - y + 1;
					if (height < 0) { y += height - 1; height = 0; }
				}
				else { height += y-value; y = value; }
			}
		}

		public float Right
		{
			get { return (width > 0) ? x + width-1 : x; }
			set
			{
				if (width >= 0) {
					width = value - x + 1;
					if (width < 0) { x += width - 1; width = 0; }
				}
				else { width += x-value; x = value; }
			}
		}

		public float MiddleX
		{
			get { return x + (Size.Width/2); }
			set { x = value - (Size.Width/2); }
		}
		public float MiddleY
		{
			get { return y + (Size.Height/2); }
			set { y = value - (Size.Height/2); }
		}
		#endregion

		public void Inflate(Size size)
		{
			Inflate(size.Width, size.Height);
		}

		public void Inflate(float x, float y)
		{
			if (width >= 0)
			{
				this.x -= x;
				this.width += x*2;
			}
			else
			{
				this.x += x;
				this.width -= x*2;
			}
			
			if (height >= 0)
			{
				this.y -= y;
				this.height += y*2;
			}
			else
			{
				this.y += y;
				this.height -= y*2;
			}
		}
		
		public void Align(SizeF size)
		{
			Align(size.Width, size.Height);
		}
		
		public void Align(float xofs, float yofs)
		{
			Top = Top - (Top % yofs);
			Left = Left - (Left % xofs);
			Right = Right + xofs - (Right % xofs);
			Bottom = Bottom + yofs - (Bottom % yofs);
		}		
		
		public static RectangleF operator *(RectangleF rect, float multiply)
		{
			var rect2 = rect;
			rect2.X *= multiply;
			rect2.Y *= multiply;
			rect2.Width *= multiply;
			rect2.Height *= multiply;
			return rect2;
		}

		public static RectangleF operator *(RectangleF rect, SizeF size)
		{
			var rect2 = rect;
			rect2.X *= size.Width;
			rect2.Y *= size.Height;
			rect2.Width *= size.Width;
			rect2.Height *= size.Height;
			return rect2;
		}

		public static RectangleF operator /(RectangleF rect, SizeF size)
		{
			var rect2 = rect;
			rect2.X /= size.Width;
			rect2.Y /= size.Height;
			rect2.Width /= size.Width;
			rect2.Height /= size.Height;
			return rect2;
		}
		
		public static bool operator ==(RectangleF rect1, RectangleF rect2)
		{
			return rect1.Equals(rect2);
		}

		public static bool operator !=(RectangleF rect1, RectangleF rect2)
		{
			return !rect1.Equals(rect2);
		}
		
		public override string ToString()
		{
			return String.Format("X={0} Y={1} Width={2} Height={3}", x, y, width, height);
		}
		
		public override bool Equals(Object o)
		{
			var rect = (RectangleF)o;
			return (x == rect.x && y == rect.y && width == rect.width && height == rect.height);
		}
		
		public override int GetHashCode()
		{
			return x.GetHashCode() ^ y.GetHashCode() ^ width.GetHashCode() ^ height.GetHashCode();
		}
		
	}
}
