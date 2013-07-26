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

	/// <summary>
	/// Provides sorting and filtering to a data store.
	/// 
	/// TODO: should this be made generic, to handle
	/// an IDataStore of T?
	/// </summary>
	public interface IDataStoreView
	{
		/// <summary>
		/// The model store. This is the input to the IDataStoreView.
		/// </summary>
		IDataStore Model { get; set; }

		/// <summary>
		/// The filtered and sorted view of the model datastore.
		/// The IDataStoreView creates this when Model is set.
		/// View's object reference does not change until the
		/// model is reset. The contents of View change
		/// when Model's contents change or when SortComparer or
		/// Filter change.
		/// </summary>
		IDataStore View { get; }

		/// <summary>
		/// Converts the index of an object in the view to an
		/// index of the same object in the model. This method
		/// always succeeds since the view is a subset of the model.
		/// </summary>
		int ViewToModel(int index);

		/// <summary>
		/// Converts the index of an object in the model to an
		/// index of the same object in the view. 
		/// 
		/// Returns -1 if the object is not in the view because
		/// it is filtered out.
		/// </summary>
		int ModelToView(int index);

		/// <summary>
		/// If non-null, sorts the model using this comparer.
		/// </summary>
		Comparison<object> SortComparer { get; set; }

		/// <summary>
		/// If non-null, excludes objects from the view for which
		/// Filter returns false.
		/// </summary>
		Func<object, bool> Filter { get; set; }
	}

	public class DataStoreView : IDataStoreView
	{
		public IDataStore model;
		public IDataStore Model
		{
			get { return model; }
			set { model = value; }
		}

		public IDataStore View
		{
			get { return model; }
		}

		public int ViewToModel(int index)
		{
			return index;
		}

		public int ModelToView(int index)
		{
			return index;
		}

		private Comparison<object> sortComparer;
		public Comparison<object> SortComparer
		{
			get { return sortComparer; }
			set { sortComparer = value; }
		}

		private Func<object, bool> filter;
		public Func<object, bool> Filter
		{
			get { return filter; }
			set { filter = value; }
		}
	}
}
