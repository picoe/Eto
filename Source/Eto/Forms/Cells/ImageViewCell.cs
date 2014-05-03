using System;


namespace Eto.Forms
{
	public interface IImageViewCell : ICell
	{
	}

	[Handler(typeof(IImageViewCell))]
	public class ImageViewCell : SingleValueCell, IImageViewCell
	{
		public ImageViewCell (int column)
		{
			Binding = new ColumnBinding (column);
		}
		
		public ImageViewCell (string property)
		{
			Binding = new PropertyBinding (property);
		}

		public ImageViewCell()
		{
		}

		[Obsolete("Use default constructor instead")]
		public ImageViewCell (Generator generator)
			: base(generator, typeof(IImageViewCell), true)
		{
		}
	}
}

