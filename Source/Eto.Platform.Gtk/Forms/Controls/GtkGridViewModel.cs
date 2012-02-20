using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using System.Runtime.InteropServices;

namespace Eto.Platform.GtkSharp.Forms.Controls
{
	public class EtoNode
	{
		public IGridItem Item { get; set; }

		public int Row { get; set; }
	}

	public class GtkGridViewModel : GLib.Object, Gtk.TreeModelImplementor
	{
		public GridViewHandler Handler { get; set; }

		public GtkGridViewModel (IntPtr ptr)
			: base (ptr)
		{
		}

		public GtkGridViewModel ()
			: base ()
		{
		}

		public IGridItem GetItemAtPath (Gtk.TreePath path)
		{
			var node = GetNodeAtPath (path);
			if (node != null)
				return node.Item;
			return null;
		}

		public IGridItem GetItemAtPath (string path)
		{
			return GetItemAtPath (new Gtk.TreePath (path));
		}

		EtoNode GetNodeAtPath (Gtk.TreePath path)
		{
			if (path.Indices.Length > 0 && Handler.store != null) {
				var row = path.Indices[0];
				var item = Handler.store.GetItem (row);
				return new EtoNode { Item = item, Row = row };
			}
			else
				return null;
		}

		public Gtk.TreeModelFlags Flags
		{
			get { return Gtk.TreeModelFlags.ListOnly; }
		}

		public int NColumns
		{
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
			colHandler.GetNullValue (ref val);
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
				if (n < Handler.store.Count) {
					child = IterFromNode (n);
					return true;
				}
			}
			child = Gtk.TreeIter.Zero;
			return false;
		}

		public bool IterParent (out Gtk.TreeIter parent, Gtk.TreeIter child)
		{
			parent = Gtk.TreeIter.Zero;
			return false;
		}

		public void RefNode (Gtk.TreeIter iter)
		{
		}

		public void UnrefNode (Gtk.TreeIter iter)
		{
		}
	}
}
