using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Eto.Test.WinRT.Data
{
	/// <summary>
	/// Visual representation of a test group section.
	/// </summary>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class TestGroup : TestItemBase
	{
		public TestGroup(String uniqueId, String title, String subtitle, String imagePath, String description)
			: base(uniqueId, title, subtitle, imagePath, description)
		{
			Items.CollectionChanged += ItemsCollectionChanged;
		}

		private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			// Provides a subset of the full items collection to bind to from a GroupedItemsPage
			// for two reasons: GridView will not virtualize large items collections, and it
			// improves the user experience when browsing through groups with large numbers of
			// items.
			//
			// A maximum of 12 items are displayed because it results in filled grid columns
			// whether there are 1, 2, 3, 4, or 6 rows displayed

			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					if (e.NewStartingIndex < 12)
					{
						TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
						if (TopItems.Count > 12)
						{
							TopItems.RemoveAt(12);
						}
					}
					break;
				case NotifyCollectionChangedAction.Move:
					if (e.OldStartingIndex < 12 && e.NewStartingIndex < 12)
					{
						TopItems.Move(e.OldStartingIndex, e.NewStartingIndex);
					}
					else if (e.OldStartingIndex < 12)
					{
						TopItems.RemoveAt(e.OldStartingIndex);
						TopItems.Add(Items[11]);
					}
					else if (e.NewStartingIndex < 12)
					{
						TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
						TopItems.RemoveAt(12);
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					if (e.OldStartingIndex < 12)
					{
						TopItems.RemoveAt(e.OldStartingIndex);
						if (Items.Count >= 12)
						{
							TopItems.Add(Items[11]);
						}
					}
					break;
				case NotifyCollectionChangedAction.Replace:
					if (e.OldStartingIndex < 12)
					{
						TopItems[e.OldStartingIndex] = Items[e.OldStartingIndex];
					}
					break;
				case NotifyCollectionChangedAction.Reset:
					TopItems.Clear();
					while (TopItems.Count < Items.Count && TopItems.Count < 12)
					{
						TopItems.Add(Items[TopItems.Count]);
					}
					break;
			}
		}

		private ObservableCollection<TestItem> _items = new ObservableCollection<TestItem>();
		public ObservableCollection<TestItem> Items
		{
			get { return this._items; }
		}

		private ObservableCollection<TestItem> _topItem = new ObservableCollection<TestItem>();
		public ObservableCollection<TestItem> TopItems
		{
			get { return this._topItem; }
		}
	}
}
