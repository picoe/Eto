using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Collections;

namespace Eto.Forms
{
	/// <summary>
	/// Base data store interface to bind to a collection of objects of a particular type.
	/// </summary>
	/// <remarks>
	/// Note that you should use an <see cref="System.Collections.ObjectModel.ObservableCollection{T}"/> if you want
	/// the control to respond to changes of the collection.
	/// </remarks>
	public interface IDataStore<out T>
	{
		/// <summary>
		/// Gets the number of items in this data store.
		/// </summary>
		/// <value>The count of items.</value>
		int Count { get; }

		/// <summary>
		/// Gets the object at the specified index.
		/// </summary>
		/// <param name="index">Index of the item to get.</param>
		T this [int index] { get; }
	}

	/// <summary>
	/// Interface for an object-based data store
	/// </summary>
	[Obsolete("No longer needed, use standard collections instead")]
	public interface IDataStore : IDataStore<object>
	{
	}

	/// <summary>
	/// Extensions for <see cref="IDataStore{T}"/>
	/// </summary>
	public static class DataStoreExtensions
	{
		/// <summary>
		/// Enumerates all items in a <see cref="IDataStore{T}"/>
		/// </summary>
		/// <returns>The enumerable of all items.</returns>
		/// <param name="store">Data store to enumerate.</param>
		/// <typeparam name="T">The item type in the data store.</typeparam>
		public static IEnumerable<T> AsEnumerable<T>(this IDataStore<T> store)
		{
			if (store == null)
				yield break;
			for (int i = 0; i < store.Count; i++)
				yield return store[i];
		}
	}

	/// <summary>
	/// Collection of items that implements the <see cref="IDataStore{T}"/> interface
	/// </summary>
	public class DataStoreCollection<T> : ObservableCollection<T>, IDataStore<T>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.DataStoreCollection{T}"/> class.
		/// </summary>
		public DataStoreCollection()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.DataStoreCollection{T}"/> class with the specified items.
		/// </summary>
		/// <param name="items">Items to initialize the collection with.</param>
		public DataStoreCollection(IEnumerable<T> items)
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

	/// <summary>
	/// A data store of plain objects, that can be bound to a grid view.
	/// </summary>
	public class DataStoreCollection : DataStoreCollection<object>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.DataStoreCollection"/> class.
		/// </summary>
		public DataStoreCollection()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.DataStoreCollection"/> class with the specified items.
		/// </summary>
		/// <param name="items">Items to populate the data store with initially.</param>
		public DataStoreCollection(IEnumerable<object> items)
			: base(items)
		{
		}
	}
}
