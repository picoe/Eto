using System;
using sc = System.ComponentModel;
using System.Globalization;

namespace Eto.Drawing
{
	/// <summary>
	/// A struct representing X and Y co-ordinates as integer values
	/// </summary>
	/// <remarks>
	/// The point struct is used for drawing and positioning of elements and widgets
	/// </remarks>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[sc.TypeConverter(typeof(PointConverterInternal))]
	public struct Point : IEquatable<Point>
	{
		int x;
		int y;
		
		/// <summary>
		/// Gets an empty point with an X and Y value of zero
		/// </summary>
		public static readonly Point Empty = new Point (0, 0);
		
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
		/// Gets the point as a normal vector (perpendicular) to the current point from the origin
		/// </summary>
		/// <value>The normal vector of this point</value>
		public Point Normal
		{
			get { return new Point (-Y, X); }
		}
		
		/// <summary>
		/// Creates a unit vector PointF (a point with a <see cref="Length"/> of 1.0 from origin 0,0) with the specified angle, in degrees
		/// </summary>
		/// <returns>A new instance of a PointF with the x,y co-ordinates set at a distance of 1.0 from the origin</returns>
		/// <param name="angle">Angle in degrees of the unit vector</param>
		public static PointF UnitVectorAtAngle (float angle)
		{
			angle = angle * Helper.DegreesToRadians;
			return new PointF ((float)Math.Cos(angle), (float)Math.Sin(angle));
		}
		
		/// <summary>
		/// Gets the current point as a unit vector (a point with a <see cref="Length"/> of 1.0 from origin 0,0)
		/// </summary>
		/// <value>The unit vector equivalent of this point's X and Y coordinates</value>
		public PointF UnitVector
		{
			get
			{
				double length = Length;
				
				const float epsilon = 1e-6f;
				var x = X;
				var y = Y;
				
				// deal with very small points without blowing up.
				int iterations = 0;
				while (length < epsilon && iterations < 3)
				{
					if (Math.Abs (x) > epsilon)
					{
						y = y * x;
						x = 1;
					}
					else if (Math.Abs (y) > epsilon)
					{
						x = x * y;
						y = 1;
					}
					
					length = Math.Sqrt (x * x + y * y);
					
					iterations++;
				}
				
				if (iterations == 3)
					return new PointF (1, 0); // arbitrary
				
				return new PointF ((float)(X / length), (float)(Y / length));
			}
		}
		
		/// <summary>
		/// Gets the length of the point as a vector from origin 0,0
		/// </summary>
		/// <value>The length of this point as a vector</value>
		public float Length
		{
			get { return (float)Math.Sqrt (X * X + Y * Y); }
		}
		
		/// <summary>
		/// Gets the squared length of the point as a vector from origin 0,0.
		/// </summary>
		/// <value>The length of the squared.</value>
		public int LengthSquared
		{
			get { return X * X + Y * Y; }
		}
		
		/// <summary>
		/// Gets a value indicating that both the X and Y co-ordinates of this point are zero
		/// </summary>
		public bool IsZero
		{
			get { return x == 0 && y == 0; }
		}
		
		/// <summary>
		/// Gets the distance between this point and the specified <paramref name="point"/>
		/// </summary>
		/// <param name="point">Point to calculate the distance from</param>
		public float Distance (Point point)
		{
			var dx = Math.Abs (X - point.X);
			var dy = Math.Abs (Y - point.Y);
			return (float)Math.Sqrt (dx * dx + dy * dy);
		}
		
