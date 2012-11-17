using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using System.Runtime.InteropServices;
using System.Collections;

namespace Eto.Platform.GtkSharp
{
	public class GtkTreeModel<T, S> : GLib.Object, Gtk.TreeModelImplementor
		where S: class, IDataStore<T>
		where T: class, ITreeItem<T>
	{

		public IGtkListModelHandler<T, S> Handler { get; set; }

		public GtkTreeModel ()
		{
		}

		class Node
		{
			public T Item { get; set; }

			public int[] Indices { get; set; }

		}
		
		Node GetNodeAtPath (Gtk.TreePath path)
		{
			if (path.Indices.Length > 0) {
				S item = (S)Handler.DataStore;
				for (int i = 0; i < path.Indices.Length; i++) {
					var idx = path.Indices [i];
					item = (S)(object)item [idx];
				}
				var node = new Node ();
				node.Item = (T)(object)item;
				node.Indices = path.Indices;
				return node;
			}
			return null;
		}
		
		public T GetItemAtPath (Gtk.TreePath path)
		{
			if (path.Indices.Length > 0) {
				var store = (S)Handler.DataStore;
				if (store != null) {
					for (int i = 0; i < path.Indices.Length; i++) {
						var idx = path.Indices [i];
						if (idx < store.Count)
							store = (S)(object)store [idx];
						else
							return null;
					}
					return (T)(object)store;
				}
			}
			return default(T);
		}

		public T GetItemAtPath (string path)
		{
			return GetItemAtPath (new Gtk.TreePath (path));
		}

		IEnumerable<T> GetParents (T item)
		{
			T parent = item.Parent;
			while (parent != null) {
				yield return parent;
				parent = parent.Parent;
			}
		}

		public Gtk.TreePath GetPathFromItem (T item)
		{
			var path = new Gtk.TreePath();
			var parents = GetParents (item);
			foreach (var parent in parents) {
				S items = (S)(object)parent;
				var found = false;
				for (int i = 0; i < items.Count; i++) {
					if (object.ReferenceEquals (items[i], item)) {
						path.PrependIndex (i);
						item = parent;
						found = true;
						break;
					}
				}
				if (!found)
					return null;
			}
			return path;
		}

		public Gtk.TreeIter GetIterFromItem (T item, Gtk.TreePath path)
		{
			GCHandle gch;
			var node = new Node{ Item = item, Indices = path.Indices };
			gch = GCHandle.Alloc (node);
			Gtk.TreeIter result = Gtk.TreeIter.Zero;
			result.UserData = (IntPtr)gch;
			return result;
		}

		Node GetNodeAtIter (Gtk.TreeIter iter)
		{
			GCHandle gch = (GCHandle)iter.UserData;
			return (Node)gch.Target;
		}
		
		public T GetItemAtIter (Gtk.TreeIter iter)
		{
			var node = GetNodeAtIter (iter);
			return node.Item;
		}
		
		public Gtk.TreeModelFlags Flags {
			get { return Gtk.TreeModelFlags.ItersPersist; }
		}

		public int NColumns {
			get { return Handler.NumberOfColumns; }
		}

		public GLib.GType GetColumnType (int col)
		{
			GLib.GType result = GLib.GType.String;
			return result;
		}

		public bool GetIter (out Gtk.TreeIter iter, Gtk.TreePath path)
		{
			if (path == null)
				throw new ArgumentNullException ("path");
			
			var item = GetItemAtPath (path);
			if (item != null) {
				iter = GetIterFromItem (item, path);
				return true;
			}
			iter = Gtk.TreeIter.Zero;
			return false;
		}

		public Gtk.TreePath GetPath (Gtk.TreeIter iter)
		{
			if (iter.UserData == IntPtr.Zero)
				return new Gtk.TreePath ();
			var node = GetNodeAtIter (iter);
			
			return new Gtk.TreePath(node.Indices);
			
		}

		public void GetValue (Gtk.TreeIter iter, int col, ref GLib.Value val)
		{
			var node = GetNodeAtIter (iter);
			if (node != null) {
				var row = node.Indices.Sum ();
				if (node.Item != null) {
					val = Handler.GetColumnValue (node.Item, col, row);
					return;
				}
			}

			val = Handler.GetColumnValue (null, col, -1);

		}

		public bool IterNext (ref Gtk.TreeIter iter)
		{
			var node = GetNodeAtIter (iter);
			if (node == null)
				return false;

			var indices = node.Indices;
			if (indices.Length > 0) {
				var store = GetParentStore (node);
				var row = indices.Last ();
				if (row >= 0 && store != null && row < store.Count - 1) {
					var newpath = (int[])indices.Clone ();
					newpath [newpath.Length - 1] = row + 1;
					iter = GetIterFromItem ((T)store [row + 1], new Gtk.TreePath (newpath));
					return true;
			
				}
			}

			iter = Gtk.TreeIter.Zero;
			return false;
		}

		public bool IterPrevious (ref Gtk.TreeIter iter)
		{
			var node = GetNodeAtIter (iter);

			var indices = node.Indices;
			var store = GetParentStore (node);
			var row = indices.Last ();
			if (row > 0 && store != null) {
				var newpath = (int[])indices;
				newpath [newpath.Length - 1] = row - 1;
				iter = GetIterFromItem ((T)store [row - 1], new Gtk.TreePath (newpath));
				return true;
			}
			iter = Gtk.TreeIter.Zero;
			return false;
		}

		S GetStore (Gtk.TreeIter item)
		{
			if (item.UserData == IntPtr.Zero) {
				return (S)Handler.DataStore;
			} else {
				var node = GetNodeAtIter (item);
				return (S)(object)node.Item;
			}
		}
		
		S GetParentStore (Node node)
		{

			if (node.Indices.Length <= 1)
				return (S)Handler.DataStore;
			else
				return (S)(object)node.Item.Parent;
		}

		public bool IterChildren (out Gtk.TreeIter child, Gtk.TreeIter parent)
		{
			S store;
			Gtk.TreePath path;
			if (parent.UserData == IntPtr.Zero) {
				store = (S)Handler.DataStore;
				path = new Gtk.TreePath ();
			} else {
				var node = GetNodeAtIter (parent);
				path = new Gtk.TreePath(node.Indices);
				store = (S)(object)node.Item;
			}
			path.AppendIndex (0);

			if (store != null) {
				child = GetIterFromItem ((T)store [0], path);
				return true;
			}
			child = Gtk.TreeIter.Zero;
			return false;
		}
		
		public bool IterHasChild (Gtk.TreeIter iter)
		{
			S store;
			if (iter.UserData == IntPtr.Zero) {
				store = (S)Handler.DataStore;
			} else {
				var node = GetNodeAtIter (iter);
				store = (S)(object)node.Item;
			}

			if (store != null) {
				return store.Count > 0;
			}
			return false;
		}

		public int IterNChildren (Gtk.TreeIter iter)
		{
			var node = GetNodeAtIter (iter);
			var store = GetStore (iter);
			if (store != null && (node == null || node.Item.Expandable))
				return store.Count;
			else
				return 0;
		}
		
		public bool IterNthChild (out Gtk.TreeIter child, Gtk.TreeIter parent, int n)
		{
			var store = GetStore (parent);
			if (store != null) {
				var path = GetPath (parent).Copy ();
				path.AppendIndex (n);
				var item = (T)store [n];
				child = GetIterFromItem (item, path);
				return true;
			}

			child = Gtk.TreeIter.Zero;
			return false;
		}

		public bool IterParent (out Gtk.TreeIter parent, Gtk.TreeIter child)
		{
			var node = GetNodeAtIter (child);
			if (node != null && node.Indices.Length > 1) {
				var indices = new int[node.Indices.Length - 1];
				Array.Copy (node.Indices, indices, indices.Length);
				parent = GetIterFromItem((T)node.Item.Parent, new Gtk.TreePath(indices));
				return true;
			}
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
