using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	public class SaveFileDialogHandler : WindowsFileDialog<SWF.SaveFileDialog, SaveFileDialog>, ISaveFileDialog
	{

		public SaveFileDialogHandler()
		{
			Control = new SWF.SaveFileDialog();
		}

	}
}
