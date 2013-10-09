using System;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Forms;
using Eto.Drawing;
using Eto.Platform.Windows.Drawing;

namespace Eto.Platform.Windows.Forms.Controls
{
	public class DrawableCellHandler : CellHandler<DrawableCellHandler.EtoCell, DrawableCell>, IDrawableCell
	{
		public class EtoCell : swf.DataGridViewCell
		{
			public DrawableCellHandler Handler { get; set; }

			public override Type FormattedValueType
			{
				get { return typeof(object); } // sd.DataGridView requires this.
			}

			protected override object GetFormattedValue(object value, int rowIndex, ref swf.DataGridViewCellStyle cellStyle, System.ComponentModel.TypeConverter valueTypeConverter, System.ComponentModel.TypeConverter formattedValueTypeConverter, swf.DataGridViewDataErrorContexts context)
			{
				return null;
			}

			public override void PositionEditingControl (bool setLocation, bool setSize, sd.Rectangle cellBounds, sd.Rectangle cellClip, swf.DataGridViewCellStyle cellStyle, bool singleVerticalBorderAdded, bool singleHorizontalBorderAdded, bool isFirstDisplayedColumn, bool isFirstDisplayedRow)
			{
				Handler.PositionEditingControl (RowIndex, ref cellClip, ref cellBounds);
				base.PositionEditingControl (setLocation, setSize, cellBounds, cellClip, cellStyle, singleVerticalBorderAdded, singleHorizontalBorderAdded, isFirstDisplayedColumn, isFirstDisplayedRow);
			}

			protected override sd.Size GetPreferredSize (sd.Graphics graphics, swf.DataGridViewCellStyle cellStyle, int rowIndex, sd.Size constraintSize)
			{
				var size = base.GetPreferredSize (graphics, cellStyle, rowIndex, constraintSize);
				size.Width += Handler.GetRowOffset (rowIndex);
				return size;
			}

			// Cache the Eto graphics between cell redraws, since rows
			// are drawn using the same sd.Graphics.
			private sd.Graphics cachedGraphicsKey = null;
			private Graphics cachedGraphics = null;

			protected override void Paint (sd.Graphics graphics, sd.Rectangle clipBounds, sd.Rectangle cellBounds, int rowIndex, swf.DataGridViewElementStates cellState, object value, object formattedValue, string errorText, swf.DataGridViewCellStyle cellStyle, swf.DataGridViewAdvancedBorderStyle advancedBorderStyle, swf.DataGridViewPaintParts paintParts)
			{
				if (!object.ReferenceEquals(cachedGraphicsKey, graphics) ||
					cachedGraphics == null)
				{
					cachedGraphicsKey = graphics;
					cachedGraphics = new Graphics(Handler.Generator, new GraphicsHandler(graphics, shouldDisposeGraphics: false));
				}

				if (Handler.Widget.PaintHandler != null)
				{
					var b = graphics.ClipBounds;
					graphics.SetClip(clipBounds);
					Handler.Widget.PaintHandler(new DrawableCellPaintArgs
					{
						Graphics = cachedGraphics,
						CellBounds = new RectangleF(cellBounds.ToEto()),
						Item = value,
						CellState = cellState.ToEto(),
					});
					graphics.SetClip(b); // restore
				}
			}

			protected override void OnMouseClick (swf.DataGridViewCellMouseEventArgs e)
			{
				if (!Handler.MouseClick (e, e.RowIndex))
					base.OnMouseClick (e);
			}

			public override object Clone ()
			{
				var val = base.Clone () as EtoCell;
				val.Handler = this.Handler;
				return val;
			}
		}


		public DrawableCellHandler ()
		{
			Control = new EtoCell { Handler = this };
		}

		public override object GetCellValue (object dataItem)
		{
			return dataItem; // the cell value of an owner drawn cell is the model item
		}

		public override void SetCellValue (object dataItem, object value)
		{
		}
	}
}

