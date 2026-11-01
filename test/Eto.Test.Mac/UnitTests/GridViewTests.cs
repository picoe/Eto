using Eto.Mac;
using Eto.Mac.Forms.Controls;
using Eto.Test.UnitTests;
using NUnit.Framework;

namespace Eto.Test.Mac.UnitTests
{
	public abstract class GridTests<T> : TestBase
		where T: Grid, new()
	{
		protected abstract Test.UnitTests.Forms.Controls.GridTests<T> BaseGridTests { get; }
		
		class GridTestItem : TreeGridItem
		{
			public string Text { get; set; }
			
			public Image Image { get; set; }

			public override string ToString() => Text ?? base.ToString();
		}
		
		protected abstract void SetDataStore(T grid, IEnumerable<object> dataStore);

		public IEnumerable<object> CreateDataStore(int rows = 40)
		{
			var list = new TreeGridItemCollection();
			Image logo = TestIcons.Logo;
			Image testImage = TestIcons.TestImage;
			for (int i = 0; i < rows; i++)
			{
				Image image = i % 2 == 0 ? logo : testImage;
				list.Add(new GridTestItem { Text = $"Item {i}", Image = image, Values = new[] { $"col {i}.2", $"col {i}.3", $"col {i}.4", $"col {i}.5" } });
			}
			return list;
		}
		
		[ManualTest]
		[TestCase(NSTableViewStyle.FullWidth, -1, 1)]
		[TestCase(NSTableViewStyle.FullWidth, -1, 3)]
		[TestCase(NSTableViewStyle.FullWidth, 3, 3)]
		[TestCase(NSTableViewStyle.FullWidth, 0, 3)]
		[TestCase(NSTableViewStyle.Inset, -1, 1)]
		[TestCase(NSTableViewStyle.Inset, -1, 3)]
		[TestCase(NSTableViewStyle.Inset, 3, 3)]
		[TestCase(NSTableViewStyle.Inset, 0, 3)]
		[TestCase(NSTableViewStyle.SourceList, -1, 1)]
		[TestCase(NSTableViewStyle.SourceList, -1, 3)]
		[TestCase(NSTableViewStyle.SourceList, 3, 3)]
		[TestCase(NSTableViewStyle.SourceList, 0, 3)]
		[TestCase(NSTableViewStyle.Plain, -1, 1)]
		[TestCase(NSTableViewStyle.Plain, -1, 3)]
		[TestCase(NSTableViewStyle.Plain, 20, 3)]
		[TestCase(NSTableViewStyle.Plain, 3, 3)]
		[TestCase(NSTableViewStyle.Plain, 0, 3)]
		public void ScrollingExpandedColumnShouldKeepItsSize(NSTableViewStyle style, int intercellSpacing, int numColumns)
		{
			ManualForm("Scrolling should not cause the widths of the columns to go beyond the width of the grid,\nso the horizontal scrollbar should never show up.", form =>
			{
				form.Title = $"TesNSTableViewStyle: {style}";
				var grid = new T();
				grid.Height = 200;
				SetParameters(style, intercellSpacing, grid);

				SetDataStore(grid, CreateDataStore());

				grid.Columns.Add(new GridColumn { DataCell = new ImageTextCell { TextBinding = Binding.Property((GridTestItem m) => m.Text), ImageBinding = Binding.Property((GridTestItem m) => m.Image) } });
				for (int i = 0; i < numColumns; i++)
					grid.Columns.Add(new GridColumn { DataCell = new TextBoxCell(i) });

				var expandColumn = grid.Columns[0];
				expandColumn.HeaderText = "Expanded";
				expandColumn.Expand = true;

				return grid;
			});
		}

		private static void SetParameters(NSTableViewStyle style, int intercellSpacing, T grid)
		{
			if (grid.ControlObject is NSTableView tableView)
			{
				tableView.Style = style;
				if (intercellSpacing >= 0)
					tableView.IntercellSpacing = new CGSize(intercellSpacing, 2);
			}
		}

		[ManualTest]
		[TestCase(NSTableViewStyle.FullWidth, -1, "Some Text", 100, 180)]
		[TestCase(NSTableViewStyle.FullWidth, -1, "Some Text", 15, -1)]
		[TestCase(NSTableViewStyle.FullWidth, -1, "Some Much Longer Text That Should Still Work", 100, 180)]
		[TestCase(NSTableViewStyle.Inset, -1, "Some Text", 100, 180)]
		[TestCase(NSTableViewStyle.Inset, -1, "Some Text", 15, -1)]
		[TestCase(NSTableViewStyle.Inset, -1, "Some Much Longer Text That Should Still Work", 100, 180)]
		[TestCase(NSTableViewStyle.SourceList, -1, "Some Text", 100, 180)]
		[TestCase(NSTableViewStyle.SourceList, -1, "Some Text", 15, -1)]
		[TestCase(NSTableViewStyle.SourceList, -1, "Some Much Longer Text That Should Still Work", 100, 180)]
		[TestCase(NSTableViewStyle.Plain, -1, "Some Text", 100, 180)]
		[TestCase(NSTableViewStyle.Plain, -1, "Some Text", 15, -1)]
		[TestCase(NSTableViewStyle.Plain, -1, "Some Much Longer Text That Should Still Work", 100, 180)]
		[TestCase(NSTableViewStyle.Plain, 20, "Some Text", 100, -1)]
		[TestCase(NSTableViewStyle.Plain, 20, "Some Text", 15, -1)]
		[TestCase(NSTableViewStyle.Plain, 3, "Some Text", 100, 180)]
		[TestCase(NSTableViewStyle.Plain, 3, "Some Text", 15, -1)]
		[TestCase(NSTableViewStyle.Plain, 0, "Some Much Longer Text That Should Still Work", 100, 180)]
		public void AutoSizedColumnShouldChangeSizeOfControl(NSTableViewStyle style, int intercellSpacing, string text, int rows, int height)
		{
			BaseGridTests.AutoSizedColumnShouldChangeSizeOfControl(text, rows, height, grid => {
				SetParameters(style, intercellSpacing, grid);
			});
		}
	}
	
