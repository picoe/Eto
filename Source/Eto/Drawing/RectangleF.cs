using System;
using System.ComponentModel;

namespace Eto.Drawing
{
	[TypeConverter(typeof(RectangleFConverter))]
	public struct RectangleF : IEquatable<RectangleF>
	{
		PointF location;
		SizeF size;

		public static float InnerOffset = 1.0f;
		
		public static implicit operator RectangleF(Rectangle rect)
		{
			return new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
		}		
		
		/* COMMON */

		public static readonly RectangleF Empty = new RectangleF (0, 0, 0, 0);

		public void Restrict (PointF PointF, SizeF SizeF)
		{
			Restrict (new RectangleF (PointF, SizeF));
		}

		public void Restrict (SizeF SizeF)
		{
			Restrict (new RectangleF (SizeF));
		}

		public void Restrict (RectangleF RectangleF)
		{
			if (Left < RectangleF.Left) Left = RectangleF.Left;
			if (Top < RectangleF.Top) Top = RectangleF.Top;
			if (Right > RectangleF.Right) Right = RectangleF.Right;
			if (Bottom > RectangleF.Bottom) Bottom = RectangleF.Bottom;
		}

		public void Normalize ()
		{
			if (Width < 0) {
				float old = X;
				X += size.Width;
				size.Width = old - X + 1;
			}
			if (Height < 0) {
				float old = Y;
				Y += Height;
				Height = old - Y + 1;
			}
		}


		public RectangleF (PointF start, PointF end)
		{
			location = start;
			size = new SizeF((end.X >= start.X) ? end.X - start.X + 1 : end.X - start.X, (end.Y >= start.Y) ? end.Y - start.Y + 1: end.Y - start.Y);
		}

		public RectangleF (PointF location, SizeF size)
		{
			this.location = location;
			this.size = size;
		}

		public RectangleF (SizeF size)
		{
			this.location = new PointF (0, 0);
			this.size = size;
		}

		public RectangleF (float x, float y, float width, float height)
		{
			this.location = new PointF (x, y);
			this.size = new SizeF (width, height);
		}

		public void Offset (float x, float y)
		{
			this.location.X += x;
			this.location.Y += y;
		}
		public void Offset (SizeF SizeF)
		{
			this.location.X += SizeF.Width;
			this.location.Y += SizeF.Height;
		}
		public bool Contains (PointF PointF)
		{
			return Contains (PointF.X, PointF.Y);
		}

		public bool Contains (float x, float y)
		{
			if (Width == 0 || Height == 0) return false;
			return (x >= Left && x <= InnerRight && y >= Top && y <= InnerBottom);
		}

		public bool Intersects (RectangleF rect)
		{
			return rect.X < this.X + this.Width && this.X < rect.X + rect.Width && rect.Y < this.Y + this.Height && this.Y < rect.Y + rect.Height;
		}

		public static RectangleF Union (RectangleF rect1, RectangleF rect2)
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
			get { return location.IsEmpty && size.IsEmpty; }
		}

		public bool IsEmpty
		{
			get { return size.IsEmpty; }
		}

		public PointF Location
		{
			get { return location; }
			set { location = value; }
		}

		public PointF EndLocation
		{
			get
			{
				float xx = (Width > 0) ? X + Width - 1 : X + Width;
				float yy = (Height > 0) ? Y + Height - 1 : Y + Height;
				return new PointF (xx, yy);
			}
			set
			{
				Width = (value.X >= X) ? (value.X - X) + 1 : value.X - X;
				Height = (value.Y >= Y) ? (value.Y - Y) + 1 : value.Y - Y;
			}
		}

		public SizeF Size
		{
			get { return size; }
			set { size = value; }
		}

		public float X
		{
			get { return location.X; }
			set { location.X = value; }
		}

		public float Y
		{
			get { return location.Y; }
			set { location.Y = value; }
		}

		public float Width
		{
			get { return size.Width; }
			set { size.Width = value; }
		}

		public float Height
		{
			get { return size.Height; }
			set { size.Height = value; }
		}

		#region Positional methods

		public float Top
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

