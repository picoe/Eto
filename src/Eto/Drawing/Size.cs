using System;
using sc = System.ComponentModel;
using System.Globalization;

namespace Eto.Drawing
{
	/// <summary>
	/// Represents a size with width and height components
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[sc.TypeConverter(typeof(SizeConverterInternal))]
	public struct Size : IEquatable<Size>
	{
		/// <summary>
		/// Gets or sets the width
		/// </summary>
		public int Width { get; set; }
		
		/// <summary>
		/// Gets or sets the height
		/// </summary>
		public int Height { get; set; }
		
		/// <summary>
		/// Gets an empty size with a zero width and height
		/// </summary>
		public static readonly Size Empty = new Size (0, 0);
		
		/// <summary>
		/// Converts a floating point <paramref name="size"/> to an integral size by rounding the width and height
		/// </summary>
		/// <param name="size">Size to convert</param>
		/// <returns>A new instance of a Size class with rounded width and height values of the specified <paramref name="size"/></returns>
		public static Size Round (SizeF size)
		{
			return new Size ((int)Math.Round (size.Width), (int)Math.Round (size.Height));
		}
		
		/// <summary>
		/// Converts a floating point <paramref name="size"/> to an integral size by truncating the width and height
		/// </summary>
		/// <param name="size">Size to convert</param>
		/// <returns>A new instance of a Size struct with truncated width and height values of the specified <paramref name="size"/></returns>
		public static Size Truncate (SizeF size)
		{
			return new Size ((int)size.Width, (int)size.Height);
		}
		
		/// <summary>
		/// Returns the minimum width and height of two sizes
		/// </summary>
		/// <param name="size1">First size to get the minimum values</param>
		/// <param name="size2">Second size to get the minimum values</param>
		/// <returns>A new instance of a Size struct with the minimum width and height of either sizes</returns>
		public static Size Min (Size size1, Size size2)
		{
			return new Size (Math.Min (size1.Width, size2.Width), Math.Min (size1.Height, size2.Height));
		}
		
		/// <summary>
		/// Returns the maximum width and height of two sizes
		/// </summary>
		/// <param name="size1">First size to get the maximum values</param>
		/// <param name="size2">Second size to get the maximum values</param>
		/// <returns>A new instance of a Size struct with the maximum width and height of either sizes</returns>
		public static Size Max (Size size1, Size size2)
		{
			return new Size (Math.Max (size1.Width, size2.Width), Math.Max (size1.Height, size2.Height));
		}
		
		/// <summary>
		/// Returns the absolute width and height of the specified <paramref name="size"/>
		/// </summary>
		/// <param name="size">Size to convert</param>
		/// <returns>A new instance of a Size struct with absolute (positive) width and height</returns>
		public static Size Abs (Size size)
		{
			return new Size (Math.Abs (size.Width), Math.Abs (size.Height));
		}

		/// <summary>
		/// Converts a floating point <paramref name="size"/> to an integral size by rounding the width and height to the 
		/// next integral value.
		/// </summary>
		/// <remarks>
		/// This is useful to get a size struct that includes the floating point values completely.  As opposed to the 
		/// <see cref="Round"/>, which will round down to the nearest integral number.
		/// 
		/// For example, a Width or Height of 2.1 or 2.6 would be translated to 3.
		/// </remarks>
		/// <param name="size">Size.</param>
		public static Size Ceiling (SizeF size)
		{
			return new Size((int)Math.Ceiling(size.Width), (int)Math.Ceiling(size.Height)); 
		}
		
		/// <summary>
		/// Size with width and height with a maximum int value
		/// </summary>
		public static readonly Size MaxValue = new Size (Int32.MaxValue, Int32.MaxValue);
		
		/// <summary>
		/// Size with width and height with a minimum int value
		/// </summary>
		public static readonly Size MinValue = new Size (Int32.MinValue, Int32.MinValue);
		
		/// <summary>
		/// Initializes a new Size class with the specified width and height
		/// </summary>
		/// <param name="width">Initial width of the size</param>
		/// <param name="height">Initial height of the size</param>
		public Size (int width, int height)
		: this()
		{
			Width = width;
			Height = height;
		}
		
