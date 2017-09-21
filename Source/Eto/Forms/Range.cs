using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Eto.Forms
{
	/// <summary>
	/// Represents an immutable, inclusive start/end range of <see cref="IComparable{T}"/> values
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public struct Range<T> : IEquatable<Range<T>>
		where T : struct, IComparable<T>
	{
		T start;
		T end;

		/// <summary>
		/// Gets the start value of the range
		/// </summary>
		/// <value>The start of the range.</value>
		public T Start { get { return start; } }

		/// <summary>
		/// Gets the end value of the range.
		/// </summary>
		/// <value>The end of the range.</value>
		public T End { get { return end; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Range{T}"/> struct with a value for both the start and end.
		/// </summary>
		/// <param name="value">Value for the start and end of the range.</param>
		public Range(T value)
		{
			start = value;
			end = value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Range{T}"/> struct.
		/// </summary>
		/// <param name="start">Start of the range (inclusive).</param>
		/// <param name="end">End of the range (inclusive).</param>
		public Range(T start, T end)
		{
			this.start = start;
			this.end = end;
		}

		/// <summary>
		/// Creates a copy of the current range with a different start value.
		/// </summary>
		/// <returns>A new instance of the range with the specified start value.</returns>
		/// <param name="start">Start of the new range.</param>
		public Range<T> WithStart(T start)
		{
			return new Range<T>(start, End);
		}

		/// <summary>
		/// Creates a copy of the current range with a different end value.
		/// </summary>
		/// <returns>A new instance of the range with the specified end value.</returns>
		/// <param name="end">End of the new range.</param>
		public Range<T> WithEnd(T end)
		{
			return new Range<T>(Start, end);
		}

		/// <summary>
		/// Determines if the specified <paramref name="value"/> is between or equal to the <see cref="Start"/> and <see cref="End"/> of this range.
		/// </summary>
		/// <param name="value">Value to check if it is within this range.</param>
		public bool Contains(T value)
		{
			var comparer = Comparer<T>.Default;
			return comparer.Compare(Start, value) <= 0
			&& comparer.Compare(End, value) >= 0;
		}

		/// <summary>
		/// Iterates the range between the start and end values.
		/// </summary>
		/// <remarks>
		/// This can be used to return an enumerable that iterates between the start and end of the range given the
		/// specified <paramref name="increment"/> function.
		/// </remarks>
		/// <example>
		/// <code>
		/// // iterate over an int range
		/// var intRange = new Range&lt;int&gt;(1, 200);
		/// foreach (var item in intRange.Iterate(i => i + 1))
		/// {
		/// 	// logic
		/// }
		/// 
		/// // iterate over a date range by minute
		/// var dateRange = new Range&lt;DateTime&gt;(DateTime.Today, DateTime.Today.AddDays(2));
		/// foreach (var item in dateRange.Iterate(i => i.AddMinutes(1)))
		/// {
		/// 	// logic
		/// }
		/// </code>
		/// </example>
		/// <param name="increment">Delegate to increment the value for each iteration of the enumerable.</param>
		public IEnumerable<T> Iterate(Func<T, T> increment)
		{
			T item = start;
			var comparer = Comparer<T>.Default;
			while (comparer.Compare(item, end) <= 0)
			{
				yield return item;
				item = increment(item);
			}
		}

		/// <summary>
		/// Determines if the specified <paramref name="range"/> touches (but doesn't intersect) this instance.
		/// </summary>
		/// <remarks>
		/// This can be used to determine if one range comes after or before another range, given the specified
		/// <paramref name="increment"/> function.
		/// The increment function is used as this class does not assume how to increment each value, e.g. for a
		/// <see cref="DateTime"/> value, you can increment by day, minute, second, etc.
		/// </remarks>
		/// <param name="range">Range to check if it touches this range.</param>
		/// <param name="increment">Delegate to increment the value for checking if the ranges touch.</param>
		/// <returns><c>true</c> if the ranges touch, <c>false</c> otherwise.</returns>
		public bool Touches(Range<T> range, Func<T, T> increment)
		{
			var comparer = Comparer<T>.Default;
			return comparer.Compare(Start, increment(range.End)) == 0
			|| comparer.Compare(increment(End), range.Start) == 0;
		}

		/// <summary>
		/// Determines if the specified <paramref name="range"/> intersects (overlaps) this instance.
		/// </summary>
		/// <param name="range">Range to check for intersection.</param>
		/// <returns><c>true</c> if the range intersects this instance, <c>false</c> otherwise.</returns>
		public bool Intersects(Range<T> range)
		{
			var comparer = Comparer<T>.Default;
			var startVal = comparer.Compare(Start, range.Start) >= 0 ? Start : range.Start;
			var endVal = comparer.Compare(End, range.End) <= 0 ? End : range.End;
			return comparer.Compare(startVal, endVal) <= 0;
		}

		/// <summary>
		/// Gets the intersection of this instance and the specified <paramref name="range"/>.
		/// </summary>
		/// <param name="range">Range to intersect with.</param>
		/// <returns>A new instance of a range that is the intersection of this instance and the specified range, or null if they do not intersect.</returns>
		public Range<T>? Intersect(Range<T> range)
		{
			var comparer = Comparer<T>.Default;
			var startVal = comparer.Compare(Start, range.Start) >= 0 ? Start : range.Start;
			var endVal = comparer.Compare(End, range.End) <= 0 ? End : range.End;
			return comparer.Compare(startVal, endVal) <= 0 ? (Range<T>?)new Range<T>(startVal, endVal) : null;
		}

		/// <summary>
		/// Gets the union of this instance and the specified <paramref name="range"/>, including touching ranges.
		/// </summary>
		/// <remarks>
		/// This is similar to <see cref="Union(Range{T})"/>, however this handles when the two ranges are touching.
		/// The <paramref name="increment"/> delegate is used to determine if the ranges are touching by incrementing the ends
		/// of the ranges and comparing that value to the start of the other range.
		/// </remarks>
		/// <param name="range">Range to union with.</param>
		/// <param name="increment">Delegate to increment the value for checking if the ranges touch.</param>
		/// <returns>The union of this instance and the specified range, or null if they are neither intersecting or touching.</returns>
		public Range<T>? Union(Range<T> range, Func<T, T> increment)
		{
			if (Intersects(range) || Touches(range, increment))
			{
				var comparer = Comparer<T>.Default;
				var startVal = comparer.Compare(Start, range.Start) <= 0 ? Start : range.Start;
				var endVal = comparer.Compare(End, range.End) >= 0 ? End : range.End;
				return comparer.Compare(startVal, endVal) <= 0 ? (Range<T>?)new Range<T>(startVal, endVal) : null;
			}
			return null;
		}

		/// <summary>
		/// Gets the union of this instance and an intersecting <paramref name="range"/>.
		/// </summary>
		/// <remarks>
		/// This is similar to <see cref="Union(Range{T},Func{T,T})"/>, however this only handles when the two ranges are intersecting.
		/// To union two ranges that touch, use the <see cref="Union(Range{T},Func{T,T})"/> method instead.
		/// </remarks>
		/// <param name="range">Range to union with.</param>
		/// <returns>The union of this instance and the specified range, or null if they are not intersecting.</returns>
		public Range<T>? Union(Range<T> range)
		{
			if (Intersects(range))
			{
				var comparer = Comparer<T>.Default;
				var startVal = comparer.Compare(Start, range.Start) <= 0 ? Start : range.Start;
				var endVal = comparer.Compare(End, range.End) >= 0 ? End : range.End;
				return comparer.Compare(startVal, endVal) <= 0 ? (Range<T>?)new Range<T>(startVal, endVal) : null;
			}
			return null;
		}

		/// <summary>
		/// Operator to compare two ranges for inequality.
		/// </summary>
		/// <param name="range1">First range to compare.</param>
		/// <param name="range2">Second range to compare.</param>
		/// <returns><c>true</c> if the two ranges are not equal, <c>false</c> if they are.</returns>
		public static bool operator !=(Range<T> range1, Range<T> range2)
		{
			return !(range1 == range2);
		}

		/// <summary>
		/// Operator to compare two ranges for equality
		/// </summary>
		/// <param name="range1">First range to compare.</param>
		/// <param name="range2">Second range to compare.</param>
		/// <returns><c>true</c> if the two ranges are equal, <c>false</c> if they are not.</returns>
		public static bool operator ==(Range<T> range1, Range<T> range2)
		{
			var comparer = Comparer<T>.Default;
			return comparer.Compare(range1.Start, range2.Start) == 0
			&& comparer.Compare(range1.End, range2.End) == 0;
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="Range{T}"/>.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="Range{T}"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current <see cref="Range{T}"/>; otherwise, <c>false</c>.</returns>
		public override bool Equals(object obj)
		{
			return obj is Range<T> && ((Range<T>)obj == this);
		}

		/// <summary>
		/// Serves as a hash function for a <see cref="Eto.Forms.Range{T}"/> object.
		/// </summary>
		/// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.</returns>
		public override int GetHashCode()
		{
			return Start.GetHashCode() ^ End.GetHashCode();
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="Range{T}"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="Range{T}"/>.</returns>
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Start={0}, End={1}", Start, End);
		}

		/// <summary>
		/// Determines whether the specified <paramref name="other"/> range is equal to the current <see cref="Range{T}"/>.
		/// </summary>
		/// <param name="other">The <see cref="Range{T}"/> to compare with the current <see cref="Range{T}"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="Range{T}"/> is equal to the current <see cref="Range{T}"/>; otherwise, <c>false</c>.</returns>
		public bool Equals(Range<T> other)
		{
			return this == other;
		}
	}

	/// <summary>
	/// Extensions for the <see cref="Range{T}"/> structure
	/// </summary>
	public static class RangeExtensions
	{
		/// <summary>
		/// Gets the interval for the specified <paramref name="range"/> between the start and end dates.
		/// </summary>
		/// <param name="range">Range to get the interval for.</param>
		/// <returns>A new TimeSpan that is the difference between the start and end dates of the specified range.</returns>
		public static TimeSpan Interval(this Range<DateTime> range)
		{
			return range.End - range.Start;
		}

		/// <summary>
		/// Gets the length of the specified <paramref name="range"/> between the start and end values.
		/// </summary>
		/// <param name="range">Range to get the length for.</param>
		/// <returns>The length between the start and end values of the specified range.</returns>
		public static int Length(this Range<int> range)
		{
			return range.End - range.Start + 1;
		}

		/// <summary>
		/// Creates a new range starting at the same position as the specified <paramref name="range"/> and a new length.
		/// </summary>
		/// <returns>The length for the new range.</returns>
		/// <param name="range">Range with the same start but different length of the specified range.</param>
		/// <param name="length">Length of the new range.</param>
		public static Range<int> WithLength(this Range<int> range, int length) => new Range<int>(range.Start, range.Start + length - 1);
	}

	/// <summary>
	/// Helpers for the <see cref="Range{T}"/> structure.
	/// </summary>
	public static class Range
	{
		/// <summary>
		/// Creates a new integer range with the specified start and length.
		/// </summary>
		/// <returns>A new range with the specified start and length.</returns>
		/// <param name="start">Start of the range.</param>
		/// <param name="length">Length of the range.</param>
		public static Range<int> FromLength(int start, int length) => new Range<int>(start, start + length - 1);

		/// <summary>
		/// Creates a new long range with the specified start and length.
		/// </summary>
		/// <returns>A new range with the specified start and length.</returns>
		/// <param name="start">Start of the range.</param>
		/// <param name="length">Length of the range.</param>
		public static Range<long> FromLength(long start, long length) => new Range<long>(start, start + length - 1);
	}
}