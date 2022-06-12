using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Sections.Behaviors
{
	[Section("Behaviors", typeof(ToolBar))]
	public class ToolBarSection : DynamicLayout
	{
		public ToolBarSection()
		{
			var showDialogButton = new Button { Text = "Show Test Dialog" };
			showDialogButton.Click += (sender, e) => ShowTestDialog();
			AddCentered(showDialogButton, verticalCenter: true);
		}

		private void ShowTestDialog()
		{
			int count = 0;
			var toolBar = new ToolBar();
			var dlg = new Dialog
			{
				ClientSize = new Size(400, 300),
				Resizable = true,
				ToolBar = toolBar
			};

			dlg.Styles.Add<Label>(null, l => l.VerticalAlignment = VerticalAlignment.Center);

			var indexStepper = new NumericStepper { MinValue = -1, MaxValue = -1, Value = -1 };
			var typeDropDown = new DropDown
			{
				Items = { "Button", "Radio", "Check", "Separator:Divider", "Separator:Space", "Separator:FlexableSpace" },
				SelectedIndex = 0
			};

			var withImageCheck = new CheckBox { Text = "With Image", Checked = true };

			Image GetImage()
			{
				if (withImageCheck.Checked == true)
					return TestIcons.TestIcon;
				return null;
			}


			void SetStepperLimit()
			{
				if (toolBar.Items.Count == 0)
					indexStepper.MinValue = indexStepper.MaxValue = -1;
				else
				{
					indexStepper.MinValue = 0;
					indexStepper.MaxValue = toolBar.Items.Count - 1;
				}
			}

			ToolItem CreateItem()
			{
				switch (typeDropDown.SelectedKey?.ToLowerInvariant())
				{
					default:
					case "button":
						return new ButtonToolItem { Text = $"Button{++count}", Image = GetImage() };
					case "radio":
						return new RadioToolItem { Text = $"Radio{++count}", Image = GetImage() };
					case "check":
						return new CheckToolItem { Text = $"Check{++count}", Image = GetImage() };
					case "separator:divider":
						return new SeparatorToolItem { Type = SeparatorToolItemType.Divider };
					case "separator:space":
						return new SeparatorToolItem { Type = SeparatorToolItemType.Space };
					case "separator:flexablespace":
						return new SeparatorToolItem { Type = SeparatorToolItemType.FlexibleSpace };
				}
			}

			var addButton = new Button { Text = "Add" };
			addButton.Click += (sender, e) =>
			{
				toolBar.Items.Add(CreateItem());
				SetStepperLimit();
			};

			var removeButton = new Button { Text = "Remove" };
			removeButton.Click += (sender, e) =>
			{
				var index = (int)indexStepper.Value;
				if (index >= 0)
					toolBar.Items.RemoveAt(index);
				SetStepperLimit();
			};

			var insertButton = new Button { Text = "Insert" };
			insertButton.Click += (sender, e) =>
			{
				var index = (int)indexStepper.Value;
				if (index >= 0)
					toolBar.Items.Insert(index, CreateItem());
				SetStepperLimit();
			};

			var clearButton = new Button { Text = "Clear" };
			clearButton.Click += (sender, e) =>
			{
				toolBar.Items.Clear();
				SetStepperLimit();
			};

			var layout = new DynamicLayout();

			layout.BeginCentered(yscale: true);
			layout.AddSeparateRow(null, "Type:", typeDropDown, withImageCheck, null);
			layout.AddSeparateRow(null, addButton, insertButton, removeButton, "Index:", indexStepper, null);
			layout.AddSeparateRow(null, clearButton, null);
			layout.EndCentered();

			dlg.Content = layout;


			dlg.ShowModal();
		}
	}
}
