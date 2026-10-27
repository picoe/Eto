using Eto.Drawing;
using Eto.Forms;
using Eto.Test.UnitTests;
using NUnit.Framework;

namespace Eto.Test.Wpf.UnitTests;

/// <summary>
/// Regression coverage for the b46b9827 scrollable-fill regression. The fix lives in the shared
/// WpfFrameworkElement.MeasureOverride, so the enclosed ScrollViewer's viewport should track the
/// grid's actual (arranged) size: a viewport much smaller than the control means the content
/// doesn't fill the scrollable area (the regression); much larger means it measured against the
/// auto-size monitor probe and won't scroll correctly.
/// </summary>
public abstract class GridScrollableFillTests<T> : TestBase
	where T : Grid, new()
{
	class GridTestItem : TreeGridItem
	{
		public string Text { get; set; }

		public override string ToString() => Text ?? base.ToString();
	}

	protected abstract void SetDataStore(T grid, IEnumerable<object> dataStore);

	static TChild FindChild<TChild>(sw.DependencyObject parent)
		where TChild : sw.DependencyObject
	{
		if (parent == null)
			return null;
		var count = sw.Media.VisualTreeHelper.GetChildrenCount(parent);
		for (int i = 0; i < count; i++)
		{
			var child = sw.Media.VisualTreeHelper.GetChild(parent, i);
			var found = child as TChild ?? FindChild<TChild>(child);
			if (found != null)
				return found;
		}
		return null;
	}

	void AssertViewportTracksActualSize(bool autoSize)
	{
		ShownAsync(form =>
		{
			if (autoSize)
			{
				form.Resizable = true;
				form.AutoSize = true;
			}
			else
			{
				form.ClientSize = new Size(500, 400);
			}

			var grid = new T { ShowHeader = false, Size = new Size(150, 100) };
			grid.Columns.Add(new GridColumn { DataCell = new TextBoxCell { Binding = Binding.Property((GridTestItem m) => m.Text) }, AutoSize = true });

			var list = new TreeGridItemCollection();
			for (int i = 0; i < 100; i++)
				list.Add(new GridTestItem { Text = $"Item {i}" });
			SetDataStore(grid, list);

			// stretch + expand so the grid fills the window (larger than its 150x100 preferred size)
			form.Content = new StackLayout { HorizontalContentAlignment = HorizontalAlignment.Stretch, Items = { new StackLayoutItem(grid, true) } };
			return grid;
		}, async grid =>
		{
			await Task.Delay(700);
			var control = grid.ControlObject as sw.FrameworkElement;
			var scrollViewer = FindChild<swc.ScrollViewer>(control);
			Assert.That(scrollViewer, Is.Not.Null, "Could not find the ScrollViewer in the grid");

			var actualWidth = control.ActualWidth;
			var viewportWidth = scrollViewer.ViewportWidth;

			Assert.That(viewportWidth, Is.GreaterThan(actualWidth * 0.6).And.LessThan(actualWidth * 1.2),
				$"ScrollViewer viewport width ({viewportWidth}) should track the grid's actual width ({actualWidth})");
		}, timeout: -1);
	}

	[Test]
	public void ContentShouldFillScrollableAreaWhenLargerThanPreferredSize() => AssertViewportTracksActualSize(autoSize: false);

	[Test]
	public void ContentShouldFillScrollableAreaInAutoSizedWindow() => AssertViewportTracksActualSize(autoSize: true);
}

[TestFixture]
public class GridViewScrollableFillTests : GridScrollableFillTests<GridView>
{
	protected override void SetDataStore(GridView grid, IEnumerable<object> dataStore) => grid.DataStore = dataStore;
}

[TestFixture]
public class TreeGridViewScrollableFillTests : GridScrollableFillTests<TreeGridView>
{
	protected override void SetDataStore(TreeGridView grid, IEnumerable<object> dataStore) => grid.DataStore = (ITreeGridStore<ITreeGridItem>)dataStore;
}
