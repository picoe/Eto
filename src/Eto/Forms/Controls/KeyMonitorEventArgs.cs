namespace Eto.Forms;

/// <summary>
/// Arguments for key events observed by <see cref="Window.PreviewKeyDown"/> and <see cref="Window.PreviewKeyUp"/>.
/// </summary>
/// <remarks>
/// Unlike <see cref="KeyEventArgs"/> there is deliberately no way to mark the event as handled.
/// Monitoring a window's keys observes them, it never consumes them, so the focused control always
/// goes on to receive the key press. Use <see cref="Control.KeyDown"/> when you need to handle or
/// suppress a key.
/// </remarks>
public class KeyMonitorEventArgs : EventArgs
{
	/// <summary>
	/// Initializes a new instance of the <see cref="KeyMonitorEventArgs"/> class.
	/// </summary>
	/// <param name="keyData">Key and modifiers that were pressed</param>
	/// <param name="keyEventType">Type of key event</param>
	public KeyMonitorEventArgs(Keys keyData, KeyEventType keyEventType)
	{
		KeyData = keyData;
		KeyEventType = keyEventType;
	}

	/// <summary>
	/// Gets the type of the key event.
	/// </summary>
	/// <value>The type of the key event.</value>
	public KeyEventType KeyEventType { get; }

	/// <summary>
	/// Gets the raw key data (the combination of the <see cref="Key"/> and <see cref="Modifiers"/>)
	/// </summary>
	public Keys KeyData { get; }

	/// <summary>
	/// Gets the key value (without modifiers)
	/// </summary>
	public Keys Key => KeyData & Keys.KeyMask;

	/// <summary>
	/// Gets the modifier keys that were pressed for this event
	/// </summary>
	public Keys Modifiers => KeyData & Keys.ModifierMask;
}
