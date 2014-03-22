using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCase = NUnit.Framework.TestCaseAttribute;
using ClassCleanup = NUnit.Framework.TestFixtureTearDownAttribute;
using ClassInitialize = NUnit.Framework.TestFixtureSetUpAttribute;
using Assert = NUnit.Framework.Assert;
using Eto.Drawing;
using Eto.Test.UnitTests.Handlers;

namespace Eto.Test.UnitTests
{
	/// <summary>
	/// Unit tests for Matrix using TestMatrixHandler.
	/// These test the test handler more than the production widget,
	/// but are useful because the test handler is an important building 
	/// block for further unit tests and needs to be bug-free.
	/// 
	/// Additionally, derived TestClasses can test platform-specific
	/// matrix handlers by overriding the matrix creation method.
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	/// </summary>	
	[TestClass]
	public class MatrixTests
	{
		protected virtual IMatrixHandler CreateMatrix()
		{
			return new TestMatrixHandler();
		}

		IMatrix Create()
		{
			var result = CreateMatrix();
			result.Create();
			return result;
		}

		IMatrix Create(float xx, float yx, float xy, float yy, float x0, float y0)
		{
			var result = CreateMatrix();
			result.Create(xx, yx, xy, yy, x0, y0);
			return result;
		}

		[TestMethod]
		public void Matrix_CreateIdentity_VerifyElements()
		{
			var m = Create();
			Assert.IsTrue(m.Elements.SequenceEqual(new float[] {1, 0, 0, 1, 0, 0}));
		}

		[TestMethod]
		public void Matrix_Translate_Translates()
		{
			var m = Create();
			m.Translate(100, 200);
			Assert.IsTrue(m.Elements.SequenceEqual(new float[] {1, 0, 0, 1, 100, 200}));
		}
	}
}
