using System;
using sc = System.ComponentModel;
using System.Globalization;

namespace Eto.Drawing
{
	/// <summary>
	/// Represents a rectangle with a location (X, Y) and size (Width, Height) components.
	/// </summary>
	/// <remarks>
	/// A rectangle is defined by a location (X, Y) and a size (Width, Height).
	/// The width and/or height can be negative.
	/// </remarks>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[sc.TypeConverter (typeof(RectangleConverterInternal))]
	public struct Rectangle : IEquatable<Rectangle>
	{
		Point location;
		Size size;
		const int InnerOffset = 1;

		/// <summary>
		/// Converts a floating point <paramref name="rectangle"/> to an integral <see cref="Rectangle"/> by rounding the X, Y, Width, and Height.
		/// </summary>
		/// <param name="rectangle">Rectangle to round</param>
		/// <returns>A new instance of a Rectangle with rounded X, Y, Width, and Height values</returns>
		public static Rectangle Round (RectangleF rectangle)
		{
			return new Rectangle ((int)Math.Round (rectangle.X), (int)Math.Round (rectangle.Y), (int)Math.Round (rectangle.Width), (int)Math.Round (rectangle.Height));
		}

		/// <summary>
		/// Converts a floating point <paramref name="rectangle"/> to an integral <see cref="Rectangle"/> by getting the smallest integral value of X, Y, Width, and Height
		/// </summary>
		/// <remarks>
		/// This is used to get an integral rectangle that bounds the floating point rectangle completely.
		/// E.g. if a RectangleF has (X,Y,W,H) values of 0.2, 0.9, 1.1, 1.9, it would return a new integral rectangle
		/// with values 0, 0, 2, 2.
		/// </remarks>
		/// <param name="rectangle">Rectangle to get the ceiling</param>
		/// <returns>A new instance of a Rectangle with truncated X, Y and a ceiling Width and Height values</returns>
		public static Rectangle Ceiling (RectangleF rectangle)
		{
			return new Rectangle ((int)Math.Truncate (rectangle.X), (int)Math.Truncate (rectangle.Y), (int)Math.Ceiling (rectangle.Width), (int)Math.Ceiling (rectangle.Height));
		}

