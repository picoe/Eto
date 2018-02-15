using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(Spinner))]
	public class SpinnerSection : Panel
	{
		public SpinnerSection()
		{
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };

			layout.AddRow(new Label { Text = "Default" }, Default());
			layout.AddRow(new Label { Text = "Larger" }, Default(new Size(100, 100)));

			layout.Add(null, null, true);

			Content = layout;
		}

		Control Default(Size? size = null)
		{
			var control = new Spinner();
			if (size != null)
				control.Size = size.Value;

			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5) };

			layout.AddCentered(control);

			layout.BeginVertical();
			layout.AddRow(null, StartStopButton(control), null);
			layout.EndVertical();

			return layout;
		}

		Control StartStopButton(Spinner spinner)
		{
			var control = new Button { Text = spinner.Enabled ? "Stop" : "Start" };
			control.Click += delegate
			{
				spinner.Enabled = !spinner.Enabled;
				control.Text = spinner.Enabled ? "Stop" : "Start";
			};
			return control;
		}
	}
}

