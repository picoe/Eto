using Eto.Mac.Forms.Cells;

#if MACOS_NET && !VSMAC
using NSDraggingInfo = AppKit.INSDraggingInfo;
#endif


namespace Eto.Mac.Forms.Controls
{
	public class GridViewHandler : GridHandler<GridViewHandler.EtoTableView, GridView, GridView.ICallback>, GridView.IHandler
	{
		CollectionHandler collection;

		public class EtoTableView : NSTableView, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public GridViewHandler Handler
			{
				get { return WeakHandler?.Target as GridViewHandler; }
				set { WeakHandler = new WeakReference(value); }
			}

			/// <summary>
			/// The area to the right and below the rows is not filled with the background
			/// color. This fixes that. See http://orangejuiceliberationfront.com/themeing-nstableview/
			/// </summary>
			public override void DrawBackground(CGRect clipRect)
			{
				var h = Handler;
				if (h == null)
					return;
				var backgroundColor = h.BackgroundColor;
				if (backgroundColor != Colors.Transparent)
				{
					backgroundColor.ToNSUI().Set();
					NSGraphics.RectFill(Bounds);
				}
				else
					base.DrawBackground(clipRect);
			}

			public override void RightMouseDown(NSEvent theEvent)
			{
				if (Handler?.HandleMouseEvent(theEvent) == true)
					return;
				base.RightMouseDown(theEvent);
				Handler?.TriggerMouseCallback();
			}

			public override void OtherMouseDown(NSEvent theEvent)
			{
				if (Handler?.HandleMouseEvent(theEvent) == true)
					return;
				base.OtherMouseDown(theEvent);
				Handler?.TriggerMouseCallback();
			}

			public override void MouseDown(NSEvent theEvent)
			{
				var h = Handler;
				if (h == null)
				{
					base.MouseDown(theEvent);
					return;
				}
				if (h.HandleMouseEvent(theEvent))
					return;

				h.IsMouseDragging = true;
				base.MouseDown(theEvent);
				h.IsMouseDragging = false;
				h.TriggerMouseCallback();
			}

			public EtoTableView(GridViewHandler handler)
			{
				FocusRingType = NSFocusRingType.None;
				WeakDataSource = new EtoTableViewDataSource { Handler = handler };
				WeakDelegate = new EtoTableDelegate { Handler = handler };
				ColumnAutoresizingStyle = NSTableViewColumnAutoresizingStyle.Uniform;
				SetDraggingSourceOperationMask(NSDragOperation.All, true);
				SetDraggingSourceOperationMask(NSDragOperation.All, false);
			}

			[Export("draggingSession:sourceOperationMaskForDraggingContext:")]
			public NSDragOperation DraggingSessionSourceOperationMask(NSDraggingSession session, IntPtr context)
			{
				return Handler?.DragInfo?.AllowedOperation ?? NSDragOperation.None;
			}

			[Export("ignoreModifierKeysForDraggingSession:")]
			public bool IgnoreModifierKeysForDraggingSession(NSDraggingSession session) => true;

			public override void Layout()
			{
				Handler?.PerformLayout();
				base.Layout();
			}
			
			public override bool ValidateProposedFirstResponder(NSResponder responder, NSEvent forEvent)
			{
				var valid = base.ValidateProposedFirstResponder(responder, forEvent);
				return Handler?.ValidateProposedFirstResponder(responder, forEvent, valid) ?? valid;
			}

#if MACOS_NET
			public override NSImage DragImageForRows(NSIndexSet dragRows, NSTableColumn[] tableColumns, NSEvent dragEvent, ref CGPoint dragImageOffset)
			{
				var dragInfo = Handler?.DragInfo;
				var img = dragInfo?.DragImage;
				if (img != null)
				{
					dragImageOffset = dragInfo.GetDragImageOffset();
					return img;
				}
				return base.DragImageForRows(dragRows, tableColumns, dragEvent, ref dragImageOffset);
			}
#else

			static readonly IntPtr selDragImageForRowsWithIndexes_TableColumns_Event_Offset_Handle = Selector.GetHandle("dragImageForRowsWithIndexes:tableColumns:event:offset:");

