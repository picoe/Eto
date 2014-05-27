using System;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Forms
{
	/// <summary>
	/// Sub menu widget interface
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[Obsolete("Use ISubmenu instead")]
	public interface ISubMenuWidget
	{
		/// <summary>
		/// Gets the collection of menu items to show in the submenu.
		/// </summary>
		/// <value>The menu items.</value>
		MenuItemCollection Items { get; }

		/// <summary>
		/// Gets a value indicating whether this sub menu should trim its child menu items when loaded onto a form
		/// </summary>
		/// <remarks>
		/// Trimming will collapse any duplicate splitter items.  This is done so that you can easily merge your menus.
		/// </remarks>
		/// <value><c>true</c> to trim the child menu items; otherwise, <c>false</c>.</value>
		bool Trim { get; }
	}

	#pragma warning disable 612,618

	/// <summary>
	/// Interface for submenus to access common Items properties
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public interface ISubmenu : ISubMenuWidget
	{
	}

	#pragma warning restore 612,618
}

