namespace Eto.Forms;

/// <summary>
/// Event arguments for live text composition updates such as inline composition preview text.
/// </summary>
public class TextCompositionEventArgs : HandledEventArgs
{
	/// <summary>
	/// Initializes a new instance of the <see cref="TextCompositionEventArgs"/> class.
	/// </summary>
	/// <param name="text">Current composition text.</param>
	/// <param name="isActive">Indicates whether composition is active.</param>
	public TextCompositionEventArgs(string text, bool isActive)
	{
		Text = text ?? string.Empty;
		IsActive = isActive;
	}

	/// <summary>
	/// Gets the current composition text.
	/// </summary>
	public string Text { get; }

	/// <summary>
	/// Gets a value indicating whether the composition is currently active.
	/// </summary>
	public bool IsActive { get; }
}
