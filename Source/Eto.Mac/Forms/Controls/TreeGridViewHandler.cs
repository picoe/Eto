using System;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;
using Eto.Mac.Forms.Cells;
using Eto.Drawing;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#if Mac64
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#if SDCOMPAT
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
#endif
#endif

#if XAMMAC
using nnint = System.Int32;
using nnint2 = System.Int32;
#elif Mac64
using nnint = System.UInt64;
using nnint2 = System.Int64;
#else
using nnint = System.UInt32;
using nnint2 = System.Int32;
#endif

namespace Eto.Mac.Forms.Controls
{
	public class TreeGridViewHandler : GridHandler<NSOutlineView, TreeGridView, TreeGridView.ICallback>, TreeGridView.IHandler, IDataViewHandler
	{
		ITreeGridStore<ITreeGridItem> store;
		readonly Dictionary<ITreeGridItem, EtoTreeItem> cachedItems = new Dictionary<ITreeGridItem, EtoTreeItem>();
		readonly Dictionary<int, EtoTreeItem> topitems = new Dictionary<int, EtoTreeItem>();
		int suppressExpandCollapseEvents;
		int skipSelectionChanged;

		public class EtoTreeItem : NSObject
		{
			Dictionary<int, EtoTreeItem> items;

			public EtoTreeItem()
			{
			}

			public EtoTreeItem(IntPtr ptr)
				: base(ptr)
			{
			}

			public EtoTreeItem(EtoTreeItem value)
			{
				this.Item = value.Item;
				this.items = value.items;
			}

			public ITreeGridItem Item { get; set; }

			public Dictionary<int, EtoTreeItem> Items
			{
				get
				{
					if (items == null)
						items = new Dictionary<int, EtoTreeItem>();
					return items;
				}
			}
		}

		bool ChildIsSelected(ITreeGridItem item)
		{
			var node = SelectedItem;

			while (node != null)
			{
				if (node == item)
					return true;
				node = node.Parent;
			}
			return false;
		}

		public class EtoOutlineDelegate : NSOutlineViewDelegate
		{
			WeakReference handler;
			public TreeGridViewHandler Handler { get { return (TreeGridViewHandler)handler.Target; } set { handler = new WeakReference(value); } }

			bool? collapsedItemIsSelected;
			ITreeGridItem lastSelected;

			public override void SelectionDidChange(NSNotification notification)
			{
				var h = Handler;
				if (h.skipSelectionChanged > 0)
					return;

				h.Callback.OnSelectionChanged(h.Widget, EventArgs.Empty);
				var item = h.SelectedItem;
				if (!ReferenceEquals(item, lastSelected))
				{
					h.Callback.OnSelectedItemChanged(h.Widget, EventArgs.Empty);
					lastSelected = item;
				}
			}

			public override void ItemDidCollapse(NSNotification notification)
			{
				var h = Handler;
				if (h.suppressExpandCollapseEvents > 0)
					return;
				var myitem = notification.UserInfo[(NSString)"NSObject"] as EtoTreeItem;
				if (myitem != null)
				{
					myitem.Item.Expanded = false;
					h.Callback.OnCollapsed(h.Widget, new TreeGridViewItemEventArgs(myitem.Item));
					if (collapsedItemIsSelected == true)
					{
						h.SelectedItem = myitem.Item;
						collapsedItemIsSelected = null;
						h.skipSelectionChanged = 0;
					}
				}
			}

			public override bool ShouldExpandItem(NSOutlineView outlineView, NSObject item)
			{
				var h = Handler;
				if (h.suppressExpandCollapseEvents > 0)
					return true;
				var myitem = item as EtoTreeItem;
				if (myitem != null)
				{
					var args = new TreeGridViewItemCancelEventArgs(myitem.Item);
					h.Callback.OnExpanding(h.Widget, args);
					return !args.Cancel;
				}
				return true;
			}

			public override bool ShouldCollapseItem(NSOutlineView outlineView, NSObject item)
			{
				var h = Handler;
				if (h.suppressExpandCollapseEvents > 0)
					return true;
				var myitem = item as EtoTreeItem;
				if (myitem != null)
				{
					var args = new TreeGridViewItemCancelEventArgs(myitem.Item);
					h.Callback.OnCollapsing(h.Widget, args);
					if (!args.Cancel && !h.AllowMultipleSelection)
					{
						collapsedItemIsSelected = h.ChildIsSelected(myitem.Item);
						if (collapsedItemIsSelected == true)
							h.skipSelectionChanged = 1;
					}
					else
						collapsedItemIsSelected = null;
					return !args.Cancel;
				}
				collapsedItemIsSelected = null;
				return true;
			}

