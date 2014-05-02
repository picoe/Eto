using System;

namespace Eto.Forms
{
	/// <summary>
	/// Event arguments when handling text input events
	/// </summary>
	/// <remarks>
	/// This is only useful on the iOS platform right now.
	/// </remarks>
	public class TextInputEventArgs : EventArgs
	{
		/// <summary>
		/// The entered text, or null if no text was entered.
		/// </summary>
		public string Text { get; private set; }

		/// <summary>
		/// Gets a value indicating that the 
		/// </summary>
		/// <value><c>true</c> if delete backwards; otherwise, <c>false</c>.</value>
		public bool DeleteBackwards { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TextInputEventArgs"/> class.
		/// </summary>
		/// <param name="text">Text that was input into the control</param>
		/// <param name="deleteBackwards">True if deleting backwards, false otherwise</param>
		public TextInputEventArgs(string text, bool deleteBackwards)
		{
			Text = text;
			DeleteBackwards = deleteBackwards;
		}
	}
}
