using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Drawing
{
	class TextureBrushesSection : Scrollable
	{
		Bitmap image = Bitmap.FromResource("Eto.Test.Textures.png");
		public TextureBrushesSection()
		{
			var layout = new DynamicLayout(this);
			for (var i = 0; i < 10; ++i)
			{
				var w = image.Size.Width / 3; // same as height
				var img = i == 0 ? image : image.Clone(
					new Rectangle((i - 1) % 3 * w, (i - 1) / 3 * w, w, w));
				var b = new TextureBrush(img);
				Drawable d = new Drawable { Size = image.Size * 2 };
				d.Paint += (s, e) => {
					var destRect = new RectangleF(new PointF(100, 100), image.Size);
					var temp = b.Transform; // save state					
					b.Transform = Matrix.FromTranslation(destRect.Location);
					e.Graphics.FillRectangle(b, destRect);
					b.Transform = temp;
				};
				layout.AddRow(d);
			}
			layout.Add(null);
		}
	}
}
