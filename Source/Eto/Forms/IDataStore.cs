using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Collections;

namespace Eto.Forms
{
	public interface IDataStore<out T>
	{
		int Count { get; }
		
		T this [int index] { 
			get;
		}
	}

	public interface IDataStore : IDataStore<object>
	{
	}

	public static class DataStoreExtensions
	{
		public static IEnumerable<T> AsEnumerable<T> (this IDataStore<T> store)
		{
			if (store == null)
				yield break;
			for (int i = 0; i < store.Count; i++)
				yield return store[i];
		}
	}

	public class DataStoreCollection<T> : ObservableCollection<T>, IDataStore<T>
	{
		public DataStoreCollection ()
		{
		}
		
		public DataStoreCollection (IEnumerable<T> items)
		{
			AddRange (items);
		}

		public void Sort (IComparer<T> comparer)
		{
			var list = Items as List<T>;
			if (list != null) {
				list.Sort (comparer);
				OnCollectionChanged (new NotifyCollectionChangedEventArgs (NotifyCollectionChangedAction.Reset));
				return;
			}
			for (int i = Count - 1; i >= 0; i--) {
				for (int j = 1; j <= i; j++) {
					var o1 = this [j - 1];
					var o2 = this [j];

					if (comparer.Compare (o1, o2) > 0) {
						Remove(o1);
						Insert(j, o1);
					}
				}
			}
		}

		public void Sort (Comparison<T> comparison)
		{
			var list = Items as List<T>;
			if (list != null) {
				list.Sort (comparison);
				OnCollectionChanged (new NotifyCollectionChangedEventArgs (NotifyCollectionChangedAction.Reset));
				return;
			}
			for (int i = Count - 1; i >= 0; i--) {
				for (int j = 1; j <= i; j++) {
					var o1 = this [j - 1];
					var o2 = this [j];

					if (comparison (o1, o2) > 0) {
						Remove(o1);
						Insert(j, o1);
					}
				}
			}
		}

		public void AddRange (IEnumerable<T> items)
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
			if (Generator.Current.IsWpf)
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			else
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, itemList, oldIndex));
		}
	}
}
