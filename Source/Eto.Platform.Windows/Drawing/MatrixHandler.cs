using System;
using System.Collections.Generic;
using System.Text;
using Eto.Drawing;
using SD = System.Drawing;
using SD2D = System.Drawing.Drawing2D;
using SWF = System.Windows.Forms;

namespace Eto.Platform.Windows.Drawing
{
	public class MatrixHandler : IMatrixHandler, IDisposable
	{
		SD2D.Matrix control;

		public SD2D.Matrix Control { get { return control; } }

		object IMatrix.ControlObject { get { return control; } }

		public MatrixHandler ()
		{
		}

		public MatrixHandler (SD2D.Matrix matrix)
		{
			control = matrix;
		}

		public float[] Elements { get { return Control.Elements; } }

		public float Xx
		{
			get { return control.Elements [0]; }
			set
			{
				var e = control.Elements;
				control = new SD2D.Matrix (value, e [1], e [2], e [3], e [4], e [5]);
			}
		}

		public float Xy
		{
			get { return control.Elements [1]; }
			set
			{
				var e = control.Elements;
				control = new SD2D.Matrix (e [0], value, e [2], e [3], e [4], e [5]);
			}
		}

		public float Yx
		{
			get { return control.Elements [2]; }
			set
			{
				var e = control.Elements;
				control = new SD2D.Matrix (e [0], e [1], value, e [3], e [4], e [5]);
			}
		}

		public float Yy
		{
			get { return control.Elements [3]; }
			set
			{
				var e = control.Elements;
				control = new SD2D.Matrix (e [0], e [1], e [2], value, e [4], e [5]);
			}
		}

		public float X0
		{
			get { return Control.OffsetX; }
			set
			{
				var e = control.Elements;
				control = new SD2D.Matrix (e [0], e [1], e [2], e [3], value, e [5]);
			}
		}

		public float Y0
		{
			get { return Control.OffsetY; }
			set
			{
				var e = control.Elements;
				control = new SD2D.Matrix (e [0], e [1], e [2], e [3], e [4], value);
			}
		}

		public void Rotate (float angle)
		{
			this.Control.Rotate (angle, SD2D.MatrixOrder.Append);
		}

		public void RotateAt (float angle, float centerX, float centerY)
		{
			this.Control.RotateAt (angle, new SD.PointF (centerX, centerY), SD2D.MatrixOrder.Append);
		}

		public void Translate (float x, float y)
		{
			this.Control.Translate (x, y, SD2D.MatrixOrder.Append);
		}

		public void Scale (float scaleX, float scaleY)
		{
			this.Control.Scale (scaleX, scaleY, SD2D.MatrixOrder.Append);
		}

		public void ScaleAt (float scaleX, float scaleY, float centerX, float centerY)
		{
			var m = new SD2D.Matrix (scaleX, 0, 0, scaleY, centerX - centerX * scaleX, centerY - centerY * scaleY);
			this.Control.Multiply (m, SD2D.MatrixOrder.Append);
		}

		public void Skew (float skewX, float skewY)
		{
			var m = new SD2D.Matrix (1, (float)Math.Tan(Conversions.DegreesToRadians(skewX)), (float)Math.Tan(Conversions.DegreesToRadians(skewY)), 1, 0, 0);
			this.Control.Multiply (m, SD2D.MatrixOrder.Append);
		}

		public void Append (IMatrix matrix)
		{
			this.Control.Multiply (matrix.ToSD (), SD2D.MatrixOrder.Append);
		}

		public void Prepend (IMatrix matrix)
		{
			this.Control.Multiply (matrix.ToSD (), SD2D.MatrixOrder.Prepend);
		}

		public void Create ()
		{
			control = new SD2D.Matrix ();
		}

		public void Create (float m11, float m12, float m21, float m22, float dx, float dy)
		{
			control = new SD2D.Matrix (m11, m12, m21, m22, dx, dy);
		}

		public void Invert ()
		{
			this.Control.Invert ();
		}

		public PointF TransformPoint (Point p)
		{
			var px = new SD.Point[] { p.ToSD () };

			this.Control.TransformPoints (px);

			return px [0].ToEto ();
		}

		public PointF TransformPoint (PointF p)
		{
			var px = new SD.PointF[] { p.ToSD () };

			this.Control.TransformPoints (px);

			return px [0].ToEto ();
		}

		public void Dispose ()
		{
			if (control != null) {
				control.Dispose ();
				control = null;
			}
		}

		public IMatrix Clone ()
		{
			return new MatrixHandler (control.Clone ());
		}
	}
}
