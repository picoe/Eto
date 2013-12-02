using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Drawing
{
	class TextureBrushesSection2 : Scrollable
	{
		Bitmap image = TestIcons.Textures;

		PointF location = new PointF(100, 100);

		public TextureBrushesSection2()
		{
			var w = image.Size.Width / 3; // same as height
			var img = image.Clone(new Rectangle(w, w, w, w));
			var textureBrush = new TextureBrush(img);
			var solidBrush = new SolidBrush(Colors.Blue);
			var linearGradientBrush = new LinearGradientBrush(Colors.White, Colors.Black, PointF.Empty, new PointF(0, 100));
			var drawable = new Drawable();
			var font = new Font(SystemFont.Default);
			this.Content = drawable;
			drawable.BackgroundColor = Colors.Green;
			drawable.MouseMove += (s, e) => {
				location = e.Location;
				drawable.Invalidate(); };
			drawable.Paint += (s, e) => {
				e.Graphics.DrawText(font, Colors.White, 3, 3, "Move the mouse in this area to move the shapes.");

				// texture brushes
				var temp = location;
				DrawShapes(textureBrush, img.Size, e.Graphics);

				// solid brushes
				location = temp + new PointF(200, 0);
				DrawShapes(solidBrush, img.Size, e.Graphics);

				// linear gradient brushes
				location = temp + new PointF(400, 0);
				DrawShapes(linearGradientBrush, img.Size, e.Graphics);
			};
		}

		private void DrawShapes(Brush brush, Size size, Graphics g)
		{
			// rectangle
			g.FillRectangle(brush, new RectangleF(location, size));

			// ellipse
			location += new PointF(0, size.Height + 20);
			g.FillEllipse(brush, new RectangleF(location, size));

			// pie
			location += new PointF(0, size.Height + 20);
			g.FillPie(brush, new RectangleF(location, new SizeF(size.Width * 2, size.Height)), 0, 360);

			// polygon
			location += new PointF(0, size.Height + 20);
			var polygon = GetPolygon(location);
			g.FillPolygon(brush, polygon);
		}

		private static PointF[] GetPolygon(PointF location)
		{
			var polygon = new PointF[] { new PointF(0, 50), new PointF(50, 100), new PointF(100, 50), new PointF(50, 0) };
			for (var i = 0; i < polygon.Length; ++i)
				polygon[i] += location;
			return polygon;
		}
	}
}
