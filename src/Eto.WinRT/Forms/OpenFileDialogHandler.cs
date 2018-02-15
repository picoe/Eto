#if TODO_XAML
using System.Collections.Generic;
using Eto.Forms;
using mw = Microsoft.Win32;
using sw = Windows.UI.Xaml;

namespace Eto.WinRT.Forms
{
	public class OpenFileDialogHandler : WpfFileDialog<mw.OpenFileDialog, OpenFileDialog>, IOpenFileDialog
	{
		public OpenFileDialogHandler ()
		{
			Control = new mw.OpenFileDialog ();
		}

		public bool MultiSelect
		{
			get { return Control.Multiselect; }
			set { Control.Multiselect = value; }
		}

		public IEnumerable<string> Filenames
		{
			get { return Control.FileNames; }
		}
	}
}
#endif