		/// <summary>
		/// Initializes a new Size class with width and height corresponding to the <see cref="Point.X"/> and <see cref="Point.Y"/> of the specified <paramref name="point"/>
		/// </summary>
		/// <param name="point">Point to convert to a Size struct</param>
		public Size (Point point)
		: this(point.X, point.Y)
		{
		}
		
		/// <summary>
		/// Initializes a new Size with the truncated width and height of size.
		/// </summary>
		/// <param name="size"></param>
		public Size(SizeF size)
			: this((int)size.Width, (int)size.Height)
		{
		}

		/// <summary>
		/// Fits this size to the specified <paramref name="constraint"/>, keeping the aspect
		/// </summary>
		/// <returns>The new size with the same aspect ratio with the width/height equal or within the constraint</returns>
		/// <param name="constraint">Constraint to fit the new size into</param>
		public Size FitTo (Size constraint)
		{
			double ratioX = (double) constraint.Width / (double)Width;
			double ratioY = (double) constraint.Height / (double)Height;
			// use whichever multiplier is smaller
			double ratio = ratioX < ratioY ? ratioX : ratioY;
			return new Size((int)(Width * ratio), (int)(Height * ratio));
		}

		/// <summary>
		/// Gets a value indicating that the specified <paramref name="point"/> is within the <see cref="Width"/> and <see cref="Height"/> of this size
		/// </summary>
		/// <param name="point">Point to test</param>
		/// <returns>True if the <paramref name="point"/> has an X and Y value between 0 and the Width and Height of this size, respectively. False otherwise</returns>
		public bool Contains (Point point)
		{
			return Contains (point.X, point.Y);
		}
		
		/// <summary>
		/// Gets a value indicating that the specified <paramref name="x"/> and <paramref name="y"/> values are within the <see cref="Width"/> and <see cref="Height"/> of this size
		/// </summary>
		/// <param name="x">X value to test</param>
		/// <param name="y">Y value to test</param>
		/// <returns>True if the <paramref name="x"/> and <paramref name="y"/> values are greater than or equal to 0 and less than the Width and Height of this size, respectively. False otherwise</returns>
		public bool Contains (int x, int y)
		{
			if (Width == 0 || Height == 0)
				return false;
			return (x >= 0 && x < Width && y >= 0 && y < Height);
		}
		
		/// <summary>
		/// Gets a value indicating that both the <see cref="Width"/> and <see cref="Height"/> are zero
		/// </summary>
		public bool IsZero
		{
			get { return Width == 0 && Height == 0; }
		}
		
		/// <summary>
		/// Gets a value indicating that either the <see cref="Width"/> or <see cref="Height"/> are zero
		/// </summary>
		public bool IsEmpty
		{
			get { return Width == 0 || Height == 0; }
		}
		
		/// <summary>
		/// Negates the Width and Height of the specified <paramref name="size"/> value
		/// </summary>		
		/// <param name="size">Size to negate</param>
		/// <returns>A new size that has a negative value of each of the Width and Height</returns>
		public static Size operator - (Size size)
		{
			return new Size (-size.Width, -size.Height);
		}
		
		/// <summary>
		/// Multiplies the <see cref="Width"/> and <see cref="Height"/> of two sizes
		/// </summary>
		/// <param name="size1">First size to multiply</param>
		/// <param name="size2">Second size to multiply</param>
		/// <returns>A new instance of a Size struct with the product of both sizes</returns>
		public static Size operator * (Size size1, Size size2)
		{
			return new Size (size1.Width * size2.Width, size1.Height * size2.Height);
		}
		
		/// <summary>
		/// Multiplies the <see cref="Width"/> and <see cref="Height"/> of a <paramref name="size"/> by the specified <paramref name="factor"/>
		/// </summary>
		/// <param name="size">Size to multiply</param>
		/// <param name="factor">Factor to multiply both the Width and Height by</param>
		/// <returns>A new instance of a Size struct with the product of the <paramref name="size"/> and <paramref name="factor"/></returns>
		public static Size operator * (Size size, int factor)
		{
			return new Size (size.Width * factor, size.Height * factor);
		}
		
