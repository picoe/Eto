using System;
using System.Globalization;

namespace Eto.Forms
{
	/// <summary>
	/// A range defined by a start index and a length.
	/// Start and End are inclusive, so End = Start + Length -1.
	/// </summary>
	public struct Range : IEquatable<Range>
	{
		public int Start { get; set; }

		public int Length { get; set; }

		public bool IsEmpty { get { return Length == 0; } }

		public Range (int start, int length)
			: this ()
		{
			this.Start = start;
			this.Length = length;
		}

		public static Range FromStartEnd(int start, int end)
		{
			return new Range(start, end - start + 1);
		}

		public int End {
			get { return Start + Length - 1; }
			set {
				Length = value - Start + 1;
			}
		}

		public bool Contains(int value)
		{
			return Start <= value && value <= End;
		}

		public Range Intersect(Range range)
		{
			if (range != null &&
				Start != null &&
				End != null &&
				range.Start != null &&
				range.End != null)
			{
				var start = Start >= range.Start ? Start : range.Start;
				var end = End <= range.End ? End : range.End;
				if (start <= end)
					return Range.FromStartEnd(start, end);
			}
			return default(Range);
		}

		public override bool Equals (object obj)
		{
			return obj is Range && (Range)obj == this;
		}

		public static bool operator == (Range value1, Range value2)
		{
			return (value1.Start == value2.Start) && (value1.Length == value2.Length);
		}

		public static bool operator != (Range value1, Range value2)
		{
			return (value1.Start != value2.Start) || (value1.Length != value2.Length);
		}

		public override int GetHashCode ()
		{
			return Start ^ Length;
		}

		public override string ToString ()
		{
			return string.Format (CultureInfo.InvariantCulture, "Start={0}, Length={1}", Start, Length);
		}

		public bool Equals (Range other)
		{
			return other == this;
		}
	}
}

