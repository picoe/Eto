namespace Eto.Forms
{
	/// <summary>
	/// Tool item to display a drop-down menu
	/// </summary>
	[Handler(typeof(DropDownToolItem.IHandler))]
	public class DropDownToolItem : ToolItem
	{
		MenuItemCollection items;

		new IHandler Handler { get { return (IHandler)base.Handler; } }

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
		public MenuItemCollection Items { get { return items ?? (items = new MenuItemCollection(Handler, null)); } }

		/// <summary>
		/// Handler for the <see cref="DropDownToolItem"/>.
		/// </summary>
		public new interface IHandler : ToolItem.IHandler, Menu.ISubmenuHandler
		{
		}
	}
}
