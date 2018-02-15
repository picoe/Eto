using Eto.Drawing;
using System;
using sd = System.Drawing;
using sd2 = System.Drawing.Drawing2D;

namespace Eto.WinForms.Drawing
{
	/// <summary>
	/// Handler for <see cref="Pen"/>
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class PenHandler : Pen.IHandler
	{
		public class EtoPen
		{
			public sd.Pen Pen { get; set; }
			public Brush Brush { get; set; }
		}

		public object Create(Brush brush, float thickness)
		{
			var pen = new sd.Pen(brush.ToSD(Rectangle.Empty), thickness);
			pen.StartCap = pen.EndCap = PenLineCap.Square.ToSD();
			pen.DashCap = sd2.DashCap.Flat;
			pen.MiterLimit = 10f;
			return new EtoPen { Pen = pen, Brush = brush };
		}

		public Brush GetBrush(Pen widget)
		{
			return ((EtoPen)widget.ControlObject).Brush;
		}

		public void SetColor(Pen widget, Color color)
		{
			GetPen(widget).Color = color.ToSD();
		}

		public float GetThickness(Pen widget)
		{
			return GetPen(widget).Width;
		}

		public void SetThickness(Pen widget, float thickness)
		{
			GetPen(widget).Width = thickness;
		}

		public PenLineJoin GetLineJoin(Pen widget)
		{
			return GetPen(widget).LineJoin.ToEto();
		}

		public void SetLineJoin(Pen widget, PenLineJoin lineJoin)
		{
			GetPen(widget).LineJoin = lineJoin.ToSD();
		}

		public PenLineCap GetLineCap(Pen widget)
		{
			return GetPen(widget).StartCap.ToEto();
		}

		public void SetLineCap(Pen widget, PenLineCap lineCap)
		{
			var pen = GetPen(widget);
			// get dash style before changing cap
			var dashStyle = widget.DashStyle;
			pen.StartCap = pen.EndCap = lineCap.ToSD();
			pen.DashCap = lineCap == PenLineCap.Round ? sd2.DashCap.Round : sd2.DashCap.Flat;
			SetDashStyle(widget, dashStyle);
		}

		public float GetMiterLimit(Pen widget)
		{
			return GetPen(widget).MiterLimit;
		}

		public void SetMiterLimit(Pen widget, float miterLimit)
		{
			GetPen(widget).MiterLimit = miterLimit;
		}

		public DashStyle GetDashStyle(Pen widget)
		{
			var pen = GetPen(widget);
			if (pen.DashStyle == sd2.DashStyle.Solid)
				return DashStyles.Solid;
			else
			{
				var offset = pen.StartCap == sd2.LineCap.Square ? pen.DashOffset - 0.5f : pen.DashOffset;
				return new DashStyle(offset, pen.DashPattern);
			}
		}

		public void SetDashStyle(Pen widget, DashStyle dashStyle)
		{
			var pen = GetPen(widget);

			pen.DashOffset = 0;
			if (dashStyle == null || dashStyle.IsSolid)
				pen.DashStyle = sd2.DashStyle.Solid;
			else if (dashStyle.Equals(DashStyles.Dash))
				pen.DashStyle = sd2.DashStyle.Dash;
			else if (dashStyle.Equals(DashStyles.DashDot))
				pen.DashStyle = sd2.DashStyle.DashDot;
			else if (dashStyle.Equals(DashStyles.DashDotDot))
				pen.DashStyle = sd2.DashStyle.DashDotDot;
			else
			{
				pen.DashStyle = sd2.DashStyle.Custom;
				pen.DashPattern = dashStyle.Dashes;
				pen.DashOffset = dashStyle.Offset;
			}
			if (pen.StartCap == sd2.LineCap.Square)
			{
				pen.DashOffset += 0.5f;
			}
		}

		public sd.Pen GetPen(Pen pen, RectangleF bounds)
		{
			var etoPen = (EtoPen)pen.ControlObject;
			bounds.Inflate(etoPen.Pen.Width, etoPen.Pen.Width);
			etoPen.Pen.Brush = etoPen.Brush.ToSD(bounds); // extend brush to bounds if needed
			return etoPen.Pen;
		}

		sd.Pen GetPen(Pen pen)
		{
			return ((EtoPen)pen.ControlObject).Pen;
		}
	}
}
