using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections.Specialized;

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
				yield return (T)store[i];
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
			var list = base.Items as List<T>;
			if (list != null) {
				list.Sort (comparer);
				OnCollectionChanged (new NotifyCollectionChangedEventArgs (NotifyCollectionChangedAction.Reset));
				return;
			}
			for (int i = this.Count - 1; i >= 0; i--) {
				for (int j = 1; j <= i; j++) {
					var o1 = this [j - 1];
					var o2 = this [j];

					if (comparer.Compare (o1, o2) > 0) {
						this.Remove (o1);
						this.Insert (j, o1);
					}
				}
			}
		}

		public void Sort (Comparison<T> comparison)
		{
			var list = base.Items as List<T>;
			if (list != null) {
				list.Sort (comparison);
				OnCollectionChanged (new NotifyCollectionChangedEventArgs (NotifyCollectionChangedAction.Reset));
				return;
			}
			for (int i = this.Count - 1; i >= 0; i--) {
				for (int j = 1; j <= i; j++) {
					var o1 = this [j - 1];
					var o2 = this [j];

					if (comparison (o1, o2) > 0) {
						this.Remove (o1);
						this.Insert (j, o1);
					}
				}
			}
		}

		public void AddRange (IEnumerable<T> items)
		{
			foreach (var item in items) {
				Add (item);
			}
		}
	}
}
