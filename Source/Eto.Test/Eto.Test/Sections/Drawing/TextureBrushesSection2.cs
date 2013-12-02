using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Drawing
{
	class TextureBrushesSection2 : Scrollable
	{
		Bitmap image = TestIcons.Textures;

		public TextureBrushesSection2()
		{
			var w = image.Size.Width / 3; // same as height
			var img = image.Clone(new Rectangle(w, w, w, w));
			var brush = new TextureBrush(img);
			var drawable = new Drawable();
			var font = new Font(SystemFont.Default);
			this.Content = drawable;
			var location = new PointF(100, 100);
			drawable.BackgroundColor = Colors.Green;
			drawable.MouseMove += (s, e) => {
				location = e.Location;
				drawable.Invalidate(); };
			drawable.Paint += (s, e) => {
				e.Graphics.DrawText(font, Colors.White, 3, 3, "Move the mouse in this area to move the textured shapes.");

				var temp = brush.Transform; // save state					
				brush.Transform = Matrix.FromTranslation(location);
				e.Graphics.FillRectangle(brush, new RectangleF(location, img.Size));
				brush.Transform = temp;

				location += new PointF(0, 100);
				temp = brush.Transform; // save state					
				brush.Transform = Matrix.FromTranslation(location);
				e.Graphics.FillEllipse(brush, new RectangleF(location, img.Size));
				brush.Transform = temp;

				location += new PointF(0, 100);
				temp = brush.Transform; // save state					
				brush.Transform = Matrix.FromTranslation(location);
				var polygon = GetPolygon(location);
				e.Graphics.FillPolygon(brush, polygon);
				brush.Transform = temp;
			};
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
