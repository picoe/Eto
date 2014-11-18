using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.WinForms.Forms
{
	public class SaveFileDialogHandler : WindowsFileDialog<SWF.SaveFileDialog, SaveFileDialog>, SaveFileDialog.IHandler
	{

		public SaveFileDialogHandler()
		{
			Control = new SWF.SaveFileDialog();
		}

	}
}
