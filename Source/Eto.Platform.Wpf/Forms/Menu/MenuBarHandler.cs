using Eto.Forms;
using swc = System.Windows.Controls;

namespace Eto.Platform.Wpf.Forms.Menu
{
	public class MenuBarHandler : WidgetHandler<System.Windows.Controls.Menu, MenuBar>, IMenuBar
	{
		public MenuBarHandler ()
		{
			Control = new swc.Menu ();
		}

		public void AddMenu (int index, MenuItem item)
		{
			Control.Items.Insert(index, item.ControlObject);
		}

		public void RemoveMenu (MenuItem item)
		{
			Control.Items.Remove(item.ControlObject);
		}

		public void Clear ()
		{
			Control.Items.Clear ();
		}
	}
}
