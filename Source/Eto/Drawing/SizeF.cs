using System;

namespace Eto.Drawing
{
	public struct SizeF
	{
		public static readonly SizeF Empty = new SizeF(0, 0);
		
		public SizeF(float width, float height)
			: this()
		{
			this.Width = width;
			this.Height = height;
		}

		public float Width { get; set; }

		public float Height { get; set; }

		public static SizeF operator *(SizeF size1, SizeF size2)
		{
			SizeF result = size1;
			result.Width = size1.Width * size2.Width;
			result.Height = size1.Height * size2.Height;
			return result;
		}

		public static SizeF operator *(SizeF size1, float multiplier)
		{
			SizeF result = size1;
			result.Width = size1.Width * multiplier;
			result.Height = size1.Height * multiplier;
			return result;
		}
		
		public static SizeF operator /(SizeF size1, SizeF size2)
		{
			SizeF result = size1;
			result.Width = size1.Width / size2.Width;
			result.Height = size1.Height / size2.Height;
			return result;
		}

		public static SizeF operator /(SizeF size1, float divider)
		{
			SizeF result = size1;
			result.Width = size1.Width / divider;
			result.Height = size1.Height / divider;
			return result;
		}
		
		public static SizeF operator +(SizeF size1, SizeF size2)
		{
			SizeF result = size1;
			result.Width = size1.Width + size2.Width;
			result.Height = size1.Height + size2.Height;
			return result;
		}

		public static SizeF operator +(SizeF size1, float adder)
		{
			SizeF result = size1;
			result.Width = size1.Width + adder;
			result.Height = size1.Height + adder;
			return result;
		}

		public static bool operator ==(SizeF size1, SizeF size2)
		{
			return (size1.Width == size2.Width && size1.Height == size2.Height);
		}

		public static bool operator !=(SizeF size1, SizeF size2)
		{
			return (size1.Width != size2.Width || size1.Height != size2.Height);
		}

		public static implicit operator SizeF(Size size)
		{
			return new SizeF(size.Width, size.Height);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is SizeF)) return false;
			SizeF size = (SizeF)obj;		
			return (Width == size.Width && Height == size.Height);
		}

		public override int GetHashCode()
		{
			return Width.GetHashCode () ^ Height.GetHashCode ();
		}


		public override string ToString()
		{
			return String.Format("Width={0} Height={1}", Width, Height);
		}

	}
}
