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
		Bitmap image = TestIcons.Textures;

		public TextureBrushesSection()
		{
			var layout = new DynamicLayout();
			for (var i = 0; i < 10; ++i)
			{
				var w = image.Size.Width / 3; // same as height
				var img = image;
				if (i > 0)
					img = img.Clone(new Rectangle((i - 1) % 3 * w, (i - 1) / 3 * w, w, w));

				var brush = new TextureBrush(img);
				var drawable = new Drawable { Size = image.Size * 2 };

				drawable.Paint += (s, e) => {
					var destRect = new RectangleF(new PointF(100, 100), image.Size);
					var temp = brush.Transform; // save state
					brush.Transform = Matrix.FromRotation(90);
					e.Graphics.TranslateTransform(destRect.Location);
					e.Graphics.FillRectangle(brush, new RectangleF(destRect.Size));
					brush.Transform = temp;
				};
				layout.AddRow(drawable);
			}
			layout.Add(null);
			Content = layout;
		}
	}
}
