using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(ColorPicker))]
	public class ColorPickerSection : Panel
	{
		public ColorPickerSection()
		{
			Content = new TableLayout
			{
				Spacing = new Size(5, 5),
				Padding = new Padding(10),
				Rows = 
				{
					new TableRow(
						"Default",
						Default(),
						null
					),
					new TableRow(
						"Initial Value",
						InitialValue(),
						null
					),
					null
				}
			};
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

