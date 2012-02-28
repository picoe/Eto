using System;
using Eto.Forms;
using System.Linq;
using System.Collections.Generic;

namespace Eto.Platform.GtkSharp.Forms.Controls
{
	public class ComboBoxHandler : GtkControl<Gtk.ComboBox, ComboBox>, IComboBox
	{
		CollectionHandler collection;
		Gtk.ListStore listStore;

		public ComboBoxHandler ()
		{
			listStore = new Gtk.ListStore (typeof(string));
			Control = new Gtk.ComboBox (listStore);
			var text = new Gtk.CellRendererText ();
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
		

		public class CollectionHandler : CollectionChangedHandler<IListItem, IListStore>
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
			get { return collection != null ? collection.DataStore : null; }
			set {
				if (collection != null)
					collection.Unregister ();
				collection = new CollectionHandler{ Handler = this };
				collection.Register (value);
			}
		}

	}
}