		/// <summary>
		/// Gets the distance between two points using pythagoras theorem
		/// </summary>
		/// <param name="point1">First point to calculate the distance from</param>
		/// <param name="point2">Second point to calculate the distance to</param>
		/// <returns>The distance between the two points</returns>
		public static float Distance (Point point1, Point point2)
		{
			return point1.Distance (point2);
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
		/// Restricts the X and Y co-ordinates of the specified <paramref name="point"/> within the <paramref name="rectangle"/>
		/// </summary>
		/// <param name="point">Point to restrict</param>
		/// <param name="rectangle">Rectangle to restrict the point within</param>
		/// <returns>A new point that falls within the <paramref name="rectangle"/></returns>
		public static Point Restrict (Point point, Rectangle rectangle)
		{
			point.Restrict (rectangle);
			return point;
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
			Offset(point.X, point.Y);
		}
		
		/// <summary>
		/// Offsets the X and Y co-ordinates of the <paramref name="point"/> by the specified <paramref name="x"/> and <paramref name="y"/> values
		/// </summary>
		/// <param name="point">Point to offset</param>
		/// <param name="x">Value to add to the X co-ordinate of this point</param>
		/// <param name="y">Value to add to the Y co-ordinate of this point</param>
		/// <returns>A new point with the offset X and Y values</returns>
		public static Point Offset (Point point, int x, int y)
		{
			point.Offset (x, y);
			return point;
		}
		
		/// <summary>
		/// Offsets the X and Y co-ordinates of the <paramref name="point"/> by the values from the specified <paramref name="offset"/>
		/// </summary>
		/// <param name="point">Point to offset</param>
		/// <param name="offset">Point with X and Y values to add to this point</param>
		/// <returns>A new point offset by the specified value</returns>
		public static Point Offset (Point point, Point offset)
		{
			point.Offset (offset);
			return point;
		}
		
		/// <summary>
		/// Gets the dot product of this instance and the specified <paramref name="point"/>
		/// </summary>
		/// <param name="point">Point to get the dot product for</param>
		/// <returns>The dot product (X * point.X + Y * point.Y) between this point and the specified point</returns>
		public int DotProduct (Point point)
		{
			return x * point.x + y * point.y;
		}
		
		/// <summary>
		/// Gets the dot product between two points
		/// </summary>
		/// <param name="point1">First point to get the dot product</param>
		/// <param name="point2">Second point to get the dot product</param>
		/// <returns>The dot product (point1.X * point2.X + poin1.Y * point2.Y) between the two points</returns>
		public static int DotProduct (Point point1, Point point2)
		{
			return point1.DotProduct (point2);
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
		/// Multiplies the X and Y co-ordinates of the specified <paramref name="point"/> with a given <paramref name="factor"/>
		/// </summary>
		/// <param name="point">Base point value</param>
		/// <param name="factor">Value to multiply the X and Y co-ordinates with</param>
		/// <returns>A new instance of a point with the product of the X and Y co-ordinates of the <paramref name="point"/> and specified <paramref name="factor"/></returns>
		public static Point operator * (Point point, int factor)
		{
			return new Point (point.X * factor, point.Y * factor);
		}
		
		/// <summary>
		/// Multiplies the X and Y co-ordinates of the specified <paramref name="point"/> with a given <paramref name="factor"/>
		/// </summary>
		/// <param name="point">Base point value</param>
		/// <param name="factor">Value to multiply the X and Y co-ordinates with</param>
		/// <returns>A new instance of a point with the product of the X and Y co-ordinates of the <paramref name="point"/> and specified <paramref name="factor"/></returns>
		public static Point operator * (int factor, Point point)
		{
			return new Point (point.X * factor, point.Y * factor);
		}
		
		/// <summary>
		/// Multiplies the <see cref="X"/> and <see cref="Y"/> of a <paramref name="point"/> by the specified floating point <paramref name="factor"/>
		/// </summary>
		/// <param name="point">Point to multiply</param>
		/// <param name="factor">Factor to multiply both the X and Y coordinates by</param>
		/// <returns>A new instance of a PointF struct with the product of the <paramref name="point"/> and <paramref name="factor"/></returns>
		public static PointF operator * (Point point, float factor)
		{
			return new PointF (point.X * factor, point.Y * factor);
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
		/// Explicit conversion from a <see cref="PointF"/> to a <see cref="Point"/> by truncating values
		/// </summary>
		/// <param name="point">Point to convert</param>
		/// <returns>A new instance of a Point with the value of the specified <paramref name="point"/></returns>
		public static explicit operator Point (PointF point)
		{
			return new Point ((int)point.X, (int)point.Y);
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
			return String.Format (CultureInfo.InvariantCulture, "{0},{1}", x, y);
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
