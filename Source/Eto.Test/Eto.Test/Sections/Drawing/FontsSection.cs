using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.Test.Sections.Behaviors;

namespace Eto.Test.Sections.Drawing
{
	public class FontsSection : AllControlsBase
	{
		protected override void LogEvents (Control control)
		{
			base.LogEvents (control);

			var cc = control as CommonControl;
			if (cc != null) {
				cc.Font = new Font (FontFamily.Serif, 20, FontStyle.Italic);
			}
			var gb = control as GroupBox;
			if (gb != null) {
				gb.Font = new Font (FontFamily.Serif, 20, FontStyle.Italic);
			}
		}
	}
}

