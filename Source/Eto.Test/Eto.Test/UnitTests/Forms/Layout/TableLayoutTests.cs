using System;
using NUnit.Framework;
using Eto.Forms;
using Eto.Drawing;
using System.Linq;

namespace Eto.Test.UnitTests.Forms.Layout
{
	[TestFixture, Category(TestBase.TestPlatformCategory)]
	public class TableLayoutTests : TestBase
	{
		[Test]
		public void ConstructorWithRowsShouldHaveCorrectSize()
		{
			Invoke(() =>
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

		[Test]
		public void AddRowsShouldSetChildren()
		{
			Invoke(() =>
			{
				var rows = new [] {
					new TableRow(new Label(), new TextBox()),
					new TableRow(new Label(), new TextBox())
				};

				var layout = new TableLayout();
				foreach (var row in rows)
					layout.Rows.Add(row);


				CollectionAssert.AreEqual(rows.SelectMany(r => r.Cells).Select(r => r.Control), layout.Children);
			});
		}

		[Test]
		public void LogicalParentShouldChangeWhenAddedOrRemoved()
		{
			Invoke(() =>
			{
				var child = new Panel();
				var table = new TableLayout();

				// adding/removing rows
				table.Rows.Add(child);
				Assert.AreSame(table, child.Parent);
				table.Rows.Clear();
				Assert.IsNull(child.Parent);
				table.Rows.Add(child);
				Assert.AreSame(table, child.Parent);
				table.Rows.RemoveAt(0);
				Assert.IsNull(child.Parent);
				table.Rows.Insert(0, child);
				Assert.AreSame(table, child.Parent);
				table.Rows[0] = new TableRow();
				Assert.IsNull(child.Parent);

				var row = new TableRow();
				row.Cells.Add(child);
				Assert.IsNull(child.Parent);
				table.Rows.Add(row);
				Assert.AreSame(table, child.Parent);
				row.Cells.Clear();
				Assert.IsNull(child.Parent);
				row.Cells.Add(child);
				Assert.AreSame(table, child.Parent);
				row.Cells.RemoveAt(0);
				Assert.IsNull(child.Parent);
				row.Cells.Insert(0, child);
				Assert.AreSame(table, child.Parent);
				row.Cells[0] = new TableCell();
				Assert.IsNull(child.Parent);
			});
		}


	}
}

