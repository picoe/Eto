using System;
using Eto.Drawing;
using System.Collections.Generic;

namespace Eto.Forms
{
	public static class ImageMenuExtensions
	{
		public static ImageMenuItem Add (this MenuItemCollection items, string text)
		{
			var item = new ImageMenuItem (items.Parent.Generator);
			item.Text = text;
			items.Add (item);
			return item;
		}
	}
	
	public interface IImageMenuItem : IMenuActionItem, ISubMenu
	{
		Image Image { get; set; }
	}
	
	public class ImageMenuItem : MenuActionItem, ISubMenuWidget
	{
		new IImageMenuItem Handler { get { return (IImageMenuItem)base.Handler; } }

		readonly MenuItemCollection menuItems;
		
		public ImageMenuItem()
			: this((Generator)null)
		{
		}

		public ImageMenuItem (Generator generator) : this (generator, typeof(IImageMenuItem))
		{
		}
		
		protected ImageMenuItem (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
			menuItems = new MenuItemCollection (this, Handler);
		}

		public override Image Image
		{
			get { return Handler.Image; }
			set { Handler.Image = value; }
		}

		public void GenerateActions (IEnumerable<MenuItem> actionItems)
		{
			foreach (var mi in actionItems) {
				this.Add(mi);
			}
		}

		public void AddSeparator(int order)
		{
			this.Add(new SeparatorMenuItem
			{
				Order = order
			});
		}

		public void Add(List<BaseAction> actions, string actionId, int order)
		{
			foreach (var a in actions)
			{
				if (a.ID == actionId)
				{
					var mi = a.CreateMenuItem();
					mi.Order = order;
					this.Add(mi);
					break;
				}
			}
		}

		/// <summary>
		/// Adds a menu item based on its Order.
		/// </summary>
		/// <param name="menuItem"></param>
		public void Add(MenuItem menuItem)
		{
			AddMenuItem(this.menuItems, menuItem);
		}

		public void Remove(MenuItem menuItem)
		{
			this.menuItems.Remove(menuItem);
		}

		public IEnumerable<MenuItem> MenuItems
		{
			get
			{
				foreach (var m in this.menuItems)
					yield return m;
			}
		}

		/// <summary>
		/// Adds a menu item to the specified collection based on its Order.
		/// </summary>
		/// <param name="menuItems"></param>
		/// <param name="menuItem"></param>
		public static void AddMenuItem(MenuItemCollection menuItems, MenuItem menuItem)
		{
			int previousIndex = -1;
			if (menuItem.Order != 0)
				for (var i = 0; i < menuItems.Count; ++i)
				{
					if (menuItems[i].Order <= menuItem.Order)
						previousIndex = i;
					else
						break;
				}
			menuItems.Insert(previousIndex + 1, menuItem);
		}
	}
}