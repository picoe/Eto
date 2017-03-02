using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Drawing
{
	[Section("Drawing", "Antialias")]
	public class AntialiasSection : Panel
	{
		public AntialiasSection()
		{
			Content = new DynamicLayout(
				new DynamicRow(
					"Antialias", AntialiasOn(),
					null
				),
				null
			) { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };
		}

		Control AntialiasOn()
		{
			var control = new Drawable { Size = new Size(400, 100), BackgroundColor = Colors.Black };

			var path = CreatePath();
			control.Paint += (sender, e) =>
			{
				e.Graphics.AntiAlias = true;
				e.Graphics.DrawText(SystemFonts.Default(), Brushes.White, 0, 0, "Antialias ON");
				e.Graphics.DrawPath(Pens.White, path);
				e.Graphics.DrawLine(Pens.White, 0, 20, 100, 100);

				e.Graphics.AntiAlias = false;
				e.Graphics.TranslateTransform(100, 0);
				e.Graphics.DrawText(SystemFonts.Default(), Brushes.White, 0, 0, "Antialias OFF");
				e.Graphics.DrawPath(Pens.White, path);
				e.Graphics.DrawLine(Pens.White, 0, 20, 100, 100);

				e.Graphics.AntiAlias = true;
				e.Graphics.TranslateTransform(100, 0);
				e.Graphics.DrawText(SystemFonts.Default(), Brushes.White, 0, 0, "Antialias ON");
				e.Graphics.DrawPath(Pens.White, path);
				e.Graphics.DrawLine(Pens.White, 0, 20, 100, 100);

				e.Graphics.AntiAlias = false;
				e.Graphics.TranslateTransform(100, 0);
				e.Graphics.DrawText(SystemFonts.Default(), Brushes.White, 0, 0, "Antialias OFF");
				e.Graphics.DrawPath(Pens.White, path);
				e.Graphics.DrawLine(Pens.White, 0, 20, 100, 100);
			};

			return control;
		}

		GraphicsPath CreatePath()
		{
			var path = new GraphicsPath();
			path.MoveTo(new Point(10, 16));
			path.LineTo(new Point(20, 90));
			path.LineTo(new Point(10, 60));
			path.LineTo(new Point(90, 80));
			path.LineTo(new Point(60, 30));
			return path;
		}
	}
}
