using System;

namespace Eto.Forms
{
    public enum KeyType
    {
        KeyDown,
        KeyUp,
    }

	/// <summary>
	/// Arguments for key press events
	/// </summary>
	public class KeyPressEventArgs : EventArgs
	{
        public KeyType KeyType { get; set; }

        char? keyChar;
		
		/// <summary>
		/// Initializes a new instance of the KeyPressEventArgs class for a character key press
		/// </summary>
		/// <param name="keyData">key and modifiers that were pressed</param>
		/// <param name="keyType">Type of key event</param>
		/// <param name="keyChar">character equivalent</param>
		public KeyPressEventArgs(Key keyData, KeyType keyType, char? keyChar = null)
		{
            this.KeyData = keyData;
			this.keyChar = keyChar;
            this.KeyType = keyType;
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

        /// Gets or sets a value indicating whether the key 
        /// event should be passed on to the underlying control.
        public bool SuppressKeyPress { get; set; } 

		/// <summary>
		/// Gets the key character corresponding to the key press (if <see cref="IsChar"/> is true)
		/// </summary>
		public char KeyChar
		{
			get { return keyChar != null ? keyChar.Value : char.MaxValue; }
        }

        #region Predicates

        public bool Shift
        {
            get { return (this.KeyData & Forms.Key.Shift) != 0; }
        }

        public bool Control
        {
            get { return (this.KeyData & Forms.Key.Control) != 0; }
        }

        public bool Alt
        {
            get { return (this.KeyData & Forms.Key.Alt) != 0; }
        }

        public bool IsKeyUp(Key key)
        {
            return
                this.KeyType == KeyType.KeyUp &&
                this.Key == key;
        }

        public bool IsControlKeyUp(Key key)
        {
            return
                this.KeyType == KeyType.KeyUp &&
                this.KeyData == (key | Key.Control);
        }

        public bool IsShiftKeyUp(Key key)
        {
            return
                this.KeyType == KeyType.KeyUp &&
                this.KeyData == (key | Key.Shift);
        }

        public bool IsAltKeyUp(Key key)
        {
            return
                this.KeyType == KeyType.KeyUp &&
                this.KeyData == (key | Key.Alt);
        }

        public bool IsAltShiftKeyUp(Key key)
        {
            return
                this.KeyType == KeyType.KeyUp &&
                this.KeyData == (key | Key.Alt | Key.Shift);
        }

        #endregion
    }
}

