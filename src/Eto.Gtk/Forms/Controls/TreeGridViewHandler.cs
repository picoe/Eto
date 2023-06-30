using Eto.GtkSharp.Forms.Cells;
namespace Eto.GtkSharp.Forms.Controls
{
	public class TreeGridViewHandler : GridHandler<TreeGridView, TreeGridView.ICallback>, TreeGridView.IHandler, ICellDataSource, IGtkTreeModelHandler<ITreeGridItem, ITreeGridStore<ITreeGridItem>>
	{
		protected new TreeGridView.ICallback Callback { get { return (TreeGridView.ICallback)base.Callback; } }

		GtkTreeModel<ITreeGridItem, ITreeGridStore<ITreeGridItem>> model;
		CollectionHandler collection;
		bool? selectCollapsingItem;
		ITreeGridItem lastSelected;
		int suppressExpandCollapseEvents;

		protected override void Initialize()
		{
			base.Initialize();

			// these are always handled to set the expanded property
			HandleEvent(TreeGridView.ExpandedEvent);
			HandleEvent(TreeGridView.CollapsedEvent);
			HandleEvent(TreeGridView.CollapsingEvent);
		}

		protected override ITreeModelImplementor CreateModelImplementor()
		{
			model = new GtkTreeModel<ITreeGridItem, ITreeGridStore<ITreeGridItem>> { Handler = this };
			return model;
		}

		public class CollectionHandler : DataStoreChangedHandler<ITreeGridItem, ITreeGridStore<ITreeGridItem>>
		{
			WeakReference handler;
			public TreeGridViewHandler Handler { get { return (TreeGridViewHandler)handler.Target; } set { handler = new WeakReference(value); } }

			public void ExpandItems(ITreeGridStore<ITreeGridItem> store, Gtk.TreePath path)
			{
				if (store == null)
					return;
				for (int i = 0; i < store.Count; i++)
				{
					var item = store[i];
					if (item.Expandable && item.Expanded)
					{
						var newpath = path.Copy();
						newpath.AppendIndex(i);
						Handler.Control.ExpandToPath(newpath);
						ExpandItems((ITreeGridStore<ITreeGridItem>)item, newpath);
					}
				}
			}

			public void ExpandItems()
			{
				var store = Handler.collection.Collection;
				var path = new Gtk.TreePath();
				ExpandItems(store, path);
			}

			public override void AddRange(IEnumerable<ITreeGridItem> items)
			{
				Handler.UpdateModel();
				ExpandItems();
			}

			public override void AddItem(ITreeGridItem item)
			{
				var path = new Gtk.TreePath();
				path.AppendIndex(Collection.Count);
				var iter = Handler.model.GetIterFromItem(item, path);
				Handler.Control.Model.EmitRowInserted(path, iter);
			}

			public override void InsertItem(int index, ITreeGridItem item)
			{
				var path = new Gtk.TreePath();
				path.AppendIndex(index);
				var iter = Handler.model.GetIterFromItem(item, path);
				Handler.Control.Model.EmitRowInserted(path, iter);
			}

			public override void RemoveItem(int index)
			{
				var path = new Gtk.TreePath();
				path.AppendIndex(index);
				Handler.Control.Model.EmitRowDeleted(path);
			}

			public override void RemoveAllItems()
			{
				Handler.UpdateModel();
			}
		}

		public ITreeGridStore<ITreeGridItem> DataStore
		{
			get { return collection != null ? collection.Collection : null; }
			set
			{
				if (collection != null)
					collection.Unregister();
				UnselectAll();
				suppressExpandCollapseEvents++;
				collection = new CollectionHandler { Handler = this };
				collection.Register(value);
				suppressExpandCollapseEvents--;
				EnsureSelection();
			}
		}

