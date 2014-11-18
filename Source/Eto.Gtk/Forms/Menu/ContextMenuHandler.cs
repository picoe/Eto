using Eto.Forms;

namespace Eto.GtkSharp.Forms.Menu
{
	public class ContextMenuHandler : MenuHandler<Gtk.Menu, ContextMenu, ContextMenu.ICallback>, ContextMenu.IHandler
	{
		public ContextMenuHandler()
		{
			Control = new Gtk.Menu();
			Control.ShowAll();
		}

		public void AddMenu(int index, MenuItem item)
		{
			Control.Insert((Gtk.Widget)item.ControlObject, index);
			SetChildAccelGroup(item);
		}

		public void RemoveMenu(MenuItem item)
		{
			Control.Remove((Gtk.Widget)item.ControlObject);
		}

		public void Clear()
		{
			foreach (Gtk.Widget w in Control.Children)
			{
				Control.Remove(w);
			}
		}

		protected override Keys GetShortcut()
		{
			return Keys.None;
		}

		public void Show(Control relativeTo)
		{
			ValidateItems();
			Control.ShowAll();
			Control.Popup();
		}
	}
}
