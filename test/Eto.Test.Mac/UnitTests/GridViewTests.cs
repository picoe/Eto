using Eto.Mac;
using Eto.Test.UnitTests;
using NUnit.Framework;

namespace Eto.Test.Mac.UnitTests
{
	public abstract class GridTests<T> : TestBase
		where T: Grid, new()
	{
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
		[TestCase(NSTableViewStyle.Inset, -1, 1)]
		[TestCase(NSTableViewStyle.Inset, -1, 3)]
		[TestCase(NSTableViewStyle.Inset, 3, 3)]
		[TestCase(NSTableViewStyle.SourceList, -1, 1)]
		[TestCase(NSTableViewStyle.SourceList, -1, 3)]
		[TestCase(NSTableViewStyle.SourceList, 3, 3)]
		[TestCase(NSTableViewStyle.Plain, -1, 1)]
		[TestCase(NSTableViewStyle.Plain, -1, 3)]
		[TestCase(NSTableViewStyle.Plain, 3, 3)]
		public void ScrollingExpandedColumnShouldKeepItsSize(NSTableViewStyle style, int intercellSpacing, int numColumns)
		{
			ManualForm("Scrolling should not cause the widths of the columns to go beyond the width of the grid,\nso the horizontal scrollbar should never show up.", form =>
			{
				form.Title = $"TesNSTableViewStyle: {style}";
				var grid = new T();
				grid.Height = 200;
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
	}
	
	[TestFixture]
	public class GridViewTests : GridTests<GridView>
	{
		protected override void SetDataStore(GridView grid, IEnumerable<object> dataStore) => grid.DataStore = dataStore;
	}

	[TestFixture]
	public class TreeGridViewTests : GridTests<TreeGridView>
	{
		protected override void SetDataStore(TreeGridView grid, IEnumerable<object> dataStore) => grid.DataStore = (ITreeGridStore<ITreeGridItem>)dataStore;
	}
}