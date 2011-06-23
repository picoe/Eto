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
			store = new Gtk.ListStore(typeof(string), typeof(IListItem));
			Control = new Gtk.ComboBox(store);
			var text = new Gtk.CellRendererText();
			Control.PackStart(text, false);
			Control.AddAttribute(text, "text", 0);			
			Control.Changed += delegate {
				Widget.OnSelectedIndexChanged(EventArgs.Empty);
			};
		}

		public void AddRange (IEnumerable<IListItem> collection)
		{
			foreach (var o in collection)
				AddItem(o);
		}
		
		public void AddItem (IListItem item)
		{
			store.AppendValues(item.Text, item);
		}

		public void RemoveItem (IListItem item)
		{
			Gtk.TreePath path = new Gtk.TreePath();
			path.AppendIndex(Widget.Items.IndexOf(item));
			Gtk.TreeIter iter;
			store.GetIter(out iter, path);
			store.Remove(ref iter);
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

