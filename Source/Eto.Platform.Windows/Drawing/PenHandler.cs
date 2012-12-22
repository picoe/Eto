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
	/// Pen handler
	/// </summary>
	/// <copyright>(c) 2012 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class PenHandler : IPenHandler
	{
		sd.Pen pen;
		DashStyle dashStyle;

		public void Create (Color color, float thickness)
		{
			pen = new sd.Pen (color.ToSD (), thickness);
			LineCap = PenLineCap.Square;
			pen.MiterLimit = 10f;
		}

		public Color Color
		{
			get { return pen.Color.ToEto (); }
			set { pen.Color = value.ToSD (); }
		}

		public float Thickness
		{
			get { return pen.Width; }
			set { pen.Width = value; }
		}

		public PenLineJoin LineJoin
		{
			get { return pen.LineJoin.ToEto (); }
			set { pen.LineJoin = value.ToSD (); }
		}

		public PenLineCap LineCap
		{
			get { return pen.StartCap.ToEto (); }
			set
			{
				pen.StartCap = pen.EndCap = value.ToSD ();
				pen.DashCap = value == PenLineCap.Round ? sd2.DashCap.Round : sd2.DashCap.Flat;
				SetDashStyle ();
			}
		}

		public float MiterLimit
		{
			get { return pen.MiterLimit; }
			set { pen.MiterLimit = value; }
		}

		public DashStyle DashStyle
		{
			get { return dashStyle; }
			set
			{
				dashStyle = value;
				SetDashStyle ();
			}
		}

		void SetDashStyle ()
		{
			if (dashStyle == null || dashStyle.IsSolid)
				pen.DashStyle = sd2.DashStyle.Solid;
			else {
				pen.DashStyle = sd2.DashStyle.Custom;
				pen.DashPattern = dashStyle.Dashes;
				pen.DashOffset = dashStyle.Offset;
				if (LineCap == PenLineCap.Square) {
					pen.DashOffset += 0.5f;
				}
			}
		}

		public object ControlObject
		{
			get { return pen; }
		}

		public void Dispose ()
		{
			pen.Dispose ();
		}
	}
}
