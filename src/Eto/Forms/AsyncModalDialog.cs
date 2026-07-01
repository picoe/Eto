namespace Eto.Forms;

/// <summary>
/// Shared orchestration for showing a <em>blocking</em> modal dialog asynchronously with optional cancellation.
/// </summary>
/// <remarks>
/// This drives the pattern used by both <see cref="CommonDialog.ShowDialogAsync(Window, CancellationToken)"/> and
/// <see cref="MessageBox.ShowAsync(Control, string, string, MessageBoxButtons, MessageBoxType, MessageBoxDefaultButton, CancellationToken)"/>:
/// the synchronous, blocking <c>showDialog</c> is posted onto the UI thread (where its native modal loop
/// keeps pumping), and <c>cancelDialog</c> is invoked - while it is showing - to break that loop when the
/// token is signalled.
///
/// This only works on platforms that run a nested modal loop for <c>ShowDialog</c> (i.e. desktop). Event-driven
/// platforms (e.g. mobile) must provide their own asynchronous display instead.
/// </remarks>
static class AsyncModalDialog
{
	/// <summary>
	/// Shows a blocking modal dialog asynchronously.
	/// </summary>
	/// <param name="showDialog">Synchronous, blocking call that displays the dialog and returns its result.</param>
	/// <param name="cancelDialog">
	/// Interrupts the active <paramref name="showDialog"/>, or <c>null</c> when the dialog cannot be cancelled.
	/// Callers must ensure a cancellable token is not paired with a <c>null</c> <paramref name="cancelDialog"/>.
	/// </param>
	/// <param name="cancellationToken">Token used to cancel the dialog while it is displayed.</param>
	public static Task<DialogResult> Show(Func<DialogResult> showDialog, Action cancelDialog, CancellationToken cancellationToken)
	{
		var tcs = new TaskCompletionSource<DialogResult>();
		var sync = new object();
		var cancellationRequested = false;
		var isShowing = false;

		var registration = default(CancellationTokenRegistration);
		if (cancelDialog != null && cancellationToken.CanBeCanceled)
		{
			registration = cancellationToken.Register(() =>
			{
				lock (sync)
				{
					cancellationRequested = true;
					// Only cancel the native dialog if it is currently being shown. If it hasn't been shown yet the
					// show callback below will observe the flag and never display it.
					if (isShowing)
						Application.Instance.AsyncInvoke(cancelDialog);
				}
			});
		}

		Application.Instance.AsyncInvoke(() =>
		{
			try
			{
				lock (sync)
				{
					if (cancellationRequested)
					{
						tcs.TrySetCanceled(cancellationToken);
						return;
					}
					isShowing = true;
				}

				var result = showDialog();

				bool wasCancelled;
				lock (sync)
				{
					isShowing = false;
					wasCancelled = cancellationRequested;
				}

				if (wasCancelled)
					tcs.TrySetCanceled(cancellationToken);
				else
					tcs.TrySetResult(result);
			}
			catch (Exception ex)
			{
				tcs.TrySetException(ex);
			}
			finally
			{
				registration.Dispose();
			}
		});

		return tcs.Task;
	}
}
