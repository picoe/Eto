namespace Eto.Forms;

/// <summary>
/// Menu item to separate menu items
/// </summary>
/// <copyright>(c) 2014 by Curtis Wensley</copyright>
/// <license type="BSD-3">See LICENSE for full terms</license>
[Handler(typeof(SeparatorMenuItem.IHandler))]
public class SeparatorMenuItem : MenuItem
{
	/// <summary>
	/// Handler interface for the <see cref="SeparatorMenuItem"/>
	/// </summary>
	public new interface IHandler : MenuItem.IHandler
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Eto.Forms.SeparatorMenuItem"/> class.
	/// </summary>
	public SeparatorMenuItem()
	{
	}
	
	/// <summary>
	/// Initializes a new instance of the <see cref="Eto.Forms.SeparatorMenuItem"/> class with the specified handler.
	/// </summary>
	/// <param name="handler">The handler for the separator menu item.</param>
	public SeparatorMenuItem(IHandler handler)
		: base(handler)
	{
	}
}