using System;
using MonoMac.AppKit;
using Eto.Forms;
using MonoMac.Foundation;
using System.Collections.Generic;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class TreeViewHandler : MacView<NSScrollView, TreeView>, ITreeView
	{
		ITreeItem top;
		NSOutlineView outline;
		Dictionary<ITreeItem, MyItem> cachedItems = new Dictionary<ITreeItem, MyItem> ();
		Dictionary<int, MyItem> topitems = new Dictionary<int, MyItem> ();
		
		class MyItem : MacImageData
		{
			Dictionary<int, MyItem> items;
			ITreeItem item;
			
			public MyItem ()
			{
			}
			
			public MyItem (IntPtr ptr)
				: base(ptr)
			{
			}
			
			public MyItem(MyItem value)
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
			
			public Dictionary<int, MyItem> Items {
				get {
					if (items == null)
						items = new Dictionary<int, MyItem> ();
					return items;
				}
			}
			
			public override object Clone ()
			{
				return new MyItem(this);
			}
			
		}
		
		class MyDelegate : NSOutlineViewDelegate
		{
			public TreeViewHandler Handler { get; set; }
			
			public override void SelectionDidChange (NSNotification notification)
			{
				Handler.Widget.OnSelectionChanged (EventArgs.Empty);
			}
			
			public override void ItemDidCollapse (NSNotification notification)
			{
				var myitem = notification.UserInfo [(NSString)"NSObject"] as MyItem;
				if (myitem != null) {
					myitem.Item.Expanded = false;
				}
			}
			
			public override void ItemDidExpand (NSNotification notification)
			{
				var myitem = notification.UserInfo [(NSString)"NSObject"] as MyItem;
				if (myitem != null) {
					myitem.Item.Expanded = true;
				}
			}
		}
			
		class MyDataSource : NSOutlineViewDataSource
		{
			public TreeViewHandler Handler { get; set; }
			
			public override NSObject GetObjectValue (NSOutlineView outlineView, NSTableColumn forTableColumn, NSObject byItem)
			{
				var myitem = byItem as MyItem;
				return myitem;
			}
			
			public override bool ItemExpandable (NSOutlineView outlineView, NSObject item)
			{
				var myitem = item as MyItem;
				if (myitem == null)
					return false;
				return myitem.Item.Expandable;
			}
			
			public override NSObject GetChild (NSOutlineView outlineView, int childIndex, NSObject ofItem)
			{
				Dictionary<int, MyItem> items;
				var myitem = ofItem as MyItem;
				if (ofItem == null)
					items = Handler.topitems;
				else
					items = myitem.Items;
				
				MyItem item;
				if (!items.TryGetValue (childIndex, out item)) {
					var parentItem = myitem != null ? myitem.Item : Handler.top;
					item = new MyItem{ Item = parentItem.GetChild (childIndex) };
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
				
				var myitem = item as MyItem;
				return myitem.Item.Count;
			}
		}
		
		public TreeViewHandler ()
		{
			outline = new NSOutlineView ();
			outline.Delegate = new MyDelegate{ Handler = this };
			outline.DataSource = new MyDataSource{ Handler = this };
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

		public ITreeItem TopNode {
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
				var myitem = outline.ItemAtRow (row) as MyItem;
				return myitem.Item;
			}
			set {
				if (value == null)
					outline.SelectRow (-1, false);
				else {
					
					MyItem myitem;
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
		
		void ExpandItems (NSObject parent)
		{
			var ds = outline.DataSource;
			var count = ds.GetChildrenCount (outline, parent);
			for (int i=0; i<count; i++) {
				var item = ds.GetChild (outline, i, parent) as MyItem;
				if (item != null && item.Item.Expanded) {
					outline.ExpandItem (item);
					ExpandItems (item);
				}
			}
		}
	}
}

