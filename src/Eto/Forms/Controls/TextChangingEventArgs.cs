namespace Eto.Forms;

/// <summary>
/// Identifies what initiated a text change in events such as <see cref="TextBox.TextChanging"/>.
/// </summary>
public enum TextChangeSource
{
	/// <summary>
	/// The change came from the user, but the specific origin could not be determined by the platform.
	/// </summary>
	/// <remarks>
	/// This is the value used by the legacy <c>fromUser: true</c> constructors and for user-initiated
	/// changes that don't map to any of the more specific values below (e.g. drag/drop, dictation, or
	/// services on some platforms).
	/// </remarks>
	Unknown,

	/// <summary>
	/// The change was made programmatically, e.g. by setting the <see cref="TextControl.Text"/> property.
	/// </summary>
	/// <remarks>
	/// Equivalent to the legacy <c>fromUser: false</c>. <see cref="TextChangingEventArgs.FromUser"/> is
	/// <c>false</c> only for this value.
	/// </remarks>
	Programmatic,

	/// <summary>
	/// The user typed the change using the keyboard.
	/// </summary>
	Keyboard,

	/// <summary>
	/// The change was committed by an input method / text composition (IME), e.g. dead keys or CJK input.
	/// </summary>
	/// <remarks>
	/// Detection of composition is best-effort and platform dependent. Where a platform does not
	/// distinguish composition from plain typing, such changes are reported as <see cref="Keyboard"/>.
	/// </remarks>
	Composition,

	/// <summary>
	/// The change was a paste from the clipboard.
	/// </summary>
	Paste,

	/// <summary>
	/// The change was a cut to the clipboard (the selected text is removed).
	/// </summary>
	Cut
}

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
	/// Gets a value indicating that the change was initiated by the user, false when the change was made programmatically.
	/// </summary>
	/// <remarks>
	/// This is a convenience that returns <c>true</c> for any <see cref="Source"/> other than
	/// <see cref="TextChangeSource.Programmatic"/>. Use <see cref="Source"/> to determine how the user
	/// initiated the change (typing, paste, etc.).
	/// </remarks>
	public bool FromUser => Source != TextChangeSource.Programmatic;

	/// <summary>
	/// Gets a value indicating how the change was initiated, e.g. by typing, pasting, or programmatically.
	/// </summary>
	/// <remarks>
	/// Some values are best-effort and platform dependent — see the <see cref="TextChangeSource"/> members.
	/// </remarks>
	public TextChangeSource Source { get; }

	static TextChangeSource FromUserSource(bool fromUser) => fromUser ? TextChangeSource.Unknown : TextChangeSource.Programmatic;

	/// <summary>
	/// Initializes a new instance of the <see cref="Eto.Forms.TextChangingEventArgs"/> class.
	/// </summary>
	/// <param name="text">Text to be replaced in the range.</param>
	/// <param name="range">Range of text to be effected.</param>
	/// <param name="fromUser">Value indicating that the change was initiated from the user</param>
	public TextChangingEventArgs(string text, Range<int> range, bool fromUser)
		: this(text, range, FromUserSource(fromUser))
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Eto.Forms.TextChangingEventArgs"/> class.
	/// </summary>
	/// <param name="text">Text to be replaced in the range.</param>
	/// <param name="range">Range of text to be effected.</param>
	/// <param name="source">Value indicating how the change was initiated.</param>
	public TextChangingEventArgs(string text, Range<int> range, TextChangeSource source)
	{
		this.text = text ?? string.Empty;
		this.range = range;
		Source = source;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Eto.Forms.TextChangingEventArgs"/> class.
	/// </summary>
	/// <param name="text">Text to be replaced in the range.</param>
	/// <param name="range">Range of text to be effected.</param>
	/// <param name="oldText">Current text in the control.</param>
	/// <param name="fromUser">Value indicating that the change was initiated from the user</param>
	public TextChangingEventArgs(string text, Range<int> range, string oldText, bool fromUser)
		: this(text, range, oldText, FromUserSource(fromUser))
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Eto.Forms.TextChangingEventArgs"/> class.
	/// </summary>
	/// <param name="text">Text to be replaced in the range.</param>
	/// <param name="range">Range of text to be effected.</param>
	/// <param name="oldText">Current text in the control.</param>
	/// <param name="source">Value indicating how the change was initiated.</param>
	public TextChangingEventArgs(string text, Range<int> range, string oldText, TextChangeSource source)
	{
		this.text = text ?? string.Empty;
		this.range = range;
		this.oldText = oldText ?? string.Empty;
		Source = source;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Eto.Forms.TextChangingEventArgs"/> class.
	/// </summary>
	/// <param name="oldText">Old text for the control</param>
	/// <param name="newText">New text for the control</param>
	/// <param name="fromUser">Value indicating that the change was initiated from the user</param>
	public TextChangingEventArgs(string oldText, string newText, bool fromUser)
		: this(oldText, newText, FromUserSource(fromUser))
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Eto.Forms.TextChangingEventArgs"/> class.
	/// </summary>
	/// <param name="oldText">Old text for the control</param>
	/// <param name="newText">New text for the control</param>
	/// <param name="source">Value indicating how the change was initiated.</param>
	public TextChangingEventArgs(string oldText, string newText, TextChangeSource source)
	{
		this.oldText = oldText ?? string.Empty;
		this.newText = newText ?? string.Empty;
		Source = source;
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
		if (old.Length == 0)
			return text;
		var r = Range;
		var start = old.Substring(0, r.Start);
		if (r.End >= old.Length)
			return start + text;
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