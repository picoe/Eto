using System;

namespace Eto.Forms
{
	public class KeyPressEventArgs : EventArgs
	{
		char? keyChar;
		
		public KeyPressEventArgs(Key key, char keyChar)
		{
			this.KeyData = key;
			this.keyChar = keyChar;
		}

		public KeyPressEventArgs(Key key)
		{
			this.KeyData = key;
			this.keyChar = null;
		}

		public Key KeyData { get; private set; }

		public Key Key
		{
			get { return KeyData & Key.KeyMask; }
		}

		public Key Modifiers
		{
			get { return KeyData & Key.ModifierMask; }
		}
		
		public bool IsChar
		{
			get { return (keyChar != null); }
		}

		public bool Handled { get; set; }
		
		public char KeyChar
		{
			get { return keyChar.HasValue ? keyChar.Value : char.MaxValue; }
		}
		
	}
}

