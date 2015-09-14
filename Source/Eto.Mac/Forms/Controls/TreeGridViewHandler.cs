using System;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;
#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using nnint = System.Int32;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#if Mac64
using CGSize = MonoMac.Foundation.NSSize;
using CGRect = MonoMac.Foundation.NSRect;
using CGPoint = MonoMac.Foundation.NSPoint;
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
using nnint = System.UInt64;
#else
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
using nnint = System.Int32;
#endif
#endif

namespace Eto.Mac.Forms.Controls
{
	public class TreeGridViewHandler : GridHandler<NSOutlineView, TreeGridView, TreeGridView.ICallback>, TreeGridView.IHandler, IDataViewHandler
	{
		ITreeGridStore<ITreeGridItem> store;
		readonly Dictionary<ITreeGridItem, EtoTreeItem> cachedItems = new Dictionary<ITreeGridItem, EtoTreeItem> ();
		readonly Dictionary<int, EtoTreeItem> topitems = new Dictionary<int, EtoTreeItem> ();

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

		bool ChildIsSelected (ITreeGridItem item)
		{
			var node = SelectedItem;
			
			while (node != null) {
				if (node == item)
					return true;
				node = node.Parent;
			}
			return false;
		}

		class EtoOutlineDelegate : NSOutlineViewDelegate
		{
			WeakReference handler;
			public TreeGridViewHandler Handler { get { return (TreeGridViewHandler)handler.Target; } set { handler = new WeakReference(value); } }

			bool? collapsedItemIsSelected;
			ITreeGridItem lastSelected;
			bool skipSelectionChanged;
			
			public override void SelectionDidChange (NSNotification notification)
			{
				if (!skipSelectionChanged) {
					Handler.Callback.OnSelectionChanged(Handler.Widget, EventArgs.Empty);
					var item = Handler.SelectedItem;
					if (!object.ReferenceEquals (item, lastSelected)) {
						Handler.Callback.OnSelectedItemChanged(Handler.Widget, EventArgs.Empty);
						lastSelected = item;
					}
				}
			}
			
			public override void ItemDidCollapse (NSNotification notification)
			{
				var myitem = notification.UserInfo [(NSString)"NSObject"] as EtoTreeItem;
				if (myitem != null) {
					myitem.Item.Expanded = false;
					Handler.Callback.OnCollapsed(Handler.Widget, new TreeGridViewItemEventArgs (myitem.Item));
					if (collapsedItemIsSelected == true) {
						Handler.SelectedItem = myitem.Item;
						collapsedItemIsSelected = null;
						skipSelectionChanged = false;
					}
				}
			}
			
			public override bool ShouldExpandItem (NSOutlineView outlineView, NSObject item)
			{
				var myitem = item as EtoTreeItem;
				if (myitem != null) {
					var args = new TreeGridViewItemCancelEventArgs (myitem.Item);
					Handler.Callback.OnExpanding(Handler.Widget, args);
					return !args.Cancel;
				}
				return true;
			}
			
			public override bool ShouldCollapseItem (NSOutlineView outlineView, NSObject item)
			{
				var myitem = item as EtoTreeItem;
				if (myitem != null) {
					var args = new TreeGridViewItemCancelEventArgs (myitem.Item);
					Handler.Callback.OnCollapsing(Handler.Widget, args);
					if (!args.Cancel && !Handler.AllowMultipleSelection) {
						collapsedItemIsSelected = Handler.ChildIsSelected (myitem.Item);
						skipSelectionChanged = collapsedItemIsSelected ?? false;
					}
					else 
						collapsedItemIsSelected = null;
					return !args.Cancel;
				}
				collapsedItemIsSelected = null;
				return true;
			}
			
