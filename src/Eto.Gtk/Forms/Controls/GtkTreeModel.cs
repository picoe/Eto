using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Forms;
using System.Runtime.InteropServices;

namespace Eto.GtkSharp.Forms.Controls
{
	public interface IGtkTreeModelHandler<TItem, TStore> : IGtkListModelHandler<TItem>
	{
		TStore DataStore { get; }
	}

	public class GtkTreeModel<TItem, TStore> : GLib.Object, ITreeModelImplementor
		where TStore: class, IDataStore<TItem>
		where TItem: class, ITreeItem<TItem>
	{
		WeakReference handler;

		public IGtkTreeModelHandler<TItem, TStore> Handler
		{
			get => (IGtkTreeModelHandler<TItem, TStore>)handler.Target;
			set => handler = new WeakReference(value);
		}

		class Node
		{
			public TItem Item { get; set; }

			public int[] Indices { get; set; }
		}

		Node GetNodeAtPath(Gtk.TreePath path)
		{
			if (path.Indices.Length > 0)
			{
				var item = Handler.DataStore;
				for (int i = 0; i < path.Indices.Length; i++)
				{
					var idx = path.Indices[i];
					item = (TStore)(object)item[idx];
				}
				var node = new Node();
				node.Item = (TItem)(object)item;
				node.Indices = path.Indices;
				return node;
			}
			return null;
		}

		public void UpdateNode(Gtk.TreeIter iter, Gtk.TreePath path)
		{
			var node = GetNodeAtIter(iter);
			if (node != null)
				node.Indices = path.Indices;
		}

		public TItem GetItemAtPath(Gtk.TreePath path)
		{
			var h = Handler;
			if (path.Indices.Length > 0 && h != null)
			{
				var store = h.DataStore;
				if (store != null)
				{
					for (int i = 0; i < path.Indices.Length; i++)
					{
						var idx = path.Indices[i];
						if (idx < store.Count)
							store = (TStore)(object)store[idx];
						else
							return null;
					}
					return (TItem)(object)store;
				}
			}
			return default(TItem);
		}

		public TItem GetItemAtPath(string path)
		{
			return GetItemAtPath(new Gtk.TreePath(path));
		}

		IEnumerable<TStore> GetParents(TItem item)
		{
			var parent = item.Parent;
			while (parent != null)
			{
				yield return parent as TStore;
				parent = parent.Parent;
			}
			if (!ReferenceEquals(parent, Handler.DataStore))
			{
				yield return Handler.DataStore;
			}
		}

		public Gtk.TreePath GetPathFromItem(TItem item)
		{
			var path = new Gtk.TreePath();
			var parents = GetParents(item);
			object cur = item;
			foreach (var parent in parents)
			{
				var items = (TStore)(object)parent;
				var found = false;
				for (int i = 0; i < items.Count; i++)
				{
					if (ReferenceEquals(items[i], cur))
					{
						path.PrependIndex(i);
						cur = parent;
						found = true;
						break;
					}
				}
				if (!found)
					return null;
			}
			return path;
		}

		public Gtk.TreeIter? GetIterFromItem(TItem item, bool expandedOnly = false)
		{
			if (ReferenceEquals(item, null))
				return null;
			var indicies = new List<int>();
			var parents = GetParents(item);
			object cur = item;
			foreach (var parent in parents)
			{
				if (expandedOnly && (parent as TItem)?.Expanded == false)
					return null;
				var items = (TStore)(object)parent;
				var found = false;
				for (int i = 0; i < items.Count; i++)
				{
					if (ReferenceEquals(items[i], cur))
					{
						indicies.Insert(0, i);
						cur = parent;
						found = true;
						break;
					}
				}
				if (!found)
					return null;
			}
			return GetIterFromItem(item, indicies.ToArray());
		}

		public Gtk.TreeIter GetIterFromItem(TItem item, Gtk.TreePath path)
		{
			return GetIterFromItem(item, path.Indices);
		}

		public Gtk.TreeIter GetIterFromItem(TItem item, params int[] indices)
		{
			GCHandle gch;
			var node = new Node{ Item = item, Indices = indices };
			gch = GCHandle.Alloc(node);
			Gtk.TreeIter result = Gtk.TreeIter.Zero;
			result.UserData = (IntPtr)gch;
			return result;
		}

		static Node GetNodeAtIter(Gtk.TreeIter iter)
		{
			if (iter.UserData == IntPtr.Zero)
				return null;
			var gch = (GCHandle)iter.UserData;
			return (Node)gch.Target;
		}

		public TItem GetItemAtIter(Gtk.TreeIter iter)
		{
			var node = GetNodeAtIter(iter);
			return node.Item;
		}

		public Gtk.TreeModelFlags Flags
		{
			get { return Gtk.TreeModelFlags.ItersPersist; }
		}

		public int NColumns
		{
			get { return Handler.NumberOfColumns; }
		}

		public GLib.GType GetColumnType(int col)
		{
			GLib.GType result = GLib.GType.String;
			return result;
		}

		public bool GetIter(out Gtk.TreeIter iter, Gtk.TreePath path)
		{
			if (path == null)
				throw new ArgumentNullException("path");

			var item = GetItemAtPath(path);
			if (item != null)
			{
				iter = GetIterFromItem(item, path);
				return true;
			}
			iter = Gtk.TreeIter.Zero;
			return false;
		}

		public int[] GetIndices(Gtk.TreeIter iter)
		{
			if (iter.UserData == IntPtr.Zero)
				return new int[0];
			var node = GetNodeAtIter(iter);
			return node.Indices;
		}

		public Gtk.TreePath GetPath(Gtk.TreeIter iter)
		{
			if (iter.UserData == IntPtr.Zero)
				return new Gtk.TreePath();
			var node = GetNodeAtIter(iter);

			return new Gtk.TreePath(node.Indices);
			
		}

		public void GetValue(Gtk.TreeIter iter, int col, ref GLib.Value val)
		{
			var node = GetNodeAtIter(iter);
			if (node != null)
			{
				if (node.Item != null)
				{
					val = Handler.GetColumnValue(node.Item, col, -1, iter);
					return;
				}
			}

			val = Handler.GetColumnValue(null, col, -1, iter);

		}

		public bool IterNext(ref Gtk.TreeIter iter)
		{
			var node = GetNodeAtIter(iter);
			if (node == null)
			{
				iter = Gtk.TreeIter.Zero;
				return false;
			}

			var indices = node.Indices;
			if (indices.Length > 0)
			{
				var store = GetParentStore(node);
				var row = indices.Last();
				if (row >= 0 && store != null && row < store.Count - 1)
				{
					var newpath = (int[])indices.Clone();
					newpath[newpath.Length - 1] = row + 1;
					iter = GetIterFromItem(store[row + 1], newpath);
					return true;
				}
			}

			iter = Gtk.TreeIter.Zero;
			return false;
		}

		public bool IterPrevious(ref Gtk.TreeIter iter)
		{
			var node = GetNodeAtIter(iter);

			var indices = node.Indices;
			var store = GetParentStore(node);
			var row = indices.Last();
			if (row > 0 && store != null)
			{
				var newpath = indices;
				newpath[newpath.Length - 1] = row - 1;
				iter = GetIterFromItem(store[row - 1], newpath);
				return true;
			}
			iter = Gtk.TreeIter.Zero;
			return false;
		}

		TStore GetStore(Gtk.TreeIter item)
		{
			if (item.UserData == IntPtr.Zero)
			{
				var h = Handler;
				return h != null ? h.DataStore : null;
			}
			else
			{
				var node = GetNodeAtIter(item);
				return (TStore)(object)node.Item;
			}
		}

		TStore GetParentStore(Node node)
		{
			if (node.Indices.Length <= 1)
			{
				var h = Handler;
				return h != null ? h.DataStore : null;
			}
			return (TStore)(object)node.Item.Parent;
		}

		public bool IterChildren(out Gtk.TreeIter child, Gtk.TreeIter parent)
		{
			TStore store;
			int[] indices;
			if (parent.UserData == IntPtr.Zero)
			{
				var h = Handler;
				store = h != null ? h.DataStore : null;
				indices = new[] { 0 };
			}
			else
			{
				var node = GetNodeAtIter(parent);
				indices = new int[node.Indices.Length + 1];
				Array.Copy(node.Indices, indices, node.Indices.Length);
				store = (TStore)(object)node.Item;
			}

			if (store != null && store.Count > 0)
			{
				child = GetIterFromItem(store[0], indices);
				return true;
			}
			child = Gtk.TreeIter.Zero;
			return false;
		}

		public bool IterHasChild(Gtk.TreeIter iter)
		{
			TStore store;
			if (iter.UserData == IntPtr.Zero)
			{
				store = Handler.DataStore;
			}
			else
			{
				var node = GetNodeAtIter(iter);
				store = (TStore)(object)node.Item;
			}

			if (store != null)
			{
				return store.Count > 0;
			}
			return false;
		}

		public int IterNChildren(Gtk.TreeIter iter)
		{
			var node = GetNodeAtIter(iter);
			var store = GetStore(iter);
			if (store != null && node?.Item.Expandable != false)
				return store.Count;
			return 0;
		}

		public bool IterNthChild(out Gtk.TreeIter child, Gtk.TreeIter parent, int n)
		{
			var store = GetStore(parent);
			if (store != null)
			{
				int[] indices;
				if (parent.UserData != IntPtr.Zero)
				{
					var parentIndices = GetNodeAtIter(parent).Indices;
					indices = new int[parentIndices.Length + 1];
					Array.Copy(parentIndices, indices, parentIndices.Length);
					indices[indices.Length - 1] = n;
				}
				else
					indices = new[] { n };
				if (n < store.Count)
				{
					var item = store[n];
					child = GetIterFromItem(item, indices);
					return true;
				}
			}

			child = Gtk.TreeIter.Zero;
			return false;
		}

		public bool IterParent(out Gtk.TreeIter parent, Gtk.TreeIter child)
		{
			var node = GetNodeAtIter(child);
			if (node != null && node.Indices.Length > 1 && node.Item != null)
			{
				var indices = new int[node.Indices.Length - 1];
				Array.Copy(node.Indices, indices, indices.Length);
				parent = GetIterFromItem(node.Item.Parent, indices);
				return true;
			}
			parent = Gtk.TreeIter.Zero;
			return false;
		}

		public void RefNode(Gtk.TreeIter iter)
		{
		}

		public void UnrefNode(Gtk.TreeIter iter)
		{
		}
	}
}