			public override void ItemDidExpand(NSNotification notification)
			{
				var h = Handler;
				if (h.suppressExpandCollapseEvents > 0)
					return;
				var myitem = notification.UserInfo[(NSString)"NSObject"] as EtoTreeItem;
				if (myitem != null)
				{
					myitem.Item.Expanded = true;
					h.suppressExpandCollapseEvents++;
					h.ExpandItems(myitem);
					h.suppressExpandCollapseEvents--;
					h.Callback.OnExpanded(h.Widget, new TreeGridViewItemEventArgs(myitem.Item));
					h.AutoSizeColumns();
				}
			}

			public override void DidClickTableColumn(NSOutlineView outlineView, NSTableColumn tableColumn)
			{
				var column = Handler.GetColumn(tableColumn);
				var args = new GridColumnEventArgs(column.Widget);
				Handler.Callback.OnColumnHeaderClick(Handler.Widget, args);
			}

			public override NSView GetView(NSOutlineView outlineView, NSTableColumn tableColumn, NSObject item)
			{
				var colHandler = Handler.GetColumn(tableColumn);
				if (colHandler != null && colHandler.DataCell != null)
				{
					var cellHandler = colHandler.DataCell.Handler as ICellHandler;
					if (cellHandler != null)
					{
						return cellHandler.GetViewForItem(outlineView, tableColumn, -1, item, (obj, row) => obj != null ? ((EtoTreeItem)obj).Item : null);
					}
				}
				return outlineView.MakeView(tableColumn.Identifier, this);
			}
		}

		public class EtoDataSource : NSOutlineViewDataSource
		{
			WeakReference handler;
			public TreeGridViewHandler Handler { get { return (TreeGridViewHandler)handler.Target; } set { handler = new WeakReference(value); } }

			public override NSObject GetObjectValue(NSOutlineView outlineView, NSTableColumn tableColumn, NSObject item)
			{
				var colHandler = Handler.GetColumn(tableColumn);
				if (colHandler != null)
				{
					var myitem = (EtoTreeItem)item;
					return colHandler.GetObjectValue(myitem.Item);
				}
				return null;
			}

			public override void SetObjectValue(NSOutlineView outlineView, NSObject theObject, NSTableColumn tableColumn, NSObject item)
			{
				var colHandler = Handler.GetColumn(tableColumn);
				if (colHandler != null)
				{
					var myitem = (EtoTreeItem)item;
					colHandler.SetObjectValue(myitem.Item, theObject);
				}
			}

			public override bool ItemExpandable(NSOutlineView outlineView, NSObject item)
			{
				var myitem = item as EtoTreeItem;
				return myitem != null && myitem.Item.Expandable;
			}

			public override NSObject GetChild(NSOutlineView outlineView, nint childIndex, NSObject item)
			{
				Dictionary<int, EtoTreeItem> items;
				var myitem = item as EtoTreeItem;
				items = myitem == null ? Handler.topitems : myitem.Items;

				EtoTreeItem etoItem;
				if (!items.TryGetValue((int)childIndex, out etoItem))
				{

					var parentItem = myitem != null ? (ITreeGridStore<ITreeGridItem>)myitem.Item : Handler.store;
					etoItem = new EtoTreeItem { Item = parentItem[(int)childIndex] };
					Handler.cachedItems[etoItem.Item] = etoItem;
					items.Add((int)childIndex, etoItem);
				}
				return etoItem;
			}

			public override nint GetChildrenCount(NSOutlineView outlineView, NSObject item)
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

			public TreeGridViewHandler Handler
			{
				get { return WeakHandler.Target as TreeGridViewHandler; }
				set { WeakHandler = new WeakReference(value); }
			}

			public override void MouseDown(NSEvent theEvent)
			{
				var handler = Handler;
				if (handler != null)
				{
					var args = MacConversions.GetMouseEvent(handler.ContainerControl, theEvent, false);
					if (theEvent.ClickCount >= 2)
						handler.Callback.OnMouseDoubleClick(handler.Widget, args);
					else
						handler.Callback.OnMouseDown(handler.Widget, args);
					if (args.Handled)
						return;

					var point = ConvertPointFromView(theEvent.LocationInWindow, null);
					int rowIndex = (int)GetRow(point);
					if (rowIndex >= 0)
					{
						int columnIndex = (int)GetColumn(point);
						var item = handler.GetItem(rowIndex);
						var column = columnIndex == -1 || columnIndex > handler.Widget.Columns.Count ? null : handler.Widget.Columns[columnIndex];
						var cellArgs = MacConversions.CreateCellMouseEventArgs(column, handler.ContainerControl, rowIndex, columnIndex, item, theEvent);
						if (theEvent.ClickCount >= 2)
							handler.Callback.OnCellDoubleClick(handler.Widget, cellArgs);
						else
							handler.Callback.OnCellClick(handler.Widget, cellArgs);
					}
					base.MouseDown(theEvent);

					// NSOutlineView uses an event loop and MouseUp() does not get called
					handler.Callback.OnMouseUp(handler.Widget, args);

					return;
				}
				base.MouseDown(theEvent);
			}

