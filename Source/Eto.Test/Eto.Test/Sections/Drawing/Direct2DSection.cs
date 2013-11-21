using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Sections.Drawing
{
#if Windows
	public class Direct2DSection : Drawable
	{
		public Direct2DSection()
		{
			var renderer = new DirectDrawingRenderer();
		
			var d2d = Generator.GetGenerator(Generators.Direct2DAssembly);
			var graphics = new Graphics(this, d2d);
			this.Paint += (s, e) => {
				using (var context = new GeneratorContext(d2d))
				{
					graphics.BeginDrawing();
					//graphics.Clear(Brushes.Black() as SolidBrush); // DirectDrawingSection's Drawable seems to automatically clear the background, but that doesn't happen in Direct2d, so we clear it explicitly.
					renderer.DrawFrame(graphics, this.Size);
					graphics.EndDrawing();
					this.Invalidate(); // loop
				}
			};
		}
	}
#endif
}
