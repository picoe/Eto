using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Eto.Forms
{
	public interface IDataStore<T>
	{
		int Count { get; }
		
		T this [int index] { 
			get;
		}
	}

	public class DataStoreCollection<T> : ObservableCollection<T>, IDataStore<T>
	{

		public static IEnumerable<T> EnumerateDataStore (IDataStore<T> store)
		{
			if (store == null)
				yield break;
			for (int i = 0; i < store.Count; i++)
				yield return store [i];
		}
		
		public void AddRange (IEnumerable<T> items)
		{
			foreach (var item in items) {
				Add (item);
			}
		}
	}
}
