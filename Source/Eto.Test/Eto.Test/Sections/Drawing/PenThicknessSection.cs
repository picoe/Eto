using Eto.Drawing;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eto.Test.Sections.Drawing
{
	public class PenThicknessSection : Scrollable
	{
		public PenThicknessSection ()
		{
			var layout = new DynamicLayout (this);

			layout.AddRow (GetDrawable ());
		}

		Drawable GetDrawable ()
		{
			var drawable = new Drawable {
				Size = new Size (560, 300)
			};
			drawable.Paint += (sender, pe) => {
				Draw (pe.Graphics, null);
			};
			return drawable;
		}

		public static void Draw(Graphics g, Action<IPen> action)
		{
			var path = new GraphicsPath ();
			path.AddLines (new PointF (0, 0), new PointF (100, 40), new PointF (0, 30), new PointF (50, 70));

			for (int i = 0; i < 4; i++) {
				float thickness = 1f + i * 5f;
				var pen = Pen.Create (Colors.Black, thickness);
				if (action != null)
					action (pen);
				var y = i * 20;
				g.DrawLine (pen, 10, y, 110, y);
				
				y = 80 + i * 50;
				g.DrawRectangle (pen, 10, y, 100, 30);

				y = i * 70;
				g.DrawArc (pen, 140, y, 100, 80, 160, 160);
				
				y = i * 70;
				g.DrawEllipse (pen, 260, y, 100, 50);
				
				g.SaveTransform ();
				y = i * 70;
				g.TranslateTransform (400, y);
				g.DrawPath (pen, path);
				g.RestoreTransform ();
			}
		}

	}
}
