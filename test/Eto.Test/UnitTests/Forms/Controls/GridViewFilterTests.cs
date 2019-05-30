using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Eto.Forms;
using System;
using System.Collections.ObjectModel;
using Eto.Drawing;
using System.Threading.Tasks;

namespace Eto.Test.UnitTests.Forms.Controls
{
	/// <summary>
	/// Unit tests for a GridView using a <see cref="FilterCollection{T}"/>
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[TestFixture]
	public class GridViewFilterTests
	{
		GridView grid;
		ObservableCollection<DataItem> model;
		FilterCollection<DataItem> filtered;

		// incremented when grid.SelectionChanged fires
		int selectionChangedCount;

		[SetUp]
		public void Setup()
		{
			TestBase.Invoke(() =>
			{
				grid = new GridView();
				// Some platforms need at least one column for selection to work
				grid.Columns.Add(new GridColumn { HeaderText = "Text", DataCell = new TextBoxCell("Id") });
				model = GridViewUtils.CreateModel();

				// create our filtered collection
				filtered = new FilterCollection<DataItem>(model);
				filtered.Change = () => grid.SelectionPreserver;
				grid.DataStore = filtered;
				grid.SelectionChanged += (s, e) => selectionChangedCount++;
				selectionChangedCount = 0;
			});
		}

		[Test]
		public void InsertItemShouldNotChangeSelection()
		{
			TestBase.Invoke(() =>
			{
				grid.SelectRow(0);
				var selectedItem = grid.SelectedItem;
				model.Insert(0, new DataItem(model.Count));
				Assert.AreEqual(selectedItem, grid.SelectedItem);
			});
		}

		[Test]
		public void DeleteSelectedItemsShouldRemoveSelectedItems()
		{
			TestBase.Invoke(() =>
			{
				grid.AllowMultipleSelection = true;

				var initialCount = model.Count;

				// Select the first half of items
				for (var i = 0; i < initialCount / 2; i++)
					grid.SelectRow(i);

				Assert.AreEqual(initialCount / 2, grid.SelectedRows.Count(), "Number of selected items should be half of the items");

				Assert.AreEqual(initialCount / 2, selectionChangedCount, "SelectionChanged event should fire for each item selected");

				// reset to test events fired when removing
				selectionChangedCount = 0;

				// Delete alternate items
				for (var i = initialCount - 1; i >= 0; i -= 2)
					model.RemoveAt(i);

				Assert.AreEqual(initialCount / 4, grid.SelectedRows.Count(), "Number of selected items should be quarter of the original items");
				var expectedSelectedItemIds = new List<int>();
				for (var i = 0; i < initialCount / 2; i += 2)
					expectedSelectedItemIds.Add(i);
				Assert.IsTrue(expectedSelectedItemIds.SequenceEqual(grid.SelectedItems.OfType<DataItem>().Select(x => x.Id).OrderBy(r => r)), "Items don't match");

				Assert.AreEqual(initialCount / 4, selectionChangedCount, "SelectionChanged event should fire for each selected item removed");
			});
		}

		[TestCase(0)]
		[TestCase(2)]
		public void SortItemsShouldNotChangeSelection(int rowToSelect)
		{
			TestBase.Invoke(() =>
			{
				filtered.Sort = GridViewUtils.SortItemsAscending;
				grid.SelectRow(rowToSelect);
				var selectedItem = filtered[rowToSelect];

				Assert.AreEqual(1, grid.SelectedRows.Count(), "The row was not selected");
				Assert.AreEqual(selectedItem, grid.SelectedItem, "The correct item was not selected");
				Assert.AreEqual(1, selectionChangedCount, "SelectionChanged event should fire once for the selected row");

				selectionChangedCount = 0; // reset the count
				filtered.Sort = GridViewUtils.SortItemsDescending;

				Assert.AreEqual(1, grid.SelectedRows.Count(), "There should still only be a single selected row");
				Assert.AreEqual(selectedItem, grid.SelectedItem, "The selected item should remain the same");

				Assert.AreEqual(0, selectionChangedCount, "SelectionChanged event should not fire when changing the Sort");
			});
		}

		[Test]
		public void FilterItemsShouldUnselectFilteredItems()
		{
			TestBase.Invoke(() =>
			{
				grid.AllowMultipleSelection = true;

				// Select the first half of items
				for (var i = 0; i < model.Count / 2; i++)
					grid.SelectRow(i);

				selectionChangedCount = 0; // reset the count
				filtered.Filter = GridViewUtils.KeepOddItemsFilter;

				Assert.AreEqual(model.Count / 4, grid.SelectedRows.Count(), "A quarter of the items should be selected");

				var selectedItems = grid.SelectedItems.OfType<DataItem>().OrderBy(r => r.Id).ToList();
				var expectedItems = model.Where((item, row) => row < model.Count / 2 && (row % 2) == 1).ToList();
				Assert.IsTrue(expectedItems.SequenceEqual(selectedItems), "Selected items should only contain items left after filtering");

				Assert.AreEqual(1, selectionChangedCount, "SelectionChanged event should fire when changing the Filter which removes items");
			});
		}
	}
}