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
					new Label { Text = "Antialias On" }, AntialiasOn(),
					new Label { Text = "Antialias Off" }, AntialiasOff(),
					null
				),
				null
			) { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };
		}

		Control AntialiasOn()
		{
			var control = new Drawable { Size = new Size(100, 100), BackgroundColor = Colors.Black };

			var path = CreatePath();
			control.Paint += (sender, e) =>
			{
				e.Graphics.AntiAlias = true;
				e.Graphics.DrawPath(Pens.White, path);
			};

			return control;
		}

		Control AntialiasOff()
		{
			var control = new Drawable { Size = new Size(100, 100), BackgroundColor = Colors.Black };

			var path = CreatePath();
			control.Paint += (sender, e) =>
			{
				e.Graphics.AntiAlias = false;
				e.Graphics.DrawPath(Pens.White, path);
			};

			return control;
		}

		GraphicsPath CreatePath()
		{
			var path = new GraphicsPath();
			path.MoveTo(new Point(10, 10));
			path.LineTo(new Point(20, 90));
			path.LineTo(new Point(10, 60));
			path.LineTo(new Point(90, 80));
			path.LineTo(new Point(60, 30));
			return path;
		}
	}
}
