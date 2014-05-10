using System;
using Eto.Drawing;

namespace Eto.Forms
{
	public enum DrawableCellState
	{
		Normal,
		Selected,
		// TODO: add Hover
	}

	public class DrawableCellPaintArgs
	{
		/// <summary>
		/// The Graphics to render the cell into
		/// </summary>
		public Graphics Graphics { get; set; }

		/// <summary>
		/// The bounds of the cell to be painted.
		/// Drawing code should not draw outside these bounds.
		/// </summary>
		public RectangleF CellBounds { get; set; }

		/// <summary>
		/// The state of the cell to be painted.
		/// </summary>
		public DrawableCellState CellState { get; set; }

		/// <summary>
		/// The model data item
		/// </summary>
		public object Item { get; set; }
	}

	/// <summary>
	/// A cell that is rendered by custom code.
	/// </summary>
	[Handler(typeof(DrawableCell.IHandler))]
	public class DrawableCell : Cell
	{
		public Action<DrawableCellPaintArgs> PaintHandler { get; set; }

		public DrawableCell()
		{
		}

		[Obsolete("Use default constructor instead")]
		public DrawableCell(Generator generator)
			: base(generator, typeof(IHandler), true)
		{
		}

		public new interface IHandler : Cell.IHandler
		{
		}
	}
}
