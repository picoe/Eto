using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Behaviors
{
	public class KeyEventsSection : AllControlsBase
	{
		protected override void LogEvents (Control control)
		{
			base.LogEvents (control);
			
			control.KeyDown += delegate(object sender, KeyPressEventArgs e) {
				Log.Write (control, "KeyDown, Key: {0}, Char: {1}", e.KeyData, e.KeyChar);
			};
		}
	}
}

