using System;
using Eto.Forms;
using System.Runtime.InteropServices;
using Eto.Drawing;
using System.Linq;

namespace Eto.Platform.GtkSharp.Forms.Controls
{

	public class GridViewHandler : GtkControl<Gtk.ScrolledWindow, GridView>, IGridView, ICellDataSource
	{
		Gtk.TreeView tree;
		GtkGridViewModel model;
		internal IGridStore store;
		
		public GridViewHandler ()
		{
			model = new GtkGridViewModel{ Handler = this };
			tree = new Gtk.TreeView (new Gtk.TreeModelAdapter (model));
			tree.HeadersVisible = true;
			
			Control = new Gtk.ScrolledWindow ();
			Control.ShadowType = Gtk.ShadowType.In;
			Control.Add (tree);
		}

		public override void AttachEvent (string handler)
		{
			switch (handler) {
				case GridView.BeginCellEditEvent:
				case GridView.EndCellEditEvent:
					SetupColumnEvents ();
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
			if (index >= 0)
				tree.InsertColumn (colHandler.Control, index);
			else
				index = tree.AppendColumn (colHandler.Control);
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
		
		public IGridStore DataStore {
			get { return store; }
			set {
				store = value;
				model = new GtkGridViewModel{ Handler = this };
				tree.Model = new Gtk.TreeModelAdapter (model);
			}
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
	}
}

