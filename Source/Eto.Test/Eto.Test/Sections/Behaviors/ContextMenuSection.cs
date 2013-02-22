#if DESKTOP
using System;
using Eto.Forms;
using Eto.Drawing;
using System.Linq;

namespace Eto.Test.Sections.Behaviors
{
	public class ContextMenuSection : Panel
	{
		public ContextMenuSection ()
		{
			var layout = new DynamicLayout(this);
			
			layout.Add (null, null, true);
			
			layout.AddRow (null, ContextMenuPanel(), null);
			
			layout.Add (null, null, true);
		}
		
		ContextMenu CreateMenu()
		{
			var menu = new ContextMenu();
			menu.MenuItems.Add (new ImageMenuItem{ Text = "Item 1" });
			menu.MenuItems.Add (new ImageMenuItem{ Text = "Item 2" });
			menu.MenuItems.Add (new ImageMenuItem{ Text = "Item 3" });
			menu.MenuItems.Add (new ImageMenuItem{ Text = "Item 4" });
			LogEvents (menu);
			return menu;
		}
		
		Control ContextMenuPanel ()
		{
			var label = new Label{ 
				Size = new Size(100, 100), 
				BackgroundColor = Colors.Blue,
				TextColor = Colors.White,
				HorizontalAlign = HorizontalAlign.Center,
				VerticalAlign = VerticalAlign.Middle,
				Text = "Click on me!"
			};
			
			label.MouseDown += delegate(object sender, MouseEventArgs e) {
				var menu = CreateMenu ();
				menu.Show (label);
			};
			return label;
		}
		
		void LogEvents(ContextMenu menu)
		{
			foreach (var item in menu.MenuItems.OfType<ImageMenuItem>()) {
				LogEvents (item);
			}
		}
		
		void LogEvents(ImageMenuItem item)
		{
			item.Click += delegate {
				Log.Write (item, "Click, Item: {0}", item.Text);
			};
		}
	}
}

#endif