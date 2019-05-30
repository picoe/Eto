using System;
using System.ComponentModel;

namespace Eto.Forms
{
	/// <summary>
	/// Cell for a text box in a <see cref="Grid"/>.
	/// </summary>
	[Handler(typeof(IHandler))]
	public class TextBoxCell : SingleValueCell<string>
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
		/// Initializes a new instance of the <see cref="Eto.Forms.TextBoxCell"/> class binding to the specified <paramref name="column"/>.
		/// </summary>
		/// <param name="column">Column in the data source to get/set the data.</param>
		public TextBoxCell(int column)
		{
			Binding = new ColumnBinding<string>(column);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TextBoxCell"/> class binding the text value to the specified <paramref name="property"/> of the data store.
		/// </summary>
		/// <param name="property">Name of the property to bind to in the data store.</param>
		public TextBoxCell(string property)
		{
			Binding = Eto.Forms.Binding.Property<string>(property);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TextBoxCell"/> class.
		/// </summary>
		public TextBoxCell()
		{
		}

		/// <summary>
		/// Handler interface for the <see cref="TextBoxCell"/>.
		/// </summary>
		public new interface IHandler : SingleValueCell<string>.IHandler
		{
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

