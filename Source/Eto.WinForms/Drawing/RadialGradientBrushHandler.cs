using Eto.Drawing;
using System.Collections.Generic;
using System.Linq;
using sd = System.Drawing;
using sd2 = System.Drawing.Drawing2D;

namespace Eto.WinForms.Drawing
{
	public class RadialGradientBrushHandler : BrushHandler, RadialGradientBrush.IHandler
	{
		class BrushObject
		{
			sd2.PathGradientBrush brush;
			float lastScale;

			IMatrix matrix;
			public IMatrix Matrix
			{
				get { return matrix; }
				set
				{
					matrix = value;
					brush = null;
				}
			}

			GradientWrapMode wrapMode;
			public GradientWrapMode WrapMode
			{
				get { return wrapMode; }
				set
				{
					wrapMode = value;
					brush = null;
				}
			}

			public PointF Center { get; set; }
			public SizeF Radius { get; set; }

			public Color StartColor { get;set;}
			public Color EndColor { get;set;}
			public PointF GradientOrigin {get;set;}

			public sd2.PathGradientBrush GetBrush(RectangleF rect)
			{
				var scale = 1f;
				var bounds = rect;
				if (Matrix != null)
				{
					bounds = Matrix.Inverse().TransformRectangle(bounds);
				}

				scale = GradientHelper.GetRadialScale(Center, Radius, GradientOrigin, bounds);

				if (brush == null || lastScale != scale)
				{
					lastScale = scale;

					var scaledRect = new RectangleF(GradientOrigin - (GradientOrigin - Center + Radius) * scale, GradientOrigin + (Center + Radius - GradientOrigin) * scale);

					var path = new sd2.GraphicsPath();
					path.AddEllipse(scaledRect.ToSD());

					brush = new sd2.PathGradientBrush(path);
					brush.CenterColor = StartColor.ToSD();
					brush.CenterPoint = GradientOrigin.ToSD();
					brush.WrapMode = wrapMode.ToSD();
					brush.SurroundColors = new[] { EndColor.ToSD() };

					if (Matrix != null)
						brush.MultiplyTransform(Matrix.ToSD());

					if (scale > 1f)
					{
						var paths = GradientHelper.GetGradientStops(StartColor.ToSD(), EndColor.ToSD(), scale, wrapMode);

						brush.InterpolationColors = new sd2.ColorBlend
						{
							Positions = paths.Reverse().Select(r => 1f - r.Item1).ToArray(),
							Colors = paths.Reverse().Select(r => r.Item2).ToArray()
						};
					}
				}

				return brush;
			}

		}

		public object Create(Color startColor, Color endColor, PointF center, PointF gradientOrigin, SizeF radius)
		{
			return new BrushObject {
				StartColor = startColor,
				EndColor = endColor,
				Center = center,
				GradientOrigin = gradientOrigin,
				Radius = radius
			};
		}

		public IMatrix GetTransform(RadialGradientBrush widget)
		{
			return ((BrushObject)widget.ControlObject).Matrix;
		}

		public void SetTransform(RadialGradientBrush widget, IMatrix transform)
		{
			var brush = ((BrushObject)widget.ControlObject);
			brush.Matrix = transform;
		}

		public GradientWrapMode GetGradientWrap(RadialGradientBrush widget)
		{
			return ((BrushObject)widget.ControlObject).WrapMode;
		}

		public void SetGradientWrap(RadialGradientBrush widget, GradientWrapMode gradientWrap)
		{
			((BrushObject)widget.ControlObject).WrapMode = gradientWrap;
		}

		public override sd.Brush GetBrush(Brush brush, RectangleF rect)
		{
			return ((BrushObject)brush.ControlObject).GetBrush(rect);
		}
	}
}
