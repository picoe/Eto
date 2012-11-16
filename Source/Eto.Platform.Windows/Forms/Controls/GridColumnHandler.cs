using System;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Forms;

namespace Eto.Platform.Windows.Forms.Controls
{

	public class GridColumnHandler : WidgetHandler<swf.DataGridViewColumn, GridColumn>, IGridColumn, ICellConfigHandler
	{
		Cell dataCell;
		bool autosize;

		public IGridHandler GridHandler { get; private set; }

		public GridColumnHandler ()
		{
			Control = new swf.DataGridViewColumn();
		}
		
		public override void Initialize ()
		{
			base.Initialize ();
			DataCell = new TextBoxCell(Widget.Generator);
			Editable = false;
			AutoSize = true;
			Resizable = true;
		}

		public string HeaderText {
			get { return this.Control.HeaderText; }
			set { this.Control.HeaderText = value; }
		}

		public bool Resizable {
			get { return this.Control.Resizable == swf.DataGridViewTriState.True; }
			set { this.Control.Resizable = value ? swf.DataGridViewTriState.True : swf.DataGridViewTriState.False; }
		}

		public bool Sortable {
			get { return this.Control.SortMode == swf.DataGridViewColumnSortMode.Programmatic; }
			set { this.Control.SortMode = (value) ? swf.DataGridViewColumnSortMode.Programmatic : swf.DataGridViewColumnSortMode.NotSortable; }
		}

		public bool AutoSize {
			get { return autosize; }
			set {
				autosize = value;
				this.Control.AutoSizeMode = (value) ? swf.DataGridViewAutoSizeColumnMode.NotSet : swf.DataGridViewAutoSizeColumnMode.None; 
			}
		}

		public int Width {
			get { return this.Control.Width; }
			set { this.Control.Width = value; }
		}

		public Cell DataCell {
			get { return dataCell; }
			set {
				dataCell = value;
				if (dataCell != null) {
					var cellHandler = (ICellHandler)dataCell.Handler;
					cellHandler.CellConfig = this;
					this.Control.CellTemplate = cellHandler.Control;
				}
				else
					this.Control.CellTemplate = null;
			}
		}

		public bool Editable {
			get { return !this.Control.ReadOnly; }
			set { this.Control.ReadOnly = !value; }
		}

		public bool Visible {
			get { return this.Control.Visible; }
			set { this.Control.Visible = value; }
		}

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
			this.GridHandler = gridHandler;
		}

		public void Paint (sd.Graphics graphics, sd.Rectangle clipBounds, sd.Rectangle cellBounds, int rowIndex, swf.DataGridViewElementStates cellState, object value, object formattedValue, string errorText, swf.DataGridViewCellStyle cellStyle, swf.DataGridViewAdvancedBorderStyle advancedBorderStyle, ref swf.DataGridViewPaintParts paintParts)
		{
			if (this.GridHandler != null)
				this.GridHandler.Paint (this, graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, ref paintParts);
		}

		public int GetRowOffset (int rowIndex)
		{
			if (this.GridHandler != null)
				return this.GridHandler.GetRowOffset (this, rowIndex);
			else
				return 0;
		}

		public bool MouseClick (swf.MouseEventArgs e, int rowIndex)
		{
			if (this.GridHandler != null)
				return this.GridHandler.CellMouseClick (this, e, rowIndex);
			else
				return false;
		}
	}
}

