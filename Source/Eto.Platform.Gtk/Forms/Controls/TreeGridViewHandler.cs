using System;
using System.Linq;
using Eto.Forms;
using Eto.Drawing;
using Eto.Platform.GtkSharp.Drawing;
using System.Collections.Generic;
using Eto.Platform.GtkSharp.Forms.Cells;

namespace Eto.Platform.GtkSharp.Forms.Controls
{
	public class TreeGridViewHandler : GridHandler<TreeGridView>, ITreeGridView, ICellDataSource, IGtkListModelHandler<ITreeGridItem, ITreeGridStore<ITreeGridItem>>
	{
		GtkTreeModel<ITreeGridItem, ITreeGridStore<ITreeGridItem>> model;
		CollectionHandler collection;
		bool? selectCollapsingItem;
		ITreeGridItem lastSelected;

		public override void Initialize ()
		{
			base.Initialize ();

			// these are always handled to set the expanded property
			Widget.HandleEvent (TreeGridView.ExpandedEvent, TreeGridView.CollapsedEvent, TreeGridView.CollapsingEvent);
		}

		protected override Gtk.TreeModelImplementor CreateModelImplementor ()
		{
			model = new GtkTreeModel<ITreeGridItem, ITreeGridStore<ITreeGridItem>> { Handler = this };
			return model;
		}

		public class CollectionHandler : DataStoreChangedHandler<ITreeGridItem, ITreeGridStore<ITreeGridItem>>
		{
			public TreeGridViewHandler Handler { get; set; }
			
			void ExpandItems(ITreeGridStore<ITreeGridItem> store, Gtk.TreePath path)
			{
				for (int i = 0; i < store.Count; i++) {
					var item = store[i];
					if (item.Expandable && item.Expanded) {
						var newpath = path.Copy ();
						newpath.AppendIndex (i);
						Handler.Tree.ExpandToPath (newpath);
						ExpandItems ((ITreeGridStore<ITreeGridItem>)item, newpath);
					}
				}
			}
			
			void ExpandItems()
			{
				var store = Handler.collection.Collection;
				Gtk.TreePath path = new Gtk.TreePath();
				ExpandItems (store, path);
			}

			public override void AddRange (IEnumerable<ITreeGridItem> items)
			{
				Handler.UpdateModel ();
				ExpandItems();
			}

			public override void AddItem (ITreeGridItem item)
			{
				var path = new Gtk.TreePath ();
				path.AppendIndex (Collection.Count);
				var iter = Handler.model.GetIterFromItem (item, path);
				Handler.Tree.Model.EmitRowInserted (path, iter);
			}

			public override void InsertItem (int index, ITreeGridItem item)
			{
				var path = new Gtk.TreePath ();
				path.AppendIndex (index);
				var iter = Handler.model.GetIterFromItem (item, path);
				Handler.Tree.Model.EmitRowInserted (path, iter);
			}

			public override void RemoveItem (int index)
			{
				var path = new Gtk.TreePath ();
				path.AppendIndex (index);
				Handler.Tree.Model.EmitRowDeleted (path);
			}

			public override void RemoveAllItems ()
			{
				Handler.UpdateModel ();
			}
		}

		public ITreeGridStore<ITreeGridItem> DataStore {
			get { return collection != null ? collection.Collection : null; }
			set {
				if (collection != null)
					collection.Unregister ();
				collection = new CollectionHandler { Handler = this };
				collection.Register (value);
			}
		}

		public ITreeGridItem SelectedItem {
			get {
				Gtk.TreeIter iter;
				if (Tree.Selection.GetSelected (out iter)) {
					return model.GetItemAtIter (iter);
				}
				return null;
			}
			set {
				if (value != null) {
					var path = model.GetPathFromItem(value);
					if (path != null) {
						Tree.ExpandToPath(path);
						Tree.Selection.SelectPath (path);
						Tree.ScrollToCell(path, null, false, 0, 0);
					}
				}
				else
					Tree.Selection.UnselectAll ();
			}
		}

