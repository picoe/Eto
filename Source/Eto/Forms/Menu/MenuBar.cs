using System;
using System.Collections.Generic;

namespace Eto.Forms
{
	/// <summary>
	/// Menu bar for a form
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[Handler(typeof(MenuBar.IHandler))]
	public class MenuBar : Menu, ISubmenu
	{
		MenuItemCollection items;
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Eto.Forms.MenuBar"/> will trim the items when set to a form
		/// </summary>
		/// <remarks>
		/// You may wish to turn this off if you are setting the menu regularily based on the context of your app, since
		/// it can effect performance.
		/// </remarks>
		/// <value><c>true</c> to auto trim; otherwise, <c>false</c>.</value>
		public bool Trim { get; set; }

		/// <summary>
		/// Gets the collection of menu items
		/// </summary>
		/// <value>The menu items</value>
		public MenuItemCollection Items { get { return items ?? (items = new MenuItemCollection(Handler)); } }

		/// <summary>
		/// Creates a menu bar with standard menu items
		/// </summary>
		/// <remarks>
		/// On OS X there are standard menu items required for all apps, otherwise keyboard commands like copy, paste, minimize, hide, etc
		/// will not function.  This creates a menu you can overlay your custom menu overtop.
		/// </remarks>
		/// <returns>A new instance of a standard menu for the platform</returns>
		/// <param name="commands">Commands to use from the platform, or null to use platform-supplied commands</param>
		public static MenuBar CreateStandardMenu(IEnumerable<Command> commands = null)
		{
			var menu = new MenuBar();
			Application.Instance.InternalCreateStandardMenu(menu.Items, commands);
			return menu;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.MenuBar"/> class.
		/// </summary>
		public MenuBar()
		{
			Trim = true;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.MenuBar"/> class with the specified items.
		/// </summary>
		/// <param name="items">Items to add to the menu bar initially.</param>
		public MenuBar(IEnumerable<MenuItem> items)
			: this()
		{
			Items.AddRange(items);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.MenuBar"/> class with the specified items.
		/// </summary>
		/// <param name="items">Items to add to the menu bar initially.</param>
		public MenuBar(params MenuItem[] items)
			: this()
		{
			Items.AddRange(items);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.MenuBar"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		[Obsolete("Use default constructor instead")]
		public MenuBar(Generator generator) : this(generator, typeof(MenuBar.IHandler))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.MenuBar"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		/// <param name="type">Type.</param>
		/// <param name="initialize">If set to <c>true</c> initialize.</param>
		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected MenuBar(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
			Trim = true;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.MenuBar"/> class.
		/// </summary>
		/// <param name="g">The green component.</param>
		/// <param name="items">Items.</param>
		[Obsolete("Use constructor without generator instead")]
		public MenuBar(Generator g, IEnumerable<MenuItem> items) : this(g)
		{
			Items.AddRange(items);
		}

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
				item.OnLoad(e);
		}

		/// <summary>
		/// Handler interface for the <see cref="MenuBar"/>
		/// </summary>
		public new interface IHandler : Menu.IHandler, ISubmenuHandler
		{
		}
	}
}