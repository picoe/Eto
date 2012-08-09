using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Sections.Controls
{
	public class ButtonSection : Panel
	{
		public ButtonSection ()
		{
			var layout = new DynamicLayout (this);
			
			//layout.SetColumnScale(0);
			
			layout.BeginVertical ();
			layout.AddRow (null, NormalButton (), null);
			layout.EndVertical ();
			layout.BeginVertical ();
			layout.AddRow (null, LongerButton (), null);
			layout.EndVertical ();
			layout.BeginVertical ();
			layout.AddRow (null, ColourButton (), null);
			layout.EndVertical ();
			
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
		
		void LogEvents(Button button)
		{
			button.Click += delegate {
				Log.Write (button, "Click");
			};
		}
	}
}

