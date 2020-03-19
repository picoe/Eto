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
#if XAMMAC2
using nnuint = System.UInt32;
#else
using nnuint = System.Int32;
#endif
#elif Mac64
using nnint = System.UInt64;
using nnint2 = System.Int64;
using nnuint = System.UInt64;
#else
using nnint = System.UInt32;
using nnint2 = System.Int32;
using nnuint = System.UInt32;
#endif

namespace Eto.Mac.Forms.Controls
{
	public class TreeGridViewHandler : GridHandler<TreeGridViewHandler.EtoOutlineView, TreeGridView, TreeGridView.ICallback>, TreeGridView.IHandler, IDataViewHandler
	{
		ITreeGridStore<ITreeGridItem> store;
		readonly Dictionary<object, EtoTreeItem> cachedItems = new Dictionary<object, EtoTreeItem>();
		readonly Dictionary<int, EtoTreeItem> topitems = new Dictionary<int, EtoTreeItem>();
		int suppressExpandCollapseEvents;
		int skipSelectionChanged;

		static readonly object ShowGroupItems_Key = new object();
		static readonly object AllowGroupSelection_Key = new object();

		/// <summary>
		/// Gets or sets a value indicating that the top level will be shown as groups
		/// </summary>
		public bool ShowGroups
		{
			get => Widget.Properties.Get<bool>(ShowGroupItems_Key);
			set => Widget.Properties.Set(ShowGroupItems_Key, value);
		}

		public bool AllowGroupSelection
		{
			get => Widget.Properties.Get<bool>(AllowGroupSelection_Key);
			set => Widget.Properties.Set(AllowGroupSelection_Key, value);
		}

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

			public override bool IsGroupItem(NSOutlineView outlineView, NSObject item)
			{
				return Handler.ShowGroups && item != null && outlineView.LevelForItem(item) == 0;
			}

			public override bool ShouldSelectItem(NSOutlineView outlineView, NSObject item)
			{
				return Handler.AllowGroupSelection || !IsGroupItem(outlineView, item);
			}

			public override bool ShouldEditTableColumn(NSOutlineView outlineView, NSTableColumn tableColumn, NSObject item)
			{
				var h = Handler;
				if (h == null)
					return false;
				var colHandler = h.GetColumn(tableColumn);
				var etoItem = h.GetEtoItem(item);
				var row = h.Control.RowForItem(item);

				var args = new GridViewCellEventArgs(colHandler.Widget, (int)row, colHandler.Column, etoItem);
				h.Callback.OnCellEditing(h.Widget, args);
				h.SetIsEditing(true);
				return true;
			}