		public float Left
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

		public float Right
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

		public float Bottom
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

		public PointF TopLeft
		{
			get { return new PointF (Left, Top); }
			set { Top = value.Y; Left = value.X; }
		}

		public PointF TopRight
		{
			get { return new PointF (Right, Top); }
			set { Top = value.Y; Right = value.X; }
		}

		public PointF BottomRight
		{
			get { return new PointF (Right, Bottom); }
			set { Bottom = value.Y; Right = value.X; }
		}

		public PointF BottomLeft
		{
			get { return new PointF (Left, Bottom); }
			set { Bottom = value.Y; Left = value.X; }
		}

		#endregion

		#region Inner Positional Methods

		public PointF InnerTopRight
		{
			get { return new PointF (InnerRight, Top); }
			set { Top = value.Y; InnerRight = value.X; }
		}

		public PointF InnerBottomRight
		{
			get { return new PointF (InnerRight, InnerBottom); }
			set { InnerBottom = value.Y; InnerRight = value.X; }
		}

		public PointF InnerBottomLeft
		{
			get { return new PointF (Left, InnerBottom); }
			set { InnerBottom = value.Y; Left = value.X; }
		}

		public float InnerBottom
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

		public float InnerRight
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

		public PointF Center
		{
			get { return new PointF (MiddleX, MiddleY); }
			set
			{
				MiddleX = value.X;
				MiddleY = value.Y;
			}
		}

		public float MiddleX
		{
			get { return X + (this.Width / 2); }
			set { X = value - (this.Width / 2); }
		}

		public float MiddleY
		{
			get { return Y + (this.Height / 2); }
			set { Y = value - (this.Height / 2); }
		}

		public void Inflate (SizeF SizeF)
		{
			Inflate (SizeF.Width, SizeF.Height);
		}

		public void Inflate (float x, float y)
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

		public void Align (SizeF SizeF)
		{
			Align (SizeF.Width, SizeF.Height);
		}


		public void Align (float xofs, float yofs)
		{
			Top = Top - (Top % yofs);
			Left = Left - (Left % xofs);
			Right = Right + xofs - (Right % xofs);
			Bottom = Bottom + yofs - (Bottom % yofs);
		}

		public static RectangleF operator * (RectangleF rect, float multiply)
		{
			var rect2 = rect;
			rect2.X *= multiply;
			rect2.Y *= multiply;
			rect2.Width *= multiply;
			rect2.Height *= multiply;
			return rect2;
		}

		public static RectangleF operator / (RectangleF rect, float divide)
		{
			var rect2 = rect;
			rect2.X /= divide;
			rect2.Y /= divide;
			rect2.Width /= divide;
			rect2.Height /= divide;
			return rect2;
		}

		public static RectangleF operator * (RectangleF rect, SizeF SizeF)
		{
			var rect2 = rect;
			rect2.X *= SizeF.Width;
			rect2.Y *= SizeF.Height;
			rect2.Width *= SizeF.Width;
			rect2.Height *= SizeF.Height;
			return rect2;
		}

		public static RectangleF operator / (RectangleF rect, SizeF SizeF)
		{
			var rect2 = rect;
			rect2.X /= SizeF.Width;
			rect2.Y /= SizeF.Height;
			rect2.Width /= SizeF.Width;
			rect2.Height /= SizeF.Height;
			return rect2;
		}

		public static bool operator == (RectangleF rect1, RectangleF rect2)
		{
			return rect1.Equals (rect2);
		}

		public static bool operator != (RectangleF rect1, RectangleF rect2)
		{
			return !rect1.Equals (rect2);
		}


		public override string ToString ()
		{
			return String.Format ("{0} {1}", location, size);
		}

		public override bool Equals (Object o)
		{
			if (!(o is RectangleF))
				return false;
			RectangleF other = (RectangleF)o;
			return (location == other.location && size == other.size);
		}

		public override int GetHashCode ()
		{
			return location.GetHashCode () ^ size.GetHashCode ();
		}

		public bool Equals (RectangleF other)
		{
			return (location == other.location && size == other.size);
		}
	}
}
