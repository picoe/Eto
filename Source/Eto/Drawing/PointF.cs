using System;
using System.ComponentModel;

namespace Eto.Drawing
{
	/// <summary>
	/// A struct representing X and Y co-ordinates as floating point values
	/// </summary>
	/// <remarks>
	/// The point struct is used for drawing and positioning of elements and widgets
	/// </remarks>
	[TypeConverter (typeof (PointFConverter))]
	public struct PointF : IEquatable<PointF>
	{
		float x;
		float y;

		#region Obsolete

		/// <summary>
		/// Obsolete. Do not use.
		/// </summary>
		[Obsolete ("Use operator + instead")]
		public static PointF Add (PointF point, SizeF size)
		{
			return new PointF (point.X + size.Width, point.Y + size.Height);
		}

		/// <summary>
		/// Gets a value indicating that both the X and Y co-ordinates of this point are zero
		/// </summary>
		[Obsolete ("Use PointF.IsZero instead")]
		public bool IsEmpty
		{
			get { return x == 0 && y == 0; }
		}

		#endregion

		/// <summary>
		/// Gets an empty point with an X and Y value of zero
		/// </summary>
		public static readonly PointF Empty = new PointF (0, 0);

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
		/// Returns the minimum X and Y components of two points
		/// </summary>
		/// <param name="point1">First point</param>
		/// <param name="point2">Second point</param>
		/// <returns>A new point with the minimum X and Y values of the two points</returns>
		public static PointF Min (PointF point1, PointF point2)
		{
			return new PointF (Math.Min (point1.X, point2.X), Math.Min (point1.Y, point2.Y));
		}

		/// <summary>
		/// Returns the maximum X and Y components of two points
		/// </summary>
		/// <param name="point1">First point</param>
		/// <param name="point2">Second point</param>
		/// <returns>A new point with the maximum X and Y values of the two points</returns>
		public static PointF Max (PointF point1, PointF point2)
		{
			return new PointF (Math.Max (point1.X, point2.X), Math.Max (point1.Y, point2.Y));
		}

		/// <summary>
		/// Returns the absolute X and Y components of the specified <paramref name="point"/>
		/// </summary>
		/// <param name="point">Point with positive or negative X and/or Y values</param>
		/// <returns>A new point with absolute (positive) X and Y values of the specified <paramref name="point"/></returns>
		public static PointF Abs (PointF point)
		{
			return new PointF (Math.Abs (point.X), Math.Abs (point.Y));
		}

		/// <summary>
		/// Initializes a new instance of a Point class with specified <paramref name="x"/> and <paramref name="y"/> values
		/// </summary>
		/// <param name="x">Initial X value for the point</param>
		/// <param name="y">Initial Y value for the point</param>
		public PointF (float x, float y)
		{
			this.x = x;
			this.y = y;
		}

        public static PointF UnitVectorAtAngle(double radians)
        {
            return new PointF(
                (float)Math.Cos(radians),
                (float)Math.Sin(radians));
        }

        public static PointF UnitVectorAtAngleD(double degrees)
        {
            return new PointF(
                (float)MathUtil.CosD(degrees),
                (float)MathUtil.SinD(degrees));
        }

		/// <summary>
		/// Initializes a new instance of a Point class with <see cref="X"/> and <see cref="Y"/> values corresponding to the <see cref="Size.Width"/> and <see cref="Size.Height"/> values 
		/// of the specified <paramref name="size"/>, respecitively
		/// </summary>
		/// <param name="size">Size to initialize the X and Y values of the new instance with</param>
		public PointF (SizeF size)
		{
			this.x = size.Width;
			this.y = size.Height;
		}

		/// <summary>
		/// Gets or sets the X co-ordinate of this point
		/// </summary>
		public float X
		{
			get { return x; }
			set { x = value; }
		}

		/// <summary>
		/// Gets or sets the Y co-ordinate of this point
		/// </summary>
		public float Y
		{
			get { return y; }
			set { y = value; }
		}

        public double Magnitude
        {
            get
            {
                return Math.Sqrt(X * X + Y * Y);
            }
        }

        public PointF Normal
        {
            get
            {
                return new PointF(-Y, X);
            }
        }

        public PointF UnitVector
        {
            get
            {
                double magnitude = Magnitude;

                var epsilon = 1e-6;

                var x = X;
                var y = Y;

                // deal with very small points without blowing up.
                int iterations = 0;
                while (
                    magnitude < epsilon &&
                    iterations < 3)
                {

                    if (Math.Abs(x) > epsilon)
                    {
                        y = y * x;
                        x = 1;
                    }
                    else if (Math.Abs(y) > epsilon)
                    {
                        x = x * y;
                        y = 1;
                    }

                    magnitude = Math.Sqrt(x * x + y * y);

                    iterations++;
                }

                if (iterations == 3)
                    return new PointF(1, 0); // arbitrary

                return new PointF(
                    (float)(X / magnitude), 
                    (float)(Y / magnitude));
            }
        }

