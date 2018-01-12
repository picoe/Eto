using Eto.Forms;
using sw = System.Windows;
using swf = System.Windows.Forms;

namespace Eto.Wpf.Forms
{
	public class SelectFolderDialogHandler : WidgetHandler<swf.FolderBrowserDialog, SelectFolderDialog>, SelectFolderDialog.IHandler
	{
		public SelectFolderDialogHandler ()
		{
			Control = new swf.FolderBrowserDialog ();
		}

		public DialogResult ShowDialog (Window parent)
		{
			var dr = Control.ShowDialog ();
			return dr == swf.DialogResult.OK ? DialogResult.Ok : DialogResult.Cancel;
		}

		public string Title
		{
			get
			{
				return Control.Description;
			}
			set
			{
				Control.Description = value;
			}
		}

		public string Directory
		{
			get
			{
				return Control.SelectedPath;
			}
			set
			{
				Control.SelectedPath = value;
			}
		}
	}
}

