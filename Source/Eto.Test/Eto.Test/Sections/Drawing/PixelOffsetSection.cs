using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Sections.Drawing
{
	public class PixelOffsetSection : Scrollable
	{
		Size canvasSize = new Size(501, 221);

		public PixelOffsetSection()
		{
			var layout = new DynamicLayout();

			var drawable = new Drawable { Size = canvasSize };
			drawable.Paint += (sender, pe) => {
				pe.Graphics.FillRectangle(Brushes.Black(Generator), pe.ClipRectangle);
				pe.Graphics.PixelOffsetMode = PixelOffsetMode.None;
				Draw(pe.Graphics);
			};
			layout.AddRow(new Label { Text = "None (Default)" }, drawable);

			drawable = new Drawable { Size = canvasSize };
			drawable.Paint += (sender, pe) => {
				pe.Graphics.FillRectangle(Brushes.Black(Generator), pe.ClipRectangle);
				pe.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
				Draw(pe.Graphics);
			};
			layout.AddRow(new Label { Text = "Half" }, drawable);
			layout.Add(null);

			Content = layout;
		}

		public static void Draw(Graphics graphics)
		{
			var generator = graphics.Generator;
			var image = TestIcons.TestImage(generator);
			// lines
			var whitePen = Pens.White(generator);
			graphics.DrawLine(whitePen, 1, 1, 99, 99);
			graphics.DrawLine(whitePen, 50, 1, 50, 99);
			graphics.DrawLine(whitePen, 1, 51, 99, 51);

			graphics.DrawRectangle(Pens.White(generator), 101, 1, 100, 100);
			graphics.DrawRectangle(Pens.White(generator), 101, 1, 10, 10);

			graphics.DrawEllipse(Pens.Green(generator), 101, 1, 100, 100);

			graphics.DrawPolygon(Pens.White(generator), new PointF(203, 1), new PointF(253, 51), new Point(203, 101), new PointF(203, 1), new PointF(253, 1), new PointF(253, 101), new PointF(203, 101));

			var rect = new RectangleF(255, 1, 100, 100);
			graphics.DrawArc(Pens.LightGreen(generator), rect, 180, 90);
			graphics.DrawArc(Pens.SkyBlue(generator), rect, 0, 90);
			rect.Inflate(-15, 0);
			graphics.DrawArc(Pens.FloralWhite(generator), rect, -45, 90);
			rect.Inflate(-5, -20);
			graphics.DrawArc(Pens.SlateGray(generator), rect, -45, 270);
			rect.Inflate(-10, -10);
			graphics.DrawArc(Pens.SteelBlue(generator), rect, 180 + 45, 270);

			graphics.DrawImage(image, 100, 1, 100, 100);

			graphics.DrawText(Fonts.Sans(12 * graphics.PointsPerPixel, generator: generator), Colors.White, 0, 104, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");

			// filled
			graphics.FillRectangle(Brushes.White(generator), 101, 120, 100, 100);
			graphics.FillRectangle(Brushes.Gray(generator), 101, 120, 10, 10);

			graphics.FillEllipse(Brushes.Green(generator), 101, 120, 100, 100);

			graphics.FillPolygon(Brushes.White(generator), new PointF(202, 120), new PointF(252, 170), new Point(202, 220), new PointF(202, 120));

			rect = new RectangleF(255, 120, 100, 100);
			graphics.FillPie(Brushes.LightGreen(generator), rect, 180, 90);
			graphics.FillPie(Brushes.SkyBlue(generator), rect, 0, 90);
			rect.Inflate(-15, 0);
			graphics.FillPie(Brushes.FloralWhite(generator), rect, -45, 90);
			rect.Inflate(-5, -20);
			graphics.FillPie(Brushes.SlateGray(generator), rect, -45, 270);
			rect.Inflate(-10, -10);
			graphics.FillPie(Brushes.SteelBlue(generator), rect, 180 + 45, 270);


			graphics.DrawImage(image, 101, 120, 100, 100);
		}
	}
}
