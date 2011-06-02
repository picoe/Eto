using System;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface IImageView : IControl
	{
		IImage Image { get; set; }
	}
	
	public class ImageView : Control
	{
		IImageView inner;
		
		public ImageView ()
			: this(Generator.Current)
		{
		}
		
		public ImageView (Generator g)
			: base(g, typeof(IImageView))
		{
			inner = (IImageView)Handler;
		}
		
		public IImage Image
		{
			get { return inner.Image; }
			set { inner.Image = value; }
		}
	}
}

