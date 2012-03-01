using System;

namespace Eto.Forms
{
	public interface IImageTextCell : ICell
	{
	}
	
	public class ImageTextCell : Cell
	{
		public ImageTextCell ()
			: this(Generator.Current)
		{
		}
		
		public ImageTextCell (Generator g)
			: base(g, typeof(IImageTextCell), true)
		{
		}
	}
}

