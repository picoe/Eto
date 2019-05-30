using Eto.Drawing;
using System;
using swm = System.Windows.Media;
using sw = System.Windows;

namespace Eto.Wpf.Drawing
{
	/// <summary>
	/// Handler for <see cref="IPen"/>
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class PenHandler : Pen.IHandler
	{
		static sw.DependencyProperty brushProp = sw.DependencyProperty.Register("brush", typeof(Brush), typeof(swm.Pen));

		public object Create (Brush brush, float thickness)
		{
			var pen = new swm.Pen (brush.ToWpf(), thickness);
			pen.SetValue(brushProp, brush);
			pen.EndLineCap = pen.StartLineCap = pen.DashCap = swm.PenLineCap.Square;
			pen.MiterLimit = 10f;
			return pen;
		}

		public Brush GetBrush (Pen widget)
		{
			var pen = (swm.Pen)widget.ControlObject;
			return pen.GetValue(brushProp) as Brush;
		}

		public float GetThickness (Pen widget)
		{
			return (float)((swm.Pen)widget.ControlObject).Thickness;
		}

		public void SetThickness (Pen widget, float thickness)
		{
			((swm.Pen)widget.ControlObject).Thickness = thickness;
		}

		public PenLineJoin GetLineJoin (Pen widget)
		{
			return ((swm.Pen)widget.ControlObject).LineJoin.ToEto ();
		}

		public void SetLineJoin (Pen widget, PenLineJoin lineJoin)
		{
			((swm.Pen)widget.ControlObject).LineJoin = lineJoin.ToWpf ();
		}

		public PenLineCap GetLineCap (Pen widget)
		{
			return ((swm.Pen)widget.ControlObject).StartLineCap.ToEto ();
		}

		public void SetLineCap (Pen widget, PenLineCap lineCap)
		{
			var swmpen = (swm.Pen)widget.ControlObject;
			swmpen.EndLineCap = swmpen.StartLineCap = swmpen.DashCap = lineCap.ToWpf ();
			SetDashStyle (widget, widget.DashStyle);
		}

		public float GetMiterLimit (Pen widget)
		{
			return (float)((swm.Pen)widget.ControlObject).MiterLimit;
		}

		public void SetMiterLimit (Pen widget, float miterLimit)
		{
			((swm.Pen)widget.ControlObject).MiterLimit = miterLimit;
		}

		public void SetDashStyle (Pen widget, DashStyle dashStyle)
		{
			var swmpen = (swm.Pen)widget.ControlObject;
			if (dashStyle == null || dashStyle.IsSolid)
				swmpen.DashStyle = swm.DashStyles.Solid;
			else {
				var dashes = dashStyle.Dashes;
				double[] wpfdashes;
				if (swmpen.DashCap == swm.PenLineCap.Flat)
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
				swmpen.DashStyle = new swm.DashStyle (wpfdashes, dashStyle.Offset);
			}
		}
	}
}
