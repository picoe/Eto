#if DESKTOP

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
	
	public partial class ActionItemSeparator
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
			ButtonMenuItem item = null;
			if (Actions.Count > 0)
			{
				item = new ButtonMenuItem(generator);
				item.Text = SubMenuText;
				item.ID = ID;
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
			var item = Action.GenerateMenuItem(generator);
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
