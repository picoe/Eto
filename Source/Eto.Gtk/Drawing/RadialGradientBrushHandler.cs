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
			SizeF radius;
			public SizeF Radius
			{
				get { return radius; }
				set
				{
					radius = value;
					SetRenderTransform();
				}
			}

			public PointF Center { get; set; }

			PointF gradientOrigin;
			public PointF GradientOrigin
			{
				get { return gradientOrigin; }
				set
				{
					gradientOrigin = value;
					SetRenderTransform();
				}
			}

			public EtoGradient(PointF origin, PointF center, SizeF radius)
				: base(origin.X, origin.Y, 0, center.X, origin.Y - (origin.Y - center.Y) / (radius.Height / radius.Width), radius.Width)
			{
				
				Center = center;
				gradientOrigin = origin;
				this.radius = radius;
				SetRenderTransform();
			}

			Cairo.Matrix transform;
			public Cairo.Matrix Transform
			{
				get { return transform; }
				set
				{
					transform = value;
					SetRenderTransform();
				}
			}

			public Cairo.Matrix RenderTransform { get; set; }
			public Cairo.Matrix RenderTransformInverse { get; set; }

			void SetRenderTransform()
			{
				var scale = Radius.Height / Radius.Width;
				RenderTransform = new Cairo.Matrix(1, 0f, 0f, scale, 0, GradientOrigin.Y - GradientOrigin.Y * scale);
				if (!ReferenceEquals(Transform, null))
					RenderTransform = Cairo.Matrix.Multiply(Transform, RenderTransform);
				
				RenderTransformInverse = (Cairo.Matrix)RenderTransform.Clone();
				RenderTransformInverse.Invert();
			}
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
			return ((EtoGradient)widget.ControlObject).Transform.ToEto() ?? Matrix.Create();
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
			graphics.Control.Transform(gradient.RenderTransform);
			graphics.Control.SetSource(gradient);
			graphics.Control.Transform(gradient.RenderTransformInverse);
		}
	}
}

