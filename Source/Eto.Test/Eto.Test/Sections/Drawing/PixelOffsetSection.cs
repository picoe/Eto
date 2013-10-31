using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Sections.Drawing
{
	public class PixelOffsetSection : Scrollable
	{
		static Image image = TestIcons.TestImage;
		Size canvasSize = new Size(501, 221);

		public PixelOffsetSection()
		{
			var layout = new DynamicLayout();

			var drawable = new Drawable { Size = canvasSize };
			drawable.Paint += (sender, pe) => {
				pe.Graphics.FillRectangle(Brushes.Black(), pe.ClipRectangle);
				pe.Graphics.PixelOffsetMode = PixelOffsetMode.None;
				Draw(pe.Graphics);
			};
			layout.AddRow(new Label { Text = "None (Default)" }, drawable);

			drawable = new Drawable { Size = canvasSize };
			drawable.Paint += (sender, pe) => {
				pe.Graphics.FillRectangle(Brushes.Black(), pe.ClipRectangle);
				pe.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
				Draw(pe.Graphics);
			};
			layout.AddRow(new Label { Text = "Half" }, drawable);
			layout.Add(null);

			Content = layout;
		}

		public static void Draw(Graphics g)
		{
			// lines
			var whitePen = Pens.White();
			g.DrawLine(whitePen, 1, 1, 99, 99);
			g.DrawLine(whitePen, 50, 1, 50, 99);
			g.DrawLine(whitePen, 1, 51, 99, 51);

			g.DrawRectangle(Pens.White(), 101, 1, 100, 100);
			g.DrawRectangle(Pens.White(), 101, 1, 10, 10);

			g.DrawEllipse(Pens.Green(), 101, 1, 100, 100);

			g.DrawPolygon(Pens.White(), new PointF(203, 1), new PointF(253, 51), new Point(203, 101), new PointF(203, 1), new PointF(253, 1), new PointF(253, 101), new PointF(203, 101));

			var rect = new RectangleF(255, 1, 100, 100);
			g.DrawArc(Pens.LightGreen(), rect, 180, 90);
			g.DrawArc(Pens.SkyBlue(), rect, 0, 90);
			rect.Inflate(-15, 0);
			g.DrawArc(Pens.FloralWhite(), rect, -45, 90);
			rect.Inflate(-5, -20);
			g.DrawArc(Pens.SlateGray(), rect, -45, 270);
			rect.Inflate(-10, -10);
			g.DrawArc(Pens.SteelBlue(), rect, 180 + 45, 270);

			g.DrawImage(image, 100, 1, 100, 100);

			g.DrawText(Fonts.Sans(12), Colors.White, 0, 104, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");

			// filled
			g.FillRectangle(Brushes.White(), 101, 120, 100, 100);
			g.FillRectangle(Brushes.Gray(), 101, 120, 10, 10);

			g.FillEllipse(Brushes.Green(), 101, 120, 100, 100);

			g.FillPolygon(Brushes.White(), new PointF(202, 120), new PointF(252, 170), new Point(202, 220), new PointF(202, 120));

			rect = new RectangleF(255, 120, 100, 100);
			g.FillPie(Brushes.LightGreen(), rect, 180, 90);
			g.FillPie(Brushes.SkyBlue(), rect, 0, 90);
			rect.Inflate(-15, 0);
			g.FillPie(Brushes.FloralWhite(), rect, -45, 90);
			rect.Inflate(-5, -20);
			g.FillPie(Brushes.SlateGray(), rect, -45, 270);
			rect.Inflate(-10, -10);
			g.FillPie(Brushes.SteelBlue(), rect, 180 + 45, 270);


		
			g.DrawImage(image, 101, 120, 100, 100);
		}
	}
}
