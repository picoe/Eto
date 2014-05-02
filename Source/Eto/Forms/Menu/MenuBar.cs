using System;
using System.Collections.Generic;

namespace Eto.Forms
{
	public interface IMenuBar : IMenu, ISubMenu
	{
	}

	/// <summary>
	/// Menu bar for a form
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class MenuBar : Menu, ISubMenuWidget
	{
		new IMenuBar Handler { get { return (IMenuBar)base.Handler; } }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Eto.Forms.MenuBar"/> will trim the items when set to a form
		/// </summary>
		/// <remarks>
		/// You may wish to turn this off if you are setting the menu regularily based on the context of your app, since
		/// it can effect performance.
		/// </remarks>
		/// <value><c>true</c> to auto trim; otherwise, <c>false</c>.</value>
		public bool AutoTrim { get; set; }

		/// <summary>
		/// Gets the collection of menu items
		/// </summary>
		/// <value>The menu items</value>
		public MenuItemCollection Items { get; private set; }

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
			var menu = new MenuBar(Generator.Current);
			Application.Instance.InternalCreateStandardMenu(menu.Items, commands);
			return menu;
		}

		public MenuBar()
			: this((Generator)null)
		{
		}

		public MenuBar(Generator generator) : this(generator, typeof(IMenuBar))
		{
		}

		protected MenuBar(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
			Items = new MenuItemCollection(Handler);
			AutoTrim = true;
		}

		public MenuBar(Generator g, IEnumerable<MenuItem> items) : this(g)
		{
			Items.AddRange(items);
		}

		internal protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			foreach (var item in Items)
				item.OnLoad(e);
		}

		internal protected override void OnUnLoad(EventArgs e)
		{
			base.OnUnLoad(e);
			foreach (var item in Items)
				item.OnLoad(e);
		}
	}
}