using System;
using NUnit.Framework;
using Eto.Forms;
using System.Linq;
using Eto.Drawing;

namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture]
	public class TreeGridViewTests : TestBase
	{
		[Test]
		public void SelectedItemsShouldNotCrash()
		{
			Invoke(() =>
			{
				var tree = new TreeGridView();

				tree.DataStore = new TreeGridItemCollection();

				var items = tree.SelectedItems.ToList();
				Assert.IsNotNull(items);
				Assert.AreEqual(0, items.Count);
			});
		}

		/// <summary>
		/// Issue #814
		/// </summary>
		[Test]
		public void ReloadDataWhenContentIsFocusedShouldNotCrash()
		{
			LinkButton hyperlink = null;
			TreeGridView tree = null;
			Shown(form =>
			{
				tree = new TreeGridView();

				tree.DataStore = new TreeGridItemCollection();
				hyperlink = new LinkButton { Text = "Some Hyperlink" };

				form.Content = new TableLayout
				{
					Rows = {
						tree,
						hyperlink
					}
				};
			}, () =>
			{
				hyperlink.Focus();
				tree.ReloadData(); // crashes on WPF.
			});
		}

		[Test, ManualTest]
		public void DrawableCellAsFirstColumnShouldNotBeWhite()
		{
			ManualForm("Both cells should show the same", form =>
			{
				TreeGridView tree = new TreeGridView();

				// add first drawable (it will be rendered all white!
				var drawableCell1 = new DrawableCell();
				drawableCell1.Paint += drawableCell_Paint;
				tree.Columns.Add(new GridColumn { HeaderText = "not working", DataCell = drawableCell1, Width = 200 });

				// add the second drawable
				var drawableCell2 = new DrawableCell();
				drawableCell2.Paint += drawableCell_Paint;
				tree.Columns.Add(new GridColumn { HeaderText = "working", DataCell = drawableCell2, Width = 200 });

				// add some data to the tree just to populate it
				TreeGridItemCollection model = new TreeGridItemCollection();
				TreeGridItem item1 = new TreeGridItem();
				item1.Values = new object[] { "text" };
				TreeGridItem item2 = new TreeGridItem();
				item2.Values = new object[] { "text" };
				item1.Children.Add(item2);
				item1.Expanded = true;
				model.Add(item1);

				tree.DataStore = model;

				return tree;
			});
		}

		static void drawableCell_Paint(object sender, CellPaintEventArgs e)
		{
			var m = e.Item as TreeGridItem;
			if (m != null)
			{
				string text = (string)m.Values[0];
				var rect = e.ClipRectangle;
				rect.Width--;
				rect.Height--;
				e.Graphics.DrawText(SystemFonts.Label(), Colors.Black, (float)0, (float)0, text != null ? text : string.Empty);
				e.Graphics.DrawLine(Colors.Blue, rect.TopLeft, rect.BottomRight);
				e.Graphics.DrawLine(Colors.Blue, rect.TopRight, rect.BottomLeft);
				e.Graphics.DrawRectangle(Colors.Green, rect);
			}
		}
	}
}
