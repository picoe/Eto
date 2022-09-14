using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;
using System.Collections.Generic;

namespace Eto.WinForms.Forms
{
	public class OpenFileDialogHandler : WindowsFileDialog<SWF.OpenFileDialog, OpenFileDialog>, OpenFileDialog.IHandler
	{
		public OpenFileDialogHandler()
		{
			Control = new SWF.OpenFileDialog();
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
