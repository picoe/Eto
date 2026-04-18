namespace Eto.Forms;

/// <summary>
/// Event arguments used to provide the current text insertion bounds for a drawable.
/// </summary>
public class TextInsertionBoundsEventArgs : EventArgs
{
	/// <summary>
	/// Initializes a new instance of the <see cref="TextInsertionBoundsEventArgs"/> class.
	/// </summary>
	/// <param name="bounds">Text insertion bounds in drawable client coordinates.</param>
	public TextInsertionBoundsEventArgs(RectangleF? bounds = null)
	{
		Bounds = bounds;
	}

	/// <summary>
	/// Gets or sets the current text insertion bounds in drawable client coordinates.
	/// </summary>
	public RectangleF? Bounds { get; set; }
}
