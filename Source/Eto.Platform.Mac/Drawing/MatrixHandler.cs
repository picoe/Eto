using System;
using Eto.Drawing;
using SD = System.Drawing;

#if OSX
using MonoMac.CoreGraphics;

namespace Eto.Platform.Mac.Drawing
#elif IOS
using MonoTouch.CoreGraphics;

namespace Eto.Platform.iOS.Drawing
#endif
{
	/// <summary>
	/// Handler for <see cref="IMatrix"/>
	/// </summary>
	/// <copyright>(c) 2012-2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class MatrixHandler : IMatrixHandler
	{
		CGAffineTransform control;
		
		public CGAffineTransform Control { get { return control; } }
		
		public MatrixHandler ()
		{
		}
		
		public MatrixHandler (CGAffineTransform matrix)
		{
			control = matrix;
		}
		
		public void Create ()
		{
			control = CGAffineTransform.MakeIdentity ();
		}
		
		public void Create (float xx, float yx, float xy, float yy, float dx, float dy)
		{
			control = new CGAffineTransform (xx, yx, xy, yy, dx, dy);
		}
		
		public float[] Elements
		{
			get
			{
				return new float[] {
					control.xx,
					control.yx,
					control.xy,
					control.yy,
					control.x0,
					control.y0
				};
			}
		}
		
		public float X0 { get { return control.x0; } set { control.x0 = value; } }
		
		public float Y0 { get { return control.y0; } set { control.y0 = value; } }
		
		public float Xx { get { return control.xx; } set { control.xx = value; } }
		
		public float Xy { get { return control.xy; } set { control.xy = value; } }
		
		public float Yx { get { return control.yx; } set { control.yx = value; } }
		
		public float Yy { get { return  control.yy; } set { control.yy = value; } }
		
		public void Rotate (float angle)
		{
			control = CGAffineTransform.Multiply (CGAffineTransform.MakeRotation (Conversions.DegreesToRadians (angle)), control);
		}
		
		public void RotateAt (float angle, float centerX, float centerY)
		{
			angle = Conversions.DegreesToRadians (angle);
			var sina = (float)Math.Sin (angle);
			var cosa = (float)Math.Cos (angle);
			var matrix = new CGAffineTransform(cosa, sina, -sina, cosa, centerX - centerX * cosa + centerY * sina, centerY - centerX * sina - centerY * cosa);
			control = CGAffineTransform.Multiply (matrix, control);
		}
		
		public void Translate (float x, float y)
		{
			control = CGAffineTransform.Multiply (CGAffineTransform.MakeTranslation (x, y), control);
		}
		
		public void Scale (float scaleX, float scaleY)
		{
			control = CGAffineTransform.Multiply (CGAffineTransform.MakeScale (scaleX, scaleY), control);
		}
		
		public void ScaleAt (float scaleX, float scaleY, float centerX, float centerY)
		{
			var matrix = new CGAffineTransform(scaleX, 0f, 0f, scaleY, centerX - centerX * scaleX, centerY - centerY * scaleY);
			control = CGAffineTransform.Multiply (matrix, control);
		}
		
		public void Skew (float skewX, float skewY)
		{
			var matrix = new CGAffineTransform (1, (float)Math.Tan (Conversions.DegreesToRadians (skewX)), (float)Math.Tan (Conversions.DegreesToRadians (skewY)), 1, 0, 0);
			control = CGAffineTransform.Multiply (matrix, control);
		}
		
		public void Append (IMatrix matrix)
		{
			var affineMatrix = (CGAffineTransform)matrix.ControlObject;
			control.Multiply (affineMatrix);
		}
		
		public void Prepend (IMatrix matrix)
		{
			var affineMatrix = (CGAffineTransform)matrix.ControlObject;
			control = CGAffineTransform.Multiply (affineMatrix, control);
		}
		
		public void Invert ()
		{
			control = control.Invert ();
		}
		
		public PointF TransformPoint (Point p)
		{
			return control.TransformPoint(p.ToSDPointF()).ToEto();
		}
		
		public PointF TransformPoint (PointF p)
		{
			return control.TransformPoint(p.ToSD()).ToEto();
		}
		
		public object ControlObject
		{
			get { return control; }
		}
		
		public IMatrix Clone ()
		{
			return new MatrixHandler (control);
		}
		
		public void Dispose()
		{
		}
	}
}
