using System;

namespace Eto.Forms
{
	public interface IContextMenu : ISubMenu
	{
		void Show (Control relativeTo);	
	}
	
	public class ContextMenu : Menu, ISubMenuWidget
	{
		IContextMenu inner;
		MenuItemCollection menuItems;
		
		public ContextMenu ()
			: this(Generator.Current)
		{
		}

		public ContextMenu (Generator g) : base(g, typeof(IContextMenu))
		{
			inner = (IContextMenu)this.Handler;
			menuItems = new MenuItemCollection (this, inner);
		}

		public ContextMenu (Generator g, ActionItemCollection actionItems) : this(g)
		{
			GenerateActions (actionItems);
		}
		
		public void GenerateActions (ActionItemCollection actionItems)
		{
			foreach (IActionItem ai in actionItems) {
				ai.Generate (this);
			}
		}

		public MenuItemCollection MenuItems {
			get { return menuItems; }
		}
		
		public void Show (Control relativeTo)
		{
			inner.Show (relativeTo);
		}
	}
}

