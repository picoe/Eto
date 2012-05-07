using System;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Forms;

namespace Eto.Platform.Windows.Forms.Controls
{
	public interface ICellConfigHandler
	{
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
	
	public abstract class CellHandler<T, W> : WidgetHandler<T, W>, ICell, ICellHandler
		where T: swf.DataGridViewCell
		where W: Cell
	{
		swf.DataGridViewCell ICellHandler.Control {
			get { return Control; }
		}

		public ICellConfigHandler CellConfig { get; set; }

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
			if (CellConfig != null)
				return CellConfig.MouseClick (e, rowIndex);
			else
				return false;
		}

		protected int GetRowOffset (int row)
		{
			if (CellConfig != null)
				return CellConfig.GetRowOffset (row);
			else
				return 0;
		}

		public abstract void SetCellValue (object dataItem, object value);

		public abstract object GetCellValue (object dataItem);

	}
}

