using System;
using NUnit.Framework;
using Eto.Forms;
using Eto.Drawing;
using System.Linq;

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
	}
}

