using System;

namespace Eto.Forms
{
	
	public interface IContextMenu : ISubMenu
	{
		
	}
	
	public class ContextMenu : Menu, ISubMenuWidget
	{
		IContextMenu inner;
		MenuItemCollection menuItems;
		
		public ContextMenu()
			: this(Generator.Current)
		{
		}

		public ContextMenu(Generator g) : base(g, typeof(IContextMenu))
		{
			inner = (IContextMenu)this.Handler;
			menuItems = new MenuItemCollection(this, inner);
		}

		public ContextMenu(Generator g, ActionItemCollection actionItems) : this(g)
		{
			GenerateActions(actionItems);
		}
		
		public void GenerateActions (ActionItemCollection actionItems)
		{
			foreach (IActionItem ai in actionItems)
			{
				ai.Generate(this);
			}
		}

		#region IParentMenuWidget implementation
		
		public MenuItemCollection MenuItems {
			get { return menuItems; }
		}
		#endregion
}
}

