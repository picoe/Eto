namespace Eto.WinForms.Forms
{
	public class OpenFileDialogHandler : WindowsFileDialog<swf.OpenFileDialog, OpenFileDialog>, OpenFileDialog.IHandler
	{
		public OpenFileDialogHandler()
		{
			Control = new swf.OpenFileDialog();
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