			[Export("dragImageForRowsWithIndexes:tableColumns:event:offset:")]
			public NSImage DragImageForRows(NSIndexSet dragRows, NSTableColumn[] tableColumns, NSEvent dragEvent, ref CGPoint dragImageOffset)
			{
				var dragInfo = Handler?.DragInfo;
				var img = dragInfo?.DragImage;
				if (img != null)
				{
					dragImageOffset = dragInfo.GetDragImageOffset();
					return img;
				}

				NSArray nSArray = NSArray.FromNSObjects(tableColumns);
				NSImage result = Runtime.GetNSObject<NSImage>(Messaging.IntPtr_objc_msgSendSuper_IntPtr_IntPtr_IntPtr_ref_CGPoint(SuperHandle, selDragImageForRowsWithIndexes_TableColumns_Event_Offset_Handle, dragRows.Handle, nSArray.Handle, dragEvent.Handle, ref dragImageOffset));
				nSArray.Dispose();
				return result;
			}
#endif
		}

		public class EtoTableViewDataSource : NSTableViewDataSource
		{
			WeakReference handler;
			
			public GridViewHandler Handler { get => (GridViewHandler)handler?.Target; set => handler = new WeakReference(value); }

			public override nint GetRowCount(NSTableView tableView) => Handler?.collection?.Count ?? 0;

			public override NSObject GetObjectValue(NSTableView tableView, NSTableColumn tableColumn, nint row)
			{
				var h = Handler;
				if (h == null)
					return null;
					
				if (row >= h.collection.Count)
				{
					// re-jig as we're off somehow.. usually because of some programming error, but let's be nice and not actually crash.
					tableView.ReloadData();
					return null;
				}
				var colHandler = h.GetColumn(tableColumn);
				var item = h.collection.ElementAt((int)row);
				return colHandler?.GetObjectValue(item);
			}

			public override void SetObjectValue(NSTableView tableView, NSObject theObject, NSTableColumn tableColumn, nint row)
			{
				var h = Handler;
				if (h == null)
					return;

				if (row >= h.collection.Count)
					return;
					
				var item = h.collection.ElementAt((int)row);
				var colHandler = h.GetColumn(tableColumn);
				if (colHandler != null && h.SuppressUpdate == 0)
				{
					colHandler.SetObjectValue(item, theObject);

					h.SetIsEditing(false);
					h.Callback.OnCellEdited(h.Widget, new GridViewCellEventArgs(colHandler.Widget, (int)row, colHandler.Column, item));
				}
			}

			public override NSDragOperation ValidateDrop(NSTableView tableView, NSDraggingInfo info, nint row, NSTableViewDropOperation dropOperation)
			{
				var h = Handler;
				if (h == null)
					return NSDragOperation.None;
				var etoInfo = GetDragInfo(info, row, dropOperation);
				var e = h.GetDragEventArgs(info, etoInfo);
				h.Callback.OnDragOver(h.Widget, e);
				if (e.AllowedEffects.HasFlag(e.Effects))
				{
					var idx = etoInfo.Index;
					if (idx >= 0)
					{
						var pos = etoInfo.Position;
						if (pos == GridDragPosition.After)
						{
							pos = GridDragPosition.Before;
							idx++;
						}
						tableView.SetDropRowDropOperation((nint)idx, pos == GridDragPosition.Over ? NSTableViewDropOperation.On : NSTableViewDropOperation.Above);
					}
					else
					{
						tableView.SetDropRowDropOperation(-1, NSTableViewDropOperation.On);
					}

					return e.Effects.ToNS();
				}
				else
					return NSDragOperation.None;
			}

			GridViewDragInfo GetDragInfo(NSDraggingInfo info, nint row, NSTableViewDropOperation dropOperation)
			{
				var h = Handler;
				var tableView = h.Control;
				var position = GridDragPosition.Over;
				if (dropOperation == NSTableViewDropOperation.On)
					position = GridDragPosition.Over;
				else
				{
					// need to check if it's actually above or below the item under the cursor
					var rowUnderMouse = tableView.GetRow(tableView.ConvertPointFromView(info.DraggingLocation, null));
					if (row == rowUnderMouse)
					{
						position = GridDragPosition.Before;
					}
					else
					{
						position = GridDragPosition.After;
						if (row > 0)
							row--;
					}
				}
				var item = row >= 0 ? h.GetItem((int)row) : null;

				return new GridViewDragInfo(h.Widget, item, (int)row, position);
			}

			public override bool AcceptDrop(NSTableView tableView, NSDraggingInfo info, nint row, NSTableViewDropOperation dropOperation)
			{
				var h = Handler;
				if (h == null)
					return false;
				var e = h.GetDragEventArgs(info, GetDragInfo(info, row, dropOperation));
				h.Callback.OnDragLeave(h.Widget, e);
				h.Callback.OnDragDrop(h.Widget, e);
				return true;
			}

