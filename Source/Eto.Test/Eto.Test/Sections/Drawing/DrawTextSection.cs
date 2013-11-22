using System.Collections.Generic;
using Eto.Forms;
using Eto.Drawing;
using System;

namespace Eto.Test.Sections.Drawing
{
	public class DrawTextSection : Scrollable
	{
		DrawingToolkit toolkit;

		public DrawTextSection(): this(null)
		{
		}

		public DrawTextSection(DrawingToolkit toolkit)
		{
			this.toolkit = toolkit ?? new DrawingToolkit();

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
			var control = new Drawable { Size = new Size (400, 500), BackgroundColor = Colors.Black };
			toolkit.Initialize(control);
			var renderer = new DrawTextRenderer();
			control.Paint += (sender, e) => toolkit.Render(e.Graphics, renderer.DrawFrame);
			return control;
		}
	}

	public class DrawTextRenderer
	{
		class DrawInfo
		{
			public Font Font { get; set; }
			public string Text { get; set; }
		}

		static IEnumerable<DrawInfo> GetDrawInfo()
		{
			yield return new DrawInfo { Font = new Font(SystemFont.Default), Text = "System Font & Size" };
			yield return new DrawInfo { Font = new Font(SystemFont.Default, 20), Text = "System Font, 20pt" };

			yield return new DrawInfo { Font = Fonts.Sans(12), Text = "Sans, 12pt" };
			yield return new DrawInfo { Font = Fonts.Serif(12), Text = "Serif, 12pt" };
			yield return new DrawInfo { Font = Fonts.Monospace(12), Text = "Monospace, 12pt" };
			yield return new DrawInfo { Font = Fonts.Cursive(12), Text = "Cursive, 12pt" };
			yield return new DrawInfo { Font = Fonts.Fantasy(12), Text = "Fantasy, 12pt" };

			yield return new DrawInfo { Font = Fonts.Sans(12, FontStyle.Bold), Text = "Sans Bold, 12pt" };
			yield return new DrawInfo { Font = Fonts.Serif(12, FontStyle.Bold), Text = "Serif Bold, 12pt" };
			yield return new DrawInfo { Font = Fonts.Monospace(12, FontStyle.Bold), Text = "Monospace Bold, 12pt" };
			yield return new DrawInfo { Font = Fonts.Cursive(12, FontStyle.Bold), Text = "Cursive Bold, 12pt" };
			yield return new DrawInfo { Font = Fonts.Fantasy(12, FontStyle.Bold), Text = "Fantasy Bold, 12pt" };

			yield return new DrawInfo { Font = Fonts.Sans(12, FontStyle.Italic), Text = "Sans Italic, 12pt" };
			yield return new DrawInfo { Font = Fonts.Serif(12, FontStyle.Italic), Text = "Serif Italic, 12pt" };
			yield return new DrawInfo { Font = Fonts.Monospace(12, FontStyle.Italic), Text = "Monospace Italic, 12pt" };
			yield return new DrawInfo { Font = Fonts.Cursive(12, FontStyle.Italic), Text = "Cursive Italic, 12pt" };
			yield return new DrawInfo { Font = Fonts.Fantasy(12, FontStyle.Italic), Text = "Fantasy Italic, 12pt" };

			yield return new DrawInfo { Font = Fonts.Sans(12, FontStyle.Bold | FontStyle.Italic), Text = "Sans Bold & Italic, 12pt" };
			yield return new DrawInfo { Font = Fonts.Serif(12, FontStyle.Bold | FontStyle.Italic), Text = "Serif Bold & Italic, 12pt" };
			yield return new DrawInfo { Font = Fonts.Monospace(12, FontStyle.Bold | FontStyle.Italic), Text = "Monospace Bold & Italic, 12pt" };
			yield return new DrawInfo { Font = Fonts.Cursive(12, FontStyle.Bold | FontStyle.Italic), Text = "Cursive Bold & Italic, 12pt" };
			yield return new DrawInfo { Font = Fonts.Fantasy(12, FontStyle.Bold | FontStyle.Italic), Text = "Fantasy Bold & Italic, 12pt" };
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
