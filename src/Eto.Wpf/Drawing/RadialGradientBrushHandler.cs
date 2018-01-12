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
		static swm.RadialGradientBrush Get(RadialGradientBrush widget) => ((FrozenBrushWrapper)widget.ControlObject).Brush as swm.RadialGradientBrush;

		static void SetFrozen(RadialGradientBrush widget) => ((FrozenBrushWrapper)widget.ControlObject).SetFrozen();

		public IMatrix GetTransform(RadialGradientBrush widget)
		{
			return Get(widget).Transform.ToEtoMatrix();
		}

		public void SetTransform(RadialGradientBrush widget, IMatrix transform)
		{
			Get(widget).Transform = transform.ToWpfTransform();
			SetFrozen(widget);
		}

		public GradientWrapMode GetGradientWrap(RadialGradientBrush widget)
		{
			return Get(widget).SpreadMethod.ToEto();
		}

		public void SetGradientWrap(RadialGradientBrush widget, GradientWrapMode gradientWrap)
		{
			Get(widget).SpreadMethod = gradientWrap.ToWpf();
			SetFrozen(widget);
		}

		public object Create(Color startColor, Color endColor, PointF center, PointF gradientOrigin, SizeF radius)
		{
			return new FrozenBrushWrapper(new swm.RadialGradientBrush(startColor.ToWpf(), endColor.ToWpf())
			{
				Center = center.ToWpf(),
				GradientOrigin = gradientOrigin.ToWpf(),
				RadiusX = radius.Width,
				RadiusY = radius.Height,
				MappingMode = swm.BrushMappingMode.Absolute,
				SpreadMethod = swm.GradientSpreadMethod.Pad
			});
		}
	}
}
