#if TODO_XAML
using Eto.Forms;
using swc = Windows.UI.Xaml.Controls;
using swm = Windows.UI.Xaml.Media;
using swi = Windows.UI.Xaml.Input;

namespace Eto.WinRT.Forms.Menu
{
	public class ButtonMenuItemHandler : MenuItemHandler<swc.MenuItem, ButtonMenuItem>, IButtonMenuItem
	{
		public ButtonMenuItemHandler ()
		{
			Control = new swc.MenuItem();
			Setup ();
		}
	}
}
#endif