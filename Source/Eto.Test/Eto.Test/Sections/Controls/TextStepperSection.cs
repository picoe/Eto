using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(TextStepper))]
	public class TextStepperSection : Scrollable
	{
		public TextStepperSection()
		{
			var textUpDown = new TextStepper();
			LogEvents(textUpDown);

			var placeholderText = new TextBox();
			placeholderText.TextBinding.Bind(textUpDown, c => c.PlaceholderText);

			var enabledCheckBox = new CheckBox { Text = "Enabled" };
			enabledCheckBox.CheckedBinding.Bind(textUpDown, c => c.Enabled);

			var readOnlyCheckBox = new CheckBox { Text = "ReadOnly" };
			readOnlyCheckBox.CheckedBinding.Bind(textUpDown, c => c.ReadOnly);

			var setTextButton = new Button { Text = "Set Text" };
			setTextButton.Click += (sender, e) => textUpDown.Text = "Some text";

			var selectAllButton = new Button { Text = "SelectAll" };
			selectAllButton.Click += (sender, e) => textUpDown.SelectAll();

			var validDirectionsDropDown = new EnumDropDown<StepperValidDirections>();
			validDirectionsDropDown.SelectedValueBinding.Bind(textUpDown, c => c.ValidDirection);

			var alignmentDropDown = new EnumDropDown<TextAlignment>();
			alignmentDropDown.SelectedValueBinding.Bind(textUpDown, c => c.TextAlignment);

			var showStepperCheckBox = new CheckBox { Text = "ShowStepper" };
			showStepperCheckBox.CheckedBinding.Bind(textUpDown, c => c.ShowStepper);

			var showBorderCheckBox = new CheckBox { Text = "ShowBorder" };
			showBorderCheckBox.CheckedBinding.Bind(textUpDown, c => c.ShowBorder);

			var maxLengthStepper = new NumericStepper { MinValue = 0 };
			maxLengthStepper.ValueBinding.Bind(textUpDown, c => c.MaxLength);

			var layout = new DynamicLayout { Padding = 10, DefaultSpacing = new Size(5, 5) };
			layout.AddSeparateRow(null, enabledCheckBox, readOnlyCheckBox, showStepperCheckBox, showBorderCheckBox, null);
			layout.AddSeparateRow(null, "TextAlignment", alignmentDropDown, "MaxLength", maxLengthStepper, null);
			layout.AddSeparateRow(null, "PlaceholderText", placeholderText, null);
			layout.AddSeparateRow(null, setTextButton, selectAllButton, null);
			layout.AddSeparateRow(null, "ValidDirection", validDirectionsDropDown, null);
			layout.Add(null);
			layout.AddCentered(textUpDown);
			layout.AddCentered(new TextStepper { Text = "Different Size (300x-1)", Size = new Size(300, -1) });
			layout.Add(null);

			Content = layout;
		}

		void LogEvents(TextStepper textUpDown)
		{
			textUpDown.Step += (sender, e) => Log.Write(textUpDown, $"Step: {e.Direction}");
			textUpDown.TextChanging += (sender, e) => Log.Write(textUpDown, $"TextChanging: {e.Text}, Current: {textUpDown.Text}");
			textUpDown.TextChanged += (sender, e) => Log.Write(textUpDown, $"TextChanged: {textUpDown.Text}");
}
	}
}
