namespace Eto.Test.Sections.Behaviors
{
	[Section("Behaviors", "Control ContextMenu")]
	public class ControlContextMenuSection : AllControlsBase
	{
		
		public ControlContextMenuSection()
		{
			ContextMenu = CreateContextMenu(this);
		}
		protected override void LogEvents(Control control)
		{
			base.LogEvents(control);

			control.ContextMenu = CreateContextMenu(control);
		}

		private ContextMenu CreateContextMenu(Control control)
		{
			var menu = new ContextMenu();
			var item1 = new ButtonMenuItem { Text = "I&tem 1" };
			item1.Click += (sender, e) => Log.Write(control, "Clicked Item 1");
			menu.Items.Add(item1);
			var item2 = new ButtonMenuItem { Text = "Item &2" };
			item2.Click += (sender, e) => Log.Write(control, "Clicked Item 2");
			menu.Items.Add(item2);
			var subMenu = new SubMenuItem { Text = "Sub Menu" };
			var subItem1 = new ButtonMenuItem { Text = "Sub Item &1" };
			subItem1.Click += (sender, e) => Log.Write(control, "Clicked Sub Item 1");
			subMenu.Items.Add(subItem1);
			var subItem2 = new ButtonMenuItem { Text = "Sub Item &2" };
			subItem2.Click += (sender, e) => Log.Write(control, "Clicked Sub Item 2");
			subMenu.Items.Add(subItem2);
			menu.Items.Add(subMenu);
			
			menu.Opening += (sender, e) => Log.Write(control, "ContextMenu Opening");
			menu.Closing += (sender, e) => Log.Write(control, "ContextMenu Closing");
			menu.Closed += (sender, e) => Log.Write(control, "ContextMenu Closed");
			
			return menu;
		}
	}
}

