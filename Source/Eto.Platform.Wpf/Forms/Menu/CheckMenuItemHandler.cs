using Eto.Forms;
using swc = System.Windows.Controls;
using swm = System.Windows.Media;
using swi = System.Windows.Input;

namespace Eto.Platform.Wpf.Forms.Menu
{
	public class CheckMenuItemHandler : WpfMenuItem<swc.MenuItem, CheckMenuItem>, ICheckMenuItem
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
