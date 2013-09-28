using System;
using System.Globalization;

namespace Eto.Forms
{
	public struct Range : IEquatable<Range>
	{
		[Obsolete("Use Start instead")]
		public int Location {
			get { return Start; }
			set { Start = value; }
		}

		public int Start { get; set; }

		public int Length { get; set; }

		public Range (int start, int length)
			: this ()
		{
			this.Start = start;
			this.Length = length;
		}

		public int End {
			get { return Start + Length - 1; }
			set {
				Length = value - Start + 1;
			}
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

