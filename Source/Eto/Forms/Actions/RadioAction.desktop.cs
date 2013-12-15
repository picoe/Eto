#if DESKTOP
using System;

namespace Eto.Forms
{
	public partial class RadioAction
	{
		RadioMenuItem menuItem;

		public override MenuItem GenerateMenuItem(Generator generator)
		{
			var mi = new RadioMenuItem((Controller != null) ? Controller.menuItem : null, generator);
			mi.Text = MenuText;
			mi.Shortcut = Accelerator;
			mi.Enabled = Enabled;
			mi.Checked = Checked;
			if (!string.IsNullOrEmpty(MenuItemStyle))
				mi.Style = MenuItemStyle;

			new MenuConnector(this, mi);
			menuItem = mi;
			return mi;
		}

		class MenuConnector
		{
			readonly RadioMenuItem menuItem;
			readonly RadioAction action;
			
			public MenuConnector(RadioAction action, RadioMenuItem menuItem)
			{
				this.action = action;
				this.menuItem = menuItem;
				this.menuItem.Click += menuItem_Click;
				this.action.EnabledChanged += new EventHandler<EventArgs>(action_EnabledChanged).MakeWeak(e => this.action.EnabledChanged -= e);
				this.action.CheckedChanged += new EventHandler<EventArgs>(action_CheckedChanged).MakeWeak(e => this.action.CheckedChanged -= e);
			}
			bool changing;
			
			void menuItem_Click(Object sender, EventArgs e)
			{
				if (!changing)
					action.OnActivated(e);
			}
			
			void action_CheckedChanged(Object sender, EventArgs e)
			{
				changing = true;
				menuItem.Checked = action.Checked;
				changing = false;
			}
			
			void action_EnabledChanged(Object sender, EventArgs e)
			{
				menuItem.Enabled = action.Enabled;
			}
		}		
	}
}
#endif
