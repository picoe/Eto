using System;
using Eto.Forms;
using Eto.GtkSharp.Drawing;
using Eto.Drawing;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace Eto.GtkSharp.Forms.Controls
{
	public class ListBoxHandler : GtkControl<Gtk.TreeView, ListBox, ListBox.ICallback>, ListBox.IHandler, IGtkEnumerableModelHandler<object>
	{
		IIndirectBinding<string> _itemTextBinding;
		readonly Gtk.ScrolledWindow scroll;
		GtkEnumerableModel<object> model;
		ContextMenu contextMenu;
		CollectionHandler collection;
		public static Size MaxImageSize = new Size(16, 16);

		public override Gtk.Widget ContainerControl
		{
			get { return scroll; }
		}

		public ListBoxHandler()
		{
			model = new GtkEnumerableModel<object>{ Handler = this };
			
			scroll = new Gtk.ScrolledWindow();
			scroll.ShadowType = Gtk.ShadowType.In;
			Control = new Gtk.TreeView(new Gtk.TreeModelAdapter(model));
			Control.FixedHeightMode = false;
			Control.ShowExpanders = false;
			scroll.Add(Control);

			Control.Events |= Gdk.EventMask.ButtonPressMask;

			Control.AppendColumn("Img", new Gtk.CellRendererPixbuf(), "pixbuf", 1);
			Control.AppendColumn("Data", new Gtk.CellRendererText(), "text", 0);
			Control.HeadersVisible = false;
		}

		protected override void Initialize()
		{
			base.Initialize();
			Size = new Size(80, 80);
			Control.ButtonPressEvent += Connector.HandleTreeButtonPressEvent;
			Control.Selection.Changed += Connector.HandleSelectionChanged;
			Control.RowActivated += Connector.HandleTreeRowActivated;
		}

		protected new ListBoxEventConnector Connector { get { return (ListBoxEventConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new ListBoxEventConnector();
		}

		protected class ListBoxEventConnector : GtkControlConnector
		{
			public new ListBoxHandler Handler { get { return (ListBoxHandler)base.Handler; } }

			[GLib.ConnectBefore]
			public void HandleTreeButtonPressEvent(object o, Gtk.ButtonPressEventArgs args)
			{
				if (Handler.contextMenu != null && args.Event.Button == 3 && args.Event.Type == Gdk.EventType.ButtonPress)
				{
					var menu = (Gtk.Menu)Handler.contextMenu.ControlObject;
					menu.Popup();
					menu.ShowAll();
				}
			}

			public void HandleSelectionChanged(object sender, EventArgs e)
			{
				Handler.Callback.OnSelectedIndexChanged(Handler.Widget, EventArgs.Empty);
			}

			public void HandleTreeRowActivated(object o, Gtk.RowActivatedArgs args)
			{
				Handler.Callback.OnActivated(Handler.Widget, EventArgs.Empty);
			}
		}

		public int SelectedIndex
		{
			get
			{
				Gtk.TreeIter iter;

				if (Control.Selection != null && Control.Selection.GetSelected(out iter))
				{
					var val = model.GetRow(iter);
					if (val >= 0)
						return val;
				}
				
				return -1;
			}
			set
			{
				if (value == -1)
				{
					if (Control.Selection != null)
						Control.Selection.UnselectAll();
					return;
				}
				var path = new Gtk.TreePath();
				path.AppendIndex(value);
				Gtk.TreeViewColumn focus_column = Control.Columns[0];

				Control.SetCursor(path, focus_column, false);
			}
		}

		public ContextMenu ContextMenu
		{
			get { return contextMenu; }
			set { contextMenu = value; }
		}

		public GLib.Value GetColumnValue(object item, int column, int row)
		{
			switch (column)
			{
				case 0:
					return new GLib.Value(Widget.ItemTextBinding?.GetValue(item) ?? string.Empty);
				case 1:
					return new GLib.Value(Widget.ItemImageBinding?.GetValue(item).ToGdk());
				default:
					throw new InvalidOperationException();
			}
		}

		public class CollectionHandler : EnumerableChangedHandler<object>
		{
			public ListBoxHandler Handler { get; set; }

			protected override void OnRegisterCollection(EventArgs e)
			{
				Handler.model = new GtkEnumerableModel<object>{ Handler = Handler, Count = Count };
				Handler.Control.Model = new Gtk.TreeModelAdapter(Handler.model);
			}

			protected override void OnUnregisterCollection(EventArgs e)
			{
				Handler.Control.Model = null;
			}

			public override void AddItem(object item)
			{
				var count = Count;
				var iter = Handler.model.GetIterAtRow(count);
				var path = Handler.model.GetPathAtRow(count);
				Handler.model.Count++;
				Handler.Control.Model.EmitRowInserted(path, iter);
			}

			public override void InsertItem(int index, object item)
			{
				var iter = Handler.model.GetIterAtRow(index);
				var path = Handler.model.GetPathAtRow(index);
				Handler.model.Count++;
				Handler.Control.Model.EmitRowInserted(path, iter);
			}

			public override void RemoveItem(int index)
			{
				var path = Handler.model.GetPathAtRow(index);
				Handler.model.Count--;
				Handler.Control.Model.EmitRowDeleted(path);
			}

			public override void RemoveAllItems()
			{
				Handler.model = new GtkEnumerableModel<object>{ Handler = Handler };
				Handler.Control.Model = new Gtk.TreeModelAdapter(Handler.model);
			}
		}

		public IEnumerable<object> DataStore
		{
			get { return collection != null ? collection.Collection : null; }
			set
			{
				if (collection != null)
					collection.Unregister();
				collection = new CollectionHandler{ Handler = this };
				collection.Register(value);
			}
		}

		public int NumberOfColumns
		{
			get { return 2; }
		}

		public int GetRowOfItem(object item)
		{
			return collection == null ? -1 : collection.IndexOf(item);
		}

		EnumerableChangedHandler<object> IGtkEnumerableModelHandler<object>.Collection
		{
			get { return collection; }
		}

		public Gtk.CellRendererText TextCell
		{
			get { return ((Gtk.CellRendererText)Control.Columns[1].Cells[0]); }
		}

		public Color TextColor
		{
			get { return TextCell.ForegroundGdk.ToEto(); }
			set
			{
				TextCell.ForegroundGdk = value.ToGdk();
				Control.QueueDraw();
			}
		}

		public override Color BackgroundColor
		{
			get { return Control.GetBase(); }
			set { Control.SetBase(value); }
		}

		public IIndirectBinding<string> ItemTextBinding
		{
			get => _itemTextBinding;
			set
			{
				_itemTextBinding = value;
				if (Widget.Loaded)
					Control.QueueDraw();
			}
		}
		public IIndirectBinding<string> ItemKeyBinding { get; set; }
	}
}
