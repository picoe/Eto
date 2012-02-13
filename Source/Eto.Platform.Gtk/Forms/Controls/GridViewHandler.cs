using System;
using Eto.Forms;
using System.Runtime.InteropServices;
using Eto.Drawing;

namespace Eto.Platform.GtkSharp.Forms.Controls
{
	public class GridViewHandler : GtkControl<Gtk.ScrolledWindow, GridView>, IGridView, ICellDataSource
	{
		Gtk.TreeView tree;
		EtoTreeModel model;
		IGridStore store;
		
		class EtoNode
		{
			public IGridItem Item { get; set; }

			public int Row { get; set; }
		}
		
		class EtoTreeModel : GLib.Object, Gtk.TreeModelImplementor
		{
			public GridViewHandler Handler { get; set; }

			public EtoTreeModel ()
			{
			}
			
			public IGridItem GetItemAtPath (string path)
			{
				var node = GetNodeAtPath (new Gtk.TreePath (path));
				if (node != null)
					return node.Item;
				return null;
			}

			EtoNode GetNodeAtPath (Gtk.TreePath path)
			{
				if (path.Indices.Length > 0 && Handler.store != null) {
					var row = path.Indices [0];
					var item = Handler.store.GetItem (row);
					return new EtoNode { Item = item, Row = row };
				} else
					return null;
			}

			public Gtk.TreeModelFlags Flags {
				get { return Gtk.TreeModelFlags.ListOnly; }
			}

			public int NColumns {
				get { return Handler.Widget != null ? Handler.Widget.Columns.Count : 0; }
			}

			public GLib.GType GetColumnType (int col)
			{
				GLib.GType result = GLib.GType.String;
				return result;
			}
			
			Gtk.TreeIter IterFromNode (int row)
			{
				return IterFromNode (new EtoNode { Item = Handler.store.GetItem (row), Row = row });
			}

			Gtk.TreeIter IterFromNode (EtoNode node)
			{
				var gch = GCHandle.Alloc (node);
				Gtk.TreeIter result = Gtk.TreeIter.Zero;
				result.UserData = (IntPtr)gch;
				return result;
			}

			EtoNode NodeFromIter (Gtk.TreeIter iter)
			{
				var gch = (GCHandle)iter.UserData;
				return gch.Target as EtoNode;
			}

			public bool GetIter (out Gtk.TreeIter iter, Gtk.TreePath path)
			{
				if (path == null)
					throw new ArgumentNullException ("path");


				var node = GetNodeAtPath (path);
				if (node != null) {
					iter = IterFromNode (node);
					return true;
				}
				iter = Gtk.TreeIter.Zero;
				return false;
			}

			public Gtk.TreePath GetPath (Gtk.TreeIter iter)
			{
				var node = NodeFromIter (iter);
				if (node == null) 
					throw new ArgumentException ("iter");
				
				var path = new Gtk.TreePath ();
				path.AppendIndex (node.Row);
				return path;
			}

			public void GetValue (Gtk.TreeIter iter, int col, ref GLib.Value val)
			{
				var node = NodeFromIter (iter);
				if (node != null) {
					var nodevalue = node.Item.GetValue (col);
					if (nodevalue != null) {
						val = new GLib.Value (nodevalue);
						return;
					}
				}
				
				var colHandler = (GridColumnHandler)Handler.Widget.Columns[col].Handler;
				colHandler.GetNullValue(ref val);
			}

			public bool IterNext (ref Gtk.TreeIter iter)
			{
				var node = NodeFromIter (iter);
				if (node != null && Handler.store != null && node.Row < Handler.store.Count - 1) {
					iter = IterFromNode (node.Row + 1);
					return true;
				}
				iter = Gtk.TreeIter.Zero;
				return false;
			}

			public bool IterPrevious (ref Gtk.TreeIter iter)
			{
				var node = NodeFromIter (iter);
				if (node != null && node.Row > 0) {
					iter = IterFromNode (node.Row - 1);
					return true;
				}
				iter = Gtk.TreeIter.Zero;
				return false;
			}

			public bool IterChildren (out Gtk.TreeIter child, Gtk.TreeIter parent)
			{
				if (parent.UserData == IntPtr.Zero && Handler.store != null && Handler.store.Count > 0) {
					child = IterFromNode (0);
					return true;
				}
				child = Gtk.TreeIter.Zero;
				return false;
			}

			public bool IterHasChild (Gtk.TreeIter iter)
			{
				return false;
			}

			public int IterNChildren (Gtk.TreeIter iter)
			{
				if (iter.UserData == IntPtr.Zero && Handler.store != null)
					return Handler.store.Count;
				else
					return 0;
			}

			public bool IterNthChild (out Gtk.TreeIter child, Gtk.TreeIter parent, int n)
			{
				if (parent.UserData == IntPtr.Zero && Handler.store != null) {
					if (n >= Handler.store.Count)
						return false;
					child = IterFromNode (n);
					return true;
				}
				return false;
			}

			public bool IterParent (out Gtk.TreeIter parent, Gtk.TreeIter child)
			{
				return false;
			}

			public void RefNode (Gtk.TreeIter iter)
			{
			}

			public void UnrefNode (Gtk.TreeIter iter)
			{
			}
		}
		
		public GridViewHandler ()
		{
			model = new EtoTreeModel{ Handler = this };
			tree = new Gtk.TreeView (new Gtk.TreeModelAdapter (model));
			tree.HeadersVisible = true;
			
			Control = new Gtk.ScrolledWindow ();
			Control.ShadowType = Gtk.ShadowType.In;
			Control.Add (tree);
		}

		public void InsertColumn (int index, GridColumn item)
		{
			var colHandler = ((GridColumnHandler)item.Handler);
			if (index >= 0)
				tree.InsertColumn (colHandler.Control, index);
			else
				index = tree.AppendColumn (colHandler.Control);
			colHandler.BindCell (this, index);
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
				model = new EtoTreeModel{ Handler = this };
				tree.Model = new Gtk.TreeModelAdapter (model);
			}
		}

		public void SetValue (string path, int column, object value)
		{
			IGridItem item = model.GetItemAtPath (path);
			if (item != null)
				item.SetValue (column, value);
		}
	}
}

