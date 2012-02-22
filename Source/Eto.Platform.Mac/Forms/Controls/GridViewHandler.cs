using System;
using MonoMac.AppKit;
using Eto.Forms;
using System.Collections.Generic;
using MonoMac.Foundation;
using Eto.Platform.Mac.Forms.Menu;
using System.Linq;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class GridViewHandler : MacView<NSScrollView, GridView>, IGridView
	{
		IGridStore store;
		NSTableView table;
		ContextMenu contextMenu;
		
		public NSTableView Table {
			get { return table; }
		}
		
		class EtoTableViewDataSource : NSTableViewDataSource
		{
			public GridViewHandler Handler { get; set; }
			
			public override int GetRowCount (NSTableView tableView)
			{
				return (Handler.store != null) ? Handler.store.Count : 0;
			}

			public override NSObject GetObjectValue (NSTableView tableView, NSTableColumn tableColumn, int row)
			{
				var item = Handler.store.GetItem (row);
				var id = tableColumn.Identifier as EtoGridColumnIdentifier;
				if (item != null && id != null) {
					var val = item.GetValue (id.Column);
					return id.Handler.GetObjectValue (val);
				}
				return null;
			}

			public override void SetObjectValue (NSTableView tableView, NSObject theObject, NSTableColumn tableColumn, int row)
			{
				var item = Handler.store.GetItem (row);
				var id = tableColumn.Identifier as EtoGridColumnIdentifier;
				if (item != null && id != null) {
					var val = id.Handler.SetObjectValue (theObject);
					item.SetValue (id.Column, val);
					
					Handler.Widget.OnEndCellEdit (new GridViewCellArgs (id.Handler.Widget, row, id.Column, item));
				}
			}
		}
		
		class EtoTableDelegate : NSTableViewDelegate
		{
			public GridViewHandler Handler { get; set; }

			public override bool ShouldEditTableColumn (NSTableView tableView, NSTableColumn tableColumn, int row)
			{
				var id = tableColumn.Identifier as EtoGridColumnIdentifier;
				var item = Handler.store.GetItem (row);
				var args = new GridViewCellArgs (id.Handler.Widget, row, id.Handler.Column, item);
				Handler.Widget.OnBeginCellEdit (args);
				return true;
			}
		}
		
		public GridViewHandler ()
		{
			table = new NSTableView {
				FocusRingType = NSFocusRingType.None,
				DataSource = new EtoTableViewDataSource { Handler = this },
				Delegate = new EtoTableDelegate { Handler = this }
			};

			Control = new NSScrollView {
				HasVerticalScroller = true,
				HasHorizontalScroller = true,
				AutohidesScrollers = true,
				BorderType = NSBorderType.BezelBorder,
				DocumentView = table
			};
		}

		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case GridView.BeginCellEditEvent:
				// handled by delegate
				/* following should work, but internal delegate to trigger event does not work
				table.ShouldEditTableColumn = (tableView, tableColumn, row) => {
					var id = tableColumn.Identifier as EtoGridColumnIdentifier;
					var item = store.GetItem (row);
					var args = new GridViewCellArgs(id.Handler.Widget, row, id.Handler.Column, item);
					this.Widget.OnBeginCellEdit (args);
					return true;
				};*/
				break;
			case GridView.EndCellEditEvent:
				// handled after object value is set
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
		}
		
		public void InsertColumn (int index, GridColumn column)
		{
			var colhandler = ((GridColumnHandler)column.Handler);
			if (index == -1 || index == table.ColumnCount) {
				colhandler.Setup (this, index);
				table.AddColumn (colhandler.Control);
			} else {
				var columns = new List<NSTableColumn> (table.TableColumns ());
				for (int i = 0; i < index; i++) {
					table.RemoveColumn (columns [i]);
				}
				columns.Insert (index, ((GridColumnHandler)column.Handler).Control);
				for (int i = index; i < columns.Count; i++) {
					var col = columns [i];
					var id = col.Identifier as EtoGridColumnIdentifier;
					if (id != null)
						id.Handler.Setup (this, i);
					table.AddColumn (col);
				}
			}
		}
		
		public override void OnLoadComplete (EventArgs e)
		{
			base.OnLoadComplete (e);
			
			int i = 0;
			foreach (var col in this.Widget.Columns) {
				((GridColumnHandler)col.Handler).Loaded (this, i++);
			}
		}

		public void RemoveColumn (int index, GridColumn column)
		{
			table.RemoveColumn (((GridColumnHandler)column.Handler).Control);
		}

		public void ClearColumns ()
		{
			foreach (var col in table.TableColumns ())
				table.RemoveColumn (col);
		}

		public bool ShowHeader {
			get {
				return table.HeaderView != null;
			}
			set {
				if (value && table.HeaderView == null) {
					table.HeaderView = new NSTableHeaderView ();
				} else if (!value && table.HeaderView != null) {
					table.HeaderView = null;
				}
			}
		}

		public IGridStore DataStore {
			get { return store; }
			set {
				store = value;
				table.ReloadData ();
			}
		}
		
		public override bool Enabled {
			get { return table.Enabled; }
			set { table.Enabled = value; }
		}
		
		public bool AllowColumnReordering {
			get { return table.AllowsColumnReordering; }
			set { table.AllowsColumnReordering = value; }
		}
		
		public ContextMenu ContextMenu {
			get { return contextMenu; }
			set {
				contextMenu = value;
				if (contextMenu != null)
					table.Menu = ((ContextMenuHandler)contextMenu.Handler).Control;
				else
					table.Menu = null;
			}
		}
	}
}

