using Eto.Drawing;

#if OSX
using MonoMac.CoreGraphics;

namespace Eto.Mac.Drawing
#elif IOS
using MonoTouch.CoreGraphics;

namespace Eto.iOS.Drawing
#endif
{
	/// <summary>
	/// Handler for <see cref="ISolidBrush"/>
	/// </summary>
	/// <copyright>(c) 2012 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class SolidBrushHandler : BrushHandler, ISolidBrush
	{
		public override void Apply (object control, GraphicsHandler graphics)
		{
			graphics.Control.SetFillColor ((CGColor)control);
		}

		public Color GetColor (SolidBrush widget)
		{
			return ((CGColor)widget.ControlObject).ToEtoColor ();
		}

		public void SetColor (SolidBrush widget, Color color)
		{
			widget.ControlObject = color.ToCGColor ();
		}

		object ISolidBrush.Create (Color color)
		{
			return color.ToCGColor ();
		}
	}
}

