using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.WinForms
{
	/// <summary>
	/// Summary description for MenuBarHandler.
	/// </summary>
	public class MenuBarHandler : WidgetHandler<SWF.MenuStrip, MenuBar>, IMenuBar, IMenu
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

	}
}