	[TestFixture]
	public class GridViewTests : GridTests<GridView>
	{
		protected override Test.UnitTests.Forms.Controls.GridTests<GridView> BaseGridTests => new Test.UnitTests.Forms.Controls.GridViewTests();
		protected override void SetDataStore(GridView grid, IEnumerable<object> dataStore) => grid.DataStore = dataStore;
	}

	[TestFixture]
	public class TreeGridViewTests : GridTests<TreeGridView>
	{
		protected override Test.UnitTests.Forms.Controls.GridTests<TreeGridView> BaseGridTests => new Test.UnitTests.Forms.Controls.TreeGridViewTests();
		protected override void SetDataStore(TreeGridView grid, IEnumerable<object> dataStore) => grid.DataStore = (ITreeGridStore<ITreeGridItem>)dataStore;

		static bool IsGroupRow(NSOutlineView outlineView, int row) => outlineView.GetRowView(row, true)?.GroupRowStyle == true;

		[Test]
		public void NestedCollapseShouldNotSuppressSelectionChanges()
		{
			var child = new TreeGridItem("Child");
			var parent = new TreeGridItem(new[] { child }, "Parent") { Expanded = true };
			var other = new TreeGridItem(new[] { new TreeGridItem("Other child") }, "Other") { Expanded = true };
			Shown(form =>
			{
				var tree = new TreeGridView { DataStore = new TreeGridItemCollection { parent, other } };
				tree.Columns.Add(new GridColumn { DataCell = new TextBoxCell(0) });
				form.Content = tree;
				return tree;
			}, tree =>
			{
				tree.SelectedItem = child;
				var changes = new List<object>();
				var selectionChanges = 0;
				tree.SelectedItemChanged += (_, _) => changes.Add(tree.SelectedItem);
				tree.SelectionChanged += (_, _) => selectionChanges++;
				var handler = (TreeGridViewHandler)tree.Handler;
				var nativeParent = handler.Control.ItemAtRow(0);
				var outlineDelegate = (TreeGridViewHandler.EtoOutlineDelegate)handler.Control.Delegate;
				tree.Collapsed += (_, e) =>
				{
					if (ReferenceEquals(e.Item, parent))
					{
						var nativeOther = handler.Control.ItemAtRow(1);
						outlineDelegate.ShouldCollapseItem(handler.Control, nativeOther);
						handler.Control.CollapseItem(nativeOther, false);
					}
				};
				// Programmatic collapse can bypass AppKit's permission callback.
				Assert.That(outlineDelegate.ShouldCollapseItem(handler.Control, nativeParent), Is.True);
				handler.Control.CollapseItem(nativeParent, false);
				Assert.That(tree.SelectedItem, Is.SameAs(parent));
				Assert.That(changes, Is.EqualTo(new[] { parent }));
				Assert.That(selectionChanges, Is.EqualTo(1));
				tree.SelectedItem = other;
				Assert.That(changes, Is.EqualTo(new[] { parent, other }));
				Assert.That(selectionChanges, Is.EqualTo(2));
			});
		}

		[Test]
		public void ShowGroupsShouldApplyWhenToggled()
		{
			TreeGridViewHandler handler = null;
			bool groupBefore = false;
			bool groupAfter = false;

			Shown(form =>
			{
				var tree = new TreeGridView { Size = new Size(200, 200) };
				tree.Columns.Add(new GridColumn { DataCell = new TextBoxCell(0) });
				tree.DataStore = new TreeGridItemCollection
				{
					new TreeGridItem(new TreeGridItem("Child")) { Values = new[] { "Group" }, Expanded = true }
				};
				handler = (TreeGridViewHandler)tree.Handler;
				form.Content = tree;
			}, () =>
			{
				groupBefore = IsGroupRow(handler.Control, 0);
				handler.ShowGroups = true;
				handler.Control.ReloadData();
				handler.Control.Layout();
				groupAfter = IsGroupRow(handler.Control, 0);
			});

			Assert.Multiple(() =>
			{
				Assert.That(groupBefore, Is.False, "Top level rows should not be group rows by default");
				Assert.That(groupAfter, Is.True, "Top level rows should become group rows when ShowGroups is turned on");
			});
		}
	}
}
