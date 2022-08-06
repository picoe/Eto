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

		public const float DegreesToRadians = (float)(Math.PI / 180);

		public object Create(RectangleF rectangle, Color startColor, Color endColor, float angle)
		{
			double angleInRadians = angle * DegreesToRadians;

			double r = Math.Sqrt(Math.Pow(rectangle.Bottom- rectangle.Top, 2) + Math.Pow(rectangle.Right- rectangle.Left, 2)) / 2;

			float centerX = rectangle.Left + (rectangle.Right - rectangle.Left) / 2;
			float centerY = rectangle.Top + (rectangle.Bottom - rectangle.Top) / 2;

			float startX = (float)Math.Max(rectangle.Left, Math.Min(rectangle.Right, centerX - r * Math.Cos(angleInRadians)));
			float startY = (float)Math.Min(rectangle.Bottom, Math.Max(rectangle.Top, centerY - r * Math.Sin(angleInRadians)));

			float endX = (float)Math.Max(rectangle.Left, Math.Min(rectangle.Right, centerX + r * Math.Cos(angleInRadians)));
			float endY = (float)Math.Min(rectangle.Bottom, Math.Max(rectangle.Top, centerY + r * Math.Sin(angleInRadians)));

			return Create(startColor, endColor, new PointF(startX, startY), new PointF(endX, endY));
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