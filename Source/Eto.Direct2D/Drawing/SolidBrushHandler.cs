using Eto.Drawing;
using sd = SharpDX.Direct2D1;

namespace Eto.Direct2D.Drawing
{
	/// <summary>
	/// Handler for <see cref="ISolidBrush"/>
	/// </summary>
	/// <copyright>(c) 2013 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class SolidBrushHandler : SolidBrush.IHandler
	{
		public class SolidBrushData : BrushData
		{
			public Color Color { get; set; }

			protected override sd.Brush Create(sd.RenderTarget target)
			{
				return new sd.SolidColorBrush(target, Color.ToDx());
			}
		}

		public object Create(Color color)
		{
			return new SolidBrushData { Color = color };
		}

		public Color GetColor(SolidBrush widget)
		{
			return ((SolidBrushData)widget.ControlObject).Color;
		}

		public void SetColor(SolidBrush widget, Color color)
		{
			var brush = ((SolidBrushData)widget.ControlObject);
			brush.Reset();
			brush.Color = color;
		}
	}
}
