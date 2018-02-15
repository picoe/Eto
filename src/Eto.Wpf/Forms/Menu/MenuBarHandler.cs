using Eto.Forms;
using System.Collections.Generic;
using swc = System.Windows.Controls;
using sw = System.Windows;
using System.Linq;

namespace Eto.Wpf.Forms.Menu
{
	public class MenuBarHandler : MenuHandler<swc.Menu, MenuBar, MenuBar.ICallback>, MenuBar.IHandler
	{
		public MenuBarHandler()
		{
			Control = new swc.Menu();
		}

		public void AddMenu(int index, MenuItem item)
		{
			Control.Items.Insert(index, item.ControlObject);
			AddKeyBindings(item.ControlObject as sw.FrameworkElement);
		}

		public void RemoveMenu(MenuItem item)
		{
			RemoveKeyBindings(item.ControlObject as sw.FrameworkElement);
			Control.Items.Remove(item.ControlObject);
		}

		public void Clear()
		{
			foreach (var item in Control.Items.OfType<sw.FrameworkElement>())
				RemoveKeyBindings(item);
			Control.Items.Clear();
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
			get { return Widget.Items.GetSubmenu("&File", -100); }
		}

		public ButtonMenuItem HelpMenu
		{
			get { return Widget.Items.GetSubmenu("&Help", 1000); }
		}
	}
}
