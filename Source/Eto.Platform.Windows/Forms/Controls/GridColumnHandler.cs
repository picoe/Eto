using System;
using swf = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows.Forms.Controls
{
	public class GridColumnHandler : WidgetHandler<swf.DataGridViewColumn, GridColumn>, IGridColumn
	{
		Cell dataCell;
		
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
			get { return this.Control.AutoSizeMode == System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells; }
			set { this.Control.AutoSizeMode = (value) ? swf.DataGridViewAutoSizeColumnMode.DisplayedCells : swf.DataGridViewAutoSizeColumnMode.None; }
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
	}
}

