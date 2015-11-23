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

		ContextMenu menu;

		ContextMenu CreateMenu()
		{
			if (menu != null)
				return menu;
			
			menu = new ContextMenu();

			menu.Opening += (sender, e) => Log.Write(sender, "Opening");
			menu.Closed += (sender, e) => Log.Write(sender, "Closed");

			menu.Items.Add(new ButtonMenuItem { Text = "Item 1" });
			menu.Items.Add(new ButtonMenuItem { Text = "Item 2", Shortcut = Keys.Control | Keys.I });
			menu.Items.Add(new ButtonMenuItem { Text = "Item 3", Shortcut = Keys.Shift | Keys.I });
			menu.Items.Add(new ButtonMenuItem { Text = "Item 4", Shortcut = Keys.Alt | Keys.I });

			var subMenu = menu.Items.GetSubmenu("Sub Menu");
			subMenu.Items.Add(new ButtonMenuItem { Text = "Item 5", Shortcut = Keys.Application | Keys.I });
			subMenu.Items.Add(new ButtonMenuItem { Text = "Item 6", Shortcut = Keys.I });

			menu.Items.AddSeparator();
			RadioMenuItem radioController;
			menu.Items.Add(radioController = new RadioMenuItem { Text = "Radio 1" });
			menu.Items.Add(new RadioMenuItem(radioController) { Text = "Radio 2", Checked = true });
			menu.Items.Add(new RadioMenuItem(radioController) { Text = "Radio 3", Shortcut = Keys.R });
			menu.Items.Add(new RadioMenuItem(radioController) { Text = "Radio 4" });

			menu.Items.AddSeparator();
			menu.Items.Add(new CheckMenuItem { Text = "Check 1" });
			menu.Items.Add(new CheckMenuItem { Text = "Check 2", Shortcut = Keys.Control | Keys.Alt | Keys.G, Checked = true });
			menu.Items.Add(new CheckMenuItem { Text = "Check 3", Shortcut = Keys.Control | Keys.Shift | Keys.G });
			menu.Items.Add(new CheckMenuItem { Text = "Check 4", Shortcut = Keys.Control | Keys.Application | Keys.G });
			menu.Items.Add(new CheckMenuItem { Text = "Check 5", Shortcut = Keys.Shift | Keys.Alt | Keys.G });
			menu.Items.Add(new CheckMenuItem { Text = "Check 6", Shortcut = Keys.Shift | Keys.Application | Keys.G });
			menu.Items.Add(new CheckMenuItem { Text = "Check 7", Shortcut = Keys.Alt | Keys.Application | Keys.G });

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
				TextAlignment = TextAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
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
			if (menu == null)
				return;
			foreach (var item in menu.Items)
			{
				LogClickEvents(item);
			}
		}

		void LogEvents(CheckMenuItem item)
		{
			if (item == null)
				return;
			item.CheckedChanged += delegate
			{
				Log.Write(item, "CheckedChanged, Item: {0}, Checked: {1}", item.Text, item.Checked);
			};
		}

		void LogEvents(RadioMenuItem item)
		{
			if (item == null)
				return;
			item.CheckedChanged += delegate
			{
				Log.Write(item, "CheckedChanged, Item: {0}, Checked: {1}", item.Text, item.Checked);
			};
		}

		void LogClickEvents(MenuItem item)
		{
			LogEvents(item as ISubmenu);
			LogEvents(item as CheckMenuItem);
			LogEvents(item as RadioMenuItem);
			item.Click += delegate
			{
				Log.Write(item, "Click, Item: {0}", item.Text);
			};
		}
	}
}