using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Eto.Forms;
using System;

namespace Eto.Test.UnitTests.Forms
{
	/// <summary>
	/// Unit tests for GridViewSelection
	/// </summary>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[TestFixture]
	public class GridViewSelectionTests
	{
		static int ItemCount { get { return GridViewUtils.ItemCount; } }
		GridView grid;
		GridView.IHandler handler;
		DataStoreCollection model;
		int selectionChangedCount; // incremented when g.SelectionChanged fires

		[SetUp]
		public void Setup()
		{
			TestUtils.Invoke(() =>
			{
				grid = new GridView();
				handler = (GridView.IHandler)grid.Handler;
				model = GridViewUtils.CreateModel();
				grid.DataStore = model;
				grid.SelectionChanged += (s, e) => selectionChangedCount++;
			});
		}

		[Test]
		public void GridViewSelection_SelectFirstRow_SelectsFirstRow()
		{
			TestUtils.Invoke(() =>
			{
				grid.SelectRow(0);
				Assert.AreEqual(model[0], grid.SelectedItem);
				Assert.AreEqual(0, grid.SelectedRows.ToList()[0]);
			});
		}

		[Test]
		public void GridViewSelection_SelectAll_SelectsAllRows()
		{
			TestUtils.Invoke(() =>
			{
				grid.SelectAll();
				Assert.AreEqual(ItemCount, grid.SelectedRows.Count());
			});
		}

		[Test]
		public void GridViewSelection_InsertItem_SelectionUnchanged()
		{
			TestUtils.Invoke(() =>
			{
				grid.SelectRow(0);
				var selectedItem = grid.SelectedItem;
				model.Insert(0, new DataItem(model.Count));
				Assert.AreEqual(selectedItem, grid.SelectedItem);
			});
		}

		[Test]
		public void GridViewSelection_DeleteSelectedItems_SelectedItemsRemoved()
		{
			TestUtils.Invoke(() =>
			{
				grid.AllowMultipleSelection = true;

				for (var i = 0; i < model.Count / 2; ++i) // Select the first half of items
				grid.SelectRow(i);

				// Delete alternate items
				for (var i = ItemCount - 1; i >= 0; i -= 2)
					model.RemoveAt(i);

				// The selection should now be a quarter of the original items
				Assert.AreEqual(ItemCount / 4, grid.SelectedItems.Count());
				var expectedSelectedItemIds = new List<int>();
				for (var i = 0; i < GridViewUtils.ItemCount / 2; i += 2)
					expectedSelectedItemIds.Add(i);
				Assert.IsTrue(expectedSelectedItemIds.SequenceEqual(grid.SelectedItems.Select(x => ((DataItem)x).Id)));
			});
		}

		[Test]
		public void GridViewSelection_SortItems_SelectionUnchanged()
		{
			TestUtils.Invoke(() =>
			{
				grid.SortComparer = GridViewUtils.SortItemsAscending;
				grid.SelectRow(0);
				Assert.AreEqual(1, grid.SelectedRows.Count()); // model
				Assert.AreEqual(0, grid.SelectedRows.ToList()[0]); // model

				selectionChangedCount = 0; // reset the count
				grid.SortComparer = GridViewUtils.SortItemsDescending;
				// After sorting, the selected rows in the model should be unchanged
				// but the selected rows in the view should have changed.
				Assert.AreEqual(1, grid.SelectedRows.Count()); // model
				Assert.AreEqual(0, grid.SelectedRows.ToList()[0]); // model
				Assert.AreEqual(ItemCount - 1, handler.SelectedRows.ToList()[0]); // view

				Assert.AreEqual(0, selectionChangedCount, "SelectionChanged event should not fire when changing the SortComparer");
			});
		}


		[Test]
		public void GridViewSelection_FilterItems_SelectionUnchanged()
		{
			TestUtils.Invoke(() =>
			{
				grid.AllowMultipleSelection = true;
				for (var i = 0; i < ItemCount / 2; ++i) // Select the first half of items
				grid.SelectRow(i);

				selectionChangedCount = 0; // reset the count
				grid.Filter = GridViewUtils.KeepOddItemsFilter;
				// After filtering , the selected rows in the model should be unchanged
				// but the selected rows in the view should have changed.
				Assert.AreEqual(ItemCount / 2, grid.SelectedRows.Count()); // model
				for (var i = 0; i < ItemCount / 2; ++i)
					Assert.AreEqual(i, grid.SelectedRows.ToList()[i]);

				Assert.AreEqual(0, selectionChangedCount, "SelectionChanged event should not fire when changing the Filter");
			});
		}
	}
}