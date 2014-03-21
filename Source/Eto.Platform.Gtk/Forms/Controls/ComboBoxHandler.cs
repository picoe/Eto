using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.Platform.GtkSharp.Drawing;

namespace Eto.Platform.GtkSharp.Forms.Controls
{
	public class ComboBoxHandler : GtkControl<Gtk.ComboBox, ComboBox>, IComboBox
	{
		Font font;
		CollectionHandler collection;
		readonly Gtk.ListStore listStore;
		readonly Gtk.CellRendererText text;

		public ComboBoxHandler()
		{
			listStore = new Gtk.ListStore(typeof(string));
			Control = new Gtk.ComboBox(listStore);
			text = new Gtk.CellRendererText();
			Control.PackStart(text, false);
			Control.AddAttribute(text, "text", 0);
		}

		protected override void Initialize()
		{
			base.Initialize();
			Control.Changed += Connector.HandleChanged;
		}

		protected new ComboBoxConnector Connector { get { return (ComboBoxConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new ComboBoxConnector();
		}

		protected class ComboBoxConnector : GtkControlConnector
		{
			public new ComboBoxHandler Handler { get { return (ComboBoxHandler)base.Handler; } }

			public void HandleChanged(object sender, EventArgs e)
			{
				Handler.Widget.OnSelectedIndexChanged(EventArgs.Empty);
			}
		}

		public int SelectedIndex
		{
			get { return Control.Active; }
			set { Control.Active = value; }
		}

		public override Font Font
		{
			get
			{
				if (font == null)
					font = new Font(Widget.Generator, new FontHandler(text.FontDesc));
				return font;
			}
			set
			{
				font = value;
				text.FontDesc = font != null ? ((FontHandler)font.Handler).Control : null;
			}
		}

		public class CollectionHandler : DataStoreChangedHandler<IListItem, IListStore>
		{
			public ComboBoxHandler Handler { get; set; }

			public override void AddItem(IListItem item)
			{
				Handler.listStore.AppendValues(item.Text);
			}

			public override void InsertItem(int index, IListItem item)
			{
				Handler.listStore.InsertWithValues(index, item.Text);
			}

			public override void RemoveItem(int index)
			{
				Gtk.TreeIter iter;
				if (Handler.listStore.IterNthChild(out iter, index))
					Handler.listStore.Remove(ref iter);
			}

			public override void RemoveAllItems()
			{
				Handler.listStore.Clear();
			}
		}

		public IListStore DataStore
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
	}
}

