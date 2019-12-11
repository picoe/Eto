using System;
using Eto.Drawing;

namespace Eto.Forms
{
	/// <summary>
	/// Event arguments for a <see cref="DrawableCell.Paint"/> or <see cref="CustomCell.Paint"/> event.
	/// </summary>
	public class CellPaintEventArgs : PaintEventArgs
	{
		/// <summary>
		/// The state of the cell to be painted.
		/// </summary>
		public CellStates CellState { get; private set; }

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
		public CellPaintEventArgs(Graphics graphics, RectangleF clipRectangle, CellStates cellState, object item)
			: base(graphics, clipRectangle)
		{
			CellState = cellState;
			Item = item;
		}

		/// <summary>
		/// Gets a value indicating whether the cell is in edit mode.
		/// </summary>
		/// <value><c>true</c> if this cell is in edit mode; otherwise, <c>false</c>.</value>
		public bool IsEditing
		{
			get { return CellState.HasFlag(CellStates.Editing); }
		}

		/// <summary>
		/// Gets a value indicating whether the cell is currently selected.
		/// </summary>
		/// <value><c>true</c> if the cell is selected; otherwise, <c>false</c>.</value>
		public bool IsSelected
		{
			get { return CellState.HasFlag(CellStates.Selected); }
		}

		internal void DrawCenteredText(string value, Color? color = null, Font font = null, RectangleF? rect = null)
		{
			var c = color ?? (IsSelected ? SystemColors.HighlightText : SystemColors.ControlText);
			var f = font ?? SystemFonts.Default();
			DrawCenteredText(rect ?? ClipRectangle, c, f, value);
		}

		internal void DrawCenteredText(RectangleF rect, Color color, Font font, string value)
		{
			var size = Graphics.MeasureString(font, value);
			var y = (rect.Height - size.Height) / 2;
			var location = rect.Location + new PointF(0, y);
			Graphics.DrawText(font, color, location, value);
		}
	}

	/// <summary>
	/// Drawable cell paint event arguments.
	/// </summary>
	[Obsolete("Since 2.2: Use CellPaintEventArgs instead")]
	public class DrawableCellPaintEventArgs : CellPaintEventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.DrawableCellPaintEventArgs"/> class.
		/// </summary>
		/// <param name="graphics">Graphics.</param>
		/// <param name="clipRectangle">Clip rectangle.</param>
		/// <param name="cellState">Cell state.</param>
		/// <param name="item">Item.</param>
		public DrawableCellPaintEventArgs(Graphics graphics, RectangleF clipRectangle, CellStates cellState, object item)
			: base(graphics, clipRectangle, cellState, item)
		{
		}
	}

	/// <summary>
	/// A cell that is rendered by custom code.
	/// </summary>
	[Handler(typeof(DrawableCell.IHandler))]
	public class DrawableCell : Cell
	{
		#pragma warning disable 618
		/// <summary>
		/// Occurs when the cell needs painting.
		/// </summary>
		public event EventHandler<DrawableCellPaintEventArgs> Paint;
		#pragma warning restore 618

		/// <summary>
		/// Raises the <see cref="Paint"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		[Obsolete("Since 2.2: Use override with CellPaintEventArgs instead")]
		protected virtual void OnPaint(DrawableCellPaintEventArgs e)
		{
			OnPaint((CellPaintEventArgs)e);
		}

		/// <summary>
		/// Raises the <see cref="Paint"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnPaint(CellPaintEventArgs e)
		{
			#pragma warning disable 618
			if (Paint != null)
				Paint(this, (DrawableCellPaintEventArgs)e);
			#pragma warning restore 618
		}

		static readonly object callback = new Callback();

		/// <summary>
		/// Gets an instance of an object used to perform callbacks to the widget from handler implementations
		/// </summary>
		/// <returns>The callback.</returns>
		protected override object GetCallback() { return callback; }

		#pragma warning disable 618
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
				using (widget.Platform.Context)
					widget.OnPaint(e);
			}
		}
		#pragma warning restore 618


		/// <summary>
		/// Handler interface for the <see cref="DrawableCell"/>.
		/// </summary>
		public new interface IHandler : Cell.IHandler
		{
		}
	}

	/// <summary>
	/// Orientation of a <see cref="Splitter"/> control.
	/// </summary>
	[Obsolete("Since 2.1: Use CellStates instead")]
	public struct DrawableCellStates
	{
		readonly CellStates value;

		DrawableCellStates(CellStates value)
		{
			this.value = value;
		}

		/// <summary>
		/// Row is selected
		/// </summary>
		public static DrawableCellStates Selected { get { return CellStates.Selected; } }

		/// <summary>
		/// Normal state
		/// </summary>
		public static DrawableCellStates None { get { return CellStates.None; } }

		/// <summary>Converts to an Orientation</summary>
		public static implicit operator CellStates(DrawableCellStates orientation)
		{
			return orientation.value;
		}

		/// <summary>Converts an Orientation to a SplitterOrientation</summary>
		public static implicit operator DrawableCellStates(CellStates orientation)
		{
			return new DrawableCellStates(orientation);
		}

		/// <summary>Compares for equality</summary>
		/// <param name="value1">New enumeration.</param>
		/// <param name="value2">Old enumeration.</param>
		public static bool operator ==(CellStates value1, DrawableCellStates value2)
		{
			return value1 == value2.value;
		}

		/// <summary>Compares for inequality</summary>
		/// <param name="value1">New enumeration.</param>
		/// <param name="value2">Old enumeration.</param>
		public static bool operator !=(CellStates value1, DrawableCellStates value2)
		{
			return value1 != value2.value;
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="Eto.Forms.SplitterOrientation"/>.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="Eto.Forms.SplitterOrientation"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
		/// <see cref="Eto.Forms.SplitterOrientation"/>; otherwise, <c>false</c>.</returns>
		public override bool Equals(object obj)
		{
			return (obj is DrawableCellStates && (this == (DrawableCellStates)obj))
				|| (obj is CellStates && (this == (CellStates)obj));
		}

		/// <summary>
		/// Serves as a hash function for a <see cref="Eto.Forms.SplitterOrientation"/> object.
		/// </summary>
		/// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.</returns>
		public override int GetHashCode()
		{
			return value.GetHashCode();
		}
	}

}
