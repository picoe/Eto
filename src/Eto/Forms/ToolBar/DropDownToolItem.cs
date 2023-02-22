using System.ComponentModel;

namespace Eto.Forms;

/// <summary>
/// Tool item to display a drop-down menu
/// </summary>
[Handler(typeof(DropDownToolItem.IHandler))]
public class DropDownToolItem : ToolItem
{
	MenuItemCollection items;

	new IHandler Handler => (IHandler)base.Handler;
		
	/// <summary>
	/// Gets or sets a value indicating that the drop arrow should be shown
	/// </summary>
	/// <value><c>true</c> to show the drop arrow, <c>false</c> to hide it</value>
	[DefaultValue(true)]
	public bool ShowDropArrow
	{
		get => Handler.ShowDropArrow;
		set => Handler.ShowDropArrow = value;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Eto.Forms.DropDownToolItem"/> class.
	/// </summary>
	public DropDownToolItem()
	{
	}

	/// <summary>
	/// Gets the collection of menu items
	/// </summary>
	/// <value>The menu items</value>
	public MenuItemCollection Items => items ?? (items = new MenuItemCollection(Handler, null));

	/// <summary>
	/// Handler for the <see cref="DropDownToolItem"/>.
	/// </summary>
	public new interface IHandler : ToolItem.IHandler, Menu.ISubmenuHandler
	{
		/// <summary>
		/// Gets or sets a value indicating that the drop arrow should be shown
		/// </summary>
		/// <value><c>true</c> to show the drop arrow, <c>false</c> to hide it</value>
		bool ShowDropArrow { get; set; }
	}
}