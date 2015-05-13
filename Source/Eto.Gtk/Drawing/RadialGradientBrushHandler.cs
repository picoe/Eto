using System;
using Eto.Drawing;
using System.Runtime.InteropServices;

namespace Eto.GtkSharp.Drawing
{
	/// <summary>
	/// Handler for <see cref="RadialGradientBrush"/>
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class RadialGradientBrushHandler : BrushHandler, RadialGradientBrush.IHandler
	{
		class EtoGradient : Cairo.RadialGradient
		{
			public SizeF Radius { get; set; }

			public PointF Center { get; set; }

			public PointF GradientOrigin { get; set; }

			public EtoGradient(PointF origin, PointF center, SizeF radius)
				: base(origin.X, origin.Y, 0, center.X, origin.Y - (origin.Y - center.Y) / (radius.Height / radius.Width), radius.Width)
			{
				
				Center = center;
				GradientOrigin = origin;
				Radius = radius;
			}

			public Cairo.Matrix Transform { get; set; }
		}

		public object Create(Color startColor, Color endColor, PointF center, PointF gradientOrigin, SizeF radius)
		{
			var gradient = new EtoGradient(gradientOrigin, center, radius);
			gradient.Radius = radius;
			gradient.Extend = Cairo.Extend.Pad;
			gradient.AddColorStop(0, startColor.ToCairo());
			gradient.AddColorStop(1, endColor.ToCairo());
			return gradient;
		}

		public IMatrix GetTransform(RadialGradientBrush widget)
		{
			return ((EtoGradient)widget.ControlObject).Transform.ToEto();
		}

		public void SetTransform(RadialGradientBrush widget, IMatrix transform)
		{
			((EtoGradient)widget.ControlObject).Transform = transform.ToCairo();
		}

		public GradientWrapMode GetGradientWrap(RadialGradientBrush widget)
		{
			var gradient = ((Cairo.RadialGradient)widget.ControlObject);
			return gradient.Extend.ToEto();
		}

		public void SetGradientWrap(RadialGradientBrush widget, GradientWrapMode gradientWrap)
		{
			var gradient = ((Cairo.RadialGradient)widget.ControlObject);
			gradient.Extend = gradientWrap.ToCairo();
		}

		public override void Apply(object control, GraphicsHandler graphics)
		{
			var gradient = ((EtoGradient)control);
			var scale = gradient.Radius.Height / gradient.Radius.Width;
			if (!object.ReferenceEquals(gradient.Transform, null))
				graphics.Control.Transform(gradient.Transform);
			graphics.Control.Transform(new Cairo.Matrix(1, 0f, 0f, scale, 0, gradient.GradientOrigin.Y - gradient.GradientOrigin.Y * scale));
			graphics.Control.Pattern = gradient;
			graphics.Control.Fill();
		}
	}
}