			public override void ItemDidExpand (NSNotification notification)
			{
				var myitem = notification.UserInfo [(NSString)"NSObject"] as EtoTreeItem;
				if (myitem != null) {
					myitem.Item.Expanded = true;
					Handler.Callback.OnExpanded(Handler.Widget, new TreeGridViewItemEventArgs (myitem.Item));
					Handler.AutoSizeColumns();
				}
			}
			
			public override void DidClickTableColumn (NSOutlineView outlineView, NSTableColumn tableColumn)
			{
				var column = Handler.GetColumn (tableColumn);
				Handler.Callback.OnColumnHeaderClick(Handler.Widget, new GridColumnEventArgs (column.Widget));
			}

			public override void WillDisplayCell (NSOutlineView outlineView, NSObject cell, NSTableColumn tableColumn, NSObject item)
			{
				var colHandler = Handler.GetColumn (tableColumn);
				var myitem = item as EtoTreeItem;
				if (myitem != null) {
					Handler.OnCellFormatting(colHandler.Widget, myitem.Item, -1, cell as NSCell);
				}
			}
		}
			
		class EtoDataSource : NSOutlineViewDataSource
		{
			WeakReference handler;
			public TreeGridViewHandler Handler { get { return (TreeGridViewHandler)handler.Target; } set { handler = new WeakReference(value); } }

			public override NSObject GetObjectValue (NSOutlineView outlineView, NSTableColumn tableColumn, NSObject item)
			{
				var colHandler = Handler.GetColumn (tableColumn);
				if (colHandler != null) {
					var myitem = (EtoTreeItem)item;
					return colHandler.GetObjectValue (myitem.Item);
				}
				return null;
			}
			
			public override void SetObjectValue (NSOutlineView outlineView, NSObject theObject, NSTableColumn tableColumn, NSObject item)
			{
				var colHandler = Handler.GetColumn (tableColumn);
				if (colHandler != null) {
					var myitem = (EtoTreeItem)item;
					colHandler.SetObjectValue (myitem.Item, theObject);
				}
			}
			
			public override bool ItemExpandable (NSOutlineView outlineView, NSObject item)
			{
				var myitem = item as EtoTreeItem;
				return myitem != null && myitem.Item.Expandable;
			}
			
			public override NSObject GetChild (NSOutlineView outlineView, nint childIndex, NSObject item)
			{
				Dictionary<int, EtoTreeItem> items;
				var myitem = item as EtoTreeItem;
				items = myitem == null ? Handler.topitems : myitem.Items;
				
				EtoTreeItem etoItem;
				if (!items.TryGetValue((int)childIndex, out etoItem)) {
					var parentItem = myitem != null ? (ITreeGridStore<ITreeGridItem>)myitem.Item : Handler.store;
					etoItem = new EtoTreeItem{ Item = parentItem [(int)childIndex] };
					Handler.cachedItems.Add (etoItem.Item, etoItem);
					items.Add((int)childIndex, etoItem);
				}
				return etoItem;
			}
			
			public override nint GetChildrenCount (NSOutlineView outlineView, NSObject item)
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
			public WeakReference WeakHandler { get; set; }

			public object Handler
			{ 
				get { return WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); } 
			}

			public override void MouseDown(NSEvent theEvent)
			{
				var point = ConvertPointFromView(theEvent.LocationInWindow, null);

				int rowIndex;
				if ((rowIndex = (int)GetRow(point)) >= 0)
				{
					int columnIndex = (int)GetColumn(point);
					var item = (Handler as TreeGridViewHandler).GetItem(rowIndex);
					var column = columnIndex == -1 || columnIndex > (Handler as TreeGridViewHandler).Widget.Columns.Count ? null : (Handler as TreeGridViewHandler).Widget.Columns[columnIndex];
					(Handler as TreeGridViewHandler).Callback.OnCellClick((Handler as TreeGridViewHandler).Widget, new GridViewCellEventArgs(column, rowIndex, columnIndex, item));
				}

				base.MouseDown(theEvent);
			}
		}
		
		public override object EventObject {
			get { return Control; }
		}
		
