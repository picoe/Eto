using System;
using Eto.Forms;

namespace Eto.Forms
{
	/// <summary>
	/// Defines an interface for controls or classes that implement keyboard events.
	/// </summary>
	public interface IKeyboardInputSource
	{
		/// <summary>
		/// Occurs when a key was released
		/// </summary>
		event EventHandler<KeyEventArgs> KeyUp;

		/// <summary>
		/// Occurs when a key has been pressed and is down
		/// </summary>
		event EventHandler<KeyEventArgs> KeyDown;

		/// <summary>
		/// Occurs when text is input for the control.
		/// </summary>
		event EventHandler<TextInputEventArgs> TextInput;
	}
}