			public EtoOutlineView(TreeGridViewHandler handler)
			{
				Delegate = new EtoOutlineDelegate { Handler = handler };
				DataSource = new EtoDataSource { Handler = handler };
				//HeaderView = null,
				//AutoresizesOutlineColumn = true,
				//AllowsColumnResizing = false,
				AllowsColumnReordering = false;
				FocusRingType = NSFocusRingType.None;
				ColumnAutoresizingStyle = NSTableViewColumnAutoresizingStyle.None;
			}
		}

		public override object EventObject
		{
			get { return Control; }
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
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
					base.AttachEvent(id);
					break;
			}
		}

		protected override NSOutlineView CreateControl()
		{
			return new EtoOutlineView(this);
		}

		public ITreeGridStore<ITreeGridItem> DataStore
		{
			get { return store; }
			set
			{
				store = value;
				topitems.Clear();
				cachedItems.Clear();
				Control.ReloadData();
				ExpandItems(null);
				if (Widget.Loaded)
					AutoSizeColumns();
			}
		}

		static IEnumerable<ITreeGridItem> GetParents(ITreeGridItem item)
		{
			var parent = item.Parent;
			while (parent != null)
			{
				yield return parent;
				parent = parent.Parent;
			}
		}

		EtoTreeItem GetCachedItem(ITreeGridItem item)
		{
			EtoTreeItem myitem;
			return cachedItems.TryGetValue(item, out myitem) ? myitem : null;
		}

		int CountRows(ITreeGridItem item)
		{
			if (!item.Expanded)
				return 0;

			var rows = 0;
			var container = item as IDataStore<ITreeGridItem>;
			if (container != null)
			{
				rows += container.Count;
				for (int i = 0; i < container.Count; i++)
				{
					rows += CountRows(container[i]);
				}
			}
			return rows;
		}

		int FindRow(IDataStore<ITreeGridItem> container, ITreeGridItem item)
		{
			int row = 0;
			for (int i = 0; i < container.Count; i++)
			{
				var current = container[i];
				if (object.ReferenceEquals(current, item))
				{
					return row;
				}
				row++;
				row += CountRows(current);
			}
			return -1;
		}

		int? ExpandToItem(ITreeGridItem item)
		{
			var parents = GetParents(item).Reverse();
			IDataStore<ITreeGridItem> lastParent = null;
			var row = 0;
			foreach (var parent in parents)
			{
				if (lastParent != null)
				{
					var foundRow = FindRow(lastParent, parent);
					if (foundRow == -1)
						return null;
					row += foundRow;
					var foundItem = Control.ItemAtRow(row) as EtoTreeItem;
					if (foundItem == null)
						return null;
					Control.ExpandItem(foundItem);
					foundItem.Item.Expanded = true;
					row++;
				}
				lastParent = parent as IDataStore<ITreeGridItem>;
			}
			if (lastParent != null)
			{
				var foundRow = FindRow(lastParent, item);
				if (foundRow == -1)
					return null;

				return foundRow + row;
			}
			return null;
		}

		public ITreeGridItem SelectedItem
		{
			get
			{
				var row = Control.SelectedRow;
				if (row == -1)
					return null;
				var myitem = (EtoTreeItem)Control.ItemAtRow(row);
				return myitem.Item;
			}
			set
			{
				if (value == null)
					Control.DeselectAll(Control);
				else {

					EtoTreeItem myitem;
					if (cachedItems.TryGetValue(value, out myitem))
					{
						var cachedRow = Control.RowForItem(myitem);
						if (cachedRow >= 0)
						{
							Control.ScrollRowToVisible(cachedRow);
							Control.SelectRow((nnint)cachedRow, false);
							return;
						}
					}

					var row = ExpandToItem(value);
					if (row != null)
					{
						Control.ScrollRowToVisible(row.Value);
						Control.SelectRow((nnint)row.Value, false);
					}
				}
			}
		}

