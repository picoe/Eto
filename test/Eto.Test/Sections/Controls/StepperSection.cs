using System;
using Eto.Forms;
namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(Stepper))]
	public class StepperSection : Panel
	{
		public StepperSection()
		{
			var stepper = new Stepper();
			LogEvents(stepper);

			var enabledCheckBox = new CheckBox { Text = "Enabled" };
			enabledCheckBox.CheckedBinding.Bind(stepper, s => s.Enabled);

			var validDirectionsDropDown = new EnumDropDown<StepperValidDirections>();
			validDirectionsDropDown.SelectedValueBinding.Bind(stepper, s => s.ValidDirection);

			Content = new StackLayout
			{
				Padding = 10,
				Spacing = 5,
				Items =
				{
					"Stepper",
					enabledCheckBox,
					validDirectionsDropDown,
					stepper
				}
			};
		}

		void LogEvents(Stepper stepper)
		{
			stepper.Step += (sender, e) => Log.Write(stepper, $"Step: {e.Direction}");
		}
	}
}
