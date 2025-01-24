using NUnit.Framework;
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
			Assert.That(filtered.Count, Is.EqualTo(model.Count));
			for (var i = 0; i < model.Count; ++i)
				Assert.That(model[i], Is.SameAs(filtered[i]));
		}

		[Test]
		public void WithOddItemFilterShouldContainOddModelItems()
		{
			var model = GridViewUtils.CreateModel();
			var filtered = new FilterCollection<DataItem>(model);
			filtered.Filter = GridViewUtils.KeepOddItemsFilter;
			Assert.That(filtered.Count, Is.EqualTo(model.Count / 2));
			for (var i = 0; i < model.Count / 2; ++i)
				Assert.That(model[i * 2 + 1], Is.SameAs(filtered[i]));
		}

		[Test]
		public void SortWithEvenItemsBeforeOddItemsShouldSortCorrectly()
		{
			var model = GridViewUtils.CreateModel();
			var filtered = new FilterCollection<DataItem>(model);
			filtered.Sort = GridViewUtils.SortEvenItemsBeforeOdd;
			Assert.That(filtered.Count, Is.EqualTo(model.Count));
			for (var i = 0; i < model.Count / 2; ++i)
				Assert.That(model[i * 2], Is.SameAs(filtered[i]));
			for (var i = 0; i < model.Count / 2; ++i)
				Assert.That(model[i * 2 + 1], Is.SameAs(filtered[model.Count / 2 + i]));
		}

		[Test]
		public void SortAscendingShouldSortCorrectly()
		{
			var model = GridViewUtils.CreateModel();
			var filtered = new FilterCollection<DataItem>(model);
			filtered.Sort = GridViewUtils.SortItemsAscending;
			Assert.That(filtered.Count, Is.EqualTo(model.Count));
			for (var i = 0; i < model.Count; ++i)
				Assert.That(model[i], Is.SameAs(filtered[i]));
		}

		[Test]
		public void SortDescendingShouldSortCorrectly()
		{
			var model = GridViewUtils.CreateModel();
			var filtered = new FilterCollection<DataItem>(model);
			filtered.Sort = GridViewUtils.SortItemsDescending;
			Assert.That(filtered.Count, Is.EqualTo(model.Count));
			for (var i = 0; i < model.Count; ++i)
				Assert.That(model[model.Count - 1 - i], Is.SameAs(filtered[i]));
		}

		[Test]
		public void SortWithEvenItemsBeforeOddItemsAndWithFilterShouldSortAndFilterCorrectly()
		{
			var model = GridViewUtils.CreateModel();
			var filtered = new FilterCollection<DataItem>(model);
			filtered.Sort = GridViewUtils.SortEvenItemsBeforeOdd;
			filtered.Filter = GridViewUtils.KeepFirstHalfOfItemsFilter;
			Assert.That(filtered.Count, Is.EqualTo(model.Count / 2));
			for (var i = 0; i < model.Count / 4; ++i)
				Assert.That(model[i * 2], Is.SameAs(filtered[i]));
			for (var i = 0; i < model.Count / 4; ++i)
				Assert.That(model[i * 2 + 1], Is.SameAs(filtered[model.Count / 4 + i]));
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
			Assert.That(filtered.Any(r => r.Id == 19), Is.False, "Removing the 80th filtered row should remove item #19");
			Assert.That(model.Any(r => r.Id == 19), Is.False, "Removing the 80th filtered row should remove item #19 from the model");

			// test removing from the model
			model.Remove(model.First(r => r.Id == 20));
			Assert.That(filtered.Any(r => r.Id == 20), Is.False, "Removing Item #20 should no longer show up in the filtered collection");
			Assert.That(model.Any(r => r.Id == 20), Is.False, "Removing Item #20 should no longer be in the model");

			// ensure they are not in the filter collection after removing the sort
			filtered.Sort = null;
			Assert.That(filtered.Any(r => r.Id == 19), Is.False, "Item #19 should not be in the list");
			Assert.That(filtered.Any(r => r.Id == 20), Is.False, "Item #20 should not be in the list");
		}

		[Test]
		public void RemoveItemWhenFilteredRemoveCorrectItems()
		{
			var model = GridViewUtils.CreateModel();
			var filtered = new FilterCollection<DataItem>(model);
			// filter out 2nd half
			filtered.Filter = item => item.Id <= 50;

			// test removing from the filtered collection
			Assert.That(filtered.Any(r => r.Id == 20), Is.True, "Item #20 should appear in the filtered list");
			Assert.That(model.Any(r => r.Id == 20), Is.True, "Item #20 should be in the model");
			filtered.Remove(filtered.First(r => r.Id == 20));
			Assert.That(filtered.Any(r => r.Id == 20), Is.False, "Removing Item #20 should no longer show up in the filtered collection");
			Assert.That(model.Any(r => r.Id == 20), Is.False, "Removing Item #20 should no longer be in the model");

			// test removing an item from the model that also shows in the filter
			Assert.That(filtered.Any(r => r.Id == 30), Is.True, "Item #30 should appear in the filtered list");
			Assert.That(model.Any(r => r.Id == 30), Is.True, "Item #30 should be in the model");
			model.Remove(model.First(r => r.Id == 30));
			Assert.That(filtered.Any(r => r.Id == 30), Is.False, "Removing Item #30 should no longer show up in the filtered collection");
			Assert.That(model.Any(r => r.Id == 30), Is.False, "Removing Item #30 should no longer be in the model");

			// test removing an item from the model that isn't in the filtered collection
			Assert.That(filtered.Any(r => r.Id == 60), Is.False, "Item #60 should NOT appear in the filtered list");
			Assert.That(model.Any(r => r.Id == 60), Is.True, "Item #60 should be in the model");
			model.Remove(model.First(r => r.Id == 60));
			Assert.That(filtered.Any(r => r.Id == 60), Is.False, "Removing Item #60 should no longer show up in the filtered collection");
			Assert.That(model.Any(r => r.Id == 60), Is.False, "Removing Item #60 should no longer be in the model");

			// ensure they are not in the filter collection after removing the filter
			filtered.Filter = null;
			Assert.That(filtered.Any(r => r.Id == 20), Is.False, "Item #20 should not be in the list");
			Assert.That(filtered.Any(r => r.Id == 30), Is.False, "Item #30 should not be in the list");
			Assert.That(filtered.Any(r => r.Id == 60), Is.False, "Item #60 should not be in the list");
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
				Assert.That(e.Action, Is.EqualTo(NotifyCollectionChangedAction.Add), "Action should be add");
				newIndex = e.NewStartingIndex;
			};
			var item = new DataItem(1000);
			filtered.Add(item);

			var index = filtered.IndexOf(item);
			Assert.That(item.Id, Is.EqualTo(filtered[index].Id), "Index reported does not have correct item");
			Assert.That(newIndex, Is.EqualTo(index), "Triggered event should have correct index");
			Assert.That(model.IndexOf(item), Is.EqualTo(model.Count - 1), "Item should be added to the end of the model");
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
				Assert.That(e.Action, Is.EqualTo(NotifyCollectionChangedAction.Add), "Action should be add");
				newIndex = e.NewStartingIndex;
			};
			var item = new DataItem(1000);
			const int insertIndex = 50;
			filtered.Insert(insertIndex, item);

			var index = filtered.IndexOf(item);
			Assert.That(item.Id, Is.EqualTo(filtered[index].Id), "Index reported does not have correct item");
			Assert.That(newIndex, Is.EqualTo(index), "Triggered event should have correct index");
			Assert.That(index, Is.EqualTo(insertIndex), "Index of item should be the inserted index");
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
			Assert.That(21, Is.EqualTo(filtered[filterInsertIndex].Id), "#1 Item should NOT be inserted at the specified index, since it is an even number");
			Assert.That(-1, Is.EqualTo(filtered.IndexOf(item)), "#2 Item should NOT be in the filtered list");
			Assert.That(model.IndexOf(item), Is.EqualTo(model.IndexOf(filtered[filterInsertIndex]) - 1), "#3 Item should be inserted right before item at filter location");


			item = new DataItem(1001);
			filtered.Insert(filterInsertIndex, item);
			Assert.That(1001, Is.EqualTo(filtered[filterInsertIndex].Id), "#4 Item with odd number should be inserted at the specified index");
			Assert.That(filterInsertIndex, Is.EqualTo(filtered.IndexOf(item)), "#5 Item should be in the filtered list at the insert location");
			Assert.That(model.IndexOf(item), Is.EqualTo(model.IndexOf(filtered[filterInsertIndex + 1]) - 1), "#6 Item should be inserted right before item at filter location");

			// refresh the list
			filtered.Refresh();

			// re-test inserted item, it should be at the same index.
			Assert.That(1001, Is.EqualTo(filtered[filterInsertIndex].Id), "#7 Item with odd number should be inserted at the specified index");
			Assert.That(filterInsertIndex, Is.EqualTo(filtered.IndexOf(item)), "#8 Item should be in the filtered list at the insert location");
			Assert.That(model.IndexOf(item), Is.EqualTo(model.IndexOf(filtered[filterInsertIndex + 1]) - 1), "#9 Item should be inserted right before item at filter location");

			// ensure they are in the filter collection after the filter changes.
			filtered.Filter = null;
			Assert.That(filtered.Any(r => r.Id == 1000), Is.True, "Item #1000 should be in the list");
			Assert.That(filtered.Any(r => r.Id == 1001), Is.True, "Item #1001 should be in the list");
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
			Assert.That(filtered.IndexOf(item), Is.EqualTo(-1), "#1-1 Item should NOT be in the filtered collection");
			Assert.That(changeArgs, Is.Null, "#1-2 Inserting an item that doesn't match the filter shouldn't raise a change notification");

			item = new DataItem(1001);
			changeArgs = null;
			model.Insert(11, item);
			Assert.That(filtered.IndexOf(item), Is.EqualTo(0), "#2-1 Item should be in the filtered collection");

			Assert.That(changeArgs, Is.Not.Null, "#3-1 Change should have been triggered");
			Assert.That(changeArgs.Action, Is.EqualTo(NotifyCollectionChangedAction.Add), "#3-2 Item should have triggered an add notification");
			Assert.That(changeArgs.NewStartingIndex, Is.EqualTo(0), "#3-3 Index of add notification is incorrect");
			Assert.That(changeArgs.NewItems, Is.Not.Empty, "#3-4 New items of change event should not be empty");
			Assert.That(((DataItem)changeArgs.NewItems[0]).Id, Is.EqualTo(1001), "#3-5 New item of notification is not correct");

			filtered.Filter = null;
			filtered.Sort = null;

			// should be in the same inserted position in the source model
			Assert.That(filtered[10].Id, Is.EqualTo(1000), "#4-1 Item 1000 was not inserted in the correct location");
			Assert.That(filtered[11].Id, Is.EqualTo(1001), "#4-2 Item 1001 was not inserted in the correct location");
			Assert.That(filtered.Count, Is.EqualTo(model.Count), "#4-3 Count in filtered does not match model");
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
			Assert.That(filtered.IndexOf(itemToRemove1), Is.EqualTo(-1), "#1-1 Item should NOT be in the filtered collection");
			changeArgs = null;
			model.RemoveAt(10);
			Assert.That(changeArgs, Is.Null, "#1-2 Change should not have been triggered");
			Assert.That(filtered.IndexOf(itemToRemove1), Is.EqualTo(-1), "#1-3 Item should NOT be in the filtered collection");

			var itemToRemove2 = model[10];
			Assert.That(filtered.IndexOf(itemToRemove2), Is.EqualTo(44), "#2-1 Item should be in the filtered collection");
			changeArgs = null;
			model.Remove(itemToRemove2);
			Assert.That(filtered.IndexOf(itemToRemove2), Is.EqualTo(-1), "#2-2 Item should NOT be in the filtered collection");

			// verify change notification
			Assert.That(changeArgs, Is.Not.Null, "#3-1 Change should have been triggered");
			Assert.That(changeArgs.Action, Is.EqualTo(NotifyCollectionChangedAction.Remove), "#3-2 Item should have triggered a remove notification");
			Assert.That(changeArgs.OldStartingIndex, Is.EqualTo(44), "#3-3 Index of remove notification is incorrect");
			Assert.That(changeArgs.OldItems, Is.Not.Empty, "#3-4 Old items of change event should not be empty");
			Assert.That(((DataItem)changeArgs.OldItems[0]).Id, Is.EqualTo(11), "#3-5 Old item of notification is not correct");


			filtered.Filter = null;
			Assert.That(changeArgs.Action, Is.EqualTo(NotifyCollectionChangedAction.Reset), "#4 Changing filter should send a reset notification");
			filtered.Sort = null;

			// should be in the same inserted position in the source model
			Assert.That(filtered.IndexOf(itemToRemove1), Is.EqualTo(-1), "#5-1 Item should NOT be in the filtered collection");
			Assert.That(filtered.IndexOf(itemToRemove2), Is.EqualTo(-1), "#5-2 Item should NOT be in the filtered collection");
			Assert.That(filtered.Count, Is.EqualTo(model.Count), "#5-3 Count in filtered does not match model");
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
			Assert.That(filtered.IndexOf(item), Is.EqualTo(-1), "#1-1 Item should NOT be in the filtered collection");
			Assert.That(changeArgs, Is.Null, "#1-2 Inserting an item that doesn't match the filter shouldn't raise a change notification");

			item = new DataItem(1001);
			changeArgs = null;
			model.Add(item);
			Assert.That(filtered.IndexOf(item), Is.EqualTo(0), "#2-1 Item should be in the filtered collection");

			Assert.That(changeArgs, Is.Not.Null, "#3-1 Change should have been triggered");
			Assert.That(changeArgs.Action, Is.EqualTo(NotifyCollectionChangedAction.Add), "#3-2 Item should have triggered an add notification");
			Assert.That(changeArgs.NewStartingIndex, Is.EqualTo(0), "#3-3 Index of add notification is incorrect");
			Assert.That(changeArgs.NewItems, Is.Not.Empty, "#3-4 New items of change event should not be empty");
			Assert.That(((DataItem)changeArgs.NewItems[0]).Id, Is.EqualTo(1001), "#3-5 New item of notification is not correct");

			filtered.Filter = null;
			filtered.Sort = null;

			// should be in the same inserted position in the source model
			Assert.That(filtered.Count, Is.EqualTo(model.Count), "#4-3 Count in filtered does not match model");
			Assert.That(filtered[filtered.Count - 2].Id, Is.EqualTo(1000), "#4-1 Item 1000 was not added in the correct location");
			Assert.That(filtered[filtered.Count - 1].Id, Is.EqualTo(1001), "#4-2 Item 1001 was not added in the correct location");
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

			Assert.That(filterCollection.Count, Is.EqualTo(10), "FilterCollection.Count should be equal to 10 after adding items in bulk");

			collection.AddRange(Enumerable.Range(1, 10).Select(r => new DataItem(r)));

			Assert.That(filterCollection.Count, Is.EqualTo(20), "FilterCollection.Count should be equal to 20 after adding more items in bulk");
		}
	}
}

