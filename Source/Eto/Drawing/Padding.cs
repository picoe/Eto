using System;
using System.ComponentModel;

namespace Eto.Drawing
{
	[TypeConverter(typeof(PaddingConverter))]
	public struct Padding : IEquatable<Padding>
	{
		public int Top { get; set; }
		public int Left { get; set; }
		public int Right { get; set; }
		public int Bottom { get; set; }
		
		public static readonly Padding Empty = new Padding(0);
		
		public Padding (int all)
			: this()
		{
			this.Left = all;
			this.Top = all;
			this.Right = all;
			this.Bottom = all;
		}

		public Padding (int horizontal, int vertical)
			: this()
		{
			this.Left = horizontal;
			this.Top = vertical;
			this.Right = horizontal;
			this.Bottom = vertical;
		}
		
		public Padding (int left, int top, int right, int bottom)
			: this()
		{
			this.Left = left;
			this.Top = top;
			this.Right = right;
			this.Bottom = bottom;
		}
		
		public int Horizontal
		{
			get { return Left + Right; }
		}
		
		public int Vertical
		{
			get { return Top + Bottom; }
		}
		
		public Size Size
		{
			get { return new Size(Horizontal, Vertical); }
		}

		public static bool operator == (Padding value1, Padding value2)
		{
			return value1.Top == value2.Top && value1.Bottom == value2.Bottom && value1.Left == value2.Left && value1.Right == value2.Right;
		}

		public static bool operator != (Padding value1, Padding value2)
		{
			return !(value1 == value2);
		}
		
		public override bool Equals (object obj)
		{
			return obj is Padding && (Padding)obj == this;
		}
		
		public override int GetHashCode ()
		{
			return Top ^ Left ^ Right ^ Bottom;
		}
		
		public override string ToString ()
		{
			return string.Format ("[Padding: Top={0}, Left={1}, Right={2}, Bottom={3}]", Top, Left, Right, Bottom);
		}

		public bool Equals (Padding other)
		{
			return other == this;
		}
	}
}