		/// <summary>
		/// Multiplies the <see cref="Width"/> and <see cref="Height"/> of a <paramref name="size"/> by the specified <paramref name="factor"/>
		/// </summary>
		/// <param name="size">Size to multiply</param>
		/// <param name="factor">Factor to multiply both the Width and Height by</param>
		/// <returns>A new instance of a Size struct with the product of the <paramref name="size"/> and <paramref name="factor"/></returns>
		public static Size operator * (int factor, Size size)
		{
			return new Size (size.Width * factor, size.Height * factor);
		}
		
		/// <summary>
		/// Multiplies the <see cref="Width"/> and <see cref="Height"/> of a <paramref name="size"/> by the specified floating point <paramref name="factor"/>
		/// </summary>
		/// <param name="size">Size to multiply</param>
		/// <param name="factor">Factor to multiply both the Width and Height by</param>
		/// <returns>A new instance of a SizeF struct with the product of the <paramref name="size"/> and <paramref name="factor"/></returns>
		public static SizeF operator * (Size size, float factor)
		{
			return new SizeF (size.Width * factor, size.Height * factor);
		}
		
		/// <summary>
		/// Divides the <see cref="Width"/> and <see cref="Height"/> of two sizes
		/// </summary>
		/// <param name="size1">Size to divide</param>
		/// <param name="size2">Size to divide by</param>
		/// <returns>A new instance of a Size struct with the division of <paramref name="size1"/> by <paramref name="size2"/></returns>
		public static Size operator / (Size size1, Size size2)
		{
			return new Size (size1.Width / size2.Width, size1.Height / size2.Height);
		}
		
		/// <summary>
		/// Divides the <see cref="Width"/> and <see cref="Height"/> of a <paramref name="size"/> by the specified <paramref name="factor"/>
		/// </summary>
		/// <param name="size">Size to divide</param>
		/// <param name="factor">Factor to divide both the Width and Height by</param>
		/// <returns>A new instance of a Size struct with the width and height of <paramref name="size"/> divided by <paramref name="factor"/></returns>
		public static Size operator / (Size size, int factor)
		{
			return new Size (size.Width / factor, size.Height / factor);
		}
		
		/// <summary>
		/// Adds the <see cref="Width"/> and <see cref="Height"/> values of two sizes together
		/// </summary>
		/// <param name="size1">First size to add</param>
		/// <param name="size2">Second size to add</param>
		/// <returns>A new instance of a Size struct with the addition of the width and height of both sizes</returns>
		public static Size operator + (Size size1, Size size2)
		{
			return new Size (size1.Width + size2.Width, size1.Height + size2.Height);
		}
		
		/// <summary>
		/// Subtracts the <see cref="Width"/> and <see cref="Height"/> value of one size from another
		/// </summary>
		/// <param name="size1">Size to subtract from</param>
		/// <param name="size2">Size to subtract</param>
		/// <returns>A new instance of a Size struct with the width and height of <paramref name="size1"/> minus <paramref name="size2"/></returns>
		public static Size operator - (Size size1, Size size2)
		{
			return new Size (size1.Width - size2.Width, size1.Height - size2.Height);
		}
		
		/// <summary>
		/// Adds the <see cref="Point.X"/> and <see cref="Point.Y"/> value to the <see cref="Width"/> and <see cref="Height"/> of a size
		/// </summary>
		/// <param name="size">Size to add to</param>
		/// <param name="point">Point with values to add</param>
		/// <returns>A new instance of a Size struct with the width and height of <paramref name="size"/> plus <paramref name="point"/></returns>
		public static Size operator + (Size size, Point point)
		{
			return new Size (size.Width + point.X, size.Height + point.Y);
		}
		
