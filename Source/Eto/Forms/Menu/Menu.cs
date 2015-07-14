using System;

namespace Eto.Forms
{
	/// <summary>
	/// Base class for menu items
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public abstract class Menu : BindableWidget
	{
		/// <summary>
		/// Called before the menu is assigned to a control/window
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected internal virtual void OnPreLoad(EventArgs e)
		{
		}

		/// <summary>
		/// Called when the menu is assigned to a control/window
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected internal virtual void OnLoad(EventArgs e)
		{
		}

		/// <summary>
		/// Called when the menu is removed from a control/window
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected internal virtual void OnUnLoad(EventArgs e)
		{
		}

		/// <summary>
		/// Handler interface for menus that implement a submenu
		/// </summary>
		/// <remarks>
		/// This is used by the <see cref="MenuItemCollection"/> to add/remove/clear menu items
		/// </remarks>
		public interface ISubmenuHandler
		{
			/// <summary>
			/// Adds the menu item
			/// </summary>
			/// <param name="index">Index to add the item</param>
			/// <param name="item">Item to add</param>
			void AddMenu(int index, MenuItem item);

			/// <summary>
			/// Removes the specified menu item
			/// </summary>
			/// <param name="item">Item to remove</param>
			void RemoveMenu(MenuItem item);

			/// <summary>
			/// Clears the menu of all items
			/// </summary>
			void Clear();
		}
	}
}