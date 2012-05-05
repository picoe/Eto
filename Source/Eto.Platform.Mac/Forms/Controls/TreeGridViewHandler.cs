using System;
using MonoMac.AppKit;
using Eto.Forms;
using MonoMac.Foundation;
using System.Collections.Generic;
using Eto.Platform.Mac.Forms.Menu;
using System.Linq;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class TreeGridViewHandler : MacView<NSOutlineView, TreeGridView>, ITreeGridView, IDataViewHandler
	{
		ITreeGridStore<ITreeGridItem> store;
		NSOutlineView table;
		ContextMenu contextMenu;
		ColumnCollection columns;
		Dictionary<ITreeGridItem, EtoTreeItem> cachedItems = new Dictionary<ITreeGridItem, EtoTreeItem> ();
		Dictionary<int, EtoTreeItem> topitems = new Dictionary<int, EtoTreeItem> ();
		
		class EtoTreeItem : NSObject
		{
			Dictionary<int, EtoTreeItem> items;
			
			public EtoTreeItem ()
			{
			}
			
			public EtoTreeItem (IntPtr ptr)
				: base(ptr)
			{
			}
			
			public EtoTreeItem (EtoTreeItem value)
			{
				this.Item = value.Item;
				this.items = value.items;
			}

			public ITreeGridItem Item { get; set; }
			
			public Dictionary<int, EtoTreeItem> Items {
				get {
					if (items == null)
						items = new Dictionary<int, EtoTreeItem> ();
					return items;
				}
			}
			
		}
		
		class EtoOutlineDelegate : NSOutlineViewDelegate
		{
			public TreeGridViewHandler Handler { get; set; }
			
			public override void SelectionDidChange (NSNotification notification)
			{
				Handler.Widget.OnSelectionChanged (EventArgs.Empty);
			}
			
			public override void ItemDidCollapse (NSNotification notification)
			{
				var myitem = notification.UserInfo [(NSString)"NSObject"] as EtoTreeItem;
				if (myitem != null) {
					myitem.Item.Expanded = false;
					Handler.Widget.OnCollapsed (new TreeGridViewItemEventArgs (myitem.Item));
				}
			}
			
			public override bool ShouldExpandItem (NSOutlineView outlineView, NSObject item)
			{
				var myitem = item as EtoTreeItem;
				if (myitem != null) {
					var args = new TreeGridViewItemCancelEventArgs (myitem.Item);
					Handler.Widget.OnExpanding (args);
					return !args.Cancel;
				}
				return true;
			}
			
			public override bool ShouldCollapseItem (NSOutlineView outlineView, NSObject item)
			{
				var myitem = item as EtoTreeItem;
				if (myitem != null) {
					var args = new TreeGridViewItemCancelEventArgs (myitem.Item);
					Handler.Widget.OnCollapsing (args);
					return !args.Cancel;
				}
				return true;
			}
			
			public override void ItemDidExpand (NSNotification notification)
			{
				var myitem = notification.UserInfo [(NSString)"NSObject"] as EtoTreeItem;
				if (myitem != null) {
					myitem.Item.Expanded = true;
					Handler.Widget.OnExpanded (new TreeGridViewItemEventArgs (myitem.Item));
				}
			}
		}
			
		class EtoDataSource : NSOutlineViewDataSource
		{
			public TreeGridViewHandler Handler { get; set; }
			
			public override NSObject GetObjectValue (NSOutlineView outlineView, NSTableColumn forTableColumn, NSObject byItem)
			{
				var myitem = byItem as EtoTreeItem;
				var id = forTableColumn.Identifier as EtoDataColumnIdentifier;
				return id.Handler.GetObjectValue (myitem.Item);
			}
			
			public override void SetObjectValue (NSOutlineView outlineView, NSObject theObject, NSTableColumn tableColumn, NSObject item)
			{
				var myitem = item as EtoTreeItem;
				var id = tableColumn.Identifier as EtoDataColumnIdentifier;
				id.Handler.SetObjectValue (myitem.Item, theObject);
			}
			
			public override bool ItemExpandable (NSOutlineView outlineView, NSObject item)
			{
				var myitem = item as EtoTreeItem;
				if (myitem == null)
					return false;
				return myitem.Item.Expandable;
			}
			
			public override NSObject GetChild (NSOutlineView outlineView, int childIndex, NSObject ofItem)
			{
				Dictionary<int, EtoTreeItem> items;
				var myitem = ofItem as EtoTreeItem;
				if (ofItem == null)
					items = Handler.topitems;
				else
					items = myitem.Items;
				
				EtoTreeItem item;
				if (!items.TryGetValue (childIndex, out item)) {
					var parentItem = myitem != null ? (ITreeGridStore<ITreeGridItem>)myitem.Item : Handler.store;
					item = new EtoTreeItem{ Item = parentItem [childIndex] };
					Handler.cachedItems.Add (item.Item, item);
					items.Add (childIndex, item);
				}
				return item;
			}
			
			public override int GetChildrenCount (NSOutlineView outlineView, NSObject item)
			{
				if (Handler.store == null)
					return 0;
				
				if (item == null)
					return Handler.store.Count;
				
				var myitem = item as EtoTreeItem;
				return ((ITreeGridStore<ITreeGridItem>)myitem.Item).Count;
			}
		}
		
		public class EtoOutlineView : NSOutlineView, IMacControl
		{
			public object Handler { get; set; }
		}
		
		public override object EventObject {
			get { return table; }
		}
		
		public override NSView ContainerControl {
			get { return ScrollView; }
		}
		
		public NSScrollView ScrollView {
			get;
			private set;
		}
		
		public TreeGridViewHandler ()
		{
			table = new EtoOutlineView { 
				Handler = this,
				Delegate = new EtoOutlineDelegate{ Handler = this },
				DataSource = new EtoDataSource{ Handler = this },
			//HeaderView = null,
			//AutoresizesOutlineColumn = true,
			//AllowsColumnResizing = false,
				AllowsColumnReordering = false,
				FocusRingType = NSFocusRingType.None,
				ColumnAutoresizingStyle = NSTableViewColumnAutoresizingStyle.LastColumnOnly
			};
			
			ScrollView = new NSScrollView {
				HasVerticalScroller = true,
				HasHorizontalScroller = true,
				AutohidesScrollers = true,
				BorderType = NSBorderType.BezelBorder,
				DocumentView = table
			};
			Control = table;
		}

		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case TreeGridView.SelectionChangedEvent:
			case TreeGridView.ExpandedEvent:
			case TreeGridView.ExpandingEvent:
			case TreeGridView.CollapsedEvent:
			case TreeGridView.CollapsingEvent:
				// handled in delegate
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
		}
		
		public override void OnLoadComplete (EventArgs e)
		{
			base.OnLoadComplete (e);
			
			int i = 0;
			foreach (var col in this.Widget.Columns) {
				((GridColumnHandler)col.Handler).Loaded (this, i++, Control);
			}
		}
		
		
		class ColumnCollection : EnumerableChangedHandler<GridColumn, GridColumnCollection>
		{
			public TreeGridViewHandler Handler { get; set; }
			
			public override void AddItem (GridColumn item)
			{
				var colhandler = (GridColumnHandler)item.Handler;
				Handler.table.AddColumn (colhandler.Control);
				colhandler.Setup (Handler.table.ColumnCount - 1);
				
				if (Handler.table.OutlineTableColumn == null) {
					Handler.table.OutlineTableColumn = Handler.table.TableColumns () [0];
				}
			}

			public override void InsertItem (int index, GridColumn item)
			{
				var outline = Handler.table;
				var columns = new List<NSTableColumn> (outline.TableColumns ());
				if (index == 0)
					outline.OutlineTableColumn = null;
				for (int i = index; i < columns.Count; i++) {
					outline.RemoveColumn (columns [i]);
				}
				var colhandler = (GridColumnHandler)item.Handler;
				columns.Insert (index, colhandler.Control);
				outline.AddColumn (colhandler.Control);
				colhandler.Setup (index);
				for (int i = index + 1; i < columns.Count; i++) {
					var col = columns [i];
					var id = col.Identifier as EtoDataColumnIdentifier;
					id.Handler.Setup (i);
					outline.AddColumn (col);
				}
				if (index == 0) {
					outline.OutlineTableColumn = columns [0];
				}
			}

			public override void RemoveItem (int index)
			{
				var outline = Handler.table;
				var columns = new List<NSTableColumn> (outline.TableColumns ());
				if (index == 0)
					outline.OutlineTableColumn = null;
				for (int i = index; i < columns.Count; i++) {
					outline.RemoveColumn (columns [i]);
				}
				columns.RemoveAt (index);
				for (int i = index; i < columns.Count; i++) {
					var col = columns [i];
					var id = col.Identifier as EtoDataColumnIdentifier;
					id.Handler.Setup (i);
					outline.AddColumn (col);
				}
			}

			public override void RemoveAllItems ()
			{
				Handler.table.OutlineTableColumn = null;
				foreach (var col in Handler.table.TableColumns ())
					Handler.table.RemoveColumn (col);
			}

		}
		
		public override void Initialize ()
		{
			base.Initialize ();
			columns = new ColumnCollection{ Handler = this };
			columns.Register (Widget.Columns);
		}
		
		public ITreeGridStore<ITreeGridItem> DataStore {
			get { return store; }
			set {
				store = value;
				topitems.Clear ();
				cachedItems.Clear ();
				table.ReloadData ();
				ExpandItems (null);
			}
		}
		
		public ITreeGridItem SelectedItem {
			get {
				var row = table.SelectedRow;
				if (row == -1)
					return null;
				var myitem = table.ItemAtRow (row) as EtoTreeItem;
				return myitem.Item;
			}
			set {
				if (value == null)
					table.SelectRow (-1, false);
				else {
					
					EtoTreeItem myitem;
					if (cachedItems.TryGetValue (value, out myitem)) {
						var row = table.RowForItem (myitem);
						if (row >= 0)
							table.SelectRow (row, false);
					}
				}
			}
		}
		
		public override bool Enabled {
			get { return table.Enabled; }
			set { table.Enabled = value; }
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
		
		void ExpandItems (NSObject parent)
		{
			var ds = table.DataSource;
			var count = ds.GetChildrenCount (table, parent);
			for (int i=0; i<count; i++) {
				
				var item = ds.GetChild (table, i, parent) as EtoTreeItem;
				if (item != null && item.Item.Expanded) {
					table.ExpandItem (item);
					ExpandItems (item);
				}
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

		public NSTableView Table {
			get { return table; }
		}
		
		public object GetItem (int row)
		{
			var item = table.ItemAtRow (row) as EtoTreeItem;
			if (item != null)
				return item.Item;
			else
				return null;
		}
		
		public bool AllowColumnReordering {
			get { return table.AllowsColumnReordering; }
			set { table.AllowsColumnReordering = value; }
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

	}
}

