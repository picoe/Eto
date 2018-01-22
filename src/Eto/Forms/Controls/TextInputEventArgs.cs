using System;
using System.ComponentModel;

namespace Eto.Forms
{
	/// <summary>
	/// Event arguments when handling text input events
	/// </summary>
	public class TextInputEventArgs : CancelEventArgs
	{
		/// <summary>
		/// The entered text, or null if no text was entered.
		/// </summary>
		public string Text { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TextInputEventArgs"/> class.
		/// </summary>
		/// <param name="text">Text that was input into the control</param>
		public TextInputEventArgs(string text)
		{
			Text = text;
		}
	}
}
