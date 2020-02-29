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
		string newText;
		string oldText;
		string text;
		Range<int>? range;

		internal bool NeedsOldText => oldText == null;

		internal void SetOldText(string oldText) => this.oldText = oldText ?? string.Empty;

		/// <summary>
		/// Gets the text that is to be inserted at the given <see cref="Range"/>, or string.Empty if text will be deleted.
		/// </summary>
		/// <value>The text to be inserted.</value>
		public string Text => text ?? (text = GetText());

		/// <summary>
		/// Gets the range that the text will be replaced or deleted.
		/// </summary>
		/// <remarks>
		/// When the <see cref="Text"/> is empty, then the specified range of text will be deleted.
		/// Otherwise, the text in the range will be replaced.
		/// Note that the length of the <see cref="Text"/> will not necessarily match the length of the range. 
		/// </remarks>
		/// <value>The range.</value>
		public Range<int> Range => range ?? (range = GetRange()).Value;

		/// <summary>
		/// Gets the entire old text for the control.
		/// </summary>
		/// <remarks>
		/// This is the same as the <see cref="TextControl.Text"/> property of the control.
		/// </remarks>
		/// <value>The old text value.</value>
		public string OldText => oldText;

		/// <summary>
		/// Gets the new text the control will contain after the change.
		/// </summary>
		/// <value>The new text.</value>
		public string NewText => newText ?? (newText = GetNewText());

		/// <summary>
		/// Gets a value indicating that the change was initiated by the user, false
		/// </summary>
		public bool FromUser { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TextChangingEventArgs"/> class.
		/// </summary>
		/// <param name="text">Text to be replaced in the range.</param>
		/// <param name="range">Range of text to be effected.</param>
		/// <param name="fromUser">Value indicating that the change was initiated from the user</param>
		public TextChangingEventArgs(string text, Range<int> range, bool fromUser)
		{
			this.text = text ?? string.Empty;
			this.range = range;
			FromUser = fromUser;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TextChangingEventArgs"/> class.
		/// </summary>
		/// <param name="text">Text to be replaced in the range.</param>
		/// <param name="range">Range of text to be effected.</param>
		/// <param name="oldText">Current text in the control.</param>
		/// <param name="fromUser">Value indicating that the change was initiated from the user</param>
		public TextChangingEventArgs(string text, Range<int> range, string oldText, bool fromUser)
		{
			this.text = text ?? string.Empty;
			this.range = range;
			this.oldText = oldText ?? string.Empty;
			FromUser = fromUser;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TextChangingEventArgs"/> class.
		/// </summary>
		/// <param name="newText">Old text for the control</param>
		/// <param name="oldText">New text for the control</param>
		/// <param name="fromUser">Value indicating that the change was initiated from the user</param>
		public TextChangingEventArgs(string oldText, string newText, bool fromUser)
		{
			this.oldText = oldText ?? string.Empty;
			this.newText = newText ?? string.Empty;
			FromUser = fromUser;
		}

		Range<int> GetRange()
		{
			var ot = OldText;
			var nt = NewText;
			int start = 0;
			for (int i = 0; i < ot.Length; i++)
			{
				if (i >= nt.Length || ot[i] != nt[i])
					break;
				start++;
			}

			int end = ot.Length - 1;
			for (int i = nt.Length - 1; i >= 0; i--)
			{
				if (end <= 0 || ot[end] != nt[i])
					break;
				end--;
			}

			return new Range<int>(start, end);
		}

		string GetNewText()
		{
			var old = OldText;
			var r = Range;
			var start = old.Substring(0, r.Start);
			var end = old.Substring(r.End + 1);
			return start + text + end;
		}

		string GetText()
		{
			var r = Range;
			if (r.Length() < 0 || newText == null)
				return string.Empty;
			var length = newText.Length - (OldText.Length - r.End - 1) - r.Start;
			if (length <= 0)
				return string.Empty;
			return newText.Substring(r.Start, length);
		}
	}
}