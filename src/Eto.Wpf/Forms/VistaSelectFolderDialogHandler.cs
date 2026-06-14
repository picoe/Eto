using cp = Microsoft.WindowsAPICodePack.Dialogs;

#if WINFORMS
namespace Eto.WinForms.Forms
#elif WPF
namespace Eto.Wpf.Forms
#endif
{
	public class VistaSelectFolderDialogHandler : WidgetHandler<cp.CommonOpenFileDialog, SelectFolderDialog>, SelectFolderDialog.IHandler, CommonDialog.ICancellableHandler
	{
		readonly Win32.CancellableModalDialog _cancellable = new Win32.CancellableModalDialog();

		public VistaSelectFolderDialogHandler()
		{
			Control = new cp.CommonOpenFileDialog
			{
				IsFolderPicker = true
			};
		}

		// A WH_CBT hook captured the native dialog's window handle when it was shown (see CancellableModalDialog),
		// so the async ShowDialogAsync can dismiss exactly this dialog when its cancellation token is signalled.
		public void CancelDialog() => _cancellable.Cancel();

		public DialogResult ShowDialog(Window parent)
		{
			if (parent?.HasFocus == false)
				parent.Focus();

#if WINFORMS
			// use reflection since adding a parameter requires us to reference PresentationFramework which we don't want in winforms
			var handle = parent.ToNative()?.Handle;
			var result = _cancellable.Show(() =>
			{
				if (handle == null)
					return Control.ShowDialog();
				var showDialogMethod = Control.GetType().GetMethod("ShowDialog", new[] { typeof(IntPtr) });
				return (cp.CommonFileDialogResult)showDialogMethod.Invoke(Control, new object[] { handle.Value });
			});
#elif WPF
			// don't use WPF window, parent might be a HwndFormHandler
			var wpfParent = parent?.NativeHandle;
			var result = _cancellable.Show(() => wpfParent != null ? Control.ShowDialog(wpfParent.Value) : Control.ShowDialog());
			WpfFrameworkElementHelper.ShouldCaptureMouse = false;
#endif
			switch (result)
			{
				case cp.CommonFileDialogResult.Cancel:
					return DialogResult.Cancel;
				case cp.CommonFileDialogResult.Ok:
					return DialogResult.Ok;
				default:
				case cp.CommonFileDialogResult.None:
					return DialogResult.None;
			}
		}

		public string Title
		{
			get { return Control.Title; }
			set { Control.Title = value; }
		}

		public string Directory
		{
			get { return Control.FileName; }
			set { Control.InitialDirectory = value; }
		}
	}
}