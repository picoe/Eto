using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;
using System.Collections.Generic;

namespace Eto.Platform.Windows
{
	public class OpenFileDialogHandler : WindowsFileDialog<SWF.OpenFileDialog, OpenFileDialog>, IOpenFileDialog
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
