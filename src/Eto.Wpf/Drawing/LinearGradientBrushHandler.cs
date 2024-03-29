namespace Eto.Wpf.Drawing
{
	/// <summary>
	/// Handler for <see cref="ILinearGradientBrush"/>
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class LinearGradientBrushHandler : LinearGradientBrush.IHandler
	{
		static swm.LinearGradientBrush Get(LinearGradientBrush widget) => ((FrozenBrushWrapper)widget.ControlObject).Brush as swm.LinearGradientBrush;

		static void SetFrozen(LinearGradientBrush widget) => ((FrozenBrushWrapper)widget.ControlObject).SetFrozen();

		public object Create(Color startColor, Color endColor, PointF startPoint, PointF endPoint)
		{
			return new FrozenBrushWrapper(new swm.LinearGradientBrush(startColor.ToWpf(), endColor.ToWpf(), startPoint.ToWpf(), endPoint.ToWpf())
			{
				MappingMode = swm.BrushMappingMode.Absolute,
				SpreadMethod = swm.GradientSpreadMethod.Pad
			});
		}

		public object Create(RectangleF rectangle, Color startColor, Color endColor, float angle)
		{
			GradientHelper.GetLinearFromRectangle(rectangle, angle, out var startPoint, out var endPoint);
			return Create(startColor, endColor, startPoint, endPoint);
		}

		public IMatrix GetTransform(LinearGradientBrush widget)
		{
			return Get(widget).Transform.ToEtoMatrix();
		}

		public void SetTransform(LinearGradientBrush widget, IMatrix transform)
		{
			Get(widget).Transform = transform.ToWpfTransform();
			SetFrozen(widget);
		}

		public GradientWrapMode GetGradientWrap(LinearGradientBrush widget)
		{
			return Get(widget).SpreadMethod.ToEto();
		}

		public void SetGradientWrap(LinearGradientBrush widget, GradientWrapMode gradientWrap)
		{
			Get(widget).SpreadMethod = gradientWrap.ToWpf();
			SetFrozen(widget);
		}
	}
}
