using System;
using System.Linq;
using Eto.Forms;
using Eto.Drawing;
using Eto.Platform.GtkSharp.Drawing;
using System.Collections.Generic;

namespace Eto.Platform.GtkSharp.Forms.Controls
{
	public class TreeViewHandler : GtkControl<Gtk.ScrolledWindow, TreeView>, ITreeView, IDataViewHandler, ICellDataSource, IGtkListModelHandler<ITreeItem, ITreeStore<ITreeItem>>
	{
		GtkTreeModel<ITreeItem, ITreeStore<ITreeItem>> model;
		Gtk.TreeView tree;
		ContextMenu contextMenu;
		CollectionHandler collection;
		public static Size MaxImageSize = new Size (16, 16);
		ColumnCollection columnCollection;
		Dictionary<int, int> columnMap = new Dictionary<int, int> ();
		int numDataColumns;
		
		public TreeViewHandler ()
		{
			model = new GtkTreeModel<ITreeItem, ITreeStore<ITreeItem>>{ Handler = this };
			tree = new Gtk.TreeView (new Gtk.TreeModelAdapter (model));

			tree.RowActivated += delegate(object o, Gtk.RowActivatedArgs args) {
				this.Widget.OnActivated (new TreeViewItemEventArgs (model.GetItemAtPath (args.Path)));
			};
			
			tree.ShowExpanders = true;
			
			Control = new Gtk.ScrolledWindow ();
			Control.ShadowType = Gtk.ShadowType.In;
			Control.Add (tree);
			
			tree.Events |= Gdk.EventMask.ButtonPressMask;
			tree.ButtonPressEvent += HandleTreeButtonPressEvent;
		}

		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case TreeView.SelectionChangedEvent:
				tree.Selection.Changed += delegate {
					this.Widget.OnSelectionChanged (EventArgs.Empty);
				};
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
		}
		
		class ColumnCollection : EnumerableChangedHandler<TreeColumn, TreeColumnCollection>
		{
			public TreeViewHandler Handler { get; set; }
			
			void RebindColumns ()
			{
				Handler.columnMap.Clear ();
				int columnIndex = 0;
				int dataIndex = 0;
				foreach (var col in Handler.Widget.Columns.Select (r => r.Handler).OfType<IDataColumnHandler>()) {
					col.BindCell (Handler, Handler, columnIndex++, ref dataIndex);
				}
				Handler.numDataColumns = dataIndex;
			}
			
			public override void AddItem (TreeColumn item)
			{
				var colhandler = (TreeColumnHandler)item.Handler;
				Handler.tree.AppendColumn (colhandler.Control);
				RebindColumns ();
			}

			public override void InsertItem (int index, TreeColumn item)
			{
				var colhandler = (TreeColumnHandler)item.Handler;
				if (Handler.tree.Columns.Length > 0)
					Handler.tree.InsertColumn (colhandler.Control, index);
				else
					Handler.tree.AppendColumn (colhandler.Control);
				RebindColumns ();
			}

			public override void RemoveItem (int index)
			{
				var colhandler = (TreeColumnHandler)Handler.Widget.Columns [index].Handler;
				Handler.tree.RemoveColumn (colhandler.Control);
				RebindColumns ();
			}

			public override void RemoveAllItems ()
			{
				foreach (var col in Handler.tree.Columns) {
					Handler.tree.RemoveColumn (col);
				}
				RebindColumns ();
			}

		}
		
		public override void Initialize ()
		{
			base.Initialize ();
			columnCollection = new ColumnCollection { Handler = this };
			columnCollection.Register (Widget.Columns);
		}

		[GLib.ConnectBefore]
		void HandleTreeButtonPressEvent (object o, Gtk.ButtonPressEventArgs args)
		{
			if (contextMenu != null && args.Event.Button == 3 && args.Event.Type == Gdk.EventType.ButtonPress) {
				var menu = ((ContextMenuHandler)contextMenu.Handler).Control;
				menu.Popup ();
				menu.ShowAll ();
			}
		}
		
