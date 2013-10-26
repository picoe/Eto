using System;
using Eto.Drawing;
using sd = System.Drawing;
using sd2 = System.Drawing.Drawing2D;

namespace Eto.Platform.Windows.Drawing
{
	/// <summary>
	/// Handler for <see cref="IMatrix"/>
	/// </summary>
	/// <copyright>(c) 2012-2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class MatrixHandler : IMatrixHandler, IDisposable
	{
		sd2.Matrix control;

		public sd2.Matrix Control { get { return control; } }

		object IControlObjectSource.ControlObject { get { return control; } }

		public MatrixHandler ()
		{
		}

		public MatrixHandler (sd2.Matrix matrix)
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
				control = new sd2.Matrix (value, e [1], e [2], e [3], e [4], e [5]);
			}
		}

		public float Xy
		{
			get { return control.Elements [1]; }
			set
			{
				var e = control.Elements;
				control = new sd2.Matrix (e [0], value, e [2], e [3], e [4], e [5]);
			}
		}

		public float Yx
		{
			get { return control.Elements [2]; }
			set
			{
				var e = control.Elements;
				control = new sd2.Matrix (e [0], e [1], value, e [3], e [4], e [5]);
			}
		}

		public float Yy
		{
			get { return control.Elements [3]; }
			set
			{
				var e = control.Elements;
				control = new sd2.Matrix (e [0], e [1], e [2], value, e [4], e [5]);
			}
		}

		public float X0
		{
			get { return Control.OffsetX; }
			set
			{
				var e = control.Elements;
				control = new sd2.Matrix (e [0], e [1], e [2], e [3], value, e [5]);
			}
		}

		public float Y0
		{
			get { return Control.OffsetY; }
			set
			{
				var e = control.Elements;
				control = new sd2.Matrix (e [0], e [1], e [2], e [3], e [4], value);
			}
		}

		public void Rotate (float angle)
		{
			this.Control.Rotate (angle, sd2.MatrixOrder.Prepend);
		}

		public void RotateAt (float angle, float centerX, float centerY)
		{
			this.Control.RotateAt (angle, new sd.PointF (centerX, centerY), sd2.MatrixOrder.Prepend);
		}

		public void Translate (float x, float y)
		{
			this.Control.Translate (x, y, sd2.MatrixOrder.Prepend);
		}

		public void Scale (float scaleX, float scaleY)
		{
			this.Control.Scale (scaleX, scaleY, sd2.MatrixOrder.Prepend);
		}

		public void ScaleAt (float scaleX, float scaleY, float centerX, float centerY)
		{
			var m = new sd2.Matrix (scaleX, 0, 0, scaleY, centerX - centerX * scaleX, centerY - centerY * scaleY);
			this.Control.Multiply (m, sd2.MatrixOrder.Prepend);
		}

		public void Skew (float skewX, float skewY)
		{
			var m = new sd2.Matrix (1, (float)Math.Tan(Conversions.DegreesToRadians(skewX)), (float)Math.Tan(Conversions.DegreesToRadians(skewY)), 1, 0, 0);
			this.Control.Multiply (m, sd2.MatrixOrder.Prepend);
		}

		public void Append (IMatrix matrix)
		{
			this.Control.Multiply (matrix.ToSD (), sd2.MatrixOrder.Append);
		}

		public void Prepend (IMatrix matrix)
		{
			this.Control.Multiply (matrix.ToSD (), sd2.MatrixOrder.Prepend);
		}

		public void Create ()
		{
			control = new sd2.Matrix ();
		}

		public void Create (float xx, float yx, float xy, float yy, float dx, float dy)
		{
			control = new sd2.Matrix (xx, yx, xy, yy, dx, dy);
		}

		public void Invert ()
		{
			this.Control.Invert ();
		}

		public PointF TransformPoint (Point p)
		{
			var px = new sd.Point[] { Platform.Conversions.ToSD (p) };

			this.Control.TransformPoints (px);

			return Platform.Conversions.ToEto (px [0]);
		}

		public PointF TransformPoint (PointF p)
		{
			var px = new sd.PointF[] { p.ToSD () };

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
