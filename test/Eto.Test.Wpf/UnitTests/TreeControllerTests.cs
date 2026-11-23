using Eto.CustomControls;
using Eto.Test.UnitTests;
using NUnit.Framework;

namespace Eto.Test.Wpf.UnitTests;

[TestFixture]
public class TreeControllerTests : TestBase
{
	class TestTreeHandler : ITreeHandler
	{
		public ITreeGridItem SelectedItem => null;
		public bool AllowMultipleSelection => false;
		public void SelectRow(int row) { }
		public bool PreResetTree(object item = null, int row = -1) => false;
		public void PostResetTree(object item = null, int row = -1) { }
	}

	static TreeGridItemCollection CreateItems(int count)
	{
		var items = new TreeGridItemCollection();
		for (int i = 0; i < count; i++)
		{
			var item = new TreeGridItem { Values = new object[] { $"Item {i}" } };
			item.Children.Add(new TreeGridItem { Values = new object[] { $"Child {i}" } });
			items.Add(item);
		}
		return items;
	}

	// The store can shrink without the tree being reloaded, so a toggle can arrive for a row that no longer exists.
	[TestCase(false)]
	[TestCase(true)]
	public void ExpandingRowRemovedFromStoreShouldNotCrash(bool withExpandedRow)
	{
		var items = CreateItems(5);
		if (withExpandedRow)
			items[0].Expanded = true;
		var controller = new TreeController { Handler = new TestTreeHandler() };
		controller.InitializeItems(items);

		var lastRow = controller.Count - 1;
		var lastItem = controller[lastRow];
		var resets = 0;
		controller.CollectionChanged += (sender, e) => resets += e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset ? 1 : 0;

		while (items.Count > 2)
			items.RemoveAt(items.Count - 1);

		Assert.That(controller.IndexOf(lastItem), Is.EqualTo(lastRow), "#1 stale row should still be cached");
		Assert.That(controller.ExpandRow(lastRow), Is.False, "#2");
		Assert.That(controller.CollapseRow(lastRow), Is.False, "#3");
		Assert.That(lastItem.Expanded, Is.False, "#4");
		Assert.That(resets, Is.EqualTo(0), "#5 tree should be left for the app to reload");

		controller.ReloadData();
		Assert.That(controller.Count, Is.EqualTo(withExpandedRow ? 3 : 2), "#6");
		Assert.That(controller.IndexOf(lastItem), Is.EqualTo(-1), "#7");
	}

	[Test]
	public void ExpandingAndCollapsingRowShouldStillWork()
	{
		var items = CreateItems(3);
		var controller = new TreeController { Handler = new TestTreeHandler() };
		controller.InitializeItems(items);

		Assert.That(controller.ExpandRow(1), Is.True, "#1");
		Assert.That(controller.Count, Is.EqualTo(4), "#2");
		Assert.That(controller[2], Is.SameAs(((TreeGridItem)items[1]).Children[0]), "#3");
		Assert.That(controller.CollapseRow(1), Is.True, "#4");
		Assert.That(controller.Count, Is.EqualTo(3), "#5");
	}
}
