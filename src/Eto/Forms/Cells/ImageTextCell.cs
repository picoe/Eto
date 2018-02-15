using System;
using System.ComponentModel;
using Eto.Drawing;


namespace Eto.Forms
{
	/// <summary>
	/// Cell for <see cref="Grid"/> controls to show image and text in one cell.
	/// </summary>
	[Handler(typeof(ImageTextCell.IHandler))]
	public class ImageTextCell : Cell
	{
		new IHandler Handler => (IHandler)base.Handler;

		/// <summary>
		/// Gets or sets the horizontal alignment of the text within the cell.
		/// </summary>
		/// <value>The text alignment.</value>
		public TextAlignment TextAlignment
		{
			get { return Handler.TextAlignment; }
			set { Handler.TextAlignment = value; }
		}

		/// <summary>
		/// Gets or sets the vertical alignment of the text within the cell.
		/// </summary>
		/// <value>The vertical text alignment.</value>
		[DefaultValue(VerticalAlignment.Center)]
		public VerticalAlignment VerticalAlignment
		{
			get { return Handler.VerticalAlignment; }
			set { Handler.VerticalAlignment = value; }
		}

		/// <summary>
		/// Gets or sets the auto selection mode.
		/// </summary>
		/// <value>The auto selection mode.</value>
		public AutoSelectMode AutoSelectMode
		{
			get { return Handler.AutoSelectMode; }
			set { Handler.AutoSelectMode = value; }
		}

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
			ImageBinding = Eto.Forms.Binding.Property<Image>(imageProperty);
			TextBinding = Eto.Forms.Binding.Property<string>(textProperty);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ImageTextCell"/> class.
		/// </summary>
		public ImageTextCell()
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
		/// Handler interface for the <see cref="ImageTextCell"/>.
		/// </summary>
		public new interface IHandler : Cell.IHandler
		{
			/// <summary>
			/// Gets or sets the interpolation mode when scaling images to fit into the cell.
			/// </summary>
			/// <value>The image interpolation.</value>
			ImageInterpolation ImageInterpolation { get; set; }

			/// <summary>
			/// Gets or sets the horizontal alignment of the text within the cell.
			/// </summary>
			/// <value>The text alignment.</value>
			TextAlignment TextAlignment { get; set; }

			/// <summary>
			/// Gets or sets the vertical alignment of the text within the cell.
			/// </summary>
			/// <value>The vertical text alignment.</value>
			VerticalAlignment VerticalAlignment { get; set; }

			/// <summary>
			/// Gets or sets the auto selection mode.
			/// </summary>
			/// <value>The auto selection mode.</value>
			AutoSelectMode AutoSelectMode { get; set; }
		}
	}
}

