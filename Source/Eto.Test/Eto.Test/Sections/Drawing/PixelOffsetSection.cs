using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Sections.Drawing
{
	public class PixelOffsetSection : Scrollable
	{
		static Image image = TestIcons.TestImage();
		Size canvasSize = new Size(501, 221);

		public PixelOffsetSection()
		{
			var layout = new DynamicLayout();

			var drawable = new Drawable { Size = canvasSize };
			drawable.Paint += (sender, pe) => {
				pe.Graphics.FillRectangle(Brushes.Black(), pe.ClipRectangle);
				pe.Graphics.PixelOffsetMode = PixelOffsetMode.None;
				Draw(pe.Graphics, Generator);
			};
			layout.AddRow(new Label { Text = "None (Default)" }, drawable);

			drawable = new Drawable { Size = canvasSize };
			drawable.Paint += (sender, pe) => {
				pe.Graphics.FillRectangle(Brushes.Black(), pe.ClipRectangle);
				pe.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
				Draw(pe.Graphics, Generator);
			};
			layout.AddRow(new Label { Text = "Half" }, drawable);
			layout.Add(null);

			Content = layout;
		}

		public static void Draw(Graphics g, Generator generator, bool drawImages = true) // TODO: remove drawImages, which currently disables drawing images with a toolkit because the image is not per-generator.
		{
			// lines
			var whitePen = Pens.White(generator);
			g.DrawLine(whitePen, 1, 1, 99, 99);
			g.DrawLine(whitePen, 50, 1, 50, 99);
			g.DrawLine(whitePen, 1, 51, 99, 51);

			g.DrawRectangle(Pens.White(generator), 101, 1, 100, 100);
			g.DrawRectangle(Pens.White(generator), 101, 1, 10, 10);

			g.DrawEllipse(Pens.Green(generator), 101, 1, 100, 100);

			g.DrawPolygon(Pens.White(generator), new PointF(203, 1), new PointF(253, 51), new Point(203, 101), new PointF(203, 1), new PointF(253, 1), new PointF(253, 101), new PointF(203, 101));

			var rect = new RectangleF(255, 1, 100, 100);
			g.DrawArc(Pens.LightGreen(generator), rect, 180, 90);
			g.DrawArc(Pens.SkyBlue(generator), rect, 0, 90);
			rect.Inflate(-15, 0);
			g.DrawArc(Pens.FloralWhite(generator), rect, -45, 90);
			rect.Inflate(-5, -20);
			g.DrawArc(Pens.SlateGray(generator), rect, -45, 270);
			rect.Inflate(-10, -10);
			g.DrawArc(Pens.SteelBlue(generator), rect, 180 + 45, 270);

			if (drawImages)
				g.DrawImage(image, 100, 1, 100, 100);

			g.DrawText(Fonts.Sans(12, generator: generator), Colors.White, 0, 104, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");

			// filled
			g.FillRectangle(Brushes.White(generator), 101, 120, 100, 100);
			g.FillRectangle(Brushes.Gray(generator), 101, 120, 10, 10);

			g.FillEllipse(Brushes.Green(generator), 101, 120, 100, 100);

			g.FillPolygon(Brushes.White(generator), new PointF(202, 120), new PointF(252, 170), new Point(202, 220), new PointF(202, 120));

			rect = new RectangleF(255, 120, 100, 100);
			g.FillPie(Brushes.LightGreen(generator), rect, 180, 90);
			g.FillPie(Brushes.SkyBlue(generator), rect, 0, 90);
			rect.Inflate(-15, 0);
			g.FillPie(Brushes.FloralWhite(generator), rect, -45, 90);
			rect.Inflate(-5, -20);
			g.FillPie(Brushes.SlateGray(generator), rect, -45, 270);
			rect.Inflate(-10, -10);
			g.FillPie(Brushes.SteelBlue(generator), rect, 180 + 45, 270);


			if (drawImages)
				g.DrawImage(image, 101, 120, 100, 100);
		}
	}
}
