using System;
using Eto.Drawing;

namespace Eto.Forms
{
	/// <summary>
	/// Cell for <see cref="Grid"/> controls to show a single <see cref="Image"/>.
	/// </summary>
	[Handler(typeof(ImageViewCell.IHandler))]
	public class ImageViewCell : SingleValueCell<Image>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ImageViewCell"/> class when binding to an indexed-based data item.
		/// </summary>
		/// <param name="column">Index of the column the image is in each data item.</param>
		public ImageViewCell(int column)
		{
			Binding = new ColumnBinding<Image>(column);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ImageViewCell"/> class when binding to properties via reflection.
		/// </summary>
		/// <param name="property">Property to bind to in each data item.</param>
		public ImageViewCell(string property)
		{
			Binding = new PropertyBinding<Image>(property);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ImageViewCell"/> class.
		/// </summary>
		public ImageViewCell()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ImageViewCell"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		[Obsolete("Use default constructor instead")]
		public ImageViewCell(Generator generator)
			: base(generator, typeof(IHandler), true)
		{
		}

		/// <summary>
		/// Handler interface for the <see cref="ImageViewCell"/>.
		/// </summary>
		public new interface IHandler : SingleValueCell<Image>.IHandler
		{
		}
	}
}

