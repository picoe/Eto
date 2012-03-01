using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Controls
{
	public class ProgressBarSection : Panel
	{
		UITimer timer;
		
		public ProgressBarSection ()
		{
			var layout = new DynamicLayout (this);
			
			layout.AddRow (new Label { Text = "Default" }, Default ());
			layout.AddRow (new Label { Text = "SetInitialValue" }, SetValue ());

			
			layout.Add (null, null, true);
			
		}
		
		Control Default ()
		{
			var control = new ProgressBar ();
			LogEvents (control);
			return control;
		}
		
		Control SetValue ()
		{
			var control = new ProgressBar{
				MinValue = 0,
				MaxValue = 1000,
				Value = 500
			};
			LogEvents (control);
			
			
			var layout = new DynamicLayout (new Panel ());
			
			layout.Add (control);
			
			layout.BeginVertical ();
			layout.AddRow (null, StartButton (control), StopButton (), null);
			layout.EndVertical ();
			
			return layout.Container;
		}
		
		Control StopButton ()
		{
			var control = new Button { Text = "Stop" };
			control.Click += delegate {
				if (timer != null) {
					timer.Stop ();
					timer.Dispose ();
					timer = null;
				}
			};
			return control;
		}
		
		Control StartButton (ProgressBar bar)
		{
			var control = new Button { Text = "Start" };
			control.Click += delegate {
				if (timer == null) {
					timer = new UITimer { Interval = 1.0 };
					timer.Elapsed += delegate {
						if (bar.Value < bar.MaxValue)
							bar.Value += 50;
						else
							bar.Value = bar.MinValue;
					};
					timer.Start ();
				}
			};
			return control;
		}

		void LogEvents (ProgressBar control)
		{

		}
		
		protected override void Dispose (bool disposing)
		{
			if (timer != null)
				timer.Dispose ();
			base.Dispose (disposing);
		}
	}
}

