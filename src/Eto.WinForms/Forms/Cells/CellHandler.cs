using System;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Forms;

namespace Eto.WinForms.Forms.Cells
{
	public interface ICellConfigHandler
	{
		swf.DataGridViewColumn Column { get; }

		void Paint (sd.Graphics graphics, sd.Rectangle clipBounds, sd.Rectangle cellBounds, int rowIndex, swf.DataGridViewElementStates cellState, object value, object formattedValue, string errorText, swf.DataGridViewCellStyle cellStyle, swf.DataGridViewAdvancedBorderStyle advancedBorderStyle, ref swf.DataGridViewPaintParts paintParts);

		int GetRowOffset (int rowIndex);

		bool MouseClick (swf.MouseEventArgs e, int rowIndex);
	}

	public interface ICellHandler
	{
		ICellConfigHandler CellConfig { get; set; }
		swf.DataGridViewCell Control { get; }
		void SetCellValue (object dataItem, object value);
		object GetCellValue (object dataItem);
	}
	
	public abstract class CellHandler<TControl, TWidget, TCallback> : WidgetHandler<TControl, TWidget, TCallback>, Cell.IHandler, ICellHandler
		where TControl: swf.DataGridViewCell
		where TWidget: Cell
	{
		swf.DataGridViewCell ICellHandler.Control {
			get { return Control; }
		}

		ICellConfigHandler _cellConfig;
		public ICellConfigHandler CellConfig
		{
			get { return _cellConfig; }
			set
			{
				_cellConfig = value;
				InitializeColumn();
			}
		}

		public swf.DataGridViewColumn Column => CellConfig?.Column;

		protected virtual void InitializeColumn()
		{
		}

		protected void PositionEditingControl (int row, ref sd.Rectangle cellClip, ref sd.Rectangle cellBounds, int customOffset = 0)
		{
			var val = GetRowOffset (row) + customOffset;
			if (val > 0) {
				cellBounds.X += val;
				cellBounds.Width -= val;
				cellClip.X += val;
				cellClip.Width -= val;
			}
		}

		protected void Paint (sd.Graphics graphics, sd.Rectangle clipBounds, ref sd.Rectangle cellBounds, int rowIndex, swf.DataGridViewElementStates cellState, object value, object formattedValue, string errorText, swf.DataGridViewCellStyle cellStyle, swf.DataGridViewAdvancedBorderStyle advancedBorderStyle, ref swf.DataGridViewPaintParts paintParts)
		{
			if (CellConfig != null) {
				CellConfig.Paint (graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, ref paintParts);

				var offset = CellConfig.GetRowOffset (rowIndex);
				cellBounds.X += offset;
				cellBounds.Width -= offset;
			}
		}

		protected bool MouseClick (swf.MouseEventArgs e, int rowIndex)
		{
			return CellConfig != null && CellConfig.MouseClick(e, rowIndex);
		}

		protected int GetRowOffset (int row)
		{
			return CellConfig != null ? CellConfig.GetRowOffset(row) : 0;
		}

		public abstract void SetCellValue (object dataItem, object value);

		public abstract object GetCellValue (object dataItem);

	}
}

