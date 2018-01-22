using SWF = System.Windows.Forms;
using SD = System.Drawing;
using Eto.Forms;

namespace Eto.WinForms.Forms
{
	public class SelectFolderDialogHandler : WidgetHandler<SWF.FolderBrowserDialog, SelectFolderDialog>, SelectFolderDialog.IHandler
	{
		public SelectFolderDialogHandler ()
		{
			Control = new SWF.FolderBrowserDialog();
		}
	

		public DialogResult ShowDialog (Window parent)
		{
			SWF.DialogResult dr;
			if (parent != null) dr = Control.ShowDialog((SWF.IWin32Window)parent.ControlObject);
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

