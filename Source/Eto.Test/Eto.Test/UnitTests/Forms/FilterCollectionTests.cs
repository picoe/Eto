using System;
using NUnit.Framework;
using Eto.Forms;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Eto.Test.UnitTests.Forms
{
	/// <summary>
	/// <see cref="FilterCollection{T}"/> tests
	/// </summary>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[TestFixture]
	public class FilterCollectionTests
	{
		readonly ObservableCollection<DataItem> model = GridViewUtils.CreateModel();

		[Test]
		public void WithNoFilterOrSortShouldContainAllModelItems()
		{
			var filtered = new FilterCollection<DataItem>(model);
			Assert.AreEqual(model.Count, filtered.Count);
			for (var i = 0; i < model.Count; ++i)
				Assert.AreSame(model[i], filtered[i]);
		}

		[Test]
		public void WithOddItemFilterShouldContainOddModelItems()
		{
			var filtered = new FilterCollection<DataItem>(model);
			filtered.Filter = GridViewUtils.KeepOddItemsFilter;
			Assert.AreEqual(model.Count / 2, filtered.Count);
			for (var i = 0; i < model.Count / 2; ++i)
				Assert.AreSame(model[i * 2 + 1], filtered[i]);
		}

		[Test]
		public void SortWithEvenItemsBeforeOddItemsShouldSortCorrectly()
		{
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
			var filtered = new FilterCollection<DataItem>(model);
			filtered.Sort = GridViewUtils.SortItemsAscending;
			Assert.AreEqual(model.Count, filtered.Count);
			for (var i = 0; i < model.Count; ++i)
				Assert.AreSame(model[i], filtered[i]);
		}

		[Test]
		public void SortDescendingShouldSortCorrectly()
		{
			var filtered = new FilterCollection<DataItem>(model);
			filtered.Sort = GridViewUtils.SortItemsDescending;
			Assert.AreEqual(model.Count, filtered.Count);
			for (var i = 0; i < model.Count; ++i)
				Assert.AreSame(model[model.Count - 1 - i], filtered[i]);
		}

		[Test]
		public void SortWithEvenItemsBeforeOddItemsAndWithFilterShouldSortAndFilterCorrectly()
		{
			var filtered = new FilterCollection<DataItem>(model);
			filtered.Sort = GridViewUtils.SortEvenItemsBeforeOdd;
			filtered.Filter = GridViewUtils.KeepFirstHalfOfItemsFilter;
			Assert.AreEqual(model.Count / 2, filtered.Count);
			for (var i = 0; i < model.Count / 4; ++i)
				Assert.AreSame(model[i * 2], filtered[i]);
			for (var i = 0; i < model.Count / 4; ++i)
				Assert.AreSame(model[i * 2 + 1], filtered[model.Count / 4 + i]);
		}
	}
}

