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
        private sd.Brush Brush { get; set; }

        private sd.StrokeStyle strokeStyle = null;
        private sd.StrokeStyle StrokeStyle
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

        private sd.DashStyle dashStyle = sd.DashStyle.Solid;
        public DashStyle DashStyle
        {
            get { return dashStyle.ToEto(); }
            set
            {
                dashStyle = value.ToWpf();
                strokeStyle = null; // invalidate
            }
        }

        public void Dispose()
        {
            if (Brush != null)
            {
                Brush.Dispose();
                Brush = null;
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
		PenData Control { get; set; }

        public Color Color
        {
            get { return Control.Color; }
            set { Control.Color = value; }
        }

        public void Create(Brush brush)
        {
            throw new NotImplementedException();
        }

        public void Create(Color color, float width, DashStyle dashStyle)
        {
            Control = new PenData
            {
                Color = color,
                Width = width,
                DashStyle = dashStyle,
            };
        }

        public float Width
        {
            get { return Control.Width; }
            set { Control.Width = value; }
        }

		public object Create(Color color, float thickness)
		{
			return new PenData { Color = color, Width = thickness };
		}

		public Color GetColor(Pen widget)
		{
			return widget.ToSD().Color;
		}

		public void SetColor(Pen widget, Color color)
		{
			widget.ToSD().Color = color;
		}

		public float GetThickness(Pen widget)
		{
			return widget.ToSD().Width;
		}

		public void SetThickness(Pen widget, float thickness)
		{
			widget.ToSD().Width = thickness;
		}

		public PenLineJoin GetLineJoin(Pen widget)
		{
			throw new NotImplementedException();
		}

		public void SetLineJoin(Pen widget, PenLineJoin lineJoin)
		{
			throw new NotImplementedException();
		}

		public PenLineCap GetLineCap(Pen widget)
		{
			throw new NotImplementedException();
		}

		public void SetLineCap(Pen widget, PenLineCap lineCap)
		{
			throw new NotImplementedException();
		}

		public float GetMiterLimit(Pen widget)
		{
			throw new NotImplementedException();
		}

		public void SetMiterLimit(Pen widget, float miterLimit)
		{
			throw new NotImplementedException();
		}

		public void SetDashStyle(Pen widget, DashStyle dashStyle)
		{
			throw new NotImplementedException();
		}
	}
}
