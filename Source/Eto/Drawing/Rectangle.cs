using System;
using System.ComponentModel;

namespace Eto.Drawing
{
	[TypeConverter (typeof (RectangleConverter))]
	public struct Rectangle : IEquatable<Rectangle>
	{
		Point location;
		Size size;

		const int InnerOffset = 1;

		public static Rectangle Round (RectangleF rectangle)
		{
			return new Rectangle ((int)Math.Round (rectangle.X), (int)Math.Round (rectangle.Y), (int)Math.Round (rectangle.Width), (int)Math.Round (rectangle.Height));
		}

		public static Rectangle Ceiling (RectangleF rectangle)
		{
			return new Rectangle ((int)Math.Truncate (rectangle.X), (int)Math.Truncate (rectangle.Y), (int)Math.Ceiling (rectangle.Width), (int)Math.Ceiling (rectangle.Height));
		}


		public static Rectangle Truncate (RectangleF rectangle)
		{
			return new Rectangle ((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height);
		}

		/* COMMON */


		public static readonly Rectangle Empty = new Rectangle (0, 0, 0, 0);

		public void Restrict (Point Point, Size Size)
		{
			Restrict (new Rectangle (Point, Size));
		}

		public void Restrict (Size Size)
		{
			Restrict (new Rectangle (Size));
		}

		public void Restrict (Rectangle Rectangle)
		{
			if (Left < Rectangle.Left) Left = Rectangle.Left;
			if (Top < Rectangle.Top) Top = Rectangle.Top;
			if (Right > Rectangle.Right) Right = Rectangle.Right;
			if (Bottom > Rectangle.Bottom) Bottom = Rectangle.Bottom;
		}

		public void Normalize ()
		{
			if (Width < 0) {
				int old = X;
				X += size.Width;
				size.Width = old - X + 1;
			}
			if (Height < 0) {
				int old = Y;
				Y += Height;
				Height = old - Y + 1;
			}
		}


		public Rectangle (Point start, Point end)
		{
			location = start;
			size = new Size((end.X >= start.X) ? end.X - start.X + 1 : end.X - start.X, (end.Y >= start.Y) ? end.Y - start.Y + 1: end.Y - start.Y);
		}

		public Rectangle (Point location, Size size)
		{
			this.location = location;
			this.size = size;
		}

		public Rectangle (Size size)
		{
			this.location = new Point (0, 0);
			this.size = size;
		}

		public Rectangle (int x, int y, int width, int height)
		{
			this.location = new Point (x, y);
			this.size = new Size (width, height);
		}

		public void Offset (int x, int y)
		{
			this.location.X += x;
			this.location.Y += y;
		}
		public void Offset (Size Size)
		{
			this.location.X += Size.Width;
			this.location.Y += Size.Height;
		}
		public bool Contains (Point Point)
		{
			return Contains (Point.X, Point.Y);
		}

		public bool Contains (int x, int y)
		{
			if (Width == 0 || Height == 0) return false;
			return (x >= Left && x <= InnerRight && y >= Top && y <= InnerBottom);
		}

		public bool Intersects (Rectangle rect)
		{
			return rect.X < this.X + this.Width && this.X < rect.X + rect.Width && rect.Y < this.Y + this.Height && this.Y < rect.Y + rect.Height;
		}

		public static Rectangle Union (Rectangle rect1, Rectangle rect2)
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
			get { return location.IsEmpty && size.IsEmpty; }
		}

		public bool IsEmpty
		{
			get { return size.IsEmpty; }
		}

		public Point Location
		{
			get { return location; }
			set { location = value; }
		}

		public Point EndLocation
		{
			get
			{
				int xx = (Width > 0) ? X + Width - 1 : X + Width;
				int yy = (Height > 0) ? Y + Height - 1 : Y + Height;
				return new Point (xx, yy);
			}
			set
			{
				Width = (value.X >= X) ? (value.X - X) + 1 : value.X - X;
				Height = (value.Y >= Y) ? (value.Y - Y) + 1 : value.Y - Y;
			}
		}

		public Size Size
		{
			get { return size; }
			set { size = value; }
		}

		public int X
		{
			get { return location.X; }
			set { location.X = value; }
		}

		public int Y
		{
			get { return location.Y; }
			set { location.Y = value; }
		}

		public int Width
		{
			get { return size.Width; }
			set { size.Width = value; }
		}

		public int Height
		{
			get { return size.Height; }
			set { size.Height = value; }
		}

		#region Positional methods

		public int Top
		{
			get { return (Height >= 0) ? Y : Y + Height; }
			set
			{
				if (Height >= 0) {
					Height += Y - value;
					Y = value;
					if (Height < 0) Height = 0;
				}
				else { Height = value - Y; }
			}
		}

		public int Left
		{
			get { return (Width >= 0) ? X : X + Width; }
			set
			{
				if (Width >= 0) {
					Width += X - value; X = value;
					if (Width < 0) Width = 0;
				}
				else { Width = value - X; }
			}
		}

		public int Right
		{
			get { return (Width >= 0) ? X + Width : X + 1; }
			set
			{
				if (Width >= 0) {
					Width = value - X;
					if (Width < 0) { X += Width; Width = 0; }
				}
				else {
					Width += Right - value;
					X = value - 1;
				}
			}
		}

