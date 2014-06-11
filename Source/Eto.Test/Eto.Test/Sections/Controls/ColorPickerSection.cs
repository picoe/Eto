using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(ColorPicker))]
	public class ColorPickerSection : Panel
	{
		public ColorPickerSection()
		{
			Content = new TableLayout(
				new TableRow(
					new Label { Text = "Default" },
					Default(),
					null
				),
				new TableRow(
					new Label { Text = "Initial Value" },
					InitialValue(),
					null
				),
				null
			);
		}

		Control Default()
		{
			var control = new ColorPicker();
			LogEvents(control);
			return control;
		}

		Control InitialValue()
		{
			var control = new ColorPicker { Value = Colors.Blue };
			LogEvents(control);
			return control;
		}

		void LogEvents(ColorPicker control)
		{
			control.ValueChanged += delegate
			{
				Log.Write(control, "SelectedColorChanged, Color: {0}", control.Value);
			};
		}
	}
}

