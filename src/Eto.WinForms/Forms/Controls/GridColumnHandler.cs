using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Forms;
using Eto.WinForms.Forms.Cells;
using System;

namespace Eto.WinForms.Forms.Controls
{

	public class GridColumnHandler : WidgetHandler<swf.DataGridViewColumn, GridColumn>, GridColumn.IHandler, ICellConfigHandler
	{
		Cell dataCell;
		bool autosize;

		public IGridHandler GridHandler { get; private set; }

		public GridColumnHandler ()
		{
			Control = new swf.DataGridViewColumn();
			DataCell = new TextBoxCell();
			Editable = false;
			AutoSize = true;
			Resizable = true;
		}

		public string HeaderText {
			get { return Control.HeaderText; }
			set { Control.HeaderText = value; }
		}

		public bool Resizable {
			get { return Control.Resizable == swf.DataGridViewTriState.True; }
			set { Control.Resizable = value ? swf.DataGridViewTriState.True : swf.DataGridViewTriState.False; }
		}

		public bool Sortable {
			get { return Control.SortMode == swf.DataGridViewColumnSortMode.Programmatic; }
			set { Control.SortMode = (value) ? swf.DataGridViewColumnSortMode.Programmatic : swf.DataGridViewColumnSortMode.NotSortable; }
		}

		public bool AutoSize {
			get { return autosize; }
			set {
				autosize = value;
				Control.AutoSizeMode = (value) ? swf.DataGridViewAutoSizeColumnMode.NotSet : swf.DataGridViewAutoSizeColumnMode.None; 
			}
		}

		public int Width {
			get { return Control.Width; }
			set { Control.Width = value; }
		}

		public Cell DataCell {
			get { return dataCell; }
			set {
				dataCell = value;
				if (dataCell != null) {
					var cellHandler = (ICellHandler)dataCell.Handler;
					cellHandler.CellConfig = this;
					Control.CellTemplate = cellHandler.Control;
				}
				else
					Control.CellTemplate = null;
			}
		}

		public bool Editable {
			get { return !Control.ReadOnly; }
			set { Control.ReadOnly = !value; }
		}

		public bool Visible {
			get { return Control.Visible; }
			set { Control.Visible = value; }
		}

		public swf.DataGridViewColumn Column => Control;

		public void SetCellValue (object dataItem, object value)
		{
			if (dataCell != null) {
				var cellHandler = (ICellHandler)dataCell.Handler;
				cellHandler.SetCellValue (dataItem, value);
			}
		}

		public object GetCellValue (object dataItem)
		{
			if (dataCell != null) {
				var cellHandler = ((ICellHandler)dataCell.Handler);
				return cellHandler.GetCellValue (dataItem);
			}
			return null;
		}

		public virtual void Setup (IGridHandler gridHandler)
		{
			GridHandler = gridHandler;
		}

		public void Paint (sd.Graphics graphics, sd.Rectangle clipBounds, sd.Rectangle cellBounds, int rowIndex, swf.DataGridViewElementStates cellState, object value, object formattedValue, string errorText, swf.DataGridViewCellStyle cellStyle, swf.DataGridViewAdvancedBorderStyle advancedBorderStyle, ref swf.DataGridViewPaintParts paintParts)
		{
			if (GridHandler != null)
				GridHandler.Paint (this, graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, ref paintParts);
		}

		public int GetRowOffset (int rowIndex)
		{
			return GridHandler != null ? GridHandler.GetRowOffset(this, rowIndex) : 0;
		}

		public bool MouseClick (swf.MouseEventArgs e, int rowIndex)
		{
			return GridHandler != null && GridHandler.CellMouseClick(this, e, rowIndex);
		}
	}
}

