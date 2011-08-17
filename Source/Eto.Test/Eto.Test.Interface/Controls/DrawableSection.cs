using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Interface.Controls
{
	public class DrawableSection : Panel
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
			};
			
			return control;
		}
	}
}

