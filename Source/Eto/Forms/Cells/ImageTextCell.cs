using System;
using Eto.Drawing;


namespace Eto.Forms
{
	/// <summary>
	/// Cell for <see cref="Grid"/> controls to show image and text in one cell.
	/// </summary>
	[Handler(typeof(ImageTextCell.IHandler))]
	public class ImageTextCell : Cell
	{
		/// <summary>
		/// Gets or sets the binding of the image to display for the cell.
		/// </summary>
		/// <value>The image binding.</value>
		public IIndirectBinding<Image> ImageBinding { get; set; }

		/// <summary>
		/// Gets or sets the binding of the text to display for the cell.
		/// </summary>
		/// <value>The text binding.</value>
		public IIndirectBinding<string> TextBinding { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ImageTextCell"/> class when binding to an indexed-based data item.
		/// </summary>
		/// <param name="imageColumn">Index of the image column in the data item.</param>
		/// <param name="textColumn">Index of the text column in the data item.</param>
		public ImageTextCell(int imageColumn, int textColumn)
		{
			ImageBinding = new ColumnBinding<Image>(imageColumn);
			TextBinding = new ColumnBinding<string>(textColumn);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ImageTextCell"/> class when binding to properties via reflection.
		/// </summary>
		/// <param name="imageProperty">Name of the image property in the data item.</param>
		/// <param name="textProperty">Name of the text property in the data item.</param>
		public ImageTextCell(string imageProperty, string textProperty)
		{
			ImageBinding = new PropertyBinding<Image>(imageProperty);
			TextBinding = new PropertyBinding<string>(textProperty);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ImageTextCell"/> class.
		/// </summary>
		public ImageTextCell()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ImageTextCell"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		[Obsolete("Use default constructor instead")]
		public ImageTextCell(Generator generator)
			: base(generator, typeof(IHandler), true)
		{
		}

		/// <summary>
		/// Handler interface for the <see cref="ImageTextCell"/>.
		/// </summary>
		public new interface IHandler : Cell.IHandler
		{
		}
	}
}

