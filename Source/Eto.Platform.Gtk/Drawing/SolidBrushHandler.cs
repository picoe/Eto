using System;
using Eto.Drawing;

namespace Eto.Platform.GtkSharp.Drawing
{
	public class SolidBrushHandler : BrushHandler, ISolidBrushHandler
	{
		public Color Color { get; set; }

		public void Create (Color color)
		{
			this.Color = color;
		}

		public override void Apply (GraphicsHandler graphics)
		{
			graphics.Control.Color = Color.ToCairo ();
		}
	}
}

