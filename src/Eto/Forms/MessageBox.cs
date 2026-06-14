namespace Eto.Forms;

/// <summary>
/// Message box buttons for methods of <see cref="MessageBox"/>
/// </summary>
/// <remarks>
/// This defined which buttons to show on the message box. 
/// If there are different buttons you require, you can use <see cref="Dialog"/> instead to create your own.
/// </remarks>
public enum MessageBoxButtons
{
	/// <summary>
	/// Only a single OK button
	/// </summary>
	OK,
	/// <summary>
	/// OK and Cancel buttons
	/// </summary>
	OKCancel,
	/// <summary>
	/// Yes and no buttons
	/// </summary>
	YesNo,
	/// <summary>
	/// Yes, no, and cancel buttons
	/// </summary>
	YesNoCancel
}

/// <summary>
/// Message box type, to define the appearance of a <see cref="MessageBox"/>
/// </summary>
/// <remarks>
/// Usually the icon shown on the message box will change depending on this value.
/// </remarks>
public enum MessageBoxType
{
	/// <summary>
	/// Informational message box, usually signified a lower case 'i' icon
	/// </summary>
	Information,
	/// <summary>
	/// Warning message box, usually signified by an exclamation mark icon
	/// </summary>
	Warning,
	/// <summary>
	/// Error message box, usually signified by a stop sign
	/// </summary>
	Error,
	/// <summary>
	/// Question message box, usually signified by a question mark icon
	/// </summary>
	Question
}

/// <summary>
/// Message box default button selection for a <see cref="MessageBox"/>
/// </summary>
/// <remarks>
/// This enumeration specifies the default button of the message box, which is usually focussed when the message
/// box is shown, and selectable when the user presses the Return key.
/// </remarks>
public enum MessageBoxDefaultButton
{
	/// <summary>
	/// Automatically select the default button, preferring the negative form first such as <see cref="Cancel"/>, then <see cref="No"/>.
	/// </summary>
	Default,
	/// <summary>
	/// The OK button is default
	/// </summary>
	OK,
	/// <summary>
	/// The Yes button is default
	/// </summary>
	Yes = OK,
	/// <summary>
	/// The No button is default
	/// </summary>
	No,
	/// <summary>
	/// The Cancel button is default
	/// </summary>
	Cancel
}

/// <summary>
/// Methods to show a standard message box with display text, buttons, and typically an icon indicating the type
/// </summary>
[Handler(typeof(IHandler))]
public static class MessageBox
{
	/// <summary>
	/// Shows a message box, blocking input to all windows of the application until closed
	/// </summary>
	/// <param name="text">Text for the body of the message box</param>
	/// <param name="type">Type of message box</param>
	public static DialogResult Show(string text, MessageBoxType type = MessageBoxType.Information)
	{
		return Show(null, text, null, type);
	}

	/// <summary>
	/// Shows a message box, blocking input to all windows of the application until closed
	/// </summary>
	/// <param name="text">Text for the body of the message box</param>
	/// <param name="caption">Caption for the title bar or heading of the message box</param>
	/// <param name="type">Type of message box</param>
	public static DialogResult Show(string text, string caption, MessageBoxType type = MessageBoxType.Information)
	{
		return Show(null, text, caption, type);
	}

	/// <summary>
	/// Shows a message box, blocking only the window of the specified <paramref name="parent"/> 
	/// </summary>
	/// <param name="parent">Parent control that triggered the message box</param>
	/// <param name="text">Text for the body of the message box</param>
	/// <param name="type">Type of message box</param>
	public static DialogResult Show(Control parent, string text, MessageBoxType type = MessageBoxType.Information)
	{
		return Show(parent, text, null, type);
	}

	/// <summary>
	/// Shows a message box, blocking only the window of the specified <paramref name="parent"/> 
	/// </summary>
	/// <param name="parent">Parent control that triggered the message box</param>
	/// <param name="text">Text for the body of the message box</param>
	/// <param name="type">Type of message box</param>
	/// <param name="caption">Caption for the title bar or heading of the message box</param>
	public static DialogResult Show(Control parent, string text, string caption, MessageBoxType type = MessageBoxType.Information)
	{
		return Show(parent, text, caption, MessageBoxButtons.OK, type);
	}

