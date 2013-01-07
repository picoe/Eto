using System;
using Eto.Drawing;
using System.Runtime.InteropServices;

namespace Eto.Platform.GtkSharp.Drawing
{
	/// <summary>
	/// Handler for <see cref="ILinearGradientBrush"/>
	/// </summary>
	/// <copyright>(c) 2012 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class LinearGradientBrushHandler : BrushHandler, ILinearGradientBrush
	{
		[DllImport ("libcairo-2.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern Cairo.Extend cairo_pattern_get_extend (IntPtr pattern);

		[DllImport ("libcairo-2.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void cairo_pattern_set_extend (IntPtr pattern, Cairo.Extend extend);

		class EtoGradient : Cairo.LinearGradient
		{
			public EtoGradient (double x0, double y0, double x1, double y1)
				: base (x0, y0, x1, y1)
			{
			}

			public Cairo.Matrix Transform { get; set; }
		}

		public object Create (Color startColor, Color endColor, PointF startPoint, PointF endPoint)
		{
			var gradient = new EtoGradient (startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);
			cairo_pattern_set_extend (gradient.Pointer, Cairo.Extend.Repeat);
			// not in windows?? gradient.Extend = Cairo.Extend.Repeat;
			gradient.AddColorStop (0, startColor.ToCairo ());
			gradient.AddColorStop (1, endColor.ToCairo ());
			return gradient;
		}

		public object Create (RectangleF rectangle, Color startColor, Color endColor, float angle)
		{
			throw new NotImplementedException ();
		}

		public IMatrix GetTransform (LinearGradientBrush widget)
		{
			return ((EtoGradient)widget.ControlObject).Transform.ToEto ();
		}

		public void SetTransform (LinearGradientBrush widget, IMatrix transform)
		{
			((EtoGradient)widget.ControlObject).Transform = transform.ToCairo ();
		}

		public GradientWrapMode GetGradientWrap (LinearGradientBrush widget)
		{
			var gradient = ((Cairo.LinearGradient)widget.ControlObject);
			// return gradient.Extend.ToEto ();
			return cairo_pattern_get_extend (gradient.Pointer).ToEto ();
		}

		public void SetGradientWrap (LinearGradientBrush widget, GradientWrapMode gradientWrap)
		{
			var gradient = ((Cairo.LinearGradient)widget.ControlObject);
			// gradient.Extend = gradientWrap.ToCairo ();
			cairo_pattern_set_extend (gradient.Pointer, gradientWrap.ToCairo ());
		}

		public override void Apply (object control, GraphicsHandler graphics)
		{
			var gradient = ((EtoGradient)control);
			graphics.Control.Transform (gradient.Transform);
			graphics.Control.Pattern = gradient;
			graphics.Control.Fill ();
		}
	}
}

