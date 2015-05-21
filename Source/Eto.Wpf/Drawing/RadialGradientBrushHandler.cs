using Eto.Drawing;
using swm = System.Windows.Media;

namespace Eto.Wpf.Drawing
{
	/// <summary>
	/// Handler for <see cref="IvGradientBrush"/>
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class RadialGradientBrushHandler : RadialGradientBrush.IHandler
	{

		public IMatrix GetTransform(RadialGradientBrush widget)
		{
			return ((swm.RadialGradientBrush)widget.ControlObject).Transform.ToEtoMatrix();
		}

		public void SetTransform(RadialGradientBrush widget, IMatrix transform)
		{
			((swm.RadialGradientBrush)widget.ControlObject).Transform = transform.ToWpfTransform();
		}

		public GradientWrapMode GetGradientWrap(RadialGradientBrush widget)
		{
			return ((swm.RadialGradientBrush)widget.ControlObject).SpreadMethod.ToEto();
		}

		public void SetGradientWrap(RadialGradientBrush widget, GradientWrapMode gradientWrap)
		{
			((swm.RadialGradientBrush)widget.ControlObject).SpreadMethod = gradientWrap.ToWpf ();
		}

		public object Create(Color startColor, Color endColor, PointF center, PointF gradientOrigin, SizeF radius)
		{
			return new swm.RadialGradientBrush(startColor.ToWpf(), endColor.ToWpf())
			{
				Center = center.ToWpf(),
				GradientOrigin = gradientOrigin.ToWpf(),
				RadiusX = radius.Width,
				RadiusY = radius.Height,
				MappingMode = swm.BrushMappingMode.Absolute,
				SpreadMethod = swm.GradientSpreadMethod.Pad
			};
		}
	}
}
