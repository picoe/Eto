using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Behaviors
{
	public class KeyEventsSection : AllControlsBase
	{
		CheckBox handleEvents;

		protected override void LogEvents (Control control)
		{
			base.LogEvents (control);
			
			control.KeyDown += (sender, e) => {
				Log.Write (control, "KeyDown, Key: {0}, Char: {1}", e.KeyData, e.IsChar ? e.KeyChar.ToString () : "no char");
				if (handleEvents.Checked == true)
					e.Handled = true;
			};

			control.KeyUp += (sender, e) => {
				Log.Write (control, "KeyUp, Key: {0}, Char: {1}", e.KeyData, e.IsChar ? e.KeyChar.ToString () : "no char");
			};
		}

		protected override Control GenerateOptions ()
		{
			var layout = new DynamicLayout (new Panel ());

			layout.AddRow (null, Handled (), null);
			layout.Add (null);

			return layout.Container;
		}

		Control Handled ()
		{
			return handleEvents = new CheckBox { Text = "Handle key events" };
		}
	}
}