			public override void DraggingSessionEnded(NSTableView tableView, NSDraggingSession draggingSession, CGPoint endedAtScreenPoint, NSDragOperation operation)
			{
				var h = Handler;
				if (h == null)
					return;

				if (h.CustomSelectedRows != null)
				{
					h.CustomSelectedRows = null;
					h.Callback.OnSelectionChanged(h.Widget, EventArgs.Empty);
				}

				var allowedOperation = h.DragInfo?.AllowedOperation ?? NSDragOperation.None;
				var data = h.DragInfo?.Data;
				var args = new DragEventArgs(h.Widget, data, allowedOperation.ToEto(), endedAtScreenPoint.ToEto(h.ContainerControl), Keyboard.Modifiers, Mouse.Buttons);
				args.Effects = operation.ToEto();
				h.Callback.OnDragEnd(h.Widget, args);
			}

			public override bool WriteRows(NSTableView tableView, NSIndexSet rowIndexes, NSPasteboard pboard)
			{
				var h = Handler;
				if (h == null)
					return false;

				if (h.IsMouseDragging)
				{
					h.DragInfo = null;
					// give MouseMove event a chance to start the drag
					h.DragPasteboard = pboard;


					// check if the dragged rows are different than the selection so we can fire a changed event
					var dragRows = rowIndexes.Select(r => (int)r).ToList();
					bool isDifferentSelection = (nint)dragRows.Count != h.Control.SelectedRowCount;
					if (!isDifferentSelection)
					{
						// same count, ensure they're not different rows
						// typically only tests one entry here, as there's no way to drag more than a single non-selected item.
						var selectedRows = h.Control.SelectedRows.ToArray();
						for (var i = 0; i < selectedRows.Length; i++)
						{
							if (!dragRows.Contains((int)selectedRows[i]))
							{
								isDifferentSelection = true;
								break;
							}
						}
					}

					if (isDifferentSelection)
					{
						h.CustomSelectedRows = dragRows;
						h.Callback.OnSelectionChanged(h.Widget, EventArgs.Empty);
					}

					var args = MacConversions.GetMouseEvent(h, NSApplication.SharedApplication.CurrentEvent, false);
					h.Callback.OnMouseMove(h.Widget, args);
					h.DragPasteboard = null;

					return h.DragInfo != null;
				}

				return false;
			}
		}

		public class EtoTableDelegate : NSTableViewDelegate
		{
			WeakReference handler;
			
			public GridViewHandler Handler { get { return (GridViewHandler)(handler != null ? handler.Target : null); } set { handler = new WeakReference(value); } }

			public override bool ShouldEditTableColumn(NSTableView tableView, NSTableColumn tableColumn, nint row)
			{
				var h = Handler;
				if (h == null)
					return false;
					
				var colHandler = h.GetColumn(tableColumn);
				var item = h.collection.ElementAt((int)row);
				var args = new GridViewCellEventArgs(colHandler.Widget, (int)row, colHandler.Column, item);
				h.Callback.OnCellEditing(h.Widget, args);
				h.SetIsEditing(true);
				return true;
			}
			
			NSIndexSet previouslySelected;
			public override void SelectionDidChange(NSNotification notification)
			{
				var h = Handler;
				if (h == null)
					return;

				// didn't start a drag (when this was set), so clear this out when the selection changes
				h.CustomSelectedRows = null;

				if (h.SuppressSelectionChanged == 0)
				{
					h.Callback.OnSelectionChanged(h.Widget, EventArgs.Empty);
					var columns = NSIndexSet.FromNSRange(new NSRange(0, h.Control.TableColumns().Length));
					if (previouslySelected?.Count > 0)
						h.Control.ReloadData(previouslySelected, columns);
					var selected = h.Control.SelectedRows;
					if (selected?.Count > 0)
						h.Control.ReloadData(selected, columns);
					previouslySelected = selected;
				}
			}

