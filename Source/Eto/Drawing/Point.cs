using System;
using System.ComponentModel;

namespace Eto.Drawing
{
	/// <summary>
	/// A struct representing X and Y co-ordinates as integer values
	/// </summary>
	/// <remarks>
	/// The point struct is used for drawing and positioning of elements and widgets
	/// </remarks>
	[TypeConverter(typeof(PointConverter))]
	public struct Point : IEquatable<Point>
	{
		int x;
		int y;

		#region Obsolete

		/// <summary>
		/// Obsolete. Do not use.
		/// </summary>
		[Obsolete ("Use operator + instead")]
		public static Point Add (Point point, Size size)
		{
			return new Point (point.X + size.Width, point.Y + size.Height);
		}

		/// <summary>
		/// Gets a value indicating that both the X and Y co-ordinates of this point are zero
		/// </summary>
		[Obsolete ("Use IsZero instead")]
		public bool IsEmpty
		{
			get { return x == 0 && y == 0; }
		}

		/// <summary>
		/// Adds the specified <paramref name="x"/> and <paramref name="y"/> values to this point
		/// </summary>
		/// <param name="x">Value to add to the X co-ordinate of this point</param>
		/// <param name="y">Value to add to the Y co-ordinate of this point</param>
		[Obsolete("Use Offset() instead")]
		public void Add (int x, int y)
		{
			this.x += x;
			this.y += y;
		}
		
		/// <summary>
		/// Adds the X and Y co-ordinate values of the specified <paramref name="point"/> to this point
		/// </summary>
		/// <param name="point">Point with X and Y values to add to this point</param>
		[Obsolete("Use Offset() instead")]
		public void Add (Point point)
		{
			this.Add (point.X, point.Y);
		}

		#endregion

		/// <summary>
		/// Gets an empty point with an X and Y value of zero
		/// </summary>
		public static readonly Point Empty = new Point (0, 0);
		
		/// <summary>
		/// Gets the distance between two points using pythagoras theorem
		/// </summary>
		/// <param name="point1">First point to calculate the distance from</param>
		/// <param name="point2">Second point to calculate the distance to</param>
		/// <returns>The distance between the two points</returns>
		public static double Distance (Point point1, Point point2)
		{
			return Math.Sqrt (Math.Abs (point1.X - point2.X) + Math.Abs (point1.Y - point2.Y));
		}
		
		/// <summary>
		/// Truncates the X and Y components of the specified <paramref name="point"/> to a <see cref="Point"/>
		/// </summary>
		/// <param name="point">Floating point value to truncate</param>
		/// <returns>A new instance of a Point with truncated X and Y values of the specified <paramref name="point"/></returns>
		public static Point Truncate (PointF point)
		{
			return new Point ((int)point.X, (int)point.Y);
		}

		/// <summary>
		/// Rounds the X and Y components of the specified <paramref name="point"/> to a <see cref="Point"/>
		/// </summary>
		/// <param name="point">Floating point value to round</param>
		/// <returns>A new instance of a Point with rounded X and Y values of the specified <paramref name="point"/></returns>
		public static Point Round (PointF point)
		{
			return new Point ((int)Math.Round (point.X), (int)Math.Round (point.Y));
		}
		
		/// <summary>
		/// Returns the minimum X and Y components of two points
		/// </summary>
		/// <param name="point1">First point</param>
		/// <param name="point2">Second point</param>
		/// <returns>A new point with the minimum X and Y values of the two points</returns>
		public static Point Min (Point point1, Point point2)
		{
			return new Point (Math.Min (point1.X, point2.X), Math.Min (point1.Y, point2.Y));
		}

		/// <summary>
		/// Returns the maximum X and Y components of two points
		/// </summary>
		/// <param name="point1">First point</param>
		/// <param name="point2">Second point</param>
		/// <returns>A new point with the maximum X and Y values of the two points</returns>
		public static Point Max (Point point1, Point point2)
		{
			return new Point (Math.Max (point1.X, point2.X), Math.Max (point1.Y, point2.Y));
		}

