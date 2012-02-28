using System;
using Eto.Forms;
using System.Runtime.InteropServices;
using Eto.Drawing;
using System.Linq;
using System.Collections.Generic;

namespace Eto.Platform.GtkSharp.Forms.Controls
{

	public class GridViewHandler : GtkControl<Gtk.ScrolledWindow, GridView>, IGridView, ICellDataSource, IGtkListModelHandler<IGridItem, IGridStore>
	{
		Gtk.TreeView tree;
		GtkListModel<IGridItem, IGridStore> model;
		CollectionHandler collection;
		ContextMenu contextMenu;
		
		public GridViewHandler ()
		{
			Control = new Gtk.ScrolledWindow {
				ShadowType = Gtk.ShadowType.In
			};
		}
		
		public override void Initialize ()
		{
			base.Initialize ();
			model = new GtkListModel<IGridItem, IGridStore>{ Handler = this };
			tree = new Gtk.TreeView (new Gtk.TreeModelAdapter (model));
			tree.HeadersVisible = true;

			Control.Add (tree);

			tree.Events |= Gdk.EventMask.ButtonPressMask;
			tree.ButtonPressEvent += HandleTreeButtonPressEvent;
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
		
		public override void AttachEvent (string handler)
		{
			switch (handler) {
				case GridView.BeginCellEditEvent:
				case GridView.EndCellEditEvent:
					SetupColumnEvents ();
					break;
				case GridView.SelectionChangedEvent:
					tree.Selection.Changed += delegate {
						Widget.OnSelectionChanged (EventArgs.Empty);
					};
					break;
				default:
					base.AttachEvent (handler);
					break;
			}
		}
		
		public override void OnLoadComplete (EventArgs e)
		{
			base.OnLoadComplete (e);
			tree.AppendColumn (new Gtk.TreeViewColumn());
		}

		void SetupColumnEvents ()
		{
			foreach (var col in Widget.Columns.Select (r => r.Handler).OfType<GridColumnHandler> ()) {
				col.SetupEvents ();
			}
		}

		public void InsertColumn (int index, GridColumn item)
		{
			var colHandler = ((GridColumnHandler)item.Handler);
			if (index >= 0 && tree.Columns.Length > 0)
				tree.InsertColumn (colHandler.Control, index);
			else
				index = tree.AppendColumn (colHandler.Control) - 1;
			colHandler.BindCell (this, this, index);
		}

		public void RemoveColumn (int index, GridColumn item)
		{
			var colHandler = ((GridColumnHandler)item.Handler);
			tree.RemoveColumn (colHandler.Control);
		}

		public void ClearColumns ()
		{
			foreach (var col in tree.Columns) {
				tree.RemoveColumn (col);
			}
		}

		public bool ShowHeader {
			get { return tree.HeadersVisible; }
			set { tree.HeadersVisible = value; }
		}
		
		public bool AllowColumnReordering {
			get { return tree.Reorderable; }
			set { tree.Reorderable = value; }
		}
		
		public class CollectionHandler : CollectionChangedHandler<IGridItem, IGridStore>
		{
			public GridViewHandler Handler { get; set; }
			

			public override void AddRange (IEnumerable<IGridItem> items)
			{
				Handler.model = new GtkListModel<IGridItem, IGridStore>{ Handler = this.Handler };
				Handler.tree.Model = new Gtk.TreeModelAdapter (Handler.model);
			}

			public override void AddItem (IGridItem item)
			{
				var iter = Handler.model.GetIterAtRow (DataStore.Count);
				var path = Handler.model.GetPathAtRow (DataStore.Count);
				Handler.tree.Model.EmitRowInserted (path, iter);
			}

			public override void InsertItem (int index, IGridItem item)
			{
				var iter = Handler.model.GetIterAtRow (index);
				var path = Handler.model.GetPathAtRow (index);
				Handler.tree.Model.EmitRowInserted (path, iter);
			}

			public override void RemoveItem (int index)
			{
				var path = Handler.model.GetPathAtRow (index);
				Handler.tree.Model.EmitRowDeleted (path);
			}

			public override void RemoveAllItems ()
			{
				Handler.model = new GtkListModel<IGridItem, IGridStore>{ Handler = Handler };
				Handler.tree.Model = new Gtk.TreeModelAdapter (Handler.model);
			}
		}
		
		public IGridStore DataStore {
			get { return collection != null ? collection.DataStore : null; }
			set {
				if (collection != null)
					collection.Unregister ();
				collection = new CollectionHandler{ Handler = this };
				collection.Register (value);
			}
		}

		public int NumberOfColumns {
			get {
				return Widget.Columns.Count;
			}
		}
		
		public GLib.Value GetColumnValue (IGridItem item, int column)
		{
			var colHandler = (GridColumnHandler)Widget.Columns[column].Handler;
			return colHandler.GetValue (item);
		}
		
		public ContextMenu ContextMenu {
			get { return contextMenu; }
			set { contextMenu = value; }
		}

		public void SetValue (string path, int column, object value)
		{
			IGridItem item = model.GetItemAtPath (path);
			if (item != null)
				item.SetValue (column, value);
		}


		public void EndCellEditing (string path, int column)
		{
			var treePath = new Gtk.TreePath (path);
			var row = treePath.Indices.Length > 0 ? treePath.Indices[0] : -1;
			var item = model.GetItemAtPath (treePath);
			Widget.OnEndCellEdit(new GridViewCellArgs(Widget.Columns[column], row, column, item));
		}

		public void BeginCellEditing (string path, int column)
		{
			var treePath = new Gtk.TreePath (path);
			var row = treePath.Indices.Length > 0 ? treePath.Indices[0] : -1;
			var item = model.GetItemAtPath (treePath);
			Widget.OnBeginCellEdit (new GridViewCellArgs (Widget.Columns[column], row, column, item));
		}


		public bool AllowMultipleSelection
		{
			get { return tree.Selection.Mode == Gtk.SelectionMode.Multiple; }
			set { tree.Selection.Mode = value ? Gtk.SelectionMode.Multiple : Gtk.SelectionMode.Browse; }
		}

		public IEnumerable<int> SelectedRows
		{
			get
			{
				var rows = tree.Selection.GetSelectedRows ();
				foreach (var row in rows) {
					yield return row.Indices[0];
				}
			}
		}

		public void SelectAll ()
		{
			tree.Selection.SelectAll ();
		}

		public void SelectRow (int row)
		{
			tree.Selection.SelectIter (model.GetIterAtRow (row));
		}

		public void UnselectRow (int row)
		{
			tree.Selection.UnselectIter (model.GetIterAtRow (row));
		}

		public void UnselectAll ()
		{
			tree.Selection.UnselectAll ();
		}
	}
}

