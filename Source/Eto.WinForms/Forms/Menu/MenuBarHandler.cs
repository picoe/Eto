using SWF = System.Windows.Forms;
using Eto.Forms;
using System.Collections.Generic;

namespace Eto.WinForms
{
	/// <summary>
	/// Summary description for MenuBarHandler.
	/// </summary>
	public class MenuBarHandler : WidgetHandler<SWF.MenuStrip, MenuBar>, MenuBar.IHandler
	{
		public MenuBarHandler()
		{
			Control = new SWF.MenuStrip();
		}

		#region IMenu Members

		public void AddMenu(int index, MenuItem item)
		{
			Control.Items.Insert(index, (SWF.ToolStripItem)item.ControlObject);
		}

		public void RemoveMenu(MenuItem item)
		{
			Control.Items.Remove((SWF.ToolStripItem)item.ControlObject);
		}

		public void Clear()
		{
			Control.Items.Clear();
		}

		#endregion

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
