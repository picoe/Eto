using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.GtkSharp.Drawing;
using System.Collections.Generic;
using Eto.GtkSharp.Forms.Menu;

namespace Eto.GtkSharp.Forms.Controls
{
	[Obsolete("Since 2.4. TreeView is deprecated, please use TreeGridView instead.")]
	public class TreeViewHandler : GtkControl<Gtk.ScrolledWindow, TreeView, TreeView.ICallback>, TreeView.IHandler, IGtkTreeModelHandler<ITreeItem, ITreeStore>
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

		public class CollectionHandler : DataStoreChangedHandler<ITreeItem, ITreeStore>
		{
			WeakReference handler;
			public TreeViewHandler Handler { get { return (TreeViewHandler)handler.Target; } set { handler = new WeakReference(value); } }

			public void ExpandItems(ITreeStore store, Gtk.TreePath path)
			{
				Handler.cancelExpandCollapseEvents = true;
				PerformExpandItems(store, path);
				Handler.cancelExpandCollapseEvents = false;
			}

			void PerformExpandItems(ITreeStore store, Gtk.TreePath path)
			{
				if (store == null)
					return;
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
		}

		protected override void Initialize()
		{
			base.Initialize();
			tree.ButtonPressEvent += Connector.HandleTreeButtonPressEvent;
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TreeView.ExpandingEvent:
					tree.TestExpandRow += Connector.HandleTestExpandRow;
					break;
				case TreeView.ExpandedEvent:
					tree.RowExpanded += Connector.HandleRowExpanded;
					break;
				case TreeView.CollapsingEvent:
					tree.TestCollapseRow += Connector.HandleTestCollapseRow;
					break;
				case TreeView.CollapsedEvent:
					tree.RowCollapsed += Connector.HandleRowCollapsed;
					break;
				case TreeView.ActivatedEvent:
					tree.RowActivated += Connector.HandleRowActivated;
					break;
				case TreeView.SelectionChangedEvent:
					tree.Selection.Changed += Connector.HandleSelectionChanged;
					break;
				case TreeView.LabelEditingEvent:
					textCell.EditingStarted += Connector.HandleEditingStarted;
					break;
				case TreeView.LabelEditedEvent:
					textCell.Edited += Connector.HandleEdited;
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

		protected new TreeViewConnector Connector { get { return (TreeViewConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new TreeViewConnector();
		}

		protected class TreeViewConnector : GtkControlConnector
		{
			public new TreeViewHandler Handler { get { return (TreeViewHandler)base.Handler; } }

			public void HandleTestExpandRow(object o, Gtk.TestExpandRowArgs args)
			{
				var handler = Handler;
				if (handler.cancelExpandCollapseEvents)
					return;
				var e = new TreeViewItemCancelEventArgs(handler.GetItem(args.Path) as ITreeItem);
				handler.Callback.OnExpanding(handler.Widget, e);
				args.RetVal = e.Cancel;
			}

			public void HandleTestCollapseRow(object o, Gtk.TestCollapseRowArgs args)
			{
				var handler = Handler;
				if (handler.cancelExpandCollapseEvents)
					return;
				var e = new TreeViewItemCancelEventArgs(handler.GetItem(args.Path) as ITreeItem);
				handler.Callback.OnCollapsing(handler.Widget, e);
				args.RetVal = e.Cancel;
			}

			public void HandleRowExpanded(object o, Gtk.RowExpandedArgs args)
			{
				var handler = Handler;
				if (handler.cancelExpandCollapseEvents)
					return;
				var item = handler.GetItem(args.Path) as ITreeItem;
				if (item != null && !item.Expanded)
				{
					item.Expanded = true;
					handler.collection.ExpandItems(item, args.Path);
					handler.Callback.OnExpanded(handler.Widget, new TreeViewItemEventArgs(item));
				}
			}

			public void HandleRowCollapsed(object o, Gtk.RowCollapsedArgs args)
			{
				var handler = Handler;
				if (handler.cancelExpandCollapseEvents)
					return;
				var item = handler.GetItem(args.Path) as ITreeItem;
				if (item != null && item.Expanded)
				{
					item.Expanded = false;
					handler.Callback.OnCollapsed(handler.Widget, new TreeViewItemEventArgs(item));
				}
			}

			public void HandleRowActivated(object o, Gtk.RowActivatedArgs args)
			{
				Handler.Callback.OnActivated(Handler.Widget, new TreeViewItemEventArgs(Handler.model.GetItemAtPath(args.Path)));
			}

			public void HandleSelectionChanged(object sender, EventArgs e)
			{
				Handler.Callback.OnSelectionChanged(Handler.Widget, EventArgs.Empty);
			}

			public void HandleEditingStarted(object o, Gtk.EditingStartedArgs args)
			{
				var handler = Handler;
				var item = handler.model.GetItemAtPath(args.Path);
				if (item != null)
				{
					var itemArgs = new TreeViewItemCancelEventArgs(item);
					handler.Callback.OnLabelEditing(handler.Widget, itemArgs);
					args.RetVal = itemArgs.Cancel;
				}
			}

			public void HandleEdited(object o, Gtk.EditedArgs args)
			{
				var handler = Handler;
				var item = handler.model.GetItemAtPath(args.Path);
				if (item != null)
				{
					handler.Callback.OnLabelEdited(handler.Widget, new TreeViewItemEditEventArgs(item, args.NewText));
					item.Text = args.NewText;
				}
			}

			[GLib.ConnectBefore]
			public void HandleTreeButtonPressEvent(object o, Gtk.ButtonPressEventArgs args)
			{
				var handler = Handler;
				if (handler.contextMenu != null && args.Event.Button == 3 && args.Event.Type == Gdk.EventType.ButtonPress)
				{
					var menu = ((ContextMenuHandler)handler.contextMenu.Handler).Control;
					menu.Popup();
					menu.ShowAll();
				}
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

		public GLib.Value GetColumnValue(ITreeItem item, int column, int row, Gtk.TreeIter iter)
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

		public ITreeItem GetItem(int row) => DataStore?[row];

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

		public int Count => DataStore?.Count ?? 0;
	}
}

