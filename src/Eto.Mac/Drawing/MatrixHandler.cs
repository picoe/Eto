using System;
using Eto.Drawing;


#if OSX

namespace Eto.Mac.Drawing
#elif IOS
using CoreGraphics;

namespace Eto.iOS.Drawing
#endif
{
	/// <summary>
	/// Handler for <see cref="IMatrix"/>
	/// </summary>
	/// <copyright>(c) 2012-2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class MatrixHandler : Matrix.IHandler
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

#if !VSMAC && (MONOMAC || XAMMAC)
		public float[] Elements => new float[] {
					(float)control.xx,
					(float)control.yx,
					(float)control.xy,
					(float)control.yy,
					(float)control.x0,
					(float)control.y0 };
		
		public float X0 { get { return (float)control.x0; } set { control.x0 = value; } }
		public float Y0 { get { return (float)control.y0; } set { control.y0 = value; } }
		public float Xx { get { return (float)control.xx; } set { control.xx = value; } }
		public float Xy { get { return (float)control.xy; } set { control.xy = value; } }
		public float Yx { get { return (float)control.yx; } set { control.yx = value; } }
		public float Yy { get { return (float)control.yy; } set { control.yy = value; } }
#else
		public float[] Elements => new float[] {
					(float)control.A,
					(float)control.B,
					(float)control.C,
					(float)control.D,
					(float)control.Tx,
					(float)control.Ty };
		
		public float X0 { get { return (float)control.Tx; } set { control.Tx = value; } }
		public float Y0 { get { return (float)control.Ty; } set { control.Ty = value; } }
		public float Xx { get { return (float)control.A; } set { control.A = value; } }
		public float Xy { get { return (float)control.C; } set { control.C = value; } }
		public float Yx { get { return (float)control.B; } set { control.B = value; } }
		public float Yy { get { return (float)control.D; } set { control.D = value; } }
#endif
		
		public void Rotate (float angle)
		{
			control = CGAffineTransform.Multiply (CGAffineTransform.MakeRotation (CGConversions.DegreesToRadians (angle)), control);
		}
		
		public void RotateAt (float angle, float centerX, float centerY)
		{
			angle = (float)CGConversions.DegreesToRadians(angle);
			var sina = (nfloat)Math.Sin (angle);
			var cosa = (nfloat)Math.Cos (angle);
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
			var matrix = new CGAffineTransform (1, (nfloat)Math.Tan (CGConversions.DegreesToRadians (skewX)), (nfloat)Math.Tan (CGConversions.DegreesToRadians (skewY)), 1, 0, 0);
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
			return control.TransformPoint(p.ToNS()).ToEto();
		}
		
		public PointF TransformPoint (PointF p)
		{
			return control.TransformPoint(p.ToNS()).ToEto();
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
