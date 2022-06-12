using System;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Forms;
using Eto.Drawing;
using Eto.WinForms.Drawing;

namespace Eto.WinForms.Forms.Cells
{
	public class CustomCellHandler : CellHandler<CustomCellHandler.EtoCell, CustomCell, CustomCell.ICallback>, CustomCell.IHandler
	{
		public class EtoEditType : swf.Control, swf.IDataGridViewEditingControl
		{
			public EtoCell Cell { get; set; }

			public EtoEditType()
			{
			}
			public swf.DataGridView EditingControlDataGridView { get; set; }

			public object EditingControlFormattedValue { get; set; }
			public int EditingControlRowIndex { get; set; }

			public bool EditingControlValueChanged { get; set; } = true;

			public swf.Cursor EditingPanelCursor => swf.Cursors.Default;

			public bool RepositionEditingControlOnValueChange => false;

			public void ApplyCellStyleToEditingControl(swf.DataGridViewCellStyle dataGridViewCellStyle)
			{
			}

			public bool EditingControlWantsInputKey(swf.Keys keyData, bool dataGridViewWantsInputKey)
			{
				return false;
			}

			public object GetEditingControlFormattedValue(swf.DataGridViewDataErrorContexts context)
			{
				return null;
			}

			public void PrepareEditingControlForEdit(bool selectAll)
			{
				Controls.Clear();
				if (Cell == null)
					return;
				var h = Cell.Handler;
				var args = Cell.CreateArgs(EditingControlRowIndex);
				var control = h.Callback.OnCreateCell(h.Widget, args);
				h.Callback.OnConfigureCell(h.Widget, args, control);
				var native = control.ToNative(true);
				if (native != null)
				{
					native.Dock = swf.DockStyle.Fill;
					Controls.Add(native);
				}
			}
		}
		public class EtoCell : swf.DataGridViewCell
		{
			public CustomCellHandler Handler { get; set; }

			public override Type EditType => typeof(EtoEditType);

			public override Type FormattedValueType => typeof(object);

			public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, swf.DataGridViewCellStyle dataGridViewCellStyle)
			{
				base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
				if (DataGridView.EditingControl is EtoEditType editType)
				{
					editType.Cell = this;
				}

			}

			public override void PositionEditingControl(bool setLocation, bool setSize, sd.Rectangle cellBounds, sd.Rectangle cellClip, swf.DataGridViewCellStyle cellStyle, bool singleVerticalBorderAdded, bool singleHorizontalBorderAdded, bool isFirstDisplayedColumn, bool isFirstDisplayedRow)
			{
				Handler.PositionEditingControl(RowIndex, ref cellClip, ref cellBounds);
				base.PositionEditingControl(setLocation, setSize, cellBounds, cellClip, cellStyle, singleVerticalBorderAdded, singleHorizontalBorderAdded, isFirstDisplayedColumn, isFirstDisplayedRow);
			}

			public CellEventArgs CreateArgs(int rowIndex)
			{
				var gridHandler = Handler?.GridHandler;
				if (gridHandler == null)
					return null;
				var item = gridHandler.GetItemAtRow(rowIndex);
				return new CellEventArgs(gridHandler.Grid, Handler.Widget, rowIndex, ColumnIndex, item, CellStates.None, null);
			}

			protected override sd.Size GetPreferredSize(sd.Graphics graphics, swf.DataGridViewCellStyle cellStyle, int rowIndex, sd.Size constraintSize)
			{
				var size = base.GetPreferredSize(graphics, cellStyle, rowIndex, constraintSize);
				var gridHandler = Handler.GridHandler;
				if (gridHandler != null)
				{
					var args = CreateArgs(rowIndex);
					size.Width = (int)Math.Ceiling(Handler.Callback.OnGetPreferredWidth(Handler.Widget, args));
					size.Width += Handler.GetRowOffset(rowIndex);
				}
				return size;
			}

			// Cache the Eto graphics between cell redraws, since rows
			// are drawn using the same sd.Graphics.
			sd.Graphics cachedGraphicsKey;
			Graphics cachedGraphics;

			protected override void Paint(sd.Graphics graphics, sd.Rectangle clipBounds, sd.Rectangle cellBounds, int rowIndex, swf.DataGridViewElementStates cellState, object value, object formattedValue, string errorText, swf.DataGridViewCellStyle cellStyle, swf.DataGridViewAdvancedBorderStyle advancedBorderStyle, swf.DataGridViewPaintParts paintParts)
			{
				Handler.Paint(graphics, clipBounds, ref cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, ref paintParts);

				// save graphics state to prevent artifacts in other paint operations in the grid
				var state = graphics.Save();
				if (!ReferenceEquals(cachedGraphicsKey, graphics) || cachedGraphics == null)
				{
					cachedGraphicsKey = graphics;
					cachedGraphics = new Graphics(new GraphicsHandler(graphics, dispose: false));
				}
				else
				{
					((GraphicsHandler)cachedGraphics.Handler).SetInitialState();
				}

				var offset = graphics.PixelOffsetMode;
				graphics.PixelOffsetMode = sd.Drawing2D.PixelOffsetMode.Half;
				graphics.SetClip(cellBounds);
				var color = new sd.SolidBrush(cellState.HasFlag(swf.DataGridViewElementStates.Selected) ? cellStyle.SelectionBackColor : cellStyle.BackColor);
                graphics.FillRectangle(color, cellBounds);
				var args = new CellPaintEventArgs(cachedGraphics, cellBounds.ToEto(), cellState.ToEto(), value);
				Handler.Callback.OnPaint(Handler.Widget, args);
				graphics.ResetClip();
				graphics.PixelOffsetMode = offset;
				graphics.Restore(state);
			}

			protected override void OnMouseClick(swf.DataGridViewCellMouseEventArgs e)
			{
				if (!Handler.MouseClick(e, e.RowIndex))
				{
					if (DataGridView == null)
						return;
					var currentCellAddress = DataGridView.CurrentCellAddress;
					if (currentCellAddress.X == e.ColumnIndex && currentCellAddress.Y == e.RowIndex && e.Button == swf.MouseButtons.Left)
					{
						if (DataGridView.EditMode != swf.DataGridViewEditMode.EditProgrammatically)
						{
							DataGridView.BeginEdit(true);
						}
					}
					base.OnMouseClick(e);
				}
			}

			public override object Clone()
			{
				var val = (EtoCell)base.Clone();
				val.Handler = Handler;
				return val;
			}
		}


		public CustomCellHandler()
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

