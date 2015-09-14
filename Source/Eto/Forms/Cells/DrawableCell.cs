using System;
using Eto.Drawing;

namespace Eto.Forms
{
	/// <summary>
	/// Cell state for the <see cref="DrawableCell"/>
	/// </summary>
	[Flags]
	public enum DrawableCellStates
	{
		/// <summary>
		/// Normal state
		/// </summary>
		None = 0,
		/// <summary>
		/// Row is selected
		/// </summary>
		Selected = 1,
	}

	/// <summary>
	/// Event arguments for a <see cref="DrawableCell.Paint"/> event
	/// </summary>
	public class DrawableCellPaintEventArgs : PaintEventArgs
	{
		/// <summary>
		/// The state of the cell to be painted.
		/// </summary>
		public DrawableCellStates CellState { get; private set; }

		/// <summary>
		/// The model data item for the row being painted.
		/// </summary>
		public object Item { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.DrawableCellPaintEventArgs"/> class.
		/// </summary>
		/// <param name="graphics">Graphics context for drawing.</param>
		/// <param name="clipRectangle">Clip rectangle for the cell's region.</param>
		/// <param name="cellState">State of the cell.</param>
		/// <param name="item">Item from the data store for the row that is being painted.</param>
		public DrawableCellPaintEventArgs(Graphics graphics, RectangleF clipRectangle, DrawableCellStates cellState, object item)
			: base(graphics, clipRectangle)
		{
			CellState = cellState;
			Item = item;
		}
	}

	/// <summary>
	/// A cell that is rendered by custom code.
	/// </summary>
	[Handler(typeof(DrawableCell.IHandler))]
	public class DrawableCell : Cell
	{
		/// <summary>
		/// Occurs when the cell needs painting.
		/// </summary>
		public event EventHandler<DrawableCellPaintEventArgs> Paint;

		/// <summary>
		/// Raises the <see cref="Paint"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnPaint(DrawableCellPaintEventArgs e)
		{
			if (Paint != null)
				Paint(this, e);
		}

		static readonly object callback = new Callback();

		/// <summary>
		/// Gets an instance of an object used to perform callbacks to the widget from handler implementations
		/// </summary>
		/// <returns>The callback.</returns>
		protected override object GetCallback() { return callback; }

		/// <summary>
		/// Callback interface for handlers of the <see cref="DrawableCell"/> cell.
		/// </summary>
		public new interface ICallback : Cell.ICallback
		{
			/// <summary>
			/// Raises the paint event.
			/// </summary>
			void OnPaint(DrawableCell widget, DrawableCellPaintEventArgs e);
		}

		/// <summary>
		/// Callback implementation for the <see cref="DrawableCell"/> cell.
		/// </summary>
		protected class Callback : ICallback
		{
			/// <summary>
			/// Raises the paint event.
			/// </summary>
			public void OnPaint(DrawableCell widget, DrawableCellPaintEventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnPaint(e));
			}
		}

		/// <summary>
		/// Handler interface for the <see cref="DrawableCell"/>.
		/// </summary>
		public new interface IHandler : Cell.IHandler
		{
		}
	}
}
