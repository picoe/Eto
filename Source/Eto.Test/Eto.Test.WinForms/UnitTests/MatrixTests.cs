using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Eto.Drawing;

namespace Eto.Test.WinForms.UnitTests
{
	[TestFixture]
	public class MatrixTests : Eto.Test.UnitTests.Drawing.MatrixTests
	{
		protected override IMatrix Create()
		{
			var matrix = new Eto.Platform.Windows.Drawing.MatrixHandler();
			matrix.Create();
			return matrix;
		}

		protected override IMatrix Create(float xx, float yx, float xy, float yy, float x0, float y0)
		{
			var matrix = new Eto.Platform.Windows.Drawing.MatrixHandler();
			matrix.Create(xx, yx, xy, yy, x0, y0);
			return matrix;
		}
	}
}
