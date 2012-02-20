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
			Control = new swf.DataGridView {
				VirtualMode = true,
				RowHeadersVisible = false,
				AllowUserToAddRows = false,
				AllowUserToResizeRows = false,
				ColumnHeadersHeightSizeMode = swf.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
			};
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

		public override void OnLoadComplete (EventArgs e)
		{
			base.OnLoadComplete (e);

			// user can resize auto-sizing columns
			foreach (swf.DataGridViewColumn col in Control.Columns) {
				var width = col.Width;
				col.AutoSizeMode = swf.DataGridViewAutoSizeColumnMode.None;
				col.Width = width;
			}
		}

		public override void AttachEvent (string handler)
		{
			switch (handler) {
				case GridView.BeginCellEditEvent:
					Control.CellBeginEdit += (sender, e) => {
						var item = store.GetItem(e.RowIndex);
						var column = Widget.Columns[e.ColumnIndex];
						Widget.OnBeginCellEdit (new GridViewCellArgs (column, e.RowIndex, e.ColumnIndex, item));
					};
					break;
				case GridView.EndCellEditEvent:
					Control.CellEndEdit += (sender, e) => {
						var item = store.GetItem (e.RowIndex);
						var column = Widget.Columns[e.ColumnIndex];
						Widget.OnEndCellEdit (new GridViewCellArgs (column, e.RowIndex, e.ColumnIndex, item));
					};
					break;
				default:
					base.AttachEvent (handler);
					break;
			}
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

