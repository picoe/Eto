using System;
using System.IO;
using Eto.Drawing;
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
using MonoMac.Foundation;
using SD = System.Drawing;

namespace Eto.Platform.Mac.Drawing
{
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
		
		public void Create (float m11, float m12, float m21, float m22, float dx, float dy)
		{
			control = new CGAffineTransform (m11, m12, m21, m22, dx, dy);
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
			control.Rotate (Conversions.DegreesToRadians (angle));
		}

		public void RotateAt (float angle, float centerX, float centerY)
		{
			angle = Conversions.DegreesToRadians (angle);
			var sina = (float)Math.Sin (angle);
			var cosa = (float)Math.Cos (angle);
			control.Multiply(new CGAffineTransform(cosa, sina, -sina, cosa, centerX - centerX * cosa + centerY * sina, centerY - centerX * sina - centerY * cosa));
		}

		public void Translate (float x, float y)
		{
			control.Translate (x, y);
		}

		public void Scale (float scaleX, float scaleY)
		{
			control.Scale (scaleX, scaleY);
		}

		public void ScaleAt (float scaleX, float scaleY, float centerX, float centerY)
		{
			control.Multiply (new CGAffineTransform(scaleX, 0, 0, scaleY, centerX - centerX * scaleX, centerY - centerY * scaleY));
		}

		public void Skew (float skewX, float skewY)
		{
			control.Multiply (new CGAffineTransform (1, (float)Math.Tan (Conversions.DegreesToRadians (skewX)), (float)Math.Tan (Conversions.DegreesToRadians (skewY)), 1, 0, 0));
		}

		public void Append (IMatrix matrix)
		{
			var affineMatrix = (CGAffineTransform)matrix.ControlObject;
			control.Multiply (affineMatrix);
		}

		public void Prepend (IMatrix matrix)
		{
			var affineMatrix = (CGAffineTransform)matrix.ControlObject;
			affineMatrix.Multiply (control);
			control = affineMatrix;
		}

		public void Invert ()
		{
			control = this.control.Invert ();
		}

		public PointF TransformPoint (Point p)
		{
			return Platform.Conversions.ToEto (control.TransformPoint (p.ToSDPointF ()));
		}

		public PointF TransformPoint (PointF p)
		{
			return Platform.Conversions.ToEto (control.TransformPoint (p.ToSD ()));
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
