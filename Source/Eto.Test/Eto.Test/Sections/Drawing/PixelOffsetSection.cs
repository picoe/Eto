using Eto.Drawing;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eto.Test.Sections.Drawing
{
	public class PixelOffsetSection : Scrollable
	{
		Image image = Bitmap.FromResource ("Eto.Test.TestImage.png");
		Size canvasSize = new Size(500, 221);


		public PixelOffsetSection ()
		{
			var layout = new DynamicLayout (this);

			var drawable = new Drawable { Size = canvasSize };
			drawable.Paint += (sender, pe) => {
				pe.Graphics.FillRectangle (Colors.Black, new RectangleF (canvasSize));
				pe.Graphics.PixelOffsetMode = PixelOffsetMode.None;
				Draw (pe.Graphics, pe.ClipRectangle);
			};
			layout.AddRow (new Label { Text = "None (Default)" }, drawable);

			drawable = new Drawable { Size = canvasSize };
			drawable.Paint += (sender, pe) => {
				pe.Graphics.FillRectangle (Colors.Black, new RectangleF (canvasSize));
				pe.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
				Draw (pe.Graphics, pe.ClipRectangle);
			};
			layout.AddRow (new Label { Text = "Half" }, drawable);
		}

		void Draw (Graphics g, Rectangle clip)
		{
			// lines
			g.DrawLine (Colors.White, 0, 1, 99, 100);
			g.DrawLine (Colors.White, 50, 1, 50, 100);
			g.DrawLine (Colors.White, 0, 51, 99, 51);

			g.DrawRectangle (Colors.White, 101, 1, 100, 100);
			g.DrawRectangle (Colors.White, 101, 1, 10, 10);

			g.DrawEllipse (Colors.Green, 101, 1, 100, 100);

			g.DrawPolygon (Colors.White, new PointF (203, 1), new PointF (253, 51), new Point (203, 101), new PointF (203, 1), new PointF (253, 1), new PointF (253, 101), new PointF (203, 101));

			var rect = new RectangleF (255, 1, 100, 100);
			g.DrawArc (Colors.LightGreen, rect, 180, 90);
			g.DrawArc (Colors.SkyBlue, rect, 0, 90);
			rect.Inflate (-15, 0);
			g.DrawArc (Colors.FloralWhite, rect, -45, 90);
			rect.Inflate (-5, -20);
			g.DrawArc (Colors.SlateGray, rect, -45, 270);
			rect.Inflate (-10, -10);
			g.DrawArc (Colors.SteelBlue, rect, 180+45, 270);

			g.DrawImage (image, 100, 1, 100, 100);

			g.DrawText (new Font (FontFamilies.Sans, 12), Colors.White, 0, 100, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");

			// filled
			g.FillRectangle (Colors.White, 101, 120, 100, 100);
			g.FillRectangle (Colors.Gray, 101, 120, 10, 10);

			g.FillEllipse (Colors.Green, 101, 120, 100, 100);

			g.FillPolygon (Colors.White, new PointF (202, 120), new PointF (252, 170), new Point (202, 220), new PointF (202, 120));

			rect = new RectangleF (255, 120, 100, 100);
			g.FillPie (Colors.LightGreen, rect, 180, 90);
			g.FillPie (Colors.SkyBlue, rect, 0, 90);
			rect.Inflate (-15, 0);
			g.FillPie (Colors.FloralWhite, rect, -45, 90);
			rect.Inflate (-5, -20);
			g.FillPie (Colors.SlateGray, rect, -45, 270);
			rect.Inflate (-10, -10);
			g.FillPie (Colors.SteelBlue, rect, 180 + 45, 270);


		
			g.DrawImage (image, 101, 120, 100, 100);
		}
	}
}
