using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	/// <summary>
	/// Summary description for MenuBarHandler.
	/// </summary>
	public class MenuBarHandler : MenuHandler<Gtk.MenuBar, MenuBar>, IMenuBar
	{

		public MenuBarHandler()
		{
			Control = new Gtk.MenuBar();
		}

		public override void AddMenu(int index, MenuItem item)
		{
			Control.Insert((Gtk.Widget)item.ControlObject, index);
			
		}

		public override void RemoveMenu(MenuItem item)
		{
			Control.Remove((Gtk.Widget)item.ControlObject);
		}

		public override void Clear()
		{
			foreach (Gtk.Widget w in Control.Children)
			{
				Control.Remove(w);
			}
		}

	}
}
