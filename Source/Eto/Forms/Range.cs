using System;

namespace Eto.Forms
{
	public struct Range : IEquatable<Range>
	{
		public int Location { get; set; }

		public int Length { get; set; }

		public Range (int location, int length)
			: this ()
		{
			this.Location = location;
			this.Length = length;
		}

		public override bool Equals (object obj)
		{
			return obj is Range && (Range)obj == this;
		}

		public static bool operator == (Range value1, Range value2)
		{
			return (value1.Location == value2.Location) && (value1.Length == value2.Length);
		}

		public static bool operator != (Range value1, Range value2)
		{
			return (value1.Location != value2.Location) || (value1.Length != value2.Length);
		}

		public override int GetHashCode ()
		{
			return Location ^ Length;
		}

		public override string ToString ()
		{
			return string.Format ("Location={0}, Length={1}", Location, Length);
		}

		public bool Equals (Range other)
		{
			return other == this;
		}
	}
}