			public override void SelectionDidChange(NSNotification notification)
			{
				var h = Handler;
				if (h.skipSelectionChanged > 0)
					return;

				// didn't start a drag (when this was set), so clear this out when the selection changes
				h.CustomSelectedItems = null;

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
				var myitem = h.GetEtoItem(notification.UserInfo[(NSString)"NSObject"]);
				if (myitem != null)
				{
					myitem.Expanded = false;
					h.Callback.OnCollapsed(h.Widget, new TreeGridViewItemEventArgs(myitem));
					if (collapsedItemIsSelected == true)
					{
						h.SelectedItem = myitem;
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
				var myitem = h.GetEtoItem(item);
				if (myitem != null)
				{
					var args = new TreeGridViewItemCancelEventArgs(myitem);
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
				var myitem = h.GetEtoItem(item);
				if (myitem != null)
				{
					var args = new TreeGridViewItemCancelEventArgs(myitem);
					h.Callback.OnCollapsing(h.Widget, args);
					if (!args.Cancel && !h.AllowMultipleSelection)
					{
						collapsedItemIsSelected = h.ChildIsSelected(myitem);
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
				var item = notification.UserInfo[(NSString)"NSObject"];
				var myitem = h.GetEtoItem(item);
				if (myitem != null)
				{
					myitem.Expanded = true;
					h.suppressExpandCollapseEvents++;
					h.ExpandItems(item);
					h.suppressExpandCollapseEvents--;
					h.Callback.OnExpanded(h.Widget, new TreeGridViewItemEventArgs(myitem));
					Application.Instance.AsyncInvoke(() => h.AutoSizeColumns(true));
				}
			}

			public override nfloat GetSizeToFitColumnWidth(NSOutlineView outlineView, nint column)
			{
				var colHandler = Handler.GetColumn(outlineView.TableColumns()[column]);
				if (colHandler != null)
				{
					// turn on autosizing for this column again
					Application.Instance.AsyncInvoke(() => colHandler.AutoSize = true);
					return colHandler.GetPreferredWidth();
				}
				return 20;
			}

			public override void DidClickTableColumn(NSOutlineView outlineView, NSTableColumn tableColumn)
			{
				var column = Handler.GetColumn(tableColumn);
				if (column.Sortable)
				{
					var args = new GridColumnEventArgs(column.Widget);
					Handler.Callback.OnColumnHeaderClick(Handler.Widget, args);
				}
			}

			public override void ColumnDidResize(NSNotification notification)
			{
				if (!Handler.IsAutoSizingColumns)
				{
					// when the user resizes the column, don't autosize anymore when data/scroll changes
					var column = notification.UserInfo["NSTableColumn"] as NSTableColumn;
					if (column != null)
					{
						var colHandler = Handler.GetColumn(column);
						colHandler.AutoSize = false;
					}
				}
			}

			public override NSView GetView(NSOutlineView outlineView, NSTableColumn tableColumn, NSObject item)
			{
				if (tableColumn == null && Handler.ShowGroups)
					tableColumn = outlineView.TableColumns()[0];

				var colHandler = Handler.GetColumn(tableColumn);
				if (colHandler != null && colHandler.DataCell != null)
				{
					var cellHandler = colHandler.DataCell.Handler as ICellHandler;
					if (cellHandler != null)
					{
						return cellHandler.GetViewForItem(outlineView, tableColumn, -1, item, (obj, row) => obj != null ? ((EtoTreeItem)obj).Item : null);
					}
				}
				return outlineView.MakeView(tableColumn?.Identifier ?? string.Empty, this);
			}
		}

		public class EtoDataSource : NSOutlineViewDataSource
		{
			WeakReference handler;
			public TreeGridViewHandler Handler { get { return (TreeGridViewHandler)handler.Target; } set { handler = new WeakReference(value); } }

			public override NSObject GetObjectValue(NSOutlineView outlineView, NSTableColumn tableColumn, NSObject item)
			{
				if (tableColumn == null && Handler.ShowGroups)
					tableColumn = outlineView.TableColumns()[0];

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

					Handler.SetIsEditing(false);
					var row = outlineView.RowForItem(item);
					Handler.Callback.OnCellEdited(Handler.Widget, new GridViewCellEventArgs(colHandler.Widget, (int)row, colHandler.Column, myitem.Item));
				}
			}

			public override bool ItemExpandable(NSOutlineView outlineView, NSObject item)
			{
				var myitem = Handler.GetEtoItem(item);
				return myitem != null && myitem.Expandable;
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
				var h = Handler;
				if (h.store == null)
					return 0;

				if (item == null)
					return h.store.Count;

				var myitem = h.GetEtoItem(item) as ITreeGridStore<ITreeGridItem>;
				return myitem?.Count ?? 0;
			}

			public override NSDragOperation ValidateDrop(NSOutlineView outlineView, NSDraggingInfo info, NSObject item, nint index)
			{
				var h = Handler;
				if (h == null)
					return NSDragOperation.None;
				var etoInfo = GetDragInfo(info, item, index);
				var e = h.GetDragEventArgs(info, etoInfo);
				h.Callback.OnDragOver(h.Widget, e);
				if (e.AllowedEffects.HasFlag(e.Effects))
				{
					if (etoInfo.Position == GridDragPosition.Over)
						outlineView.SetDropItem(h.GetCachedItem(etoInfo.Item), -1);
					else
						outlineView.SetDropItem(h.GetCachedItem(etoInfo.Parent), etoInfo.InsertIndex);

					return e.Effects.ToNS();
				}
				else
					return NSDragOperation.None;
			}

			static Selector selGetChildIndex = new Selector("childIndexForItem:");

			TreeGridViewDragInfo GetDragInfo(NSDraggingInfo info, NSObject item, nint index)
			{
				var h = Handler;
				var outlineView = h.Control;
				var position = GridDragPosition.Over;
				int? childIndex;
				object etoitem;
				object parent;

				if (index != -1)
				{
					childIndex = (int)index;
					var row = outlineView.GetRow(outlineView.ConvertPointFromView(info.DraggingLocation, null));

					var nchildren = outlineView.DataSource.GetChildrenCount(outlineView, item);
					if (nchildren > 0)
					{
						if (index == nchildren)
						{
							position = GridDragPosition.After;
							var childItem = outlineView.DataSource.GetChild(outlineView, nchildren - 1, item);
							etoitem = h.GetEtoItem(childItem);
							childIndex = (int)nchildren - 1;
						}
						else
						{
							var childItem = outlineView.DataSource.GetChild(outlineView, index, item);
							var itemRow = outlineView.RowForItem(childItem);
							if (itemRow > row)
							{
								if (index > 0)
								{
									childItem = outlineView.DataSource.GetChild(outlineView, index - 1, item);
									if (outlineView.RespondsToSelector(selGetChildIndex)) // 10.11+
										childIndex = (int?)outlineView.GetChildIndex(childItem);
									else
										childIndex = null;
								}
								else
								{
									childItem = null;
									childIndex = -1;
								}
								position = GridDragPosition.After;
							}
							else
							{
								position = GridDragPosition.Before;
							}

							etoitem = h.GetEtoItem(childItem);
						}
					}
					else
						etoitem = null;
					parent = h.GetEtoItem(item);
				}
				else
				{
					if (item == null)
						childIndex = -1;
					else if (outlineView.RespondsToSelector(selGetChildIndex)) // 10.11+
						childIndex = (int?)outlineView.GetChildIndex(item);
					else
						childIndex = null;

					etoitem = h.GetEtoItem(item);
					parent = (etoitem as ITreeGridItem)?.Parent;
				}
				return new TreeGridViewDragInfo(h.Widget, parent, etoitem, childIndex, position);
			}

			public override bool AcceptDrop(NSOutlineView outlineView, NSDraggingInfo info, NSObject item, nint index)
			{
				var h = Handler;
				if (h == null)
					return false;
				var e = h.GetDragEventArgs(info, GetDragInfo(info, item, index));
				h.Callback.OnDragLeave(h.Widget, e);
				h.Callback.OnDragDrop(h.Widget, e);
				return true;
			}

			[Export("outlineView:draggingSession:endedAtPoint:operation:")]
#if XAMMAC
			public new void DraggingSessionEnded(NSOutlineView outlineView, NSDraggingSession session, CGPoint screenPoint, NSDragOperation operation)
#else
			public void DraggingSessionEnded(NSOutlineView outlineView, NSDraggingSession session, CGPoint screenPoint, NSDragOperation operation)
#endif
			{
				var h = Handler;
				if (h == null)
					return;

				if (h.CustomSelectedItems != null)
				{
					h.CustomSelectedItems = null;
					h.Callback.OnSelectionChanged(h.Widget, EventArgs.Empty);
				}
			}

			public override bool OutlineViewwriteItemstoPasteboard(NSOutlineView outlineView, NSArray items, NSPasteboard pboard)
			{
				var h = Handler;
				if (h == null)
					return false;

				if (h.IsMouseDragging)
				{
					h.Control.DragInfo = null;
					// give MouseMove event a chance to start the drag
					h.DragPasteboard = pboard;

					// check if the items are different than the selection so we can fire a changed event
					bool isDifferentSelection = (nint)items.Count != h.Control.SelectedRowCount;
					if (!isDifferentSelection)
					{
						// same count, ensure they're not different rows
						// typically only tests one entry here, as there's no way to drag more than a single non-selected item.
						var selectedRows = h.Control.SelectedRows.ToArray();
						for (var i = 0; i < selectedRows.Length; i++)
						{
							if (items.ValueAt((nuint)i) != h.Control.ItemAtRow((nint)selectedRows[i]).Handle)
							{
								isDifferentSelection = true;
								break;
							}
						}
					}

					if (isDifferentSelection)
					{
						h.CustomSelectedItems = GetItems(items).ToList();
						h.Callback.OnSelectionChanged(h.Widget, EventArgs.Empty);
					}
					var args = MacConversions.GetMouseEvent(h, NSApplication.SharedApplication.CurrentEvent, false);
					h.Callback.OnMouseMove(h.Widget, args);
					h.DragPasteboard = null;
					return h.Control.DragInfo != null;
				}

				return false;
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

			internal GridDragInfo DragInfo { get; set; }

#if XAMMAC2
			public override NSImage DragImageForRowsWithIndexestableColumnseventoffset(NSIndexSet dragRows, NSTableColumn[] tableColumns, NSEvent dragEvent, ref CGPoint dragImageOffset)
			{
				var img = DragInfo?.DragImage;
				if (img != null)
				{
					dragImageOffset = DragInfo.GetDragImageOffset();
					return img;
				}
				return base.DragImageForRowsWithIndexestableColumnseventoffset(dragRows, tableColumns, dragEvent, ref dragImageOffset);
			}
#else
			static readonly IntPtr selDragImageForRowsWithIndexes_TableColumns_Event_Offset_Handle = Selector.GetHandle("dragImageForRowsWithIndexes:tableColumns:event:offset:");

			[Export("dragImageForRowsWithIndexes:tableColumns:event:offset:")]
			public NSImage DragImageForRows(NSIndexSet dragRows, NSTableColumn[] tableColumns, NSEvent dragEvent, ref CGPoint dragImageOffset)
			{
				var img = DragInfo?.DragImage;
				if (img != null)
				{
					dragImageOffset = DragInfo.GetDragImageOffset();
					return img;
				}

				NSArray nSArray = NSArray.FromNSObjects(tableColumns);
				NSImage result = Runtime.GetNSObject<NSImage>(Messaging.IntPtr_objc_msgSendSuper_IntPtr_IntPtr_IntPtr_ref_CGPoint(SuperHandle, selDragImageForRowsWithIndexes_TableColumns_Event_Offset_Handle, dragRows.Handle, nSArray.Handle, dragEvent.Handle, ref dragImageOffset));
				nSArray.Dispose();
				return result;
			}
#endif

			[Export("draggingSession:sourceOperationMaskForDraggingContext:")]
			public NSDragOperation DraggingSessionSourceOperationMask(NSDraggingSession session, IntPtr context)
			{
				return DragInfo?.AllowedOperation ?? NSDragOperation.None;
			}

			public override void MouseDown(NSEvent theEvent)
			{
				var handler = Handler;
				if (handler != null)
				{
					var args = MacConversions.GetMouseEvent(handler, theEvent, false);
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
					handler.IsMouseDragging = true;
					base.MouseDown(theEvent);
					handler.IsMouseDragging = false;

					// NSOutlineView uses an event loop and MouseUp() does not get called
					handler.Callback.OnMouseUp(handler.Widget, args);

					return;
				}
				base.MouseDown(theEvent);
			}

			public EtoOutlineView(TreeGridViewHandler handler)
			{
				Delegate = new EtoOutlineDelegate { Handler = handler };
				//HeaderView = null,
				AutoresizesOutlineColumn = false;
				//AllowsColumnResizing = false,
				AllowsColumnReordering = false;
				FocusRingType = NSFocusRingType.None;
				ColumnAutoresizingStyle = NSTableViewColumnAutoresizingStyle.None;
				SetDraggingSourceOperationMask(NSDragOperation.All, true);
				SetDraggingSourceOperationMask(NSDragOperation.All, false);
			}

			public override void Layout()
			{
				if (MacView.NewLayout)
					base.Layout();
				Handler?.PerformLayout();
				if (!MacView.NewLayout)
					base.Layout();
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
				case TreeGridView.ActivatedEvent:
					Widget.KeyDown += (sender, e) =>
					{
						if (!e.Handled && e.KeyData == Keys.Enter)
						{
							Callback.OnActivated(Widget, new TreeGridViewItemEventArgs(SelectedItem));
							e.Handled = true;
						}
					};
					Widget.MouseDoubleClick += (sender, e) =>
					{
						var cell = GetCellAt(e.Location, out var column);
						if (cell != null)
						{
							Callback.OnActivated(Widget, new TreeGridViewItemEventArgs(SelectedItem));
							e.Handled = true;
						}
					};
					break;
				case TreeGridView.ExpandedEvent:
				case TreeGridView.ExpandingEvent:
				case TreeGridView.CollapsedEvent:
				case TreeGridView.CollapsingEvent:
				case TreeGridView.SelectedItemChangedEvent:
				case Grid.SelectionChangedEvent:
				case Grid.ColumnHeaderClickEvent:
					// handled in delegate
					break;
				case Grid.CellDoubleClickEvent:
				case Grid.CellClickEvent:
					// Handled in EtoOutlineView
					break;
				case Eto.Forms.Control.DragOverEvent:
				case Eto.Forms.Control.DragDropEvent:
					// handled in EtoDataSource
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		protected override EtoOutlineView CreateControl()
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
				if (Control.DataSource == null)
					Control.DataSource = new EtoDataSource { Handler = this };
				else
					Control.ReloadData();
				suppressExpandCollapseEvents++;
				ExpandItems(null);
				suppressExpandCollapseEvents--;
				if (Widget.Loaded)
					AutoSizeColumns(true);
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

		EtoTreeItem GetCachedItem(object item)
		{
			if (ReferenceEquals(item, null))
				return null;
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
				else
				{

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
				if (item != null && item.Item.Expanded && !Control.IsItemExpanded(item))
				{
					Control.ExpandItem(item);
					ExpandItems(item);
				}
			}
		}

		protected override void UpdateColumns()
		{
			base.UpdateColumns();
			if (Widget.Columns.Count > 0)
				Control.OutlineTableColumn = ((GridColumnHandler)Widget.Columns[0].Handler).Control;
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
				var cachedItem = GetCachedItem(sel as ITreeGridItem);
				if (cachedItem == null)
					continue;
				var row = Control.RowForItem(cachedItem);
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

		public IEnumerable<object> SelectedItems => CustomSelectedItems ?? SelectedItemsFromRows();

		public override IEnumerable<int> SelectedRows
		{
			get
			{
				var items = CustomSelectedItems;
				if (items != null)
				{
					return CustomSelectedItems.Select(r => (int)Control.RowForItem(GetCachedItem(r)));
				}
				return base.SelectedRows;
			}
			set
			{
				base.SelectedRows = value;
			}
		}

		IEnumerable<object> SelectedItemsFromRows()
		{
			foreach (var row in Control.SelectedRows)
			{
				var item = Control.ItemAtRow((nnint2)row) as EtoTreeItem;
				if (item != null)
					yield return item.Item;
			}
		}

		public void ReloadItem(ITreeGridItem item, bool reloadChildren)
		{
			EtoTreeItem myitem;
			if (cachedItems.TryGetValue(item, out myitem))
			{
				if (reloadChildren)
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
					AutoSizeColumns(true);
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
						if (!ReferenceEquals(selectedItem, SelectedItem))
							Callback.OnSelectedItemChanged(Widget, EventArgs.Empty);
					}
				}
				else
				{
					Control.ReloadItem(myitem, false);
				}
			}
			else if (reloadChildren)
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

		public TreeGridViewDragInfo GetDragInfo(DragEventArgs args) => args.ControlObject as TreeGridViewDragInfo;


		static readonly object DragPasteboard_Key = new object();

		NSPasteboard DragPasteboard
		{
			get { return Widget.Properties.Get<NSPasteboard>(DragPasteboard_Key); }
			set { Widget.Properties.Set(DragPasteboard_Key, value); }
		}

		static readonly object CustomSelectedItems_Key = new object();

		IList<object> CustomSelectedItems
		{
			get { return Widget.Properties.Get<IList<object>>(CustomSelectedItems_Key); }
			set { Widget.Properties.Set(CustomSelectedItems_Key, value); }
		}

		public override void DoDragDrop(DataObject data, DragEffects allowedAction, Image image, PointF origin)
		{
			if (DragPasteboard != null)
			{
				var handler = data.Handler as IDataObjectHandler;
				handler?.Apply(DragPasteboard);
				Control.DragInfo = new GridDragInfo
				{
					AllowedOperation = allowedAction.ToNS(),
					DragImage = image.ToNS(),
					ImageOffset = origin
				};
			}
			else
			{
				base.DoDragDrop(data, allowedAction, image, origin);
			}
		}

		static IEnumerable<object> GetItems(NSArray items)
		{
			for (nnuint i = 0; i < items.Count; i++)
			{
				var item = items.GetItem<EtoTreeItem>(i);
				if (item != null)
					yield return item.Item;
			}
		}

		ITreeGridItem GetEtoItem(NSObject item) => (item as EtoTreeItem)?.Item;

		public override ContextMenu ContextMenu
		{
			get => base.ContextMenu;
			set
			{
				var old = ContextMenu;
				if (old != null)
				{
					old.Opening -= ContextMenu_Opening;
				}
				base.ContextMenu = value;
				if (value != null)
				{
					value.Opening += ContextMenu_Opening;
				}
			}
		}

		void ContextMenu_Closed(object sender, EventArgs e)
		{
			var menu = (ContextMenu)sender;
			menu.Closed -= ContextMenu_Closed;

			// action is called after this, so we can't clear selected items immediately
			if (CustomSelectedItems != null)
			{
				Application.Instance.AsyncInvoke(() =>
				{
					CustomSelectedItems = null;
					Callback.OnSelectionChanged(Widget, EventArgs.Empty);
				});
			}
		}

		void ContextMenu_Opening(object sender, EventArgs e)
		{
			var row = (int)Control.ClickedRow;
			if (!SelectedRows.Contains(row))
			{
				var menu = (ContextMenu)sender;
				menu.Closed += ContextMenu_Closed;
				CustomSelectedItems = new[] { GetItem(row) };
				Callback.OnSelectionChanged(Widget, EventArgs.Empty);
			}
		}
	}
}

