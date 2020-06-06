using System;
using Eto.Drawing;
using System.Runtime.InteropServices;

namespace Eto.GtkSharp.Drawing
{
	/// <summary>
	/// Handler for <see cref="LinearGradientBrush"/>
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class LinearGradientBrushHandler : BrushHandler, LinearGradientBrush.IHandler
	{
		class EtoGradient : Cairo.LinearGradient
		{
			public EtoGradient(double x0, double y0, double x1, double y1)
				: base(x0, y0, x1, y1)
			{
			}

			public Cairo.Matrix Transform { get; set; }
			public Cairo.Matrix TransformInverse { get; set; }
		}

		public object Create(Color startColor, Color endColor, PointF startPoint, PointF endPoint)
		{
			var gradient = new EtoGradient(startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);
			gradient.Extend = Cairo.Extend.Pad;
			gradient.AddColorStop(0, startColor.ToCairo());
			gradient.AddColorStop(1, endColor.ToCairo());
			return gradient;
		}

		public object Create(RectangleF rectangle, Color startColor, Color endColor, float angle)
		{
			GradientHelper.GetLinearFromRectangle(rectangle, angle, out var startPoint, out var endPoint);
			return Create(startColor, endColor, startPoint, endPoint);
		}

		public IMatrix GetTransform(LinearGradientBrush widget)
		{
			return ((EtoGradient)widget.ControlObject).Transform.ToEto() ?? Matrix.Create();
		}

		public void SetTransform(LinearGradientBrush widget, IMatrix transform)
		{
			((EtoGradient)widget.ControlObject).Transform = transform.ToCairo();
			((EtoGradient)widget.ControlObject).TransformInverse = transform.Inverse().ToCairo();
		}

		public GradientWrapMode GetGradientWrap(LinearGradientBrush widget)
		{
			return ((Cairo.LinearGradient)widget.ControlObject).Extend.ToEto();
		}

		public void SetGradientWrap(LinearGradientBrush widget, GradientWrapMode gradientWrap)
		{
			var gradient = ((Cairo.LinearGradient)widget.ControlObject);
			gradient.Extend = gradientWrap.ToCairo();
		}

		public override void Apply(object control, GraphicsHandler graphics)
		{
			var gradient = ((EtoGradient)control);
			if (!ReferenceEquals(gradient.Transform, null))
				graphics.Control.Transform(gradient.Transform);
			graphics.Control.SetSource(gradient);
			if (!ReferenceEquals(gradient.TransformInverse, null))
				graphics.Control.Transform(gradient.TransformInverse);
		}
	}
}

