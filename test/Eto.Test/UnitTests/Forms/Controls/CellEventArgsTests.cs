using NUnit.Framework;
namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture]
	public class CellEventArgsTests : TestBase
	{
		[Test]
		public void GridShouldBeAvailableWhileAlive()
		{
			Invoke(() =>
			{
				var cell = new TextBoxCell();
				var grid = new GridView();
				grid.Columns.Add(new GridColumn { DataCell = cell });
				var args = new CellEventArgs(grid, cell, 1, 0, "item", CellStates.None, null);

				Assert.That(args.Grid, Is.SameAs(grid));
				Assert.That(args.Cell, Is.SameAs(cell));
				Assert.That(args.GridColumn, Is.SameAs(grid.Columns[0]));
			});
		}

		[Test]
		public void ArgsShouldNotKeepGridAlive()
		{
			// Platforms keep these args with native cell views that can outlive the grid.
			CellEventArgs args = null;
			WeakReference gridReference = null;
			Invoke(() =>
			{
				var grid = new GridView();
				args = new CellEventArgs(grid, new TextBoxCell(), 1, 0, "item", CellStates.None, null);
				gridReference = new WeakReference(grid);
			});

			for (int i = 0; gridReference.IsAlive && i < 10; i++)
			{
				GC.Collect();
				GC.WaitForPendingFinalizers();
			}

			Assert.That(gridReference.IsAlive, Is.False, "Grid should be collected");
			Assert.That(args.Grid, Is.Null);
			Assert.That(args.Item, Is.EqualTo("item"));
		}
	}
}
