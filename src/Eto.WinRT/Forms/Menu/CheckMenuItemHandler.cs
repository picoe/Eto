#if TODO_XAML
using Eto.Forms;
using swc = Windows.UI.Xaml.Controls;
using swm = Windows.UI.Xaml.Media;
using swi = Windows.UI.Xaml.Input;

namespace Eto.WinRT.Forms.Menu
{
	public class CheckMenuItemHandler : MenuItemHandler<swc.MenuItem, CheckMenuItem>, ICheckMenuItem
	{
		public CheckMenuItemHandler ()
		{
			Control = new swc.MenuItem {
				IsCheckable = true
			};
			Setup ();
		}


		public bool Checked
		{
			get { return Control.IsChecked; }
			set { Control.IsChecked = value; }
		}
	}
}
#endif