using Eto.Drawing;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eto.Test.Sections.Drawing
{
	public class PenLineJoinSection : Scrollable
	{
		public PenLineJoinSection ()
		{
			var layout = new DynamicLayout (this);

			layout.AddRow (new Label { Text = "Miter (default)" }, GetDrawable (PenLineJoin.Miter));
			layout.AddRow (new Label { Text = "Round" }, GetDrawable (PenLineJoin.Round));
			layout.AddRow (new Label { Text = "Bevel" }, GetDrawable (PenLineJoin.Bevel));
		}

		Drawable GetDrawable (PenLineJoin join)
		{
			var drawable = new Drawable {
				Size = new Size (560, 300)
			};
			drawable.Paint += (sender, pe) => {
				PenThicknessSection.Draw (pe.Graphics, pen => { pen.LineJoin = join; });
			};
			return drawable;
		}
	}
}