		public int Bottom
		{
			get { return (Height >= 0) ? Y + Height : Y + 1; }
			set
			{
				if (Height >= 0) {
					Height = value - Y;
					if (Height < 0) { Y += Height; Height = 0; }
				}
				else {
					Height += Bottom - value;
					Y = value - 1;
				}
			}
		}

		public Point TopLeft
		{
			get { return new Point (Left, Top); }
			set { Top = value.Y; Left = value.X; }
		}

		public Point TopRight
		{
			get { return new Point (Right, Top); }
			set { Top = value.Y; Right = value.X; }
		}

		public Point BottomRight
		{
			get { return new Point (Right, Bottom); }
			set { Bottom = value.Y; Right = value.X; }
		}

		public Point BottomLeft
		{
			get { return new Point (Left, Bottom); }
			set { Bottom = value.Y; Left = value.X; }
		}

		#endregion

		#region Inner Positional Methods

		public Point InnerTopRight
		{
			get { return new Point (InnerRight, Top); }
			set { Top = value.Y; InnerRight = value.X; }
		}

		public Point InnerBottomRight
		{
			get { return new Point (InnerRight, InnerBottom); }
			set { InnerBottom = value.Y; InnerRight = value.X; }
		}

		public Point InnerBottomLeft
		{
			get { return new Point (Left, InnerBottom); }
			set { InnerBottom = value.Y; Left = value.X; }
		}

		public int InnerBottom
		{
			get { return (Height > 0) ? Y + Height - InnerOffset : Y; }
			set
			{
				if (Height >= 0) {
					Height = value - Y + InnerOffset;
					if (Height < 0) { Y += Height - InnerOffset; Height = 0; }
				}
				else { Height += Y - value; Y = value; }
			}
		}

		public int InnerRight
		{
			get { return (Width > 0) ? X + Width - InnerOffset : X; }
			set
			{
				if (Width >= 0) {
					Width = value - X + InnerOffset;
					if (Width < 0) { X += Width - InnerOffset; Width = 0; }
				}
				else { Width += X - value; X = value; }
			}
		}

		#endregion

		public Point Center
		{
			get { return new Point (MiddleX, MiddleY); }
			set
			{
				MiddleX = value.X;
				MiddleY = value.Y;
			}
		}

		public int MiddleX
		{
			get { return X + (this.Width / 2); }
			set { X = value - (this.Width / 2); }
		}

		public int MiddleY
		{
			get { return Y + (this.Height / 2); }
			set { Y = value - (this.Height / 2); }
		}

		public void Inflate (Size Size)
		{
			Inflate (Size.Width, Size.Height);
		}

		public void Inflate (int x, int y)
		{
			if (Width >= 0) {
				this.X -= x;
				this.Width += x * 2;
			}
			else {
				this.X += x;
				this.Width -= x * 2;
			}

			if (Height >= 0) {
				this.Y -= y;
				this.Height += y * 2;
			}
			else {
				this.Y += y;
				this.Height -= y * 2;
			}
		}

		public void Align (Size Size)
		{
			Align (Size.Width, Size.Height);
		}


		public void Align (int xofs, int yofs)
		{
			Top = Top - (Top % yofs);
			Left = Left - (Left % xofs);
			Right = Right + xofs - (Right % xofs);
			Bottom = Bottom + yofs - (Bottom % yofs);
		}

		public static Rectangle operator * (Rectangle rect, int multiply)
		{
			var rect2 = rect;
			rect2.X *= multiply;
			rect2.Y *= multiply;
			rect2.Width *= multiply;
			rect2.Height *= multiply;
			return rect2;
		}

		public static Rectangle operator / (Rectangle rect, int divide)
		{
			var rect2 = rect;
			rect2.X /= divide;
			rect2.Y /= divide;
			rect2.Width /= divide;
			rect2.Height /= divide;
			return rect2;
		}

		public static Rectangle operator * (Rectangle rect, Size Size)
		{
			var rect2 = rect;
			rect2.X *= Size.Width;
			rect2.Y *= Size.Height;
			rect2.Width *= Size.Width;
			rect2.Height *= Size.Height;
			return rect2;
		}

		public static Rectangle operator / (Rectangle rect, Size Size)
		{
			var rect2 = rect;
			rect2.X /= Size.Width;
			rect2.Y /= Size.Height;
			rect2.Width /= Size.Width;
			rect2.Height /= Size.Height;
			return rect2;
		}

		public static bool operator == (Rectangle rect1, Rectangle rect2)
		{
			return rect1.Equals (rect2);
		}

		public static bool operator != (Rectangle rect1, Rectangle rect2)
		{
			return !rect1.Equals (rect2);
		}


		public override string ToString ()
		{
			return String.Format ("{0} {1}", location, size);
		}

		public override bool Equals (Object o)
		{
			if (!(o is Rectangle))
				return false;
			Rectangle other = (Rectangle)o;
			return (location == other.location && size == other.size);
		}

		public override int GetHashCode ()
		{
			return location.GetHashCode () ^ size.GetHashCode ();
		}

		public bool Equals (Rectangle other)
		{
			return (location == other.location && size == other.size);
		}
	}
}
