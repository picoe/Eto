namespace Eto.Forms
{
	/// <summary>
	/// Segmented item that can have a drop down menu, and optionally be selected.
	/// </summary>
    [Handler(typeof(IHandler))]
    public class MenuSegmentedItem : SegmentedItem
    {
        new IHandler Handler => (IHandler)base.Handler;

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="T:Eto.Forms.MenuSegmentedItem"/> can be selected.
		/// </summary>
		/// <remarks>
		/// When this is <c>true</c>, the user will typically have to hold the button down to bring up the menu, otherwise
		/// a single click will bring up the menu and the item will never be selected/clicked.
		/// </remarks>
		/// <value><c>true</c> if this item can be selected; otherwise, <c>false</c>.</value>
        public bool CanSelect
        {
            get => Handler.CanSelect;
            set => Handler.CanSelect = value;
        }

		/// <summary>
		/// Gets or sets the menu to display when the user clicks the item.
		/// </summary>
		/// <seealso cref="CanSelect"/>.
		/// <value>The menu to display.</value>
        public ContextMenu Menu
        {
            get => Handler.Menu;
            set => Handler.Menu = value;
        }

		/// <summary>
		/// Handler interface for the <see cref="MenuSegmentedItem"/>.
		/// </summary>
        public new interface IHandler : SegmentedItem.IHandler
        {
			/// <summary>
			/// Gets or sets the menu to display when the user clicks the item.
			/// </summary>
			/// <seealso cref="CanSelect"/>.
			/// <value>The menu to display.</value>
			ContextMenu Menu { get; set; }

			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="T:Eto.Forms.MenuSegmentedItem"/> can be selected.
			/// </summary>
			/// <remarks>
			/// When this is <c>true</c>, the user will typically have to hold the button down to bring up the menu, otherwise
			/// a single click will bring up the menu and the item will never be selected/clicked.
			/// </remarks>
			/// <value><c>true</c> if this item can be selected; otherwise, <c>false</c>.</value>
			bool CanSelect { get; set; }
        }
    }
}
