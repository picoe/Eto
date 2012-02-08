using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Controls
{
	public class SliderSection : Panel
	{
		public SliderSection ()
		{
			var layout = new DynamicLayout (this);
			
			layout.AddRow (new Label { Text = "Default" }, Default ());
			layout.AddRow (new Label { Text = "SetInitialValue" }, SetInitialValue ());
			layout.AddRow (new Label { Text = "Disabled" }, Disabled ());
			layout.AddRow (new Label { Text = "Vertical" }, Vertical ());
			
			layout.Add (null, null, true);
			
		}
		
		Control Default ()
		{
			var control = new Slider ();
			LogEvents(control);
			return control;
		}
		
		Slider SetInitialValue ()
		{
			var control = new Slider{
				MinValue = 0,
				MaxValue = 1000, 
				TickFrequency = 100,
				Value = 500
			};
			LogEvents(control);
			return control;
		}
		
		Control Disabled ()
		{
			var control = SetInitialValue ();
			control.Enabled = false;
			return control;
		}

		Control Vertical ()
		{
			var control = SetInitialValue ();
			control.Size = new Size(-1, 150);
			control.Orientation = SliderOrientation.Vertical;
			var layout = new DynamicLayout (new Panel ());
			layout.AddCentered (control);
			return layout.Container;
		}
		
		void LogEvents (Slider control)
		{
			control.ValueChanged += delegate {
				Log.Write (control, "ValueChanged, Value: {0}", control.Value);
			};
		}
	}
}

