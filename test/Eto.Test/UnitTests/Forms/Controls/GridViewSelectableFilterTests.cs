using NUnit.Framework;
namespace Eto.Test.UnitTests.Forms.Controls
{
	/// <summary>
	/// Unit tests for a GridView using a <see cref="SelectableFilterCollection{T}"/>
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[TestFixture]
	public class GridViewSelectableFilterTests : TestBase
	{
		GridView grid;
		ObservableCollection<DataItem> model;
		SelectableFilterCollection<DataItem> filtered;

		// incremented when grid.SelectionChanged fires
		int modelSelectionChangedCount;
		int viewSelectionChangedCount;

		[SetUp]
		public void Setup()
		{
			Invoke(() =>
			{
				grid = new GridView();
				// Some platforms need at least one column for selection to work
				grid.Columns.Add(new GridColumn { HeaderText = "Text", DataCell = new TextBoxCell("Id") });
				model = GridViewUtils.CreateModel();

				// create our filtered collection
				filtered = new SelectableFilterCollection<DataItem>(grid, model);
				filtered.SelectionChanged += (s, e) => modelSelectionChangedCount++;
				grid.DataStore = filtered;
				grid.SelectionChanged += (s, e) => viewSelectionChangedCount++;
				modelSelectionChangedCount = viewSelectionChangedCount = 0;
			});
		}

		[Test]
		public void InsertItemShouldNotChangeSelection()
		{
			Invoke(() =>
			{
				grid.SelectRow(0);
				var selectedItem = grid.SelectedItem;
				model.Insert(0, new DataItem(model.Count));
				Assert.That(grid.SelectedItem, Is.EqualTo(selectedItem));
			});
		}

		[Test]
		public void DeleteSelectedItemsShouldRemoveSelectedItems()
		{
			Invoke(() =>
			{
				grid.AllowMultipleSelection = true;

				var initialCount = model.Count;

				// Select the first half of items
				for (var i = 0; i < initialCount / 2; i++)
					grid.SelectRow(i);

				Assert.That(grid.SelectedRows.Count(), Is.EqualTo(initialCount / 2), "Number of selected items should be half of the items");
				Assert.That(viewSelectionChangedCount, Is.EqualTo(initialCount / 2), "View SelectionChanged event should fire for each item selected");
				Assert.That(modelSelectionChangedCount, Is.EqualTo(initialCount / 2), "Model SelectionChanged event should fire for each item selected");

				// reset to test events fired when removing
				modelSelectionChangedCount = viewSelectionChangedCount = 0;

				// Delete alternate items
				for (var i = initialCount - 1; i >= 0; i -= 2)
					model.RemoveAt(i);

				Assert.That(grid.SelectedRows.Count(), Is.EqualTo(initialCount / 4), "Number of selected items should be quarter of the original items");
				var expectedSelectedItemIds = new List<int>();
				for (var i = 0; i < initialCount / 2; i += 2)
					expectedSelectedItemIds.Add(i);
				Assert.That(expectedSelectedItemIds.SequenceEqual(grid.SelectedItems.OfType<DataItem>().Select(x => x.Id).OrderBy(r => r)), Is.True, "Items don't match");

				Assert.That(viewSelectionChangedCount, Is.EqualTo(initialCount / 4), "View SelectionChanged event should fire for each selected item removed");
				Assert.That(modelSelectionChangedCount, Is.EqualTo(initialCount / 4), "Model SelectionChanged event should fire for each selected item removed");
			});
		}

		[TestCase(0)]
		[TestCase(2)]
		public void SortItemsShouldNotChangeSelection(int rowToSelect)
		{
			Invoke(() =>
			{
				filtered.Sort = GridViewUtils.SortItemsAscending;
				grid.SelectRow(rowToSelect);
				var selectedItem = filtered[rowToSelect];

				Assert.That(grid.SelectedRows.Count(), Is.EqualTo(1), "The row was not selected");
				Assert.That(grid.SelectedItem, Is.EqualTo(selectedItem), "The correct item was not selected");
				Assert.That(viewSelectionChangedCount, Is.EqualTo(1), "SelectionChanged event should fire once for the selected row");

				viewSelectionChangedCount = 0; // reset the count
				filtered.Sort = GridViewUtils.SortItemsDescending;

				Assert.That(grid.SelectedRows.Count(), Is.EqualTo(1), "There should still only be a single selected row");
				Assert.That(grid.SelectedItem, Is.EqualTo(selectedItem), "The selected item should remain the same");

				Assert.That(viewSelectionChangedCount, Is.EqualTo(0), "SelectionChanged event should not fire when changing the Sort");
			});
		}

		[Test]
		public void FilterItemsShouldNotChangeSelection()
		{
			Invoke(() =>
			{
				grid.AllowMultipleSelection = true;

				// Select the first half of items
				for (var i = 0; i < model.Count / 2; i++)
					grid.SelectRow(i);

				viewSelectionChangedCount = modelSelectionChangedCount = 0; // reset the counts
				filtered.Filter = GridViewUtils.KeepOddItemsFilter;

				Assert.That(grid.SelectedRows.Count(), Is.EqualTo(model.Count / 4), "A quarter of the items should be selected");

				// view's selected items should change
				var selectedItems = grid.SelectedItems.OfType<DataItem>().OrderBy(r => r.Id).ToList();
				var expectedItems = model.Where((item, row) => row < model.Count / 2 && (row % 2) == 1).ToList();
				Assert.That(expectedItems.SequenceEqual(selectedItems), Is.True, "Selected items should only contain items left after filtering");
				Assert.That(viewSelectionChangedCount, Is.EqualTo(1), "View SelectionChanged event should fire when changing the Filter which removes items");

				// model's selected items should not have changed
				selectedItems = filtered.SelectedItems.OfType<DataItem>().OrderBy(r => r.Id).ToList();
				expectedItems = model.Where((item, row) => row < model.Count / 2).ToList();
				Assert.That(expectedItems.SequenceEqual(selectedItems), Is.True, "Model's selected items should not have changed");
				Assert.That(modelSelectionChangedCount, Is.EqualTo(0), "Model SelectionChanged event should not fire when changing filter");
			});
		}
		
		[Test]
		public void SortedCollectionShouldGetCorrectRow()
		{
			GridView grid = null;
			Shown(form =>
			{
				grid = new GridView { Size = new Size(200, 200) };
				var collection = new SelectableFilterCollection<GridItem>(grid)
				{
					new("Hello"),
					new("There"),
					new("Fine"),
					new("World"),
					new("Of"),
					new("Eto")
				};
				
				grid.Columns.Add(new GridColumn
				{
					DataCell = new TextBoxCell(0)
				});
				grid.DataStore = collection;
				collection.Sort = (x, y) => string.Compare((string)x.Values[0], (string)y.Values[0], StringComparison.Ordinal);
				// goes to this order:
				// Eto
				// Fine
				// Hello
				// Of
				// There
				// World

				collection.SelectRow(4);
				form.Content = grid;
			}, () => {
				
				Assert.That(grid.SelectedRow, Is.EqualTo(3), "#1");
				Assert.That(grid.SelectedItem, Is.Not.Null, "#2");
				Assert.That(((GridItem)grid.SelectedItem).Values[0], Is.EqualTo("Of"), "#3");
			});
		}
		
	}
}