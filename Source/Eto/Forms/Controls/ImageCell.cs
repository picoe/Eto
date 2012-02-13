using System;

namespace Eto.Forms
{
	public interface IImageCell : ICell
	{
	}
	
	public class ImageCell : Cell, IImageCell
	{
		public ImageCell ()
			: this(Generator.Current)
		{
		}
		
		public ImageCell (Generator g)
			: base(g, typeof(IImageCell), true)
		{
		}
	}
}

