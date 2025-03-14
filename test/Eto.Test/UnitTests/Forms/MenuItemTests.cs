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

			Assert.That(disabledWasClicked, Is.False, "#1 - Disabled item should not be clickable");
			Assert.That(validateWasCalled, Is.GreaterThan(0), "#2 - Validate was never called!");
		}
		
		[Test, InvokeOnUI]
		public void MenuItemEnabledShouldUpdateCommandIfSpecified()
		{
			var command = new Command { MenuText = "Hello" };
			Assert.That(command.Enabled, Is.True, "#1");
			
			var item = new ButtonMenuItem(command);
			Assert.That(command.MenuText, Is.EqualTo(item.Text), "#2.1");
			Assert.That(command.Enabled, Is.EqualTo(item.Enabled), "#2.2");
			Assert.That(item.Enabled, Is.True, "#2.3");
			
			item.Enabled = false;
			
			Assert.That(command.Enabled, Is.False, "#3.1");
			Assert.That(command.Enabled, Is.EqualTo(item.Enabled), "#3.2");
		}

		[TestCase(true, Keys.E, false, true)] // Fails in Gtk, Wpf (shortcut takes precedence over intrinsic behaviour)
		[TestCase(false, Keys.E, false, true)]
		[TestCase(true, Keys.E | Keys.Control, false, false)] // Fails in Gtk, Mac (menu shortcuts always takes precedence)
		[TestCase(true, Keys.E | Keys.Control, true, false)] // Fails in Gtk, Mac (menu shortcuts always takes precedence)
		[TestCase(false, Keys.E | Keys.Control, true, false)]
		[TestCase(false, Keys.E | Keys.Control, false, false)]
		[ManualTest]
		public void TextBoxShouldAcceptInputEvenWhenShortcutDefined(bool enabled, Keys key, bool handleKey, bool shouldBeIntrinsic)
		{
			ControlShouldAcceptInputEvenWhenShortcutDefined<TextBox>(enabled, key, handleKey, shouldBeIntrinsic);
		}

		[TestCase(true, Keys.E, false, false)]
		[TestCase(false, Keys.E, false, false)]
		[TestCase(true, Keys.E, true, false)]
		[TestCase(false, Keys.E, true, false)]
		[TestCase(true, Keys.E | Keys.Control, false, false)] // Fails in Gtk, Mac (menu shortcuts always takes precedence)
		[TestCase(true, Keys.E | Keys.Control, true, false)] // Fails in Gtk, Mac (menu shortcuts always takes precedence)
		[TestCase(false, Keys.E | Keys.Control, true, false)]
		[TestCase(false, Keys.E | Keys.Control, false, false)]
		[ManualTest]
		public void DrawableShouldAcceptInputEvenWhenShortcutDefined(bool enabled, Keys key, bool handleKey, bool shouldBeIntrinsic)
		{
			ControlShouldAcceptInputEvenWhenShortcutDefined<Drawable>(enabled, key, handleKey, shouldBeIntrinsic, d =>
			{
				d.CanFocus = true;
				d.BackgroundColor = Colors.Blue;
				d.KeyDown += (sender, e) =>
				{
					if (e.KeyData == key)
					{
						d.BackgroundColor = Colors.Green;
					}
				};
				d.KeyUp += (sender, e) =>
				{
					if (e.KeyData == key)
					{
						d.BackgroundColor = Colors.Blue;
					}
				};
			});
		}
		
		void ControlShouldAcceptInputEvenWhenShortcutDefined<T>(bool enabled, Keys key, bool handleKey, bool shouldBeIntrinsic, Action<T> initialize = null)
			where T: Control, new()
		{
			var itemWasClicked = false;
			var keyWasPressed = false;
			ManualForm($"Press the {key.ToShortcutString()} key", form =>
			{
				var menu = new MenuBar();
				var item = new ButtonMenuItem { Text = "Disabled Item", Enabled = enabled, Shortcut = key };
				item.Click += (sender, e) =>
				{
					itemWasClicked = true;
					Log.Write(sender, "Item was clicked");
				};
				menu.Items.Add(new SubMenuItem { Text = "File", Items = { item } });
				form.Menu = menu;
				var control = new T { Size = new Size(200, 200) };
				initialize?.Invoke(control);
				control.KeyDown += (sender, e) =>
				{
					if (e.KeyData == key)
					{
						// key was pressed! yay.
						Log.Write(sender, "Key was pressed on control");
						keyWasPressed = true;
						
						if (handleKey)
							e.Handled = true;
					}
				};
				form.Shown += (sender, e) => control.Focus();

				return control;
			});

			if (!enabled || handleKey || shouldBeIntrinsic)
			{
				Assert.That(itemWasClicked, Is.False, "#1 - ButtonMenuItem was triggered, but should not have been");
			}
			else
			{
				Assert.That(itemWasClicked, Is.True, "#1 - ButtonMenuItem was not triggered");
			}

			Assert.That(keyWasPressed, Is.True, "#2 - Key was not pressed");
		}

	}
}