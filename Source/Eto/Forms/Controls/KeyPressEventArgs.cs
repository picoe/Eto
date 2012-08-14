using System;

namespace Eto.Forms
{
	/// <summary>
	/// Arguments for key press events
	/// </summary>
	public class KeyPressEventArgs : EventArgs
	{
		char? keyChar;
		
		/// <summary>
		/// Initializes a new instance of the KeyPressEventArgs class for a character key press
		/// </summary>
		/// <param name="key">key and modifiers that were pressed</param>
		/// <param name="keyChar">character equivalent</param>
		public KeyPressEventArgs(Key key, char keyChar)
		{
			this.KeyData = key;
			this.keyChar = keyChar;
		}

		/// <summary>
		/// Initializes a new instance of the KeyPressEventArgs class for a non-character key press
		/// </summary>
		/// <param name="key">key and modifiers that were pressed</param>
		public KeyPressEventArgs(Key key)
		{
			this.KeyData = key;
			this.keyChar = null;
		}

		/// <summary>
		/// Gets the raw key data (the combination of the <see cref="Key"/> and <see cref="Modifiers"/>)
		/// </summary>
		public Key KeyData { get; private set; }

		/// <summary>
		/// Gets the key value (without modifiers)
		/// </summary>
		public Key Key
		{
			get { return KeyData & Key.KeyMask; }
		}

		/// <summary>
		/// Gets the modifier keys that were pressed for this event
		/// </summary>
		public Key Modifiers
		{
			get { return KeyData & Key.ModifierMask; }
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
			get { return keyChar.HasValue ? keyChar.Value : char.MaxValue; }
		}
		
	}
}