			public override nfloat GetSizeToFitColumnWidth(NSTableView tableView, nint column)
			{
				var h = Handler;
				if (h == null)
					return 20;
				var columns = tableView.TableColumns();
				if (column >= (int)columns.Length)
					return 20;
				var colHandler = h.GetColumn(columns[column]);
				if (colHandler != null)
				{
					// turn on autosizing for this column again
					colHandler.AutoSize = true;
					h.DidSetAutoSizeColumn = true;
					Application.Instance.AsyncInvoke(() => h.DidSetAutoSizeColumn = false);
					return colHandler.GetPreferredWidth();
				}
				return 20;
			}

			public override void DidClickTableColumn(NSTableView tableView, NSTableColumn tableColumn)
			{
				var h = Handler;
				if (h == null)
					return;
				var colHandler = h.GetColumn(tableColumn);
				if (colHandler.Sortable)
					h.Callback.OnColumnHeaderClick(h.Widget, new GridColumnEventArgs(colHandler.Widget));
			}

			public override void ColumnDidResize(NSNotification notification)
			{
				Handler?.ColumnDidResize(notification);
			}

			public override NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, nint row)
			{
				var h = Handler;
				if (h != null)
				{
					var cellHandler = h.GetColumn(tableColumn)?.DataCellHandler;
					if (cellHandler != null)
					{
						return cellHandler.GetViewForItem(tableView, tableColumn, (int)row, null, (obj, r) => h.GetItem(r));
					}
				}

				return tableView.MakeView(tableColumn?.Identifier ?? string.Empty, this);
			}

			public override void DidAddRowView(NSTableView tableView, NSTableRowView rowView, nint row)
			{
				Handler?.OnDidAddRowView(rowView, row);
			}

			public override void DidRemoveRowView(NSTableView tableView, NSTableRowView rowView, nint row)
			{
				var h = Handler;
				if (h == null)
					return;
				foreach (var col in h.ColumnHandlers)
				{
					if (col.DisplayIndex != -1)
					{
						var view = rowView.ViewAtColumn(col.DisplayIndex);
						col.DataCellHandler?.ViewRemoved(view);
					}
				}
			}

			public override void DidDragTableColumn(NSTableView tableView, NSTableColumn tableColumn)
			{
				var h = Handler;
				if (h == null)
					return;
				var column = h.GetColumn(tableColumn);
				h.Callback.OnColumnOrderChanged(h.Widget, new GridColumnEventArgs(column.Widget));
			}
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Grid.CellEditingEvent:
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
				case Grid.CellEditedEvent:
					// handled after object value is set
					break;
				case Grid.SelectionChangedEvent:
					/* handled by delegate, for now
					table.SelectionDidChange += delegate {
						Widget.OnSelectionChanged (EventArgs.Empty);
					};*/
					break;
				case Grid.ColumnHeaderClickEvent:
					/*
					table.DidClickTableColumn += delegate(object sender, NSTableViewTableEventArgs e) {
						var column = Handler.Widget.Columns.First (r => object.ReferenceEquals (r.ControlObject, tableColumn));
						Handler.Widget.OnHeaderClick (new GridColumnEventArgs (column));
					};
					*/
					break;
				case Grid.CellDoubleClickEvent:
				case Grid.CellClickEvent:
					// Handled in EtoTableView
					break;
				case Grid.CellFormattingEvent:
					break;
				case Grid.RowFormattingEvent:
					break;
				case Eto.Forms.Control.DragOverEvent:
				case Eto.Forms.Control.DragDropEvent:
					// handled in EtoTableViewDataSource
					break;
				case Grid.ColumnOrderChangedEvent:
					// handled in EtoTableDelegate
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		protected override EtoTableView CreateControl()
		{
			return new EtoTableView(this);
		}

		public IEnumerable<object> SelectedItems
		{
			get
			{
				if (collection != null)
				{
					foreach (var row in SelectedRows)
						yield return collection.ElementAt(row);
				}
			}
		}

		static readonly object CustomSelectedRows_Key = new object();

		protected IEnumerable<int> CustomSelectedRows
		{
			get { return Widget.Properties.Get<IEnumerable<int>>(CustomSelectedRows_Key); }
			set { Widget.Properties.Set(CustomSelectedRows_Key, value); }
		}


		public override IEnumerable<int> SelectedRows
		{
			get { return CustomSelectedRows ?? base.SelectedRows; }
			set { base.SelectedRows = value; }
		}

		class CollectionHandler : EnumerableChangedHandler<object>
		{
			public GridViewHandler Handler { get; set; }

			protected override void BeginUpdates()
			{
				base.BeginUpdates();
				Handler.Control.BeginUpdates();
			}

