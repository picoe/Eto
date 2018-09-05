using System;
using NUnit.Framework;
using Eto.Forms;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Specialized;

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

			// ensure they are not in the filter collection after removing the sort
			filtered.Sort = null;
			Assert.IsFalse(filtered.Any(r => r.Id == 19), "Item #19 should not be in the list");
			Assert.IsFalse(filtered.Any(r => r.Id == 20), "Item #20 should not be in the list");
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

			// ensure they are not in the filter collection after removing the filter
			filtered.Filter = null;
			Assert.IsFalse(filtered.Any(r => r.Id == 20), "Item #20 should not be in the list");
			Assert.IsFalse(filtered.Any(r => r.Id == 30), "Item #30 should not be in the list");
			Assert.IsFalse(filtered.Any(r => r.Id == 60), "Item #60 should not be in the list");
		}

		[Test]
		public void AddItemShouldTriggerCorrectChange()
		{
			var model = GridViewUtils.CreateModel();
			var filtered = new FilterCollection<DataItem>(model);
			filtered.Sort = GridViewUtils.SortEvenItemsBeforeOdd;

			int newIndex = -1;
			filtered.CollectionChanged += (sender, e) =>
			{
				Assert.AreEqual(NotifyCollectionChangedAction.Add, e.Action, "Action should be add");
				newIndex = e.NewStartingIndex;
			};
			var item = new DataItem(1000);
			filtered.Add(item);

			var index = filtered.IndexOf(item);
			Assert.AreEqual(filtered[index].Id, item.Id, "Index reported does not have correct item");
			Assert.AreEqual(index, newIndex, "Triggered event should have correct index");
			Assert.AreEqual(model.Count - 1, model.IndexOf(item), "Item should be added to the end of the model");
		}

		[Test]
		public void InsertItemShouldTriggerCorrectChange()
		{
			var model = GridViewUtils.CreateModel();
			var filtered = new FilterCollection<DataItem>(model);
			filtered.Sort = GridViewUtils.SortEvenItemsBeforeOdd;

			int newIndex = -1;
			filtered.CollectionChanged += (sender, e) =>
			{
				Assert.AreEqual(NotifyCollectionChangedAction.Add, e.Action, "Action should be add");
				newIndex = e.NewStartingIndex;
			};
			var item = new DataItem(1000);
			const int insertIndex = 50;
			filtered.Insert(insertIndex, item);

			var index = filtered.IndexOf(item);
			Assert.AreEqual(filtered[index].Id, item.Id, "Index reported does not have correct item");
			Assert.AreEqual(index, newIndex, "Triggered event should have correct index");
			Assert.AreEqual(insertIndex, index, "Index of item should be the inserted index");
		}

		[Test]
		public void InsertItemShouldBeInSameOrderInModel()
		{
			var model = GridViewUtils.CreateModel();
			var filtered = new FilterCollection<DataItem>(model);
			filtered.Filter = GridViewUtils.KeepOddItemsFilter;

			const int filterInsertIndex = 10;

			var item = new DataItem(1000);
			filtered.Insert(filterInsertIndex, item);
			Assert.AreEqual(filtered[filterInsertIndex].Id, 21, "#1 Item should NOT be inserted at the specified index, since it is an even number");
			Assert.AreEqual(filtered.IndexOf(item), -1, "#2 Item should NOT be in the filtered list");
			Assert.AreEqual(model.IndexOf(filtered[filterInsertIndex]) - 1, model.IndexOf(item), "#3 Item should be inserted right before item at filter location");


			item = new DataItem(1001);
			filtered.Insert(filterInsertIndex, item);
			Assert.AreEqual(filtered[filterInsertIndex].Id, 1001, "#4 Item with odd number should be inserted at the specified index");
			Assert.AreEqual(filtered.IndexOf(item), filterInsertIndex, "#5 Item should be in the filtered list at the insert location");
			Assert.AreEqual(model.IndexOf(filtered[filterInsertIndex + 1]) - 1, model.IndexOf(item), "#6 Item should be inserted right before item at filter location");

			// refresh the list
			filtered.Refresh();

			// re-test inserted item, it should be at the same index.
			Assert.AreEqual(filtered[filterInsertIndex].Id, 1001, "#7 Item with odd number should be inserted at the specified index");
			Assert.AreEqual(filtered.IndexOf(item), filterInsertIndex, "#8 Item should be in the filtered list at the insert location");
			Assert.AreEqual(model.IndexOf(filtered[filterInsertIndex + 1]) - 1, model.IndexOf(item), "#9 Item should be inserted right before item at filter location");

			// ensure they are in the filter collection after the filter changes.
			filtered.Filter = null;
			Assert.IsTrue(filtered.Any(r => r.Id == 1000), "Item #1000 should be in the list");
			Assert.IsTrue(filtered.Any(r => r.Id == 1001), "Item #1001 should be in the list");
		}

		[Test]
		public void InsertIntoParentWhileFilteredShouldKeepSameIndecies()
		{
			var model = GridViewUtils.CreateModel();
			var filtered = new FilterCollection<DataItem>(model);
			filtered.Filter = GridViewUtils.KeepOddItemsFilter;
			filtered.Sort = GridViewUtils.SortItemsDescending;

			NotifyCollectionChangedEventArgs changeArgs = null;
			filtered.CollectionChanged += (sender, e) => changeArgs = e;

			var item = new DataItem(1000);
			changeArgs = null;
			model.Insert(10, item);
			// should not exist in filtered
			Assert.AreEqual(-1, filtered.IndexOf(item), "#1-1 Item should NOT be in the filtered collection");
			Assert.IsNull(changeArgs, "#1-2 Inserting an item that doesn't match the filter shouldn't raise a change notification");

			item = new DataItem(1001);
			changeArgs = null;
			model.Insert(11, item);
			Assert.AreEqual(0, filtered.IndexOf(item), "#2-1 Item should be in the filtered collection");

			Assert.IsNotNull(changeArgs, "#3-1 Change should have been triggered");
			Assert.AreEqual(NotifyCollectionChangedAction.Add, changeArgs.Action, "#3-2 Item should have triggered an add notification");
			Assert.AreEqual(0, changeArgs.NewStartingIndex, "#3-3 Index of add notification is incorrect");
			Assert.IsNotEmpty(changeArgs.NewItems, "#3-4 New items of change event should not be empty");
			Assert.AreEqual(1001, ((DataItem)changeArgs.NewItems[0]).Id, "#3-5 New item of notification is not correct");

			filtered.Filter = null;
			filtered.Sort = null;

			// should be in the same inserted position in the source model
			Assert.AreEqual(1000, filtered[10].Id, "#4-1 Item 1000 was not inserted in the correct location");
			Assert.AreEqual(1001, filtered[11].Id, "#4-2 Item 1001 was not inserted in the correct location");
			Assert.AreEqual(model.Count, filtered.Count, "#4-3 Count in filtered does not match model");
		}

		[Test]
		public void RemoveFromParentWhileFilteredShouldBeRemoved()
		{
			var model = GridViewUtils.CreateModel();

			var filtered = new FilterCollection<DataItem>(model);
			filtered.Filter = GridViewUtils.KeepOddItemsFilter;
			filtered.Sort = GridViewUtils.SortItemsDescending;

			NotifyCollectionChangedEventArgs changeArgs = null;
			filtered.CollectionChanged += (sender, e) => changeArgs = e;

			var itemToRemove1 = model[10];
			Assert.AreEqual(-1, filtered.IndexOf(itemToRemove1), "#1-1 Item should NOT be in the filtered collection");
			changeArgs = null;
			model.RemoveAt(10);
			Assert.IsNull(changeArgs, "#1-2 Change should not have been triggered");
			Assert.AreEqual(-1, filtered.IndexOf(itemToRemove1), "#1-3 Item should NOT be in the filtered collection");

			var itemToRemove2 = model[10];
			Assert.AreEqual(44, filtered.IndexOf(itemToRemove2), "#2-1 Item should be in the filtered collection");
			changeArgs = null;
			model.Remove(itemToRemove2);
			Assert.AreEqual(-1, filtered.IndexOf(itemToRemove2), "#2-2 Item should NOT be in the filtered collection");

			// verify change notification
			Assert.IsNotNull(changeArgs, "#3-1 Change should have been triggered");
			Assert.AreEqual(NotifyCollectionChangedAction.Remove, changeArgs.Action, "#3-2 Item should have triggered a remove notification");
			Assert.AreEqual(44, changeArgs.OldStartingIndex, "#3-3 Index of remove notification is incorrect");
			Assert.IsNotEmpty(changeArgs.OldItems, "#3-4 Old items of change event should not be empty");
			Assert.AreEqual(11, ((DataItem)changeArgs.OldItems[0]).Id, "#3-5 Old item of notification is not correct");


			filtered.Filter = null;
			Assert.AreEqual(NotifyCollectionChangedAction.Reset, changeArgs.Action, "#4 Changing filter should send a reset notification");
			filtered.Sort = null;

			// should be in the same inserted position in the source model
			Assert.AreEqual(-1, filtered.IndexOf(itemToRemove1), "#5-1 Item should NOT be in the filtered collection");
			Assert.AreEqual(-1, filtered.IndexOf(itemToRemove2), "#5-2 Item should NOT be in the filtered collection");
			Assert.AreEqual(model.Count, filtered.Count, "#5-3 Count in filtered does not match model");
		}

		[Test]
		public void AddFromSourceWhileFilteredShouldAddCorrectly()
		{
			var model = GridViewUtils.CreateModel();
			var filtered = new FilterCollection<DataItem>(model);
			filtered.Filter = GridViewUtils.KeepOddItemsFilter;
			filtered.Sort = GridViewUtils.SortItemsDescending;

			NotifyCollectionChangedEventArgs changeArgs = null;
			filtered.CollectionChanged += (sender, e) => changeArgs = e;

			var item = new DataItem(1000);
			changeArgs = null;
			model.Add(item);
			// should not exist in filtered
			Assert.AreEqual(-1, filtered.IndexOf(item), "#1-1 Item should NOT be in the filtered collection");
			Assert.IsNull(changeArgs, "#1-2 Inserting an item that doesn't match the filter shouldn't raise a change notification");

			item = new DataItem(1001);
			changeArgs = null;
			model.Add(item);
			Assert.AreEqual(0, filtered.IndexOf(item), "#2-1 Item should be in the filtered collection");

			Assert.IsNotNull(changeArgs, "#3-1 Change should have been triggered");
			Assert.AreEqual(NotifyCollectionChangedAction.Add, changeArgs.Action, "#3-2 Item should have triggered an add notification");
			Assert.AreEqual(0, changeArgs.NewStartingIndex, "#3-3 Index of add notification is incorrect");
			Assert.IsNotEmpty(changeArgs.NewItems, "#3-4 New items of change event should not be empty");
			Assert.AreEqual(1001, ((DataItem)changeArgs.NewItems[0]).Id, "#3-5 New item of notification is not correct");

			filtered.Filter = null;
			filtered.Sort = null;

			// should be in the same inserted position in the source model
			Assert.AreEqual(model.Count, filtered.Count, "#4-3 Count in filtered does not match model");
			Assert.AreEqual(1000, filtered[filtered.Count - 2].Id, "#4-1 Item 1000 was not added in the correct location");
			Assert.AreEqual(1001, filtered[filtered.Count - 1].Id, "#4-2 Item 1001 was not added in the correct location");
		}

		class MyCollection : ObservableCollection<DataItem>
		{
			public void AddRange(IEnumerable<DataItem> items)
			{
				foreach (var item in items)
					Items.Add(item);
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}
		}


		[Test]
		public void ResetShouldAddCurrentItemsToFilteredList()
		{
			var collection = new MyCollection();
			var filterCollection = new FilterCollection<DataItem>(collection);

			collection.AddRange(Enumerable.Range(1, 10).Select(r => new DataItem(r)));

			Assert.AreEqual(10, filterCollection.Count, "FilterCollection.Count should be equal to 10 after adding items in bulk");

			collection.AddRange(Enumerable.Range(1, 10).Select(r => new DataItem(r)));

			Assert.AreEqual(20, filterCollection.Count, "FilterCollection.Count should be equal to 20 after adding more items in bulk");
		}
	}
}

