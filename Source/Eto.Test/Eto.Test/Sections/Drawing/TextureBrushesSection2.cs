using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Drawing
{
	class TextureBrushesSection2 : Scrollable
	{
		Bitmap image = Bitmap.FromResource("Eto.Test.Textures.png");
		public TextureBrushesSection2()
		{
			var w = image.Size.Width / 3; // same as height
			var img = image.Clone(new Rectangle(w, w, w, w));
			var brush = new TextureBrush(img);
			var drawable = new Drawable();
			var font = new Font(SystemFont.Default);
			this.AddDockedControl(drawable);
			var location = new Point(100, 100);
			drawable.BackgroundColor = Colors.Green;
			drawable.MouseMove += (s, e) => { location = e.Location; drawable.Invalidate(); };
			drawable.Paint += (s, e) => {
				e.Graphics.DrawText(font, Colors.White, 3, 3, "Move the mouse in this area to move the image.");

				var temp = brush.Transform; // save state					
				brush.Transform = Matrix.FromTranslation(location);
				e.Graphics.FillRectangle(brush, new RectangleF(location, img.Size));
				brush.Transform = temp;
			};
		}
	}
}
