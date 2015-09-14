using Eto.Drawing;
using sd = SharpDX.Direct2D1;
using s = SharpDX;

namespace Eto.Direct2D.Drawing
{
	public class LinearGradientBrushHandler : LinearGradientBrush.IHandler
	{
		public class LinearBrushData : TransformBrushData
		{
			public PointF StartPoint { get; set; }
			public PointF EndPoint { get; set; }
			public Color StartColor { get; set; }
			public Color EndColor { get; set; }
			public GradientWrapMode WrapMode { get; set; }

			protected override sd.Brush Create(sd.RenderTarget target)
			{
				return new sd.LinearGradientBrush(
					target,
					new sd.LinearGradientBrushProperties
					{
						StartPoint = StartPoint.ToDx(),
						EndPoint = EndPoint.ToDx()
					},
					new sd.GradientStopCollection(GraphicsHandler.CurrentRenderTarget, new[] {
					new sd.GradientStop { Color = StartColor.ToDx(), Position = 0f }, 
					new sd.GradientStop { Color = EndColor.ToDx(), Position = 1f }
				}, WrapMode.ToDx())
				);
			}
		}

		public object Create(Color startColor, Color endColor, PointF startPoint, PointF endPoint)
		{
			return new LinearBrushData
			{
				StartColor = startColor,
				EndColor = endColor,
				StartPoint = startPoint,
				EndPoint = endPoint
			};
		}

		public object Create(RectangleF rectangle, Color startColor, Color endColor, float angle)
		{
			var matrix = new MatrixHandler();
			var startPoint = rectangle.Location;
			matrix.RotateAt(angle - 45, startPoint.X, startPoint.Y);
			var endPoint = matrix.TransformPoint(rectangle.EndLocation);
			return new LinearBrushData
			{
				StartColor = startColor,
				EndColor = endColor,
				StartPoint = startPoint,
				EndPoint = endPoint
			};
		}

		public IMatrix GetTransform(LinearGradientBrush widget)
		{
			var brush = (LinearBrushData)widget.ControlObject;
			return brush.Transform;
		}

		public void SetTransform(LinearGradientBrush widget, IMatrix transform)
		{
			var brush = (LinearBrushData)widget.ControlObject;
			brush.Transform = transform;
		}

		public GradientWrapMode GetGradientWrap(LinearGradientBrush widget)
		{
			var brush = (LinearBrushData)widget.ControlObject;
			return brush.WrapMode;
		}

		public void SetGradientWrap(LinearGradientBrush widget, GradientWrapMode gradientWrap)
		{
			var brush = (LinearBrushData)widget.ControlObject;
			brush.WrapMode = gradientWrap;
			brush.Reset();
		}
	}
}
