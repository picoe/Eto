using System;
using System.Collections.Generic;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Drawing
{
	class TextureBrushesSection : Panel
	{
		Bitmap image = TestIcons.Textures;

		public TextureBrushesSection()
		{
			var drawable = new Drawable { Size = image.Size * 2 };
			var drawableTarget = new DrawableTarget (drawable);
			var layout = new DynamicLayout(new Padding(10));
			layout.AddSeparateRow(null, drawableTarget.Checkbox(), null);
			layout.Add(new Scrollable { Content = drawable, ScrollSize =  new Size(image.Size.Width, image.Size.Height * 11) }); // Setting ScrollSize does not seem to work
			this.Content = layout;

			var renderers = new List<Action<Graphics>> ();

			for (var i = 0; i < 10; ++i)
			{
				var w = image.Size.Width / 3; // same as height
				var img = image;
				if (i > 0)
					img = img.Clone(new Rectangle((i - 1) % 3 * w, (i - 1) / 3 * w, w, w));

				var brush = new TextureBrush(img);

				renderers.Add(g => {
					var temp = brush.Transform; // save state
					brush.Transform = Matrix.FromRotation(90);
					g.FillRectangle(brush, new RectangleF(image.Size));
					g.TranslateTransform(0, image.Size.Height);
					brush.Transform = temp;
				});
			}

			drawable.Paint += (s, e) =>
			{
				var g = drawableTarget.BeginDraw(e.Graphics);
				foreach(var r in renderers)
					r(g);
				drawableTarget.EndDraw(e.Graphics);
			};
		}
	}
}
