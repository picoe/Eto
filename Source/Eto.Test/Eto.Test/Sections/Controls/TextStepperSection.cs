using System;
using Eto.Forms;
namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(TextStepper))]
	public class TextStepperSection : Panel
	{
		public TextStepperSection()
		{
			var textUpDown = new TextStepper { PlaceholderText = "<multiple values>" };
			LogEvents(textUpDown);

			var enabledCheckBox = new CheckBox { Text = "Enabled" };
			enabledCheckBox.CheckedBinding.Bind(textUpDown, c => c.Enabled);

			var readOnlyCheckBox = new CheckBox { Text = "ReadOnly" };
			readOnlyCheckBox.CheckedBinding.Bind(textUpDown, c => c.ReadOnly);

			var setTextButton = new Button { Text = "Set Text" };
			setTextButton.Click += (sender, e) => textUpDown.Text = "Some text";

			var validDirectionsDropDown = new EnumDropDown<StepperValidDirections>();
			validDirectionsDropDown.SelectedValueBinding.Bind(textUpDown, s => s.ValidDirection);

			Content = new StackLayout
			{
				Padding = 10,
				Spacing = 5,
				Items =
				{
					enabledCheckBox,
					TableLayout.Horizontal(2, readOnlyCheckBox, setTextButton),
					TableLayout.Horizontal(2, "ValidDirection", validDirectionsDropDown),
					textUpDown
				}
			};
		}

		void LogEvents(TextStepper textUpDown)
		{
			textUpDown.Step += (sender, e) => Log.Write(textUpDown, $"Step: {e.Direction}");
			textUpDown.TextChanging += (sender, e) => Log.Write(textUpDown, $"TextChanging: {e.Text}, Current: {textUpDown.Text}");
			textUpDown.TextChanged += (sender, e) => Log.Write(textUpDown, $"TextChanged: {textUpDown.Text}");
}
	}
}
