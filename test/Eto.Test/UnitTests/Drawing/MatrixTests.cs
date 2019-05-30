using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Eto.Drawing;

namespace Eto.Test.UnitTests.Drawing
{
	/// <summary>
	/// Unit tests for Matrix using TestMatrixHandler.
	/// This class tests the test matrix handler.
	/// 
	/// Platform-specific matrix handlers can be tested
	/// by deriving from this class and overriding CreateMatrix().
	/// </summary>	
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[TestFixture]
	public class MatrixTests
	{
		public MatrixTests()
		{
			// initialize test generator if running through IDE or nunit-gui
			TestBase.Initialize();
		}

		public static bool Equals(IMatrix m, float xx, float yx, float xy, float yy, float x0, float y0)
		{
			var e = m.Elements;
			return
				FloatEquals(e[0], xx) &&
				FloatEquals(e[1], yx) &&
				FloatEquals(e[2], xy) &&
				FloatEquals(e[3], yy) &&
				FloatEquals(e[4], x0) &&
				FloatEquals(e[5], y0);
		}

		private static bool FloatEquals(float p, float q)
		{
			return Math.Abs(p - q) < 0.001f;
		}

		private bool AreEqual(PointF a, PointF b)
		{
			return FloatEquals(a.X, b.X) && FloatEquals(a.Y, b.Y);
		}

		[Test]
		public void Matrix_CreateIdentity_VerifyElements()
		{
			var m = Matrix.Create();
			Assert.IsTrue(Equals(m, 1, 0, 0, 1, 0, 0));
		}

		[TestCase(
			1, 0, 0, 1, 10, 20,
			1, 0, 0, 1, 30, 40,
			1, 0, 0, 1, 40, 60)]
		public void Matrix_Append_Appends(
			float xx, float yx, float xy, float yy, float x0, float y0, // matrix
			float XX, float YX, float XY, float YY, float X0, float Y0,	// prepended matrix
			float Xx, float Yx, float Xy, float Yy, float a0, float b0)	// expected matrix
		{
			var m = Matrix.Create(xx, yx, xy, yy, x0, y0);
			var a = Matrix.Create(XX, YX, XY, YY, X0, Y0);
			m.Append(a);
			Assert.IsTrue(Equals(m, Xx, Yx, Xy, Yy, a0, b0));
		}

		[TestCase(
			1, 0, 0, 1, 10, 20,
			1, 0, 0, 1, 30, 40,
			1, 0, 0, 1, 40, 60)]
		public void Matrix_Prepend_Prepends(
			float xx, float yx, float xy, float yy, float x0, float y0, // matrix
			float XX, float YX, float XY, float YY, float X0, float Y0, // prepended matrix
			float Xx, float Yx, float Xy, float Yy, float a0, float b0) // expected matrix
		{
			var m = Matrix.Create(xx, yx, xy, yy, x0, y0);
			var a = Matrix.Create(XX, YX, XY, YY, X0, Y0);
			m.Prepend(a);
			Assert.IsTrue(Equals(m, Xx, Yx, Xy, Yy, a0, b0));
		}

		[TestCase(
			1, 0, 0, 1, 0, 0,
			1, 0, 0, 1, 0, 0)]
		[TestCase(
			1, 0, 0, 1, 10, 20,
			1, 0, 0, 1, -10, -20)]
		[TestCase(
			10, 0, 0, 100, 0, 0,
			.1f, 0, 0, .01f, 0, 0)]
		public void Matrix_Invert_Inverts(
			float xx, float yx, float xy, float yy, float x0, float y0, // matrix
			float XX, float YX, float XY, float YY, float X0, float Y0)	// expected matrix
		{
			var m = Matrix.Create(xx, yx, xy, yy, x0, y0);
			m.Invert();
			Assert.IsTrue(Equals(m, XX, YX, XY, YY, X0, Y0));
		}

		[Test]
		public void Matrix_Translate_Translates()
		{
			var m = Matrix.Create();
			m.Translate(100, 200);
			Assert.IsTrue(Equals(m, 1, 0, 0, 1, 100, 200));
		}

		[TestCase(
			90, 1, 0, 0, 1, 0, 0, 0, 1, -1, 0, 0, 0)]
		public void Matrix_Rotate_Rotates(
			float degrees, 
			float xx, float yx, float xy, float yy, float x0, float y0,
			float XX, float YX, float XY, float YY, float X0, float Y0)
		{
			var m = Matrix.Create(xx, yx, xy, yy, x0, y0);
			m.Rotate(degrees);
			Assert.IsTrue(Equals(m, XX, YX, XY, YY, X0, Y0));
		}

		[TestCase(
			2, 3, 1, 0, 0, 1, 0, 0, 2, 0, 0, 3, 0, 0)]
		public void Matrix_Scale_Scales(
			float sx, float sy,
			float xx, float yx, float xy, float yy, float x0, float y0,
			float XX, float YX, float XY, float YY, float X0, float Y0)
		{
			var m = Matrix.Create(xx, yx, xy, yy, x0, y0);
			m.Scale(sx, sy);
			Assert.IsTrue(Equals(m, XX, YX, XY, YY, X0, Y0));
		}


		[TestCase(1, 1,     1, 1,      1, 0, 0, 1, 0, 0)]
		[TestCase(100, 100, 120, 160,  0.4f, 0.5f, 0.7f, 0.9f, 10, 20)]
		public void Matrix_TransformPoint_TransformsPoint(
			float x, float y, // input point
			float X, float Y, // expected transformed point
			float xx, float yx, float xy, float yy, float x0, float y0)
		{
			var m = Matrix.Create(xx, yx, xy, yy, x0, y0);
			var p = m.TransformPoint(new PointF(x, y));
			Assert.IsTrue(AreEqual(new PointF(X, Y), p));
		}
	}
}