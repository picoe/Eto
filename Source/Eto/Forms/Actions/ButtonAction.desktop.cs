using System;
using System.Reflection;
using System.Collections;
using Eto.Drawing;

namespace Eto.Forms
{
	
	public partial class ButtonAction : BaseAction
	{
		protected virtual MenuItem GenerateMenu(ActionItem actionItem, Menu menu)
		{
			ImageMenuItem mi = new ImageMenuItem(menu.Generator);
			mi.Text = MenuText;
			mi.Shortcut = Accelerator;
			mi.Enabled = this.Enabled;
			if (Icon != null) mi.Icon = Icon;
			mi.Click += new EventHandler<EventArgs>(delegate { this.Activate(); }).MakeWeak((e) => mi.Click -= e);
			this.EnabledChanged += new EventHandler<EventArgs>(delegate { mi.Enabled = this.Enabled; }).MakeWeak((e) => this.EnabledChanged -= e);
			//new MenuConnector(this, mi);
			return mi;
		}
		
		public override void Generate(ActionItem actionItem, Menu menu)
		{
			var mi = GenerateMenu(actionItem, menu);
			menu.MenuItems.Add(mi);
		}
		
		protected class MenuConnector
		{
			ImageMenuItem menuItem;
			ButtonAction action;
			
			public MenuConnector(ButtonAction action, ImageMenuItem menuItem)
			{
				this.action = action;
				this.menuItem = menuItem;
				this.menuItem.Click += menuItem_Clicked;
				this.action.EnabledChanged += new EventHandler<EventArgs>(action_EnabledChanged).MakeWeak((e) => this.action.EnabledChanged -= e);
			}
			
			void menuItem_Clicked(Object sender, EventArgs e)
			{
				action.OnActivated(e);
			}
			
			void action_EnabledChanged(Object sender, EventArgs e)
			{
				menuItem.Enabled = action.Enabled;
			}
		}
	}
}
