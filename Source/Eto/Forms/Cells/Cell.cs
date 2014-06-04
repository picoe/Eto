using System;

namespace Eto.Forms
{
	/// <summary>
	/// Base class for cells in a <see cref="Grid"/>.
	/// </summary>
	public abstract class Cell : Widget
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Cell"/> class.
		/// </summary>
		protected Cell()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Cell"/> class.
		/// </summary>
		/// <param name="g">The green component.</param>
		/// <param name="type">Type.</param>
		/// <param name="initialize">If set to <c>true</c> initialize.</param>
		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected Cell (Generator g, Type type, bool initialize)
			: base(g, type, initialize)
		{
		}
	}
}

