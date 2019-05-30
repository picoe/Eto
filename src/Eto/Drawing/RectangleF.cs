using System;
using sc = System.ComponentModel;
using System.Globalization;

namespace Eto.Drawing
{
	/// <summary>
	/// Represents a floating point rectangle with a location (X, Y) and size (Width, Height) components.
	/// </summary>
	/// <remarks>
	/// A rectangle is defined by a location (X, Y) and a size (Width, Height).
	/// The width and/or height can be negative.
	/// </remarks>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[sc.TypeConverter (typeof(RectangleFConverterInternal))]
	public struct RectangleF : IEquatable<RectangleF>
	{
		PointF location;
		SizeF size;
		const float InnerOffset = 1.0f;
		
		/* COMMON */

		/// <summary>
		/// Gets an empty rectangle with zero X, Y, Width, and Height components
		/// </summary>
		/// <remarks>
		/// Useful when you want a rectangle no size or location.
		/// </remarks>
		public static readonly RectangleF Empty = new RectangleF (0, 0, 0, 0);

		/// <summary>
		/// Normalizes the rectangle so both the <see cref="Width"/> and <see cref="Height"/> are positive, without changing the location of the rectangle
		/// </summary>
		/// <remarks>
		/// Rectangles can have negative widths/heights, which means that the starting location will not always be at the top left
		/// corner.  Normalizing the rectangle will ensure that the <see cref="X"/> and <see cref="Y"/> co-ordinates of the rectangle
		/// are at the top left.
		/// </remarks>
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

		/// <summary>
		/// Creates a new instance of a RectangleF from the values of the <paramref name="left"/>, <paramref name="top"/>, <paramref name="right"/> and <paramref name="bottom"/> sides
		/// </summary>
		/// <param name="left">Left side of the rectangle to create</param>
		/// <param name="top">Top of the rectangle to create</param>
		/// <param name="right">Right side of the rectangle to create</param>
		/// <param name="bottom">Bottom of the rectangle to create</param>
		/// <returns>A new instance of a RectangleF with values for the Left, Top, Right, and Bottom sides</returns>
		public static RectangleF FromSides (float left, float top, float right, float bottom)
		{
			return new RectangleF (left, top, right - left, bottom - top);
		}

		/// <summary>
		/// Creates a new instance of a RectangleF with a specified <paramref name="center"/> and <paramref name="size"/>
		/// </summary>
		/// <param name="center">Center of the rectangle</param>
		/// <param name="size">Size of the rectangle</param>
		/// <returns>A new instance of a RectangleF with the specified center and size</returns>
		public static RectangleF FromCenter (PointF center, SizeF size)
		{
			return new RectangleF (center - size / 2, size);
		}

		/// <summary>
		/// Initializes a new instance of the Rectangle class with two points
		/// </summary>
		/// <param name="start">Starting point of the rectangle</param>
		/// <param name="end">Ending point of the rectangle</param>
		public RectangleF (PointF start, PointF end)
		{
			location = start;
			size = new SizeF((end.X >= start.X) ? end.X - start.X + 1 : end.X - start.X, (end.Y >= start.Y) ? end.Y - start.Y + 1: end.Y - start.Y);
		}

		/// <summary>
		/// Initializes a new instance of the Rectangle class with the specified <paramref name="location"/> and <paramref name="size"/>
		/// </summary>
		/// <param name="location">Location of the rectangle</param>
		/// <param name="size">Size of the rectangle</param>
		public RectangleF (PointF location, SizeF size)
		{
			this.location = location;
			this.size = size;
		}

		/// <summary>
		/// Initilizes a new instance of the RectangleF class with the specified <paramref name="rectangle"/>.
		/// </summary>
		/// <param name="rectangle"></param>
		public RectangleF(Rectangle rectangle)
		{
			location = new PointF(rectangle.X, rectangle.Y);
			size = new SizeF(rectangle.Width, rectangle.Height);
		}

		/// <summary>
		/// Initializes a new instance of the Rectangle class with X, Y co-ordinates at 0,0 and the specified <paramref name="size"/>
		/// </summary>
		/// <param name="size">Size to give the rectangle</param>
		public RectangleF (SizeF size)
		{
			location = new PointF (0, 0);
			this.size = size;
		}

		/// <summary>
		/// Initializes a new instance of the Rectangle class with the specified <paramref name="x"/>, <paramref name="y"/>, <paramref name="width"/>, and <paramref name="height"/>
		/// </summary>
		/// <param name="x">X co-ordinate for the location of the rectangle</param>
		/// <param name="y">Y co-ordinate for the location of the rectangle</param>
		/// <param name="width">Width of the rectangle</param>
		/// <param name="height">Height of the rectangle</param>
		public RectangleF (float x, float y, float width, float height)
		{
			location = new PointF (x, y);
			size = new SizeF (width, height);
		}

