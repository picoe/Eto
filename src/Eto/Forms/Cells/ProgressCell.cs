using Eto.Forms;

namespace Eto.Forms
{
	/// <summary>
	/// Cell for <see cref="Grid"/> controls to show and bind a int value to a progress bar.
	/// </summary>
	[Handler(typeof(ProgressCell.IHandler))]
	public class ProgressCell : SingleValueCell<float?>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ProgressCell"/> class.
		/// </summary>
		/// <param name="column">Index of the column to bind to.</param>
		public ProgressCell(int column)
		{
			Binding = new ColumnBinding<float?>(column);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ProgressCell"/> class with the specified property to bind to.
		/// </summary>
		/// <param name="property">Property to bind the value of the progress bar to.</param>
		/// <param name="ignoreCase">True to ignore case for the property, false to be case sensitive. Default is true.</param>
		public ProgressCell(string property, bool ignoreCase = true)
		{
			Binding = Eto.Forms.Binding.Property<float?>(property, ignoreCase);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ProgressCell"/> class.
		/// </summary>
		public ProgressCell()
		{
		}

		/// <summary>
		/// Handler interface for the <see cref="ProgressCell"/>.
		/// </summary>
		public new interface IHandler : SingleValueCell<int>.IHandler
		{
		}
	}
}
