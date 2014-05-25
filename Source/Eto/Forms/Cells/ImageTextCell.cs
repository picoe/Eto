using System;
using Eto.Drawing;


namespace Eto.Forms
{
	[Handler(typeof(ImageTextCell.IHandler))]
	public class ImageTextCell : Cell
	{
		public IIndirectBinding<Image> ImageBinding { get; set; }
		
		public IIndirectBinding<string> TextBinding { get; set; }
		
		public ImageTextCell (int imageColumn, int textColumn)
		{
			ImageBinding = new ColumnBinding<Image> (imageColumn);
			TextBinding = new ColumnBinding<string> (textColumn);
		}
		
		public ImageTextCell (string imageProperty, string textProperty)
		{
			ImageBinding = new PropertyBinding<Image>(imageProperty);
			TextBinding = new PropertyBinding<string>(textProperty);
		}
		
		public ImageTextCell()
		{
		}

		[Obsolete("Use default constructor instead")]
		public ImageTextCell (Generator generator)
			: base(generator, typeof(IHandler), true)
		{
		}

		public new interface IHandler : Cell.IHandler
		{
		}
	}
}

