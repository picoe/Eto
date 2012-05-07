using System;
using MonoMac.AppKit;
using Eto.Forms;
using MonoMac.Foundation;
using System.Collections.Generic;
using Eto.Platform.Mac.Forms.Menu;
using System.Linq;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class TreeGridViewHandler : GridHandler<NSOutlineView, TreeGridView>, ITreeGridView, IDataViewHandler
	{
		ITreeGridStore<ITreeGridItem> store;
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
			
			public override void DidClickTableColumn (NSOutlineView outlineView, NSTableColumn tableColumn)
			{
				var column = Handler.Widget.Columns.First (r => object.ReferenceEquals (r.ControlObject, tableColumn));
				Handler.Widget.OnColumnHeaderClick (new GridColumnEventArgs (column));
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
			get { return Control; }
		}
		
		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case TreeGridView.SelectionChangedEvent:
			case TreeGridView.ExpandedEvent:
			case TreeGridView.ExpandingEvent:
			case TreeGridView.CollapsedEvent:
			case TreeGridView.CollapsingEvent:
			case Grid.ColumnHeaderClickEvent:
				// handled in delegate
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
		}
		
		public override void Initialize ()
		{
			Control = new EtoOutlineView { 
				Handler = this,
				Delegate = new EtoOutlineDelegate{ Handler = this },
				DataSource = new EtoDataSource{ Handler = this },
				//HeaderView = null,
				//AutoresizesOutlineColumn = true,
				//AllowsColumnResizing = false,
				AllowsColumnReordering = false,
				FocusRingType = NSFocusRingType.None,
				ColumnAutoresizingStyle = NSTableViewColumnAutoresizingStyle.None
			};

			base.Initialize ();
		}
		
		public ITreeGridStore<ITreeGridItem> DataStore {
			get { return store; }
			set {
				store = value;
				topitems.Clear ();
				cachedItems.Clear ();
				Control.ReloadData ();
				ExpandItems (null);
			}
		}
		
		public ITreeGridItem SelectedItem {
			get {
				var row = Control.SelectedRow;
				if (row == -1)
					return null;
				var myitem = Control.ItemAtRow (row) as EtoTreeItem;
				return myitem.Item;
			}
			set {
				if (value == null)
					Control.SelectRow (-1, false);
				else {
					
					EtoTreeItem myitem;
					if (cachedItems.TryGetValue (value, out myitem)) {
						var row = Control.RowForItem (myitem);
						if (row >= 0)
							Control.SelectRow (row, false);
					}
				}
			}
		}
		
		void ExpandItems (NSObject parent)
		{
			var ds = Control.DataSource;
			var count = ds.GetChildrenCount (Control, parent);
			for (int i=0; i<count; i++) {
				
				var item = ds.GetChild (Control, i, parent) as EtoTreeItem;
				if (item != null && item.Item.Expanded) {
					Control.ExpandItem (item);
					ExpandItems (item);
				}
			}
		}
		
		protected override void PreUpdateColumn (int index)
		{
			base.PreUpdateColumn (index);
			if (index == 0)
				Control.OutlineTableColumn = null;
		}
		
		protected override void UpdateColumns ()
		{
			base.UpdateColumns ();
			if (Control.OutlineTableColumn == null) {
				if (Widget.Columns.Count > 0)
					Control.OutlineTableColumn = ((GridColumnHandler)Widget.Columns[0].Handler).Control;
			}
			else if (Widget.Columns.Count == 0)
				Control.OutlineTableColumn = null;
		}

		public override object GetItem (int row)
		{
			var item = Control.ItemAtRow (row) as EtoTreeItem;
			if (item != null)
				return item.Item;
			else
				return null;
		}
	}
}