        public static PointF operator -(PointF point1)
        {
            return new PointF(-point1.x, -point1.y);
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
		public void Restrict (RectangleF rectangle)
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
		/// Adds the specified <paramref name="x"/> and <paramref name="y"/> values to this point
		/// </summary>
		/// <param name="x">Value to add to the X co-ordinate of this point</param>
		/// <param name="y">Value to add to the Y co-ordinate of this point</param>
		public void Add (float x, float y)
		{
			this.x += x;
			this.y += y;
		}

		/// <summary>
		/// Adds the X and Y co-ordinate values of the specified <paramref name="point"/> to this point
		/// </summary>
		/// <param name="point">Point with X and Y values to add to this point</param>
		public void Add (PointF point)
		{
			this.Add (point.X, point.Y);
		}

		/// <summary>
		/// Operator to return the difference between two points as a <see cref="PointF"/>
		/// </summary>
		/// <param name="point1">Base point value</param>
		/// <param name="point2">Point to subtract</param>
		/// <returns>A new instance of a PointF with the X and Y equal to the difference of the X and Y co-ordinates, respectively</returns>
		public static PointF operator - (PointF point1, PointF point2)
		{
            return new PointF(point1.x - point2.x, point1.y - point2.y);
		}

		/// <summary>
		/// Operator to return the addition of two points as a <see cref="PointF"/>
		/// </summary>
		/// <param name="point1">Base point value</param>
		/// <param name="point2">Point to add</param>
		/// <returns>A new instance of a PointF with the X and Y equal to the sum of the two point's X and Y co-ordinates, respectively</returns>
		public static PointF operator + (PointF point1, PointF point2)
		{
			return new PointF (point1.x + point2.x, point1.y + point2.y);
		}

		/// <summary>
		/// Operator to add a size to a point
		/// </summary>
		/// <param name="point">Base point value</param>
		/// <param name="size">Size to add to the point's X and Y co-ordinates</param>
		/// <returns>A new point with the sum of the specified <paramref name="point"/>'s X and Y components and the <paramref name="size"/></returns>
		public static PointF operator + (PointF point, SizeF size)
		{
			return new PointF (point.x + size.Width, point.y + size.Height);
		}

		/// <summary>
		/// Operator to subtract a size from a point
		/// </summary>
		/// <param name="point">Base point value</param>
		/// <param name="size">Size to subtract to the point's X and Y co-ordinates</param>
		/// <returns>A new point with the sum of the specified <paramref name="point"/>'s X and Y components and the <paramref name="size"/></returns>
		public static PointF operator - (PointF point, SizeF size)
		{
			return new PointF (point.x - size.Width, point.y - size.Height);
		}

        public static PointF operator +(PointF point, SizeF size)
        {
            return new PointF(point.x + size.Width, point.y + size.Height);
        }

        public static PointF operator -(PointF point, SizeF size)
        {
            return new PointF(point.x - size.Width, point.y - size.Height);
        }

		/// <summary>
		/// Operator to add a <paramref name="value"/> to both the X and Y co-ordinates of a point
		/// </summary>
		/// <param name="point">Base point value</param>
		/// <param name="value">Value to add to both the X and Y co-ordinates of the point</param>
		/// <returns>A new instance of a point with the sum of the <paramref name="point"/>'s X and Y co-ordinates and the specified <paramref name="value"/></returns>
		public static PointF operator + (PointF point, float value)
		{
			return new PointF (point.x + value, point.y + value);
		}

		/// <summary>
		/// Operator to subtract a <paramref name="value"/> from both the X and Y co-ordinates of a point
		/// </summary>
		/// <param name="point">Base point value</param>
		/// <param name="value">Value to subtract to both the X and Y co-ordinates of the point</param>
		/// <returns>A new instance of a point with the value of the <paramref name="point"/>'s X and Y co-ordinates minus the specified <paramref name="value"/></returns>
		public static PointF operator - (PointF point, float value)
		{
			return new PointF (point.x - value, point.y - value);
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
		public static bool operator == (PointF point1, PointF point2)
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
		public static bool operator != (PointF point1, PointF point2)
		{
			return point1.x != point2.x || point1.y != point2.y;
		}

		/// <summary>
		/// Multiplies the specified <paramref name="point"/> with a <paramref name="size"/>
		/// </summary>
		/// <param name="point">Base point value</param>
		/// <param name="size">Size to multiply the X and Y co-ordinates with the Width and Height of the <paramref name="size"/>, respectively</param>
		/// <returns>A new instance of a point with the product of the specified <paramref name="point"/> and <paramref name="size"/></returns>
		public static PointF operator * (PointF point, SizeF size)
		{
			PointF result = point;
			result.x *= size.Width;
			result.y *= size.Height;
			return result;
		}

        public static PointF operator *(PointF p1, PointF p2)
        {
            PointF result = p1;
            result.x *= p2.X;
            result.y *= p2.Y;
            return result;
		}

		/// <summary>
		/// Divides the specified <paramref name="point"/> with a <paramref name="size"/>
		/// </summary>
		/// <param name="point">Base point value</param>
		/// <param name="size">Size to divide the X and Y co-ordinates with the Width and Height of the <paramref name="size"/>, respectively</param>
		/// <returns>A new instance of a point with the division of the specified <paramref name="point"/> and <paramref name="size"/></returns>
		public static PointF operator / (PointF point, SizeF size)
		{
			PointF result = point;
			result.x /= size.Width;
			result.y /= size.Height;
			return result;
		}

		/// <summary>
		/// Multiplies the X and Y co-ordinates of the specified <paramref name="point"/> with a given <paramref name="value"/>
		/// </summary>
		/// <param name="point">Base point value</param>
		/// <param name="value">Value to multiply the X and Y co-ordinates with</param>
		/// <returns>A new instance of a point with the product of the X and Y co-ordinates of the <paramref name="point"/> and specified <paramref name="value"/></returns>
		public static PointF operator * (PointF point, float value)
		{
            var result = point;
			result.x *= value;
			result.y *= value;
			return result;
		}

        public static PointF operator *(float value, PointF p)
        {
            return new PointF(
                p.X * value,
                p.Y * value);
        }

		/// <summary>
		/// Divides the X and Y co-ordinates of the specified <paramref name="point"/> with a given <paramref name="value"/>
		/// </summary>
		/// <param name="point">Base point value</param>
		/// <param name="value">Value to divide the X and Y co-ordinates with</param>
		/// <returns>A new instance of a point with the division of the X and Y co-ordinates of the <paramref name="point"/> and specified <paramref name="value"/></returns>
		public static PointF operator / (PointF point, float value)
		{
			var result = point;
			result.x /= value;
			result.y /= value;
			return result;
		}
		
		/// <summary>
		/// Implicit conversion from a <see cref="Point"/> to a <see cref="PointF"/>
		/// </summary>
		/// <remarks>
		/// Since no precision is lost, this can be implicit.
		/// </remarks>
		/// <param name="point">Point to convert</param>
		/// <returns>A new instance of a PointF with the value of the specified <paramref name="point"/></returns>
		public static implicit operator PointF (Point point)
		{
			return new PointF (point.X, point.Y);
		}

        public float SquaredLength
        {
            get
            {
                return X * X + Y * Y;
            }
        }

        public float Length
        {
            get
            {
                return (float)Math.Sqrt(X * X + Y * Y);
            }
        }

        public float Dot(PointF p)
        {
            return x * p.x + y * p.y;
        }

        public void Offset(PointF offset)
        {
            this.x += offset.X;
            this.y += offset.y;
        }

        public void Offset(float x, float y)
        {
            this.x += x;
            this.y += y;
        }

        public PointF RotateD(double degrees)
        {
            return 
                Rotate(
                    degrees * MathUtil.DegreesToRadians);
        }

        /// <summary>
        /// Treats the point as a vector and rotates it
        /// clockwise by theta.
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        public PointF Rotate(double radians)
        {
            return
                new PointF(
                    // X
                    (float)
                    (this.X * Math.Cos(radians) -
                     this.Y * Math.Sin(radians)),
                    // Y
                    (float)
                    (this.X * Math.Sin(radians) +
                     this.Y * Math.Cos(radians)));

        }

        public Point ToPoint()
        {
            return new Point((int)X, (int)Y);
        }

		/// <summary>
		/// Returns a value indicating that the specified <paramref name="obj"/> is equal to this point
		/// </summary>
		/// <param name="obj">Object to compare</param>
		/// <returns>True if the specified <paramref name="obj"/> is a Point and is equal to this instance, false otherwise</returns>
        public override bool Equals (object obj)
		{
			return obj is PointF && (PointF)obj == this;
		}

		/// <summary>
		/// Gets the hash code of this point
		/// </summary>
		/// <returns>Hash code for this point</returns>
		public override int GetHashCode ()
		{
			return X.GetHashCode () ^ Y.GetHashCode ();
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
		public bool Equals (PointF other)
		{
			return other == this;
		}
	}
}
