using System;

namespace Eto.Drawing
{
	public interface IImage : IInstanceWidget
	{
		Size Size { get; }
	}
	
	public class Image : InstanceWidget, IImage
	{
		IImage inner;
		public Image(Generator g, Type type) : base(g, type)
		{
			inner = (IImage)Handler;
		}

		public Image(Generator g, IImage inner) : base(g, inner)
		{
			inner = (IImage)Handler;
		}
		
		
		public Size Size
		{
			get { return inner.Size; }
		}
	}
}
