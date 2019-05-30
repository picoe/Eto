using Eto.Forms;
using Eto.Drawing;
using System;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(Label))]
	public class LabelSection : Panel
	{
		public LabelSection()
		{
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };

			layout.Add(NormalLabel());
			layout.Add(FontLabel());
			layout.Add(WrapLabel());
			layout.AddSeparateRow(null, UnderlineLabel(), HotkeyLabel(), HotkeyUnderlineLabel(), null);
			layout.AddSeparateRow(null, ColorLabel(), BackgroundColorLabel(), null);
			layout.Add(CenterLabel());
			layout.Add(RightLabel());
			layout.Add(MiddleLabel(), yscale: true);
			layout.Add(BottomLabel(), yscale: true);

			Content = layout;
		}

		Control NormalLabel()
		{
			return new Label
			{
				Text = "Normal Label"
			};
		}

		Control FontLabel()
		{
			return new Label
			{
				Text = "Font Label",
				Font = Fonts.Sans(14, FontStyle.Bold),
			};
		}

		Control CenterLabel()
		{
			return new Label
			{
				Text = "Center Align",
				TextAlignment = TextAlignment.Center
			};
		}

		Control RightLabel()
		{
			return new Label
			{
				Text = "Right Align",
				TextAlignment = TextAlignment.Right
			};
		}

		Control MiddleLabel()
		{
			return new Label
			{
				Text = "Middle Center Align",
				TextAlignment = TextAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				BackgroundColor = Colors.AliceBlue
			};
		}

		Control BottomLabel()
		{
			return new Label
			{
				Text = "Bottom Center Align",
				TextAlignment = TextAlignment.Center,
				VerticalAlignment = VerticalAlignment.Bottom,
				BackgroundColor = Colors.AliceBlue
			};
		}

		Control ColorLabel()
		{
			return new Label
			{
				Text = "Custom Color",
				TextColor = Color.FromArgb(unchecked((int)0xFF00a000))
			};
		}

		Control BackgroundColorLabel()
		{
			return new Label
			{
				Text = "Custom Background Color",
				BackgroundColor = Colors.Green,
			};
		}

		Control UnderlineLabel()
		{
			return new Label
			{
				Text = "Underlined Text",
				Font = SystemFonts.Label(decoration: FontDecoration.Underline)
			};
		}

		Control HotkeyLabel()
		{
			return new Label
			{
				Text = "Some &Hotkey"
			};
		}

		Control HotkeyUnderlineLabel()
		{
			return new Label
			{
				Text = "Some Underlined &Hotkey",
				Font = SystemFonts.Label(decoration: FontDecoration.Underline)
			};
		}

		Control WrapLabel()
		{
			const string text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
			var label = new Label
			{
				Text = text
			};

			var wrapDropDown = new EnumDropDown<WrapMode>();
			wrapDropDown.SelectedValueBinding.Bind(label, l => l.Wrap);

			var textAlignmentDropDown = new EnumDropDown<TextAlignment>();
			textAlignmentDropDown.SelectedValueBinding.Bind(label, l => l.TextAlignment);

			var verticalAlignmentDropDown = new EnumDropDown<VerticalAlignment>();
			verticalAlignmentDropDown.SelectedValueBinding.Bind(label, l => l.VerticalAlignment);

			var testVerticalAlignment = new CheckBox { Text = "Test VerticalAlignment" };
			testVerticalAlignment.CheckedChanged += (sender, e) => label.Size = new Size(-1, testVerticalAlignment.Checked == true ? 200 : -1);
			testVerticalAlignment.CheckedBinding.Bind(verticalAlignmentDropDown, c => c.Enabled, DualBindingMode.OneWayToSource);

			var fontSelector = new FontPicker();
			fontSelector.Bind(c => c.Value, label, l => l.Font);

			Func<Control> spacer = () => new Panel { BackgroundColor = Colors.DarkGray, Size = new Size(10, 10) };

			return new StackLayout
			{
				HorizontalContentAlignment = HorizontalAlignment.Stretch,
				Items =
				{
					TableLayout.Horizontal(5, null, "Wrap:", wrapDropDown, "Font:", fontSelector, null),
					TableLayout.Horizontal(5, null, testVerticalAlignment, verticalAlignmentDropDown, "TextAlignment:", textAlignmentDropDown, null),
					spacer(),
					new TableLayout(
						new TableRow(
							spacer(),
							new TableCell(label, true),
							spacer()
						)
					),
					spacer()
				}
			};
		}
	}
}

