using System;
using Eto.Drawing;

namespace Eto.Platform.GtkSharp.Drawing
{
	/// <summary>
	/// Handler for the <see cref="ISolidBrush"/>
	/// </summary>
	/// <copyright>(c) 2012 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class SolidBrushHandler : BrushHandler, ISolidBrush
	{
		public override void Apply (object control, GraphicsHandler graphics)
		{
			graphics.Control.Color = (Cairo.Color)control;
			graphics.Control.Fill ();
		}

		public Color GetColor (SolidBrush widget)
		{
			return ((Cairo.Color)widget.ControlObject).ToEto ();
		}

		public void SetColor (SolidBrush widget, Color color)
		{
			widget.ControlObject = color.ToCairo ();
		}

		public object Create (Color color)
		{
			return color.ToCairo ();
		}
	}
}

