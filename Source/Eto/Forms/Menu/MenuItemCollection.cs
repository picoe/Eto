using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Forms
{
	public class MenuItemCollection : Collection<MenuItem>
	{
		internal readonly ISubMenu parent;

		internal MenuItemCollection(ISubMenu parent)
		{
			this.parent = parent;
		}

		protected override void InsertItem(int index, MenuItem item)
		{
			base.InsertItem(index, item);
			parent.AddMenu(index, item);
		}

		protected override void RemoveItem(int index)
		{
			var item = this[index];
			base.RemoveItem(index);
			parent.RemoveMenu(item);
		}

		protected override void ClearItems()
		{
			base.ClearItems();
			parent.Clear();
		}

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

				var subMenu = item as ButtonMenuItem;
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
		/// <param name="item"></param>
		public new void Add(MenuItem item)
		{
			if (item.Order >= 0)
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
			else
				base.Add(item);
		}

		public void Add(Command command, int order = -1)
		{
			var item = command.CreateMenuItem(parent.Generator);
			item.Order = order;
			Add(item);
		}

		public void AddSeparator(int order = -1)
		{
			Add(new SeparatorMenuItem(parent.Generator) { Order = order });
		}

		public void AddRange(IEnumerable<MenuItem> items)
		{
			foreach (var item in items)
			{
				Add(item);
			}
		}

		public void AddRange(IEnumerable<Command> commands, int order = -1)
		{
			foreach (var command in commands)
			{
				Add(command, order);
			}
		}

		public ButtonMenuItem GetSubmenu(string submenuText, int order = -1, bool plaintextMatch = true, bool create = true)
		{
			// replace accelerators if plaintextMatch is true
			Func<string, string> convert = s => plaintextMatch ? s.Replace("&", "") : s;

			var matchText = convert(submenuText);
			var submenu = this.OfType<ButtonMenuItem>().FirstOrDefault(r => convert(r.Text) == matchText);

			if (submenu == null && create)
			{
				submenu = new ButtonMenuItem { Text = submenuText, Order = order, Trim = true };
				Add(submenu);
			}
			return submenu;
		}
	}
}
