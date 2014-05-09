using Eto.Forms;

namespace Eto.GtkSharp
{
	/// <summary>
	/// Summary description for MenuBarHandler.
	/// </summary>
	public class MenuBarHandler : MenuHandler<Gtk.MenuBar, MenuBar>, MenuBar.IHandler
	{

		public MenuBarHandler()
		{
			Control = new Gtk.MenuBar();
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

	}
}
