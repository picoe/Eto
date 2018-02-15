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
			public swf.DataGridView EditingControlDataGridView
			{
				get; set;
			}

			public object EditingControlFormattedValue
			{
				get; set;
			}

			public int EditingControlRowIndex
			{
				get; set;
			}

			public bool EditingControlValueChanged
			{
				get; set;
			}

			public swf.Cursor EditingPanelCursor
			{
				get { return swf.Cursors.Default; }
			}

			public bool RepositionEditingControlOnValueChange
			{
				get { return false; }
			}

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
			}
		}
		public class EtoCell : swf.DataGridViewTextBoxCell
		{
			public CustomCellHandler Handler { get; set; }

			public override Type EditType
			{
				get { return typeof(EtoEditType); }
			}

			public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, swf.DataGridViewCellStyle dataGridViewCellStyle)
			{
				base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
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
				if (!ReferenceEquals(cachedGraphicsKey, graphics) || cachedGraphics == null)
				{
					cachedGraphicsKey = graphics;
					cachedGraphics = new Graphics(new GraphicsHandler(graphics, dispose: false));
				}
				else
				{
					((GraphicsHandler)cachedGraphics.Handler).SetInitialState();
				}
				graphics.PixelOffsetMode = sd.Drawing2D.PixelOffsetMode.Half;
				graphics.SetClip(cellBounds);
				var color = new sd.SolidBrush(cellState.HasFlag(swf.DataGridViewElementStates.Selected) ? cellStyle.SelectionBackColor : cellStyle.BackColor);
                graphics.FillRectangle(color, cellBounds);
				var args = new CellPaintEventArgs(cachedGraphics, cellBounds.ToEto(), cellState.ToEto(), value);
				Handler.Callback.OnPaint(Handler.Widget, args);
				graphics.ResetClip();
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

