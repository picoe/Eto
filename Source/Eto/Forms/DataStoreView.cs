using System;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Forms
{
	/// <summary>
	/// Provides sorting and filtering to a data store.
	/// 
	/// TODO: should this be made generic, to handle
	/// an IDataStore of T?
	/// 
	/// <copyright>(c) 2013 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
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
		/// The model indexes of the displayed rows.
		/// E.g. ViewRows[5] is the index in the data store of
		/// the 6th displayed item.
		/// </summary>		
		IEnumerable<int> ViewRows { get; }
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
		/// Returns null if the object is not in the view because
		/// it is filtered out.
		/// </summary>
		int? ModelToView(int index);

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
		List<int> viewToModel;
		Dictionary<int, int> modelToView;

		IDataStore model;
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

		public IEnumerable<int> ViewRows
		{
			get { return viewToModel ?? new List<int>(); }
		}

		public int ViewToModel(int index)
		{
			var result = index;

			if (HasSortOrFilter
				&& viewToModel != null
				&& index >= 0
				&& viewToModel.Count > index)
				result = viewToModel[index];

			return result;
		}

		public int? ModelToView(int index)
		{
			if (!HasSortOrFilter)
				return index;

			var temp = 0;
			if (modelToView != null && modelToView.TryGetValue(index, out temp))
				return temp;

			return null;
		}

		/// <summary>
		/// A comparer used to sort the displayed items.
		/// If SortComparer or Filter is specified, the model items
		/// should implement Equals so that model-to-view
		/// mapping can be done.
		/// </summary>
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

		Func<object, bool> filter;
		
		/// <summary>
		/// Used to filter the displayed items.
		/// If SortComparer or Filter is specified, the model items
		/// should implement Equals so that model-to-view
		/// mapping can be done.
		/// </summary>
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
				return SortComparer != null ? SortComparer(x, y) : 0;
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
		void UpdateView()
		{
			if (view != null &&
				model != null)
			{
				var temp = new DataStoreVirtualCollection<object>(model);

				// filter if needed
				var viewItems = (Filter != null) ? temp.Where(Filter).ToList() : temp.ToList();

				// sort if needed
				if (comparer != null)
					viewItems.Sort(comparer);

				// Clear and re-add the list
				view.Clear();

				view.AddRange(viewItems);

				// If a sort or filter is specified, create a dictionary
				// of the item indices. This materializes a list of all the
				// objects.
				// If no sort or filter is specified, this overhead is avoided.
				viewToModel = null;
				modelToView = null;
				if (HasSortOrFilter)
				{
					viewToModel = new List<int>();
					modelToView = new Dictionary<int, int>();

					// Create a temporary dictionary of model items
					var modelIndexes = new Dictionary<object, int>();
					for (var i = 0; i < temp.Count; ++i)
					{
						var o = temp[i];
						if (o != null)
							modelIndexes[o] = i;
					}

					var viewIndex = 0;
					foreach (var o in viewItems)
					{
						int modelIndex = -1;
						if (o != null && modelIndexes.TryGetValue(o, out modelIndex))
						{
							modelToView[modelIndex] = viewIndex;
						}
						viewToModel.Add(modelIndex); // always add to viewToModel because the number of items must match those in viewItems
						viewIndex++;
					}
				}
			}
		}

		bool HasSortOrFilter
		{
			get { return SortComparer != null || Filter != null; }
		}

		/// <summary>
		/// BUGBUG: what happens if there was a sort or filter, but they are being removed?
		/// </summary>
		class CollectionHandler : DataStoreChangedHandler<object, IDataStore>
		{
			public DataStoreView Handler { get; set; }

			public override void AddRange(IEnumerable<object> items)
			{
				if (Handler.HasSortOrFilter)
					Handler.UpdateView();
				else
					Handler.view.AddRange(items);
			}

			public override void AddItem(object item)
			{
				if (Handler.HasSortOrFilter)
					Handler.UpdateView();
				else
					Handler.view.Add(item);
			}

			public override void InsertItem(int index, object item)
			{
				if (Handler.HasSortOrFilter)
					Handler.UpdateView();
				else
					Handler.view.Insert(index, item);
			}

			public override void RemoveItem(int index)
			{
				if (Handler.HasSortOrFilter)
					Handler.UpdateView();
				else
					Handler.view.RemoveAt(index);
			}

			public override void RemoveAllItems()
			{
				if (Handler.HasSortOrFilter)
					Handler.UpdateView();
				else
					Handler.view.Clear();
			}

			public override void RemoveRange(IEnumerable<object> items)
			{
				// TODO: the base implementation is slow
				base.RemoveRange(items); 
			}

			public override void InsertRange(int index, IEnumerable<object> items)
			{
				// TODO: the base implementation is slow
				base.InsertRange(index, items); 
			}

			public override void RemoveRange(int index, int count)
			{
				// TODO: the base implementation is slow
				base.RemoveRange(index, count); 
			}
		}
	}
}
