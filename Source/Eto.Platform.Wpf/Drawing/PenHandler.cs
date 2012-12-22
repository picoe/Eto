using Eto.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swm = System.Windows.Media;

namespace Eto.Platform.Wpf.Drawing
{
	/// <summary>
	/// Pen handler
	/// </summary>
	/// <copyright>(c) 2012 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class PenHandler : IPenHandler
	{
		swm.Pen pen;
		swm.SolidColorBrush brush;
		DashStyle dashStyle;

		public void Create (Color color, float thickness)
		{
			brush = new swm.SolidColorBrush (color.ToWpf ());
			pen = new swm.Pen (brush, thickness);
			LineCap = PenLineCap.Square;
			pen.MiterLimit = 10f;
		}

		public Color Color
		{
			get { return brush.Color.ToEto (); }
			set { brush.Color = value.ToWpf (); }
		}

		public float Thickness
		{
			get { return (float)pen.Thickness; }
			set { pen.Thickness = value; }
		}

		public PenLineJoin LineJoin
		{
			get { return pen.LineJoin.ToEto (); }
			set { pen.LineJoin = value.ToWpf (); }
		}

		public PenLineCap LineCap
		{
			get { return pen.EndLineCap.ToEto (); }
			set
			{
				pen.EndLineCap = pen.StartLineCap = pen.DashCap = value.ToWpf ();
				SetDashStyle ();
			}
		}

		public float MiterLimit
		{
			get { return (float)pen.MiterLimit; }
			set { pen.MiterLimit = value; }
		}

		public object ControlObject
		{
			get { return pen; }
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
				pen.DashStyle = swm.DashStyles.Solid;
			else {
				var dashes = dashStyle.Dashes;
				double[] wpfdashes;
				if (pen.DashCap == swm.PenLineCap.Flat)
					wpfdashes = Array.ConvertAll (dashStyle.Dashes, x => (double)x);
				else {
					wpfdashes = new double[dashes.Length];
					for (int i = 0; i < wpfdashes.Length; i++) {
						var dash = (double)dashes[i];
						if ((i % 2) == 1) {
							// gap must include square/round thickness
							dash += 1;
						} else {
							// dash must exclude square/round thickness
							dash -= 1;
						}
						wpfdashes[i] = dash;
					}
				}
				pen.DashStyle = new swm.DashStyle (wpfdashes, dashStyle.Offset);
			}
		}

		public void Dispose ()
		{
		}
	}
}
