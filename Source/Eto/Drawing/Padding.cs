using System;

namespace Eto.Drawing
{
	public struct Padding
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
		
		public override bool Equals (object obj)
		{
			if (!(obj is Padding)) return false;
			var val = (Padding)obj;
			return val.Top == Top && val.Bottom == Bottom && val.Left == Left && val.Right == Right;
		}
		
		public override int GetHashCode ()
		{
			return Top ^ Left ^ Right ^ Bottom;
		}
		
		public override string ToString ()
		{
			return string.Format ("[Padding: Top={0}, Left={1}, Right={2}, Bottom={3}]", Top, Left, Right, Bottom);
		}
	}
}

