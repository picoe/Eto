using Eto.Forms;
using System.Collections.Generic;

namespace Eto.GtkSharp.Forms.Menu
{
	/// <summary>
	/// Summary description for MenuBarHandler.
	/// </summary>
	public class MenuBarHandler : MenuHandler<Gtk.MenuBar, MenuBar, MenuBar.ICallback>, MenuBar.IHandler
	{

		public MenuBarHandler()
		{
			Control = new Gtk.MenuBar();
			Control.ShowAll();
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

		MenuItem quitItem;
		public void SetQuitItem(MenuItem item)
		{
			item.Order = 1000;
			if (quitItem != null)
				ApplicationMenu.Items.Remove(quitItem);
			else
				ApplicationMenu.Items.AddSeparator(999);
			ApplicationMenu.Items.Add(item);
			quitItem = item;
		}

		MenuItem aboutItem;
		public void SetAboutItem(MenuItem item)
		{
			item.Order = 1000;
			if (aboutItem != null)
				HelpMenu.Items.Remove(aboutItem);
			else
				HelpMenu.Items.AddSeparator(999);
			HelpMenu.Items.Add(item);
			aboutItem = item;
		}

		public void CreateSystemMenu()
		{
			// no system menu items
		}

		public void CreateLegacySystemMenu()
		{

		}
		public IEnumerable<Command> GetSystemCommands()
		{
			yield break;
		}

		public ButtonMenuItem ApplicationMenu
		{
			get { return Widget.Items.GetSubmenu(Application.Instance.Localize(Widget, "&File"), -100); }
		}

		public ButtonMenuItem HelpMenu
		{
			get { return Widget.Items.GetSubmenu(Application.Instance.Localize(Widget, "&Help"), 1000); }
		}
	}
}
