using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Sections.Drawing
{
#if Windows
	public class Direct2DSection : Drawable
	{
		public Direct2DSection()
		{			
			var d2d = Generator.GetGenerator(Generators.Direct2DAssembly);

			using (var context = new GeneratorContext(d2d))
			{
				var graphics = new Graphics(this);

				this.Paint += (s, e) => {
					graphics.BeginDrawing();
					graphics.Clear(Brushes.Black() as SolidBrush);
					graphics.DrawLine(Colors.Blue, new PointF(0, 0), new PointF(100, 100));
					graphics.EndDrawing();
				};
			}
		}
	}
#endif
}
