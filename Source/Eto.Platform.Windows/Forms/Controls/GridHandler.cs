using System;
using swf = System.Windows.Forms;
using Eto.Forms;
using System.Linq;
using System.Collections.Generic;
using sd = System.Drawing;

namespace Eto.Platform.Windows.Forms.Controls
{
	public interface IGridHandler
	{
		void Paint (GridColumnHandler column, sd.Graphics graphics, sd.Rectangle clipBounds, sd.Rectangle cellBounds, int rowIndex, swf.DataGridViewElementStates cellState, object value, object formattedValue, string errorText, swf.DataGridViewCellStyle cellStyle, swf.DataGridViewAdvancedBorderStyle advancedBorderStyle, ref swf.DataGridViewPaintParts paintParts);
		int GetRowOffset (GridColumnHandler column, int rowIndex);
		bool CellMouseClick (GridColumnHandler column, swf.MouseEventArgs e, int rowIndex);
	}

	public abstract class GridHandler<W> : WindowsControl<swf.DataGridView, W>, IGrid, IGridHandler
		where W: Grid
	{
		ContextMenu contextMenu;
		ColumnCollection columns;

		protected abstract IGridItem GetItemAtRow (int row);

		public GridHandler ()
		{
			Control = new swf.DataGridView {
				VirtualMode = true,
				MultiSelect = false,
				SelectionMode = swf.DataGridViewSelectionMode.FullRowSelect,
				RowHeadersVisible = false,
				AllowUserToAddRows = false,
				AllowUserToResizeRows = false,
				AutoSizeColumnsMode = swf.DataGridViewAutoSizeColumnsMode.DisplayedCells,
				ColumnHeadersHeightSizeMode = swf.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
			};
			Control.CellValueNeeded += (sender, e) => {
				var item = GetItemAtRow(e.RowIndex);
				var col = Widget.Columns [e.ColumnIndex].Handler as GridColumnHandler;
				if (item != null && col != null)
					e.Value = col.GetCellValue (item);
			};

			Control.CellValuePushed += (sender, e) => {
				var item = GetItemAtRow(e.RowIndex);
				var col = Widget.Columns [e.ColumnIndex].Handler as GridColumnHandler;
				if (item != null && col != null)
					col.SetCellValue (item, e.Value);
			};
			Control.RowPostPaint += new swf.DataGridViewRowPostPaintEventHandler (Control_RowPostPaint);
		}

		bool handledAutoSize = false;
		void Control_RowPostPaint (object sender, swf.DataGridViewRowPostPaintEventArgs e)
		{
			if (handledAutoSize) return;

			handledAutoSize = true;
			int colNum = 0;
			foreach (var col in Widget.Columns) {
				var colHandler = col.Handler as GridColumnHandler;
				if (col.AutoSize) {
					Control.AutoResizeColumn (colNum, colHandler.Control.InheritedAutoSizeMode);
					var width = col.Width;
					colHandler.Control.AutoSizeMode = swf.DataGridViewAutoSizeColumnMode.None;
					col.Width = width;
				}
				colNum++;
			}
		}

		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case GridView.BeginCellEditEvent:
				Control.CellBeginEdit += (sender, e) => {
					var item = GetItemAtRow (e.RowIndex);
					var column = Widget.Columns [e.ColumnIndex];
					Widget.OnBeginCellEdit (new GridViewCellArgs (column, e.RowIndex, e.ColumnIndex, item));
				};
				break;
			case GridView.EndCellEditEvent:
				Control.CellEndEdit += (sender, e) => {
					var item = GetItemAtRow (e.RowIndex);
					var column = Widget.Columns [e.ColumnIndex];
					Widget.OnEndCellEdit (new GridViewCellArgs (column, e.RowIndex, e.ColumnIndex, item));
				};
				break;
			case GridView.SelectionChangedEvent:
				Control.SelectionChanged += delegate {
					Widget.OnSelectionChanged (EventArgs.Empty);
				};
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
		}

		public override void Initialize ()
		{
			base.Initialize ();
			columns = new ColumnCollection { Handler = this };
			columns.Register (Widget.Columns);
		}

		class ColumnCollection : EnumerableChangedHandler<GridColumn, GridColumnCollection>
		{
			public GridHandler<W> Handler { get; set; }

			public override void AddItem (GridColumn item)
			{
				var colhandler = (GridColumnHandler)item.Handler;
				colhandler.Setup (Handler);
				Handler.Control.Columns.Add (colhandler.Control);
			}

			public override void InsertItem (int index, GridColumn item)
			{
				var colhandler = (GridColumnHandler)item.Handler;
				colhandler.Setup (Handler);
				Handler.Control.Columns.Insert (index, colhandler.Control);
			}

			public override void RemoveItem (int index)
			{
				Handler.Control.Columns.RemoveAt (index);
			}

			public override void RemoveAllItems ()
			{
				Handler.Control.Columns.Clear ();
			}
		}

		public bool ShowHeader {
			get { return this.Control.ColumnHeadersVisible; }
			set { this.Control.ColumnHeadersVisible = value; }
		}

		public bool AllowColumnReordering {
			get { return this.Control.AllowUserToOrderColumns; }
			set { this.Control.AllowUserToOrderColumns = value; }
		}
		
		public ContextMenu ContextMenu {
			get { return contextMenu; }
			set {
				contextMenu = value;
				if (contextMenu != null)
					this.Control.ContextMenuStrip = ((ContextMenuHandler)contextMenu.Handler).Control;
				else
					this.Control.ContextMenuStrip = null;
			}
		}

		public bool AllowMultipleSelection {
			get { return Control.MultiSelect; }
			set { Control.MultiSelect = value; }
		}

		public IEnumerable<int> SelectedRows {
			get { return Control.SelectedRows.OfType<swf.DataGridViewRow> ().Select (r => r.Index); }
		}

		public void SelectAll ()
		{
			Control.SelectAll ();
		}

		public void SelectRow (int row)
		{
			Control.Rows [row].Selected = true;
		}

		public void UnselectRow (int row)
		{
			Control.Rows [row].Selected = false;
		}

		public void UnselectAll ()
		{
			Control.ClearSelection ();
		}

		public virtual void Paint (GridColumnHandler column, System.Drawing.Graphics graphics, System.Drawing.Rectangle clipBounds, System.Drawing.Rectangle cellBounds, int rowIndex, swf.DataGridViewElementStates cellState, object value, object formattedValue, string errorText, swf.DataGridViewCellStyle cellStyle, swf.DataGridViewAdvancedBorderStyle advancedBorderStyle, ref swf.DataGridViewPaintParts paintParts)
		{
		}

		public virtual int GetRowOffset (GridColumnHandler column, int rowIndex)
		{
			return 0;
		}

		public virtual bool CellMouseClick (GridColumnHandler column, swf.MouseEventArgs e, int rowIndex)
		{
			return false;
		}
	}
}

