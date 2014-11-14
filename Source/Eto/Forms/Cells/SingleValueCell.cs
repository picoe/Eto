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

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.SingleValueCell{T}"/> class.
		/// </summary>
		/// <param name="g">The green component.</param>
		/// <param name="type">Type.</param>
		/// <param name="initialize">If set to <c>true</c> initialize.</param>
		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected SingleValueCell (Generator g, Type type, bool initialize)
			: base(g, type, initialize)
		{
		}
	}
}

