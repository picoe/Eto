using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.Platform.GtkSharp.Drawing;
using System.Collections.Generic;

namespace Eto.Platform.GtkSharp.Forms
{
	public class TreeViewHandler : GtkControl<Gtk.ScrolledWindow, TreeView>, ITreeView, IGtkListModelHandler<ITreeItem, ITreeStore>
	{
		GtkTreeModel<ITreeItem, ITreeStore> model;
		CollectionHandler collection;
		Gtk.TreeView tree;
		ContextMenu contextMenu;
		public static Size MaxImageSize = new Size (16, 16);
		
		class CellRendererTextImage : Gtk.CellRendererText
		{
			
			protected override void Render (Gdk.Drawable window, Gtk.Widget widget, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, Gdk.Rectangle expose_area, Gtk.CellRendererState flags)
			{
				base.Render (window, widget, background_area, cell_area, expose_area, flags);
			}
			
			public override void GetSize (Gtk.Widget widget, ref Gdk.Rectangle cell_area, out int x_offset, out int y_offset, out int width, out int height)
			{
				base.GetSize (widget, ref cell_area, out x_offset, out y_offset, out width, out height);
				width += 16;
			}
		}
		
		public class CollectionHandler : DataStoreChangedHandler<ITreeItem, ITreeStore>
		{
			public TreeViewHandler Handler { get; set; }
			
			void ExpandItems (ITreeStore store, Gtk.TreePath path)
			{
				for (int i = 0; i < store.Count; i++) {
					var item = store [i];
					if (item.Expandable && item.Expanded) {
						var newpath = path.Copy ();
						newpath.AppendIndex (i);
						Handler.tree.ExpandToPath (newpath);
						ExpandItems ((ITreeStore)item, newpath);
					}
				}
			}
			
			void ExpandItems ()
			{
				var store = Handler.collection.DataStore;
				Gtk.TreePath path = new Gtk.TreePath ();
				ExpandItems (store, path);
			}

			public override void AddRange (IEnumerable<ITreeItem> items)
			{
				Handler.UpdateModel ();
				ExpandItems ();
			}

			public override void AddItem (ITreeItem item)
			{
				var path = new Gtk.TreePath ();
				path.AppendIndex (DataStore.Count);
				var iter = Handler.model.GetIterFromItem (item, path);
				Handler.tree.Model.EmitRowInserted (path, iter);
			}

			public override void InsertItem (int index, ITreeItem item)
			{
				var path = new Gtk.TreePath ();
				path.AppendIndex (index);
				var iter = Handler.model.GetIterFromItem (item, path);
				Handler.tree.Model.EmitRowInserted (path, iter);
			}

			public override void RemoveItem (int index)
			{
				var path = new Gtk.TreePath ();
				path.AppendIndex (index);
				Handler.tree.Model.EmitRowDeleted (path);
			}

			public override void RemoveAllItems ()
			{
				Handler.UpdateModel ();
			}
		}
		
		void UpdateModel ()
		{
			model = new GtkTreeModel<ITreeItem, ITreeStore> { Handler = this };
			tree.Model = new Gtk.TreeModelAdapter (model);
		}

		public TreeViewHandler ()
		{
			tree = new Gtk.TreeView ();
			UpdateModel ();
			tree.HeadersVisible = false;
			
			tree.Selection.Changed += delegate {
				this.Widget.OnSelectionChanged (EventArgs.Empty);
			};
			tree.RowActivated += delegate(object o, Gtk.RowActivatedArgs args) {
				this.Widget.OnActivated (new TreeViewItemEventArgs (model.GetItemAtPath (args.Path)));
			};
			
			var col = new Gtk.TreeViewColumn ();
			var pbcell = new Gtk.CellRendererPixbuf ();
			col.PackStart (pbcell, false);
			col.SetAttributes (pbcell, "pixbuf", 1);
			var textcell = new Gtk.CellRendererText ();
			col.PackStart (textcell, true);
			col.SetAttributes (textcell, "text", 0);
			tree.AppendColumn (col);
			
			tree.ShowExpanders = true;
			
			Control = new Gtk.ScrolledWindow ();
			Control.ShadowType = Gtk.ShadowType.In;
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
		
		public ITreeStore DataStore {
			get { return collection != null ? collection.DataStore : null; }
			set {
				if (collection != null)
					collection.Unregister ();
				collection = new CollectionHandler { Handler = this };
				collection.Register (value);
			}
		}

		public ContextMenu ContextMenu {
			get { return contextMenu; }
			set { contextMenu = value; }
		}
		
		public ITreeItem SelectedItem {
			get {
				Gtk.TreeIter iter;
				if (tree.Selection.GetSelected (out iter)) {
					return model.GetItemAtIter (iter);
				}
				return null;
			}
			set {
				//model.GetIterFromItem(value)
				//Control.Selection.SelectPath (iter);
			}
		}

		#region IGtkListModelHandler[ITreeItem,ITreeStore] implementation
		
		public GLib.Value GetColumnValue (ITreeItem item, int column)
		{
			switch (column) {
			case 0: 
				return new GLib.Value(item.Text);
			case 1:
				if (item.Image != null) {
					var image = item.Image.Handler as IGtkPixbuf;
					if (image != null)
						return new GLib.Value(image.GetPixbuf (MaxImageSize));
				}
				return new GLib.Value((Gdk.Pixbuf)null);
			}
			throw new InvalidOperationException();
		}

		public int NumberOfColumns {
			get { return 2; }
		}
		
		#endregion
	}
}

