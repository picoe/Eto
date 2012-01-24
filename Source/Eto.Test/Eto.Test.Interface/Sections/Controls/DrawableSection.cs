using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.Test.Interface.Controls;

namespace Eto.Test.Interface.Sections.Controls
{
	public class DrawableSection : SectionBase
	{
		public DrawableSection ()
		{
			var layout = new TableLayout(this, 3, 2);
			
			layout.Add (new Label{ Text = "Default"}, 0, 0);
			layout.Add (this.Drawable(), 1, 0);
		}
		
		Control Drawable ()
		{
			var control = new Drawable{
				Size = new Size(100, 100)
			};
			control.Paint += delegate(object sender, PaintEventArgs pe) {
				pe.Graphics.FillRectangle (Color.Black, pe.ClipRectangle);
				pe.Graphics.DrawLine (Color.White, 0, 0, 99, 99);
				Log (control, "Paint, ClipRectangle: {0}", pe.ClipRectangle);
			};
			
			return control;
		}
	}
}