		/// <summary>
		/// Returns the absolute X and Y components of the specified <paramref name="point"/>
		/// </summary>
		/// <param name="point">Point with positive or negative X and/or Y values</param>
		/// <returns>A new point with absolute (positive) X and Y values of the specified <paramref name="point"/></returns>
		public static Point Abs (Point point)
		{
			return new Point (Math.Abs (point.X), Math.Abs (point.Y));
		}
		
		/// <summary>
		/// Initializes a new instance of a Point class with specified <paramref name="x"/> and <paramref name="y"/> values
		/// </summary>
		/// <param name="x">Initial X value for the point</param>
		/// <param name="y">Initial Y value for the point</param>
		public Point (int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		/// <summary>
		/// Initializes a new instance of a Point class with <see cref="X"/> and <see cref="Y"/> values corresponding to the <see cref="Size.Width"/> and <see cref="Size.Height"/> values 
		/// of the specified <paramref name="size"/>, respecitively
		/// </summary>
		/// <param name="size">Size to initialize the X and Y values of the new instance with</param>
		public Point (Size size)
		{
			this.x = size.Width;
			this.y = size.Height;
		}
		
		/// <summary>
		/// Initializes a new instance of a Point class with truncated values of the specified floating-point <paramref name="point"/>
		/// </summary>
		/// <param name="point">PointF to initialize the X and Y values of the new instance with</param>
		public Point (PointF point)
		{
			this.x = (int)point.X;
			this.y = (int)point.Y;
		}
		
		/// <summary>
		/// Gets or sets the X co-ordinate of this point
		/// </summary>
		public int X
		{
			get { return x; }
			set { x = value; }
		}

		/// <summary>
		/// Gets or sets the Y co-ordinate of this point
		/// </summary>
		public int Y
		{
			get { return y; }
			set { y = value; }
		}

		/// <summary>
		/// Gets the magnitude of this point from 0,0 using Pythagoras' theorem
		/// </summary>
		public double Magnitude
		{
			get { return Math.Sqrt (X * X + Y * Y); }
		}

		/// <summary>
		/// Gets a value indicating that both the X and Y co-ordinates of this point are zero
		/// </summary>
		public bool IsZero
		{
			get { return x == 0 && y == 0; }
		}
		
		/// <summary>
		/// Restricts the X and Y co-ordinates within the specified <paramref name="rectangle"/>
		/// </summary>
		/// <remarks>
		/// This will update the X and Y co-ordinates to be within the specified <paramref name="rectangle"/>'s bounds.
		/// The updated co-ordinates will be the closest to the original value as possible.
		/// E.g. if the X co-ordinate is greater than the <see cref="Rectangle.Right"/> of the rectangle, it will be set
		/// to be <see cref="Rectangle.Right"/> minus one, to be within the rectangle's bounds.
		/// </remarks>
		/// <param name="rectangle">Rectangle to restrict the X and Y co-ordinates in</param>
		public void Restrict (Rectangle rectangle)
		{
			if (x < rectangle.Left)
				x = rectangle.Left;
			if (x > rectangle.InnerRight)
				x = rectangle.InnerRight;
			if (y < rectangle.Top)
				y = rectangle.Top;
			if (y > rectangle.InnerBottom)
				y = rectangle.InnerBottom;
		}
		
		/// <summary>
		/// Offsets the X and Y co-ordinates of this point by the specified <paramref name="x"/> and <paramref name="y"/> values
		/// </summary>
		/// <param name="x">Value to add to the X co-ordinate of this point</param>
		/// <param name="y">Value to add to the Y co-ordinate of this point</param>
		public void Offset (int x, int y)
		{
			this.x += x;
			this.y += y;
		}
		
		/// <summary>
		/// Offsets the X and Y co-ordinates of this point by the values from the specified <paramref name="point"/>
		/// </summary>
		/// <param name="point">Point with X and Y values to add to this point</param>
		public void Offset (Point point)
		{
			this.Offset (point.X, point.Y);
		}

		/// <summary>
		/// Returns a new Point with negative x and y values of the specified <paramref name="point"/>
		/// </summary>
		/// <param name="point">Point to negate</param>
		public static Point operator - (Point point)
		{
			return new Point (-point.x, -point.y);
		}

		/// <summary>
		/// Operator to return the difference between two points as a <see cref="Size"/>
		/// </summary>
		/// <param name="point1">Base point value</param>
		/// <param name="point2">Point to subtract</param>
		/// <returns>A new instance of a Size with the X and Y equal to the difference of the X and Y co-ordinates, respectively</returns>
		public static Point operator - (Point point1, Point point2)
		{
			return new Point (point1.x - point2.x, point1.y - point2.y);
		}

		/// <summary>
		/// Operator to return the addition of two points as a <see cref="Point"/>
		/// </summary>
		/// <param name="point1">Base point value</param>
		/// <param name="point2">Point to add</param>
		/// <returns>A new instance of a Point with the X and Y equal to the sum of the two point's X and Y co-ordinates, respectively</returns>
		public static Point operator + (Point point1, Point point2)
		{
			return new Point (point1.x + point2.x, point1.y + point2.y);
		}

		/// <summary>
		/// Operator to add a size to a point
		/// </summary>
		/// <param name="point">Base point value</param>
		/// <param name="size">Size to add to the point's X and Y co-ordinates</param>
		/// <returns>A new point with the sum of the specified <paramref name="point"/>'s X and Y components and the <paramref name="size"/></returns>
		public static Point operator + (Point point, Size size)
		{
			return new Point (point.x + size.Width, point.y + size.Height);
		}

		/// <summary>
		/// Operator to subtract a size from a point
		/// </summary>
		/// <param name="point">Base point value</param>
		/// <param name="size">Size to subtract to the point's X and Y co-ordinates</param>
		/// <returns>A new point with the sum of the specified <paramref name="point"/>'s X and Y components and the <paramref name="size"/></returns>
		public static Point operator - (Point point, Size size)
		{
			return new Point (point.x - size.Width, point.y - size.Height);
		}

		/// <summary>
		/// Operator to add a <paramref name="value"/> to both the X and Y co-ordinates of a point
		/// </summary>
		/// <param name="point">Base point value</param>
		/// <param name="value">Value to add to both the X and Y co-ordinates of the point</param>
		/// <returns>A new instance of a point with the sum of the <paramref name="point"/>'s X and Y co-ordinates and the specified <paramref name="value"/></returns>
		public static Point operator + (Point point, int value)
		{
			return new Point (point.x + value, point.y + value);
		}

		/// <summary>
		/// Operator to subtract a <paramref name="value"/> from both the X and Y co-ordinates of a point
		/// </summary>
		/// <param name="point">Base point value</param>
		/// <param name="value">Value to subtract to both the X and Y co-ordinates of the point</param>
		/// <returns>A new instance of a point with the value of the <paramref name="point"/>'s X and Y co-ordinates minus the specified <paramref name="value"/></returns>
		public static Point operator - (Point point, int value)
		{
			return new Point (point.x - value, point.y - value);
		}
		
		/// <summary>
		/// Determines equality between two points
		/// </summary>
		/// <remarks>
		/// Equality is when both the X and Y values of both points are equal
		/// </remarks>
		/// <param name="point1">First point to compare</param>
		/// <param name="point2">Second point to compare</param>
		/// <returns>True if both points are equal, false if not</returns>
		public static bool operator == (Point point1, Point point2)
		{
			return point1.x == point2.x && point1.y == point2.y;
		}

		/// <summary>
		/// Determines the inequality between two points
		/// </summary>
		/// <remarks>
		/// Inequality is when either the X and Y values of both points are different
		/// </remarks>
		/// <param name="point1">First point to compare</param>
		/// <param name="point2">Second point to compare</param>
		/// <returns>True if the two points are not equal, false if not</returns>
		public static bool operator != (Point point1, Point point2)
		{
			return point1.x != point2.x || point1.y != point2.y;
		}

		/// <summary>
		/// Multiplies the specified <paramref name="point"/> with a <paramref name="size"/>
		/// </summary>
		/// <param name="point">Base point value</param>
		/// <param name="size">Size to multiply the X and Y co-ordinates with the Width and Height of the <paramref name="size"/>, respectively</param>
		/// <returns>A new instance of a point with the product of the specified <paramref name="point"/> and <paramref name="size"/></returns>
		public static Point operator * (Point point, Size size)
		{
			return new Point (point.X * size.Width, point.Y * size.Height);
		}

		/// <summary>Multiplies the X and Y co-ordinates of the two specified point values</summary>
		/// <param name="point1">First point to multiply</param>
		/// <param name="point2">Secont point to multiply</param>
		public static Point operator * (Point point1, Point point2)
		{
			return new Point (point1.X * point2.X, point1.Y * point2.Y);
		}

		/// <summary>
		/// Divides the specified <paramref name="point"/> with a <paramref name="size"/>
		/// </summary>
		/// <param name="point">Base point value</param>
		/// <param name="size">Size to divide the X and Y co-ordinates with the Width and Height of the <paramref name="size"/>, respectively</param>
		/// <returns>A new instance of a point with the division of the specified <paramref name="point"/> and <paramref name="size"/></returns>
		public static Point operator / (Point point, Size size)
		{
			return new Point (point.X / size.Width, point.Y / size.Height);
		}

		/// <summary>
		/// Multiplies the X and Y co-ordinates of the specified <paramref name="point"/> with a given <paramref name="value"/>
		/// </summary>
		/// <param name="point">Base point value</param>
		/// <param name="value">Value to multiply the X and Y co-ordinates with</param>
		/// <returns>A new instance of a point with the product of the X and Y co-ordinates of the <paramref name="point"/> and specified <paramref name="value"/></returns>
		public static Point operator * (Point point, int value)
		{
			return new Point (point.X * value, point.Y * value);
		}

		/// <summary>
		/// Divides the X and Y co-ordinates of the specified <paramref name="point"/> with a given <paramref name="value"/>
		/// </summary>
		/// <param name="point">Base point value</param>
		/// <param name="value">Value to divide the X and Y co-ordinates with</param>
		/// <returns>A new instance of a point with the division of the X and Y co-ordinates of the <paramref name="point"/> and specified <paramref name="value"/></returns>
		public static Point operator / (Point point, int value)
		{
			return new Point (point.X / value, point.Y / value);
		}

		/// <summary>
		/// Explicit conversion from a <paramref name="size"/> to a Point with a X and Y of the Width and Height values of the size, respectively
		/// </summary>
		/// <param name="size">Size to convert</param>
		/// <returns>A new size with the width and height of the X and Y values of the point, respectively</returns>
		public static explicit operator Point (Size size)
		{
			return new Point (size);
		}

        public PointF ToPointF()
        {
            return new PointF(x, y);
        }

		/// <summary>
		/// Returns a value indicating that the specified <paramref name="obj"/> is equal to this point
		/// </summary>
		/// <param name="obj">Object to compare</param>
		/// <returns>True if the specified <paramref name="obj"/> is a Point and is equal to this instance, false otherwise</returns>
		public override bool Equals (object obj)
		{
			return obj is Point && (Point)obj == this;
		}

		/// <summary>
		/// Gets the hash code of this point
		/// </summary>
		/// <returns>Hash code for this point</returns>
		public override int GetHashCode ()
		{
			return X ^ Y;
		}

		/// <summary>
		/// Converts this point to a string
		/// </summary>
		/// <returns>String representation of this point</returns>
		public override string ToString ()
		{
			return String.Format ("X={0} Y={1}", x, y);
		}

		/// <summary>
		/// Returns a value indicating that the specified <paramref name="other"/> point is equal to this point
		/// </summary>
		/// <param name="other">Other point to compare</param>
		/// <returns>True if the other point is equal to this point, otherwise false</returns>
		public bool Equals (Point other)
		{
			return other == this;
		}
	}
}
