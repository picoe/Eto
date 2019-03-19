using System;
using Eto.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Forms
{
	/// <summary>
	/// Menu item for a button / submenu
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[ContentProperty("Items")]
	[Handler(typeof(ButtonMenuItem.IHandler))]
	public class ButtonMenuItem : MenuItem, ISubmenu, IBindableWidgetContainer
	{
		MenuItemCollection items;

		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Gets the collection of menu items.
		/// </summary>
		/// <value>The items.</value>
		public MenuItemCollection Items { get { return items ?? (items = new MenuItemCollection(Handler, this)); } }

		/// <summary>
		/// Gets a value indicating whether this sub menu should trim its child menu items when loaded onto a form
		/// </summary>
		/// <remarks>Trimming will collapse any duplicate splitter items. This is done so that you can easily merge your menus.</remarks>
		public bool Trim { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ButtonMenuItem"/> class.
		/// </summary>
		public ButtonMenuItem()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ButtonMenuItem"/> class with the specified <paramref name="click"/> handler.
		/// </summary>
		/// <remarks>
		/// This is a convenience constructor to set up the click event.
		/// </remarks>
		/// <param name="click">Delegate to handle when the menu item is clicked.</param>
		public ButtonMenuItem(EventHandler<EventArgs> click)
		{
			Click += click;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ButtonMenuItem"/> class with the specified command.
		/// </summary>
		/// <param name="command">Command to initialize the menu item with.</param>
		public ButtonMenuItem(Command command)
			: base(command)
		{
			Image = command.Image;
			Handler.CreateFromCommand(command);
		}

		/// <summary>
		/// Gets or sets the image to show for the menu item.
		/// </summary>
		/// <remarks>
		/// Some platforms (e.g. OS X) will not show an image by default, but can be enabled using the handler implementation
		/// via styles.
		/// </remarks>
		/// <value>The image for the menu item.</value>
		public Image Image
		{
			get { return Handler.Image; }
			set { Handler.Image = value; }
		}

		IEnumerable<BindableWidget> IBindableWidgetContainer.Children => Items;

		/// <summary>
		/// Called when the menu is assigned to a control/window
		/// </summary>
		/// <param name="e">Event arguments</param>
		internal protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			foreach (var item in Items)
				item.OnLoad(e);
		}

		/// <summary>
		/// Called when the menu is removed from a control/window
		/// </summary>
		/// <param name="e">Event arguments</param>
		internal protected override void OnUnLoad(EventArgs e)
		{
			base.OnUnLoad(e);
			foreach (var item in Items)
				item.OnUnLoad(e);
		}

		/// <summary>
		/// Handler interface for the <see cref="ButtonMenuItem"/>.
		/// </summary>
		public new interface IHandler : MenuItem.IHandler, ISubmenuHandler
		{
			/// <summary>
			/// Gets or sets the image to show for the menu item.
			/// </summary>
			/// <remarks>
			/// Some platforms (e.g. OS X) will not show an image by default, but can be enabled using the handler implementation
			/// via styles.
			/// </remarks>
			/// <value>The image for the menu item.</value>
			Image Image { get; set; }
		}
	}
}