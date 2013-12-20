using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using s = SharpDX;
using sd = SharpDX.Direct2D1;
using Eto.Drawing;

namespace Eto.Platform.Direct2D.Drawing
{
    /// <summary>
    /// Combines a brush and a stroke style
    /// </summary>
    public class PenData : IDisposable
    {
		sd.Brush brush;
		public sd.Brush GetBrush(sd.RenderTarget target)
		{
			if (brush == null || !ReferenceEquals(brush.Tag, target))
			{
				if (brush != null)
					brush.Dispose();
				brush = new sd.SolidColorBrush(target, Color.ToDx()) { Tag = target };
			}
			return brush;
		}

        sd.StrokeStyle strokeStyle;
        public sd.StrokeStyle StrokeStyle
        {
            get
            {
                if (strokeStyle == null)
                {
                    strokeStyle = new sd.StrokeStyle(
                        SDFactory.D2D1Factory,
                        new sd.StrokeStyleProperties
                        {
                            DashStyle = sd.DashStyle.Solid,
                        });
                }

                return strokeStyle;
            }
        }

        public Color Color { get; set; }

        public float Width { get; set; }

        sd.DashStyle dashStyle = sd.DashStyle.Solid;
        public DashStyle DashStyle
        {
            get { return dashStyle.ToEto(); }
            set
            {
				dashStyle = value.ToDx();
                strokeStyle = null; // invalidate
            }
        }

        public void Dispose()
        {
			if (brush != null)
            {
				brush.Dispose();
                brush = null;
            }

            if (strokeStyle != null)
            {
                strokeStyle.Dispose();
                strokeStyle = null;
            }
        }
    }

    public class PenHandler : IPen
    {
		public object Create(Color color, float thickness)
		{
			return new PenData { Color = color, Width = thickness };
		}

		public Color GetColor(Pen widget)
		{
			return widget.ToPenData().Color;
		}

		public void SetColor(Pen widget, Color color)
		{
			widget.ToPenData().Color = color;
		}

		public float GetThickness(Pen widget)
		{
			return widget.ToPenData().Width;
		}

		public void SetThickness(Pen widget, float thickness)
		{
			widget.ToPenData().Width = thickness;
		}

		public PenLineJoin GetLineJoin(Pen widget)
		{
			return PenLineJoin.Bevel;
		}

		public void SetLineJoin(Pen widget, PenLineJoin lineJoin)
		{
			//throw new NotImplementedException();
		}

		public PenLineCap GetLineCap(Pen widget)
		{
			return PenLineCap.Round;
		}

		public void SetLineCap(Pen widget, PenLineCap lineCap)
		{
			//throw new NotImplementedException();
		}

		public float GetMiterLimit(Pen widget)
		{
			throw new NotImplementedException();
		}

		public void SetMiterLimit(Pen widget, float miterLimit)
		{
			//throw new NotImplementedException();
		}

		public void SetDashStyle(Pen widget, DashStyle dashStyle)
		{
			//throw new NotImplementedException();
		}
	}
}
