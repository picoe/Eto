using System;
using System.Collections;
using Eto.Drawing;

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
		Icon Icon { get; set; }
	}
	
	public class ImageMenuItem : MenuActionItem, ISubMenuWidget
	{
		IImageMenuItem handler;
		MenuItemCollection menuItems;
		
		public ImageMenuItem () : this (Generator.Current)
		{
		}
		
		public ImageMenuItem (Generator g) : this (g, typeof(IImageMenuItem))
		{
		}
		
		protected ImageMenuItem (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
			handler = (IImageMenuItem)base.Handler;
			menuItems = new MenuItemCollection (this, handler);
		}

		public MenuItemCollection MenuItems {
			get { return menuItems; }
		}

		public Icon Icon {
			get { return handler.Icon; }
			set { handler.Icon = value; }
		}
		
		public void GenerateActions (ActionItemCollection actionItems)
		{
			foreach (IActionItem ai in actionItems) {
				ai.Generate (this);
			}
		}
		
	}
}
