#if !NO_UNITTESTS
using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCase = NUnit.Framework.TestCaseAttribute;
using ClassCleanup = NUnit.Framework.TestFixtureTearDownAttribute;
using ClassInitialize = NUnit.Framework.TestFixtureSetUpAttribute;
using Assert = NUnit.Framework.Assert;
using Eto.Forms;
using Eto;
using Eto.Test.UnitTests.Handlers;

namespace Eto.Test.UnitTests
{
	/// <summary>
	/// Unit tests for GridViewSelection
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	/// </summary>
	[TestClass]
	public class GridViewSelectionTests
	{
		static int ItemCount { get { return GridViewUtils.ItemCount; } }
		GridView g;
		DataStoreCollection model;
		TestGridViewHandler h;
		int selectionChangedCount; // incremented when g.SelectionChanged fires

		[TestInitialize]
		public void Setup()
		{
			g = new GridView(null, h = new TestGridViewHandler());
			model = GridViewUtils.CreateModel();
			g.DataStore = model;
			g.SelectionChanged += (s, e) => selectionChangedCount++;
		}

		[TestMethod]
		public void GridViewSelection_SelectFirstRow_SelectsFirstRow()
		{
			g.SelectRow(0);
			Assert.AreEqual(model[0], g.SelectedItem);
			Assert.AreEqual(0, h.SelectedRows.ToList()[0]);
		}

		[TestMethod]
		public void GridViewSelection_SelectAll_SelectsAllRows()
		{
			g.SelectAll();
			Assert.AreEqual(ItemCount, g.SelectedRows.Count());
			Assert.AreEqual(ItemCount, h.SelectedRows.Count());
		}

		[TestMethod]
		public void GridViewSelection_InsertItem_SelectionUnchanged()
		{
			g.SelectRow(0);
			var selectedItem = g.SelectedItem;
			model.Insert(0, new DataItem(model.Count));
			Assert.AreEqual(selectedItem, g.SelectedItem);
		}

		[TestMethod]
		public void GridViewSelection_DeleteSelectedItems_SelectedItemsRemoved()
		{
			g.AllowMultipleSelection = true;

			for (var i = 0; i < model.Count / 2; ++i) // Select the first half of items
				g.SelectRow(i);

			// Delete alternate items
			for (var i = ItemCount - 1; i >= 0; i -= 2)
				model.RemoveAt(i);

			// The selection should now be a quarter of the original items
			Assert.AreEqual(ItemCount / 4, g.SelectedItems.Count());
			var expectedSelectedItemIds = new List<int>();
			for (var i = 0; i < GridViewUtils.ItemCount / 2; i += 2)
				expectedSelectedItemIds.Add(i);
			Assert.IsTrue(expectedSelectedItemIds.SequenceEqual(g.SelectedItems.Select(x => ((DataItem)x).Id)));
		}

		[TestMethod]
		public void GridViewSelection_SortItems_SelectionUnchanged()
		{
			g.SortComparer = GridViewUtils.SortItemsAscending;
			g.SelectRow(0);
			Assert.AreEqual(1, g.SelectedRows.Count()); // model
			Assert.AreEqual(0, g.SelectedRows.ToList()[0]); // model
			Assert.AreEqual(1, h.SelectedRows.Count()); // view
			Assert.AreEqual(0, h.SelectedRows.ToList()[0]); // view

			selectionChangedCount = 0; // reset the count
			g.SortComparer = GridViewUtils.SortItemsDescending;
			// After sorting, the selected rows in the model should be unchanged
			// but the selected rows in the view should have changed.
			Assert.AreEqual(1, g.SelectedRows.Count()); // model
			Assert.AreEqual(0, g.SelectedRows.ToList()[0]); // model
			Assert.AreEqual(1, h.SelectedRows.Count()); //  view
			Assert.AreEqual(ItemCount -1, h.SelectedRows.ToList()[0]); // view

			Assert.AreEqual(0, selectionChangedCount); // verify that no selection changed events are fired.
		}

		[TestMethod]
		public void GridViewSelection_FilterItems_SelectionUnchanged()
		{
			g.AllowMultipleSelection = true;
			for (var i = 0; i < ItemCount / 2; ++i) // Select the first half of items
				g.SelectRow(i);

			selectionChangedCount = 0; // reset the count
			g.Filter = GridViewUtils.KeepOddItemsFilter;
			// After filtering , the selected rows in the model should be unchanged
			// but the selected rows in the view should have changed.
			Assert.AreEqual(ItemCount / 2, g.SelectedRows.Count()); // model
			for (var i = 0; i < ItemCount / 2; ++i)
				Assert.AreEqual(i, g.SelectedRows.ToList()[i]);
			Assert.AreEqual(ItemCount /4, h.SelectedRows.Count()); //  view
			for (var i = 0; i < ItemCount / 4; ++i)
				Assert.AreEqual(i, h.SelectedRows.ToList()[i]);

			Assert.AreEqual(0, selectionChangedCount); // verify that no selection changed events are fired.
		}
	}
}
#endif