using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using ag = Android.Graphics;

namespace Eto.Android.Drawing
{
	/// <summary>
	/// Handler for <see cref="IMatrix"/>
	/// </summary>
	/// <copyright>(c) 2013 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class MatrixHandler : Matrix.IHandler, IDisposable
	{
		ag.Matrix control;

		public ag.Matrix Control { get { return control; } }

		object IControlObjectSource.ControlObject { get { return control; } }

		public MatrixHandler()
		{
		}

		public MatrixHandler(ag.Matrix matrix)
		{
			control = matrix;
		}

		public float[] Elements
		{
			get
			{
				// Extract values in the order Eto expects
				var nineValues = NineValues;
				var result = new float[6]
				{
					nineValues[0], nineValues[1],
					nineValues[3], nineValues[4],
					nineValues[2], nineValues[5]
				};

				return result;
			}
		}

		private float[] NineValues
		{
			get
			{
				var nineValues = new float[9];
				control.GetValues(nineValues);
				return nineValues;
			}
		}

		private void SetValue(int indexInNineValues, float value)
		{
			var nineValues = new float[9];
			control.GetValues(nineValues);
			nineValues[indexInNineValues] = value;
			control.SetValues(nineValues);
		}

		public float Xx
		{
			get { return NineValues[0]; }
			set { SetValue(0, value); }
		}

		public float Xy
		{
			get { return NineValues[3]; }
			set { SetValue(3, value); }
		}

		public float Yx
		{
			get { return NineValues[1]; }
			set { SetValue(1, value); }
		}

		public float Yy
		{
			get { return NineValues[4]; }
			set { SetValue(4, value); }
		}

		public float X0
		{
			get { return NineValues[2]; }
			set { SetValue(2, value); }
		}

		public float Y0
		{
			get { return NineValues[5]; }
			set { SetValue(5, value); }
		}

		public void Rotate(float angle)
		{
			this.Control.PreRotate(angle);
		}

		public void RotateAt(float angle, float centerX, float centerY)
		{
#if TODO
			this.Control.RotateAt(angle, new ag.PointF(centerX, centerY), ag.MatrixOrder.Prepend);
#else
			throw new NotImplementedException();
#endif
		}

		public void Translate(float x, float y)
		{
			this.Control.PreTranslate(x, y);
		}

		public void Scale(float scaleX, float scaleY)
		{
			this.Control.PreScale(scaleX, scaleY);
		}

		public void ScaleAt(float scaleX, float scaleY, float centerX, float centerY)
		{
#if TODO
			var m = new ag.Matrix(scaleX, 0, 0, scaleY, centerX - centerX * scaleX, centerY - centerY * scaleY);
			this.Control.Multiply(m, ag.MatrixOrder.Prepend);
#else
			throw new NotImplementedException();
#endif
		}

		public void Skew(float skewX, float skewY)
		{
#if TODO
			var m = new ag.Matrix(1, (float)Math.Tan(Conversions.DegreesToRadians(skewX)), (float)Math.Tan(Conversions.DegreesToRadians(skewY)), 1, 0, 0);
			this.Control.Multiply(m, ag.MatrixOrder.Prepend);
#else
			throw new NotImplementedException();
#endif
		}

		public void Append(IMatrix matrix)
		{
			this.Control.PostConcat(matrix.ToAndroid());
		}

		public void Prepend(IMatrix matrix)
		{
			this.Control.PreConcat(matrix.ToAndroid());
		}

		public void Create()
		{
			control = new ag.Matrix();
		}

		public void Create(float xx, float yx, float xy, float yy, float x0, float y0)
		{
			control = new ag.Matrix();

			// Put values in the order Android expects.
			var values = new float[]
			{
				xx, yx, x0,
				xy, yy, y0,
				0,  0,  1
			};
			control.SetValues(values);
		}

		public void Invert()
		{
			var inverse = new ag.Matrix();
			this.Control.Invert(inverse);
			this.control = inverse;
		}

		public PointF TransformPoint(Point p)
		{
			var px = new float[] { p.X, p.Y };
			this.Control.MapPoints(px);
			return new PointF(px[0], px[1]);
		}

		public PointF TransformPoint(PointF p)
		{
			var px = new float[] { p.X, p.Y };
			this.Control.MapPoints(px);
			return new PointF(px[0], px[1]);
		}

		public void Dispose()
		{
			if (control != null)
			{
				control.Dispose();
				control = null;
			}
		}

		public IMatrix Clone()
		{
			return new MatrixHandler(new ag.Matrix(control));
		}
	}
}