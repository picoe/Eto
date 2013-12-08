#if MENU_TOOLBAR_REFACTORING
#if DESKTOP
using System;

namespace Eto.Forms
{
	
	public partial class ButtonAction
	{
		public override MenuItem GenerateMenuItem(Generator generator)
		{
			var mi = new ImageMenuItem(generator);
			mi.Text = MenuText;
			mi.Shortcut = Accelerator;
			mi.Enabled = Enabled;
			if (Image != null) mi.Image = Image;
			if (!string.IsNullOrEmpty(MenuItemStyle))
				mi.Style = MenuItemStyle;
			new MenuConnector(this, mi);
			return mi;
		}
		
		protected class MenuConnector
		{
			readonly ImageMenuItem menuItem;
			readonly ButtonAction action;
			
			public MenuConnector(ButtonAction action, ImageMenuItem menuItem)
			{
				this.action = action;
				this.menuItem = menuItem;
				this.menuItem.Click += HandleClick;
				this.menuItem.Validate += HandleValidate;
			}

			void HandleClick (object sender, EventArgs e)
			{
				action.OnActivated(e);
			}

			void HandleValidate (object sender, EventArgs e)
			{
				menuItem.Enabled = action.Enabled;
			}
		}
	}
}
#endif
#endif