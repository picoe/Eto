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

namespace Eto.Test.WinForms.UnitTests
{
	[TestClass]
	public class MatrixTests : Eto.Test.UnitTests.MatrixTests
	{
		protected override IMatrixHandler CreateMatrix()
		{
			return new Eto.Platform.Windows.Drawing.MatrixHandler();
		}
	}
}
