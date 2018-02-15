#if TODO_XAML
using Eto.Forms;
using swc = Windows.UI.Xaml.Controls;

namespace Eto.WinRT.Forms.Menu
{
	public class MenuBarHandler : WidgetHandler<Windows.UI.Xaml.Controls.Menu, MenuBar>, IMenuBar
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
#endif