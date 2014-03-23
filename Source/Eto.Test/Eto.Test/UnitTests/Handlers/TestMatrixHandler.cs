using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;

namespace Eto.Test.UnitTests.Handlers
{
	/// <summary>
	/// Test handler for Matrix.
	/// The elements of the matrix are:
	/// <para>
	/// 	| xx yx 0 |
	/// 	| xy yy 0 |
	/// 	| x0 y0 1 |
	/// </para>
	/// </remarks>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class TestMatrixHandler : IMatrixHandler
	{
		const double Pi = Math.PI;

		float xx, yx, xy, yy, x0, y0;

		float CosD(float degrees)
		{
			return (float) Math.Cos(degrees * Pi / 180);
		}

		float SinD(float degrees)
		{
			return (float) Math.Sin(degrees * Pi / 180);
		}

		float Cos(float radians)
		{
			return (float) Math.Cos(radians);
		}

		float Sin(float degrees)
		{
			return (float) Math.Sin(degrees);
		}

		static float[] Multiply(float[] e1, float[] e2)
		{
			return new float[]
			{
				e1[0]*e2[0] + e1[1]*e2[2],
				e1[0]*e2[1] + e1[1]*e2[3],
				e1[2]*e2[0] + e1[3]*e2[2],
				e1[2]*e2[1] + e1[3]*e2[3],

				e1[4]*e2[0] + e1[5]*e2[2] + e2[4],
				e1[4]*e2[1] + e1[5]*e2[3] + e2[5],
			};
		}

		public void Create()
		{
			xx = yy = 1;
		}

		public void Create(float xx, float yx, float xy, float yy, float x0, float y0)
		{
			this.xx = xx;
			this.yx = yx;
			this.xy = xy;
			this.yy = yy;
			this.x0 = x0;
			this.y0 = y0;
		}

		public float[] Elements
		{
			get { return new float[] { xx, yx, xy, yy, x0, y0 }; }
			private set
			{
				xx = value[0];
				yx = value[1];
				xy = value[2];
				yy = value[3];
				x0 = value[4];
				y0 = value[5];
			}
		}

		public float Xx { get { return xx; } set { xx = value; } }
		public float Yx { get { return xx; } set { yx = value; } }
		public float Xy { get { return xx; } set { xy = value; } }
		public float Yy { get { return xx; } set { yy = value; } }
		public float X0 { get { return xx; } set { x0 = value; } }
		public float Y0 { get { return xx; } set { y0 = value; } }

		public void Rotate(float angle)
		{
			var m = new TestMatrixHandler();
			var c = CosD(angle);
			var s = SinD(angle);
			m.Create(c, s, -s, c, 0, 0);
			Append(m);
		}

		public void RotateAt(float angle, float centerX, float centerY)
		{
			throw new NotImplementedException();
		}

		public void Translate(float offsetX, float offsetY)
		{
			x0 += offsetX;
			y0 += offsetY;
		}

		public void Scale(float scaleX, float scaleY)
		{
			throw new NotImplementedException();
		}

		public void ScaleAt(float scaleX, float scaleY, float centerX, float centerY)
		{
			throw new NotImplementedException();
		}

		public void Skew(float skewX, float skewY)
		{
			throw new NotImplementedException();
		}

		public void Append(IMatrix matrix)
		{
			Elements = Multiply(Elements, matrix.Elements);
		}

		public void Prepend(IMatrix matrix)
		{
			Elements = Multiply(matrix.Elements, Elements);
		}

		public void Invert()
		{
			throw new NotImplementedException();
		}

		public PointF TransformPoint(Point point)
		{
			throw new NotImplementedException();
		}

		public PointF TransformPoint(PointF point)
		{
			throw new NotImplementedException();
		}

		public IMatrix Clone()
		{
			var result = new TestMatrixHandler();
			result.Create(xx, yx, xy, yy, x0, y0);
			return result;
		}

		public object ControlObject
		{
			get { return null; }
		}

		public void Dispose()
		{
		}
	}
}
