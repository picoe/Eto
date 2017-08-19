using System;
using System.ComponentModel;

namespace Eto.Forms
{
	/// <summary>
	/// Arguments for events that handle when text is about to change, such as the <see cref="TextBox.TextChanging"/> event.
	/// </summary>
	/// <remarks>
	/// To cancel the change, set the inherited <see cref="CancelEventArgs.Cancel"/> property to true.
	/// </remarks>
	public class TextChangingEventArgs : CancelEventArgs
	{
		/// <summary>
		/// Gets the text that is to be inserted at the given <see cref="Range"/>, or string.Empty if text will be deleted.
		/// </summary>
		/// <value>The text to be inserted.</value>
		public string Text { get; private set; }

		/// <summary>
		/// Gets the range that the text will be replaced or deleted.
		/// </summary>
		/// <remarks>
		/// When the <see cref="Text"/> is empty, then the specified range of text will be deleted.
		/// Otherwise, the text in the range will be replaced.
		/// Note that the length of the <see cref="Text"/> will not necessarily match the length of the range. 
		/// </remarks>
		/// <value>The range.</value>
		public Range<int> Range { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TextChangingEventArgs"/> class.
		/// </summary>
		/// <param name="text">Text to be replaced in the range.</param>
		/// <param name="range">Range of text to be effected.</param>
		public TextChangingEventArgs(string text, Range<int> range)
		{
			Text = text;
			Range = range;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Eto.Forms.TextChangingEventArgs"/> class.
        /// </summary>
        /// <param name="text">Text to be replaced in the range.</param>
        public TextChangingEventArgs(string text)
        {
            Text = text;
        }
	}
	
}
