using Eto.Forms;
using mw = Microsoft.Win32;
using sw = System.Windows;

namespace Eto.Wpf.Forms
{
	public class SaveFileDialogHandler : WpfFileDialog<mw.SaveFileDialog, SaveFileDialog>, SaveFileDialog.IHandler
	{
		public SaveFileDialogHandler ()
		{
			Control = new mw.SaveFileDialog ();
		}
	}
}