		void ExpandItems(NSObject parent)
		{
			var ds = (EtoDataSource)Control.DataSource;
			var count = ds.GetChildrenCount(Control, parent);
			for (int i = 0; i < count; i++)
			{
				var item = ds.GetChild(Control, i, parent) as EtoTreeItem;
				if (item != null && item.Item.Expanded)
				{
					Control.ExpandItem(item);
					ExpandItems(item);
				}
			}
		}

		protected override void PreUpdateColumn(int index)
		{
			base.PreUpdateColumn(index);
			if (index == 0)
				Control.OutlineTableColumn = null;
		}

		protected override void UpdateColumns()
		{
			base.UpdateColumns();
			if (Control.OutlineTableColumn == null)
			{
				if (Widget.Columns.Count > 0)
					Control.OutlineTableColumn = ((GridColumnHandler)Widget.Columns[0].Handler).Control;
			}
			else if (Widget.Columns.Count == 0)
				Control.OutlineTableColumn = null;
		}

		public override object GetItem(int row)
		{
			var item = Control.ItemAtRow(row) as EtoTreeItem;
			return item != null ? item.Item : null;
		}

		public void ReloadData()
		{
			skipSelectionChanged++;
			suppressExpandCollapseEvents++;
			var selection = SelectedItems.ToList();

			var contentView = ScrollView.ContentView;
			var loc = contentView.Bounds.Location;
			if (!Control.IsFlipped)
				loc.Y = Control.Frame.Height - contentView.Frame.Height - loc.Y;

			topitems.Clear();
			cachedItems.Clear();
			Control.ReloadData();
			ExpandItems(null);

			if (Control.IsFlipped)
				contentView.ScrollToPoint(loc);
			else
				contentView.ScrollToPoint(new CGPoint(loc.X, Control.Frame.Height - contentView.Frame.Height - loc.Y));

			bool isSelectionChanged = false;
			foreach (var sel in selection)
			{
				var row = Control.RowForItem(GetCachedItem(sel as ITreeGridItem));
				if (row >= 0)
					Control.SelectRow((nnint)row, true);
				else
					isSelectionChanged = true;
			}

			ScrollView.ReflectScrolledClipView(contentView);
			suppressExpandCollapseEvents--;
			skipSelectionChanged--;

			if (isSelectionChanged)
			{
				Callback.OnSelectionChanged(Widget, EventArgs.Empty);
			}
		}

		public IEnumerable<object> SelectedItems
		{
			get
			{
				foreach (var row in Control.SelectedRows)
				{
					var item = Control.ItemAtRow((nnint2)row) as EtoTreeItem;
					if (item != null)
						yield return item.Item;
				}
			}
		}

		public void ReloadItem(ITreeGridItem item)
		{
			EtoTreeItem myitem;
			if (cachedItems.TryGetValue(item, out myitem))
			{
				skipSelectionChanged++;
				suppressExpandCollapseEvents++;
				var selectedItem = SelectedItem;
				var selection = SelectedItems.ToList();
				var row = Control.RowForItem(myitem);
				if (row >= 0)
					topitems.Remove((int)row);
				myitem.Items.Clear();

				Control.ReloadItem(myitem, true);
				SetItemExpansion(myitem);
				ExpandItems(myitem);
				AutoSizeColumns();
				var isSelectionChanged = false;
				foreach (var sel in selection)
				{
					row = Control.RowForItem(GetCachedItem(sel as ITreeGridItem));
					if (row >= 0)
						Control.SelectRow((nnint)row, true);
					else
						isSelectionChanged = true;
				}
				skipSelectionChanged--;
				suppressExpandCollapseEvents--;
				if (isSelectionChanged)
				{
					Callback.OnSelectionChanged(Widget, EventArgs.Empty);
					if (!ReferenceEquals(selectedItem , SelectedItem))
						Callback.OnSelectedItemChanged(Widget, EventArgs.Empty);
				}
			}
			else
				ReloadData();
		}

		void SetItemExpansion(NSObject parent)
		{
			suppressExpandCollapseEvents++;
			var item = parent as EtoTreeItem;
			if (item != null && item.Item.Expandable && item.Item.Expanded != Control.IsItemExpanded(item))
			{
				if (item.Item.Expanded)
					Control.ExpandItem(item);
				else
					Control.CollapseItem(item, false);
			}
			suppressExpandCollapseEvents--;
		}
	
		public ITreeGridItem GetCellAt(PointF location, out int column)
		{
			location += ScrollView.ContentView.Bounds.Location.ToEto();
			column = (int)Control.GetColumn(location.ToNS());
			var row = Control.GetRow(location.ToNS());
			if (row >= 0)
			{
				var item = Control.ItemAtRow(row) as EtoTreeItem;
				if (item != null)
					return item.Item;
			}
			return null;
		}
	}
}

