using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Sections.Drawing
{
#if Windows
	public class DrawTextSectionD2D : Drawable
	{
		public DrawTextSectionD2D()
		{
			var renderer = new DrawTextRenderer();
		
			var d2d = Generator.GetGenerator(Generators.Direct2DAssembly);
			var graphics = new Graphics(this, d2d);
			this.Paint += (s, e) => {
				graphics.BeginDrawing();
				//graphics.Clear(Brushes.Black() as SolidBrush); // DirectDrawingSection's Drawable seems to automatically clear the background, but that doesn't happen in Direct2d, so we clear it explicitly.
				try {
					using (var context = new GeneratorContext(d2d))
						renderer.DrawFrame(graphics);
				} catch (Exception) { }
				graphics.EndDrawing();
			};
		}
	}
#endif
}
