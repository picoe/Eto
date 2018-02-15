using Eto.Drawing;
using sd = System.Drawing;

namespace Eto.WinForms.Drawing
{
	/// <summary>
	/// Handler for <see cref="SolidBrush"/>
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class SolidBrushHandler : BrushHandler, SolidBrush.IHandler
	{
		public object Create(Color color)
		{
			return new sd.SolidBrush(color.ToSD());
		}

		public Color GetColor(SolidBrush widget)
		{
			return ((sd.SolidBrush)widget.ControlObject).Color.ToEto();
		}

		public void SetColor(SolidBrush widget, Color color)
		{
			((sd.SolidBrush)widget.ControlObject).Color = color.ToSD();
		}

		public override sd.Brush GetBrush(Brush brush, RectangleF rect)
		{
			return (sd.Brush)brush.ControlObject;
		}
	}
}