		bool ChildIsSelected (ITreeGridItem item)
		{
			var node = this.SelectedItem;
			
			while (node != null) {
				if (node == item)
					return true;
				node = node.Parent;
			}
			return false;
		}

		
		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case TreeGridView.ExpandingEvent:
				this.Tree.TestExpandRow += delegate(object o, Gtk.TestExpandRowArgs args) {
					var e = new TreeGridViewItemCancelEventArgs(GetItem(args.Path) as ITreeGridItem);
					Widget.OnExpanding (e);
					args.RetVal = e.Cancel;
				};
				break;
			case TreeGridView.ExpandedEvent:
				this.Tree.RowExpanded += delegate(object o, Gtk.RowExpandedArgs args) {
					var e = new TreeGridViewItemEventArgs(GetItem(args.Path) as ITreeGridItem);
					e.Item.Expanded = true;
					Widget.OnExpanded (e);
				};
				break;
			case TreeGridView.CollapsingEvent:
				this.Tree.TestCollapseRow += delegate(object o, Gtk.TestCollapseRowArgs args) {
					var e = new TreeGridViewItemCancelEventArgs(GetItem(args.Path) as ITreeGridItem);
					Widget.OnCollapsing (e);
					args.RetVal = e.Cancel;
					if (!e.Cancel)
					{
						selectCollapsingItem = AllowMultipleSelection ? false : ChildIsSelected (e.Item);
						SkipSelectedChange = true;
					}
				};
				break;
			case TreeGridView.CollapsedEvent:
				this.Tree.RowCollapsed += delegate(object o, Gtk.RowCollapsedArgs args) {
					var e = new TreeGridViewItemEventArgs(GetItem(args.Path) as ITreeGridItem);
					e.Item.Expanded = false;
					Widget.OnCollapsed (e);
					SkipSelectedChange = false;
					if (selectCollapsingItem == true)
					{
						Tree.Selection.UnselectAll ();
						Tree.Selection.SelectPath(args.Path);
						selectCollapsingItem = null;
					}
				};
				break;
			case TreeGridView.SelectedItemChangedEvent:
				this.Tree.Selection.Changed += (o, args) => {
					var item = this.SelectedItem;
					if (!SkipSelectedChange && !object.ReferenceEquals (item, lastSelected))
					{
						Widget.OnSelectedItemChanged (EventArgs.Empty);
						lastSelected = item;
					}
				};
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
		}

		public override object GetItem (Gtk.TreePath path)
		{
			return model.GetItemAtPath (path);
		}

		public override Gtk.TreeIter GetIterAtRow (int row)
		{
			throw new NotImplementedException ();
		}

		public GLib.Value GetColumnValue (ITreeGridItem item, int dataColumn, int row)
		{
			int column;
			if (ColumnMap.TryGetValue (dataColumn, out column)) {
				var colHandler = (IGridColumnHandler)Widget.Columns[column].Handler;
				return colHandler.GetValue (item, dataColumn, row);
			}
			return new GLib.Value ((string)null);
		}

		public int GetRowOfItem (ITreeGridItem item)
		{
			if (collection == null) return -1;
			return collection.IndexOf (item);
		}

		int GetCount (Gtk.TreeIter parent, int upToIndex)
		{
			int rows = upToIndex == -1 ? model.IterNChildren(parent) : upToIndex;
			int count = 0;
			for (int i = 0; i < rows; i ++)
			{
				Gtk.TreeIter iter;
				if (model.IterNthChild(out iter, parent, i)) {
					var childPath = model.GetPath (iter);
					
					if (Tree.GetRowExpanded(childPath))
					{
						count += GetCount (iter, -1);
					}
				}
				count++;
			}
			return count;
		}

		public override IEnumerable<int> SelectedRows
		{
			get
			{
				var rows = Tree.Selection.GetSelectedRows ();
				foreach (var row in rows)
				{
					int count = 0;
					Gtk.TreePath path = new Gtk.TreePath();
					count += GetCount (Gtk.TreeIter.Zero, row.Indices[0]);
					// slow but works for now
					for (int i = 0; i < row.Indices.Length-1; i++)
					{
						path.AppendIndex (row.Indices[i]);
						Gtk.TreeIter iter;
						if (model.GetIter (out iter, path))
							count += GetCount (iter, row.Indices[i+1]);
					}
					count += row.Indices.Length - 1;
					//count += row.Indices[row.Indices.Length - 1];

					yield return count;
				}
				
			}
		}


	}
}

