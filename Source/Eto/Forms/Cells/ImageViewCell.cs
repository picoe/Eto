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
		new IHandler Handler { get { return (IHandler)base.Handler; } }

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
			Binding = Eto.Forms.Binding.Property<Image>(property);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ImageViewCell"/> class.
		/// </summary>
		public ImageViewCell()
		{
		}

		/// <summary>
		/// Gets or sets the interpolation mode when scaling images to fit into the cell.
		/// </summary>
		/// <value>The image interpolation.</value>
		public ImageInterpolation ImageInterpolation
		{
			get { return Handler.ImageInterpolation; }
			set { Handler.ImageInterpolation = value; }
		}

		/// <summary>
		/// Handler interface for the <see cref="ImageViewCell"/>.
		/// </summary>
		public new interface IHandler : SingleValueCell<Image>.IHandler
		{
			/// <summary>
			/// Gets or sets the interpolation mode when scaling images to fit into the cell.
			/// </summary>
			/// <value>The image interpolation.</value>
			ImageInterpolation ImageInterpolation { get; set; }
		}
	}
}

