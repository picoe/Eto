#if TODO_XAML
using Eto.Forms;
using mw = Microsoft.Win32;
using sw = Windows.UI.Xaml;

namespace Eto.Platform.Xaml.Forms
{
	public class SaveFileDialogHandler : WpfFileDialog<mw.SaveFileDialog, SaveFileDialog>, ISaveFileDialog
	{
		public SaveFileDialogHandler ()
		{
			Control = new mw.SaveFileDialog ();
		}
	}
}
#endif