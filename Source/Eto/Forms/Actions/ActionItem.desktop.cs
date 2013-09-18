#if DESKTOP
using System;

namespace Eto.Forms
{
	public partial interface IActionItem
	{
		MenuItem Generate(Generator generator);
	}
	
	public abstract partial class ActionItemBase
	{
		public abstract MenuItem Generate(Generator generator);
	}
	
	public partial class ActionItemSeparator : ActionItemBase
	{
		public override MenuItem Generate(Generator generator)
		{
			var mi = new SeparatorMenuItem(generator);
			if (!string.IsNullOrEmpty(MenuItemStyle))
				mi.Style = MenuItemStyle;
			return mi;
		}
	}
		
	public partial class ActionItemSubMenu
	{
		public override MenuItem Generate(Generator generator)
		{
			ImageMenuItem item = null;
			if (Actions.Count > 0)
			{
				item = new ImageMenuItem(generator);
				item.Text = SubMenuText;
				if (!string.IsNullOrEmpty(MenuItemStyle))
					item.Style = MenuItemStyle;
				Actions.Generate(item);
			}
			return item;
		}
	}
	
	public partial class ActionItem
	{
		public override MenuItem Generate(Generator generator)
		{
			var item = this.Action.Generate(this, generator);
			if (item != null)
			{
				if (!string.IsNullOrEmpty(MenuItemStyle))
					item.Style = MenuItemStyle;
			}
			return item;
		}
	}
}
#endif