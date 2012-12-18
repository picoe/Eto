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
	public class PenHandler : IPenHandler
	{
		CGColor cgcolor;

		public Color Color { get; set; }

		public float Thickness { get; set; }

		public PenLineJoin LineJoin { get; set; }

		public PenLineCap LineCap { get; set; }

		public void Create (Color color, float thickness)
		{
			this.Color = color;
			this.Thickness = thickness;
		}

		public void Dispose ()
		{
		}

		public object ControlObject { get { return this; } }

		public void Apply (GraphicsHandler graphics)
		{
			cgcolor = cgcolor ?? Color.ToCGColor ();
			graphics.Control.SetStrokeColor (cgcolor);
			graphics.Control.SetLineCap (LineCap.ToCG ());
			graphics.Control.SetLineJoin (LineJoin.ToCG ());
			graphics.Control.SetLineWidth (Thickness);
		}
	}
}

