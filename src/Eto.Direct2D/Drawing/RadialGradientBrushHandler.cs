using Eto.Drawing;
using sd = SharpDX.Direct2D1;
using s = SharpDX;

namespace Eto.Direct2D.Drawing
{
	public class RadialGradientBrushHandler : RadialGradientBrush.IHandler
	{
		public class RadialBrushData : TransformBrushData
		{
			public PointF Center { get; set; }
			public PointF GradientOrigin { get; set; }
			public SizeF Radius { get; set; }
			public Color StartColor { get; set; }
			public Color EndColor { get; set; }
			public GradientWrapMode WrapMode { get; set; }

			protected override sd.Brush Create(sd.RenderTarget target)
			{
				return new sd.RadialGradientBrush(
					target,
					new sd.RadialGradientBrushProperties
					{ 
						Center = Center.ToDx(),
						GradientOriginOffset = (GradientOrigin - Center).ToDx(),
						RadiusX = Radius.Width,
						RadiusY = Radius.Height
					},
					new sd.GradientStopCollection(GraphicsHandler.CurrentRenderTarget, new[] {
					new sd.GradientStop { Color = StartColor.ToDx(), Position = 0f }, 
					new sd.GradientStop { Color = EndColor.ToDx(), Position = 1f }
				}, WrapMode.ToDx())
				);
			}
		}

		public IMatrix GetTransform(RadialGradientBrush widget)
		{
			var brush = (RadialBrushData)widget.ControlObject;
			return brush.Transform;
		}

		public void SetTransform(RadialGradientBrush widget, IMatrix transform)
		{
			var brush = (RadialBrushData)widget.ControlObject;
			brush.Transform = transform;
		}

		public GradientWrapMode GetGradientWrap(RadialGradientBrush widget)
		{
			var brush = (RadialBrushData)widget.ControlObject;
			return brush.WrapMode;
		}

		public void SetGradientWrap(RadialGradientBrush widget, GradientWrapMode gradientWrap)
		{
			var brush = (RadialBrushData)widget.ControlObject;
			brush.WrapMode = gradientWrap;
			brush.Reset();
		}

		public object Create(Color startColor, Color endColor, PointF center, PointF gradientOrigin, SizeF radius)
		{
			return new RadialBrushData
			{
				StartColor = startColor,
				EndColor = endColor,
				Center = center,
				GradientOrigin = gradientOrigin,
				Radius = radius
			};
		}
	}
}
