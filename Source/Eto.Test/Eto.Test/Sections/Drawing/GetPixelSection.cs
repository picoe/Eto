using System;
using System.Collections.Generic;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Drawing
{
	[Section("Drawing", "Bitmap GetPixel")]
	public class GetPixelSection : Panel
	{
		public GetPixelSection()
		{
			var location = new Point(100, 100);
			var image = TestIcons.Textures;
			var drawable = new BufferedDrawable { EnableDoubleBuffering = true };
			this.Content = drawable;

			EventHandler<MouseEventArgs> mouseHandler = (s, e) =>
			{
				location = new Point(e.Location);
				((Control)s).Invalidate();
				e.Handled = true;
			};

			drawable.MouseMove += mouseHandler;
			drawable.MouseDown += mouseHandler;

			var font = SystemFonts.Default();
			drawable.BackgroundColor = Colors.Green;
			drawable.Paint += (s, e) =>
			{
				var graphics = e.Graphics;
				var imageLocation = new PointF(100, 100);
				graphics.DrawText(font, Colors.White, 3, 3, "Move the mouse in this area to read the pixel color.");
				graphics.DrawImage(image, imageLocation);

				var loc = location - (Point)imageLocation;
				loc.Restrict(new Rectangle(image.Size));
				var pixelColor = image.GetPixel(loc.X, loc.Y);
				graphics.DrawText(font, Colors.White, 3, 20, "Color: " + pixelColor);
			};
		}
	}
}
