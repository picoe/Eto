using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(TextBox))]
	public class TextBoxSection : Scrollable
	{
		public TextBoxSection()
		{
			var textBox = new TextBox();
			LogEvents(textBox);

			var placeholderText = new TextBox();
			placeholderText.TextBinding.Bind(textBox, c => c.PlaceholderText);

			var setTextButton = new Button { Text = "Set Text" };
			setTextButton.Click += (sender, e) => textBox.Text = "Some Text";

			var selectAllButton = new Button { Text = "SelectAll" };
			selectAllButton.Click += (sender, e) => textBox.SelectAll();

			var enabledCheckBox = new CheckBox { Text = "Enabled" };
			enabledCheckBox.CheckedBinding.Bind(textBox, c => c.Enabled);

			var readOnlyCheckBox = new CheckBox { Text = "ReadOnly" };
			readOnlyCheckBox.CheckedBinding.Bind(textBox, c => c.ReadOnly);

			var alignmentDropDown = new EnumDropDown<TextAlignment>();
			alignmentDropDown.SelectedValueBinding.Bind(textBox, c => c.TextAlignment);

			var showBorderCheckBox = new CheckBox { Text = "ShowBorder" };
			showBorderCheckBox.CheckedBinding.Bind(textBox, c => c.ShowBorder);

			var maxLengthStepper = new NumericStepper { MinValue = 0 };
			maxLengthStepper.ValueBinding.Bind(textBox, c => c.MaxLength);

			var selectionMode = new EnumDropDown<AutoSelectMode>();
			selectionMode.SelectedValueBinding.Bind(textBox, c => c.AutoSelectMode);

			var layout = new DynamicLayout { Padding = 10, DefaultSpacing = new Size(5, 5) };
			layout.AddSeparateRow(null, enabledCheckBox, readOnlyCheckBox, showBorderCheckBox, null);
			layout.AddSeparateRow(null, "TextAlignment", alignmentDropDown, "SelectionMode", selectionMode, null);
			layout.AddSeparateRow(null, "MaxLength", maxLengthStepper, "PlaceholderText", placeholderText, null);
			layout.AddSeparateRow(null, setTextButton, selectAllButton, null);
			layout.Add(null);
			layout.AddCentered(textBox);
			layout.AddCentered(DifferentSize());
			layout.Add(null);

			Content = layout;
		}

		Control DifferentSize()
		{
			var control = new TextBox { Text = "Different Size (300x50)", Size = new Size(300, 50) };
			LogEvents(control);
			return control;
		}

		void LogEvents(TextBox control)
		{
			control.TextChanging += (sender, e) => Log.Write(control, $"TextChanging, Range: {e.Range}, Text: {e.Text}");
			control.TextChanged += (sender, e) => Log.Write(control, $"TextChanged, Text: {control.Text}, Selection: {control.Selection}");
			control.TextInput += (sender, e) => Log.Write(control, "TextInput: {0}", e.Text);
			control.KeyDown += (sender, e) =>
			{
				if (e.KeyData == (Keys.Slash | Keys.Shift))
				{
					e.Handled = true;
					Log.Write(control, $"Selection: {control.Selection}");
				}
			};
		}
	}
}

