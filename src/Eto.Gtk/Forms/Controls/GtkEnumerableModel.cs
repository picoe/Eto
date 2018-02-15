using System;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;

namespace Eto.GtkSharp.Forms.Controls
{
	public interface IGtkEnumerableModelHandler<TItem>
		where TItem: class
	{
		EnumerableChangedHandler<TItem> Collection { get; }

		int NumberOfColumns { get; }

		GLib.Value GetColumnValue(TItem item, int column, int row);

		int GetRowOfItem(TItem item);
	}

	public class GtkEnumerableModel<TItem> : GLib.Object, ITreeModelImplementor
		where TItem: class
	{
		WeakReference weakHandler;

		public IGtkEnumerableModelHandler<TItem> Handler { get { return (IGtkEnumerableModelHandler<TItem>)weakHandler.Target; } set { weakHandler = new WeakReference(value); } }

		public int Count { get; set; }

		public Gtk.TreeIter GetIterAtRow(int row)
		{
			return new Gtk.TreeIter { UserData = (IntPtr)(row + 1) };
		}

		public Gtk.TreePath GetPathAtRow(int row)
		{
			var path = new Gtk.TreePath();
			path.AppendIndex(row);
			return path;
		}

		public TItem GetItemAtPath(Gtk.TreePath path)
		{
			var row = GetRow(path);
			return row >= 0 ? Handler.Collection.ElementAt(row) : default(TItem);
		}

		public TItem GetItemAtPath(string path)
		{
			return GetItemAtPath(new Gtk.TreePath(path));
		}

		public TItem GetItemAtIter(Gtk.TreeIter iter)
		{
			var node = GetRow(iter);
			return node >= 0 ? Handler.Collection.ElementAt(node) : default(TItem);
		}

		int GetRow(Gtk.TreePath path)
		{
			var handler = Handler;
			if (handler != null && path.Indices.Length > 0 && Count > 0)
				return path.Indices[0];
			return -1;
		}

		public Gtk.TreeModelFlags Flags
		{
			get { return Gtk.TreeModelFlags.ListOnly; }
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

		public int GetRow(Gtk.TreeIter iter)
		{
			return ((int)iter.UserData) - 1;
		}

		public bool GetIter(out Gtk.TreeIter iter, Gtk.TreePath path)
		{
			if (path == null)
				throw new ArgumentNullException("path");

				
			var row = GetRow(path);
			if (row >= 0)
			{
				iter = GetIterAtRow(row);
				return true;
			}
			iter = Gtk.TreeIter.Zero;
			return false;
		}

		public Gtk.TreePath GetPath(Gtk.TreeIter iter)
		{
			var row = GetRow(iter);

			var path = new Gtk.TreePath();
			path.AppendIndex(row);
			return path;
		}

		public void GetValue(Gtk.TreeIter iter, int col, ref GLib.Value val)
		{
			var row = GetRow(iter);
			if (row >= 0)
			{
				var item = Handler.Collection.ElementAt(row);
				val = Handler.GetColumnValue(item, col, row);
			}
			else
				val = Handler.GetColumnValue(null, col, row);

		}

		public bool IterNext(ref Gtk.TreeIter iter)
		{
			var row = GetRow(iter);
			if (row >= 0 && Handler.Collection != null && row < Count - 1)
			{
				iter = GetIterAtRow(row + 1);
				return true;
			}
			iter = Gtk.TreeIter.Zero;
			return false;
		}

		public bool IterPrevious(ref Gtk.TreeIter iter)
		{
			var row = GetRow(iter);
			if (row > 0)
			{
				iter = GetIterAtRow(row - 1);
				return true;
			}
			iter = Gtk.TreeIter.Zero;
			return false;
		}

		public bool IterChildren(out Gtk.TreeIter child, Gtk.TreeIter parent)
		{
			if (parent.UserData == IntPtr.Zero && Count > 0)
			{
				child = GetIterAtRow(0);
				return true;
			}
			child = Gtk.TreeIter.Zero;
			return false;
		}

		public bool IterHasChild(Gtk.TreeIter iter)
		{
			return iter.UserData == IntPtr.Zero;;
		}

		public int IterNChildren(Gtk.TreeIter iter)
		{
			var handler = Handler;
			if (iter.UserData == IntPtr.Zero && handler.Collection != null)
				return handler.Collection.Count;
			return 0;
		}

		public bool IterNthChild(out Gtk.TreeIter child, Gtk.TreeIter parent, int n)
		{
			var handler = Handler;
			if (parent.UserData == IntPtr.Zero && handler != null && handler.Collection != null)
			{
				if (n < handler.Collection.Count)
				{
					child = GetIterAtRow(n);
					return true;
				}
			}
			child = Gtk.TreeIter.Zero;
			return false;
		}

		public bool IterParent(out Gtk.TreeIter parent, Gtk.TreeIter child)
		{
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
