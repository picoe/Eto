using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace Eto.Forms
{
	/// <summary>
	/// Collection for menu items.
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class MenuItemCollection : Collection<MenuItem>, IList
	{
		internal readonly Menu.ISubmenuHandler parent;
		internal readonly Menu parentItem;

		internal MenuItemCollection(Menu.ISubmenuHandler parent, Menu parentItem)
		{
			this.parent = parent;
			this.parentItem = parentItem;
		}

		/// <summary>
		/// Inserts an menu item at the specified index
		/// </summary>
		/// <param name="index">Index to add the item.</param>
		/// <param name="item">Item to add.</param>
		protected override void InsertItem(int index, MenuItem item)
		{
			base.InsertItem(index, item);
			parent.AddMenu(index, item);
			item.Parent = parentItem;
		}

		/// <summary>
		/// Removes the item at the specified index.
		/// </summary>
		/// <param name="index">Index of the item to remove.</param>
		protected override void RemoveItem(int index)
		{
			var item = this[index];
			base.RemoveItem(index);
			parent.RemoveMenu(item);
			item.Parent = null;
		}

		/// <summary>
		/// Clears the items.
		/// </summary>
		protected override void ClearItems()
		{
			foreach (var item in this)
				item.Parent = null;
			base.ClearItems();
			parent.Clear();
		}

		/// <summary>
		/// Trims the items in this collection and all submenus.
		/// </summary>
		public void Trim()
		{
			bool trimSeparator = true;
			for (int i = 0; i < Count;)
			{
				var item = this[i];
				if (item is SeparatorMenuItem)
				{
					if (trimSeparator)
					{
						RemoveAt(i);
						continue;
					}
					trimSeparator = true;
				}
				else
					trimSeparator = false;

				var subMenu = item as ISubmenu;
				if (subMenu != null && subMenu.Trim)
				{
					subMenu.Items.Trim();
					if (subMenu.Items.Count == 0)
					{
						RemoveAt(i);
						continue;
					}
				}
				i++;
			}
			if (trimSeparator && Count > 0)
				RemoveAt(Count - 1);
		}

		/// <summary>
		/// Adds a menu item to the specified collection based on its Order.
		/// </summary>
		/// <param name="item">Menu item to add</param>
		public new void Add(MenuItem item)
		{
			int previousIndex = -1;
			for (var i = 0; i < Count; ++i)
			{
				if (this[i].Order <= item.Order)
					previousIndex = i;
				else
					break;
			}
			Insert(previousIndex + 1, item);
		}

		/// <summary>
		/// Add the specified command with the specified order.
		/// </summary>
		/// <param name="command">Command to add.</param>
		/// <param name="order">Order of the command to add.</param>
		public MenuItem Add(Command command, int order = 0)
		{
			var item = command.CreateMenuItem();
			item.Order = order;
			Add(item);
			return item;
		}

		/// <summary>
		/// Adds the separator with the specified order.
		/// </summary>
		/// <param name="order">Order of the separator to add.</param>
		public void AddSeparator(int order = 0)
		{
			Add(new SeparatorMenuItem { Order = order });
		}

		/// <summary>
		/// Adds the specified menu items to the collection.
		/// </summary>
		/// <param name="items">Items to add.</param>
		public void AddRange(IEnumerable<MenuItem> items)
		{
			foreach (var item in items)
			{
				Add(item);
			}
		}

		/// <summary>
		/// Adds the specified commands to the collection starting at the specified order.
		/// </summary>
		/// <param name="commands">Commands to add.</param>
		/// <param name="order">Order of the items to add.</param>
		public void AddRange(IEnumerable<Command> commands, int order = 0)
		{
			foreach (var command in commands)
			{
				Add(command, order);
			}
		}

		/// <summary>
		/// Gets the submenu from the collection with the specified text, optionally creating one if not found.
		/// </summary>
		/// <returns>The submenu instance if found, or a new submenu instance added at the specified order.</returns>
		/// <param name="submenuText">Text of the submenu to find or add.</param>
		/// <param name="order">Order of the submenu item to add. Not used if there is already a submenu with the specified text.</param>
		/// <param name="plaintextMatch">If set to <c>true</c>, matches excluding any mnemonic symbol idenfifiers.</param>
		/// <param name="create">If set to <c>true</c>, creates the menu if it doesn't exist in the collection, otherwise <c>false</c>.</param>
		public ButtonMenuItem GetSubmenu(string submenuText, int order = 0, bool plaintextMatch = true, bool create = true)
		{
			if (string.IsNullOrEmpty(submenuText))
				throw new ArgumentOutOfRangeException(nameof(submenuText), "submenuText must be a non null, non-empty value");

			// replace accelerators if plaintextMatch is true
			string convert(string s) => plaintextMatch ? s?.Replace("&", "") : s;

			var matchText = convert(submenuText);

			bool match(ButtonMenuItem r) => convert(r.Text) == matchText;

			var submenu = this.OfType<ButtonMenuItem>().FirstOrDefault(match);

			if (submenu == null && create)
			{
				submenu = new ButtonMenuItem { Text = submenuText, Order = order, Trim = true };
				Add(submenu);
			}
			return submenu;
		}

		int IList.Add(object value)
		{
			var command = value as Command;
			if (command != null)
				Add(command.CreateMenuItem());
			else
				Add((MenuItem)value);
			return Count - 1;
		}
	}
}
