namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(SegmentedButton))]
	public class SegmentedButtonSection : DynamicLayout
	{
		public SegmentedButtonSection()
		{
			var checkCommand = new CheckCommand
			{
				ToolBarText = "CheckCommand"
			};
			checkCommand.CheckedChanged += (sender, e) => Log.Write(sender, $"CheckedChanged: {checkCommand.Checked}");
			checkCommand.Executed += (sender, e) => Log.Write(sender, "Executed");

			var segbutton = new SegmentedButton();

			segbutton.Items.Add(new ButtonSegmentedItem { Image = TestIcons.TestIcon.WithSize(16, 16) });
			segbutton.Items.Add(new ButtonSegmentedItem { Text = "Some Text", Image = TestIcons.TestImage.WithSize(16, 16) });
			segbutton.Items.Add(new ButtonSegmentedItem(checkCommand));
			segbutton.Items.Add(new ButtonSegmentedItem { Text = "Width=150", Width = 150 });
			segbutton.Items.Add(new MenuSegmentedItem
			{
				Text = "Selectable Menu",
				CanSelect = true,
				Menu = new ContextMenu
				{
					Items =
					{
						CreateMenuItem("Menu Item 1"),
						CreateMenuItem("Menu Item 2")
					}
				}
			});
			segbutton.Items.Add(new MenuSegmentedItem
			{
				Text = "Menu Only",
				Image = TestIcons.TestImage.WithSize(16, 16),
				CanSelect = false,
				Menu = new ContextMenu
				{
					Items =
					{
						CreateMenuItem("Menu Item 1"),
						CreateMenuItem("Menu Item 2")
					}
				}
			});

			LogEvents(segbutton);


			var selectionModeDropDown = new EnumDropDown<SegmentedSelectionMode>();
			selectionModeDropDown.SelectedValueBinding.Bind(segbutton, b => b.SelectionMode);

			var selectAllButton = new Button { Text = "SelectAll" };
			selectAllButton.Click += (sender, e) => segbutton.SelectAll();
			//selectAllButton.Bind(c => c.Enabled, selectionModeDropDown.SelectedValueBinding.Convert(r => r == SegmentedSelectionMode.Multiple));

			var clearSelectionButton = new Button { Text = "ClearSelection" };
			clearSelectionButton.Click += (sender, e) => segbutton.ClearSelection();
			clearSelectionButton.Bind(c => c.Enabled, selectionModeDropDown.SelectedValueBinding.Convert(r => r != SegmentedSelectionMode.None));

			var checkCommandEnabled = new CheckBox { Text = "CheckCommand.Enabled" };
			checkCommandEnabled.Bind(c => c.Checked, checkCommand, c => c.Enabled);

			var backgroundColorPicker = new ColorPicker { AllowAlpha = true };
			backgroundColorPicker.ValueBinding.Bind(segbutton, b => b.BackgroundColor);

			// layout
			Padding = 10;
			DefaultSpacing = new Size(5, 5);

			BeginCentered();
			AddSeparateRow("SelectionMode:", selectionModeDropDown, null);
			AddSeparateRow(selectAllButton, clearSelectionButton, null);
			AddSeparateRow(checkCommandEnabled, null);
			AddSeparateRow("BackgroundColor", backgroundColorPicker, null);
			EndCentered();

			AddSeparateRow(null, segbutton, null);
			AddSpace();
		}

		void LogEvents(SegmentedButton button)
		{
			button.SelectedIndexesChanged += (sender, e) => Log.Write(sender, $"SelectedIndexesChanged: {string.Join(", ", button.SelectedIndexes.Select(r => r.ToString()))}");
			button.SelectedItemsChanged += (sender, e) => Log.Write(sender, $"SelectedItemsChanged: {string.Join(", ", button.SelectedItems.Select(ItemDesc))}");
			button.ItemClick += (sender, e) => Log.Write(sender, $"ItemClick: {ItemDesc(e.Item)}, Index: {e.Index}");
			button.SelectedIndexChanged += (sender, e) => Log.Write(sender, $"SelectedIndexChanged: {button.SelectedIndex}");
			button.SelectedItemChanged += (sender, e) => Log.Write(sender, $"SelectedItemChanged: {ItemDesc(button.SelectedItem)}");

			foreach (var item in button.Items)
			{
				LogEvents(item);
			}
		}

		void LogEvents(SegmentedItem item)
		{
			item.Click += (sender, e) => Log.Write(sender, $"Click: {ItemDesc(item)}");
		}

		MenuItem CreateMenuItem(string text)
		{
			var item = new ButtonMenuItem { Text = text };
			item.Click += (sender, e) => Log.Write(sender, $"Click, {text}");
			return item;
		}

		string ItemDesc(SegmentedItem item) => item != null ? $"{item.GetType().Name}, {item.Text}" : "(null)";
	}
}
