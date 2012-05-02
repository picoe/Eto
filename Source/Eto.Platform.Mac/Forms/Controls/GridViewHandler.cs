using System;
using MonoMac.AppKit;
using Eto.Forms;
using System.Collections.Generic;
using MonoMac.Foundation;
using Eto.Platform.Mac.Forms.Menu;
using System.Linq;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class GridViewHandler : MacView<NSScrollView, GridView>, IGridView, IDataViewHandler
	{
		CollectionHandler collection;
		ColumnCollection columns;
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
				return (Handler.collection != null && Handler.collection.DataStore != null) ? Handler.collection.DataStore.Count : 0;
			}

			public override NSObject GetObjectValue (NSTableView tableView, NSTableColumn tableColumn, int row)
			{
				var item = Handler.collection.DataStore [row];
				var id = tableColumn.Identifier as EtoDataColumnIdentifier;
				if (id != null) {
					return id.Handler.GetObjectValue (item);
				}
				return null;
			}

			public override void SetObjectValue (NSTableView tableView, NSObject theObject, NSTableColumn tableColumn, int row)
			{
				var item = Handler.collection.DataStore [row];
				var id = tableColumn.Identifier as EtoDataColumnIdentifier;
				if (id != null) {
					id.Handler.SetObjectValue (item, theObject);
					
					Handler.Widget.OnEndCellEdit (new GridViewCellArgs ((GridColumn)id.Handler.Widget, row, id.Column, item));
				}
			}
		}
		
		class EtoTableDelegate : NSTableViewDelegate
		{
			public GridViewHandler Handler { get; set; }

			public override bool ShouldEditTableColumn (NSTableView tableView, NSTableColumn tableColumn, int row)
			{
				var id = tableColumn.Identifier as EtoDataColumnIdentifier;
				var item = Handler.collection.DataStore [row];
				var args = new GridViewCellArgs ((GridColumn)id.Handler.Widget, row, id.Column, item);
				Handler.Widget.OnBeginCellEdit (args);
				return true;
			}
			
			public override void SelectionDidChange (NSNotification notification)
			{
				Handler.Widget.OnSelectionChanged (EventArgs.Empty);
			}
		}

		class ColumnCollection : EnumerableChangedHandler<GridColumn, GridColumnCollection>
		{
			public GridViewHandler Handler { get; set; }

			public override void AddItem (GridColumn item)
			{
				var colhandler = (GridColumnHandler)item.Handler;
				Handler.table.AddColumn (colhandler.Control);
				colhandler.Setup (Handler.table.ColumnCount - 1);
			}

			public override void InsertItem (int index, GridColumn item)
			{
				var outline = Handler.table;
				var columns = new List<NSTableColumn> (outline.TableColumns ());
				for (int i = index; i < columns.Count; i++) {
					outline.RemoveColumn (columns[i]);
				}
				var colhandler = (GridColumnHandler)item.Handler;
				columns.Insert (index, colhandler.Control);
				outline.AddColumn (colhandler.Control);
				colhandler.Setup (index);
				for (int i = index + 1; i < columns.Count; i++) {
					var col = columns[i];
					var id = col.Identifier as EtoDataColumnIdentifier;
					id.Handler.Setup (i);
					outline.AddColumn (col);
				}
			}

			public override void RemoveItem (int index)
			{
				var table = Handler.table;
				var columns = new List<NSTableColumn> (table.TableColumns ());
				for (int i = index; i < columns.Count; i++) {
					table.RemoveColumn (columns[i]);
				}
				columns.RemoveAt (index);
				for (int i = index; i < columns.Count; i++) {
					var col = columns[i];
					var id = col.Identifier as EtoDataColumnIdentifier;
					id.Handler.Setup (i);
					table.AddColumn (col);
				}
			}

			public override void RemoveAllItems ()
			{
				foreach (var col in Handler.table.TableColumns ())
					Handler.table.RemoveColumn (col);
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
			case GridView.SelectionChangedEvent:
				/* handled by delegate, for now
				table.SelectionDidChange += delegate {
					Widget.OnSelectionChanged (EventArgs.Empty);
				};*/
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
		
		public override void OnLoadComplete (EventArgs e)
		{
			base.OnLoadComplete (e);
			
			int i = 0;
			foreach (var col in this.Widget.Columns) {
				((GridColumnHandler)col.Handler).Loaded (this, i++, null);
			}
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

		class CollectionHandler : DataStoreChangedHandler<IGridItem, IGridStore>
		{
			public GridViewHandler Handler { get; set; }

			public override int IndexOf (IGridItem item)
			{
				return -1; // not needed
			}
			
			public override void AddRange (IEnumerable<IGridItem> items)
			{
				Handler.table.ReloadData ();
			}

			public override void AddItem (IGridItem item)
			{
				Handler.table.ReloadData ();
			}

			public override void InsertItem (int index, IGridItem item)
			{
				Handler.table.ReloadData ();
			}

			public override void RemoveItem (int index)
			{
				Handler.table.ReloadData ();
			}

			public override void RemoveAllItems ()
			{
				Handler.table.ReloadData ();
			}
		}

		public IGridStore DataStore {
			get { return collection != null ? collection.DataStore : null; }
			set {
				if (collection != null)
					collection.Unregister ();
				collection = new CollectionHandler{ Handler = this };
				collection.Register (value);
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

		public bool AllowMultipleSelection {
			get { return table.AllowsMultipleSelection; }
			set { table.AllowsMultipleSelection = value; }
		}

		public IEnumerable<int> SelectedRows {
			get { return table.SelectedRows.Select (r => (int)r); }
		}

		public void SelectAll ()
		{
			table.SelectAll (table);
		}

		public void SelectRow (int row)
		{
			table.SelectRow (row, false);
		}

		public void UnselectRow (int row)
		{
			table.DeselectRow (row);
		}

		public void UnselectAll ()
		{
			table.DeselectAll (table);
		}

		public object GetItem (int row)
		{
			return collection.DataStore [row];
		}
	}
}

