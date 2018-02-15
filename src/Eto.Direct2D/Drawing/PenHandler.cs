using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using s = SharpDX;
using sd = SharpDX.Direct2D1;
using Eto.Drawing;

namespace Eto.Direct2D.Drawing
{
	/// <summary>
	/// Combines a brush and a stroke style
	/// </summary>
	public class PenData : IDisposable
	{
		public sd.Brush GetBrush(sd.RenderTarget target)
		{
			return Brush.ToDx(target);
		}

		sd.StrokeStyle strokeStyle;
		public sd.StrokeStyle StrokeStyle
		{
			get
			{
				if (strokeStyle == null)
				{
					var properties = new sd.StrokeStyleProperties
					{
						DashStyle = dashStyle.ToDx(),
						LineJoin = lineJoin.ToDx(),
						MiterLimit = miterLimit,
						DashCap = lineCap == PenLineCap.Round ? sd.CapStyle.Round : sd.CapStyle.Square,
						DashOffset = lineCap == PenLineCap.Butt ? -.5f + dashStyle.Offset : dashStyle.Offset
					};
					properties.StartCap = properties.EndCap = lineCap.ToDx();
					if (properties.DashStyle == sd.DashStyle.Custom)
						strokeStyle = new sd.StrokeStyle(SDFactory.D2D1Factory, properties, dashStyle.Dashes);
					else
						strokeStyle = new sd.StrokeStyle(SDFactory.D2D1Factory, properties);
				}

				return strokeStyle;
			}
		}

		void Reset()
		{
			if (strokeStyle != null)
			{
				strokeStyle.Dispose();
				strokeStyle = null;
			}
		}

		float miterLimit = 10f;
		public float MiterLimit
		{
			get { return miterLimit; }
			set
			{
				miterLimit = value;
				Reset();
			}
		}

		public Brush Brush { get; set; }

		public float Width { get; set; }

		DashStyle dashStyle = DashStyles.Solid;
		public DashStyle DashStyle
		{
			get { return dashStyle; }
			set
			{
				dashStyle = value ?? DashStyles.Solid;
				Reset();
			}
		}

		PenLineCap lineCap;
		public PenLineCap LineCap
		{
			get { return lineCap; }
			set
			{
				lineCap = value;
				Reset();
			}
		}

		PenLineJoin lineJoin;
		public PenLineJoin LineJoin
		{
			get { return lineJoin; }
			set
			{
				lineJoin = value;
				Reset();
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

	public class PenHandler : Pen.IHandler
	{
		public object Create(Brush brush, float thickness)
		{
			return new PenData { Brush = brush, Width = thickness };
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
			return widget.ToPenData().LineJoin;
		}

		public void SetLineJoin(Pen widget, PenLineJoin lineJoin)
		{
			widget.ToPenData().LineJoin = lineJoin;
		}

		public PenLineCap GetLineCap(Pen widget)
		{
			return widget.ToPenData().LineCap;
		}

		public void SetLineCap(Pen widget, PenLineCap lineCap)
		{
			widget.ToPenData().LineCap = lineCap;
		}

		public float GetMiterLimit(Pen widget)
		{
			return widget.ToPenData().MiterLimit;
		}

		public void SetMiterLimit(Pen widget, float miterLimit)
		{
			widget.ToPenData().MiterLimit = miterLimit;
		}

		public void SetDashStyle(Pen widget, DashStyle dashStyle)
		{
			widget.ToPenData().DashStyle = dashStyle;
		}

		public Brush GetBrush(Pen widget)
		{
			return widget.ToPenData().Brush;
		}
	}
}
