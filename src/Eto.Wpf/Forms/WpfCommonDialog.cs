using mw = Microsoft.Win32;

namespace Eto.Wpf.Forms
{
	public abstract class WpfCommonDialog<TControl, TWidget> : WidgetHandler<TControl, TWidget>, CommonDialog.IHandler, CommonDialog.ICancellableHandler
		where TControl : mw.CommonDialog
		where TWidget : CommonDialog
	{
		readonly Win32.CancellableModalDialog _cancellable = new Win32.CancellableModalDialog();

		// A WH_CBT hook captured the native dialog's window handle when it was shown (see CancellableModalDialog),
		// so the async ShowDialogAsync can dismiss exactly this dialog when its cancellation token is signalled.
		public void CancelDialog() => _cancellable.Cancel();

		public virtual DialogResult ShowDialog (Window parent)
		{
			var result = _cancellable.Show(() => parent != null
				? Control.ShowDialog(parent.ControlObject as sw.Window)
				: Control.ShowDialog());
			WpfFrameworkElementHelper.ShouldCaptureMouse = false;
			return result != null && result.Value ? DialogResult.Ok : DialogResult.Cancel;
		}
	}
}
