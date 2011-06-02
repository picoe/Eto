using System;

namespace Eto.Forms
{
	public partial interface IActionItem
	{
		void Generate(Menu menu);
	}
	
	public abstract partial class ActionItemBase
	{
		public abstract void Generate(Menu menu);
	}
	
	public partial class ActionItemSeparator : ActionItemBase
	{
		public override void Generate(Menu menu)
		{
			menu.MenuItems.Add(new SeparatorMenuItem(menu.Generator));
		}
	}
	
	
	public partial class ActionItemSubMenu : ActionItemBase
	{
		public override void Generate(Menu menu)
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
		//public BaseAction Action { get; set; }
		
		public override void Generate(Menu menu)
		{
			this.Action.Generate(this, menu);
		}
	}
	
	
}

