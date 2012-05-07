using System;

namespace Eto.Drawing
{
	public interface IImage : IInstanceWidget
	{
		Size Size { get; }
	}
	
	public abstract class Image : InstanceWidget, IImage
	{
		IImage inner;
		protected Image(Generator g, Type type) : base(g, type)
		{
			inner = (IImage)Handler;
		}

		protected Image(Generator g, IImage handler) : base(g, handler)
		{
			inner = (IImage)Handler;
		}
		
		
		public Size Size
		{
			get { return inner.Size; }
		}
	}
}
