using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;

using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using ag = Android.Graphics;

namespace Eto.Android.Drawing
{
	public class LinearGradientBrushHandler : BrushHandler, LinearGradientBrush.IHandler
	{
		// TODO: Android does not have the concept of a linear 
		class BrushObject
		{
			public ag.Paint Paint { get; set; }
			public ag.Matrix InitialMatrix { get; set; }
			public IMatrix Matrix { get; set; }
		}

		public override ag.Paint GetPaint(Brush brush)
		{
			return ((BrushObject)brush.ControlObject).Paint;
		}

		public object Create(Color startColor, Color endColor, PointF startPoint, PointF endPoint)
		{
			var shader = new ag.LinearGradient(startPoint.X, startPoint.Y, endPoint.X, endPoint.Y, startColor.ToAndroid(), endColor.ToAndroid(), 
				// is this correct?
				ag.Shader.TileMode.Clamp);
			var paint = new ag.Paint();
			paint.SetShader(shader);
			return new BrushObject { Paint = paint }; // TODO: initial matrix
		}

		public object Create(RectangleF rectangle, Color startColor, Color endColor, float angle)
		{
			throw new NotImplementedException();
		}

		public IMatrix GetTransform(LinearGradientBrush widget)
		{
			throw new NotImplementedException();
		}

		public void SetTransform(LinearGradientBrush widget, IMatrix transform)
		{
			throw new NotImplementedException();
		}

		public GradientWrapMode GetGradientWrap(LinearGradientBrush widget)
		{
			throw new NotImplementedException();
		}

		public void SetGradientWrap(LinearGradientBrush widget, GradientWrapMode gradientWrap)
		{
			throw new NotImplementedException();
		}
	}
}