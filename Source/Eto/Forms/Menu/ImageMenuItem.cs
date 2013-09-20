#if DESKTOP
using System;
using System.Collections;
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

		public Image Image
		{
			get { return handler.Image; }
			set { handler.Image = value; }
		}

		[Obsolete ("Use Image instead")]
		public Icon Icon
		{
			get { return Image as Icon; }
			set { Image = value; }
		}

		public void GenerateActions (IEnumerable<IActionItem> actionItems)
		{
			foreach (IActionItem ai in actionItems) {
				var mi = ai.Generate (this.Generator);
				if (mi != null)
					this.MenuItems.Add(mi);
			}
		}
	}
}
#endif