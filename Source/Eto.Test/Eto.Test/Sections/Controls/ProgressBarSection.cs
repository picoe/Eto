using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Controls
{
	public class ProgressBarSection : Panel
	{
		UITimer timer;

		public ProgressBarSection()
		{
			var layout = new DynamicLayout();
			
			layout.AddRow(new Label { Text = "Default" }, Default());
			layout.AddRow(new Label { Text = "SetInitialValue" }, SetValue());
			layout.AddRow(new Label { Text = "Indeterminate" }, Indeterminate());

			layout.Add(null, null, true);

			Content = layout;
		}

		Control Default()
		{
			var control = new ProgressBar();
			return control;
		}

		Control SetValue()
		{
			var control = new ProgressBar
			{
				MinValue = 0,
				MaxValue = 1000,
				Value = 500
			};

			var layout = new DynamicLayout();
			
			layout.Add(control);
			
			layout.BeginVertical();
			layout.AddRow(null, StartStopButton(control), null);
			layout.EndVertical();
			
			return layout;
		}

		Control Indeterminate()
		{
			var control = new ProgressBar
			{
				Indeterminate = true
			};
			return control;
		}

		Control StartStopButton(ProgressBar bar)
		{
			var control = new Button { Text = "Start Timer" };
			control.Click += delegate
			{
				if (timer == null)
				{
					timer = new UITimer { Interval = 0.5 };
					timer.Elapsed += delegate
					{
						if (bar.Value < bar.MaxValue)
							bar.Value += 50;
						else
							bar.Value = bar.MinValue;
					};
					timer.Start();
					control.Text = "Stop Timer";
				}
				else
				{
					timer.Stop();
					timer.Dispose();
					timer = null;
					control.Text = "Start Timer";
				}
			};
			return control;
		}

		protected override void Dispose(bool disposing)
		{
			if (timer != null)
				timer.Dispose();
			base.Dispose(disposing);
		}
	}
}

