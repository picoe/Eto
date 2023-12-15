using NUnit.Framework;
namespace Eto.Test.UnitTests.Forms.Layout
{
	[TestFixture]
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
				var rows = new[] {
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

		[Test, ManualTest]
		public void MultipleScaledRowsShouldAutoSizeCorrectly()
		{
			ManualForm("The two lines of text above should be fully visible and have equal space between them", form =>
			{
				return new TableLayout
				{
					Spacing = new Size(10, 10),
					Rows =
					{
						new TableRow(new ProgressBar { Indeterminate = true }),
						new TableRow(new Label { Text = "This is some text that should be fully visible" }) { ScaleHeight = true },
						new TableRow(new Panel () ) { ScaleHeight = true },
						new TableRow(new Label { Text = "This is some other text that should be fully visible" }) { ScaleHeight = true },
					}
				};
			});
		}

		[Test, ManualTest]
		public void ScaledRowsOfDifferentHeightShouldMakeAllRowsMaxHeight()
		{
			ManualForm("This form should be tall enough to make the blue box 80px high", form =>
			{
				var drawable = new Drawable();
				drawable.Paint += (sender, e) =>
				{
					var p = new Pen(Colors.Red, 1);
					e.Graphics.DrawRectangle(p, 0, 0, 79, 79);
				};

				var panel = new Panel { BackgroundColor = Colors.Blue, Content = drawable, Size = new Size(80, 80) };
				drawable.MouseDown += (sender, e) => panel.Visible = false;

				return new TableLayout
				{
					Spacing = new Size(10, 10),
					Rows =
					{
						new TableRow(new ProgressBar { Indeterminate = true }),
						new TableRow(new Panel { Content = new Label { Text = "There should be space below" } }) { ScaleHeight = true },
						new TableRow(panel) { ScaleHeight = true },
						new TableRow(new Panel { Content = new Label { Text = "And the blue box should be 80px high and show the red rectangle fully." } }) { ScaleHeight = true },
					}
				};
			});
		}

		[Test, ManualTest]
		public void ScaledRowSizeShouldChangeWhenInAScrollable()
		{
			ManualForm("This form should be tall enough to make the blue box 80px high, and should dissapear when clicked.", form =>
			{
				var drawable = new Drawable();
				drawable.Paint += (sender, e) =>
				{
					var p = new Pen(Colors.Red, 1);
					e.Graphics.DrawRectangle(p, 0, 0, 79, 79);
				};

				var panel = new Panel { BackgroundColor = Colors.Blue, Content = drawable, Size = new Size(80, 80) };
				drawable.MouseDown += (sender, e) => panel.Visible = false;

				return new Scrollable
				{
					ExpandContentWidth = false,
					ExpandContentHeight = false,
					Content = new TableLayout
					{
						Spacing = new Size(10, 10),
						Rows =
						{
							new TableRow(new ProgressBar { Indeterminate = true }),
							new TableRow(new Label { Text = "There should be space below" }) { ScaleHeight = true },
							new TableRow(panel) { ScaleHeight = true },
							new TableRow(new Label { Text = "And the blue box should be 80px high and show the red rectangle fully." }) { ScaleHeight = true },
						}
					}
				};
			});
		}

		/// <summary>
		/// Bug in macOS when sizing labels when parent size is fixed
		/// </summary>
		[Test]
		public void LabelInAutoSizedColumnShouldHaveCorrectWidth()
		{
			Shown(form =>
			{
				var label = new Label { Text = "Hello Then" };

				form.ClientSize = new Size(400, 200);
				form.Content = new TableLayout
				{
					Rows = {
						new TableRow(new TextBox(), label, null),
						null
					}
				};
				return label;
			}, label =>
			{
				Assert.Greater(label.Width, 0, "Label didn't get correct width!");
				Assert.Greater(label.Height, 0, "Label didn't get correct height!");
			});
		}

		[Test]
		public void DestroyingChildShouldRemoveFromParent()
		{
			TableLayout layout = null;
			Label child = null;
			Shown(form =>
			{
				child = new Label { Text = "I should not be shown" };
				layout = new TableLayout
				{
					Rows = {
						new TableRow(child)
					}
				};
				child.Dispose();
				form.Content = layout;
			}, () =>
			{
				Assert.IsTrue(child.IsDisposed);
				Assert.IsNull(child.Parent);
				CollectionAssert.DoesNotContain(layout.Children, child);
			});
		}

		[Test, ManualTest]
		public void CopyingRowsShouldReparentChildren()
		{
			ManualForm("Label above should show", form =>
			{
				var child = new Label { Text = "I should be shown!" };

				var layout1 = new TableLayout { ID = "layout1" };
				layout1.Rows.Add(child);
				Assert.AreEqual(child.Parent, layout1, "#1.1 Child's parent should now be the 2nd table");
				Assert.AreEqual(child.LogicalParent, layout1, "#1.2 Child's logical parent should now be the 2nd table");

				// copy rows to a new layout
				var layout2 = new TableLayout { ID = "layout2" };
				foreach (var row in layout1.Rows.ToList())
					layout2.Rows.Add(row);

				Assert.AreEqual(0, layout1.Rows.Count, "#2.1 All rows should now be in the 2nd table");
				Assert.AreEqual(child.Parent, layout2, "#2.2 Child's parent should now be the 2nd table");
				Assert.AreEqual(child.LogicalParent, layout2, "#2.3 Child's logical parent should now be the 2nd table");
				return layout2;
			});
		}

		[Test, ManualTest]
		public void CopyingCellsShouldReparentChildren()
		{
			ManualForm("Label above should show", form =>
			{
				var child = new Label { Text = "I should be shown!" };

				var layout1 = new TableLayout { ID = "layout1" };
				var cell1 = new TableCell(child);
				var row1 = new TableRow(child);
				layout1.Rows.Add(row1);
				Assert.AreEqual(child.Parent, layout1, "#1.1 Child's parent should now be the 2nd table");
				Assert.AreEqual(child.LogicalParent, layout1, "#1.2 Child's logical parent should now be the 2nd table");

				// copy rows to a new layout
				var layout2 = new TableLayout { ID = "layout2" };
				var row2 = new TableRow();
				foreach (var row in layout1.Rows.ToList())
				{
					foreach (var cell in row.Cells.ToList())
						row2.Cells.Add(cell);
				}
				layout2.Rows.Add(row2);

				Assert.AreEqual(1, layout1.Rows.Count, "#2.1 Should still have a single row");
				Assert.AreEqual(0, layout1.Rows[0].Cells.Count, "#2.2 Cells should be removed from old table");
				Assert.AreEqual(1, layout2.Rows.Count, "#2.3 2nd table should have a single row");
				Assert.AreEqual(1, layout2.Rows[0].Cells.Count, "#2.4 All cells should now be in the 2nd table");
				Assert.AreEqual(child.Parent, layout2, "#2.5 Child's parent should now be the 2nd table");
				Assert.AreEqual(child.LogicalParent, layout2, "#2.6 Child's logical parent should now be the 2nd table");
				return layout2;
			});
		}
	}
}

