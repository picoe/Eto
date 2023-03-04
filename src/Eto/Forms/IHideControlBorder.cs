using System.ComponentModel;

namespace Eto.Forms;

/// <summary>
/// Defines an interface for controls or classes that implement toggling the visibility of their borders.
/// </summary>
public interface IHideControlBorder
{
	/// <summary>
	/// Gets or sets a value indicating whether to show the control's border.
	/// </summary>
	/// <remarks>
	/// This is a hint to omit the border of the control and show it as plainly as possible.
	/// Typically used when you want to show the control within a cell of the <see cref="GridView"/>.
	/// </remarks>
	/// <value><see langword="true"/> to show the control border; otherwise, <see langword="false"/>.</value>
	[DefaultValue(true)] 
	public bool ShowBorder { get; set; }
}