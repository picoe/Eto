using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

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
		CollectionHandler collectionHandler;

		/// <summary>
		/// The collection that serves as the view.
		/// 
		/// This object reference is kept throughout the life of the
		/// DataStoreView.
		/// </summary>
		readonly GridItemCollection view = new GridItemCollection();
		readonly MyComparer comparer = new MyComparer();

		public IDataStore model;
		public IDataStore Model
		{
			get { return model; }
			set 
			{
				// unregister the old dataStoreChangedHandler if there is one
				if (collectionHandler != null)
					collectionHandler.Unregister();

				model = value;
				collectionHandler = new CollectionHandler { Handler = this };
				collectionHandler.Register(value);				
			}
		}

		public IDataStore View
		{
			get { return view; }
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
			get { return comparer.SortComparer; }
			set
			{
				if (!object.ReferenceEquals(SortComparer, value))
				{
					comparer.SortComparer = value;
					UpdateView();
				}
			}
		}

		private Func<object, bool> filter;
		public Func<object, bool> Filter
		{
			get { return filter; }
			set
			{
				if (!object.ReferenceEquals(filter, value))
				{
					filter = value;
					UpdateView();
				}
			}
		}

		class MyComparer : IComparer<object>
		{
			public Comparison<object> SortComparer { get; set; }
			public int Compare(object x, object y)
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// When the sort or filter changes, creates or destroys the view
		/// as needed.
		/// 
		/// This should only be called if a sort or filter is applied.
		/// 
		/// BUGBUG: what happens if there was a sort or filter, but they are being removed?
		/// </summary>
		private void UpdateView()
		{
			if (view != null &&
				model != null)
			{
				var temp = new DataStoreVirtualCollection<object>(view);
				// filter if needed
				var list = (Filter != null) ? temp.Where(Filter).ToList() : temp.ToList();
				// sort if needed
				list.OrderBy(x => x, this.comparer);
				// Clear and re-add the list
				view.Clear();				
				view.AddRange(list);
			}
		}


		private bool HasSortOrFilter
		{
			get { return SortComparer != null || Filter != null; }
		}

		class CollectionHandler : DataStoreChangedHandler<object, IDataStore>
		{
			public DataStoreView Handler { get; set; }

			public override void AddRange(IEnumerable<object> items)
			{
				/// BUGBUG: what happens if there was a sort or filter, but they are being removed?
				if (Handler.HasSortOrFilter)
					Handler.UpdateView();
				else
					Handler.view.AddRange(items);

			}

			public override void AddItem(object item)
			{
				/// BUGBUG: what happens if there was a sort or filter, but they are being removed?
				if (Handler.HasSortOrFilter)
					Handler.UpdateView();
				else
					Handler.view.Add(item);
			}

			public override void InsertItem(int index, object item)
			{
				/// BUGBUG: what happens if there was a sort or filter, but they are being removed?
				if (Handler.HasSortOrFilter)
					Handler.UpdateView();
				else
					Handler.view.Insert(index, item);
			}

			public override void RemoveItem(int index)
			{
				/// BUGBUG: what happens if there was a sort or filter, but they are being removed?
				if (Handler.HasSortOrFilter)
					Handler.UpdateView();
				else
					Handler.view.RemoveAt(index);
			}

			public override void RemoveAllItems()
			{
				/// BUGBUG: what happens if there was a sort or filter, but they are being removed?
				if (Handler.HasSortOrFilter)
					Handler.UpdateView();
				else
					Handler.view.Clear();
			}
		}
	}
}
