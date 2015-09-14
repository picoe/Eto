using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Sections.Drawing
{
	[Section("Drawing", "PixelOffsetMode")]
	public class PixelOffsetSection : Scrollable
	{
		Size canvasSize = new Size(501, 221);

		public PixelOffsetSection()
		{
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };

			var drawable = new Drawable { Size = canvasSize };
			drawable.Paint += (sender, pe) =>
			{
				pe.Graphics.FillRectangle(Brushes.Black, pe.ClipRectangle);
				pe.Graphics.PixelOffsetMode = PixelOffsetMode.None;
				Draw(pe.Graphics);
			};
			layout.AddRow(new Label { Text = "None (Default)" }, drawable);

			drawable = new Drawable { Size = canvasSize };
			drawable.Paint += (sender, pe) =>
			{
				pe.Graphics.FillRectangle(Brushes.Black, pe.ClipRectangle);
				pe.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
				Draw(pe.Graphics);
			};
			layout.AddRow(new Label { Text = "Half" }, drawable);
			layout.Add(null);

			Content = layout;
		}

		public static void Draw(Graphics graphics)
		{
			var image = TestIcons.TestImage;
			// lines
			var whitePen = new Pen(Colors.White, 1);
			graphics.DrawLine(whitePen, 1, 1, 99, 99);
			graphics.DrawLine(whitePen, 50, 1, 50, 99);
			graphics.DrawLine(whitePen, 1, 51, 99, 51);

			graphics.DrawRectangle(whitePen, 101, 1, 100, 100);
			graphics.DrawRectangle(whitePen, 101, 1, 10, 10);

			graphics.DrawEllipse(Pens.Green, 101, 1, 100, 100);

			graphics.DrawPolygon(whitePen, new PointF(203, 1), new PointF(253, 51), new Point(203, 101), new PointF(203, 1), new PointF(253, 1), new PointF(253, 101), new PointF(203, 101));

			var rect = new RectangleF(255, 1, 100, 100);
			graphics.DrawArc(Pens.LightGreen, rect, 180, 90);
			graphics.DrawArc(Pens.SkyBlue, rect, 0, 90);
			rect.Inflate(-15, 0);
			graphics.DrawArc(Pens.FloralWhite, rect, -45, 90);
			rect.Inflate(-5, -20);
			graphics.DrawArc(Pens.SlateGray, rect, -45, 270);
			rect.Inflate(-10, -10);
			graphics.DrawArc(Pens.SteelBlue, rect, 180 + 45, 270);

			graphics.DrawImage(image, 100, 1, 100, 100);

			graphics.DrawText(Fonts.Sans(12 * graphics.PointsPerPixel), Colors.White, 0, 104, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");

			// filled
			graphics.FillRectangle(Brushes.White, 101, 120, 100, 100);
			graphics.FillRectangle(Brushes.Gray, 101, 120, 10, 10);

			graphics.FillEllipse(Brushes.Green, 101, 120, 100, 100);

			graphics.FillPolygon(Brushes.White, new PointF(202, 120), new PointF(252, 170), new Point(202, 220), new PointF(202, 120));

			rect = new RectangleF(255, 120, 100, 100);
			graphics.FillPie(Brushes.LightGreen, rect, 180, 90);
			graphics.FillPie(Brushes.SkyBlue, rect, 0, 90);
			rect.Inflate(-15, 0);
			graphics.FillPie(Brushes.FloralWhite, rect, -45, 90);
			rect.Inflate(-5, -20);
			graphics.FillPie(Brushes.SlateGray, rect, -45, 270);
			rect.Inflate(-10, -10);
			graphics.FillPie(Brushes.SteelBlue, rect, 180 + 45, 270);


			graphics.DrawImage(image, 101, 120, 100, 100);

			const int offset = 440;
			var xoffset = offset;
			var yoffset = 112;
			for (int i = 1; i < 14; i++)
			{
				var pen = new Pen(Colors.White, i);
				pen.LineCap = PenLineCap.Butt;
				graphics.DrawLine(pen, xoffset, 1, xoffset, 110);

				graphics.DrawLine(pen, offset, yoffset, offset + 100, yoffset);

				xoffset += i + 2;
				yoffset += i + 2;
			}

		}
	}
}
