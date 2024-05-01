namespace Eto.Forms;

/// <summary>
/// General keyboard methods
/// </summary>
[Handler(typeof(IHandler))]
public static class Keyboard
{
	static IHandler Handler => Platform.Instance.CreateShared<IHandler>();

	/// <summary>
	/// Gets an enumeration of all keys supported by the <see cref="IsKeyLocked"/> method.
	/// </summary>
	/// <value>The supported lock keys.</value>
	public static IEnumerable<Keys> SupportedLockKeys => Handler.SupportedLockKeys;

	/// <summary>
	/// Determines if the specified <paramref name="key"/> is in a locked state, such as the <see cref="Keys.CapsLock"/>, 
	/// <see cref="Keys.ScrollLock"/>, <see cref="Keys.NumberLock"/>, or <see cref="Keys.Insert"/> key.
	/// </summary>
	/// <returns><c>true</c> if the specified key is locked; otherwise, <c>false</c>.</returns>
	/// <param name="key">Key to determine the state.</param>
	public static bool IsKeyLocked(Keys key) => Handler.IsKeyLocked(key);

	/// <summary>
	/// Gets the current modifier state for keys such as <see cref="Keys.Control"/>, <see cref="Keys.Alt"/> and <see cref="Keys.Shift"/>.
	/// </summary>
	/// <remarks>
	/// This typically will only return a value for the current event, such as during a mouse or keyboard event.
	/// </remarks>
	public static Keys Modifiers => Handler.Modifiers;
	
	/// <summary>
	/// Event to handle when the <see cref="Modifiers"/> or <see cref="SupportedLockKeys"/> state has changed
	/// </summary>
	/// <remarks>
	/// Note that this event is long-lived, so if you subscribe to this event to an instance method of a control or 
	/// short-lived object be sure to unsubscribe it otherwise that object will never be garbage collected.
	/// </remarks>
	public static event EventHandler<EventArgs> ModifiersChanged
	{
		add => Handler.ModifiersChanged += value;
		remove => Handler.ModifiersChanged -= value;
	}

	/// <summary>
	/// Handler for platforms to implement the <see cref="Keyboard"/> functionality.
	/// </summary>
	public interface IHandler
	{
		/// <summary>
		/// Gets an enumeration of all keys supported by the <see cref="IsKeyLocked"/> method.
		/// </summary>
		/// <value>The supported lock keys.</value>
		IEnumerable<Keys> SupportedLockKeys { get; }

		/// <summary>
		/// Determines if the specified <paramref name="key"/> is in a locked state, such as the <see cref="Keys.CapsLock"/>, 
		/// <see cref="Keys.ScrollLock"/>, <see cref="Keys.NumberLock"/>, or <see cref="Keys.Insert"/> key.
		/// </summary>
		/// <returns><c>true</c> if the specified key is locked; otherwise, <c>false</c>.</returns>
		/// <param name="key">Key to determine the state.</param>
		bool IsKeyLocked(Keys key);

		/// <summary>
		/// Gets the current modifier state for keys such as <see cref="Keys.Control"/>, <see cref="Keys.Alt"/> and <see cref="Keys.Shift"/>.
		/// </summary>
		/// <remarks>
		/// This typically will only return a value for the current event, such as during a mouse or keyboard event.
		/// </remarks>
		Keys Modifiers { get; }
		
		/// <summary>
		/// Event to handle when the <see cref="Modifiers"/> or <see cref="SupportedLockKeys"/> state has changed
		/// </summary>
		/// <remarks>
		/// Note that this event is long-lived, so if you subscribe to this event to an instance method of a control or 
		/// short-lived object be sure to unsubscribe it otherwise that object will never be garbage collected.
		/// </remarks>
		event EventHandler<EventArgs> ModifiersChanged;
	}
}