		/// <summary>
		/// Subtracts the <see cref="Point.X"/> and <see cref="Point.Y"/> value from the <see cref="Width"/> and <see cref="Height"/> of a size
		/// </summary>
		/// <param name="size">Size to subtract from</param>
		/// <param name="point">Point with values to subtract</param>
		/// <returns>A new instance of a Size struct with the width and height of <paramref name="size"/> minus <paramref name="point"/></returns>
		public static Size operator - (Size size, Point point)
		{
			return new Size (size.Width - point.X, size.Height - point.Y);
		}
		
		/// <summary>
		/// Subtracts a <paramref name="value"/> from the <see cref="Width"/> and <see cref="Height"/> of the specified <paramref name="size"/>
		/// </summary>
		/// <param name="size">Size to subtract from</param>
		/// <param name="value">Value to subtract from the width and height</param>
		/// <returns>A new instance of a Size struct with the width and height of <paramref name="size"/> minus <paramref name="value"/></returns>
		public static Size operator - (Size size, int value)
		{
			return new Size (size.Width - value, size.Height - value);
		}
		
		/// <summary>
		/// Adds a <paramref name="value"/> to the <see cref="Width"/> and <see cref="Height"/> of the specified <paramref name="size"/>
		/// </summary>
		/// <param name="size">Size to add to</param>
		/// <param name="value">Value to add to the width and height</param>
		/// <returns>A new instance of a Size struct with the width and height of <paramref name="size"/> plus <paramref name="value"/></returns>
		public static Size operator + (Size size, int value)
		{
			return new Size (size.Width + value, size.Height + value);
		}
		
		/// <summary>
		/// Compares two sizes for equality
		/// </summary>
		/// <param name="size1">First size to compare</param>
		/// <param name="size2">Second size to compare</param>
		/// <returns>True if both the width and height of both sizes are equal, false otherwise</returns>
		public static bool operator == (Size size1, Size size2)
		{
			return (size1.Width == size2.Width && size1.Height == size2.Height);
		}
		
		/// <summary>
		/// Compares two sizes for inequality
		/// </summary>
		/// <param name="size1">First size to compare</param>
		/// <param name="size2">Second size to compare</param>
		/// <returns>True if either the width and height of both sizes are not equal, false if they are both equal</returns>
		public static bool operator != (Size size1, Size size2)
		{
			return (size1.Width != size2.Width || size1.Height != size2.Height);
		}
		
		/// <summary>
		/// Explicit conversion from a <see cref="SizeF"/> to a <see cref="Size"/> by truncating values
		/// </summary>
		/// <param name="size">Size to convert</param>
		/// <returns>A new instance of a Size with the value of the specified <paramref name="size"/></returns>
		public static explicit operator Size (SizeF size)
		{
			return new Size ((int)size.Width, (int)size.Height);
		}
		
		/// <summary>
		/// Explicit conversion from a <paramref name="point"/> to a Size with a Width and Height of the X and Y values of the point, respectively
		/// </summary>
		/// <param name="point">Point to convert</param>
		/// <returns>A new size with the width and height of the X and Y values of the point, respectively</returns>
		public static explicit operator Size (Point point)
		{
			return new Size (point);
		}
		
		/// <summary>
		/// Compares this size to the specified <paramref name="obj"/>
		/// </summary>
		/// <param name="obj">Object to compare with</param>
		/// <returns>True if the specified <paramref name="obj"/> is a Size and is equal to this instance</returns>
		public override bool Equals (object obj)
		{
			return obj is Size && (Size)obj == this;
		}
		
		/// <summary>
		/// Gets the hash code for this Size
		/// </summary>
		/// <returns>Hash code value for this size</returns>
		public override int GetHashCode ()
		{
			return Width ^ Height;
		}
		
		/// <summary>
		/// Converts this Size struct to a string
		/// </summary>
		/// <returns>String representation of this Size</returns>
		public override string ToString ()
		{
			return String.Format (CultureInfo.InvariantCulture, "{0},{1}", Width, Height);
		}
		
		/// <summary>
		/// Compares this size to the <paramref name="other"/> size
		/// </summary>
		/// <param name="other">Other size to compare with</param>
		/// <returns>True if the <paramref name="other"/> size is equal to this instance</returns>
		public bool Equals (Size other)
		{
			return other == this;
		}
	}
}
