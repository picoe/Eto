using System;
using swf = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows.Forms.Controls
{
	public class GridViewHandler : WindowsControl<swf.DataGridView, GridView>, IGridView
	{
		IGridStore store;
		
		public GridViewHandler ()
		{
			Control = new swf.DataGridView();
			Control.VirtualMode = true;
			Control.RowHeadersVisible = false;
			Control.AllowUserToAddRows = false;
			Control.AllowUserToResizeRows = false;
			Control.CellValueNeeded += (sender, e) => {
				var item = store.GetItem (e.RowIndex);
				var col = Widget.Columns[e.ColumnIndex].Handler as GridColumnHandler;
				if (item != null && col != null)
					e.Value = col.GetCellValue(item.GetValue (e.ColumnIndex));
			};

			Control.CellValuePushed += (sender, e) => {
				var item = store.GetItem(e.RowIndex);
				var col = Widget.Columns[e.ColumnIndex].Handler as GridColumnHandler;
				if (item != null && col != null)
					item.SetValue (e.ColumnIndex, col.GetItemValue(e.Value));
			};
		}

		public void InsertColumn (int index, GridColumn column)
		{
			var colHandler = ((GridColumnHandler)column.Handler);
			if (index >= 0 && this.Control.Columns.Count != 0)
				this.Control.Columns.Insert (index, colHandler.Control);
			else
				this.Control.Columns.Add (colHandler.Control);
		}

		public void RemoveColumn (int index, GridColumn column)
		{
			var colHandler = ((GridColumnHandler)column.Handler);
			if (index >= 0)
				this.Control.Columns.RemoveAt(index);
			else
				this.Control.Columns.Remove (colHandler.Control);
		}

		public void ClearColumns ()
		{
			this.Control.Columns.Clear ();
		}

		public bool ShowHeader {
			get { return this.Control.ColumnHeadersVisible; }
			set { this.Control.ColumnHeadersVisible = value; }
		}

		public bool AllowColumnReordering {
			get { return this.Control.AllowUserToOrderColumns; }
			set { this.Control.AllowUserToOrderColumns = value; }
		}

		public IGridStore DataStore {
			get { return store; }
			set {
				store = value;
				if (store != null)
					Control.RowCount = store.Count;
				else
					Control.RowCount = 0;
			}
		}
	}
}

