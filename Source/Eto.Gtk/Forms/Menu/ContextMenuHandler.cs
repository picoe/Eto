using Eto.Forms;

namespace Eto.GtkSharp
{
	public class ContextMenuHandler : MenuHandler<Gtk.Menu, ContextMenu>, IContextMenu
	{

		public ContextMenuHandler()
		{
			Control = new Gtk.Menu();
		}

		public void AddMenu(int index, MenuItem item)
		{
			Control.Insert((Gtk.Widget)item.ControlObject, index);
			
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
		
		public void Show (Control relativeTo)
		{
			ValidateItems ();
			Control.ShowAll ();
			Control.Popup ();
		}

	}
}
