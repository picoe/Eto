using System;
using Eto.Forms;
using System.Linq;
using System.Collections.Generic;

namespace Eto.Platform.GtkSharp.Forms.Controls
{
	public class ComboBoxHandler : GtkControl<Gtk.ComboBox, ComboBox>, IComboBox
	{
		Gtk.ListStore store;
		public ComboBoxHandler ()
		{
			store = new Gtk.ListStore(typeof(string));
			Control = new Gtk.ComboBox(store);
			var text = new Gtk.CellRendererText();
			Control.PackStart(text, false);
			Control.AddAttribute(text, "text", 0);			
			Control.Changed += delegate {
				Widget.OnSelectedIndexChanged(EventArgs.Empty);
			};
		}

		public void AddRange (IEnumerable<object> collection)
		{
			store.AppendValues(collection.ToArray());
		}
		
		public void AddItem (object item)
		{
			store.AppendValues(Convert.ToString(item));
		}

		public void RemoveItem (object item)
		{
			//store.AppendValues(Convert.ToString(item));
		}

		public void RemoveAll ()
		{
			store.Clear();
		}

		public int SelectedIndex {
			get {
				return Control.Active;
			}
			set {
				Control.Active = value;
			}
		}
	}
}

