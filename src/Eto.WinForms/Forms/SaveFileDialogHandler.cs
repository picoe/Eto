namespace Eto.WinForms.Forms
{
	public class SaveFileDialogHandler : WindowsFileDialog<swf.SaveFileDialog, SaveFileDialog>, SaveFileDialog.IHandler
	{

		public SaveFileDialogHandler()
		{
			Control = new swf.SaveFileDialog();
		}

	}
}
