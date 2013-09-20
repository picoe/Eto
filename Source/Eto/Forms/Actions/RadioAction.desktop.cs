#if DESKTOP
using System;
using Eto.Drawing;

namespace Eto.Forms
{
	public partial class RadioAction : BaseAction
	{
		RadioMenuItem menuItem;

		public override MenuItem GenerateMenuItem(Generator generator)
		{
			RadioMenuItem mi = new RadioMenuItem(generator, (Controller != null) ? Controller.menuItem : null);
			mi.Text = MenuText;
			mi.Shortcut = Accelerator;
			mi.Enabled = this.Enabled;
			mi.Checked = Checked;
			if (!string.IsNullOrEmpty(MenuItemStyle))
				mi.Style = MenuItemStyle;

			new MenuConnector(this, mi);
			menuItem = mi;
			return mi;
		}

		class MenuConnector
		{
			RadioMenuItem menuItem;
			RadioAction action;
			
			public MenuConnector(RadioAction action, RadioMenuItem menuItem)
			{
				this.action = action;
				this.menuItem = menuItem;
				this.menuItem.Click += menuItem_Click;
				this.action.EnabledChanged += new EventHandler<EventArgs>(action_EnabledChanged).MakeWeak(e => this.action.EnabledChanged -= e);
				this.action.CheckedChanged += new EventHandler<EventArgs>(action_CheckedChanged).MakeWeak(e => this.action.CheckedChanged -= e);
			}
			private bool blah = false;
			
			private void menuItem_Click(Object sender, EventArgs e)
			{
				if (!blah)
				action.OnActivated(e);
			}
			
			private void action_CheckedChanged(Object sender, EventArgs e)
			{
				blah = true;
				menuItem.Checked = action.Checked;
				blah = false;
			}
			
			private void action_EnabledChanged(Object sender, EventArgs e)
			{
				menuItem.Enabled = action.Enabled;
			}
		}		
	}
}
#endif