	/// <summary>
	/// Shows a message box, blocking input to all windows of the application until closed
	/// </summary>
	/// <param name="text">Text for the body of the message box</param>
	/// <param name="type">Type of message box</param>
	/// <param name="buttons">Buttons to show on the message box</param>
	/// <param name="defaultButton">Button to set focus to by default</param>
	public static DialogResult Show(string text, MessageBoxButtons buttons, MessageBoxType type = MessageBoxType.Information, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Default)
	{
		return Show((Control)null, text, buttons, type, defaultButton);
	}

	/// <summary>
	/// Shows a message box, blocking input to all windows of the application until closed
	/// </summary>
	/// <param name="text">Text for the body of the message box</param>
	/// <param name="type">Type of message box</param>
	/// <param name="buttons">Buttons to show on the message box</param>
	/// <param name="defaultButton">Button to set focus to by default</param>
	/// <param name="caption">Caption for the title bar or heading of the message box</param>
	public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxType type = MessageBoxType.Information, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Default)
	{
		return Show((Control)null, text, caption, buttons, type, defaultButton);
	}

	/// <summary>
	/// Shows a message box, blocking only the window of the specified <paramref name="parent"/> 
	/// </summary>
	/// <param name="parent">Parent control that triggered the message box</param>
	/// <param name="text">Text for the body of the message box</param>
	/// <param name="type">Type of message box</param>
	/// <param name="buttons">Buttons to show on the message box</param>
	/// <param name="defaultButton">Button to set focus to by default</param>
	public static DialogResult Show(Control parent, string text, MessageBoxButtons buttons, MessageBoxType type = MessageBoxType.Information, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Default)
	{
		return Show(parent, text, null, buttons, type, defaultButton);
	}

	/// <summary>
	/// Shows a message box, blocking only the window of the specified <paramref name="parent"/> 
	/// </summary>
	/// <param name="parent">Parent control that triggered the message box</param>
	/// <param name="text">Text for the body of the message box</param>
	/// <param name="caption">Caption for the title bar or heading of the message box</param>
	/// <param name="type">Type of message box</param>
	/// <param name="buttons">Buttons to show on the message box</param>
	/// <param name="defaultButton">Button to set focus to by default</param>
	public static DialogResult Show(Control parent, string text, string caption, MessageBoxButtons buttons, MessageBoxType type = MessageBoxType.Information, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Default)
	{
		Application.Instance.EnsureUIThread();
		var mb = Platform.Instance.Create<IHandler>();
		mb.Text = text;
		mb.Caption = caption;
		mb.Type = type;
		mb.Buttons = buttons;
		mb.DefaultButton = defaultButton;
		return mb.ShowDialog(parent);
	}

	/// <summary>
	/// Shows a message box asynchronously, blocking input to all windows of the application until closed
	/// </summary>
	/// <param name="text">Text for the body of the message box</param>
	/// <param name="type">Type of message box</param>
	/// <param name="cancellationToken">Token used to cancel/close the dialog.</param>
	public static Task<DialogResult> ShowAsync(string text, MessageBoxType type = MessageBoxType.Information, CancellationToken cancellationToken = default)
	{
		return ShowAsync((Control)null, text, null, type, cancellationToken);
	}

	/// <summary>
	/// Shows a message box asynchronously, blocking input to all windows of the application until closed
	/// </summary>
	/// <param name="text">Text for the body of the message box</param>
	/// <param name="caption">Caption for the title bar or heading of the message box</param>
	/// <param name="type">Type of message box</param>
	/// <param name="cancellationToken">Token used to cancel/close the dialog.</param>
	public static Task<DialogResult> ShowAsync(string text, string caption, MessageBoxType type = MessageBoxType.Information, CancellationToken cancellationToken = default)
	{
		return ShowAsync((Control)null, text, caption, type, cancellationToken);
	}

	/// <summary>
	/// Shows a message box asynchronously, blocking only the window of the specified <paramref name="parent"/> 
	/// </summary>
	/// <param name="parent">Parent control that triggered the message box</param>
	/// <param name="text">Text for the body of the message box</param>
	/// <param name="type">Type of message box</param>
	/// <param name="cancellationToken">Token used to cancel/close the dialog.</param>
	public static Task<DialogResult> ShowAsync(Control parent, string text, MessageBoxType type = MessageBoxType.Information, CancellationToken cancellationToken = default)
	{
		return ShowAsync(parent, text, null, type, cancellationToken);
	}

	/// <summary>
	/// Shows a message box asynchronously, blocking only the window of the specified <paramref name="parent"/> 
	/// </summary>
	/// <param name="parent">Parent control that triggered the message box</param>
	/// <param name="text">Text for the body of the message box</param>
	/// <param name="type">Type of message box</param>
	/// <param name="caption">Caption for the title bar or heading of the message box</param>
	/// <param name="cancellationToken">Token used to cancel/close the dialog.</param>
	public static Task<DialogResult> ShowAsync(Control parent, string text, string caption, MessageBoxType type = MessageBoxType.Information, CancellationToken cancellationToken = default)
	{
		return ShowAsync(parent, text, caption, MessageBoxButtons.OK, type, MessageBoxDefaultButton.Default, cancellationToken);
	}

	/// <summary>
	/// Shows a message box asynchronously, blocking input to all windows of the application until closed
	/// </summary>
	/// <param name="text">Text for the body of the message box</param>
	/// <param name="type">Type of message box</param>
	/// <param name="buttons">Buttons to show on the message box</param>
	/// <param name="defaultButton">Button to set focus to by default</param>
	/// <param name="cancellationToken">Token used to cancel/close the dialog.</param>
	public static Task<DialogResult> ShowAsync(string text, MessageBoxButtons buttons, MessageBoxType type = MessageBoxType.Information, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Default, CancellationToken cancellationToken = default)
	{
		return ShowAsync((Control)null, text, buttons, type, defaultButton, cancellationToken);
	}

	/// <summary>
	/// Shows a message box asynchronously, blocking input to all windows of the application until closed
	/// </summary>
	/// <param name="text">Text for the body of the message box</param>
	/// <param name="type">Type of message box</param>
	/// <param name="buttons">Buttons to show on the message box</param>
	/// <param name="defaultButton">Button to set focus to by default</param>
	/// <param name="caption">Caption for the title bar or heading of the message box</param>
	/// <param name="cancellationToken">Token used to cancel/close the dialog.</param>
	public static Task<DialogResult> ShowAsync(string text, string caption, MessageBoxButtons buttons, MessageBoxType type = MessageBoxType.Information, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Default, CancellationToken cancellationToken = default)
	{
		return ShowAsync((Control)null, text, caption, buttons, type, defaultButton, cancellationToken);
	}

	/// <summary>
	/// Shows a message box asynchronously, blocking only the window of the specified <paramref name="parent"/> 
	/// </summary>
	/// <param name="parent">Parent control that triggered the message box</param>
	/// <param name="text">Text for the body of the message box</param>
	/// <param name="type">Type of message box</param>
	/// <param name="buttons">Buttons to show on the message box</param>
	/// <param name="defaultButton">Button to set focus to by default</param>
	/// <param name="cancellationToken">Token used to cancel/close the dialog.</param>
	public static Task<DialogResult> ShowAsync(Control parent, string text, MessageBoxButtons buttons, MessageBoxType type = MessageBoxType.Information, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Default, CancellationToken cancellationToken = default)
	{
		return ShowAsync(parent, text, null, buttons, type, defaultButton, cancellationToken);
	}

	/// <summary>
	/// Shows a message box asynchronously, blocking only the window of the specified <paramref name="parent"/> 
	/// </summary>
	/// <param name="parent">Parent control that triggered the message box</param>
	/// <param name="text">Text for the body of the message box</param>
	/// <param name="caption">Caption for the title bar or heading of the message box</param>
	/// <param name="type">Type of message box</param>
	/// <param name="buttons">Buttons to show on the message box</param>
	/// <param name="defaultButton">Button to set focus to by default</param>
	/// <param name="cancellationToken">Token used to cancel/close the dialog.</param>
	public static Task<DialogResult> ShowAsync(Control parent, string text, string caption, MessageBoxButtons buttons, MessageBoxType type = MessageBoxType.Information, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Default, CancellationToken cancellationToken = default)
	{
		Application.Instance.EnsureUIThread();
		var mb = Platform.Instance.Create<IHandler>();
		mb.Text = text;
		mb.Caption = caption;
		mb.Type = type;
		mb.Buttons = buttons;
		mb.DefaultButton = defaultButton;

		if (cancellationToken.IsCancellationRequested)
			return Task.FromCanceled<DialogResult>(cancellationToken);

		// Platforms that cannot run a nested modal loop (e.g. mobile) display message boxes in an event-driven
		// fashion and provide their own asynchronous implementation.
		if (mb is IAsyncHandler asyncHandler)
			return asyncHandler.ShowDialogAsync(parent, cancellationToken);

		// Otherwise use the shared blocking-modal orchestration. Cancellation requires the handler to be able to
		// dismiss an active dialog; if it can't and a cancellable token was supplied, fail fast.
		var cancellableHandler = mb as ICancellableHandler;
		if (cancellationToken.CanBeCanceled && cancellableHandler == null)
			throw new NotSupportedException("This platform does not support cancellation of asynchronous message boxes.");

		return AsyncModalDialog.Show(
			() => mb.ShowDialog(parent),
			cancellableHandler != null ? cancellableHandler.CancelDialog : (Action)null,
			cancellationToken);
	}

	/// <summary>
	/// Handler interface for the <see cref="MessageBox"/>
	/// </summary>
	public interface IHandler
	{
		/// <summary>
		/// Gets or sets the body text of the message box.
		/// </summary>
		/// <value>The body text.</value>
		string Text { get; set; }
		/// <summary>
		/// Gets or sets the caption for the title bar or heading of the message box.
		/// </summary>
		/// <value>The caption.</value>
		string Caption { get; set; }
		/// <summary>
		/// Gets or sets the type of message box, usually changing the icon displayed.
		/// </summary>
		/// <value>The type of message box.</value>
		MessageBoxType Type { get; set; }
		/// <summary>
		/// Gets or sets which buttons to show on the message box.
		/// </summary>
		/// <value>The buttons to show.</value>
		MessageBoxButtons Buttons { get; set; }
		/// <summary>
		/// Gets or sets the default button, or <see cref="MessageBoxDefaultButton.Default"/> to automatically select either Cancel or No if displayed.
		/// </summary>
		/// <value>The default button.</value>
		MessageBoxDefaultButton DefaultButton { get; set; }
		/// <summary>
		/// Shows the dialog, blocking until a result is returned.
		/// </summary>
		/// <returns>The dialog result.</returns>
		/// <param name="parent">Optional parent. If specified, the parent's window should be blocked from input. If null, all windows should be blocked.</param>
		DialogResult ShowDialog(Control parent);
	}

	/// <summary>
	/// Handler interface for message boxes that can dismiss an active blocking modal display, enabling
	/// cancellation of <see cref="ShowAsync(Control, string, string, MessageBoxButtons, MessageBoxType, MessageBoxDefaultButton, CancellationToken)"/>.
	/// </summary>
	/// <remarks>
	/// Desktop platforms implement this; <see cref="CancelDialog"/> is invoked on the UI thread while the message
	/// box returned by <see cref="IHandler.ShowDialog(Control)"/> is being shown, and should dismiss it as if cancelled.
	/// </remarks>
	public interface ICancellableHandler : IHandler
	{
		/// <summary>
		/// Cancels the active message box, dismissing it as if the user cancelled it.
		/// </summary>
		void CancelDialog();
	}

	/// <summary>
	/// Handler interface for platforms that display message boxes asynchronously rather than via a blocking modal
	/// loop (e.g. mobile), providing their own asynchronous implementation.
	/// </summary>
	public interface IAsyncHandler : IHandler
	{
		/// <summary>
		/// Shows the message box asynchronously, completing when it closes.
		/// </summary>
		/// <param name="parent">Optional parent. If specified, the parent's window should be blocked from input. If null, all windows should be blocked.</param>
		/// <param name="cancellationToken">Optional cancellation token; handlers should close with a cancel response when requested.</param>
		Task<DialogResult> ShowDialogAsync(Control parent, CancellationToken cancellationToken = default);
	}
}
