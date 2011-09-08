using System;

namespace Eto.Forms
{
	public partial interface IActionItem
	{
		void Generate(ISubMenuWidget menu);
	}
	
	public abstract partial class ActionItemBase
	{
		public abstract void Generate(ISubMenuWidget menu);
	}
	
	public partial class ActionItemSeparator : ActionItemBase
	{
		public override void Generate(ISubMenuWidget menu)
		{
			menu.MenuItems.Add(new SeparatorMenuItem(menu.Generator));
		}
	}
	
	
	public partial class ActionItemSubMenu : ActionItemBase
	{
		public override void Generate(ISubMenuWidget menu)
		{
			if (actions.Count > 0)
			{
				var item = new ImageMenuItem(menu.Generator);
				item.Text = SubMenuText;
				actions.Generate(item);
				menu.MenuItems.Add(item);
			}
		}
	}
	
	public partial class ActionItem : ActionItemBase
	{
		public override void Generate(ISubMenuWidget menu)
		{
			var item = this.Action.Generate(this, menu);
			if (item != null)
				menu.MenuItems.Add (item);
		}
	}
	
	
}

