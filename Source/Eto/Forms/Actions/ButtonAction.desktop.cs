#if DESKTOP
using System;
using System.Reflection;
using System.Collections;
using Eto.Drawing;

namespace Eto.Forms
{
	
	public partial class ButtonAction : BaseAction
	{
		public override MenuItem GenerateMenuItem(ActionItem actionItem, Generator generator)
		{
			ImageMenuItem mi = new ImageMenuItem(generator);
			mi.Text = MenuText;
			mi.Shortcut = Accelerator;
			mi.Enabled = this.Enabled;
			if (Image != null) mi.Image = Image;
			if (!string.IsNullOrEmpty(MenuItemStyle))
				mi.Style = MenuItemStyle;
			new MenuConnector(this, mi);
			return mi;
		}
		
		protected class MenuConnector
		{
			ImageMenuItem menuItem;
			ButtonAction action;
			
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