		/// <summary>
		/// Converts a floating point <paramref name="rectangle"/> to an integral <see cref="Rectangle"/> by truncating the X, Y, Width, and Height values
		/// </summary>
		/// <param name="rectangle">Rectangle to truncate</param>
		/// <returns>A new instance of a Rectangle with truncated X, Y, Width, and Height values</returns>
		public static Rectangle Truncate (RectangleF rectangle)
		{
			return new Rectangle ((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height);
		}

		/* COMMON */

		/// <summary>
		/// Gets an empty rectangle with zero X, Y, Width, and Height components
		/// </summary>
		/// <remarks>
		/// Useful when you want a rectangle no size or location.
		/// </remarks>
		public static readonly Rectangle Empty = new Rectangle (0, 0, 0, 0);

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

		/// <summary>
		/// Creates a new instance of a RectangleF from the values of the <paramref name="left"/>, <paramref name="top"/>, <paramref name="right"/> and <paramref name="bottom"/> sides
		/// </summary>
		/// <param name="left">Left side of the rectangle to create</param>
		/// <param name="top">Top of the rectangle to create</param>
		/// <param name="right">Right side of the rectangle to create</param>
		/// <param name="bottom">Bottom of the rectangle to create</param>
		/// <returns>A new instance of a RectangleF with values for the Left, Top, Right, and Bottom sides</returns>
		public static Rectangle FromSides (int left, int top, int right, int bottom)
		{
			return new Rectangle (left, top, right - left, bottom - top);
		}

		/// <summary>
		/// Creates a new instance of a Rectangle with a specified <paramref name="center"/> and <paramref name="size"/>
		/// </summary>
		/// <param name="center">Center of the rectangle</param>
		/// <param name="size">Size of the rectangle</param>
		/// <returns>A new instance of a Rectangle with the specified center and size</returns>
		public static Rectangle FromCenter (Point center, Size size)
		{
			return new Rectangle (center - size / 2, size);
		}

		/// <summary>
		/// Initializes a new instance of the Rectangle class with two points
		/// </summary>
		/// <param name="start">Starting point of the rectangle</param>
		/// <param name="end">Ending point of the rectangle</param>
		public Rectangle (Point start, Point end)
		{
			location = start;
			size = new Size((end.X >= start.X) ? end.X - start.X + 1 : end.X - start.X, (end.Y >= start.Y) ? end.Y - start.Y + 1: end.Y - start.Y);
		}

		/// <summary>
		/// Initializes a new instance of the Rectangle class with the specified <paramref name="location"/> and <paramref name="size"/>
		/// </summary>
		/// <param name="location">Location of the rectangle</param>
		/// <param name="size">Size of the rectangle</param>
		public Rectangle (Point location, Size size)
		{
			this.location = location;
			this.size = size;
		}

		/// <summary>
		/// Initializes a new instance of the Rectangle class with X, Y co-ordinates at 0,0 and the specified <paramref name="size"/>
		/// </summary>
		/// <param name="size">Size to give the rectangle</param>
		public Rectangle (Size size)
		{
			location = new Point (0, 0);
			this.size = size;
		}

		/// <summary>
		/// Initializes a new instance of the Rectangle class with the specified <paramref name="x"/>, <paramref name="y"/>, <paramref name="width"/>, and <paramref name="height"/>
		/// </summary>
		/// <param name="x">X co-ordinate for the location of the rectangle</param>
		/// <param name="y">Y co-ordinate for the location of the rectangle</param>
		/// <param name="width">Width of the rectangle</param>
		/// <param name="height">Height of the rectangle</param>
		public Rectangle (int x, int y, int width, int height)
		{
			this.location = new Point (x, y);
			this.size = new Size (width, height);
		}

		/// <summary>
		/// Initializes a new instance of a Rectangle class with truncated values of the specified floating-point <paramref name="rectangle"/>
		/// </summary>
		/// <param name="rectangle">RectangleF to initialize the X and Y values of the new instance with</param>
		public Rectangle(RectangleF rectangle)
		{
			this.location = new Point((int)rectangle.X, (int)rectangle.Y);
			this.size = new Size((int)rectangle.Width, (int)rectangle.Height);
		}

		/// <summary>
		/// Gets a value indicating that the specified <paramref name="point"/> is within the bounds of this rectangle
		/// </summary>
		/// <param name="point">Point to test</param>
		/// <returns>True if the point is within the bounds of this rectangle, false if it is outside the bounds</returns>
		public bool Contains (Point point)
		{
			return Contains (point.X, point.Y);
		}

		/// <summary>
		/// Gets a value indicating that the specified <paramref name="x"/> and <paramref name="y"/> co-ordinates are within the bounds of this rectangle
		/// </summary>
		/// <param name="x">X co-ordinate to test</param>
		/// <param name="y">Y co-ordinate to test</param>
		/// <returns>True if the rectangle contains the x and y co-ordinates, false otherwise</returns>
		public bool Contains (int x, int y)
		{
			if (Width == 0 || Height == 0)
				return false;
			return (x >= Left && x <= InnerRight && y >= Top && y <= InnerBottom);
		}

		/// <summary>
		/// Gets a value indicating that the specified <paramref name="rectangle"/> is entirely contained within the bounds of this rectangle
		/// </summary>
		/// <param name="rectangle">Rectangle to test if it is contained within this instance</param>
		public bool Contains (Rectangle rectangle)
		{
			return Left <= rectangle.Left && Top <= rectangle.Top && Right >= rectangle.Right && Bottom >= rectangle.Bottom;
		}

		/// <summary>
		/// Gets a value indicating that the specified <paramref name="rectangle"/> overlaps this rectangle
		/// </summary>
		/// <param name="rectangle">Rectangle to test for intersection/overlap</param>
		/// <returns>True if the rectangle overlaps this instance, false otherwise</returns>
		public bool Intersects (Rectangle rectangle)
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
		public Point Location
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

		/// <summary>
		/// Gets or sets the size of the rectangle
		/// </summary>
		public Size Size
		{
			get { return size; }
			set { size = value; }
		}

		/// <summary>
		/// Gets or sets the X co-ordinate of the <see cref="Location"/> of this rectangle
		/// </summary>
		public int X
		{
			get { return location.X; }
			set { location.X = value; }
		}

		/// <summary>
		/// Gets or sets the Y co-ordinate of the <see cref="Location"/> of this rectangle
		/// </summary>
		public int Y
		{
			get { return location.Y; }
			set { location.Y = value; }
		}

		/// <summary>
		/// Gets or sets the Width of this rectangle
		/// </summary>
		public int Width
		{
			get { return size.Width; }
			set { size.Width = value; }
		}

		/// <summary>
		/// Gets or sets the Height of this rectangle
		/// </summary>
		public int Height
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
		public int Top
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
		public int Left
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
		public int Right
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
		public int Bottom
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
		public Point TopLeft
		{
			get { return new Point (Left, Top); }
			set { Top = value.Y; Left = value.X; }
		}

		/// <summary>
		/// Gets or sets the point at the <see cref="Top"/> and <see cref="Right"/> of the rectangle
		/// </summary>
		public Point TopRight
		{
			get { return new Point (Right, Top); }
			set { Top = value.Y; Right = value.X; }
		}

		/// <summary>
		/// Gets or sets the point at the <see cref="Bottom"/> and <see cref="Right"/> of the rectangle
		/// </summary>
		public Point BottomRight
		{
			get { return new Point (Right, Bottom); }
			set { Bottom = value.Y; Right = value.X; }
		}

		/// <summary>
		/// Gets or sets the point at the <see cref="Bottom"/> and <see cref="Left"/> of the rectangle
		/// </summary>
		public Point BottomLeft
		{
			get { return new Point (Left, Bottom); }
			set { Bottom = value.Y; Left = value.X; }
		}

		/// <summary>
		/// Gets or sets the point at the <see cref="Left"/> and <see cref="MiddleY"/> of the rectangle
		/// </summary>
		public Point MiddleLeft
		{
			get { return new Point (Left, MiddleY); }
			set { Left = value.X; MiddleY = value.Y; }
		}
		
		/// <summary>
		/// Gets or sets the point at the <see cref="Right"/> and <see cref="MiddleY"/> of the rectangle
		/// </summary>
		public Point MiddleRight
		{
			get { return new Point (Right, MiddleY); }
			set { Right = value.X; MiddleY = value.Y; }
		}
		
		/// <summary>
		/// Gets or sets the point at the <see cref="MiddleX"/> and <see cref="Top"/> of the rectangle
		/// </summary>
		public Point MiddleTop
		{
			get { return new Point (MiddleX, Top); }
			set { MiddleX = value.X; Top = value.Y; }
		}
		
		/// <summary>
		/// Gets or sets the point at the <see cref="MiddleX"/> and <see cref="Bottom"/> of the rectangle
		/// </summary>
		public Point MiddleBottom
		{
			get { return new Point (MiddleX, Bottom); }
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
		public Point InnerTopRight
		{
			get { return new Point (InnerRight, Top); }
			set { Top = value.Y; InnerRight = value.X; }
		}

		/// <summary>
		/// Gets or sets the point at the <see cref="InnerBottom"/> and <see cref="InnerRight"/> of the rectangle
		/// </summary>
		/// <remarks>
		/// Similar to <seealso cref="BottomRight"/> but inside the rectangle's bounds instead of just to the right and bottom
		/// </remarks>
		public Point InnerBottomRight
		{
			get { return new Point (InnerRight, InnerBottom); }
			set { InnerBottom = value.Y; InnerRight = value.X; }
		}

		/// <summary>
		/// Gets or sets the point at the <see cref="InnerBottom"/> and <see cref="Left"/> of the rectangle
		/// </summary>
		/// <remarks>
		/// Similar to <seealso cref="BottomLeft"/> but inside the rectangle's bounds instead of just below the bottom
		/// </remarks>
		public Point InnerBottomLeft
		{
			get { return new Point (Left, InnerBottom); }
			set { InnerBottom = value.Y; Left = value.X; }
		}

		/// <summary>
		/// Gets or sets the bottom of the rectangle that is inside the bounds
		/// </summary>
		/// <remarks>
		/// Similar to <seealso cref="Bottom"/> but inside the rectangle's bounds instead of just below the bottom
		/// </remarks>
		public int InnerBottom
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
		public int InnerRight
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
		public Point Center
		{
			get { return new Point (MiddleX, MiddleY); }
			set { MiddleX = value.X; MiddleY = value.Y; }
		}

		/// <summary>
		/// Gets or sets the rectangle's middle horizontal position
		/// </summary>
		public int MiddleX
		{
			get { return X + (Width / 2); }
			set { X = value - (Width / 2); }
		}

		/// <summary>
		/// Gets or sets the rectangle's middle vertical position
		/// </summary>
		public int MiddleY
		{
			get { return Y + (Height / 2); }
			set { Y = value - (Height / 2); }
		}

		/// <summary>
		/// Offsets the location of the rectangle by the specified <paramref name="x"/> and <paramref name="y"/> values
		/// </summary>
		/// <param name="x">Horizontal offset to move the rectangle</param>
		/// <param name="y">Vertical offset to move the rectangle</param>
		public void Offset (int x, int y)
		{
			location.X += x;
			location.Y += y;
		}
		
		/// <summary>
		/// Offsets the location of the rectangle by the specified <paramref name="size"/>
		/// </summary>
		/// <param name="size">Width and Height to move the rectangle</param>
		public void Offset (Size size)
		{
			location.X += size.Width;
			location.Y += size.Height;
		}
		
		/// <summary>
		/// Offsets the location of the rectangle by the X and Y values of the specified <paramref name="point"/>
		/// </summary>
		/// <param name="point">Point with values to offset the rectangle</param>
		public void Offset (Point point)
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
		public static Rectangle Offset (Rectangle rectangle, int x, int y)
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
		public static Rectangle Offset (Rectangle rectangle, Size size)
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
		public static Rectangle Offset (Rectangle rectangle, Point point)
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
		public void Inflate (Size size)
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
		public void Inflate (int width, int height)
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
		public static Rectangle Inflate (Rectangle rectangle, Size size)
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
		public static Rectangle Inflate (Rectangle rectangle, int width, int height)
		{
			rectangle.Inflate (width, height);
			return rectangle;
		}

		/// <summary>
		/// Aligns the rectangle to a grid of the specified <paramref name="gridSize"/>
		/// </summary>
		/// <remarks>
		/// This will align the top, left, right, and bottom to a grid by inflating each edge to the next grid line.
		/// </remarks>
		/// <param name="gridSize">Size of the grid to align the rectangle to</param>
		public void Align (Size gridSize)
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
		public void Align (int gridWidth, int gridHeight)
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
		public static Rectangle Align (Rectangle rectangle, Size gridSize)
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
		public static Rectangle Align (Rectangle rectangle, int gridWidth, int gridHeight)
		{
			rectangle.Align (gridWidth, gridHeight);
			return rectangle;
		}

		/// <summary>
		/// Union the <paramref name="rectangle"/> into this instance to encompass both rectangles
		/// </summary>
		/// <param name="rectangle">Rectangle to union with this instance</param>
		public void Union (Rectangle rectangle)
		{
			var left = Math.Min (Left, rectangle.Left);
			var top = Math.Min (Top, rectangle.Top);
			var right = Math.Max (Right, rectangle.Right);
			var bottom = Math.Max (Bottom, rectangle.Bottom);
			location = new Point (left, top);
			size = new Size (right - left, bottom - top);
		}

		/// <summary>
		/// Combines two rectangles into one rectangle that encompasses both
		/// </summary>
		/// <param name="rect1">First rectangle to union</param>
		/// <param name="rect2">Second rectangle to union</param>
		/// <returns>A new Rectangle that encompasses both <paramref name="rect1"/> and <paramref name="rect2"/></returns>
		public static Rectangle Union (Rectangle rect1, Rectangle rect2)
		{
			rect1.Union (rect2);
			return rect1;
		}

		/// <summary>
		/// Intersect the rectangle with the specified <paramref name="rectangle"/>
		/// </summary>
		/// <param name="rectangle">Rectangle to intersect with</param>
		public void Intersect (Rectangle rectangle)
		{
			var left = Math.Max (Left, rectangle.Left);
			var top = Math.Max (Top, rectangle.Top);
			var width = Math.Max (Math.Min (Right, rectangle.Right) - left, 0);
			var height = Math.Max (Math.Min (Bottom, rectangle.Bottom) - top, 0);
			location = new Point (left, top);
			size = new Size (width, height);
		}
		
		/// <summary>
		/// Intersect the two specified rectangles
		/// </summary>
		/// <param name="rect1">First rectangle to intersect</param>
		/// <param name="rect2">Second rectangle to intersect</param>
		/// <returns>A new RectangleF with the intersection of the two rectangles</returns>
		public static Rectangle Intersect (Rectangle rect1, Rectangle rect2)
		{
			rect1.Intersect (rect2);
			return rect1;
		}

		/// <summary>
		/// Restricts the rectangle to be within the specified <paramref name="location"/> and <paramref name="size"/>
		/// </summary>
		/// <remarks>
		/// This is a shortcut for <seealso cref="Restrict(Rectangle)"/>
		/// </remarks>
		/// <param name="location">Minimum location for the rectangle</param>
		/// <param name="size">Maximum size for the rectangle</param>
		public void Restrict (Point location, Size size)
		{
			Restrict (new Rectangle (location, size));
		}
		
		/// <summary>
		/// Restricts the rectangle to be within the specified <paramref name="size"/> at an X,Y location of 0, 0
		/// </summary>
		/// <remarks>
		/// This is a shortcut for <seealso cref="Restrict(Rectangle)"/>
		/// </remarks>
		/// <param name="size">Maxiumum size for the rectangle</param>
		public void Restrict (Size size)
		{
			Restrict (new Rectangle (size));
		}
		
		/// <summary>
		/// Restricts the rectangle to be within the specified <paramref name="rectangle"/>
		/// </summary>
		/// <remarks>
		/// This ensures that the current rectangle's bounds fall within the bounds of the specified <paramref name="rectangle"/>.
		/// It is useful to ensure that the rectangle does not exceed certain limits (e.g. for drawing)
		/// </remarks>
		/// <param name="rectangle">Rectangle to restrict this instance to</param>
		public void Restrict (Rectangle rectangle)
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
		/// Restricts the <paramref name="rectangle"/> to be within the specified <paramref name="size"/>
		/// </summary>
		/// <remarks>
		/// This ensures that <paramref name="rectangle"/>'s bounds fall within the bounds of the specified <paramref name="size"/>
		/// It is useful to ensure that the rectangle does not exceed certain limits (e.g. for drawing)
		/// </remarks>
		/// <param name="rectangle">Rectangle to restrict</param>
		/// <param name="size">Size to restrict to</param>
		/// <returns>A new rectangle restricted to the restrict bounds</returns>
		public static Rectangle Restrict (Rectangle rectangle, Size size)
		{
			rectangle.Restrict (size);
			return rectangle;
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
		public static Rectangle operator * (Rectangle rectangle, int factor)
		{
			return new Rectangle (rectangle.X * factor, rectangle.Y * factor, rectangle.Width * factor, rectangle.Height * factor);
		}

		/// <summary>
		/// Divides all X, Y, Width, Height components of the <paramref name="rectangle"/> by a <paramref name="factor"/> factor
		/// </summary>
		/// <param name="rectangle">Rectangle to divide</param>
		/// <param name="factor">Factor to divide by</param>
		/// <returns>A new instance of a Rectangle with the value of <paramref name="rectangle"/> divided by a <paramref name="factor"/></returns>
		public static Rectangle operator / (Rectangle rectangle, int factor)
		{
			return new Rectangle (rectangle.X / factor, rectangle.Y / factor, rectangle.Width / factor, rectangle.Height / factor);
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
		public static Rectangle operator * (Rectangle rectangle, Size size)
		{
			return new Rectangle (rectangle.X * size.Width, rectangle.Y * size.Height, rectangle.Width * size.Width, rectangle.Height * size.Height);
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
		public static Rectangle operator / (Rectangle rectangle, Size size)
		{
			return new Rectangle (rectangle.X / size.Width, rectangle.Y / size.Height, rectangle.Width / size.Width, rectangle.Height / size.Height);
		}

		/// <summary>
		/// Adds the <paramref name="offset"/> to the specified <paramref name="rectangle"/>, moving its location
		/// </summary>
		/// <param name="rectangle">Rectangle to offset</param>
		/// <param name="offset">Offset to move the location by</param>
		public static Rectangle operator + (Rectangle rectangle, Point offset)
		{
			rectangle.Offset (offset);
			return rectangle;
		}

		/// <summary>
		/// Subtracts the <paramref name="offset"/> from the specified <paramref name="rectangle"/>, moving its location
		/// </summary>
		/// <param name="rectangle">Rectangle to offset</param>
		/// <param name="offset">Offset to move the location by</param>
		public static Rectangle operator - (Rectangle rectangle, Point offset)
		{
			rectangle.Offset (-offset);
			return rectangle;
		}
		
		/// <summary>
		/// Adds the <paramref name="offset"/> to the specified <paramref name="rectangle"/>, moving its location
		/// </summary>
		/// <param name="rectangle">Rectangle to offset</param>
		/// <param name="offset">Offset to move the location by</param>
		public static Rectangle operator + (Rectangle rectangle, Size offset)
		{
			rectangle.Offset (offset);
			return rectangle;
		}
		
		/// <summary>
		/// Subtracts the <paramref name="offset"/> from the specified <paramref name="rectangle"/>, moving its location
		/// </summary>
		/// <param name="rectangle">Rectangle to offset</param>
		/// <param name="offset">Offset to move the location by</param>
		public static Rectangle operator - (Rectangle rectangle, Size offset)
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
		public static bool operator == (Rectangle rect1, Rectangle rect2)
		{
			return rect1.location == rect2.location && rect1.size == rect2.size;
		}

		/// <summary>
		/// Compares two rectangles for inequality
		/// </summary>
		/// <param name="rect1">First rectangle to compare</param>
		/// <param name="rect2">Second rectangle to compare</param>
		/// <returns>True if the two rectangles are not equal, false otherwise</returns>
		public static bool operator != (Rectangle rect1, Rectangle rect2)
		{
			return !(rect1 == rect2);
		}

		/// <summary>
		/// Implicit conversion from a <see cref="RectangleF"/> to a <see cref="Rectangle"/>
		/// </summary>
		/// <param name="rectangle">Point to convert</param>
		/// <returns>A new instance of a Rectangle with the value of the specified <paramref name="rectangle"/></returns>
		public static explicit operator Rectangle (RectangleF rectangle)
		{
			return new Rectangle ((Point)rectangle.Location, (Size)rectangle.Size);
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
			return obj is Rectangle && (Rectangle)obj == this;
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
		public bool Equals (Rectangle other)
		{
			return other == this;
		}
	}
}
