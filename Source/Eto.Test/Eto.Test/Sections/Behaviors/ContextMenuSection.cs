using Eto.Forms;
using Eto.Drawing;
using System.Linq;

namespace Eto.Test.Sections.Behaviors
{
	[Section("Behaviors", "ContextMenu")]
	public class ContextMenuSection : Panel
	{
		public ContextMenuSection()
		{
			var layout = new DynamicLayout();
			
			layout.Add(null, null, true);
			
			layout.AddRow(null, ContextMenuPanel(), null);
			
			layout.Add(null, null, true);

			Content = layout;
		}

		ContextMenu CreateMenu()
		{
			var menu = new ContextMenu();
			menu.Items.Add(new ButtonMenuItem { Text = "Item 1" });
			menu.Items.Add(new ButtonMenuItem { Text = "Item 2" });
			menu.Items.Add(new ButtonMenuItem { Text = "Item 3" });
			menu.Items.Add(new ButtonMenuItem { Text = "Item 4" });
			var subMenu = menu.Items.GetSubmenu("Sub Menu");
			subMenu.Items.Add(new ButtonMenuItem { Text = "Item 5" });
			subMenu.Items.Add(new ButtonMenuItem { Text = "Item 6" });
			LogEvents(menu);
			return menu;
		}

		Control ContextMenuPanel()
		{
			var label = new Label
			{ 
				Size = new Size(100, 100), 
				BackgroundColor = Colors.Blue,
				TextColor = Colors.White,
				HorizontalAlign = HorizontalAlign.Center,
				VerticalAlign = VerticalAlign.Middle,
				Text = "Click on me!"
			};
			label.MouseDown += (sender, e) =>
			{
				var menu = CreateMenu();
				menu.Show(label);
			};
			return label;
		}

		void LogEvents(ISubmenu menu)
		{
			foreach (var item in menu.Items.OfType<ButtonMenuItem>())
			{
				LogClickEvents(item);
			}
		}

		void LogClickEvents(ButtonMenuItem item)
		{
			LogEvents(item);
			item.Click += delegate
			{
				Log.Write(item, "Click, Item: {0}", item.Text);
			};
		}
	}
}