		/// <summary>
		/// Gets a value indicating that the specified <paramref name="point"/> is within the bounds of this rectangle
		/// </summary>
		/// <param name="point">Point to test</param>
		/// <returns>True if the point is within the bounds of this rectangle, false if it is outside the bounds</returns>
		public bool Contains (PointF point)
		{
			return Contains (point.X, point.Y);
		}

		/// <summary>
		/// Gets a value indicating that the specified <paramref name="x"/> and <paramref name="y"/> co-ordinates are within the bounds of this rectangle
		/// </summary>
		/// <param name="x">X co-ordinate to test</param>
		/// <param name="y">Y co-ordinate to test</param>
		/// <returns>True if the rectangle contains the x and y co-ordinates, false otherwise</returns>
		public bool Contains (float x, float y)
		{
			if (Width == 0 || Height == 0)
				return false;
			return (x >= Left && x <= InnerRight && y >= Top && y <= InnerBottom);
		}

		/// <summary>
		/// Gets a value indicating that the specified <paramref name="rectangle"/> is entirely contained within the bounds of this rectangle
		/// </summary>
		/// <param name="rectangle">Rectangle to test if it is contained within this instance</param>
		public bool Contains (RectangleF rectangle)
		{
			return Left <= rectangle.Left && Top <= rectangle.Top && Right >= rectangle.Right && Bottom >= rectangle.Bottom;
		}

		/// <summary>
		/// Gets a value indicating that the specified <paramref name="rectangle"/> overlaps this rectangle
		/// </summary>
		/// <param name="rectangle">Rectangle to test for intersection/overlap</param>
		/// <returns>True if the rectangle overlaps this instance, false otherwise</returns>
		public bool Intersects (RectangleF rectangle)
		{
			return rectangle.X < X + Width && X < rectangle.X + rectangle.Width && rectangle.Y < Y + Height && Y < rectangle.Y + rectangle.Height;
		}

		/// <summary>
		/// Gets a value indicating that both the <see name="Location"/> and <see cref="Size"/> of this rectangle are zero
		/// </summary>
		/// <remarks>
		/// The X, Y, Width, and Height components of this rectangle must be zero for this to return true.
		/// </remarks>
		public bool IsZero
		{
			get { return location.IsZero && size.IsZero; }
		}

		/// <summary>
		/// Gets a value indicating that the <see cref="Size"/> of this rectangle is empty (either the width or height are zero)
		/// </summary>
		public bool IsEmpty
		{
			get { return size.IsEmpty; }
		}

		/// <summary>
		/// Gets the location of this rectangle
		/// </summary>
		/// <remarks>
		/// Same as getting the <see cref="X"/> and <see cref="Y"/> co-ordinates of this rectangle
		/// </remarks>
		public PointF Location
		{
			get { return location; }
			set { location = value; }
		}

		/// <summary>
		/// Gets the ending location of this rectangle
		/// </summary>
		/// <remarks>
		/// This gets/sets the product of the <see cref="Location"/> + <see cref="Size"/>. If the Width or Height of this rectangle
		/// is positive, then the X/Y of the returned location will be minus 1 so as to be inside of the rectangle's bounds.
		/// </remarks>
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

		/// <summary>
		/// Gets or sets the size of the rectangle
		/// </summary>
		public SizeF Size
		{
			get { return size; }
			set { size = value; }
		}

		/// <summary>
		/// Gets or sets the X co-ordinate of the <see cref="Location"/> of this rectangle
		/// </summary>
		public float X
		{
			get { return location.X; }
			set { location.X = value; }
		}

		/// <summary>
		/// Gets or sets the Y co-ordinate of the <see cref="Location"/> of this rectangle
		/// </summary>
		public float Y
		{
			get { return location.Y; }
			set { location.Y = value; }
		}

		/// <summary>
		/// Gets or sets the Width of this rectangle
		/// </summary>
		public float Width
		{
			get { return size.Width; }
			set { size.Width = value; }
		}

		/// <summary>
		/// Gets or sets the Height of this rectangle
		/// </summary>
		public float Height
		{
			get { return size.Height; }
			set { size.Height = value; }
		}

		#region Positional methods

		/// <summary>
		/// Gets or sets the logical top of this rectangle (Y co-ordinate if Height is positive, Y + Height if negative) 
		/// </summary>
		/// <remarks>
		/// This is always the logical top, where if the <see cref="Height"/> is positive it will adjust the Y co-ordinate.
		/// If the Height of the rectangle is negative, then this will adjust the Height when setting the value.
		/// </remarks>
		public float Top
		{
			get { return (Height >= 0) ? Y : Y + Height; }
			set
			{
				if (Height >= 0) {
					Height += Y - value;
					Y = value;
					if (Height < 0)
						Height = 0;
				} else {
					Height = value - Y;
				}
			}
		}

