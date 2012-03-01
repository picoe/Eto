using System;
using MonoMac.AppKit;
using Eto.Forms;
using MonoMac.Foundation;
using System.Collections.Generic;
using Eto.Platform.Mac.Forms.Menu;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class TreeViewHandler : MacView<NSScrollView, TreeView>, ITreeView, IDataViewHandler
	{
		ITreeStore store;
		NSOutlineView outline;
		ContextMenu contextMenu;
		ColumnCollection columns;
		Dictionary<ITreeItem, EtoTreeItem> cachedItems = new Dictionary<ITreeItem, EtoTreeItem> ();
		Dictionary<int, EtoTreeItem> topitems = new Dictionary<int, EtoTreeItem> ();
		
		class EtoTreeItem : NSObject
		{
			Dictionary<int, EtoTreeItem> items;
			ITreeItem item;
			
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

			public ITreeItem Item { get; set; }
			
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
			public TreeViewHandler Handler { get; set; }
			
			public override void SelectionDidChange (NSNotification notification)
			{
				Handler.Widget.OnSelectionChanged (EventArgs.Empty);
			}
			
			public override void ItemDidCollapse (NSNotification notification)
			{
				var myitem = notification.UserInfo [(NSString)"NSObject"] as EtoTreeItem;
				if (myitem != null) {
					myitem.Item.Expanded = false;
				}
			}
			
			public override void ItemDidExpand (NSNotification notification)
			{
				var myitem = notification.UserInfo [(NSString)"NSObject"] as EtoTreeItem;
				if (myitem != null) {
					myitem.Item.Expanded = true;
				}
			}
		}
			
		class EtoDataSource : NSOutlineViewDataSource
		{
			public TreeViewHandler Handler { get; set; }
			
			public override NSObject GetObjectValue (NSOutlineView outlineView, NSTableColumn forTableColumn, NSObject byItem)
			{
				var myitem = byItem as EtoTreeItem;
				var id = forTableColumn.Identifier as EtoDataColumnIdentifier;
				var val = myitem.Item.GetValue (id.Column);
				return id.Handler.GetObjectValue(val);
			}
			
			public override void SetObjectValue (NSOutlineView outlineView, NSObject theObject, NSTableColumn tableColumn, NSObject item)
			{
				var myitem = item as EtoTreeItem;
				var id = tableColumn.Identifier as EtoDataColumnIdentifier;
				var val = id.Handler.SetObjectValue(theObject);
				myitem.Item.SetValue (id.Column, val);
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
					var parentItem = myitem != null ? myitem.Item : Handler.store;
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
				return myitem.Item.Count;
			}
		}
		
		public class EtoOutlineView : NSOutlineView, IMacControl
		{
			public object Handler { get; set; }
		}
		
		public override object EventObject {
			get { return outline; }
		}
		
		public TreeViewHandler ()
		{
			outline = new EtoOutlineView { Handler = this };
			outline.Delegate = new EtoOutlineDelegate{ Handler = this };
			outline.DataSource = new EtoDataSource{ Handler = this };
			//outline.HeaderView = null;
			outline.AutoresizesOutlineColumn = true;
			//outline.AllowsColumnResizing = false;
			outline.AllowsColumnReordering = false;
			//outline.ColumnAutoresizingStyle = NSTableViewColumnAutoresizingStyle.FirstColumnOnly;
			
			Control = new NSScrollView ();
			Control.HasVerticalScroller = true;
			Control.HasHorizontalScroller = true;
			Control.AutohidesScrollers = true;
			Control.BorderType = NSBorderType.BezelBorder;
			Control.DocumentView = outline;
		}
		
		public override void OnLoadComplete (EventArgs e)
		{
			base.OnLoadComplete (e);
			
			int i = 0;
			foreach (var col in this.Widget.Columns) {
				((TreeColumnHandler)col.Handler).Loaded (this, i++);
			}
		}
		
		
		class ColumnCollection : EnumerableChangedHandler<TreeColumn, TreeColumnCollection>
		{
			public TreeViewHandler Handler { get; set; }
			
			public override void AddItem (TreeColumn item)
			{
				var colhandler = ((TreeColumnHandler)item.Handler);
				Handler.outline.AddColumn (colhandler.Control);
				colhandler.Setup (Handler.outline.ColumnCount - 1);
				
				if (Handler.outline.OutlineTableColumn == null) {
					Handler.outline.OutlineTableColumn = Handler.outline.TableColumns()[0];
				}
			}

			public override void InsertItem (int index, TreeColumn item)
			{
				var outline = Handler.outline;
				var columns = new List<NSTableColumn> (outline.TableColumns ());
				if (index == 0)
					outline.OutlineTableColumn = null;
				for (int i = index; i < columns.Count; i++) {
					outline.RemoveColumn (columns [i]);
				}
				var colhandler = (TreeColumnHandler)item.Handler;
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
					outline.OutlineTableColumn = columns[0];
				}
			}

			public override void RemoveItem (int index)
			{
				var outline = Handler.outline;
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
				Handler.outline.OutlineTableColumn = null;
				foreach (var col in Handler.outline.TableColumns ())
					Handler.outline.RemoveColumn (col);
			}

		}
		
		public override void Initialize ()
		{
			base.Initialize ();
			columns = new ColumnCollection{ Handler = this };
			columns.Register (Widget.Columns);
		}
		
		public ITreeStore DataStore {
			get { return store; }
			set {
				store = value;
				topitems.Clear ();
				cachedItems.Clear ();
				outline.ReloadData ();
				ExpandItems (null);
			}
		}
		
		public ITreeItem SelectedItem {
			get {
				var row = outline.SelectedRow;
				if (row == -1)
					return null;
				var myitem = outline.ItemAtRow (row) as EtoTreeItem;
				return myitem.Item;
			}
			set {
				if (value == null)
					outline.SelectRow (-1, false);
				else {
					
					EtoTreeItem myitem;
					if (cachedItems.TryGetValue (value, out myitem)) {
						var row = outline.RowForItem (myitem);
						if (row >= 0)
							outline.SelectRow (row, false);
					}
				}
			}
		}
		
		public override bool Enabled {
			get { return outline.Enabled; }
			set { outline.Enabled = value; }
		}
		
		public ContextMenu ContextMenu {
			get { return contextMenu; }
			set {
				contextMenu = value;
				if (contextMenu != null)
					outline.Menu = ((ContextMenuHandler)contextMenu.Handler).Control;
				else
					outline.Menu = null;
			}
		}
		
		void ExpandItems (NSObject parent)
		{
			var ds = outline.DataSource;
			var count = ds.GetChildrenCount (outline, parent);
			for (int i=0; i<count; i++) {
				
				var item = ds.GetChild (outline, i, parent) as EtoTreeItem;
				if (item != null && item.Item.Expanded) {
					outline.ExpandItem (item);
					ExpandItems (item);
				}
			}
		}

		public bool ShowHeader {
			get {
				return outline.HeaderView != null;
			}
			set {
				if (value && outline.HeaderView == null) {
					outline.HeaderView = new NSTableHeaderView ();
				} else if (!value && outline.HeaderView != null) {
					outline.HeaderView = null;
				}
			}
		}

		public NSTableView Table {
			get { return outline; }
		}
		
		public object GetDataValue (int row, int column)
		{
			var ds = outline.DataSource;
			var item = outline.ItemAtRow (row) as EtoTreeItem;
			if (item != null)
				return item.Item.GetValue (column);
			else
				return null;
		}
	}
}

