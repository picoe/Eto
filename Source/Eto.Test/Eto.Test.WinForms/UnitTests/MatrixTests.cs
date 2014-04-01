using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Eto.Drawing;

namespace Eto.Test.WinForms.UnitTests
{
	[TestFixture]
	public class MatrixTests : Eto.UnitTest.Drawing.MatrixTests
	{
		protected override IMatrixHandler CreateMatrix()
		{
			return new Eto.Platform.Windows.Drawing.MatrixHandler();
		}
	}
}