		/// <summary>
		/// Gets or sets the logical left of this rectangle (X co-ordinate if Width is positive, X + Width if negative)
		/// </summary>
		public float Left
		{
			get { return (Width >= 0) ? X : X + Width; }
			set
			{
				if (Width >= 0) {
					Width += X - value;
					X = value;
					if (Width < 0)
						Width = 0;
				} else {
					Width = value - X;
				}
			}
		}

		/// <summary>
		/// Gets or sets the logical right of this rectangle (X + Width if Width is positive, X + 1 if negative)
		/// </summary>
		/// <remarks>
		/// This differs from the <seealso cref="InnerRight"/> in that this will return the co-ordinate adjacent to the right edge
		/// of the rectangle, whereas <seealso cref="InnerRight"/> returns the co-ordinate that is inside the rectangle
		/// </remarks>
		public float Right
		{
			get { return (Width >= 0) ? X + Width : X + 1; }
			set
			{
				if (Width >= 0) {
					Width = value - X;
					if (Width < 0) {
						X += Width;
						Width = 0;
					}
				} else {
					Width += Right - value;
					X = value - 1;
				}
			}
		}

		/// <summary>
		/// Gets or sets the logical bottom of this rectangle (Y + Height if Height is positive, Y + 1 if negative)
		/// </summary>
		/// <remarks>
		/// This differs from the <seealso cref="InnerBottom"/> in that this will return the co-ordinate adjacent to the bottom edge
		/// of the rectangle, whereas <seealso cref="InnerBottom"/> returns the co-ordinate that is inside the rectangle
		/// </remarks>
		public float Bottom
		{
			get { return (Height >= 0) ? Y + Height : Y + 1; }
			set
			{
				if (Height >= 0) {
					Height = value - Y;
					if (Height < 0) {
						Y += Height;
						Height = 0;
					}
				} else {
					Height += Bottom - value;
					Y = value - 1;
				}
			}
		}

		/// <summary>
		/// Gets or sets the point at the <see cref="Top"/> and <see cref="Left"/> of the rectangle
		/// </summary>
		public PointF TopLeft
		{
			get { return new PointF (Left, Top); }
			set { Top = value.Y; Left = value.X; }
		}

		/// <summary>
		/// Gets or sets the point at the <see cref="Top"/> and <see cref="Right"/> of the rectangle
		/// </summary>
		public PointF TopRight
		{
			get { return new PointF (Right, Top); }
			set { Top = value.Y; Right = value.X; }
		}

		/// <summary>
		/// Gets or sets the point at the <see cref="Bottom"/> and <see cref="Right"/> of the rectangle
		/// </summary>
		public PointF BottomRight
		{
			get { return new PointF (Right, Bottom); }
			set { Bottom = value.Y; Right = value.X; }
		}

		/// <summary>
		/// Gets or sets the point at the <see cref="Bottom"/> and <see cref="Left"/> of the rectangle
		/// </summary>
		public PointF BottomLeft
		{
			get { return new PointF (Left, Bottom); }
			set { Bottom = value.Y; Left = value.X; }
		}

		/// <summary>
		/// Gets or sets the point at the <see cref="Left"/> and <see cref="MiddleY"/> of the rectangle
		/// </summary>
		public PointF MiddleLeft
		{
			get { return new PointF (Left, MiddleY); }
			set { Left = value.X; MiddleY = value.Y; }
		}
		
		/// <summary>
		/// Gets or sets the point at the <see cref="Right"/> and <see cref="MiddleY"/> of the rectangle
		/// </summary>
		public PointF MiddleRight
		{
			get { return new PointF (Right, MiddleY); }
			set { Right = value.X; MiddleY = value.Y; }
		}

		/// <summary>
		/// Gets or sets the point at the <see cref="MiddleX"/> and <see cref="Top"/> of the rectangle
		/// </summary>
		public PointF MiddleTop
		{
			get { return new PointF (MiddleX, Top); }
			set { MiddleX = value.X; Top = value.Y; }
		}

		/// <summary>
		/// Gets or sets the point at the <see cref="MiddleX"/> and <see cref="Bottom"/> of the rectangle
		/// </summary>
		public PointF MiddleBottom
		{
			get { return new PointF (MiddleX, Bottom); }
			set { MiddleX = value.X; Bottom = value.Y; }
		}

		#endregion

		#region Inner Positional Methods

		/// <summary>
		/// Gets or sets the point at the <see cref="Top"/> and <see cref="InnerRight"/> of the rectangle
		/// </summary>
		/// <remarks>
		/// Similar to <seealso cref="TopRight"/> but inside the rectangle's bounds instead of just to the right
		/// </remarks>
		public PointF InnerTopRight
		{
			get { return new PointF (InnerRight, Top); }
			set { Top = value.Y; InnerRight = value.X; }
		}

		/// <summary>
		/// Gets or sets the point at the <see cref="InnerBottom"/> and <see cref="InnerRight"/> of the rectangle
		/// </summary>
		/// <remarks>
		/// Similar to <seealso cref="BottomRight"/> but inside the rectangle's bounds instead of just to the right and bottom
		/// </remarks>
		public PointF InnerBottomRight
		{
			get { return new PointF (InnerRight, InnerBottom); }
			set { InnerBottom = value.Y; InnerRight = value.X; }
		}

