using Eto.Drawing;
using sd = SharpDX.Direct2D1;

namespace Eto.Platform.Direct2D.Drawing
{
	/// <summary>
	/// Handler for <see cref="ISolidBrush"/>
	/// </summary>
	/// <copyright>(c) 2013 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class SolidBrushHandler : BrushHandler, ISolidBrush
	{
		public new object Create(Color color)
		{
			return BrushHandler.CreateBrush(ref color);
		}

		public Color GetColor(SolidBrush widget)
		{
			return ((sd.SolidColorBrush)widget.ControlObject).Color.ToEto();
		}

		public void SetColor(SolidBrush widget, Color color)
		{
			((sd.SolidColorBrush)widget.ControlObject).Color = color.ToSD();
		}
	}
}
