using System;
using Eto.Forms;
using System.Linq;
using System.Collections.Generic;
using Eto.Platform.GtkSharp.Drawing;

namespace Eto.Platform.GtkSharp
{
	public class ListBoxHandler : GtkControl<Gtk.ScrolledWindow, ListBox>, IListBox
	{
		Gtk.TreeView tree;
		Gtk.TreeStore store;
		ContextMenu contextMenu;
		
		public ListBoxHandler()
		{
			store = new Gtk.TreeStore(typeof(IListItem), typeof(string), typeof(Gdk.Pixbuf));

			Control = new Gtk.ScrolledWindow();
			Control.ShadowType = Gtk.ShadowType.In;
			tree = new Gtk.TreeView(store);
			tree.ShowExpanders = false;
			Control.Add(tree);
			
			tree.Events |= Gdk.EventMask.ButtonPressMask;
			tree.ButtonPressEvent += HandleTreeButtonPressEvent;

			tree.AppendColumn("Img", new Gtk.CellRendererPixbuf(), "pixbuf", 2);
			tree.AppendColumn("Data", new Gtk.CellRendererText(), "text", 1);
			tree.HeadersVisible = false;
			tree.Selection.Changed += selection_Changed;
			tree.RowActivated += tree_RowActivated;
		}
		
		[GLib.ConnectBefore]
		void HandleTreeButtonPressEvent (object o, Gtk.ButtonPressEventArgs args)
		{
			Console.WriteLine ("Button Pressed");
			if (contextMenu != null && args.Event.Button == 3 && args.Event.Type == Gdk.EventType.ButtonPress) {
			Console.WriteLine ("Showing menu?!");
				var menu = contextMenu.ControlObject as Gtk.Menu;
				menu.Popup ();
				menu.ShowAll ();
			}
		}

		public override void Focus()
		{
			tree.GrabFocus();
		}

		public void AddRange (IEnumerable<IListItem> collection)
		{
			foreach (var o in collection)
				AddItem(o);
		}
		
		public void AddItem(IListItem item)
		{
			var imgitem = item as IImageListItem;
			if (imgitem != null) {
				var imgsrc = imgitem.Image.Handler as IGtkPixbuf;
				if (imgsrc != null) {
					store.AppendValues(item, item.Text, imgsrc.Pixbuf);
					return;
				}
			}

			store.AppendValues(item, item.Text, null);
		}

		public void RemoveItem(IListItem item)
		{
			Gtk.TreePath path = new Gtk.TreePath();
			path.AppendIndex(((ListBox)Widget).Items.IndexOf(item));
			Gtk.TreeIter iter;
			store.GetIter(out iter, path);
			store.Remove(ref iter);
		}

		public int SelectedIndex
		{
			get
			{
				Gtk.TreeIter iter;
				
				if (tree.Selection != null && tree.Selection.GetSelected(out iter))
				{
					IListItem val = (IListItem)store.GetValue(iter, 0);
					if (val != null)
					{
						return ((ListBox)Widget).Items.IndexOf(val);
					}
				}
				
				return -1;
			}
			set
			{
				if (value == -1) {
					if (tree.Selection != null) tree.Selection.UnselectAll ();
					return;
				}
				Gtk.TreePath path = new Gtk.TreePath();
				path.AppendIndex(value);
				Gtk.TreeViewColumn focus_column = tree.Columns[0];
				
				tree.SetCursor(path, focus_column, false);
			}
		}

		public ContextMenu ContextMenu {
			get {
				return contextMenu;
			}
			set {
				contextMenu = value;
			}
		}
		
		public void RemoveAll()
		{
			store.Clear();
		}

		private void selection_Changed(object sender, EventArgs e)
		{
			Widget.OnSelectedIndexChanged(EventArgs.Empty);
		}

		private void tree_RowActivated(object o, Gtk.RowActivatedArgs args)
		{
			Widget.OnActivated(EventArgs.Empty);
		}
	}
}