		/// <summary>
		/// Gets or sets the point at the <see cref="InnerBottom"/> and <see cref="Left"/> of the rectangle
		/// </summary>
		/// <remarks>
		/// Similar to <seealso cref="BottomLeft"/> but inside the rectangle's bounds instead of just below the bottom
		/// </remarks>
		public PointF InnerBottomLeft
		{
			get { return new PointF (Left, InnerBottom); }
			set { InnerBottom = value.Y; Left = value.X; }
		}

		/// <summary>
		/// Gets or sets the bottom of the rectangle that is inside the bounds
		/// </summary>
		/// <remarks>
		/// Similar to <seealso cref="Bottom"/> but inside the rectangle's bounds instead of just below the bottom
		/// </remarks>
		public float InnerBottom
		{
			get { return (Height > 0) ? Y + Height - InnerOffset : Y; }
			set
			{
				if (Height >= 0) {
					Height = value - Y + InnerOffset;
					if (Height < 0) {
						Y += Height - InnerOffset;
						Height = 0;
					}
				} else {
					Height += Y - value;
					Y = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the right of the rectangle that is inside the bounds
		/// </summary>
		/// <remarks>
		/// Similar to <seealso cref="Right"/> but inside the rectangle's bounds instead of just to the right
		/// </remarks>
		public float InnerRight
		{
			get { return (Width > 0) ? X + Width - InnerOffset : X; }
			set
			{
				if (Width >= 0) {
					Width = value - X + InnerOffset;
					if (Width < 0) {
						X += Width - InnerOffset;
						Width = 0;
					}
				} else {
					Width += X - value;
					X = value;
				}
			}
		}

		#endregion

		/// <summary>
		/// Gets or sets the rectangle's center position
		/// </summary>
		/// <remarks>
		/// This gets/sets the <see cref="MiddleX"/> and <see cref="MiddleY"/> as a point
		/// </remarks>
		public PointF Center
		{
			get { return new PointF (MiddleX, MiddleY); }
			set { MiddleX = value.X; MiddleY = value.Y; }
		}

		/// <summary>
		/// Gets or sets the rectangle's middle horizontal position
		/// </summary>
		public float MiddleX
		{
			get { return X + (Width / 2); }
			set { X = value - (Width / 2); }
		}

		/// <summary>
		/// Gets or sets the rectangle's middle vertical position
		/// </summary>
		public float MiddleY
		{
			get { return Y + (Height / 2); }
			set { Y = value - (Height / 2); }
		}

		/// <summary>
		/// Offsets the location of the rectangle by the specified <paramref name="x"/> and <paramref name="y"/> values
		/// </summary>
		/// <param name="x">Horizontal offset to move the rectangle</param>
		/// <param name="y">Vertical offset to move the rectangle</param>
		public void Offset (float x, float y)
		{
			location.X += x;
			location.Y += y;
		}
		
		/// <summary>
		/// Offsets the location of the rectangle by the specified <paramref name="size"/>
		/// </summary>
		/// <param name="size">Width and Height to move the rectangle</param>
		public void Offset (SizeF size)
		{
			location.X += size.Width;
			location.Y += size.Height;
		}
		
		/// <summary>
		/// Offsets the location of the rectangle by the X and Y values of the specified <paramref name="point"/>
		/// </summary>
		/// <param name="point">Point with values to offset the rectangle</param>
		public void Offset (PointF point)
		{
			location.X += point.X;
			location.Y += point.Y;
		}

		/// <summary>
		/// Offsets the location of the <paramref name="rectangle"/> by the specified <paramref name="x"/> and <paramref name="y"/> values
		/// </summary>
		/// <param name="rectangle">Rectangle to offset</param>
		/// <param name="x">Horizontal offset to move the rectangle</param>
		/// <param name="y">Vertical offset to move the rectangle</param>
		/// <returns>A new Rectangle with the offset location</returns>
		public static RectangleF Offset (RectangleF rectangle, float x, float y)
		{
			rectangle.Offset (x, y);
			return rectangle;
		}

		/// <summary>
		/// Offsets the location of the <paramref name="rectangle"/> by the specified <paramref name="size"/>
		/// </summary>
		/// <param name="rectangle">Rectangle to inflate</param>
		/// <param name="size">Width and Height to move the rectangle</param>
		/// <returns>A new Rectangle with the offset location</returns>
		public static RectangleF Offset (RectangleF rectangle, SizeF size)
		{
			rectangle.Offset (size);
			return rectangle;
		}

		/// <summary>
		/// Offsets the location of the <paramref name="rectangle"/> by the X and Y values of the specified <paramref name="point"/>
		/// </summary>
		/// <param name="rectangle">Rectangle to offset</param>
		/// <param name="point">Point with values to offset the rectangle</param>
		/// <returns>A new Rectangle with the offset location</returns>
		public static RectangleF Offset (RectangleF rectangle, PointF point)
		{
			rectangle.Offset (point);
			return rectangle;
		}

		/// <summary>
		/// Inflates all dimensions of this rectangle by the specified <paramref name="size"/>
		/// </summary>
		/// <remarks>
		/// This inflates the rectangle in all dimensions by the width and height specified by <paramref name="size"/>.
		/// The resulting rectangle will be increased in width and height twice that of the specified size, and the center
		/// will be in the same location.
		/// A negative width and/or height can be passed in to deflate the rectangle.
		/// </remarks>
		/// <param name="size">Size to inflate the rectangle by</param>
		public void Inflate (SizeF size)
		{
			Inflate (size.Width, size.Height);
		}

		/// <summary>
		/// Inflates all dimensions of this rectangle by the specified <paramref name="width"/> and <paramref name="height"/>
		/// </summary>
		/// <remarks>
		/// This inflates the rectangle in all dimensions by the specified <paramref name="width"/> and <paramref name="height"/>.
		/// The resulting rectangle will be increased in width and height twice that of the specified size, and the center
		/// will be in the same location.
		/// A negative width and/or height can be passed in to deflate the rectangle.
		/// </remarks>
		/// <param name="width">Width to inflate the left and right of the rectangle by</param>
		/// <param name="height">Height to inflate the top and bottom of the rectangle by</param>
		public void Inflate (float width, float height)
		{
			if (Width >= 0) {
				X -= width;
				Width += width * 2;
			} else {
				X += width;
				Width -= width * 2;
			}

			if (Height >= 0) {
				Y -= height;
				Height += height * 2;
			} else {
				Y += height;
				Height -= height * 2;
			}
		}

		/// <summary>
		/// Inflates all dimensions of the <paramref name="rectangle"/> by the specified <paramref name="size"/>
		/// </summary>
		/// <remarks>
		/// This inflates the <paramref name="rectangle"/> in all dimensions by the width and height specified by <paramref name="size"/>.
		/// The resulting rectangle will be increased in width and height twice that of the specified size, and the center
		/// will be in the same location.
		/// A negative width and/or height can be passed in to deflate the rectangle.
		/// </remarks>
		/// <param name="rectangle">Rectangle to inflate</param>
		/// <param name="size">Size to inflate the rectangle by</param>
		/// <returns>A new rectangle inflated by the specified width and height</returns>
		public static RectangleF Inflate (RectangleF rectangle, SizeF size)
		{
			rectangle.Inflate (size);
			return rectangle;
		}

		/// <summary>
		/// Inflates all dimensions of this <paramref name="rectangle"/> by the specified <paramref name="width"/> and <paramref name="height"/>
		/// </summary>
		/// <remarks>
		/// This inflates the <paramref name="rectangle"/> in all dimensions by the specified <paramref name="width"/> and <paramref name="height"/>.
		/// The resulting rectangle will be increased in width and height twice that of the specified size, and the center
		/// will be in the same location.
		/// A negative width and/or height can be passed in to deflate the rectangle.
		/// </remarks>
		/// <param name="rectangle">Rectangle to inflate</param>
		/// <param name="width">Width to inflate the left and right of the rectangle by</param>
		/// <param name="height">Height to inflate the top and bottom of the rectangle by</param>
		/// <returns>A new rectangle inflated by the specified width and height</returns>
		public static RectangleF Inflate (RectangleF rectangle, float width, float height)
		{
			rectangle.Inflate (width, height);
			return rectangle;
		}

		/// <summary>
		/// Calculates the distance between the specified <paramref name="point"/> and <paramref name="rect"/>.
		/// </summary>
		/// <remarks>
		/// This calculates the horizontal and vertical distance of any of the edges of the rectangle to the specified point.
		/// </remarks>
		/// <param name="rect">Rectangle for the source.</param>
		/// <param name="point">Point to determine the distance from the rectangle.</param>
		public static SizeF Distance(RectangleF rect, PointF point)
		{
			var cx = Math.Max(Math.Min(point.X, rect.X + rect.Width), rect.X);
			var cy = Math.Max(Math.Min(point.Y, rect.Y + rect.Height), rect.Y);
			return new SizeF(point.X - cx, point.Y - cy); 
		}

		/// <summary>
		/// Calculates the distance between two rectangles.
		/// </summary>
		/// <remarks>
		/// This calculates the horizontal and vertical distance of any of the edges of each rectangle.
		/// If the rectangles intersect, the distance will be empty.
		/// </remarks>
		/// <param name="rect1">First rectangle to compare</param>
		/// <param name="rect2">Second rectangle to compare</param>
		public static SizeF Distance(RectangleF rect1, RectangleF rect2)
		{
			if (rect1.Intersects(rect2))
				return SizeF.Empty;

			var left = rect1.X < rect2.X ? rect1 : rect2;
			var right = rect2.X < rect1.X ? rect1 : rect2;
			var xdiff = Math.Max(0, right.X - (left.X + left.Width));

			var top = rect1.Y < rect2.Y ? rect1 : rect2;
			var bottom = rect2.Y < rect1.Y ? rect1 : rect2;
			var ydiff = Math.Max(0, bottom.Y - (top.Y + top.Height));

			return new SizeF(xdiff, ydiff);
		}

		/// <summary>
		/// Aligns the rectangle to a grid of the specified <paramref name="gridSize"/>
		/// </summary>
		/// <remarks>
		/// This will align the top, left, right, and bottom to a grid by inflating each edge to the next grid line.
		/// </remarks>
		/// <param name="gridSize">Size of the grid to align the rectangle to</param>
		public void Align (SizeF gridSize)
		{
			Align (gridSize.Width, gridSize.Height);
		}

		/// <summary>
		/// Aligns the rectangle to a grid of the specified <paramref name="gridWidth"/> and <paramref name="gridHeight"/>
		/// </summary>
		/// <remarks>
		/// This will align the top, left, right, and bottom to a grid by inflating each edge to the next grid line.
		/// </remarks>
		/// <param name="gridWidth">Grid width</param>
		/// <param name="gridHeight">Grid height</param>
		public void Align (float gridWidth, float gridHeight)
		{
			Top = Top - (Top % gridHeight);
			Left = Left - (Left % gridWidth);
			Right = Right + gridWidth - (Right % gridWidth);
			Bottom = Bottom + gridHeight - (Bottom % gridHeight);
		}

		/// <summary>
		/// Aligns the <paramref name="rectangle"/> to a grid of the specified <paramref name="gridSize"/>
		/// </summary>
		/// <remarks>
		/// This will align the top, left, right, and bottom to a grid by inflating each edge to the next grid line.
		/// </remarks>
		/// <param name="rectangle">Rectangle to align</param>
		/// <param name="gridSize">Size of the grid to align the rectangle to</param>
		/// <returns>A new Rectangle aligned to the grid</returns>
		public static RectangleF Align (RectangleF rectangle, SizeF gridSize)
		{
			rectangle.Align (gridSize);
			return rectangle;
		}

		/// <summary>
		/// Aligns the <paramref name="rectangle"/> to a grid of the specified <paramref name="gridWidth"/> and <paramref name="gridHeight"/>
		/// </summary>
		/// <remarks>
		/// This will align the top, left, right, and bottom to a grid by inflating each edge to the next grid line.
		/// </remarks>
		/// <param name="rectangle">Rectangle to align</param>
		/// <param name="gridWidth">Grid width</param>
		/// <param name="gridHeight">Grid height</param>
		/// <returns>A new Rectangle aligned to the grid</returns>
		public static RectangleF Align (RectangleF rectangle, float gridWidth, float gridHeight)
		{
			rectangle.Align (gridWidth, gridHeight);
			return rectangle;
		}

		/// <summary>
		/// Union the <paramref name="rectangle"/> into this instance to encompass both rectangles
		/// </summary>
		/// <param name="rectangle">Rectangle to union with this instance</param>
		public void Union (RectangleF rectangle)
		{
			var left = Math.Min(Left, rectangle.Left);
			var top = Math.Min(Top, rectangle.Top);
			var right = Math.Max(Right, rectangle.Right);
			var bottom = Math.Max(Bottom, rectangle.Bottom);
			location = new PointF (left, top);
			size = new SizeF (right - left, bottom - top);
		}
		
		/// <summary>
		/// Combines two rectangles into one rectangle that encompasses both
		/// </summary>
		/// <param name="rect1">First rectangle to union</param>
		/// <param name="rect2">Second rectangle to union</param>
		/// <returns>A new RectangleF that encompasses both <paramref name="rect1"/> and <paramref name="rect2"/></returns>
		public static RectangleF Union (RectangleF rect1, RectangleF rect2)
		{
			rect1.Union (rect2);
			return rect1;
		}

		/// <summary>
		/// Intersect the rectangle with the specified <paramref name="rectangle"/>
		/// </summary>
		/// <param name="rectangle">Rectangle to intersect with</param>
		public void Intersect (RectangleF rectangle)
		{
			var left = Math.Max(Left, rectangle.Left);
			var top = Math.Max(Top, rectangle.Top);
			var width = Math.Max (Math.Min(Right, rectangle.Right) - left, 0);
			var height = Math.Max (Math.Min(Bottom, rectangle.Bottom) - top, 0);
			location = new PointF (left, top);
			size = new SizeF (width, height);
		}

		/// <summary>
		/// Intersect the two specified rectangles
		/// </summary>
		/// <param name="rect1">First rectangle to intersect</param>
		/// <param name="rect2">Second rectangle to intersect</param>
		/// <returns>A new RectangleF with the intersection of the two rectangles</returns>
		public static RectangleF Intersect (RectangleF rect1, RectangleF rect2)
		{
			rect1.Intersect (rect2);
			return rect1;
		}

		/// <summary>
		/// Restricts the rectangle to be within the specified <paramref name="point"/> and <paramref name="size"/>
		/// </summary>
		/// <remarks>
		/// This is a shortcut for <seealso cref="Restrict(RectangleF)"/>
		/// </remarks>
		/// <param name="point">Minimum location for the rectangle</param>
		/// <param name="size">Maximum size for the rectangle</param>
		public void Restrict (PointF point, SizeF size)
		{
			Restrict (new RectangleF (point, size));
		}
		
		/// <summary>
		/// Restricts the rectangle to be within the specified <paramref name="size"/> at an X,Y location of 0, 0
		/// </summary>
		/// <remarks>
		/// This is a shortcut for <seealso cref="Restrict(RectangleF)"/>
		/// </remarks>
		/// <param name="size">Maxiumum size for the rectangle</param>
		public void Restrict (SizeF size)
		{
			Restrict (new RectangleF (size));
		}
		
		/// <summary>
		/// Restricts the rectangle to be within the specified <paramref name="rectangle"/>
		/// </summary>
		/// <remarks>
		/// This ensures that the current rectangle's bounds fall within the bounds of the specified <paramref name="rectangle"/>.
		/// It is useful to ensure that the rectangle does not exceed certain limits (e.g. for drawing)
		/// </remarks>
		/// <param name="rectangle">Rectangle to restrict this instance to</param>
		public void Restrict (RectangleF rectangle)
		{
			if (Left < rectangle.Left)
				Left = rectangle.Left;
			if (Top < rectangle.Top)
				Top = rectangle.Top;
			if (Right > rectangle.Right)
				Right = rectangle.Right;
			if (Bottom > rectangle.Bottom)
				Bottom = rectangle.Bottom;
		}

		/// <summary>
		/// Restricts the <paramref name="rectangle"/> to be within the <paramref name="restrict"/> rectangle
		/// </summary>
		/// <remarks>
		/// This ensures that <paramref name="rectangle"/>'s bounds fall within the bounds of the specified <paramref name="restrict"/> rectangle
		/// It is useful to ensure that the rectangle does not exceed certain limits (e.g. for drawing)
		/// </remarks>
		/// <param name="rectangle">Rectangle to restrict</param>
		/// <param name="restrict">Rectangle to restrict to</param>
		/// <returns>A new rectangle restricted to the restrict bounds</returns>
		public static Rectangle Restrict (Rectangle rectangle, Rectangle restrict)
		{
			rectangle.Restrict (restrict);
			return rectangle;
		}

		/// <summary>
		/// Multiplies all X, Y, Width, Height components of the <paramref name="rectangle"/> by a <paramref name="factor"/>
		/// </summary>
		/// <param name="rectangle">Rectangle to multiply</param>
		/// <param name="factor">Factor to mulitply by</param>
		/// <returns>A new instance of a Rectangle with the product of the specified <paramref name="rectangle"/> and the <paramref name="factor"/></returns>
		public static RectangleF operator * (RectangleF rectangle, float factor)
		{
			return new RectangleF (rectangle.X * factor, rectangle.Y * factor, rectangle.Width * factor, rectangle.Height * factor);
		}

		/// <summary>
		/// Divides all X, Y, Width, Height components of the <paramref name="rectangle"/> by a <paramref name="factor"/> factor
		/// </summary>
		/// <param name="rectangle">Rectangle to divide</param>
		/// <param name="factor">Factor to divide by</param>
		/// <returns>A new instance of a Rectangle with the value of <paramref name="rectangle"/> divided by a <paramref name="factor"/></returns>
		public static RectangleF operator / (RectangleF rectangle, float factor)
		{
			return new RectangleF (rectangle.X / factor, rectangle.Y / factor, rectangle.Width / factor, rectangle.Height / factor);
		}

		/// <summary>
		/// Multiplies the specified <paramref name="rectangle"/> by the Width and Height of <paramref name="size"/>
		/// </summary>
		/// <remarks>
		/// The X and Width components will be multiplied by the Width of the specified <paramref name="size"/>, and
		/// the Y and Height components will be multiplied by the Height.
		/// </remarks>
		/// <param name="rectangle">Rectangle to multiply</param>
		/// <param name="size">Width and Height to multiply the rectangle by</param>
		/// <returns>A new instance of a Rectangle with the product of the <paramref name="rectangle"/> and <paramref name="size"/></returns>
		public static RectangleF operator * (RectangleF rectangle, SizeF size)
		{
			return new RectangleF (rectangle.X * size.Width, rectangle.Y * size.Height, rectangle.Width * size.Width, rectangle.Height * size.Height);
		}

		/// <summary>
		/// Divides the specified <paramref name="rectangle"/> by the Width and Height of <paramref name="size"/>
		/// </summary>
		/// <remarks>
		/// The X and Width components will be divided by the Width of the specified <paramref name="size"/>, and
		/// the Y and Height components will be divided by the Height.
		/// </remarks>
		/// <param name="rectangle">Rectangle to divide</param>
		/// <param name="size">Width and Height to divide the rectangle by</param>
		/// <returns>A new instance of a Rectangle with the value of <paramref name="rectangle"/> divided by <paramref name="size"/></returns>
		public static RectangleF operator / (RectangleF rectangle, SizeF size)
		{
			return new RectangleF (rectangle.X / size.Width, rectangle.Y / size.Height, rectangle.Width / size.Width, rectangle.Height / size.Height);
		}

		/// <summary>
		/// Adds the <paramref name="offset"/> to the specified <paramref name="rectangle"/>, moving its location
		/// </summary>
		/// <param name="rectangle">Rectangle to offset</param>
		/// <param name="offset">Offset to move the location by</param>
		public static RectangleF operator + (RectangleF rectangle, PointF offset)
		{
			rectangle.Offset (offset);
			return rectangle;
		}

		/// <summary>
		/// Subtracts the <paramref name="offset"/> from the specified <paramref name="rectangle"/>, moving its location
		/// </summary>
		/// <param name="rectangle">Rectangle to offset</param>
		/// <param name="offset">Offset to move the location by</param>
		public static RectangleF operator - (RectangleF rectangle, PointF offset)
		{
			rectangle.Offset (-offset);
			return rectangle;
		}

		/// <summary>
		/// Adds the <paramref name="offset"/> to the specified <paramref name="rectangle"/>, moving its location
		/// </summary>
		/// <param name="rectangle">Rectangle to offset</param>
		/// <param name="offset">Offset to move the location by</param>
		public static RectangleF operator + (RectangleF rectangle, SizeF offset)
		{
			rectangle.Offset (offset);
			return rectangle;
		}
		
		/// <summary>
		/// Subtracts the <paramref name="offset"/> from the specified <paramref name="rectangle"/>, moving its location
		/// </summary>
		/// <param name="rectangle">Rectangle to offset</param>
		/// <param name="offset">Offset to move the location by</param>
		public static RectangleF operator - (RectangleF rectangle, SizeF offset)
		{
			rectangle.Offset (-offset);
			return rectangle;
		}
		
		/// <summary>
		/// Compares two rectangles for equality
		/// </summary>
		/// <param name="rect1">First rectangle to compare</param>
		/// <param name="rect2">Second rectangle to compare</param>
		/// <returns>True if the two rectangles are equal, false otherwise</returns>
		public static bool operator == (RectangleF rect1, RectangleF rect2)
		{
			return rect1.location == rect2.location && rect1.size == rect2.size;
		}

		/// <summary>
		/// Compares two rectangles for inequality
		/// </summary>
		/// <param name="rect1">First rectangle to compare</param>
		/// <param name="rect2">Second rectangle to compare</param>
		/// <returns>True if the two rectangles are not equal, false otherwise</returns>
		public static bool operator != (RectangleF rect1, RectangleF rect2)
		{
			return !(rect1 == rect2);
		}

		/// <summary>
		/// Implicit conversion from a <see cref="Rectangle"/> to a <see cref="RectangleF"/>
		/// </summary>
		/// <remarks>
		/// Since no precision is lost, this can be implicit.
		/// </remarks>
		/// <param name="rectangle">Point to convert</param>
		/// <returns>A new instance of a RectangleF with the value of the specified <paramref name="rectangle"/></returns>
		public static implicit operator RectangleF (Rectangle rectangle)
		{
			return new RectangleF (rectangle.Location, rectangle.Size);
		}

		/// <summary>
		/// Converts this rectangle to a string
		/// </summary>
		/// <returns>String representation of this rectangle</returns>
		public override string ToString ()
		{
			return String.Format (CultureInfo.InvariantCulture, "{0},{1}", location, size);
		}

		/// <summary>
		/// Compares this rectangle to an object for equality
		/// </summary>
		/// <param name="obj">Object to compare with</param>
		/// <returns>True if the <paramref name="obj"/> is a Rectangle and is equal to this instance, false otherwise</returns>
		public override bool Equals (Object obj)
		{
			return obj is RectangleF && (RectangleF)obj == this;
		}

		/// <summary>
		/// Gets the hash code for this rectangle
		/// </summary>
		/// <returns>Hash code value for this rectangle</returns>
		public override int GetHashCode ()
		{
			return location.GetHashCode () ^ size.GetHashCode ();
		}

		/// <summary>
		/// Compares this rectangle with the specified <paramref name="other"/> rectangle
		/// </summary>
		/// <param name="other">Other rectangle to compare with</param>
		/// <returns>True if the <paramref name="other"/> rectangle is equal to this instance, false otherwise</returns>
		public bool Equals (RectangleF other)
		{
			return other == this;
		}
	}
}
