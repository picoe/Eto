using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.Platform.GtkSharp.Drawing;

namespace Eto.Platform.GtkSharp.Forms
{
	public class TreeViewHandler : GtkControl<Gtk.ScrolledWindow, TreeView>, ITreeView
	{
		Gtk.TreeStore model;
		Gtk.TreeView tree;
		ITreeItem top;
		
		public static Size MaxImageSize = new Size(16, 16);
		
		class CellRendererTextImage : Gtk.CellRendererText {
			
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
		
		public TreeViewHandler ()
		{
			model = new Gtk.TreeStore (typeof(ITreeItem), typeof(string), typeof(Gdk.Pixbuf));
			tree = new Gtk.TreeView (model);
			tree.HeadersVisible = false;
			
			tree.Selection.Changed += delegate {
				this.Widget.OnSelectionChanged (EventArgs.Empty);
			};
			tree.RowActivated += delegate(object o, Gtk.RowActivatedArgs args) {
				this.Widget.OnActivated (new TreeViewItemEventArgs(GetTreeItem(args.Path)));
			};
			
			var col = new Gtk.TreeViewColumn();
			var pbcell = new Gtk.CellRendererPixbuf ();
			col.PackStart (pbcell, false);
			col.SetAttributes (pbcell, "pixbuf", 2);
			var textcell = new Gtk.CellRendererText ();
			col.PackStart (textcell, true);
			col.SetAttributes (textcell, "text", 1);
			tree.AppendColumn (col);
			
			tree.ShowExpanders = true;
			
			Control = new Gtk.ScrolledWindow();
			Control.ShadowType = Gtk.ShadowType.In;
			Control.Add(tree);
		}

		public ITreeItem TopNode {
			get { return top; }
			set {
				top = value;
				Populate ();
			}
		}
		
		void Populate ()
		{
			model.Clear ();
			if (top == null) return;
			Populate (null, top);
			
		}
		
		void Populate (Gtk.TreeIter? parent, ITreeItem item)
		{
			for (int i=0; i<item.Count; i++) {
				var child = item.GetChild (i);
				var img = child.Image;
				Gdk.Pixbuf pixbuf = null;
				if (img != null) {
					var imghandler =  img.Handler as IGtkPixbuf;
					pixbuf = imghandler.GetPixbuf (MaxImageSize);
				}
				
				Gtk.TreeIter? parentIter = null;
				if (parent != null)
					parentIter = model.AppendValues (parent.Value, child, child.Text, pixbuf);
				else
					parentIter = model.AppendValues (child, child.Text, pixbuf);
				if (child.Expandable) {
					Populate (parentIter, child);
					if (child.Expanded) {
						if (parentIter != null)
							tree.ExpandRow (model.GetPath (parentIter.Value), false);
					}
				}
			}
		}
		
		ITreeItem GetTreeItem (Gtk.TreeIter iter)
		{
			return model.GetValue (iter, 0) as ITreeItem;
		}

		ITreeItem GetTreeItem (Gtk.TreePath path)
		{
			Gtk.TreeIter iter;
			if (model.GetIter (out iter, path))
				return model.GetValue (iter, 0) as ITreeItem;
			else
				return null;
		}
		
		public ITreeItem SelectedItem {
			get {
				Gtk.TreeIter iter;
				if (tree.Selection.GetSelected (out iter)) {
					return GetTreeItem (iter);
				}
				return null;
			}
			set {
				//Control.Selection.SelectPath (iter);
			}
		}
	}
}

