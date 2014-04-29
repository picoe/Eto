using Eto.Drawing;
using sd = System.Drawing;
using sd2 = System.Drawing.Drawing2D;

namespace Eto.WinForms.Drawing
{
	/// <summary>
	/// Handler for <see cref="ILinearGradientBrush"/>
	/// </summary>
	/// <copyright>(c) 2012 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class LinearGradientBrushHandler : BrushHandler, ILinearGradientBrush
	{
		class BrushObject
		{
			public sd2.LinearGradientBrush Brush { get; set; }
			public sd2.Matrix InitialMatrix { get; set; }
			public IMatrix Matrix { get; set; }
		}

		public object Create (Color startColor, Color endColor, PointF startPoint, PointF endPoint)
		{
			var brush = new sd2.LinearGradientBrush (startPoint.ToSD (), endPoint.ToSD (), startColor.ToSD (), endColor.ToSD ());
			return new BrushObject {
				Brush = brush,
				InitialMatrix = brush.Transform
			};
		}

		public object Create (RectangleF rectangle, Color startColor, Color endColor, float angle)
		{
			var brush = new sd2.LinearGradientBrush (rectangle.ToSD (), startColor.ToSD (), endColor.ToSD (), angle, true);
			return new BrushObject {
				Brush = brush,
				InitialMatrix = brush.Transform
			};
		}

		public IMatrix GetTransform (LinearGradientBrush widget)
		{
			return ((BrushObject)widget.ControlObject).Matrix;
		}

		public void SetTransform (LinearGradientBrush widget, IMatrix transform)
		{
			var brush = ((BrushObject)widget.ControlObject);
			brush.Matrix = transform;
			var newmatrix = brush.InitialMatrix.Clone ();
			newmatrix.Multiply (transform.ToSD ());
			brush.Brush.Transform = newmatrix;
		}

		public GradientWrapMode GetGradientWrap (LinearGradientBrush widget)
		{
			return ((BrushObject)widget.ControlObject).Brush.WrapMode.ToEtoGradientWrap ();
		}

		public void SetGradientWrap (LinearGradientBrush widget, GradientWrapMode gradientWrap)
		{
			((BrushObject)widget.ControlObject).Brush.WrapMode = gradientWrap.ToSD ();
		}

		public override sd.Brush GetBrush (Brush brush)
		{
			return ((BrushObject)brush.ControlObject).Brush;
		}
	}
}
