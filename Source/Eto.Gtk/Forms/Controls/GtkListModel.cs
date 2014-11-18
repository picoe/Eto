using System;
using Eto.Forms;

namespace Eto.GtkSharp.Forms.Controls
{
	public interface IGtkListModelHandler<TItem, TStore>
		where TStore: IDataStore<TItem>
		where TItem: class
	{
		TStore DataStore { get; }
		
		int NumberOfColumns { get; }
		
		GLib.Value GetColumnValue (TItem item, int column, int row);

		int GetRowOfItem (TItem item);
	}

	public class GtkListModel<TItem, TStore> : GLib.Object, ITreeModelImplementor
		where TStore: class, IDataStore<TItem>
		where TItem: class
	{
		WeakReference handler;
		public IGtkListModelHandler<TItem, TStore> Handler { get { return (IGtkListModelHandler<TItem, TStore>)handler.Target; } set { handler = new WeakReference(value); } }

		public Gtk.TreeIter GetIterAtRow (int row)
		{
			return new Gtk.TreeIter { UserData = (IntPtr)(row+1) };
		}

		public Gtk.TreePath GetPathAtRow (int row)
		{
			var path = new Gtk.TreePath ();
			path.AppendIndex (row);
			return path;
		}
		
		public TItem GetItemAtPath (Gtk.TreePath path)
		{
			var row = GetRow (path);
			return row >= 0 ? Handler.DataStore[row] : default(TItem);
		}

		public TItem GetItemAtPath (string path)
		{
			return GetItemAtPath (new Gtk.TreePath (path));
		}
		
		public TItem GetItemAtIter (Gtk.TreeIter iter)
		{
			var node = NodeFromIter (iter);
			return node >= 0 ? Handler.DataStore[node] : default(TItem);
		}

		int GetRow (Gtk.TreePath path)
		{
			var h = Handler;
			if (h != null && path.Indices.Length > 0 && h.DataStore != null && h.DataStore.Count > 0)
				return path.Indices[0];
			return -1;
		}

		public Gtk.TreeModelFlags Flags {
			get { return Gtk.TreeModelFlags.ListOnly; }
		}

		public int NColumns {
			get { return Handler.NumberOfColumns; }
		}

		public GLib.GType GetColumnType (int col)
		{
			GLib.GType result = GLib.GType.String;
			return result;
		}

		public int NodeFromIter (Gtk.TreeIter iter)
		{
			return ((int)iter.UserData) - 1;
		}

		public bool GetIter (out Gtk.TreeIter iter, Gtk.TreePath path)
		{
			if (path == null)
				throw new ArgumentNullException ("path");

				
			var row = GetRow (path);
			if (row >= 0) {
				iter = new Gtk.TreeIter { UserData = (IntPtr)(row + 1) };
				return true;
			}
			iter = Gtk.TreeIter.Zero;
			return false;
		}

		public Gtk.TreePath GetPath (Gtk.TreeIter iter)
		{
			var node = NodeFromIter (iter);

			var path = new Gtk.TreePath ();
			path.AppendIndex (node);
			return path;
		}

		public void GetValue (Gtk.TreeIter iter, int col, ref GLib.Value val)
		{
			var row = ((int)iter.UserData) - 1;
			if (row >= 0) {
				var item = Handler.DataStore [row];
				val = Handler.GetColumnValue (item, col, row);
			} else 
				val = Handler.GetColumnValue (null, col, row);

		}

		public bool IterNext (ref Gtk.TreeIter iter)
		{
			var row = ((int)iter.UserData) - 1;
			if (row >= 0 && Handler.DataStore != null && row < Handler.DataStore.Count - 1) {
				iter = new Gtk.TreeIter { UserData = (IntPtr)(row + 2) };
				return true;
			}
			iter = Gtk.TreeIter.Zero;
			return false;
		}

		public bool IterPrevious (ref Gtk.TreeIter iter)
		{
			var row = (int)iter.UserData - 1;
			if (row > 0) {
				iter = new Gtk.TreeIter { UserData = (IntPtr)(row) };
				return true;
			}
			iter = Gtk.TreeIter.Zero;
			return false;
		}

		public bool IterChildren (out Gtk.TreeIter child, Gtk.TreeIter parent)
		{
			if (parent.UserData == IntPtr.Zero && Handler.DataStore != null && Handler.DataStore.Count > 0) {
				child = new Gtk.TreeIter { UserData = (IntPtr)1 };
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
			var h = Handler;
			if (iter.UserData == IntPtr.Zero && h.DataStore != null)
				return h.DataStore.Count;
			return 0;
		}

		public bool IterNthChild (out Gtk.TreeIter child, Gtk.TreeIter parent, int n)
		{
			var h = Handler;
			if (parent.UserData == IntPtr.Zero && h != null && h.DataStore != null) {
				if (n < h.DataStore.Count) {
					child = new Gtk.TreeIter { UserData = (IntPtr)(n+1) };
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
