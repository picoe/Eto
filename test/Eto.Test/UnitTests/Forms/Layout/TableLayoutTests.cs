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
				Assert.That(new Size(2, 2), Is.EqualTo(layout.Dimensions), "Table size should be 2x2");
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


				Assert.That(layout.Children, Is.EqualTo(rows.SelectMany(r => r.Cells).Select(r => r.Control)));
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
				Assert.That(table, Is.SameAs(child.Parent));
				table.Rows.Clear();
				Assert.That(child.Parent, Is.Null);
				table.Rows.Add(child);
				Assert.That(table, Is.SameAs(child.Parent));
				table.Rows.RemoveAt(0);
				Assert.That(child.Parent, Is.Null);
				table.Rows.Insert(0, child);
				Assert.That(table, Is.SameAs(child.Parent));
				table.Rows[0] = new TableRow();
				Assert.That(child.Parent, Is.Null);

				var row = new TableRow();
				row.Cells.Add(child);
				Assert.That(child.Parent, Is.Null);
				table.Rows.Add(row);
				Assert.That(table, Is.SameAs(child.Parent));
				row.Cells.Clear();
				Assert.That(child.Parent, Is.Null);
				row.Cells.Add(child);
				Assert.That(table, Is.SameAs(child.Parent));
				row.Cells.RemoveAt(0);
				Assert.That(child.Parent, Is.Null);
				row.Cells.Insert(0, child);
				Assert.That(table, Is.SameAs(child.Parent));
				row.Cells[0] = new TableCell();
				Assert.That(child.Parent, Is.Null);
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
				Assert.That(label.Width, Is.GreaterThan(0), "Label didn't get correct width!");
				Assert.That(label.Height, Is.GreaterThan(0), "Label didn't get correct height!");
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
				Assert.That(child.IsDisposed, Is.True);
				Assert.That(child.Parent, Is.Null);
				Assert.That(layout.Children, Does.Not.Contain(child));
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
				Assert.That(layout1, Is.EqualTo(child.Parent), "#1.1 Child's parent should now be the 2nd table");
				Assert.That(layout1, Is.EqualTo(child.LogicalParent), "#1.2 Child's logical parent should now be the 2nd table");

				// copy rows to a new layout
				var layout2 = new TableLayout { ID = "layout2" };
				foreach (var row in layout1.Rows.ToList())
					layout2.Rows.Add(row);

				Assert.That(layout1.Rows.Count, Is.EqualTo(0), "#2.1 All rows should now be in the 2nd table");
				Assert.That(layout2, Is.EqualTo(child.Parent), "#2.2 Child's parent should now be the 2nd table");
				Assert.That(layout2, Is.EqualTo(child.LogicalParent), "#2.3 Child's logical parent should now be the 2nd table");
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
				Assert.That(layout1, Is.EqualTo(child.Parent), "#1.1 Child's parent should now be the 2nd table");
				Assert.That(layout1, Is.EqualTo(child.LogicalParent), "#1.2 Child's logical parent should now be the 2nd table");

				// copy rows to a new layout
				var layout2 = new TableLayout { ID = "layout2" };
				var row2 = new TableRow();
				foreach (var row in layout1.Rows.ToList())
				{
					foreach (var cell in row.Cells.ToList())
						row2.Cells.Add(cell);
				}
				layout2.Rows.Add(row2);

				Assert.That(layout1.Rows.Count, Is.EqualTo(1), "#2.1 Should still have a single row");
				Assert.That(layout1.Rows[0].Cells.Count, Is.EqualTo(0), "#2.2 Cells should be removed from old table");
				Assert.That(layout2.Rows.Count, Is.EqualTo(1), "#2.3 2nd table should have a single row");
				Assert.That(layout2.Rows[0].Cells.Count, Is.EqualTo(1), "#2.4 All cells should now be in the 2nd table");
				Assert.That(layout2, Is.EqualTo(child.Parent), "#2.5 Child's parent should now be the 2nd table");
				Assert.That(layout2, Is.EqualTo(child.LogicalParent), "#2.6 Child's logical parent should now be the 2nd table");
				return layout2;
			});
		}
		
		[Test]
		public void AddingChildToAnotherPanelShouldRemoveFromTableLayout()
		{
			Invoke(() =>
			{
				var child = new Label { Text = "I should be shown!" };

				var layout1 = new TableLayout { ID = "layout1" };
				layout1.Rows.Add(child);
				Assert.That(layout1, Is.EqualTo(child.Parent), "#1.1 Child's parent should now be layout1");
				Assert.That(layout1, Is.EqualTo(child.LogicalParent), "#1.2 Child's logical parent should now be layout1");

				// use the child somewheres else
				var panel = new Panel { ID = "panel" };
				panel.Content = child;

				Assert.That(layout1.Rows.Count, Is.EqualTo(1), "#2.1 layout1 should still have a row");
				Assert.That(layout1.Children, Does.Not.Contain(child), "#2.2 Child should no longer be in layout1");

				Assert.That(panel, Is.EqualTo(child.Parent), "#2.3 Child's parent should now be panel");
				Assert.That(panel, Is.EqualTo(child.LogicalParent), "#2.4 Child's logical parent should now be panel");
				Assert.That(panel.Children, Does.Contain(child), "#2.5 Child should be in panel");
			});
		}
		
	}
}

