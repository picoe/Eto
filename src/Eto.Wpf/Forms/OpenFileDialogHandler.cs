using System.Collections.Generic;
using Eto.Forms;
using mw = Microsoft.Win32;
using sw = System.Windows;

namespace Eto.Wpf.Forms
{
	public class OpenFileDialogHandler : WpfFileDialog<mw.OpenFileDialog, OpenFileDialog>, OpenFileDialog.IHandler
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