		public class CollectionHandler : DataStoreChangedHandler<ITreeItem, ITreeStore<ITreeItem>>
		{
			public TreeViewHandler Handler { get; set; }
			
			void ExpandItems(ITreeStore<ITreeItem> store, Gtk.TreePath path)
			{
				for (int i = 0; i < store.Count; i++) {
					var item = store[i];
					if (item.Expandable && item.Expanded) {
						var newpath = path.Copy ();
						newpath.AppendIndex (i);
						Handler.tree.ExpandToPath (newpath);
						ExpandItems ((ITreeStore<ITreeItem>)item, newpath);
					}
				}
			}
			
			void ExpandItems()
			{
				var store = Handler.collection.DataStore;
				Gtk.TreePath path = new Gtk.TreePath();
				ExpandItems (store, path);
			}

			public override void AddRange (IEnumerable<ITreeItem> items)
			{
				Handler.model = new GtkTreeModel<ITreeItem, ITreeStore<ITreeItem>>{ Handler = this.Handler };
				Handler.tree.Model = new Gtk.TreeModelAdapter (Handler.model);
				ExpandItems();
			}

			public override void AddItem (ITreeItem item)
			{
				var path = new Gtk.TreePath ();
				path.AppendIndex (DataStore.Count);
				var iter = Handler.model.GetIterFromItem (item, path);
				Handler.tree.Model.EmitRowInserted (path, iter);
			}

			public override void InsertItem (int index, ITreeItem item)
			{
				var path = new Gtk.TreePath ();
				path.AppendIndex (index);
				var iter = Handler.model.GetIterFromItem (item, path);
				Handler.tree.Model.EmitRowInserted (path, iter);
			}

			public override void RemoveItem (int index)
			{
				var path = new Gtk.TreePath ();
				path.AppendIndex (index);
				Handler.tree.Model.EmitRowDeleted (path);
			}

			public override void RemoveAllItems ()
			{
				Handler.model = new GtkTreeModel<ITreeItem, ITreeStore<ITreeItem>>{ Handler = this.Handler };
				Handler.tree.Model = new Gtk.TreeModelAdapter (Handler.model);
			}
		}
		
		public ITreeStore<ITreeItem> DataStore {
			get { return collection != null ? collection.DataStore : null; }
			set {
				if (collection != null)
					collection.Unregister ();
				collection = new CollectionHandler { Handler = this };
				collection.Register (value);
			}
		}

		public ContextMenu ContextMenu {
			get { return contextMenu; }
			set { contextMenu = value; }
		}
		
		public ITreeItem SelectedItem {
			get {
				Gtk.TreeIter iter;
				if (tree.Selection.GetSelected (out iter)) {
					return model.GetItemAtIter (iter);
				}
				return null;
			}
			set {
				//Control.Selection.SelectPath (iter);
			}
		}
		
		public bool ShowHeader {
			get { return tree.HeadersVisible; }
			set { tree.HeadersVisible = value; }
		}

		public object GetItem (string path)
		{
			return model.GetItemAtPath (path);
		}

		public void EndCellEditing (string path, int column)
		{
			
		}

		public void BeginCellEditing (string path, int column)
		{
			
		}

		public void SetColumnMap (int dataIndex, int column)
		{
			columnMap [dataIndex] = column;
		}

		public GLib.Value GetColumnValue (ITreeItem item, int dataColumn)
		{
			int column;
			if (columnMap.TryGetValue (dataColumn, out column)) {
				var colHandler = (IDataColumnHandler)Widget.Columns [column].Handler;
				return colHandler.GetValue (item, dataColumn);
			}
			return new GLib.Value ((string)null);
		}

		public int NumberOfColumns {
			get { return numDataColumns; }
		}
	}
}

