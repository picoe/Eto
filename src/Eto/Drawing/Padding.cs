using System;
using sc = System.ComponentModel;
using System.Globalization;

namespace Eto.Drawing
{
	/// <summary>
	/// Represents an amount of padding to apply to an object at the top, left, right, and bottom.
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[sc.TypeConverter(typeof(PaddingConverterInternal))]
	public struct Padding : IEquatable<Padding>
	{
		int top, left, right, bottom;
		/// <summary>
		/// Gets or sets the padding on the top
		/// </summary>
		public int Top
		{
			get => top;
			set => top = value;
		}

		/// <summary>
		/// Gets or sets the padding on the left
		/// </summary>
		public int Left
		{
			get => left;
			set => left = value;
		}

		/// <summary>
		/// Gets or sets the padding on the right
		/// </summary>
		public int Right
		{
			get => right;
			set => right = value;
		}

		/// <summary>
		/// Gets or sets the padding on the bottom
		/// </summary>
		public int Bottom
		{
			get => bottom;
			set => bottom = value;
		}

		/// <summary>
		/// Gets an empty padding with zero for each side
		/// </summary>
		public static readonly Padding Empty = new Padding(0);

		/// <summary>
		/// Initializes a new instance of the Padding class with the specified padding for all sides
		/// </summary>
		/// <param name="all">Amount of padding to apply to each side</param>
		public Padding(int all)
		{
			this.left = all;
			this.top = all;
			this.right = all;
			this.bottom = all;
		}

		/// <summary>
		/// Initializes a new instance of the Padding class with the specified padding for horizontal and vertical sides
		/// </summary>
		/// <param name="horizontal">Amount of padding to set the <see cref="Left"/> and <see cref="Right"/> sides</param>
		/// <param name="vertical">Amount of padding to set the <see cref="Top"/> and <see cref="Bottom"/> sides</param>
		public Padding(int horizontal, int vertical)
		{
			this.left = horizontal;
			this.top = vertical;
			this.right = horizontal;
			this.bottom = vertical;
		}

		/// <summary>
		/// Initializes a new instance of the Padding class
		/// </summary>
		/// <param name="left">Amount of padding to apply to the left</param>
		/// <param name="top">Amount of padding to apply to the top</param>
		/// <param name="right">Amount of padding to apply to the right</param>
		/// <param name="bottom">Amount of padding to apply to the bottom</param>
		public Padding(int left, int top, int right, int bottom)
		{
			this.left = left;
			this.top = top;
			this.right = right;
			this.bottom = bottom;
		}

		/// <summary>
		/// Converts a floating point <paramref name="padding"/> to an integral padding by rounding each of its components
		/// </summary>
		/// <param name="padding">Padding to convert</param>
		/// <returns>A new instance of a Padding class with rounded components of the specified <paramref name="padding"/></returns>
		public static Padding Round(PaddingF padding)
		{
			return new Padding((int)Math.Round(padding.Left), (int)Math.Round(padding.Top), (int)Math.Round(padding.Right), (int)Math.Round(padding.Bottom));
		}

		/// <summary>
		/// Converts a floating point <paramref name="padding"/> to an integral padding by truncating each of its components
		/// </summary>
		/// <param name="padding">Padding to convert</param>
		/// <returns>A new instance of a Padding class with truncated components of the specified <paramref name="padding"/></returns>
		public static Padding Truncate(PaddingF padding)
		{
			return new Padding((int)padding.Left, (int)padding.Top, (int)padding.Right, (int)padding.Bottom);
		}

		/// <summary>
		/// Converts a floating point <paramref name="padding"/> to an integral padding by taking the ceiling of its components
		/// </summary>
		/// <param name="padding">Padding to convert</param>
		/// <returns>A new instance of a Padding class with the ceiling of the specified <paramref name="padding"/></returns>
		public static Padding Ceiling(PaddingF padding)
		{
			return new Padding((int)Math.Ceiling(padding.Left), (int)Math.Ceiling(padding.Top), (int)Math.Ceiling(padding.Right), (int)Math.Ceiling(padding.Bottom));
		}

		/// <summary>
		/// Gets the total horizontal padding, which is the sum of <see cref="Left"/> and <see cref="Right"/>.
		/// </summary>
		public int Horizontal
		{
			get { return Left + Right; }
		}

		/// <summary>
		/// Gets the total vertical padding, which is the sum of <see cref="Top"/> and <see cref="Bottom"/>
		/// </summary>
		public int Vertical
		{
			get { return Top + Bottom; }
		}

