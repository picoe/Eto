using Eto.Mac;
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
				if (ObjCExtensions.InstancesRespondToSelector<NSTableView>(Selector.GetHandle("style")))
					tableView.Style = style;
				else
				{
					// macos 10.15 and older
					if (style == NSTableViewStyle.SourceList)
						tableView.SelectionHighlightStyle = NSTableViewSelectionHighlightStyle.SourceList;
				}
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
		[TestCase(NSTableViewStyle.Plain, 20, "Some Text", 100)]
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
	}
}