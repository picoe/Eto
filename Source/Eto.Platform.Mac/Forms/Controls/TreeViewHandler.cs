using System;
using MonoMac.AppKit;
using Eto.Forms;
using MonoMac.Foundation;
using System.Collections.Generic;
using Eto.Platform.Mac.Forms.Menu;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class TreeViewHandler : MacView<NSScrollView, TreeView>, ITreeView
	{
		ITreeStore top;
		NSOutlineView outline;
		ContextMenu contextMenu;
		Dictionary<ITreeItem, EtoTreeItem> cachedItems = new Dictionary<ITreeItem, EtoTreeItem> ();
		Dictionary<int, EtoTreeItem> topitems = new Dictionary<int, EtoTreeItem> ();
		
		class EtoTreeItem : MacImageData
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
			
			public EtoTreeItem(EtoTreeItem value)
			{
				this.Item = value.Item;
				this.items = value.items;
			}

			public ITreeItem Item 
			{
				get { return item; }
				set
				{
					item = value;
					if (item.Image != null)
						base.Image = Item.Image.ControlObject as NSImage;
					base.Text = (NSString)item.Text;
				}
			}
			
			public Dictionary<int, EtoTreeItem> Items {
				get {
					if (items == null)
						items = new Dictionary<int, EtoTreeItem> ();
					return items;
				}
			}
			
			public override object Clone ()
			{
				return new EtoTreeItem(this);
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
				return myitem;
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
					var parentItem = myitem != null ? myitem.Item : Handler.top;
					item = new EtoTreeItem{ Item = parentItem.GetChild (childIndex) };
					Handler.cachedItems.Add (item.Item, item);
					items.Add (childIndex, item);
				}
				return item;
			}
			
			public override int GetChildrenCount (NSOutlineView outlineView, NSObject item)
			{
				if (Handler.top == null)
					return 0;
				
				if (item == null)
					return Handler.top.Count;
				
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
			outline.HeaderView = null;
			var col = new NSTableColumn ();
			col.DataCell = new MacImageListItemCell ();
			//col.ResizingMask = NSTableColumnResizingMask.None;
			outline.AddColumn (col);
			outline.OutlineTableColumn = col;
			outline.AutoresizesOutlineColumn = true;
			outline.AllowsColumnResizing = false;
			outline.ColumnAutoresizingStyle = NSTableViewColumnAutoresizingStyle.FirstColumnOnly;
			
			Control = new NSScrollView ();
			Control.HasVerticalScroller = true;
			Control.HasHorizontalScroller = true;
			Control.AutohidesScrollers = true;
			Control.BorderType = NSBorderType.BezelBorder;
			Control.DocumentView = outline;
		}

		public ITreeStore DataStore {
			get { return top; }
			set {
				top = value;
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
	}
}

