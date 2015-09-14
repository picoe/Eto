using System;
using NUnit.Framework;
using Eto.Forms;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Eto.Test.UnitTests.Forms
{
	/// <summary>
	/// <see cref="FilterCollection{T}"/> tests
	/// </summary>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[TestFixture, Category(TestUtils.TestPlatformCategory)]
	public class FilterCollectionTests
	{
		[Test]
		public void WithNoFilterOrSortShouldContainAllModelItems()
		{
			var model = GridViewUtils.CreateModel();
			var filtered = new FilterCollection<DataItem>(model);
			Assert.AreEqual(model.Count, filtered.Count);
			for (var i = 0; i < model.Count; ++i)
				Assert.AreSame(model[i], filtered[i]);
		}

		[Test]
		public void WithOddItemFilterShouldContainOddModelItems()
		{
			var model = GridViewUtils.CreateModel();
			var filtered = new FilterCollection<DataItem>(model);
			filtered.Filter = GridViewUtils.KeepOddItemsFilter;
			Assert.AreEqual(model.Count / 2, filtered.Count);
			for (var i = 0; i < model.Count / 2; ++i)
				Assert.AreSame(model[i * 2 + 1], filtered[i]);
		}

		[Test]
		public void SortWithEvenItemsBeforeOddItemsShouldSortCorrectly()
		{
			var model = GridViewUtils.CreateModel();
			var filtered = new FilterCollection<DataItem>(model);
			filtered.Sort = GridViewUtils.SortEvenItemsBeforeOdd;
			Assert.AreEqual(model.Count, filtered.Count);
			for (var i = 0; i < model.Count / 2; ++i)
				Assert.AreSame(model[i * 2], filtered[i]);
			for (var i = 0; i < model.Count / 2; ++i)
				Assert.AreSame(model[i * 2 + 1], filtered[model.Count / 2 + i]);
		}

		[Test]
		public void SortAscendingShouldSortCorrectly()
		{
			var model = GridViewUtils.CreateModel();
			var filtered = new FilterCollection<DataItem>(model);
			filtered.Sort = GridViewUtils.SortItemsAscending;
			Assert.AreEqual(model.Count, filtered.Count);
			for (var i = 0; i < model.Count; ++i)
				Assert.AreSame(model[i], filtered[i]);
		}

		[Test]
		public void SortDescendingShouldSortCorrectly()
		{
			var model = GridViewUtils.CreateModel();
			var filtered = new FilterCollection<DataItem>(model);
			filtered.Sort = GridViewUtils.SortItemsDescending;
			Assert.AreEqual(model.Count, filtered.Count);
			for (var i = 0; i < model.Count; ++i)
				Assert.AreSame(model[model.Count - 1 - i], filtered[i]);
		}

		[Test]
		public void SortWithEvenItemsBeforeOddItemsAndWithFilterShouldSortAndFilterCorrectly()
		{
			var model = GridViewUtils.CreateModel();
			var filtered = new FilterCollection<DataItem>(model);
			filtered.Sort = GridViewUtils.SortEvenItemsBeforeOdd;
			filtered.Filter = GridViewUtils.KeepFirstHalfOfItemsFilter;
			Assert.AreEqual(model.Count / 2, filtered.Count);
			for (var i = 0; i < model.Count / 4; ++i)
				Assert.AreSame(model[i * 2], filtered[i]);
			for (var i = 0; i < model.Count / 4; ++i)
				Assert.AreSame(model[i * 2 + 1], filtered[model.Count / 4 + i]);
		}

		[Test]
		public void RemoveItemWhenSortedShouldRemoveCorrectItems()
		{
			var model = GridViewUtils.CreateModel();
			var filtered = new FilterCollection<DataItem>(model);
			// sort in reverse
			filtered.Sort = (x, y) => y.Id.CompareTo(x.Id);

			// test removing from the filtered collection
			filtered.RemoveAt(80);
			Assert.IsFalse(filtered.Any(r => r.Id == 19), "Removing the 80th filtered row should remove item #19");
			Assert.IsFalse(model.Any(r => r.Id == 19), "Removing the 80th filtered row should remove item #19 from the model");

			// test removing from the model
			model.Remove(model.First(r => r.Id == 20));
			Assert.IsFalse(filtered.Any(r => r.Id == 20), "Removing Item #20 should no longer show up in the filtered collection");
			Assert.IsFalse(model.Any(r => r.Id == 20), "Removing Item #20 should no longer be in the model");
		}

		[Test]
		public void RemoveItemWhenFilteredRemoveCorrectItems()
		{
			var model = GridViewUtils.CreateModel();
			var filtered = new FilterCollection<DataItem>(model);
			// filter out 2nd half
			filtered.Filter = item => item.Id <= 50;

			// test removing from the filtered collection
			Assert.IsTrue(filtered.Any(r => r.Id == 20), "Item #20 should appear in the filtered list");
			Assert.IsTrue(model.Any(r => r.Id == 20), "Item #20 should be in the model");
			filtered.Remove(filtered.First(r => r.Id == 20));
			Assert.IsFalse(filtered.Any(r => r.Id == 20), "Removing Item #20 should no longer show up in the filtered collection");
			Assert.IsFalse(model.Any(r => r.Id == 20), "Removing Item #20 should no longer be in the model");

			// test removing an item from the model that also shows in the filter
			Assert.IsTrue(filtered.Any(r => r.Id == 30), "Item #30 should appear in the filtered list");
			Assert.IsTrue(model.Any(r => r.Id == 30), "Item #30 should be in the model");
			model.Remove(model.First(r => r.Id == 30));
			Assert.IsFalse(filtered.Any(r => r.Id == 30), "Removing Item #30 should no longer show up in the filtered collection");
			Assert.IsFalse(model.Any(r => r.Id == 30), "Removing Item #30 should no longer be in the model");

			// test removing an item from the model that isn't in the filtered collection
			Assert.IsFalse(filtered.Any(r => r.Id == 60), "Item #60 should NOT appear in the filtered list");
			Assert.IsTrue(model.Any(r => r.Id == 60), "Item #60 should be in the model");
			model.Remove(model.First(r => r.Id == 60));
			Assert.IsFalse(filtered.Any(r => r.Id == 60), "Removing Item #60 should no longer show up in the filtered collection");
			Assert.IsFalse(model.Any(r => r.Id == 60), "Removing Item #60 should no longer be in the model");
		}
	}
}

