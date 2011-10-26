using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Interface.Controls
{
	public class DateTimePickerSection : Panel
	{
		public DateTimePickerSection ()
		{
			var layout = new DynamicLayout (this);

			layout.BeginHorizontal ();
			layout.Add (new Label{ Text = "Default" });
			layout.Add (new DateTimePicker{  });
			layout.EndHorizontal ();
			
			layout.BeginHorizontal ();
			layout.Add (new Label{ Text = "Date" });
			layout.Add (new DateTimePicker{ Mode = DateTimePickerMode.Date });
			layout.EndHorizontal ();
			
			layout.BeginHorizontal ();
			layout.Add (new Label{ Text = "Time" });
			layout.Add (new DateTimePicker{ Mode = DateTimePickerMode.Time });
			layout.EndHorizontal ();

			layout.BeginHorizontal ();
			layout.Add (new Label{ Text = "Date/Time" });
			layout.Add (new DateTimePicker{ Mode = DateTimePickerMode.DateTime });
			layout.EndHorizontal ();
			
			// growing space at end is blank!
			layout.Add (null);
		}
	}
}

