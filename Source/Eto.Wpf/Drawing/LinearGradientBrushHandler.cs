using Eto.Drawing;
using swm = System.Windows.Media;

namespace Eto.Wpf.Drawing
{
	/// <summary>
	/// Handler for <see cref="ILinearGradientBrush"/>
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class LinearGradientBrushHandler : LinearGradientBrush.IHandler
	{
		public object Create(Color startColor, Color endColor, PointF startPoint, PointF endPoint)
		{
			return new swm.LinearGradientBrush(startColor.ToWpf(), endColor.ToWpf(), startPoint.ToWpf(), endPoint.ToWpf())
			{
				MappingMode = swm.BrushMappingMode.Absolute,
				SpreadMethod = swm.GradientSpreadMethod.Pad
			};
		}

		public object Create(RectangleF rectangle, Color startColor, Color endColor, float angle)
		{
			var matrix = swm.Matrix.Identity;
			var startPoint = rectangle.Location.ToWpf();
			matrix.RotateAtPrepend(angle - 45, startPoint.X, startPoint.Y);
			var endPoint = matrix.Transform(rectangle.EndLocation.ToWpf());
			return new swm.LinearGradientBrush(startColor.ToWpf(), endColor.ToWpf(), startPoint, endPoint)
			{
				MappingMode = swm.BrushMappingMode.Absolute,
				SpreadMethod = swm.GradientSpreadMethod.Pad
			};
		}

		public IMatrix GetTransform(LinearGradientBrush widget)
		{
			return ((swm.LinearGradientBrush)widget.ControlObject).Transform.ToEtoMatrix();
		}

		public void SetTransform(LinearGradientBrush widget, IMatrix transform)
		{
			((swm.LinearGradientBrush)widget.ControlObject).Transform = transform.ToWpfTransform();
		}

		public GradientWrapMode GetGradientWrap(LinearGradientBrush widget)
		{
			return ((swm.LinearGradientBrush)widget.ControlObject).SpreadMethod.ToEto();
		}

		public void SetGradientWrap(LinearGradientBrush widget, GradientWrapMode gradientWrap)
		{
			((swm.LinearGradientBrush)widget.ControlObject).SpreadMethod = gradientWrap.ToWpf();
		}
	}
}
