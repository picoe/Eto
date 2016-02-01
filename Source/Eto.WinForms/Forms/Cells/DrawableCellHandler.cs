using System;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Forms;
using Eto.Drawing;
using Eto.WinForms.Drawing;

namespace Eto.WinForms.Forms.Cells
{
	public class DrawableCellHandler : CellHandler<DrawableCellHandler.EtoCell, DrawableCell, DrawableCell.ICallback>, DrawableCell.IHandler
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

			public override void PositionEditingControl(bool setLocation, bool setSize, sd.Rectangle cellBounds, sd.Rectangle cellClip, swf.DataGridViewCellStyle cellStyle, bool singleVerticalBorderAdded, bool singleHorizontalBorderAdded, bool isFirstDisplayedColumn, bool isFirstDisplayedRow)
			{
				Handler.PositionEditingControl(RowIndex, ref cellClip, ref cellBounds);
				base.PositionEditingControl(setLocation, setSize, cellBounds, cellClip, cellStyle, singleVerticalBorderAdded, singleHorizontalBorderAdded, isFirstDisplayedColumn, isFirstDisplayedRow);
			}

			protected override sd.Size GetPreferredSize(sd.Graphics graphics, swf.DataGridViewCellStyle cellStyle, int rowIndex, sd.Size constraintSize)
			{
				var size = base.GetPreferredSize(graphics, cellStyle, rowIndex, constraintSize);
				size.Width += Handler.GetRowOffset(rowIndex);
				return size;
			}

			// Cache the Eto graphics between cell redraws, since rows
			// are drawn using the same sd.Graphics.
			sd.Graphics cachedGraphicsKey;
			Graphics cachedGraphics;

			protected override void Paint(sd.Graphics graphics, sd.Rectangle clipBounds, sd.Rectangle cellBounds, int rowIndex, swf.DataGridViewElementStates cellState, object value, object formattedValue, string errorText, swf.DataGridViewCellStyle cellStyle, swf.DataGridViewAdvancedBorderStyle advancedBorderStyle, swf.DataGridViewPaintParts paintParts)
			{
				// save graphics state to prevent artifacts in other paint operations in the grid
				var state = graphics.Save();
				if (!object.ReferenceEquals(cachedGraphicsKey, graphics) || cachedGraphics == null)
				{
					cachedGraphicsKey = graphics;
					cachedGraphics = new Graphics(new GraphicsHandler(graphics, dispose: false));
				}
				else
				{
					((GraphicsHandler)cachedGraphics.Handler).SetInitialState();
				}
				graphics.SetClip(cellBounds);
#pragma warning disable 618
				var args = new DrawableCellPaintEventArgs(cachedGraphics, cellBounds.ToEto(), cellState.ToEto(), value);
				Handler.Callback.OnPaint(Handler.Widget, args);
#pragma warning restore 618
				graphics.ResetClip();
				graphics.Restore(state);
			}

			protected override void OnMouseClick(swf.DataGridViewCellMouseEventArgs e)
			{
				if (!Handler.MouseClick(e, e.RowIndex))
					base.OnMouseClick(e);
			}

			public override object Clone()
			{
				var val = (EtoCell)base.Clone();
				val.Handler = Handler;
				return val;
			}
		}


		public DrawableCellHandler()
		{
			Control = new EtoCell { Handler = this };
		}

		public override object GetCellValue(object dataItem)
		{
			return dataItem; // the cell value of an owner drawn cell is the model item
		}

		public override void SetCellValue(object dataItem, object value)
		{
		}
	}
}