		/// <summary>
		/// Gets the padding as a size value with the <see cref="Horizontal"/> and <see cref="Vertical"/> values as 
		/// the <see cref="Eto.Drawing.Size.Width"/> and <see cref="Eto.Drawing.Size.Height"/>, respectively.
		/// </summary>
		public Size Size
		{
			get { return new Size(Horizontal, Vertical); }
		}

		/// <summary>
		/// Gets a value indicating that all sides of the padding are zero
		/// </summary>
		public bool IsZero
		{
			get { return Top == 0 && Left == 0 && Bottom == 0 && Right == 0; }
		}

		/// <summary>
		/// Adds two padding values together
		/// </summary>
		/// <param name="value1">First padding value to add</param>
		/// <param name="value2">Second padding value to add</param>
		/// <returns>The sum of both padding values</returns>
		public static Padding operator +(Padding value1, Padding value2)
		{
			return new Padding(value1.Left + value2.Left, value1.Top + value2.Top, value1.Right + value2.Right, value1.Bottom + value2.Bottom);
		}

		/// <summary>
		/// Subtracts a padding value from another value
		/// </summary>
		/// <param name="value1">Padding value to subtract from</param>
		/// <param name="value2">Padding value to subtract from the first value</param>
		/// <returns>The value of the first padding minus the second padding value</returns>
		public static Padding operator -(Padding value1, Padding value2)
		{
			return new Padding(value1.Left - value2.Left, value1.Top - value2.Top, value1.Right - value2.Right, value1.Bottom - value2.Bottom);
		}

		/// <summary>
		/// Determines the equality of two padding objects
		/// </summary>
		/// <param name="value1">First padding value to compare</param>
		/// <param name="value2">Second padding value to compare</param>
		/// <returns>True if the two padding values are equal, false otherwise</returns>
		public static bool operator ==(Padding value1, Padding value2)
		{
			return value1.Top == value2.Top && value1.Bottom == value2.Bottom && value1.Left == value2.Left && value1.Right == value2.Right;
		}

		/// <summary>
		/// Determines the inequality of two padding objects
		/// </summary>
		/// <param name="value1">First padding value to compare</param>
		/// <param name="value2">Second padding value to compare</param>
		/// <returns>True if the values are not equal, false if they are equal</returns>
		public static bool operator !=(Padding value1, Padding value2)
		{
			return !(value1 == value2);
		}

		/// <summary>
		/// Implicitly converts a single integer to a padding with all sides of equal value.
		/// </summary>
		/// <param name="all">Value for padding on all sides</param>
		public static implicit operator Padding(int all)
		{
			return new Padding(all);
		}

		/// <summary>
		/// Determines the equality between this instance and the specified object
		/// </summary>
		/// <param name="obj">Object to compare to</param>
		/// <returns>True if obj is a Padding object and is equal to this instance, false if not</returns>
		public override bool Equals(object obj)
		{
			return obj is Padding && (Padding)obj == this;
		}

		/// <summary>
		/// Gets the hash code for this Padding instance
		/// </summary>
		/// <returns>Hash code for this instance</returns>
		public override int GetHashCode()
		{
			return Top ^ Left ^ Right ^ Bottom;
		}

		/// <summary>
		/// Converts this object to a string
		/// </summary>
		/// <returns>String representation of this object</returns>
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Top={0}, Left={1}, Right={2}, Bottom={3}", Top, Left, Right, Bottom);
		}

		/// <summary>
		/// Determines equality between this instance and the specified padding
		/// </summary>
		/// <param name="other">Other padding instance to compare with</param>
		/// <returns>True if the specified padding is equal to this instance, false if not</returns>
		public bool Equals(Padding other)
		{
			return other == this;
		}

		/// <summary>
		/// Converts a floating precision padding to integral values by truncating them.
		/// </summary>
		/// <param name="padding">Padding to truncate</param>
		public static explicit operator Padding(PaddingF padding)
		{
			return Padding.Truncate(padding);
		}

		/// <summary>
		/// Returns a new Padding with negative left, top, right, and bottom values
		/// </summary>
		/// <param name="padding">Padding to negate</param>
		/// <returns>A new padding with negative values</returns>
		public static Padding operator -(Padding padding)
		{
			return new Padding(-padding.Left, -padding.Top, -padding.Right, -padding.Bottom);
		}
	}
}

