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
		readonly Gtk.TreeView tree;
		ContextMenu contextMenu;
		bool cancelExpandCollapseEvents;
		readonly Gtk.CellRendererText textCell;
		public static Size MaxImageSize = new Size(16, 16);

		public override Gtk.Widget EventControl
		{
			get { return tree; }
		}

		class CellRendererTextImage : Gtk.CellRendererText
		{
#if GTK2
			public override void GetSize(Gtk.Widget widget, ref Gdk.Rectangle cell_area, out int x_offset, out int y_offset, out int width, out int height)
			{
				base.GetSize(widget, ref cell_area, out x_offset, out y_offset, out width, out height);
				width += 16;
			}
#else
			protected override void OnRender (Cairo.Context cr, Gtk.Widget widget, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, Gtk.CellRendererState flags)
			{
				base.OnRender (cr, widget, background_area, cell_area, flags);
			}

			protected override void OnGetSize (Gtk.Widget widget, ref Gdk.Rectangle cell_area, out int x_offset, out int y_offset, out int width, out int height)
			{
				base.OnGetSize (widget, ref cell_area, out x_offset, out y_offset, out width, out height);
			}
#endif
		}

		public class CollectionHandler : DataStoreChangedHandler<ITreeItem, ITreeStore>
		{
			public TreeViewHandler Handler { get; set; }

			public void ExpandItems(ITreeStore store, Gtk.TreePath path)
			{
				Handler.cancelExpandCollapseEvents = true;
				PerformExpandItems(store, path);
				Handler.cancelExpandCollapseEvents = false;
			}

			void PerformExpandItems(ITreeStore store, Gtk.TreePath path)
			{
				var newpath = path.Copy();
				newpath.AppendIndex(0);
				for (int i = 0; i < store.Count; i++)
				{
					var item = store[i];
					if (item.Expandable)
					{
						if (item.Expanded)
						{
							Handler.tree.ExpandToPath(newpath);
							PerformExpandItems(item, newpath);
						}
						else
						{
							Handler.tree.CollapseRow(newpath);
						}
					}
					newpath.Next();
				}
			}

			public void ExpandItems()
			{
				var store = Handler.collection.Collection;
				ExpandItems(store, new Gtk.TreePath());
			}

			public override void AddRange(IEnumerable<ITreeItem> items)
			{
				Handler.UpdateModel();
				ExpandItems();
			}

			public override void AddItem(ITreeItem item)
			{
				var path = new Gtk.TreePath();
				path.AppendIndex(Collection.Count);
				var iter = Handler.model.GetIterFromItem(item, path);
				Handler.tree.Model.EmitRowInserted(path, iter);
			}

			public override void InsertItem(int index, ITreeItem item)
			{
				var path = new Gtk.TreePath();
				path.AppendIndex(index);
				var iter = Handler.model.GetIterFromItem(item, path);
				Handler.tree.Model.EmitRowInserted(path, iter);
			}

			public override void RemoveItem(int index)
			{
				var path = new Gtk.TreePath();
				path.AppendIndex(index);
				Handler.tree.Model.EmitRowDeleted(path);
			}

			public override void RemoveAllItems()
			{
				Handler.UpdateModel();
			}
		}

		void UpdateModel()
		{
			model = new GtkTreeModel<ITreeItem, ITreeStore> { Handler = this };
			tree.Model = new Gtk.TreeModelAdapter(model);
		}

		public TreeViewHandler()
		{
			tree = new Gtk.TreeView();
			UpdateModel();
			tree.HeadersVisible = false;
			
			var col = new Gtk.TreeViewColumn();
			var pbcell = new Gtk.CellRendererPixbuf();
			col.PackStart(pbcell, false);
			col.SetAttributes(pbcell, "pixbuf", 1);
			textCell = new Gtk.CellRendererText();
			col.PackStart(textCell, true);
			col.SetAttributes(textCell, "text", 0);
			tree.AppendColumn(col);

			
			tree.ShowExpanders = true;
			
			Control = new Gtk.ScrolledWindow();
			Control.ShadowType = Gtk.ShadowType.In;
			Control.Add(tree);
			
			tree.Events |= Gdk.EventMask.ButtonPressMask;
			tree.ButtonPressEvent += HandleTreeButtonPressEvent;
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TreeView.ExpandingEvent:
					tree.TestExpandRow += delegate(object o, Gtk.TestExpandRowArgs args)
					{
						if (cancelExpandCollapseEvents)
							return;
						var e = new TreeViewItemCancelEventArgs(GetItem(args.Path) as ITreeItem);
						Widget.OnExpanding(e);
						args.RetVal = e.Cancel;
					};
					break;
				case TreeView.ExpandedEvent:
					tree.RowExpanded += delegate(object o, Gtk.RowExpandedArgs args)
					{
						if (cancelExpandCollapseEvents)
							return;
						var item = GetItem(args.Path) as ITreeItem;
						if (item != null && !item.Expanded)
						{
							item.Expanded = true;
							collection.ExpandItems(item, args.Path);
							Widget.OnExpanded(new TreeViewItemEventArgs(item));
						}
					};
					break;
				case TreeView.CollapsingEvent:
					tree.TestCollapseRow += delegate(object o, Gtk.TestCollapseRowArgs args)
					{
						if (cancelExpandCollapseEvents)
							return;
						var e = new TreeViewItemCancelEventArgs(GetItem(args.Path) as ITreeItem);
						Widget.OnCollapsing(e);
						args.RetVal = e.Cancel;
					};
					break;
				case TreeView.CollapsedEvent:
					tree.RowCollapsed += delegate(object o, Gtk.RowCollapsedArgs args)
					{
						if (cancelExpandCollapseEvents)
							return;
						var item = GetItem(args.Path) as ITreeItem;
						if (item != null && item.Expanded)
						{
							item.Expanded = false;
							Widget.OnCollapsed(new TreeViewItemEventArgs(item));
						}
					};
					break;
				case TreeView.ActivatedEvent:
					tree.RowActivated += delegate(object o, Gtk.RowActivatedArgs args)
					{
						Widget.OnActivated(new TreeViewItemEventArgs(model.GetItemAtPath(args.Path)));
					};
					break;
				case TreeView.SelectionChangedEvent:
					tree.Selection.Changed += delegate
					{
						Widget.OnSelectionChanged(EventArgs.Empty);
					};

					break;
				case TreeView.LabelEditingEvent:
					textCell.EditingStarted += (o, args) => {
						var item = model.GetItemAtPath(args.Path);
						if (item != null)
						{
							var itemArgs = new TreeViewItemCancelEventArgs(item);
							Widget.OnLabelEditing(itemArgs);
							args.RetVal = itemArgs.Cancel;
						}
					};
					break;
				case TreeView.LabelEditedEvent:
					textCell.Edited += (o, args) => {
						var item = model.GetItemAtPath(args.Path);
						if (item != null)
						{
							Widget.OnLabelEdited(new TreeViewItemEditEventArgs(item, args.NewText));
							item.Text = args.NewText;
						}
					};
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public object GetItem(Gtk.TreePath path)
		{
			return model.GetItemAtPath(path);
		}

		[GLib.ConnectBefore]
		void HandleTreeButtonPressEvent(object o, Gtk.ButtonPressEventArgs args)
		{
			if (contextMenu != null && args.Event.Button == 3 && args.Event.Type == Gdk.EventType.ButtonPress)
			{
				var menu = ((ContextMenuHandler)contextMenu.Handler).Control;
				menu.Popup();
				menu.ShowAll();
			}
		}

		public ITreeStore DataStore
		{
			get { return collection != null ? collection.Collection : null; }
			set
			{
				if (collection != null)
					collection.Unregister();
				collection = new CollectionHandler { Handler = this };
				collection.Register(value);
			}
		}

		public ContextMenu ContextMenu
		{
			get { return contextMenu; }
			set { contextMenu = value; }
		}

		public ITreeItem SelectedItem
		{
			get
			{
				Gtk.TreeIter iter;
				if (tree.Selection.GetSelected(out iter))
				{
					return model.GetItemAtIter(iter);
				}
				return null;
			}
			set
			{
				if (value != null)
				{
					var path = model.GetPathFromItem(value);
					if (path != null)
					{
						tree.ExpandToPath(path);
						tree.Selection.SelectPath(path);
						tree.ScrollToCell(path, null, false, 0, 0);
					}
				}
				else
					tree.Selection.UnselectAll();
			}
		}

		public GLib.Value GetColumnValue(ITreeItem item, int column, int row)
		{
			switch (column)
			{
				case 0: 
					return new GLib.Value(item.Text);
				case 1:
					if (item.Image != null)
					{
						var image = item.Image.Handler as IGtkPixbuf;
						if (image != null)
							return new GLib.Value(image.GetPixbuf(MaxImageSize));
					}
					return new GLib.Value((Gdk.Pixbuf)null);
			}
			throw new InvalidOperationException();
		}

		public int NumberOfColumns
		{
			get { return 2; }
		}

		public int GetRowOfItem(ITreeItem item)
		{
			return collection != null ? collection.IndexOf(item) : -1;
		}

		public void RefreshData()
		{
			UpdateModel();
			collection.ExpandItems();
		}

		public void RefreshItem(ITreeItem item)
		{
			var path = model.GetPathFromItem(item);
			if (path != null && path.Depth > 0 && !object.ReferenceEquals(item, collection.Collection))
			{
				Gtk.TreeIter iter;
				tree.Model.GetIter(out iter, path);
				tree.Model.EmitRowChanged(path, iter);
				tree.Model.EmitRowHasChildToggled(path, iter);
				cancelExpandCollapseEvents = true;
				if (item.Expanded)
				{
					tree.CollapseRow(path);
					tree.ExpandRow(path, false);
					collection.ExpandItems(item, path);
				}
				else
					tree.CollapseRow(path);
				cancelExpandCollapseEvents = false;
			}
			else
				RefreshData();
		}

		public ITreeItem GetNodeAt(PointF point)
		{
			Gtk.TreePath path;
			if (tree.GetPathAtPos((int)point.X, (int)point.Y, out path))
			{
				return model.GetItemAtPath(path);
			}
			return null;
		}

		public bool LabelEdit
		{
			get { return textCell.Editable; }
			set { textCell.Editable = value; }
		}

		public Color TextColor
		{
			get { return textCell.ForegroundGdk.ToEto(); }
			set { textCell.ForegroundGdk = value.ToGdk(); }
		}
	}
}