		public ITreeGridItem SelectedItem
		{
			get
			{
				if (AllowMultipleSelection)
					return SelectedItems.FirstOrDefault() as ITreeGridItem;
				Gtk.TreeIter iter;
				return Control.Selection.GetSelected(out iter) ? model.GetItemAtIter(iter) : null;
			}
			set
			{
				if (value != null)
				{
					var path = model.GetPathFromItem(value);
					if (path != null)
					{
						Control.ExpandToPath(path);
						Control.Selection.SelectPath(path);
						Control.ScrollToCell(path, null, false, 0, 0);
					}
				}
				else
					Control.Selection.UnselectAll();
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

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TreeGridView.ActivatedEvent:
					Control.RowActivated += Connector.HandleRowActivated;
					break;
				case TreeGridView.ExpandingEvent:
					Control.TestExpandRow += Connector.HandleTestExpandRow;
					break;
				case TreeGridView.ExpandedEvent:
					Control.RowExpanded += Connector.HandleRowExpanded;
					break;
				case TreeGridView.CollapsingEvent:
					Control.TestCollapseRow += Connector.HandleTestCollapseRow;
					break;
				case TreeGridView.CollapsedEvent:
					Control.RowCollapsed += Connector.HandleRowCollapsed;
					break;
				case TreeGridView.SelectedItemChangedEvent:
					Control.Selection.Changed += Connector.HandleSelectionChanged;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		protected new TreeGridViewConnector Connector => (TreeGridViewConnector)base.Connector;

		protected override WeakConnector CreateConnector() => new TreeGridViewConnector();

		protected class TreeGridViewConnector : GridConnector
		{
			public new TreeGridViewHandler Handler { get { return (TreeGridViewHandler)base.Handler; } }

			TreeGridViewDragInfo _dragInfo;

			public void HandleTestExpandRow(object o, Gtk.TestExpandRowArgs args)
			{
				var handler = Handler;
				if (handler == null)
					return;
				if (handler.suppressExpandCollapseEvents > 0)
					return;
				var e = new TreeGridViewItemCancelEventArgs(handler.GetItem(args.Path) as ITreeGridItem);
				handler.Callback.OnExpanding(handler.Widget, e);
				args.RetVal = e.Cancel;
			}

			public void HandleRowExpanded(object o, Gtk.RowExpandedArgs args)
			{
				var handler = Handler;
				if (handler == null)
					return;
				if (handler.suppressExpandCollapseEvents > 0)
					return;
				var e = new TreeGridViewItemEventArgs(handler.GetItem(args.Path) as ITreeGridItem);
				e.Item.Expanded = true;
				handler.suppressExpandCollapseEvents++;
				handler.collection.ExpandItems(e.Item as ITreeGridStore<ITreeGridItem>, args.Path);
				handler.suppressExpandCollapseEvents--;
				handler.Callback.OnExpanded(handler.Widget, e);
			}

			public void HandleTestCollapseRow(object o, Gtk.TestCollapseRowArgs args)
			{
				var handler = Handler;
				if (handler == null)
					return;
				if (handler.suppressExpandCollapseEvents > 0)
					return;
				var e = new TreeGridViewItemCancelEventArgs(handler.GetItem(args.Path) as ITreeGridItem);
				handler.Callback.OnCollapsing(handler.Widget, e);
				args.RetVal = e.Cancel;
				if (!e.Cancel)
				{
					handler.selectCollapsingItem = !handler.AllowMultipleSelection && handler.ChildIsSelected(e.Item);
					handler.SkipSelectedChange = true;
				}
			}

			public void HandleRowCollapsed(object o, Gtk.RowCollapsedArgs args)
			{
				var handler = Handler;
				if (handler == null)
					return;
				if (handler.suppressExpandCollapseEvents > 0)
					return;
				var e = new TreeGridViewItemEventArgs(handler.GetItem(args.Path) as ITreeGridItem);
				e.Item.Expanded = false;
				handler.Callback.OnCollapsed(handler.Widget, e);
				handler.SkipSelectedChange = false;
				if (handler.selectCollapsingItem == true)
				{
					handler.Control.Selection.UnselectAll();
					handler.Control.Selection.SelectPath(args.Path);
					handler.selectCollapsingItem = null;
				}
			}

			public void HandleSelectionChanged(object sender, EventArgs e)
			{
				var handler = Handler;
				if (handler == null)
					return;
				var item = handler.SelectedItem;
				if (!handler.SkipSelectedChange && !object.ReferenceEquals(item, handler.lastSelected))
				{
					handler.Callback.OnSelectedItemChanged(handler.Widget, EventArgs.Empty);
					handler.lastSelected = item;
				}
			}

			public void HandleRowActivated(object o, Gtk.RowActivatedArgs args)
			{
				Handler?.Callback.OnActivated(Handler.Widget, new TreeGridViewItemEventArgs(Handler.model.GetItemAtPath(args.Path)));
			}

			protected override DragEventArgs GetDragEventArgs(Gdk.DragContext context, PointF? location, uint time = 0, object controlObject = null, DataObject data = null)
			{
				var h = Handler;
				var t = h?.Control;
				TreeGridViewDragInfo dragInfo = _dragInfo;
				if (dragInfo == null && location != null)
				{
					if (t.GetDestRowAtPos((int)location.Value.X, (int)location.Value.Y, out var path, out var pos))
					{
						var item = h.model.GetItemAtPath(path);
						var indices = path.Indices;
						var childIndex = indices[indices.Length - 1];
						object parent;
						if (pos == Gtk.TreeViewDropPosition.After && t.GetRowExpanded(path))
						{
							parent = item;
							item = null;
							childIndex = -1;
						}
						else
						{
							parent = path.Up() ? h.model.GetItemAtPath(path) : null;
						}
						dragInfo = new TreeGridViewDragInfo(h.Widget, parent, item, childIndex, pos.ToEto());
					}
				}

				return base.GetDragEventArgs(context, location, time, dragInfo, data);
			}

			public override void HandleDragMotion(object o, Gtk.DragMotionArgs args)
			{
				base.HandleDragMotion(o, args);

				var h = Handler;
				if (h == null)
					return;
				var info = h.GetDragInfo(DragArgs);
				if (info == null)
					return;
				var item = info.Item;
				var insertIndex = info.InsertIndex;
				if (!ReferenceEquals(item, null))
				{
					var path = h.model.GetPathFromItem(item as ITreeGridItem);
					if (path == null)
						return;
					var pos = info.Position.ToGtk();

					// make sure we are in range of child indecies
					var iter = h.model.GetIterFromItem(info.Parent as ITreeGridItem);
					if (iter != null && insertIndex != -1)
					{
						var i = iter.Value;
						var numChildren = h.model.IterNChildren(i);
						if (insertIndex >= numChildren)
						{
							insertIndex = numChildren - 1;
							pos = Gtk.TreeViewDropPosition.After;
							h.model.IterNthChild(out i, i, insertIndex);
							path = h.model.GetPath(i);
						}
					}

					if (path.Depth > 0)
						h.Control.SetDragDestRow(path, pos);
				}
				else if (insertIndex != -1 && !ReferenceEquals(info.Parent, null) && info.Position == GridDragPosition.After)
				{
					var path = h.model.GetPathFromItem(info.Parent as ITreeGridItem);
					if (path == null)
						return;

					path.AppendIndex(0);
					h.Control.SetDragDestRow(path, Gtk.TreeViewDropPosition.Before);
				}

			}

			public override void HandleDragDrop(object o, Gtk.DragDropArgs args)
			{
				// use the info from last drag if it was set
				var info = Handler?.GetDragInfo(DragArgs);
				if (info?.IsChanged == true)
					_dragInfo = info;
				base.HandleDragDrop(o, args);
				_dragInfo = null;
			}
		}

		public override object GetItem(Gtk.TreePath path)
		{
			return model.GetItemAtPath(path);
		}

		public override int GetRowIndexOfPath(Gtk.TreePath path)
		{
			var tempPath = new Gtk.TreePath();
			var item = DataStore;
			int count = item.GetExpandedRowCount(path.Indices[0]);
			// slow but works for now
			for (int i = 0; i < path.Indices.Length - 1; i++)
			{
				item = item[path.Indices[i]] as ITreeGridStore<ITreeGridItem>;
				if (item != null)
					count += item.GetExpandedRowCount(path.Indices[i + 1]);
			}
			count += path.Indices.Length - 1;
			//count += path.Indices[row.Indices.Length - 1];

			return count;
		}

		public override Gtk.TreeIter GetIterAtRow(int row)
		{
			Gtk.TreeIter iter;
			model.GetIter(out iter, GetPathAtRow(row));
			return iter;
		}

		public override Gtk.TreePath GetPathAtRow(int row)
		{
			Gtk.TreePath path;
			Gtk.TreeIter iter;
			Gtk.TreeIter temp;

			bool valid = Control.Model.GetIterFirst(out iter);
			while (valid)
			{
				// Check
				path = Control.Model.GetPath(iter);
				if (model.GetRowIndexOfIter(iter) == row)
					return path;

				// Go Down
				if (Control.GetRowExpanded(path) && Control.Model.IterChildren(out iter, iter))
					continue;

				// Go Next
				temp = iter;
				if (Control.Model.IterNext(ref iter))
					continue;
				else
					iter = temp;

				while (valid)
				{
					// Go Up
					if (Control.Model.IterParent(out iter, iter))
					{
						// Go Next
						temp = iter;
						if (Control.Model.IterNext(ref iter))
							break;
						else
							iter = temp;
					}
					else
						valid = false;
				}
			}

			// Get and return first if given row does not exist
			Control.Model.GetIterFirst(out iter);
			return Control.Model.GetPath(iter);
		}

		protected override void SetSelectedRows(IEnumerable<int> value)
		
		{
			Control.Selection.UnselectAll();
			var dataStore = DataStore;
			if (value != null && dataStore != null)
			{
				int start = -1;
				int end = -1;
				var count = dataStore.GetExpandedRowCount();

				foreach (var row in value.Where(r => r < count).OrderBy(r => r))
				{
					if (start == -1)
						start = end = row;
					else if (row == end + 1)
						end = row;
					else
					{
						if (start == end)
							Control.Selection.SelectIter(GetIterAtRow(start));
						else
							Control.Selection.SelectRange(GetPathAtRow(start), GetPathAtRow(end));
						start = end = row;
					}
				}
				if (start != -1)
				{
					if (start == end)
						Control.Selection.SelectIter(GetIterAtRow(start));
					else
						Control.Selection.SelectRange(GetPathAtRow(start), GetPathAtRow(end));
				}
			}
		}

		public GLib.Value GetColumnValue(ITreeGridItem item, int dataColumn, int row, Gtk.TreeIter iter)
		{
			// yes, we can get the row.. but it slows down the TreeGridView too much when there are many items
			// This is only used when formatting the cell, and all other platforms return row=-1 with TreeGridView
			if (dataColumn == RowDataColumn)
				return new GLib.Value(row); //model.GetRowIndexOfIter(iter));

			if (dataColumn == ItemDataColumn)
				return new GLib.Value(item);

			int column;
			if (ColumnMap.TryGetValue(dataColumn, out column))
			{
				var colHandler = (GridColumnHandler)Widget.Columns[column].Handler;
				return colHandler.GetValue(item, dataColumn, row);
			}
			return new GLib.Value((string)null);
		}

		public int GetRowOfItem(ITreeGridItem item)
		{
			return collection == null ? -1 : collection.IndexOf(item);
		}
		

		public void ReloadData()
		{
			// save selected items
			var items = SelectedItems.ToArray();
			// save scroll state
			var scrollState = SaveScrollState();

			// reload data and expand items
			suppressExpandCollapseEvents++;
			UpdateModel();
			collection.ExpandItems();
			suppressExpandCollapseEvents--;

			// restore selection
			SkipSelectedChange = true;
			bool selectionChanged = false;
			Control.Selection.UnselectAll();
			foreach (var item in items.OfType<ITreeGridItem>())
			{
				var iter = model.GetIterFromItem(item, true);
				if (iter != null)
					Control.Selection.SelectIter(iter.Value);
				else
					selectionChanged = true;
			}
			if (selectionChanged)
			{
				Callback.OnSelectionChanged(Widget, EventArgs.Empty);
			}
			SkipSelectedChange = false;
			RestoreScrollState(scrollState);
		}

		public void ReloadItem(ITreeGridItem item, bool reloadChildren)
		{
			var tree = Control;
			var path = model.GetPathFromItem(item);
			if (path != null && path.Depth > 0 && !ReferenceEquals(item, collection.Collection))
			{
				suppressExpandCollapseEvents++;
				var wasExpanded = tree.GetRowExpanded(path);

				Gtk.TreeIter iter;
				tree.Model.GetIter(out iter, path);
				if (item.Expandable)
				{
					tree.Model.EmitRowChanged(path, iter);
					if (reloadChildren || item.Expanded != wasExpanded)
					{
						tree.Model.EmitRowHasChildToggled(path, iter);
						tree.CollapseRow(path);
						if (item.Expanded)
						{
							tree.ExpandRow(path, false);
							collection.ExpandItems((ITreeGridStore<ITreeGridItem>)item, path);
						}
					}
				}
				else if (wasExpanded)
				{
					// it was expanded (and had children), but now it won't be.
					// Gtk requires that we know at this time how many children we have to remove, but instead let's 
					// just delete this node and re-add it.
					// EmitRowHasChildToggled should have done this IMO, but I guess it does not.
					tree.Model.EmitRowDeleted(path);
					tree.Model.EmitRowInserted(path, iter);
				}
				else
				{
					tree.Model.EmitRowChanged(path, iter);
				}
				suppressExpandCollapseEvents--;
			}
			else
				ReloadData();
		}

		public TreeGridCell GetCellAt(PointF location)
		{
			int columnIndex;
			int rowIndex;
			object item;
			GridCellType cellType;

			var isData = Control.GetPathAtPos((int)location.X, (int)location.Y, out var path, out var col);
			
			if (isData)
			{
				columnIndex = GetColumnIndex(col);
				rowIndex = GetRowIndexOfPath(path);
				item = model.GetItemAtPath(path);
				if (columnIndex == -1)
					cellType = GridCellType.None;
				else
					cellType = GridCellType.Data;
			}
			else
			{
				columnIndex = -1;
				rowIndex = -1;
				item = null;
				cellType = GridCellType.None;
			}
			var column = columnIndex != -1 ? Widget.Columns[columnIndex] : null;
			return new TreeGridCell(column, columnIndex, cellType, item);
		}

		public TreeGridViewDragInfo GetDragInfo(DragEventArgs args) => args.ControlObject as TreeGridViewDragInfo;

		ITreeGridItem IGtkListModelHandler<ITreeGridItem>.GetItem(int row) => DataStore?[row];

		public override IEnumerable<int> SelectedRows => Control.Selection.GetSelectedRows().Select(GetRowIndexOfPath);

		public IEnumerable<object> SelectedItems => Control.Selection.GetSelectedRows().Select(GetItem);

		protected override bool HasRows => model.IterHasChild(Gtk.TreeIter.Zero);

		public int Count => DataStore?.Count ?? 0;
	}
}
