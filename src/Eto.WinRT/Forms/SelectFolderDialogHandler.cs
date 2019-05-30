#if TODO_XAML
using Eto.Forms;
using sw = Windows.UI.Xaml;
using swf = Windows.UI.Xaml.Forms;

namespace Eto.WinRT.Forms
{
	public class SelectFolderDialogHandler : WidgetHandler<swf.FolderBrowserDialog, SelectFolderDialog>, ISelectFolderDialog
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

#endif