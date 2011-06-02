using System;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	public class OpenFileDialogHandler : WindowsFileDialog<SWF.OpenFileDialog, OpenFileDialog>, IOpenFileDialog
	{
		public OpenFileDialogHandler()
		{
			Control = new SWF.OpenFileDialog();
		}

	}
}
