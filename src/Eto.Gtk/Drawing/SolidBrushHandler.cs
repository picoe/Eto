using Eto.Drawing;

namespace Eto.GtkSharp.Drawing
{
	/// <summary>
	/// Handler for the <see cref="SolidBrush"/>
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class SolidBrushHandler : BrushHandler, SolidBrush.IHandler
	{
		public override void Apply(object control, GraphicsHandler graphics)
		{
			graphics.Control.SetSourceColor((Cairo.Color)control);
		}

		public Color GetColor(SolidBrush widget)
		{
			return ((Cairo.Color)widget.ControlObject).ToEto();
		}

		public void SetColor(SolidBrush widget, Color color)
		{
			widget.ControlObject = color.ToCairo();
		}

		public object Create(Color color)
		{
			return color.ToCairo();
		}
	}
}