			protected override void EndUpdates()
			{
				base.EndUpdates();
				Handler.Control.EndUpdates();
				Handler.AutoSizeColumns(true);
			}

			public override void AddRange(IEnumerable<object> items)
			{
				Handler.Control.ReloadData();
			}

			public override void AddItem(object item)
			{
				Handler.Control.InsertRows(new NSIndexSet(Count), NSTableViewAnimation.SlideDown);
			}

			public override void ReplaceItem(int index, object newItem)
			{
				Handler.Control.ReloadData(NSIndexSet.FromIndex((nint)index), NSIndexSet.FromNSRange(new NSRange(0, Handler.Control.TableColumns().Length)));
			}
			
			public override void InsertItem(int index, object item)
			{
				Handler.Control.InsertRows(new NSIndexSet(index), NSTableViewAnimation.SlideDown);
			}

			public override void RemoveItem(int index)
			{
				Handler.Control.RemoveRows(new NSIndexSet(index), NSTableViewAnimation.SlideUp);
			}

			public override void RemoveAllItems()
			{
				Handler.Control.ReloadData();
			}
		}

		public IEnumerable<object> DataStore
		{
			get => collection?.Collection;
			set
			{
				if (collection != null)
				{
					if (ReferenceEquals(collection.Collection, value))
						return;
					collection.Unregister();
				}
				collection = new CollectionHandler { Handler = this };
				collection.Register(value);
				Control.ReloadData();
				AutoSizeColumns(true);
				ResetAutoSizedColumns();
				InvalidateMeasure();
			}
		}

		public override object GetItem(int row)
		{
			if (row == -1 || collection == null || row >= collection.Count)
				return null;
			return collection.ElementAt(row);
		}

		public void ReloadData(IEnumerable<int> rows)
		{
			Control.ReloadData(NSIndexSet.FromArray(rows.Select(r => (nuint)r).ToArray()), NSIndexSet.FromNSRange(new NSRange(0, Control.TableColumns().Length)));
			if (Widget.Columns.Any(r => r.AutoSize))
			{
				AutoSizeColumns(true);
			}
		}

		public GridCell GetCellAt(PointF location)
		{
			int columnIndex;
			int rowIndex;
			object item;
			bool isHeader;

			if (ShowHeader)
			{
				// check if we're over header first, as data can be under the header
				var headerBounds = Control.HeaderView.Bounds.ToEto();
				var nslocation = (location + headerBounds.Location).ToNS();
				columnIndex = (int)Control.HeaderView.GetColumn(nslocation);
				isHeader = columnIndex != -1 || headerBounds.Contains(nslocation.ToEto());
			}
			else
			{
				columnIndex = -1;
				isHeader = false;
			}
			
			// not over header, check where we are in the data cells
			if (!isHeader)
			{
				var nslocation = (location + ScrollView.ContentView.Bounds.Location.ToEto()).ToNS();
				columnIndex = (int)Control.GetColumn(nslocation);
				rowIndex = (int)Control.GetRow(nslocation);
				item = GetItem(rowIndex);
			}
			else
			{
				rowIndex = -1;
				item = null;
			}
			
			GridCellType cellType;
			if (isHeader)
				cellType = GridCellType.ColumnHeader;
			else if (columnIndex != -1 && rowIndex != -1)
				cellType = GridCellType.Data;
			else
				cellType = GridCellType.None;

			columnIndex = DisplayIndexToColumnIndex(columnIndex);
			var column = columnIndex != -1 ? Widget.Columns[columnIndex] : null;
			return new GridCell(column, columnIndex, rowIndex, cellType, item);
		}

		public GridViewDragInfo GetDragInfo(DragEventArgs args) => args.ControlObject as GridViewDragInfo;

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

			// action is called after this, so we can't clear selected rows immediately
			if (CustomSelectedRows != null)
			{
				Application.Instance.AsyncInvoke(() =>
				{
					CustomSelectedRows = null;
					Callback.OnSelectionChanged(Widget, EventArgs.Empty);
				});
			}
		}

		void ContextMenu_Opening(object sender, EventArgs e)
		{
			var row = (int)Control.ClickedRow;
			if (row != -1 && !SelectedRows.Contains(row))
			{
				var menu = (ContextMenu)sender;
				menu.Closed += ContextMenu_Closed;
				CustomSelectedRows = new[] { row };
				Callback.OnSelectionChanged(Widget, EventArgs.Empty);
			}
		}
	}
}

