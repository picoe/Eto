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
			drawable.MouseMove += HandleMouseMove;
			drawable.MouseDown += HandleMouseMove;
			drawable.Paint += (s, e) =>
			{
				var g = e.Graphics;
				g.DrawText(font, Colors.White, 3, 3, "Move the mouse in this area to move the shapes.");

				// texture brushes
				var temp = location;
				DrawShapes(textureBrush, img.Size, g);

				// solid brushes
				location = temp + new PointF(200, 0);
				DrawShapes(solidBrush, img.Size, g);

				// linear gradient brushes
				location = temp + new PointF(400, 0);
				DrawShapes(linearGradientBrush, img.Size, g);
			};
		}

		void HandleMouseMove(object sender, MouseEventArgs e)
		{
			location = e.Location;
			((Control)sender).Invalidate();
			e.Handled = true;
		}

		void DrawShapes(Brush brush, Size size, Graphics g)
		{
			g.SaveTransform();
			g.TranslateTransform(location);
			g.RotateTransform(20);

			// rectangle
			g.FillRectangle(brush, new RectangleF(size));

			// ellipse
			g.TranslateTransform(0, size.Height + 20);
			g.FillEllipse(brush, new RectangleF(size));

			// pie
			g.TranslateTransform(0, size.Height + 20);
			g.FillPie(brush, new RectangleF(new SizeF(size.Width * 2, size.Height)), 0, 360);

			// polygon
			g.TranslateTransform(0, size.Height + 20);
			var polygon = GetPolygon();
			g.FillPolygon(brush, polygon);

			g.RestoreTransform();
		}

		static PointF[] GetPolygon()
		{
			var polygon = new PointF[] { new PointF(0, 50), new PointF(50, 100), new PointF(100, 50), new PointF(50, 0) };
			return polygon;
		}
	}
}
