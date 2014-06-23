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

	public class DrawableCellPaintEventArgs : PaintEventArgs
	{
		/// <summary>
		/// The state of the cell to be painted.
		/// </summary>
		public DrawableCellStates CellState { get; private set; }

		/// <summary>
		/// The model data item
		/// </summary>
		public object Item { get; private set; }

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
		public event EventHandler<DrawableCellPaintEventArgs> Paint;

		protected virtual void OnPaint(DrawableCellPaintEventArgs e)
		{
			if (Paint != null)
				Paint(this, e);
		}

		public DrawableCell()
		{
		}

		[Obsolete("Use default constructor instead")]
		public DrawableCell(Generator generator)
			: base(generator, typeof(IHandler), true)
		{
		}

		static readonly object callback = new Callback();

		protected override object GetCallback() { return callback; }

		public new interface ICallback : Cell.ICallback
		{
			void OnPaint(DrawableCell widget, DrawableCellPaintEventArgs e);
		}

		protected class Callback : ICallback
		{
			public void OnPaint(DrawableCell widget, DrawableCellPaintEventArgs e)
			{
				widget.OnPaint(e);
			}
		}

		public new interface IHandler : Cell.IHandler
		{
		}
	}
}
