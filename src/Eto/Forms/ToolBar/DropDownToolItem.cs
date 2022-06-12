namespace Eto.Forms
{
	/// <summary>
	/// Tool element to display a dropdown ContextMenu
	/// </summary>
	[Handler(typeof(DropDownToolItem.IHandler))]
	public class DropDownToolItem : ToolItem
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Gets or sets the context menu to show when the user clicks the button
		/// </summary>
		/// <value>The context menu to show, or null to have no menu</value>
		public ContextMenu ContextMenu
		{
			get { return Handler.ContextMenu; }
			set { Handler.ContextMenu = value; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.DropDownToolItem"/> class.
		/// </summary>
		public DropDownToolItem() : base()
		{
		}

		/// <summary>
		/// Handler for the <see cref="DropDownToolItem"/>.
		/// </summary>
		public new interface IHandler : ToolItem.IHandler, IContextMenuHost
		{
		}
	}
}
