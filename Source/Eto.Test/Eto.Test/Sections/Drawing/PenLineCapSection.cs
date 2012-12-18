using Eto.Drawing;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eto.Test.Sections.Drawing
{
	public class PenLineCapSection : Scrollable
	{
		public PenLineCapSection ()
		{
			var layout = new DynamicLayout (this);

			layout.AddRow (new Label { Text = "Butt (default)" }, GetDrawable (PenLineCap.Butt));
			layout.AddRow (new Label { Text = "Round" }, GetDrawable (PenLineCap.Round));
			layout.AddRow (new Label { Text = "Square" }, GetDrawable (PenLineCap.Square));
		}

		Drawable GetDrawable (PenLineCap lineCap)
		{
			var drawable = new Drawable {
				Size = new Size (560, 300)
			};
			drawable.Paint += (sender, pe) => {
				PenThicknessSection.Draw (pe.Graphics, pen => { pen.LineCap = lineCap; });
			};
			return drawable;
		}
	}
}
