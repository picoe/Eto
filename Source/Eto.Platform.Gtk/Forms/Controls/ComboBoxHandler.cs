using System;
using Eto.Forms;
using System.Linq;
using System.Collections.Generic;
using Eto.Drawing;
using Eto.Platform.GtkSharp.Drawing;

namespace Eto.Platform.GtkSharp.Forms.Controls
{
	public class ComboBoxHandler : GtkControl<Gtk.ComboBox, ComboBox>, IComboBox
	{
		Font font;
		CollectionHandler collection;
		Gtk.ListStore listStore;
		Gtk.CellRendererText text;

		public ComboBoxHandler ()
		{
			listStore = new Gtk.ListStore (typeof(string));
			Control = new Gtk.ComboBox (listStore);
			text = new Gtk.CellRendererText ();
			Control.PackStart (text, false);
			Control.AddAttribute (text, "text", 0);
			Control.Changed += delegate {
				Widget.OnSelectedIndexChanged (EventArgs.Empty);
			};
		}
		
		public int SelectedIndex {
			get {
				return Control.Active;
			}
			set {
				Control.Active = value;
			}
		}

		public override Font Font
		{
			get
			{
				if (font == null)
					font = new Font (Widget.Generator, new FontHandler (text.FontDesc));
				return font;
			}
			set
			{
				font = value;
				if (font != null)
					text.FontDesc = ((FontHandler)font.Handler).Control;
				else
					text.FontDesc = null;
			}
		}

		public class CollectionHandler : DataStoreChangedHandler<IListItem, IListStore>
		{
			public ComboBoxHandler Handler { get; set; }

			public override void AddItem (IListItem item)
			{
				Handler.listStore.AppendValues (item.Text);
			}

			public override void InsertItem (int index, IListItem item)
			{
				Handler.listStore.InsertWithValues (index, item.Text);
			}

			public override void RemoveItem (int index)
			{
				Gtk.TreeIter iter;
				if (Handler.listStore.IterNthChild (out iter, index))
					Handler.listStore.Remove (ref iter);
			}

			public override void RemoveAllItems ()
			{
				Handler.listStore.Clear ();
			}
		}

		public IListStore DataStore {
			get { return collection != null ? collection.Collection : null; }
			set {
				if (collection != null)
					collection.Unregister ();
				collection = new CollectionHandler{ Handler = this };
				collection.Register (value);
			}
		}

	}
}

