using System;

namespace Eto.Forms
{
	public interface IImageViewCell : ICell
	{
	}
	
	public class ImageViewCell : SingleValueCell, IImageViewCell
	{
		public ImageViewCell (int column)
			: this()
		{
			Binding = new ColumnBinding (column);
		}
		
		public ImageViewCell (string property)
			: this()
		{
			Binding = new PropertyBinding (property);
		}

		public ImageViewCell ()
			: this(Generator.Current)
		{
		}
		
		public ImageViewCell (Generator g)
			: base(g, typeof(IImageViewCell), true)
		{
		}
	}
}

