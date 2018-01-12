using System;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Forms
{
	/// <summary>
	/// Interface for submenus to access common Items properties
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public interface ISubmenu
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

	/// <summary>
	/// Extensions for the <see cref="ISubmenu"/> interface.
	/// </summary>
	public static class SubmenuExtensions
	{
		/// <summary>
		/// Gets an enumeration of all children of the specified submenu.
		/// </summary>
		/// <remarks>
		/// This traverses all items of the submenu, and any children of those items if they implement <see cref="ISubmenu"/>
		/// as well.
		/// </remarks>
		/// <param name="submenu">Submenu to get all child menu items for.</param>
		/// <returns>Enumeration of all child menu items of the specified submenu.</returns>
		public static IEnumerable<MenuItem> GetChildren(this ISubmenu submenu)
		{
			foreach (var item in submenu.Items)
			{
				yield return item;
				var childmenu = item as ISubmenu;
				if (childmenu != null)
				{
					foreach (var child in childmenu.GetChildren())
					{
						yield return child;
					}
				}
			}
		}
	}
}

