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
	/// Unit tests for GridView's select functionality
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[TestFixture]
	public class GridViewSelectTests
	{
		GridView grid;
		ObservableCollection<DataItem> model;

		// incremented when g.SelectionChanged fires
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
				grid.DataStore = model;
				grid.SelectionChanged += (s, e) => selectionChangedCount++;
				selectionChangedCount = 0;
			});
		}

		[Test]
		public void SelectFirstRowShouldSelectFirstRow()
		{
			TestBase.Invoke(() =>
			{
				grid.SelectRow(0);
				Assert.AreEqual(model[0], grid.SelectedItem);
				Assert.AreEqual(0, grid.SelectedRows.First());
			});
		}

		[Test]
		public void SelectAllShouldSelectAllRows()
		{
			TestBase.Invoke(() =>
			{
				grid.AllowMultipleSelection = true;
				grid.SelectAll();
				Assert.AreEqual(model.Count, grid.SelectedRows.Count());
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
		public void UnselectAllShouldUnselectAllRows()
		{
			TestBase.Invoke(() =>
			{
				grid.AllowMultipleSelection = true;
				var initialCount = model.Count;

				// Select half of the items (every 2nd row)
				for (var i = 0; i < initialCount; i += 2)
					grid.SelectRow(i);

				Assert.AreEqual(initialCount / 2, grid.SelectedRows.Count(), "Number of selected items should be half of the items");

				Assert.AreEqual(initialCount / 2, selectionChangedCount, "SelectionChanged event should fire for each item selected");

				selectionChangedCount = 0;

				grid.UnselectAll();

				Assert.AreEqual(0, grid.SelectedRows.Count(), "There should be zero selected items after UnselectAll");

				Assert.AreEqual(1, selectionChangedCount, "SelectionChanged event should fire once after UnselectAll");
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
	}
}