		public override void AttachEvent (string id)
		{
			switch (id) {
			case TreeGridView.ExpandedEvent:
			case TreeGridView.ExpandingEvent:
			case TreeGridView.CollapsedEvent:
			case TreeGridView.CollapsingEvent:
			case TreeGridView.SelectedItemChangedEvent:
			case Grid.SelectionChangedEvent:
			case Grid.ColumnHeaderClickEvent:
				// handled in delegate
				break;
			case Grid.CellClickEvent:
				// Handled in EtoOutlineView
				break;
			default:
				base.AttachEvent (id);
				break;
			}
		}

		public TreeGridViewHandler()
		{
			Control = new EtoOutlineView
			{
				Handler = this,
				Delegate = new EtoOutlineDelegate { Handler = this },
				DataSource = new EtoDataSource { Handler = this },
				//HeaderView = null,
				//AutoresizesOutlineColumn = true,
				//AllowsColumnResizing = false,
				AllowsColumnReordering = false,
				FocusRingType = NSFocusRingType.None,
				ColumnAutoresizingStyle = NSTableViewColumnAutoresizingStyle.None
			};

		}

		public ITreeGridStore<ITreeGridItem> DataStore {
			get { return store; }
			set {
				store = value;
				topitems.Clear ();
				cachedItems.Clear ();
				Control.ReloadData ();
				ExpandItems (null);
				if (Widget.Loaded)
					AutoSizeColumns ();
			}
		}

		static IEnumerable<ITreeGridItem> GetParents (ITreeGridItem item)
		{
			var parent = item.Parent;
			while (parent != null) {
				yield return parent;
				parent = parent.Parent;
			}
		}

		EtoTreeItem GetCachedItem (ITreeGridItem item)
		{
			EtoTreeItem myitem;
			return cachedItems.TryGetValue(item, out myitem) ? myitem : null;
		}

		int CountRows (ITreeGridItem item)
		{
			if (!item.Expanded)
				return 0;

			var rows = 0;
			var container = item as IDataStore<ITreeGridItem>;
			if (container != null) {
				rows += container.Count;
				for (int i = 0; i < container.Count; i++)
				{
					rows += CountRows (container[i]);
				}
			}
			return rows;
		}

		int FindRow (IDataStore<ITreeGridItem> container, ITreeGridItem item)
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
		
		int? ExpandToItem (ITreeGridItem item)
		{
			var parents = GetParents (item).Reverse ();
			IDataStore<ITreeGridItem> lastParent = null;
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
				lastParent = parent as IDataStore<ITreeGridItem>;
			}
			if (lastParent != null) {
				var foundRow = FindRow (lastParent, item);
				if (foundRow == -1)
					return null;

				return foundRow + row;
			}
			return null;
		}

		public ITreeGridItem SelectedItem {
			get {
				var row = Control.SelectedRow;
				if (row == -1)
					return null;
				var myitem = (EtoTreeItem)Control.ItemAtRow(row);
				return myitem.Item;
			}
			set {
				if (value == null)
					Control.DeselectAll(Control);
				else {
					
					EtoTreeItem myitem;
					if (cachedItems.TryGetValue (value, out myitem)) {
						var cachedRow = Control.RowForItem (myitem);
						if (cachedRow >= 0) {
							Control.ScrollRowToVisible (cachedRow);
							Control.SelectRow ((nnint)cachedRow, false);
							return;
						}
					}

					var row = ExpandToItem (value);
					if (row != null) {
						Control.ScrollRowToVisible (row.Value);
						Control.SelectRow ((nnint)row.Value, false);
					}
				}
			}
		}
		
		void ExpandItems (NSObject parent)
		{
			var ds = (EtoDataSource)Control.DataSource;
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
			var item = Control.ItemAtRow(row) as EtoTreeItem;
			return item != null ? item.Item : null;
		}
	}
}

