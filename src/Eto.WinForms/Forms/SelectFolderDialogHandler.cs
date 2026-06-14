namespace Eto.WinForms.Forms
{
	public class SelectFolderDialogHandler : WidgetHandler<swf.FolderBrowserDialog, SelectFolderDialog>, SelectFolderDialog.IHandler, CommonDialog.ICancellableHandler
	{
		readonly Win32.CancellableModalDialog _cancellable = new Win32.CancellableModalDialog();

		public SelectFolderDialogHandler ()
		{
			Control = new swf.FolderBrowserDialog();
		}


		public DialogResult ShowDialog (Window parent)
		{
			if (parent?.HasFocus == false)
				parent.Focus();

			var dr = _cancellable.Show(() => parent != null
				? Control.ShowDialog((swf.IWin32Window)parent.ControlObject)
				: Control.ShowDialog());
			return dr.ToEto ();
		}

		// A WH_CBT hook captured the native dialog's window handle when it was shown (see CancellableModalDialog),
		// so the async ShowDialogAsync can dismiss exactly this dialog when its cancellation token is signalled.
		public void CancelDialog() => _cancellable.Cancel();

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

