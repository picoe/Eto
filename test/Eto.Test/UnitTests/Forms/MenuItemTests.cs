using System;
using Eto.Drawing;
using Eto.Forms;
using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms
{
	public class MenuItemTests : TestBase
	{
		[Test, ManualTest]
		public void DynamicallyAddedItemsShouldValidateAndDisable()
		{
			bool disabledWasClicked = false;
			int validateWasCalled = 0;

			void AddMenuItems(ISubmenu submenu, Window window, int initialItems = 0)
			{
				if (submenu.Items.Count > initialItems)
					return;
					
				var commandButton = new Command { MenuText = "A Command Item" };
				commandButton.Executed += (sender, e) => Log.Write(sender, $"{commandButton.MenuText} was clicked");

				var disabledCommandButton = new Command { MenuText = "A Disabled command Item", Enabled = false };
				disabledCommandButton.Executed += (sender, e) => {
					Log.Write(sender, $"{disabledCommandButton.MenuText} was clicked");
				};

				var enabledButton = new ButtonMenuItem { Text = "An Enabled Button" };
				enabledButton.Validate += (sender, e) =>
				{
					validateWasCalled++;
					Log.Write(sender, "Validate was called!");
				};
				enabledButton.Click += (s2, e2) => Log.Write(s2, $"{enabledButton.Text} was clicked");

				var disabledButton = new ButtonMenuItem { Text = "Disabled button", Enabled = false };
				disabledButton.Click += (sender, e) =>
				{
					Log.Write(sender, $"{disabledButton.Text} was clicked");
					disabledWasClicked = true;
					window.Close();
				};

				var toggledButton = new ButtonMenuItem { Text = "Toggled button" };
				toggledButton.Click += (s2, e2) => Log.Write(s2, $"{toggledButton.Text} was clicked");

				var checkButton = new CheckMenuItem { Text = "toggled button enabled", Checked = true };
				toggledButton.Bind(c => c.Enabled, checkButton, c => c.Checked);
				checkButton.Click += (s2, e2) => Log.Write(s2, $"{checkButton.Text} was clicked");


				submenu.Items.Add(enabledButton);
				submenu.Items.Add(disabledButton);
				submenu.Items.AddSeparator();
				submenu.Items.Add(toggledButton);
				submenu.Items.Add(checkButton);
				submenu.Items.AddSeparator();
				submenu.Items.Add(commandButton);
				submenu.Items.Add(disabledCommandButton);
			}

			ManualForm("Ensure the items in the\nFile menu are correct", form =>
			{
				var sub = new SubMenuItem { Text = "A Child Menu" };
				sub.Opening += (sender, e) => AddMenuItems((ISubmenu)sender, form);

				var file = new SubMenuItem { Text = "&File" };
				file.Opening += (sender, e) => AddMenuItems((ISubmenu)sender, form, 3);
				file.Items.Add(sub);

				var menu = new MenuBar();
				menu.Items.Add(file);
				
				var showContextMenuButton = new Button { Text = "Show ContextMenu" };
				showContextMenuButton.Click += (sender, e) => {
					var contextMenu = new ContextMenu();
					
					var subMenuItem = new SubMenuItem { Text = "A Child Menu" };
					subMenuItem.Opening += (s2, e2) => AddMenuItems((ISubmenu)s2, form);
					contextMenu.Items.Add(subMenuItem);
					
					contextMenu.Opening += (s2, e2) => AddMenuItems((ISubmenu)s2, form, 3);
					contextMenu.Show();
				};

				form.Menu = menu;
				return new Panel { Size = new Size(200, 50), Content = TableLayout.AutoSized(showContextMenuButton, centered: true) };
			});

			Assert.IsFalse(disabledWasClicked, "#1 - Disabled item should not be clickable");
			Assert.Greater(validateWasCalled, 0, "#2 - Validate was never called!");
		}
		
		[Test, InvokeOnUI]
		public void MenuItemEnabledShouldUpdateCommandIfSpecified()
		{
			var command = new Command { MenuText = "Hello" };
			Assert.IsTrue(command.Enabled, "#1");
			
			var item = new ButtonMenuItem(command);
			Assert.AreEqual(item.Text, command.MenuText, "#2.1");
			Assert.AreEqual(item.Enabled, command.Enabled, "#2.2");
			Assert.IsTrue(item.Enabled, "#2.3");
			
			item.Enabled = false;
			
			Assert.IsFalse(command.Enabled, "#3.1");
			Assert.AreEqual(item.Enabled, command.Enabled, "#3.2");
		}
		
	}
}