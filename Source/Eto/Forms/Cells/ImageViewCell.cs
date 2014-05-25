using System;
using Eto.Drawing;


namespace Eto.Forms
{
	[Handler(typeof(ImageViewCell.IHandler))]
	public class ImageViewCell : SingleValueCell<Image>
	{
		public ImageViewCell (int column)
		{
			Binding = new ColumnBinding<Image> (column);
		}
		
		public ImageViewCell (string property)
		{
			Binding = new PropertyBinding<Image> (property);
		}

		public ImageViewCell()
		{
		}

		[Obsolete("Use default constructor instead")]
		public ImageViewCell (Generator generator)
			: base(generator, typeof(IHandler), true)
		{
		}

		public new interface IHandler : SingleValueCell<Image>.IHandler
		{
		}
	}
}

