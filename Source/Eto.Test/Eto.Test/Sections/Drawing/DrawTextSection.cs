using System.Collections.Generic;
using Eto.Forms;
using Eto.Drawing;
using System;

namespace Eto.Test.Sections.Drawing
{
	public class DrawTextSection : Scrollable
	{
		public DrawTextSection(): this(null)
		{
		}

		public DrawTextSection(Generator generator)
			: base(generator)
		{
			var layout = new DynamicLayout();

			layout.AddRow(
				new Label { Text = "Default" }, Default(),
				null
			);

			layout.Add(null);

			Content = layout;
		}

		Control Default()
		{
			var control = new Drawable(Generator) { Size = new Size(400, 500), BackgroundColor = Colors.Black };
			control.Paint += (sender, e) => DrawFrame(e.Graphics);
			return control;
		}

		class DrawInfo
		{
			public Font Font { get; set; }
			public string Text { get; set; }
		}

		IEnumerable<DrawInfo> GetDrawInfo()
		{
			yield return new DrawInfo { Font = new Font(SystemFont.Default, generator: Generator), Text = "System Font & Size" };
			yield return new DrawInfo { Font = new Font(SystemFont.Default, 20, generator: Generator), Text = "System Font, 20pt" };

			yield return new DrawInfo { Font = Fonts.Sans(12, generator: Generator), Text = "Sans, 12pt" };
			yield return new DrawInfo { Font = Fonts.Serif(12, generator: Generator), Text = "Serif, 12pt" };
			yield return new DrawInfo { Font = Fonts.Monospace(12, generator: Generator), Text = "Monospace, 12pt" };
			yield return new DrawInfo { Font = Fonts.Cursive(12, generator: Generator), Text = "Cursive, 12pt" };
			yield return new DrawInfo { Font = Fonts.Fantasy(12, generator: Generator), Text = "Fantasy, 12pt" };

			yield return new DrawInfo { Font = Fonts.Sans(12, FontStyle.Bold, generator: Generator), Text = "Sans Bold, 12pt" };
			yield return new DrawInfo { Font = Fonts.Serif(12, FontStyle.Bold, generator: Generator), Text = "Serif Bold, 12pt" };
			yield return new DrawInfo { Font = Fonts.Monospace(12, FontStyle.Bold, generator: Generator), Text = "Monospace Bold, 12pt" };
			yield return new DrawInfo { Font = Fonts.Cursive(12, FontStyle.Bold, generator: Generator), Text = "Cursive Bold, 12pt" };
			yield return new DrawInfo { Font = Fonts.Fantasy(12, FontStyle.Bold, generator: Generator), Text = "Fantasy Bold, 12pt" };

			yield return new DrawInfo { Font = Fonts.Sans(12, FontStyle.Italic, generator: Generator), Text = "Sans Italic, 12pt" };
			yield return new DrawInfo { Font = Fonts.Serif(12, FontStyle.Italic, generator: Generator), Text = "Serif Italic, 12pt" };
			yield return new DrawInfo { Font = Fonts.Monospace(12, FontStyle.Italic, generator: Generator), Text = "Monospace Italic, 12pt" };
			yield return new DrawInfo { Font = Fonts.Cursive(12, FontStyle.Italic, generator: Generator), Text = "Cursive Italic, 12pt" };
			yield return new DrawInfo { Font = Fonts.Fantasy(12, FontStyle.Italic, generator: Generator), Text = "Fantasy Italic, 12pt" };

			yield return new DrawInfo { Font = Fonts.Sans(12, FontStyle.Bold | FontStyle.Italic, generator: Generator), Text = "Sans Bold & Italic, 12pt" };
			yield return new DrawInfo { Font = Fonts.Serif(12, FontStyle.Bold | FontStyle.Italic, generator: Generator), Text = "Serif Bold & Italic, 12pt" };
			yield return new DrawInfo { Font = Fonts.Monospace(12, FontStyle.Bold | FontStyle.Italic, generator: Generator), Text = "Monospace Bold & Italic, 12pt" };
			yield return new DrawInfo { Font = Fonts.Cursive(12, FontStyle.Bold | FontStyle.Italic, generator: Generator), Text = "Cursive Bold & Italic, 12pt" };
			yield return new DrawInfo { Font = Fonts.Fantasy(12, FontStyle.Bold | FontStyle.Italic, generator: Generator), Text = "Fantasy Bold & Italic, 12pt" };
		}

		internal void DrawFrame(Graphics g)
		{
			float y = 0;
			foreach (var info in GetDrawInfo())
			{
				var size = g.MeasureString(info.Font, info.Text);
				g.DrawText(info.Font, Colors.White, 10, y, info.Text);
				y += size.Height;
			}
		}
	}
}
