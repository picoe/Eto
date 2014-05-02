using System;
using System.Collections.Generic;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Drawing
{
	public class GetPixelSection : Panel
	{
		public GetPixelSection()
		{
			var location = new Point(100, 100);
			var image = TestIcons.Textures();
			var drawable = new Drawable();
			var drawableTarget = new DrawableTarget(drawable) { UseOffScreenBitmap = true };
			this.Content = drawable;

			EventHandler<MouseEventArgs> mouseHandler = (s, e) => {
				location = new Point(e.Location);
				((Control)s).Invalidate();
				e.Handled = true;
			};

			drawable.MouseMove += mouseHandler;
			drawable.MouseDown += mouseHandler;

			var font = SystemFonts.Default();
			drawable.BackgroundColor = Colors.Green;
			drawable.Paint += (s, e) => {
				var graphics = drawableTarget.BeginDraw(e);

				graphics.DrawText(font, Colors.White, 3, 3, "Move the mouse in this area to read the pixel color.");
				graphics.DrawImage(image, new PointF(100, 100));

				var pixelColor = drawableTarget.OffscreenBitmap.GetPixel(location.X, location.Y);
				graphics.DrawText(font, Colors.White, 3, 20, "Color: " + pixelColor);

				drawableTarget.EndDraw(e);
			};
		}
	}
}
