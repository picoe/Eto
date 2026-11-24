namespace Eto.Forms;

/// <summary>
/// Enumeration to define the dock position of a control, such as tabs in the <see cref="TabControl"/>
/// </summary>
public enum DockPosition
{
	/// <summary>
	/// Element is docked to the top of the content.
	/// </summary>
	Top,
	/// <summary>
	/// Element is docked to the left of the content, or the right when the control is right-to-left.
	/// </summary>
	Left,
	/// <summary>
	/// Element is docked to the right of the content, or the left when the control is right-to-left.
	/// </summary>
	Right,
	/// <summary>
	/// Element is docked to the bottom of the content.
	/// </summary>
	Bottom
}