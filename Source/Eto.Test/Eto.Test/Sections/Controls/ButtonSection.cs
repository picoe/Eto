using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Sections.Controls
{
	public class ButtonSection : Scrollable
	{
		public ButtonSection ()
		{
			ExpandContentWidth = true;
			ExpandContentHeight = true;
			var layout = new DynamicLayout (this);
			
			//layout.SetColumnScale(0);

			layout.AddAutoSized (NormalButton (), centered: true);
			layout.AddAutoSized (LongerButton (), centered: true);
			layout.AddAutoSized (ColourButton (), centered: true);
			layout.AddAutoSized (DisabledButton (), centered: true);

			layout.Add (null);
		}
		
		Control NormalButton ()
		{
			var control = new Button{ Text = "Click Me" };
			LogEvents(control);
			return control;
		}

		Control LongerButton ()
		{
			var control = new Button{ Text = "This is a long(er) button title" };
			LogEvents(control);
			return control;
		}

		Control ColourButton ()
		{
			var control = new Button{ Text = "Button with Color", BackgroundColor = Colors.Lime };
			LogEvents(control);
			return control;
		}

		Control DisabledButton ()
		{
			var control = new Button{ Text = "Disabled Button", Enabled = false };
			LogEvents(control);
			return control;
		}

		void LogEvents(Button button)
		{
			button.Click += delegate {
				Log.Write (button, "Click");
			};
		}
	}
}

