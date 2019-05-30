using System;


namespace Eto.Forms
{
	/// <summary>
	/// Cell for <see cref="Grid"/> controls to show and bind a boolean value to a check box.
	/// </summary>
	[Handler(typeof(CheckBoxCell.IHandler))]
	public class CheckBoxCell : SingleValueCell<bool?>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.CheckBoxCell"/> class.
		/// </summary>
		/// <param name="column">Index of the column to bind to.</param>
		public CheckBoxCell(int column)
			: this()
		{
			Binding = new ColumnBinding<bool?>(column);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.CheckBoxCell"/> class with the specified property to bind to.
		/// </summary>
		/// <param name="property">Property to bind the value of the check box to.</param>
		public CheckBoxCell(string property)
			: this()
		{
			Binding = Eto.Forms.Binding.Property<bool?>(property);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.CheckBoxCell"/> class.
		/// </summary>
		public CheckBoxCell()
		{
		}

		/// <summary>
		/// Handler interface for the <see cref="CheckBoxCell"/>.
		/// </summary>
		public new interface IHandler : SingleValueCell<bool?>.IHandler
		{
		}
	}
}

