using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Eto.Forms
{
	/// <summary>
	/// Flags for the groups of system <see cref="MenuBar"/> items
	/// </summary>
	[Flags]
	public enum MenuBarSystemItems
	{
		/// <summary>
		/// Do not add any system items to the menu
		/// </summary>
		None = 0,
		/// <summary>
		/// Add common menu items
		/// </summary>
		Common = 1 << 0,
		/// <summary>
		/// Add a Quit menu item, if one is not specified by <see cref="MenuBar.QuitItem"/>
		/// </summary>
		Quit = 1 << 2,
		/// <summary>
		/// Add all system-defined menu bar items
		/// </summary>
		All = Common | Quit
	}

	/// <summary>
	/// Menu bar for a form
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[ContentProperty("Items")]
	[Handler(typeof(MenuBar.IHandler))]
	public class MenuBar : Menu, ISubmenu, IBindableWidgetContainer
	{
		bool loaded;
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
		/// Gets or sets which system items will be automatically included with the menu.
		/// </summary>
		/// <remarks>
		/// Some operating systems, such as OS X require you to create your own standard items to enable the ability to do things
		/// such as close a window, cut/paste, hide apps, quit an app, etc.  By default, the menu will be merged with your
		/// custom menu so that all of these system functions will work.
		/// 
		/// For Quit and About menu items, use the <see cref="QuitItem"/> and <see cref="AboutItem"/> instead so that they are placed
		/// in the correct/expected location on every platform.
		/// </remarks>
		/// <value>The include system items.</value>
		public MenuBarSystemItems IncludeSystemItems { get; set; }

		static readonly object SystemCommandsKey = new object();

		/// <summary>
		/// Gets the system commands for the menu.
		/// </summary>
		/// <remarks>
		/// These system commands are used for any of the items added when <see cref="IncludeSystemItems"/> is set to anything other than <see cref="MenuBarSystemItems.None"/>.
		/// You can modify this collection to remove items from the system menu, or update which items should be used instead.
		/// This is only needed for advanced scenarios and should not be required to be used in normal circumstances.
		/// </remarks>
		/// <value>The system commands.</value>
		public Collection<Command> SystemCommands
		{
			get { return Properties.Create<Collection<Command>>(SystemCommandsKey, () => new Collection<Command>(Handler.GetSystemCommands().ToList())); }
		}

		/// <summary>
		/// Gets the collection of menu items
		/// </summary>
		/// <value>The menu items</value>
		public MenuItemCollection Items { get { return items ?? (items = new MenuItemCollection(Handler, this)); } }

		static readonly object QuitItemKey = new object();

		/// <summary>
		/// Gets or sets the quit item for the application.
		/// </summary>
		/// <remarks>
		/// This allows you to set the quit item for the application.  Some platforms (OS X) may add a quit item
		/// to the menu even if one is not defined as it is standard practice to allow users to quit the application with
		///  a menu item.
		/// This will be in the File menu for most platforms, and the Application menu for OS X.
		/// </remarks>
		/// <value>The quit item on the menu.</value>
		public MenuItem QuitItem
		{
			get { return Properties.Get<MenuItem>(QuitItemKey); }
			set
			{ 
				Properties[QuitItemKey] = value;
				Handler.SetQuitItem(value);
			}
		}

		static readonly object AboutItemKey = new object();

		/// <summary>
		/// Gets or sets the item to show an about dialog for the application
		/// </summary>
		/// <remarks>
		/// This allows you to set an item to show an about dialog for the application.
		/// OS X will place this in the Application menu, other platforms place this at the bottom of the Help menu.
		/// </remarks>
		/// <value>The about item on the menu.</value>
		public MenuItem AboutItem
		{
			get { return Properties.Get<MenuItem>(AboutItemKey); }
			set
			{ 
				Properties[AboutItemKey] = value;
				Handler.SetAboutItem(value);
			}
		}

		/// <summary>
		/// Gets the menu that contains application-level items.
		/// </summary>
		/// <remarks>
		/// This allows you to change the application menu's text (File for most platforms, Application menu for OS X)
		/// </remarks>
		/// <value>The application menu.</value>
		public ButtonMenuItem ApplicationMenu
		{
			get { return Handler.ApplicationMenu; }
		}

		/// <summary>
		/// Gets the item collection for the <see cref="ApplicationMenu"/>, to easily add items declaratively/programatically.
		/// </summary>
		/// <seealso cref="ApplicationMenu"/>
		/// <value>The application items collection.</value>
		public MenuItemCollection ApplicationItems
		{
			get { return ApplicationMenu.Items; }
		}

		/// <summary>
		/// Gets the help menu for the application for showing help items
		/// </summary>
		/// <remarks>
		/// This allows you to change the help menu's text ('Help' by default on all platforms). This is usually used
		/// for the <see cref="AboutItem"/> on most platforms, other than OS X.
		/// </remarks>
		/// <value>The help menu.</value>
		public ButtonMenuItem HelpMenu
		{
			get { return Handler.HelpMenu; }
		}

		/// <summary>
		/// Gets the item collection for the <see cref="HelpMenu"/>, to easily add items declaratively/programatically.
		/// </summary>
		/// <value>The help items collection.</value>
		public MenuItemCollection HelpItems
		{
			get { return HelpMenu.Items; }
		}

		IEnumerable<BindableWidget> IBindableWidgetContainer.Children => Items;

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.MenuBar"/> class.
		/// </summary>
		public MenuBar()
		{
			Trim = true;
			IncludeSystemItems = MenuBarSystemItems.All;
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
		/// Called before the menu is assigned to a control/window
		/// </summary>
		/// <param name="e">Event arguments</param>
		internal protected override void OnPreLoad(EventArgs e)
		{
			if (!loaded)
			{
				Handler.CreateSystemMenu();
				if (Trim)
					Items.Trim();
				loaded = true;
			}

			base.OnPreLoad(e);
			foreach (var item in Items)
				item.OnPreLoad(e);
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
			/// <summary>
			/// Gets the menu that contains application-level items.
			/// </summary>
			/// <remarks>
			/// This allows you to change the application menu's text (File for most platforms, Application menu for OS X)
			/// </remarks>
			/// <value>The application menu.</value>
			ButtonMenuItem ApplicationMenu { get; }

			/// <summary>
			/// Gets the help menu for the application for showing help items
			/// </summary>
			/// <remarks>
			/// This allows you to change the help menu's text ('Help' by default on all platforms). This is usually used
			/// for the <see cref="AboutItem"/> on most platforms, other than OS X.
			/// </remarks>
			/// <value>The help menu.</value>
			ButtonMenuItem HelpMenu { get; }

			/// <summary>
			/// Gets or sets the quit item for the application.
			/// </summary>
			/// <remarks>
			/// This allows you to set the quit item for the application.  Some platforms (OS X) may add a quit item
			/// to the menu even if one is not defined as it is standard practice to allow users to quit the application with
			///  a menu item.
			/// This will be in the File menu for most platforms, and the Application menu for OS X.
			/// </remarks>
			/// <value>The quit item on the menu.</value>
			void SetQuitItem(MenuItem item);

			/// <summary>
			/// Gets or sets the item to show an about dialog for the application
			/// </summary>
			/// <remarks>
			/// This allows you to set an item to show an about dialog for the application.
			/// OS X will place this in the Application menu, other platforms place this at the bottom of the Help menu.
			/// </remarks>
			/// <value>The about item on the menu.</value>
			void SetAboutItem(MenuItem item);

			/// <summary>
			/// Creates the system menu when it is loaded onto a window for the first time.
			/// </summary>
			/// <remarks>
			/// This is called only once for a menu bar, when it is first set using <see cref="Window.Menu"/>.
			/// </remarks>
			void CreateSystemMenu();

			/// <summary>
			/// Obsolete version to create a system menu using legacy symantics.
			/// </summary>
			/// <remarks>
			/// For OS X, this means it uses the <see cref="Application.Name"/> instead of "Application". The new symantics
			/// uses "Application" always (OS X automatically shows the app's local name regardless of the name of the menu item).
			/// </remarks>
			void CreateLegacySystemMenu();

			/// <summary>
			/// Gets the system commands for the menu bar.
			/// </summary>
			/// <remarks>
			/// The system commands should be used by the <see cref="CreateSystemMenu"/> implementation by ID.
			/// Note that the commands may or may not exist in the collection of <see cref="MenuBar.SystemCommands"/>, as
			/// the collection can be modified to remove or modify commands before the menu is created.
			/// </remarks>
			/// <returns>The system commands to use when creating the system menu.</returns>
			IEnumerable<Command> GetSystemCommands();
		}
	}
}