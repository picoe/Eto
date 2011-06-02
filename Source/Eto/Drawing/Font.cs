using System;

namespace Eto.Drawing
{
	public enum FontFamily
	{
		Monospace,
		Sans,
		Serif
	}
	
	public interface IFont : IInstanceWidget
	{
		void Create(FontFamily family);
		bool Bold { get; set; }
		bool Italic { get; set; }
		float Size { get; set; }
	}
	
	public class Font : InstanceWidget
	{
		IFont inner;

		public Font(FontFamily family, float size)
			: this(Generator.Current, family, size)
		{
		}

		public Font(Generator g, FontFamily family, float size)
			: base(g, typeof(IFont))
		{
			inner = (IFont)Handler;
			inner.Create(family);
			inner.Size = size;
		}

		public float Size
		{
			get { return inner.Size; }
			set { inner.Size = value; }
		}

		public bool Bold
		{
			get { return inner.Bold; }
			set { inner.Bold = value; }
		}

		public bool Italic
		{
			get { return inner.Italic; }
			set { inner.Italic = value; }
		}
	}
}
