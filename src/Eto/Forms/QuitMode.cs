namespace Eto.Forms;

/// <summary>
/// Specifies when the application quits on its own, set via <see cref="Application.QuitMode"/>.
/// </summary>
/// <remarks>
/// The <see cref="Application.Terminating"/> event is raised before quitting, and cancelling it also cancels
/// the close of the window that would have quit the application.
/// </remarks>
public enum QuitMode
{
	/// <summary>
	/// Uses the platform convention: <see cref="Explicit"/> on macOS, where the <see cref="Application.MainForm"/>
	/// is hidden when closed and shown again when clicking the dock icon, and <see cref="MainFormClosed"/> elsewhere.
	/// </summary>
	Default,

	/// <summary>
	/// Quits when the <see cref="Application.MainForm"/> is closed.
	/// </summary>
	MainFormClosed,

	/// <summary>
	/// Quits when the last open window is closed.
	/// </summary>
	/// <remarks>
	/// Only visible or minimized windows count. Dialogs always count, while <see cref="FloatingForm"/> and forms
	/// whose <see cref="Window.Owner"/> is still open do not.
	/// </remarks>
	LastWindowClosed,

	/// <summary>
	/// Only quits when <see cref="Application.Quit"/> is called.
	/// </summary>
	Explicit
}
