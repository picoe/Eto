using System;
using sc = System.ComponentModel;
using System.Globalization;

namespace Eto.Drawing
{
	/// <summary>
	/// Represents an amount of padding to apply to an object at the top, left, right, and bottom.
	/// </summary>
	[sc.TypeConverter(typeof(PaddingFConverterInternal))]
	public struct PaddingF : IEquatable<PaddingF>
	{
		float top, left, right, bottom;
		/// <summary>
		/// Gets or sets the padding on the top
		/// </summary>
		public float Top
		{
			get => top;
			set => top = value;
		}

		/// <summary>
		/// Gets or sets the padding on the left
		/// </summary>
		public float Left
		{
			get => left;
			set => left = value;
		}

		/// <summary>
		/// Gets or sets the padding on the right
		/// </summary>
		public float Right
		{
			get => right;
			set => right = value;
		}

		/// <summary>
		/// Gets or sets the padding on the bottom
		/// </summary>
		public float Bottom
		{
			get => bottom;
			set => bottom = value;
		}

		/// <summary>
		/// Gets an empty padding with zero for each side
		/// </summary>
		public static readonly PaddingF Empty = new PaddingF(0f);

		/// <summary>
		/// Initializes a new instance of the Padding class with the specified padding for all sides
		/// </summary>
		/// <param name="all">Amount of padding to apply to each side</param>
		public PaddingF(float all)
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
		public PaddingF(float horizontal, float vertical)
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
		public PaddingF(float left, float top, float right, float bottom)
		{
			this.left = left;
			this.top = top;
			this.right = right;
			this.bottom = bottom;
		}

		/// <summary>
		/// Gets the total horizontal padding, which is the sum of <see cref="Left"/> and <see cref="Right"/>.
		/// </summary>
		public float Horizontal
		{
			get { return Left + Right; }
		}

		/// <summary>
		/// Gets the total vertical padding, which is the sum of <see cref="Top"/> and <see cref="Bottom"/>
		/// </summary>
		public float Vertical
		{
			get { return Top + Bottom; }
		}

		/// <summary>
		/// Gets the padding as a size value with the <see cref="Horizontal"/> and <see cref="Vertical"/> values as 
		/// the <see cref="Eto.Drawing.Size.Width"/> and <see cref="Eto.Drawing.Size.Height"/>, respectively.
		/// </summary>
		public SizeF Size
		{
			get { return new SizeF(Horizontal, Vertical); }
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
		public static PaddingF operator +(PaddingF value1, PaddingF value2)
		{
			return new PaddingF(value1.Left + value2.Left, value1.Top + value2.Top, value1.Right + value2.Right, value1.Bottom + value2.Bottom);
		}

		/// <summary>
		/// Subtracts a padding value from another value
		/// </summary>
		/// <param name="value1">Padding value to subtract from</param>
		/// <param name="value2">Padding value to subtract from the first value</param>
		/// <returns>The value of the first padding minus the second padding value</returns>
		public static PaddingF operator -(PaddingF value1, PaddingF value2)
		{
			return new PaddingF(value1.Left - value2.Left, value1.Top - value2.Top, value1.Right - value2.Right, value1.Bottom - value2.Bottom);
		}

		/// <summary>
		/// Determines the equality of two padding objects
		/// </summary>
		/// <param name="value1">First padding value to compare</param>
		/// <param name="value2">Second padding value to compare</param>
		/// <returns>True if the two padding values are equal, false otherwise</returns>
		public static bool operator ==(PaddingF value1, PaddingF value2)
		{
			return value1.Top == value2.Top && value1.Bottom == value2.Bottom && value1.Left == value2.Left && value1.Right == value2.Right;
		}

		/// <summary>
		/// Determines the inequality of two padding objects
		/// </summary>
		/// <param name="value1">First padding value to compare</param>
		/// <param name="value2">Second padding value to compare</param>
		/// <returns>True if the values are not equal, false if they are equal</returns>
		public static bool operator !=(PaddingF value1, PaddingF value2)
		{
			return !(value1 == value2);
		}

		/// <summary>
		/// Implicitly converts a single integer to a padding with all sides of equal value.
		/// </summary>
		/// <param name="all">Value for padding on all sides</param>
		public static implicit operator PaddingF(float all)
		{
			return new PaddingF(all);
		}

		/// <summary>
		/// Determines the equality between this instance and the specified object
		/// </summary>
		/// <param name="obj">Object to compare to</param>
		/// <returns>True if obj is a Padding object and is equal to this instance, false if not</returns>
		public override bool Equals(object obj)
		{
			return obj is PaddingF && (PaddingF)obj == this;
		}

		/// <summary>
		/// Gets the hash code for this Padding instance
		/// </summary>
		/// <returns>Hash code for this instance</returns>
		public override int GetHashCode()
		{
			return Top.GetHashCode() ^ Left.GetHashCode() ^ Right.GetHashCode() ^ Bottom.GetHashCode();
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
		public bool Equals(PaddingF other)
		{
			return other == this;
		}

		/// <summary>
		/// Converts an integral padding to floading precision padding.
		/// </summary>
		/// <param name="padding">Padding to convert</param>
		public static implicit operator PaddingF(Padding padding)
		{
			return new PaddingF(padding.Left, padding.Top, padding.Right, padding.Bottom);
		}

		/// <summary>
		/// Returns a new Padding with negative left, top, right, and bottom values
		/// </summary>
		/// <param name="padding">Padding to negate</param>
		/// <returns>A new padding with negative values</returns>
		public static PaddingF operator -(PaddingF padding)
		{
			return new PaddingF(-padding.Left, -padding.Top, -padding.Right, -padding.Bottom);
		}

	}
}

