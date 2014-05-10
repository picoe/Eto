using System;


namespace Eto.Forms
{
	[Handler(typeof(ImageViewCell.IHandler))]
	public class ImageViewCell : SingleValueCell
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
			: base(generator, typeof(IHandler), true)
		{
		}

		public new interface IHandler : SingleValueCell.IHandler
		{
		}
	}
}

