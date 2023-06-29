namespace Eto.WinForms.Forms
{
	public class SelectFolderDialogHandler : WidgetHandler<swf.FolderBrowserDialog, SelectFolderDialog>, SelectFolderDialog.IHandler
	{
		public SelectFolderDialogHandler ()
		{
			Control = new swf.FolderBrowserDialog();
		}
	

		public DialogResult ShowDialog (Window parent)
		{
			if (parent?.HasFocus == false)
				parent.Focus();

			swf.DialogResult dr;
			if (parent != null) dr = Control.ShowDialog((swf.IWin32Window)parent.ControlObject);
			else dr = Control.ShowDialog();
			return dr.ToEto ();
		}

		public string Title {
			get {
				return Control.Description;
			}
			set {
				Control.Description = value;
			}
		}

		public string Directory {
			get {
				return Control.SelectedPath;
			}
			set {
				Control.SelectedPath = value;
			}
		}
}
}

