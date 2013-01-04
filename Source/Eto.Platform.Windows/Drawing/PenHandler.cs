using Eto.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sd = System.Drawing;
using sd2 = System.Drawing.Drawing2D;

namespace Eto.Platform.Windows.Drawing
{
	/// <summary>
	/// Handler for <see cref="IPen"/>
	/// </summary>
	/// <copyright>(c) 2012 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class PenHandler : IPen
	{
		public object Create (Color color, float thickness)
		{
			var pen = new sd.Pen (color.ToSD (), thickness);
			pen.StartCap = pen.EndCap = PenLineCap.Square.ToSD ();
			pen.DashCap = sd2.DashCap.Flat;
			pen.MiterLimit = 10f;
			return pen;
		}

		public Color GetColor (Pen widget)
		{
			return widget.ToSD ().Color.ToEto ();
		}

		public void SetColor (Pen widget, Color color)
		{
			widget.ToSD ().Color = color.ToSD ();
		}

		public float GetThickness (Pen widget)
		{
			return widget.ToSD ().Width;
		}

		public void SetThickness (Pen widget, float thickness)
		{
			widget.ToSD ().Width = thickness;
		}

		public PenLineJoin GetLineJoin (Pen widget)
		{
			return widget.ToSD ().LineJoin.ToEto ();
		}

		public void SetLineJoin (Pen widget, PenLineJoin lineJoin)
		{
			widget.ToSD ().LineJoin = lineJoin.ToSD ();
		}

		public PenLineCap GetLineCap (Pen widget)
		{
			return widget.ToSD ().StartCap.ToEto ();
		}

		public void SetLineCap (Pen widget, PenLineCap lineCap)
		{
			var pen = widget.ToSD ();
			pen.StartCap = pen.EndCap = lineCap.ToSD ();
			pen.DashCap = lineCap == PenLineCap.Round ? sd2.DashCap.Round : sd2.DashCap.Flat;
			SetDashStyle (widget, widget.DashStyle);
		}

		public float GetMiterLimit (Pen widget)
		{
			return widget.ToSD ().MiterLimit;
		}

		public void SetMiterLimit (Pen widget, float miterLimit)
		{
			widget.ToSD ().MiterLimit = miterLimit;
		}

		public DashStyle GetDashStyle (Pen widget)
		{
			var pen = widget.ToSD ();
			if (pen.DashStyle == sd2.DashStyle.Solid)
				return DashStyles.Solid;
			else {
				var offset = pen.StartCap == sd2.LineCap.Square ? pen.DashOffset - 0.5f : pen.DashOffset;
				return new DashStyle (offset, pen.DashPattern);
			}
		}

		public void SetDashStyle (Pen widget, DashStyle dashStyle)
		{
			var pen = widget.ToSD ();

			if (dashStyle == null || dashStyle.IsSolid)
				pen.DashStyle = sd2.DashStyle.Solid;
			else {
				pen.DashStyle = sd2.DashStyle.Custom;
				pen.DashPattern = dashStyle.Dashes;
				pen.DashOffset = dashStyle.Offset;
				if (pen.StartCap == sd2.LineCap.Square) {
					pen.DashOffset += 0.5f;
				}
			}
		}
	}
}
