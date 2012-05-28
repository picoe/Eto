using System;
using System.Collections;
using Eto.Drawing;
using Eto;

namespace Eto.Forms
{
	public partial class CheckAction : BaseAction
	{
		public override MenuItem Generate(ActionItem actionItem, ISubMenuWidget menu)
		{
			CheckMenuItem mi = new CheckMenuItem(menu.Generator);
			mi.Text = MenuText;
			mi.Shortcut = Accelerator;
			mi.Enabled = this.Enabled;
			mi.Checked = Checked;
			if (!string.IsNullOrEmpty (MenuItemStyle))
				mi.Style = MenuItemStyle;

			new MenuConnector(this, mi);
			return mi;
		}

		class MenuConnector
		{
			CheckMenuItem menuItem;
			CheckAction action;
			bool blah;
			
			public MenuConnector(CheckAction action, CheckMenuItem menuItem)
			{
				this.action = action;
				this.menuItem = menuItem;
				this.menuItem.Click += menuItem_Clicked;
				this.action.EnabledChanged += new EventHandler<EventArgs>(action_EnabledChanged).MakeWeak(e => this.action.EnabledChanged -= e);
				this.action.CheckedChanged += new EventHandler<EventArgs>(action_CheckedChanged).MakeWeak(e => this.action.CheckedChanged -= e);
			}
			
			private void menuItem_Clicked(Object sender, EventArgs e)
			{
				if (!blah) action.OnActivated(e);
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
