using Eto.Forms;

namespace Eto.GtkSharp
{
	/// <summary>
	/// Summary description for MenuBarHandler.
	/// </summary>
	public class MenuBarHandler : MenuHandler<Gtk.MenuBar, MenuBar, MenuBar.ICallback>, MenuBar.IHandler
	{

		public MenuBarHandler()
		{
			Control = new Gtk.MenuBar();
		}

		protected override Keys GetShortcut()
		{
			return Keys.None;
		}

		public void AddMenu(int index, MenuItem item)
		{
			Control.Insert((Gtk.Widget)item.ControlObject, index);
			SetChildAccelGroup(item);
		}

		public void RemoveMenu(MenuItem item)
		{
			Control.Remove((Gtk.Widget)item.ControlObject);
			var handler = item.Handler as IMenuHandler;
			if (handler != null)
				handler.SetAccelGroup(null);
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
