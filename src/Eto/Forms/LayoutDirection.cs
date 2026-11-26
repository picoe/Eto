namespace Eto.Forms;

/// <summary>
/// Specifies the direction controls are laid out in, such as for right-to-left languages.
/// </summary>
/// <remarks>
/// In right-to-left, layouts are mirrored and Left/Right alignment, padding and positions mean start/end.
/// Drawing, mouse locations and control coordinates are never mirrored, so x=0 is always the left edge.
/// </remarks>
/// <seealso cref="Control.LayoutDirection"/>
/// <seealso cref="Application.LayoutDirection"/>
/// <since>2.14</since>
public enum LayoutDirection
{
	/// <summary>
	/// Uses the direction of the parent control, the application for a window, or the system for the application.
	/// </summary>
	Inherit,
	/// <summary>
	/// Controls are laid out from left to right.
	/// </summary>
	LeftToRight,
	/// <summary>
	/// Controls are laid out from right to left, mirroring the layout.
	/// </summary>
	RightToLeft
}
