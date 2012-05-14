using System;
using Eto.Forms;
using System.Linq;
using System.Collections.Generic;
using Eto.Platform.GtkSharp.Drawing;
using Eto.Drawing;

namespace Eto.Platform.GtkSharp
{
	public class ListBoxHandler : GtkControl<Gtk.TreeView, ListBox>, IListBox, IGtkListModelHandler<IListItem, IListStore>
	{
		Gtk.ScrolledWindow scroll;
		GtkListModel<IListItem, IListStore> model;
		ContextMenu contextMenu;
		CollectionHandler collection;
		public static Size MaxImageSize = new Size (16, 16);

		public override Gtk.Widget ContainerControl
		{
			get { return scroll; }
		}
		
		public ListBoxHandler ()
		{
			model = new GtkListModel<IListItem, IListStore>{ Handler = this };
			
			scroll = new Gtk.ScrolledWindow ();
			scroll.ShadowType = Gtk.ShadowType.In;
			Control = new Gtk.TreeView (new Gtk.TreeModelAdapter (model));
			//tree.FixedHeightMode = true;
			Control.ShowExpanders = false;
			scroll.Add (Control);

			Control.Events |= Gdk.EventMask.ButtonPressMask;
			Control.ButtonPressEvent += HandleTreeButtonPressEvent;

			Control.AppendColumn ("Img", new Gtk.CellRendererPixbuf (), "pixbuf", 1);
			Control.AppendColumn ("Data", new Gtk.CellRendererText (), "text", 0);
			Control.HeadersVisible = false;
			Control.Selection.Changed += selection_Changed;
			Control.RowActivated += tree_RowActivated;
		}
		
		[GLib.ConnectBefore]
		void HandleTreeButtonPressEvent (object o, Gtk.ButtonPressEventArgs args)
		{
			if (contextMenu != null && args.Event.Button == 3 && args.Event.Type == Gdk.EventType.ButtonPress) {
				var menu = contextMenu.ControlObject as Gtk.Menu;
				menu.Popup ();
				menu.ShowAll ();
			}
		}

		public override void Focus ()
		{
			Control.GrabFocus ();
		}

		public int SelectedIndex {
			get {
				Gtk.TreeIter iter;

				if (Control.Selection != null && Control.Selection.GetSelected (out iter)) {
					var val = model.NodeFromIter (iter);
					if (val >= 0)
						return val;
				}
				
				return -1;
			}
			set {
				if (value == -1) {
					if (Control.Selection != null)
						Control.Selection.UnselectAll ();
					return;
				}
				Gtk.TreePath path = new Gtk.TreePath ();
				path.AppendIndex (value);
				Gtk.TreeViewColumn focus_column = Control.Columns[0];

				Control.SetCursor (path, focus_column, false);
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

		void selection_Changed (object sender, EventArgs e)
		{
			Widget.OnSelectedIndexChanged (EventArgs.Empty);
		}

		void tree_RowActivated (object o, Gtk.RowActivatedArgs args)
		{
			Widget.OnActivated (EventArgs.Empty);
		}

		public GLib.Value GetColumnValue (IListItem item, int column)
		{
			switch (column) {
			case 0:
				if (item != null) 
					return new GLib.Value (item.Text);
				else 
					return new GLib.Value ((string)null);
			case 1:
				var imageItem = item as IImageListItem;
				if (imageItem != null) {
					var img = imageItem.Image;
					if (img != null) {
						var imgHandler = img.Handler as IGtkPixbuf;
						if (imgHandler != null)
							return new GLib.Value (imgHandler.GetPixbuf (MaxImageSize));
					}
				}
				return new GLib.Value ((Gdk.Pixbuf)null);
			default:
				throw new InvalidOperationException ();
			}
		}
		
		public class CollectionHandler : DataStoreChangedHandler<IListItem, IListStore>
		{
			public ListBoxHandler Handler { get; set; }
			
			public override int IndexOf (IListItem item)
			{
				return -1;
			}
			
			protected override void OnRegisterCollection (EventArgs e)
			{
				Handler.model = new GtkListModel<IListItem, IListStore>{ Handler = this.Handler };
				Handler.Control.Model = new Gtk.TreeModelAdapter (Handler.model);
			}

			protected override void OnUnregisterCollection (EventArgs e)
			{
				Handler.Control.Model = null;
			}

			public override void AddItem (IListItem item)
			{
				var iter = Handler.model.GetIterAtRow (DataStore.Count);
				var path = Handler.model.GetPathAtRow (DataStore.Count);
				Handler.Control.Model.EmitRowInserted (path, iter);
			}

			public override void InsertItem (int index, IListItem item)
			{
				var iter = Handler.model.GetIterAtRow (index);
				var path = Handler.model.GetPathAtRow (index);
				Handler.Control.Model.EmitRowInserted (path, iter);
			}

			public override void RemoveItem (int index)
			{
				var path = Handler.model.GetPathAtRow (index);
				Handler.Control.Model.EmitRowDeleted (path);
			}
			
			public override void RemoveAllItems ()
			{
				Handler.model = new GtkListModel<IListItem, IListStore>{ Handler = Handler };
				Handler.Control.Model = new Gtk.TreeModelAdapter (Handler.model);
			}
		}
		
		public IListStore DataStore {
			get { return collection != null ? collection.DataStore : null; }
			set {
				if (collection != null)
					collection.Unregister ();
				collection = new CollectionHandler{ Handler = this };
				collection.Register (value);
			}
		}

		public int NumberOfColumns {
			get { return 2; }
		}
	}
}
