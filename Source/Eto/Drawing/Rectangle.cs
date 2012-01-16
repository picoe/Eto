using System;
using System.ComponentModel;

namespace Eto.Drawing
{
	[TypeConverter(typeof(RectangleConverter))]
	public struct Rectangle
	{
		int x;
		int y;
		int width;
		int height;

		public static Rectangle Round(RectangleF rectangle)
		{
			return new Rectangle((int)Math.Round(rectangle.X), (int)Math.Round(rectangle.Y), (int)Math.Round(rectangle.Width), (int)Math.Round(rectangle.Height));
		}

		public static Rectangle Ceiling(RectangleF rectangle)
		{
			return new Rectangle((int)Math.Truncate(rectangle.X), (int)Math.Truncate(rectangle.Y), (int)Math.Ceiling(rectangle.Width), (int)Math.Ceiling(rectangle.Height));
		}

		
		public static Rectangle Truncate(RectangleF rectangle)
		{
			return new Rectangle((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height);
		}

		/* COMMON */

		public static readonly Rectangle Empty = new Rectangle(0, 0, 0, 0);
		
		public void Restrict(Point point, Size size)
		{
			Restrict(new Rectangle(point, size));
		}

		public void Restrict(Size size)
		{
			Restrict(new Rectangle(size));
		}
		
		public void Restrict(Rectangle rectangle)
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
				int old = x;
				x = x+width;
				width = old-x+1;
			}
			if (height < 0)
			{
				int old = y;
				y = y+height;
				height = old-y+1;
			}
		}


		public Rectangle(Point start, Point end)
		{
			this.x = start.X;
			this.y = start.Y;
			this.width = (end.X >= start.X) ? end.X - start.X + 1 : end.X - start.X;
			this.height = (end.Y >= start.Y) ? end.Y - start.Y + 1: end.Y - start.Y;
		}

		public Rectangle(Point point, Size size)
		{
			this.x = point.X;
			this.y = point.Y;
			this.width = size.Width;
			this.height = size.Height;
		}

		public Rectangle(Size size)
		{
			this.x = 0;
			this.y = 0;
			this.width = size.Width;
			this.height = size.Height;
		}
		
		public Rectangle(int x, int y, int width, int height)
		{
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
		}

		public void Offset(int x, int y)
		{
			this.x += x;
			this.y += y;
		}
		public void Offset(Size size)
		{
			this.x += size.Width;
			this.y += size.Height;
		}
		public bool Contains(Point point)
		{
			return Contains(point.X, point.Y);
		}
		
		public bool Contains(int x, int y)
		{
			if (Width == 0 || height == 0) return false;
			return (x >= Left && x <= Right && y >= Top && y <= Bottom);
		}
		
		public bool Intersects(Rectangle rect)
		{
			return Right >= rect.Left && Left <= rect.Right && Bottom >= rect.Top && Top <= rect.Bottom;
		}

		public static Rectangle Union(Rectangle rect1, Rectangle rect2)
		{
			Rectangle rect = rect1;
			if (rect2.Left < rect.Left) rect.Left = rect2.Left;
			if (rect2.Top < rect.Top) rect.Top = rect2.Top;
			if (rect2.Right > rect.Right) rect.Right = rect2.Right;
			if (rect2.Bottom > rect.Bottom) rect.Bottom = rect2.Bottom;
			return rect;
		}
		
		public bool IsZero
		{
			get { return width == 0 && height == 0 && x == 0 && y == 0; }
		}
		
		public bool IsEmpty
		{
			get { return width==0 || height==0; }
		}
		
		public Point Location
		{
			get { return new Point(x, y); }
			set { x = value.X; y = value.Y; }
		}
		
		public Point EndLocation
		{
			get
			{
				int xx = (width > 0) ? x+width-1 : x+width;
				int yy = (height > 0) ? y+height-1 : y+height;
				return new Point(xx, yy);
			}
			set
			{
				width = (value.X >= x) ? (value.X - x) + 1 : value.X - x;
				height = (value.Y >= y) ? (value.Y - y) + 1 : value.Y - y;
			}
		}

		public Size Size
		{
			get { return new Size(width, height); }
			set { width = value.Width; height = value.Height; }
		}
		
		public Point TopLeft
		{
			get { return new Point(Left, Top); }
			set { Top = value.Y; Left = value.X; }
		}

		public Point TopRight
		{
			get { return new Point(Right, Top); }
			set { Top = value.Y; Right = value.X; }
		}
		
		public Point BottomRight
		{
			get { return new Point(Right, Bottom); }
			set { Bottom = value.Y; Right = value.X; }
		}

		public Point BottomLeft
		{
			get { return new Point(Left, Bottom); }
			set { Bottom = value.Y; Left = value.X; }
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

		public int Width
		{
			get { return width; }
			set { width = value; }
		}

		public int Height
		{
			get { return height; }
			set { height = value; }
		}

		#region Positional methods
		public int Top
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

		public int Left
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

		public int Bottom
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

		public int Right
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
		
		public Point Center
		{
			get { return new Point(MiddleX, MiddleY); }
			set { 
				MiddleX = value.X;
				MiddleY = value.Y;
			}
		}

		public int MiddleX
		{
			get { return x + (Size.Width/2); }
			set { x = value - (Size.Width/2); }
		}
		public int MiddleY
		{
			get { return y + (Size.Height/2); }
			set { y = value - (Size.Height/2); }
		}
		#endregion

		public void Inflate(Size size)
		{
			Inflate(size.Width, size.Height);
		}

		public void Inflate(int x, int y)
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
		
		public void Align(Size size)
		{
			Align(size.Width, size.Height);
		}

		
		public void Align(int xofs, int yofs)
		{
			Top = Top - (Top % yofs);
			Left = Left - (Left % xofs);
			Right = Right + xofs - (Right % xofs);
			Bottom = Bottom + yofs - (Bottom % yofs);
		}		

		public static Rectangle operator *(Rectangle rect, int multiply)
		{
			var rect2 = rect;
			rect2.X *= multiply;
			rect2.Y *= multiply;
			rect2.Width *= multiply;
			rect2.Height *= multiply;
			return rect2;
		}
		
		public static Rectangle operator *(Rectangle rect, Size size)
		{
			var rect2 = rect;
			rect2.X *= size.Width;
			rect2.Y *= size.Height;
			rect2.Width *= size.Width;
			rect2.Height *= size.Height;
			return rect2;
		}

		public static Rectangle operator /(Rectangle rect, Size size)
		{
			var rect2 = rect;
			rect2.X /= size.Width;
			rect2.Y /= size.Height;
			rect2.Width /= size.Width;
			rect2.Height /= size.Height;
			return rect2;
		}
		
		public static bool operator ==(Rectangle rect1, Rectangle rect2)
		{
			return rect1.Equals(rect2);
		}

		public static bool operator !=(Rectangle rect1, Rectangle rect2)
		{
			return !rect1.Equals(rect2);
		}
		

		public override string ToString()
		{
			return String.Format("X={0} Y={1} Width={2} Height={3}", x, y, width, height);
		}
		
		public override bool Equals(Object o)
		{
			Rectangle rect = (Rectangle)o;
			return (x == rect.x && y == rect.y && width == rect.width && height == rect.height);
		}
		
		public override int GetHashCode()
		{
			return x.GetHashCode() ^ y.GetHashCode() ^ width.GetHashCode() ^ height.GetHashCode();
		}
	}
}
