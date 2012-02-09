using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Controls
{
	public class DateTimePickerSection : Panel
	{
		public DateTimePickerSection ()
		{
			var layout = new DynamicLayout (this);
			
			layout.AddRow (new Label{ Text = "Default" }, Default (), new Label { Text = "Default with Value" }, DefaultWithValue (), null);
			layout.AddRow (new Label{ Text = "Date" }, DateControl (), new Label { Text = "Date with Value" }, DateControlWithValue ());
			layout.AddRow (new Label{ Text = "Time" }, TimeControl (), new Label { Text = "Time with Value" }, TimeControlWithValue ());
			layout.AddRow (new Label{ Text = "Date/Time" }, DateTimeControl (), new Label { Text = "Date/Time with Value" }, DateTimeControlWithValue ());
			
			// growing space at end is blank!
			layout.Add (null);
		}
		
		DateTimePicker Default ()
		{
			var control = new DateTimePicker{  };
			LogEvents (control);
			return control;
		}
		
		Control DefaultWithValue ()
		{
			var control = Default ();
			control.Value = DateTime.Now;
			return control;
		}
		
		DateTimePicker DateControl ()
		{
			var control = new DateTimePicker{ Mode = DateTimePickerMode.Date };
			LogEvents (control);
			return control;
		}

		Control DateControlWithValue ()
		{
			var control = DateControl ();
			control.Value = DateTime.Now;
			return control;
		}
		
		DateTimePicker TimeControl ()
		{
			var control = new DateTimePicker{ Mode = DateTimePickerMode.Time };
			LogEvents (control);
			return control;
		}
		
		Control TimeControlWithValue ()
		{
			var control = TimeControl ();
			control.Value = DateTime.Now;
			return control;
		}
		
		DateTimePicker DateTimeControl ()
		{
			var control = new DateTimePicker{ Mode = DateTimePickerMode.DateTime };
			LogEvents (control);
			return control;
		}

		Control DateTimeControlWithValue ()
		{
			var control = DateTimeControl ();
			control.Value = DateTime.Now;
			return control;
		}
		
		void LogEvents (DateTimePicker control)
		{
			control.ValueChanged += delegate {
				Log.Write (control, "ValueChanged, Value: {0}", control.Value);
			};
		}
	}
}

