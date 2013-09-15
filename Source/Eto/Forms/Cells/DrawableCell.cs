using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface IDrawableCell : ICell
	{
	}

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
	public class DrawableCell : Cell
	{
		public Action<DrawableCellPaintArgs> PaintHandler { get; set; }

		public DrawableCell ()
			: this(Generator.Current)
		{
		}

		public DrawableCell(Generator g)
			: base(g, typeof(IDrawableCell), true)
		{
		}
	}
}
