#if OSX

namespace Eto.Mac.Drawing
#elif IOS
using CoreGraphics;

namespace Eto.iOS.Drawing
#endif
{
	/// <summary>
	/// Handler for <see cref="SolidBrush"/>
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class SolidBrushHandler : BrushHandler, SolidBrush.IHandler
	{
		struct BrushData
		{
			public Color Color;
			public NSColor NSColor;
			public BrushData(Color color)
			{
				Color = color;
				NSColor = color.ToNSUI();
			}
		}

		public override void Draw(object control, GraphicsHandler graphics, bool stroke, FillMode fillMode, bool clip)
		{
			var nscolor = ((BrushData)control).NSColor;
			if (stroke)
			{
				nscolor.SetStroke();
				graphics.Control.StrokePath();
			}
			else
			{
				nscolor.SetFill();
				graphics.Fill(fillMode);
			}
		}

		public Color GetColor(SolidBrush widget)
		{
			return ((BrushData)widget.ControlObject).Color;
		}

		public void SetColor(SolidBrush widget, Color color)
		{
			widget.ControlObject = new BrushData(color);
		}

		object SolidBrush.IHandler.Create(Color color)
		{
			return new BrushData(color);
		}
	}
}

