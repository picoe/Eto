using System;
using swf = System.Windows.Forms;
using Eto.Forms;
using System.Linq;
using System.Collections.Generic;
using sd = System.Drawing;
using Eto.Drawing;
using Eto.Platform.Windows.Drawing;

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
			Control.RowPostPaint += HandleRowPostPaint;
		}

		bool handledAutoSize = false;
		void HandleRowPostPaint (object sender, swf.DataGridViewRowPostPaintEventArgs e)
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

		class FormattingArgs : GridCellFormatEventArgs
		{
			public swf.DataGridViewCellFormattingEventArgs Args { get; private set; }

			public FormattingArgs (swf.DataGridViewCellFormattingEventArgs args, GridColumn column, object item, int row)
				: base(column, item, row)
			{
				this.Args = args;
			}

			Font font;
			public override Eto.Drawing.Font Font
			{
				get {
					if (font == null)
						font = new Font (Column.Generator, new FontHandler (Args.CellStyle.Font));
					return font;
				}
				set {
					font = value;
					if (font != null)
						Args.CellStyle.Font = ((FontHandler)font.Handler).Control;
					else
						Args.CellStyle.Font = null;
				}
			}

			public override Eto.Drawing.Color BackgroundColor {
				get { return Args.CellStyle.BackColor.ToEto (); }
				set { Args.CellStyle.BackColor = value.ToSD (); }
			}

			public override Eto.Drawing.Color ForegroundColor {
				get { return Args.CellStyle.ForeColor.ToEto (); }
				set { Args.CellStyle.ForeColor = value.ToSD (); }
			}
		}

		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case Grid.ColumnHeaderClickEvent:
				Control.ColumnHeaderMouseClick += (sender, e) => {
					Widget.OnColumnHeaderClick (new GridColumnEventArgs (Widget.Columns[e.ColumnIndex]));
				};
				break;
			case Grid.BeginCellEditEvent:
				Control.CellBeginEdit += (sender, e) => {
					var item = GetItemAtRow (e.RowIndex);
					var column = Widget.Columns [e.ColumnIndex];
					Widget.OnBeginCellEdit (new GridViewCellArgs (column, e.RowIndex, e.ColumnIndex, item));
				};
				break;
			case Grid.EndCellEditEvent:
				Control.CellEndEdit += (sender, e) => {
					var item = GetItemAtRow (e.RowIndex);
					var column = Widget.Columns [e.ColumnIndex];
					Widget.OnEndCellEdit (new GridViewCellArgs (column, e.RowIndex, e.ColumnIndex, item));
				};
				break;
			case Grid.SelectionChangedEvent:
				Control.SelectionChanged += delegate {
					Widget.OnSelectionChanged (EventArgs.Empty);
				};
				break;
			case Grid.CellFormattingEvent:
				Control.CellFormatting += (sender, e) => {
					var column = Widget.Columns[e.ColumnIndex];
					var item = GetItemAtRow (e.RowIndex);
					Widget.OnCellFormatting (new FormattingArgs(e, column, item, e.RowIndex));
				};
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
		}

		protected override void Initialize ()
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

		public int RowHeight
		{
			get { return Control.RowTemplate.Height; }
			set { 
				Control.RowTemplate.Height = value;
				foreach (swf.DataGridViewRow row in Control.Rows) {
					row.Height = value;
				}
			}
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

