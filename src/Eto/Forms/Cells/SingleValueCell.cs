using System;

namespace Eto.Forms
{
	/// <summary>
	/// Base class for cells that bind to a single value.
	/// </summary>
	public abstract class SingleValueCell<T> : Cell
	{
		/// <summary>
		/// Gets or sets the binding to get/set the value of the cell.
		/// </summary>
		/// <value>The cell's binding.</value>
		public IIndirectBinding<T> Binding { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.SingleValueCell{T}"/> class.
		/// </summary>
		protected SingleValueCell()
		{
		}
	}
}

