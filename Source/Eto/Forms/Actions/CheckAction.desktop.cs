#if DESKTOP
using System;
using Eto;

namespace Eto.Forms
{
	public partial class CheckAction
	{
		public override MenuItem GenerateMenuItem(Generator generator)
		{
			var mi = new CheckMenuItem(generator);
			mi.Text = MenuText;
			mi.Shortcut = Accelerator;
			mi.Enabled = Enabled;
			mi.Checked = Checked;
			if (!string.IsNullOrEmpty(MenuItemStyle))
				mi.Style = MenuItemStyle;

			new MenuConnector(this, mi);
			return mi;
		}

		#pragma warning disable 0618
		class MenuConnector
		{
			readonly CheckMenuItem menuItem;
			readonly CheckAction action;
			bool blah;
			
			public MenuConnector(CheckAction action, CheckMenuItem menuItem)
			{
				this.action = action;
				this.menuItem = menuItem;
				this.menuItem.Click += menuItem_Clicked;
				this.action.EnabledChanged += new EventHandler<EventArgs>(action_EnabledChanged).MakeWeak(e => this.action.EnabledChanged -= e);
				this.action.CheckedChanged += new EventHandler<EventArgs>(action_CheckedChanged).MakeWeak(e => this.action.CheckedChanged -= e);
			}
			
			void menuItem_Clicked(Object sender, EventArgs e)
			{
				if (!blah) action.OnActivated(e);
			}
			
			void action_CheckedChanged(Object sender, EventArgs e)
			{
				blah = true;
				menuItem.Checked = action.Checked;
				blah = false;
			}
			
			void action_EnabledChanged(Object sender, EventArgs e)
			{
				menuItem.Enabled = action.Enabled;
			}
		}
		#pragma warning restore 0618
		
	}
}

#endif
