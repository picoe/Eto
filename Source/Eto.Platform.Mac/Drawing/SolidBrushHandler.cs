using System;
using Eto.Drawing;

#if OSX
using MonoMac.CoreGraphics;

namespace Eto.Platform.Mac.Drawing
#elif IOS
using MonoTouch.CoreGraphics;

namespace Eto.Platform.iOS.Drawing
#endif
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
			graphics.Control.SetFillColor (Color.ToCGColor ());
		}
	}
}

