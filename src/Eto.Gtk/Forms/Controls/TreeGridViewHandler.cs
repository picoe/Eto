using System;
using Eto.Forms;
using System.Collections.Generic;
using Eto.GtkSharp.Forms.Cells;
using System.Linq;
using Eto.Drawing;

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
						Handler.Tree.ExpandToPath(newpath);
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
				Handler.Tree.Model.EmitRowInserted(path, iter);
			}

			public override void InsertItem(int index, ITreeGridItem item)
			{
				var path = new Gtk.TreePath();
				path.AppendIndex(index);
				var iter = Handler.model.GetIterFromItem(item, path);
				Handler.Tree.Model.EmitRowInserted(path, iter);
			}

			public override void RemoveItem(int index)
			{
				var path = new Gtk.TreePath();
				path.AppendIndex(index);
				Handler.Tree.Model.EmitRowDeleted(path);
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
				collection = new CollectionHandler { Handler = this };
				collection.Register(value);
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
				return Tree.Selection.GetSelected(out iter) ? model.GetItemAtIter(iter) : null;
			}
			set
			{
				if (value != null)
				{
					var path = model.GetPathFromItem(value);
					if (path != null)
					{
						Tree.ExpandToPath(path);
						Tree.Selection.SelectPath(path);
						Tree.ScrollToCell(path, null, false, 0, 0);
					}
				}
				else
					Tree.Selection.UnselectAll();
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
					Tree.RowActivated += Connector.HandleRowActivated;
					break;
				case TreeGridView.ExpandingEvent:
					Tree.TestExpandRow += Connector.HandleTestExpandRow;
					break;
				case TreeGridView.ExpandedEvent:
					Tree.RowExpanded += Connector.HandleRowExpanded;
					break;
				case TreeGridView.CollapsingEvent:
					Tree.TestCollapseRow += Connector.HandleTestCollapseRow;
					break;
				case TreeGridView.CollapsedEvent:
					Tree.RowCollapsed += Connector.HandleRowCollapsed;
					break;
				case TreeGridView.SelectedItemChangedEvent:
					Tree.Selection.Changed += Connector.HandleSelectionChanged;
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
				var h = Handler;
				if (h.suppressExpandCollapseEvents > 0)
					return;
				var e = new TreeGridViewItemCancelEventArgs(h.GetItem(args.Path) as ITreeGridItem);
				h.Callback.OnExpanding(h.Widget, e);
				args.RetVal = e.Cancel;
			}

			public void HandleRowExpanded(object o, Gtk.RowExpandedArgs args)
			{
				var h = Handler;
				if (h.suppressExpandCollapseEvents > 0)
					return;
				var e = new TreeGridViewItemEventArgs(h.GetItem(args.Path) as ITreeGridItem);
				e.Item.Expanded = true;
				h.suppressExpandCollapseEvents++;
				h.collection.ExpandItems(e.Item as ITreeGridStore<ITreeGridItem>, args.Path);
				h.suppressExpandCollapseEvents--;
				h.Callback.OnExpanded(h.Widget, e);
			}

			public void HandleTestCollapseRow(object o, Gtk.TestCollapseRowArgs args)
			{
				var h = Handler;
				if (h.suppressExpandCollapseEvents > 0)
					return;
				var e = new TreeGridViewItemCancelEventArgs(h.GetItem(args.Path) as ITreeGridItem);
				h.Callback.OnCollapsing(h.Widget, e);
				args.RetVal = e.Cancel;
				if (!e.Cancel)
				{
					h.selectCollapsingItem = !h.AllowMultipleSelection && h.ChildIsSelected(e.Item);
					h.SkipSelectedChange = true;
				}
			}

			public void HandleRowCollapsed(object o, Gtk.RowCollapsedArgs args)
			{
				var h = Handler;
				if (h.suppressExpandCollapseEvents > 0)
					return;
				var e = new TreeGridViewItemEventArgs(h.GetItem(args.Path) as ITreeGridItem);
				e.Item.Expanded = false;
				h.Callback.OnCollapsed(h.Widget, e);
				h.SkipSelectedChange = false;
				if (h.selectCollapsingItem == true)
				{
					h.Tree.Selection.UnselectAll();
					h.Tree.Selection.SelectPath(args.Path);
					h.selectCollapsingItem = null;
				}
			}

			public void HandleSelectionChanged(object sender, EventArgs e)
			{
				var h = Handler;
				var item = h.SelectedItem;
				if (!h.SkipSelectedChange && !object.ReferenceEquals(item, h.lastSelected))
				{
					h.Callback.OnSelectedItemChanged(h.Widget, EventArgs.Empty);
					h.lastSelected = item;
				}
			}

			public void HandleRowActivated(object o, Gtk.RowActivatedArgs args)
			{
				Handler.Callback.OnActivated(Handler.Widget, new TreeGridViewItemEventArgs(Handler.model.GetItemAtPath(args.Path)));
			}

			protected override DragEventArgs GetDragEventArgs(Gdk.DragContext context, PointF? location, uint time = 0, object controlObject = null)
			{
				var h = Handler;
				var t = h?.Tree;
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

				return base.GetDragEventArgs(context, location, time, dragInfo);
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
						h.Tree.SetDragDestRow(path, pos);
				}
				else if (insertIndex != -1 && !ReferenceEquals(info.Parent, null) && info.Position == GridDragPosition.After)
				{
					var path = h.model.GetPathFromItem(info.Parent as ITreeGridItem);
					if (path == null)
						return;

					path.AppendIndex(0);
					h.Tree.SetDragDestRow(path, Gtk.TreeViewDropPosition.Before);
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
			int count = GetCount(Gtk.TreeIter.Zero, path.Indices[0]);
			// slow but works for now
			for (int i = 0; i < path.Indices.Length - 1; i++)
			{
				tempPath.AppendIndex(path.Indices[i]);
				Gtk.TreeIter iter;
				if (model.GetIter(out iter, tempPath))
					count += GetCount(iter, path.Indices[i + 1]);
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

			bool valid = Tree.Model.GetIterFirst(out iter);
			while (valid)
			{
				// Check
				path = Tree.Model.GetPath(iter);
				if (GetRowIndexOfPath(path) == row)
					return path;

				// Go Down
				if (Tree.GetRowExpanded(path) && Tree.Model.IterChildren(out iter, iter))
					continue;

				// Go Next
				temp = iter;
				if (Tree.Model.IterNext(ref iter))
					continue;
				else
					iter = temp;

				while (valid)
				{
					// Go Up
					if (Tree.Model.IterParent(out iter, iter))
					{
						// Go Next
						temp = iter;
						if (Tree.Model.IterNext(ref iter))
							break;
						else
							iter = temp;
					}
					else
						valid = false;
				}
			}

			// Get and return first if given row does not exist
			Tree.Model.GetIterFirst(out iter);
			return Tree.Model.GetPath(iter);
		}

		protected int GetRowCount()
		{
			Gtk.TreePath path;
			Gtk.TreeIter iter;
			Gtk.TreeIter temp;

			bool valid = Tree.Model.GetIterFirst(out iter);
			int count = 0;
			while (valid)
			{
				count++;

				// Go Down
				path = Tree.Model.GetPath(iter);
				if (Tree.GetRowExpanded(path) && Tree.Model.IterChildren(out iter, iter))
					continue;

				// Go Next
				temp = iter;
				if (Tree.Model.IterNext(ref iter))
					continue;
				else
					iter = temp;

				// Go Up and Next
				while (true)
				{
					// Go Up
					if (Tree.Model.IterParent(out iter, iter))
					{
						// Go Next
						temp = iter;
						if (Tree.Model.IterNext(ref iter))
							break;
						else
							iter = temp;
					}
					else
						return count;
				}
			}
			return count;
		}

		protected override void SetSelectedRows(IEnumerable<int> value)
		{
			Tree.Selection.UnselectAll();
			if (value != null && collection != null)
			{
				int start = -1;
				int end = -1;
				var count = GetRowCount();

				foreach (var row in value.Where(r => r < count).OrderBy(r => r))
				{
					if (start == -1)
						start = end = row;
					else if (row == end + 1)
						end = row;
					else
					{
						if (start == end)
							Tree.Selection.SelectIter(GetIterAtRow(start));
						else
							Tree.Selection.SelectRange(GetPathAtRow(start), GetPathAtRow(end));
						start = end = row;
					}
				}
				if (start != -1)
				{
					if (start == end)
						Tree.Selection.SelectIter(GetIterAtRow(start));
					else
						Tree.Selection.SelectRange(GetPathAtRow(start), GetPathAtRow(end));
				}
			}
		}

		public GLib.Value GetColumnValue(ITreeGridItem item, int dataColumn, int row, Gtk.TreeIter iter)
		{
			if (dataColumn == RowDataColumn)
			{
				return new GLib.Value(GetRowIndexOfPath(model.GetPath(iter)));
			}

			int column;
			if (ColumnMap.TryGetValue(dataColumn, out column))
			{
				var colHandler = (IGridColumnHandler)Widget.Columns[column].Handler;
				return colHandler.GetValue(item, dataColumn, row);
			}
			return new GLib.Value((string)null);
		}

		public int GetRowOfItem(ITreeGridItem item)
		{
			return collection == null ? -1 : collection.IndexOf(item);
		}

		int GetCount(Gtk.TreeIter parent, int upToIndex)
		{
			int rows = upToIndex == -1 ? model.IterNChildren(parent) : upToIndex;
			int count = 0;
			var path = model.GetPath(parent);
			path.AppendIndex(0);
			for (int i = 0; i < rows; i++)
			{
				Gtk.TreeIter iter;
				if (Tree.GetRowExpanded(path))
				{
					if (model.IterNthChild(out iter, parent, i))
					{
						count += GetCount(iter, -1);
					}
				}
				path.Next();
				count++;
			}
			return count;
		}

		public void ReloadData()
		{
			// save selected items
			var items = SelectedItems.ToArray();
			// save scroll state
			var hscrollbar = Control.HScrollbar as Gtk.HScrollbar;
			var vscrollbar = Control.VScrollbar as Gtk.VScrollbar;
			var hscroll = hscrollbar?.Value;
			var vscroll = vscrollbar?.Value;

			// reload data and expand items
			suppressExpandCollapseEvents++;
			UpdateModel();
			collection.ExpandItems();
			suppressExpandCollapseEvents--;

			// restore selection
			SkipSelectedChange = true;
			bool selectionChanged = false;
			Tree.Selection.UnselectAll();
			foreach (var item in items.OfType<ITreeGridItem>())
			{
				var iter = model.GetIterFromItem(item, true);
				if (iter != null)
					Tree.Selection.SelectIter(iter.Value);
				else
					selectionChanged = true;
			}
			if (selectionChanged)
			{
				Callback.OnSelectionChanged(Widget, EventArgs.Empty);
			}
			SkipSelectedChange = false;
			if (hscroll != null)
				vscrollbar.Value = hscroll.Value;
			if (vscroll != null)
				vscrollbar.Value = vscroll.Value;
		}

		public void ReloadItem(ITreeGridItem item, bool reloadChildren)
		{
			var tree = Tree;
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

		public ITreeGridItem GetCellAt(PointF location, out int column)
		{
			Gtk.TreePath path;
			Gtk.TreeViewColumn col;
			if (Tree.GetPathAtPos((int)location.X, (int)location.Y, out path, out col))
			{
				column = GetColumnOfItem(col);
				return model.GetItemAtPath(path);
			}
			column = -1;
			return null;
		}

		public TreeGridViewDragInfo GetDragInfo(DragEventArgs args) => args.ControlObject as TreeGridViewDragInfo;

		ITreeGridItem IGtkListModelHandler<ITreeGridItem>.GetItem(int row) => DataStore?[row];

		public override IEnumerable<int> SelectedRows => Tree.Selection.GetSelectedRows().Select(GetRowIndexOfPath);

		public IEnumerable<object> SelectedItems => Tree.Selection.GetSelectedRows().Select(GetItem);

		protected override bool HasRows => model.IterHasChild(Gtk.TreeIter.Zero);

		public int Count => DataStore?.Count ?? 0;
	}
}
