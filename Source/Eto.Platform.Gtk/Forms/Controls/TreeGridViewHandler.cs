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
				var store = Handler.collection.DataStore;
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
				path.AppendIndex (DataStore.Count);
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
			get { return collection != null ? collection.DataStore : null; }
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
				//Control.Selection.SelectPath (iter);
			}
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
					Widget.OnExpanded (e);
				};
				break;
			case TreeGridView.CollapsingEvent:
				this.Tree.TestCollapseRow += delegate(object o, Gtk.TestCollapseRowArgs args) {
					var e = new TreeGridViewItemCancelEventArgs(GetItem(args.Path) as ITreeGridItem);
					Widget.OnCollapsing (e);
					args.RetVal = e.Cancel;
				};
				break;
			case TreeGridView.CollapsedEvent:
				this.Tree.RowCollapsed += delegate(object o, Gtk.RowCollapsedArgs args) {
					var e = new TreeGridViewItemEventArgs(GetItem(args.Path) as ITreeGridItem);
					Widget.OnCollapsed (e);
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


	}
}

