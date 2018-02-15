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
				var nineValues = NineValues;
				var result = new float[6]
				{
					nineValues[0], nineValues[1],  // array[2] not copied as it is always 1
					nineValues[3], nineValues[4],  // array[5] not copied as it is always 1
					nineValues[6], nineValues[7]   // array[8] not copied as it is always 1
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
			get { return NineValues[1]; }
			set { SetValue(1, value); }
		}

		public float Yx
		{
			get { return NineValues[3]; }
			set { SetValue(3, value); }
		}

		public float Yy
		{
			get { return NineValues[4]; }
			set { SetValue(4, value); }
		}

		public float X0
		{
			get { return NineValues[6]; }
			set { SetValue(6, value); }
		}

		public float Y0
		{
			get { return NineValues[7]; }
			set { SetValue(7, value); }
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

		public void Create(float xx, float yx, float xy, float yy, float dx, float dy)
		{
			control = new ag.Matrix();
			var values = new float[]
			{
				xx, yx, 1,
				xy, yy, 1,
				dx, dy, 1
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
			var px = new ag.Point[] { Conversions.ToAndroidPoint(p) };

#if TODO
			this.Control.TransformPoints(px);

			return Platform.Conversions.ToEto(px[0]);
#else 
			throw new NotImplementedException();
#endif
		}

		public PointF TransformPoint(PointF p)
		{
			var px = new ag.PointF[] { p.ToAndroid() };

#if TODO
			this.Control.TransformPoints(px);
#else
			throw new NotImplementedException();
#endif


			return px[0].ToEto();
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
#if TODO
			return new MatrixHandler(control.Clone());
#else
			throw new NotImplementedException();
#endif
		}
	}
}