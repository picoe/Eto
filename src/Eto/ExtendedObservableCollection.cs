using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Collections;

namespace Eto
{

	/// <summary>
	/// Observable collection with extended functionality such as sorting and adding a range of items
	/// </summary>
	public class ExtendedObservableCollection<T> : ObservableCollection<T>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.ExtendedObservableCollection{T}"/> class.
		/// </summary>
		public ExtendedObservableCollection()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.ExtendedObservableCollection{T}"/> class with the specified items.
		/// </summary>
		/// <param name="items">Items to initialize the collection with.</param>
		public ExtendedObservableCollection(IEnumerable<T> items)
			: base(items)
		{
		}

		/// <summary>
		/// Sorts the collection using the specified <paramref name="comparer"/>.
		/// </summary>
		/// <param name="comparer">Comparer for the sort.</param>
		public void Sort(IComparer<T> comparer)
		{
			var list = Items as List<T>;
			if (list != null)
			{
				list.Sort(comparer);
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
				return;
			}
			for (int i = Count - 1; i >= 0; i--)
			{
				for (int j = 1; j <= i; j++)
				{
					var o1 = this[j - 1];
					var o2 = this[j];

					if (comparer.Compare(o1, o2) > 0)
					{
						Remove(o1);
						Insert(j, o1);
					}
				}
			}
		}

		/// <summary>
		/// Sorts the collection using the specified <paramref name="comparison"/>.
		/// </summary>
		/// <param name="comparison">Comparison for the sort.</param>
		public void Sort(Comparison<T> comparison)
		{
			var list = Items as List<T>;
			if (list != null)
			{
				list.Sort(comparison);
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
				return;
			}
			for (int i = Count - 1; i >= 0; i--)
			{
				for (int j = 1; j <= i; j++)
				{
					var o1 = this[j - 1];
					var o2 = this[j];

					if (comparison(o1, o2) > 0)
					{
						Remove(o1);
						Insert(j, o1);
					}
				}
			}
		}

		/// <summary>
		/// Adds the specified <paramref name="items"/> to the collection.
		/// </summary>
		/// <param name="items">Items to add to the collection.</param>
		public void AddRange(IEnumerable<T> items)
		{
			// We don't call Add(item) for each item because
			// that triggers a notification each time.
			// Instead we add all items to the underlying collection
			// and then trigger a single notification for the range
			// http://stackoverflow.com/a/851129/90291

			var itemList = items as IList ?? items.ToArray(); // don't enumerate more than once
			var oldIndex = Items.Count;
			foreach (T item in itemList)
				Items.Add(item);

			// range is not supported by WPF, so send a reset
			if (Platform.Instance != null &&
				Platform.Instance.IsWpf)
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			else
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, itemList, oldIndex));
		}
	}
	
}
