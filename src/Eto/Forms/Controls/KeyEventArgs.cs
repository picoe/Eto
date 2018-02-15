using System;

namespace Eto.Forms
{
	/// <summary>
	/// Type of key event
	/// </summary>
	public enum KeyEventType
	{
		/// <summary>
		/// The key was pressed
		/// </summary>
		KeyDown,
		/// <summary>
		/// The key was released
		/// </summary>
		KeyUp
	}

	/// <summary>
	/// Arguments for key press events
	/// </summary>
	public class KeyEventArgs : EventArgs
	{
		readonly char? keyChar;
		
		/// <summary>
		/// Initializes a new instance of the KeyPressEventArgs class for a character key press
		/// </summary>
		/// <param name="keyData">Key and modifiers that were pressed</param>
		/// <param name="keyEventType">Type of key event</param>
		/// <param name="keyChar">Character equivalent</param>
		public KeyEventArgs(Keys keyData, KeyEventType keyEventType, char? keyChar = null)
		{
            this.KeyData = keyData;
			this.KeyEventType = keyEventType;
			this.keyChar = keyChar;
		}

		/// <summary>
		/// Gets the type of the key event.
		/// </summary>
		/// <value>The type of the key event.</value>
		public KeyEventType KeyEventType { get; private set; }

		/// <summary>
		/// Gets the raw key data (the combination of the <see cref="Key"/> and <see cref="Modifiers"/>)
		/// </summary>
		public Keys KeyData { get; private set; }

		/// <summary>
		/// Gets the key value (without modifiers)
		/// </summary>
		public Keys Key
		{
			get { return KeyData & Keys.KeyMask; }
		}

		/// <summary>
		/// Gets the modifier keys that were pressed for this event
		/// </summary>
		public Keys Modifiers
		{
			get { return KeyData & Keys.ModifierMask; }
		}
		
		/// <summary>
		/// Gets a value indicating that the key press corresponds to a character input value
		/// </summary>
		public bool IsChar
		{
			get { return (keyChar != null); }
		}

		/// <summary>
		/// Gets or sets a value indicating that this event was handled by user code
		/// </summary>
		/// <remarks>
		/// If you pass true for this, typically the key press will not be passed to the control
		/// for event processing. This also allows controls to handle key combinations that would
		/// otherwise be handled as a shortcut in the menu or toolbar items.
		/// </remarks>
		public bool Handled { get; set; }

		/// <summary>
		/// Gets the key character corresponding to the key press (if <see cref="IsChar"/> is true)
		/// </summary>
		public char KeyChar
		{
			get { return keyChar != null ? keyChar.Value : char.MaxValue; }
        }

		/// <summary>
		/// Gets whether the shift key was pressed/released for the event
		/// </summary>
        public bool Shift
        {
			get { return KeyData.HasFlag (Keys.Shift); }
        }

		/// <summary>
		/// Gets whether the control key was pressed/released for the event
		/// </summary>
		public bool Control
        {
			get { return KeyData.HasFlag (Keys.Control); }
		}

		/// <summary>
		/// Gets whether the alt key was pressed/released for the event
		/// </summary>
		public bool Alt
        {
			get { return KeyData.HasFlag (Keys.Alt); }
		}

		/// <summary>
		/// Gets whether the application key was pressed/released for the event
		/// </summary>
		public bool Application
		{
			get { return KeyData.HasFlag (Keys.Application); }
		}

		/// <summary>
		/// Determines whether the specified key and modifier was released
		/// </summary>
		/// <returns><c>true</c> the key with modifier was released; otherwise, <c>false</c>.</returns>
		/// <param name="key">Key to test if it was released</param>
		/// <param name="modifier">Modifier of the key, or null to allow any modifiers</param>
		public bool IsKeyUp (Keys key, Keys? modifier = null)
		{
			return KeyEventType == KeyEventType.KeyUp && Key == key && (modifier == null || modifier == Modifiers);
		}

		/// <summary>
		/// Determines whether the specified key and modifier was pressed
		/// </summary>
		/// <returns><c>true</c> the key with modifier was pressed; otherwise, <c>false</c>.</returns>
		/// <param name="key">Key to test if it was pressed</param>
		/// <param name="modifier">Modifier of the key, or null to allow any modifiers</param>
		public bool IsKeyDown (Keys key, Keys? modifier = null)
		{
			return KeyEventType == KeyEventType.KeyDown && Key == key && (modifier == null || modifier == Modifiers);
		}
    }
}

