using System;
using MonoMac.AppKit;
using Eto.Forms;
using MonoMac.Foundation;
using System.Collections.Generic;
using Eto.Platform.Mac.Forms.Menu;
using System.Linq;
using sd = System.Drawing;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class TreeViewHandler : MacView<NSOutlineView, TreeView>, ITreeView
	{
		ITreeStore top;
		ContextMenu contextMenu;
		Dictionary<ITreeItem, EtoTreeItem> cachedItems = new Dictionary<ITreeItem, EtoTreeItem> ();
		Dictionary<int, EtoTreeItem> topitems = new Dictionary<int, EtoTreeItem> ();
		bool selectionChanging;
		
		public NSScrollView Scroll { get; private set; }
		
		public class EtoTreeItem : MacImageData
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
				: base (value)
			{
				this.Item = value.Item;
				this.items = value.items;
			}

			public ITreeItem Item {
				get { return item; }
				set {
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
				return new EtoTreeItem (this);
			}
			
		}
		
		public class EtoOutlineDelegate : NSOutlineViewDelegate
		{
			public TreeViewHandler Handler { get; set; }
			
			public override void SelectionDidChange (NSNotification notification)
			{
				if (!Handler.selectionChanging)
					Handler.Widget.OnSelectionChanged (EventArgs.Empty);
			}
			
			public override void ItemDidCollapse (NSNotification notification)
			{
				var myitem = notification.UserInfo [(NSString)"NSObject"] as EtoTreeItem;
				if (myitem != null) {
					myitem.Item.Expanded = false;
					Handler.Widget.OnCollapsed (new TreeViewItemEventArgs (myitem.Item));
				}
			}
			
			public override bool ShouldExpandItem (NSOutlineView outlineView, NSObject item)
			{
				var myitem = item as EtoTreeItem;
				if (myitem != null) {
					var args = new TreeViewItemCancelEventArgs (myitem.Item);
					Handler.Widget.OnExpanding (args);
					return !args.Cancel;
				}
				return true;
			}
			
			public override bool ShouldCollapseItem (NSOutlineView outlineView, NSObject item)
			{
				var myitem = item as EtoTreeItem;
				if (myitem != null) {
					var args = new TreeViewItemCancelEventArgs (myitem.Item);
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
					Handler.Widget.OnExpanded (new TreeViewItemEventArgs (myitem.Item));
				}
			}
		}
			
		public class EtoDataSource : NSOutlineViewDataSource
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
					item = new EtoTreeItem{ Item = parentItem [childIndex] };
					Handler.cachedItems[item.Item] = item;
					items[childIndex] = item;
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
		
		public override NSView ContainerControl {
			get { return Scroll; }
		}
		
		public TreeViewHandler ()
		{
			Control = new EtoOutlineView { 
				Handler = this,
				Delegate = new EtoOutlineDelegate{ Handler = this },
				DataSource = new EtoDataSource{ Handler = this },
				HeaderView = null,
				AutoresizesOutlineColumn = true,
				AllowsColumnResizing = false,
				ColumnAutoresizingStyle = NSTableViewColumnAutoresizingStyle.FirstColumnOnly
			};
			var col = new NSTableColumn {
				DataCell = new MacImageListItemCell { 
					UsesSingleLineMode = true
				}
			};
			
			
			Control.AddColumn (col);
			Control.OutlineTableColumn = col;
			
			Scroll = new NSScrollView {
				HasVerticalScroller = true,
				HasHorizontalScroller = true,
				AutohidesScrollers = true,
				BorderType = NSBorderType.BezelBorder,
				DocumentView = Control
			};
		}
		
		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case TreeView.ExpandedEvent:
			case TreeView.ExpandingEvent:
			case TreeView.CollapsedEvent:
			case TreeView.CollapsingEvent:
				// handled in delegate
				break;
			case TreeView.ActivatedEvent:
				this.Widget.KeyDown += (sender, e) => {
					if (e.KeyData == Key.Enter) {
						Widget.OnActivated (new TreeViewItemEventArgs(this.SelectedItem));
					}
				};
				Control.DoubleClick += (sender, e) => {
					Widget.OnActivated (new TreeViewItemEventArgs(this.SelectedItem));
				};
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
		}

		public ITreeStore DataStore {
			get { return top; }
			set {
				top = value;
				topitems.Clear ();
				cachedItems.Clear ();
				Control.ReloadData ();
				ExpandItems (null);
			}
		}
		
		public ITreeItem SelectedItem {
			get {
				var row = Control.SelectedRow;
				if (row == -1)
					return null;
				var myitem = Control.ItemAtRow (row) as EtoTreeItem;
				return myitem.Item;
			}
			set {
				PerformSelect (value, true);
			}
		}
		
		void PerformSelect (ITreeItem item, bool scrollToRow)
		{
			if (item == null)
				Control.SelectRow (-1, false);
			else {
				
				EtoTreeItem myitem;
				if (cachedItems.TryGetValue (item, out myitem)) {
					var cachedRow = Control.RowForItem (myitem);
					if (cachedRow >= 0) {
						if (scrollToRow) Control.ScrollRowToVisible (cachedRow);
						Control.SelectRow (cachedRow, false);
						return;
					}
				}
				
				var row = ExpandToItem (item);
				if (row != null) {
					if (scrollToRow) Control.ScrollRowToVisible (row.Value);
					Control.SelectRow (row.Value, false);
				}
			}
		}
		
		public override bool Enabled {
			get { return Control.Enabled; }
			set { Control.Enabled = value; }
		}
		
		public ContextMenu ContextMenu {
			get { return contextMenu; }
			set {
				contextMenu = value;
				if (contextMenu != null)
					Control.Menu = ((ContextMenuHandler)contextMenu.Handler).Control;
				else
					Control.Menu = null;
			}
		}
		
		IEnumerable<ITreeItem> GetParents (ITreeItem item)
		{
			var parent = item.Parent;
			while (parent != null) {
				yield return parent;
				parent = parent.Parent;
			}
		}

		int CountRows (ITreeItem item)
		{
			if (!item.Expanded)
				return 0;
			
			var rows = 0;
			var container = item as IDataStore<ITreeItem>;
			if (container != null) {
				rows += container.Count;
				for (int i = 0; i < container.Count; i++)
				{
					rows += CountRows (container[i]);
				}
			}
			return rows;
		}
		

		int FindRow (IDataStore<ITreeItem> container, ITreeItem item)
		{
			int row = 0;
			for (int i = 0; i < container.Count; i++) {
				var current = container [i];
				if (object.ReferenceEquals (current, item)) {
					return row;
				}
				row ++;
				row += CountRows (current);
			}
			return -1;
		}
		
		int? ExpandToItem (ITreeItem item)
		{
			var parents = GetParents (item).Reverse ();
			IDataStore<ITreeItem> lastParent = null;
			var row = 0;
			foreach (var parent in parents) {
				if (lastParent != null) {
					var foundRow = FindRow (lastParent, parent);
					if (foundRow == -1)
						return null;
					row += foundRow;
					var foundItem = Control.ItemAtRow (row) as EtoTreeItem;
					if (foundItem == null)
						return null;
					Control.ExpandItem (foundItem);
					foundItem.Item.Expanded = true;
					row ++;
				}
				lastParent = parent as IDataStore<ITreeItem>;
			}
			if (lastParent != null) {
				var foundRow = FindRow (lastParent, item);
				if (foundRow == -1)
					return null;
				
				return foundRow + row;
			}
			return null;
		}
		
		void ExpandItems (NSObject parent)
		{
			var ds = Control.DataSource;
			var count = ds.GetChildrenCount (Control, parent);
			for (int i=0; i<count; i++) {
				var item = ds.GetChild (Control, i, parent) as EtoTreeItem;
				if (item != null && item.Item.Expandable) {
					if (item.Item.Expanded) {
						Control.ExpandItem (item);
						ExpandItems (item);
					}
					else
						Control.CollapseItem (item);
				}
			}
		}

		public void RefreshData ()
		{
			selectionChanging = true;
			var selectedItem = SelectedItem;
			
			var loc = Scroll.ContentView.Bounds.Location;
			if (!Control.IsFlipped)
				loc.Y = Control.Frame.Height - Scroll.ContentView.Frame.Height - loc.Y;

			topitems.Clear ();
			cachedItems.Clear ();
			Control.ReloadData ();
			ExpandItems (null);
			PerformSelect(selectedItem, false);
			
			if (Control.IsFlipped)
				Scroll.ContentView.ScrollToPoint (loc);
			else
				Scroll.ContentView.ScrollToPoint (new sd.PointF (loc.X, Control.Frame.Height - Scroll.ContentView.Frame.Height - loc.Y));
			
			Scroll.ReflectScrolledClipView (Scroll.ContentView);
			
			selectionChanging = false;
		}

		public void RefreshItem (ITreeItem item)
		{
			EtoTreeItem myitem;
			if (cachedItems.TryGetValue (item, out myitem))
			{
				Control.ReloadItem (myitem);
				var row = Control.RowForItem (myitem);
				if (row >= 0)
					topitems.Remove (row);
			}
		}
	}
}

