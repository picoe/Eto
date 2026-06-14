namespace Eto.Forms;

/// <summary>
/// Result codes for <see cref="CommonDialog"/> or <see cref="MessageBox"/> dialogs
/// </summary>
/// <copyright>(c) 2014 by Curtis Wensley</copyright>
/// <license type="BSD-3">See LICENSE for full terms</license>
public enum DialogResult
{
	/// <summary>
	/// No specific result
	/// </summary>
	None,
	/// <summary>
	/// User clicked 'OK'
	/// </summary>
	Ok,
	/// <summary>
	/// User clicked 'Cancel' or pressed escape to cancel
	/// </summary>
	Cancel,
	/// <summary>
	/// User clicked 'Yes'
	/// </summary>
	Yes,
	/// <summary>
	/// User clicked 'No'
	/// </summary>
	No,
	/// <summary>
	/// User clicked 'Abort'
	/// </summary>
	Abort,
	/// <summary>
	/// User clicked 'Ignore'
	/// </summary>
	Ignore,
	/// <summary>
	/// User clicked 'Retry'
	/// </summary>
	Retry
}

/// <summary>
/// Base class for common dialogs
/// </summary>
public abstract class CommonDialog : Widget
{
	new IHandler Handler { get { return (IHandler)base.Handler; } }

	/// <summary>
	/// Initializes a new instance of the <see cref="Eto.Forms.CommonDialog"/> class.
	/// </summary>
	protected CommonDialog()
	{
	}

	/// <summary>
	/// Shows the dialog with the specified parent, blocking until a result is returned.
	/// </summary>
	/// <returns>The dialog result.</returns>
	/// <param name="parent">Parent control</param>
	public DialogResult ShowDialog(Control parent)
	{
		return ShowDialog(parent != null ? parent.ParentWindow : null);
	}

	/// <summary>
	/// Shows the dialog with the specified parent window, blocking until a result is returned.
	/// </summary>
	/// <returns>The dialog result.</returns>
	/// <param name="parent">Parent window.</param>
	public virtual DialogResult ShowDialog(Window parent)
	{
		return Handler.ShowDialog(parent);
	}

	/// <summary>
	/// Shows the dialog asynchronously with the specified parent.
	/// </summary>
	public Task<DialogResult> ShowDialogAsync(Control parent, CancellationToken cancellationToken = default)
	{
		return ShowDialogAsync(parent != null ? parent.ParentWindow : null, cancellationToken);
	}

	/// <summary>
	/// Shows the dialog asynchronously with the specified parent window.
	/// </summary>
	/// <remarks>
	/// The dialog is shown using the platform's native modal display, which keeps the UI responsive while the
	/// returned task is awaited.
	///
	/// Passing a <see cref="CancellationToken"/> that <see cref="CancellationToken.CanBeCanceled">can be cancelled</see>
	/// requires the platform handler to implement <see cref="ICancellableHandler"/>, otherwise a
	/// <see cref="NotSupportedException"/> is thrown. When no cancellable token is supplied the dialog is shown
	/// asynchronously on all platforms regardless of cancellation support.
	/// </remarks>
	/// <param name="parent">Parent window.</param>
	/// <param name="cancellationToken">Token used to cancel the dialog while it is displayed.</param>
	public virtual Task<DialogResult> ShowDialogAsync(Window parent, CancellationToken cancellationToken = default)
	{
		Application.Instance.EnsureUIThread();

		if (cancellationToken.IsCancellationRequested)
			return Task.FromCanceled<DialogResult>(cancellationToken);

		// Only require cancellation support when the caller actually supplies a token that can be cancelled.
		// A missing/non-cancellable token still shows the dialog asynchronously on every platform.
		var cancellableHandler = Handler as ICancellableHandler;
		if (cancellationToken.CanBeCanceled && cancellableHandler == null)
			throw new NotSupportedException($"{GetType().Name} does not support cancellation of asynchronous display.");

		return AsyncModalDialog.Show(
			() => Handler.ShowDialog(parent),
			cancellableHandler != null ? cancellableHandler.CancelDialog : (Action)null,
			cancellationToken);
	}

	/// <summary>
	/// Handler interface for the <see cref="CommonDialog"/>
	/// </summary>
	public new interface IHandler : Widget.IHandler
	{
		/// <summary>
		/// Shows the dialog with the specified parent window, blocking until a result is returned.
		/// </summary>
		/// <returns>The dialog result.</returns>
		/// <param name="parent">Parent window.</param>
		DialogResult ShowDialog(Window parent);
	}

	/// <summary>
	/// Handler interface for common dialogs which support cancelling an active modal display.
	/// </summary>
	/// <remarks>
	/// Handlers implement this to allow <see cref="ShowDialogAsync(Window, CancellationToken)"/> to be cancelled
	/// via a <see cref="CancellationToken"/> while the dialog is shown. <see cref="CancelDialog"/> is always
	/// invoked on the UI thread while the dialog is being displayed.
	/// </remarks>
	public interface ICancellableHandler : IHandler
	{
		/// <summary>
		/// Cancels the active dialog, dismissing it as if the user cancelled it.
		/// </summary>
		void CancelDialog();
	}
}
