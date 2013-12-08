#if DESKTOP
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

		public MenuItemCollection MenuItems {
			get { return menuItems; }
		}

		public Image Image
		{
			get { return Handler.Image; }
			set { Handler.Image = value; }
		}

		[Obsolete ("Use Image instead")]
		public Icon Icon
		{
			get { return Image as Icon; }
			set { Image = value; }
		}

		public void GenerateActions (IEnumerable<MenuItem> actionItems)
		{
			foreach (var mi in actionItems) {
				MenuItems.Add(mi);
			}
		}
	}
}
#endif