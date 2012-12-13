using System;
using Eto.Drawing;

namespace Eto.Platform.GtkSharp.Drawing
{
	public class MatrixHandler : IMatrixHandler
	{
		Cairo.Matrix control;

		public MatrixHandler ()
		{
		}

		public MatrixHandler (Cairo.Matrix matrix)
		{
			this.control = matrix;
		}

		public float[] Elements
		{
			get
			{
				return new float[] {
					(float)control.Xx,
					(float)control.Yx,
					(float)control.Xy,
					(float)control.Yy,
					(float)control.X0,
					(float)control.Y0
				};
			}
		}

		public float Xx { get { return (float)control.Xx; } set { control.Xx = value; } }
		
		public float Xy { get { return (float)control.Xx; } set { control.Xx = value; } }
		
		public float Yx { get { return (float)control.Yx; } set { control.Yx = value; } }
		
		public float Yy { get { return (float)control.Yy; } set { control.Yy = value; } }

		public float X0 { get { return (float)control.X0; } set { control.X0 = value; } }

		public float Y0 { get { return (float)control.Y0; } set { control.Y0 = value; } }

		public void Create ()
		{
			control = new Cairo.Matrix ();
		}

		public void Create (float xx, float yx, float xy, float yy, float x0, float y0)
		{
			control = new Cairo.Matrix (xx, yx, xy, yy, x0, y0);
		}

		public void Rotate (float angle)
		{
			var matrix = new Cairo.Matrix ();
			matrix.InitRotate (Conversions.DegreesToRadians (angle));
			control = Cairo.Matrix.Multiply (control, matrix);
		}

		public void RotateAt (float angle, float centerX, float centerY)
		{
			angle = Conversions.DegreesToRadians (angle);
			var sina = Math.Sin (angle);
			var cosa = Math.Cos (angle);
			var matrix = new Cairo.Matrix (cosa, sina, -sina, cosa, centerX - centerX * cosa + centerY * sina, centerY - centerX * sina - centerY * cosa);;
			control = Cairo.Matrix.Multiply (control, matrix);
		}

		public void Translate (float x, float y)
		{
			var matrix = new Cairo.Matrix ();
			matrix.InitTranslate (x, y);	
			control = Cairo.Matrix.Multiply (control, matrix);
		}

		public void Scale (float scaleX, float scaleY)
		{
			var matrix = new Cairo.Matrix ();
			matrix.InitScale (scaleX, scaleY);
			control = Cairo.Matrix.Multiply (control, matrix);
		}

		public void ScaleAt (float scaleX, float scaleY, float centerX, float centerY)
		{
			var matrix = new Cairo.Matrix (scaleX, 0, 0, scaleY, centerX - centerX * scaleX, centerY - centerY * scaleY);
			control = Cairo.Matrix.Multiply (control, matrix);
		}

		public void Skew (float skewX, float skewY)
		{
			var matrix = new Cairo.Matrix (1, Math.Tan (Conversions.DegreesToRadians (skewX)), Math.Tan (Conversions.DegreesToRadians (skewY)), 1, 0, 0);
			control = Cairo.Matrix.Multiply (control, matrix);
		}

		public void Append (IMatrix matrix)
		{
			var cairoMatrix = matrix.ControlObject as Cairo.Matrix;
			control.Multiply (cairoMatrix);
		}

		public void Prepend (IMatrix matrix)
		{
			var cairoMatrix = matrix.ControlObject as Cairo.Matrix;
			control = Cairo.Matrix.Multiply (cairoMatrix, control);
		}

		public PointF TransformPoint (Point point)
		{
			double x = point.X;
			double y = point.Y;
			control.TransformPoint (ref x, ref y);
			return new PointF ((float)x, (float)y);
		}

		public PointF TransformPoint (PointF point)
		{
			double x = point.X;
			double y = point.Y;
			control.TransformPoint (ref x, ref y);
			return new PointF ((float)x, (float)y);
		}

		public void Invert ()
		{
			control.Invert ();
		}

		public object ControlObject
		{
			get { return control; }
		}

		public IMatrixHandler CloneHandler ()
		{
			var matrix = new Cairo.Matrix(control.Xx, control.Yx, control.Xy, control.Yy, control.X0, control.Y0);
			return new MatrixHandler (matrix);
		}
		
		public IMatrix Clone ()
		{
			return this.CloneHandler ();
		}
	}
}

