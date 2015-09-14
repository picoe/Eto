using System;
using NUnit.Framework;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.UnitTests.Forms.Layout
{
	[TestFixture, Category(TestUtils.TestPlatformCategory)]
	public class TableLayoutTests
	{
		[Test]
		public void ConstructorWithRowsShouldHaveCorrectSize()
		{
			TestUtils.Invoke(() =>
			{
				var layout = new TableLayout(
					            new TableRow(
						            new Label(),
						            new TextBox()
					            ),
					            new TableRow(
						            new Label(),
						            new TextBox()
					            )
				            );
				Assert.AreEqual(layout.Dimensions, new Size(2, 2), "Table size should be 2x2");
			});
		}
	}
}

