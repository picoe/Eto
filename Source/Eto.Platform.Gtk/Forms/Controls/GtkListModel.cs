using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using System.Runtime.InteropServices;

namespace Eto.Platform.GtkSharp
{
	public interface IGtkListModelHandler<T, S>
		where S: IDataStore<T>
		where T: class
	{
		S DataStore { get; }
		
		int NumberOfColumns { get; }
		
		GLib.Value GetColumnValue (T item, int column);
	}

	public class GtkListModel<T, S> : GLib.Object, Gtk.TreeModelImplementor
		where S: IDataStore<T>
		where T: class
	{

		public IGtkListModelHandler<T, S> Handler { get; set; }

		public GtkListModel ()
		{
		}
		
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
		
		public T GetItemAtPath (Gtk.TreePath path)
		{
			var row = GetRow (path);
			if (row >= 0)
				return Handler.DataStore [row];
			return default(T);
		}

		public T GetItemAtPath (string path)
		{
			return GetItemAtPath (new Gtk.TreePath (path));
		}
		
		public T GetItemAtIter (Gtk.TreeIter iter)
		{
			var node = NodeFromIter (iter);
			if (node >= 0)
				return Handler.DataStore [node];
			return default(T);
		}

		int GetRow (Gtk.TreePath path)
		{
			if (path.Indices.Length > 0 && Handler.DataStore != null && Handler.DataStore.Count > 0)
				return path.Indices [0];
			else
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
				val = Handler.GetColumnValue (item, col);
			} else 
				val = Handler.GetColumnValue (null, col);

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
			if (iter.UserData == IntPtr.Zero && Handler.DataStore != null)
				return Handler.DataStore.Count;
			else
				return 0;
		}

		public bool IterNthChild (out Gtk.TreeIter child, Gtk.TreeIter parent, int n)
		{
			if (parent.UserData == IntPtr.Zero && Handler.DataStore != null) {
				if (n < Handler.DataStore.Count) {
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
