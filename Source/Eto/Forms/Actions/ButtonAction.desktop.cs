#if DESKTOP
using System;

namespace Eto.Forms
{
	
	public partial class ButtonAction
	{
		public override MenuItem GenerateMenuItem(Generator generator)
		{
			if (command != null)
				return command.CreateMenuItem(generator);

			var mi = new ButtonMenuItem(generator);
			mi.Text = MenuText;
			mi.Shortcut = Accelerator;
			mi.Enabled = Enabled;
			if (Image != null) mi.Image = Image;
			if (!string.IsNullOrEmpty(MenuItemStyle))
				mi.Style = MenuItemStyle;
			new MenuConnector(this, mi);
			return mi;
		}

		#pragma warning disable 0618
		protected class MenuConnector
		{
			readonly ButtonMenuItem menuItem;
			readonly ButtonAction action;
			
			public MenuConnector(ButtonAction action, ButtonMenuItem menuItem)
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
		#pragma warning restore 0618
	}
}
#endif
