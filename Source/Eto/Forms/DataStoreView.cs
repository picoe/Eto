using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections.Specialized;
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


	/// <summary>
	/// Manages the selection of a grid view, and maintains it
	/// under four types of operations:
	/// 
	/// 1) The user changes the selection in the control. In this case the selection indexes are recalculated.
	/// 2) The application programmatically changes the selection using SelectRow(), SelectAll(), etc.
	/// 3) A sort or filter is applied to the GridView. 
	/// 4) The DataStore changes, i.e. items are added, removed or modified.
	/// </summary>
	public class GridViewSelection
	{
		enum GridViewSelectionState
		{
			Normal,
			/// <summary>
			/// The selection is being changed programmatically 
			/// by GridViewSelection, not by a user.
			/// </summary>
			SelectionChanging,
			/// <summary>
			/// The selection was changed programmatically 
			/// by GridViewSelection, not by a user.
			/// </summary>
			SelectionChanged
		}

		GridViewSelectionState state;
		bool areAllObjectsSelected;
		GridView gridView;
		private IDataStore DataStore { get { return gridView != null ? gridView.DataStore : null; } }
		private IDataStoreView DataStoreView { get { return gridView != null ? gridView.DataStoreView : null; } }
		private IGridView Handler { get { return gridView != null ? gridView.Handler : null; } }
		private bool AllowMultipleSelection { get { return gridView != null && gridView.AllowMultipleSelection; } }


		/// <summary>
		/// Called when the underlying control's selection changes.
		/// Returns true if the change notification should be suppressed. This is
		/// the case when this GridViewSelection is making changes programmatically
		/// and needs to suppress the selection change notifications.
		/// </summary>
		public bool SuppressSelectionChanged
		{
			get
			{
				// In the Normal state, this is called when the user
				// changed the selection. We refer back to the control
				// to determine the new set of rows.
				if (state == GridViewSelectionState.Normal)
				{
					if (!areAllObjectsSelected) // optimization to avoid iterating if all objects are selected
					{
						selectedRows.Clear();
						var s = Handler.SelectedRows;
						if (s != null)
							foreach (var i in s)
								selectedRows.Add(DataStoreView.ViewToModel(i));
					}
				}

				// Don't suppress notifications when the state is Normal or SelectionChanged.
				var suppressSelectionChanged = state == GridViewSelectionState.SelectionChanging;
				if (state == GridViewSelectionState.SelectionChanged)
					state = GridViewSelectionState.Normal;
				return state == GridViewSelectionState.SelectionChanging;
			}
		}

		/// <summary>
		/// The indexes of the selected objects in the model 
		/// (not the view).
		/// </summary>
		SortedSet<int> selectedRows;
		public IEnumerable<int> SelectedRows
		{
			get
			{
				// If all objects are selected, return the range [0, count-1]
				if (areAllObjectsSelected && this.DataStore != null)
					return Enumerable.Range(0, this.DataStore.Count);
				return selectedRows ?? (IEnumerable<int>)new List<int>();
			}
		}

		private void ChangeSelection(Action a)
		{
			state = GridViewSelectionState.SelectionChanging;
			a(); // Causes GridView.OnSelectionChanged to trigger which calls SuppressSelectionChanged which returns true.
			state = GridViewSelectionState.SelectionChanged;
			gridView.OnSelectionChanged(); // Calls SuppressSelectionChanged which returns false.
			state = GridViewSelectionState.Normal; // This should already be done in SuppressSelectionChanged but repeated for robustness
		}

		/// <summary>
		/// Programmatically selects a row.
		/// </summary>
		public void SelectRow(int row)
		{
			ChangeSelection(() => {
				if (!AllowMultipleSelection)
					selectedRows.Clear();

				selectedRows.Add(row);
				var viewIndex = DataStoreView.ModelToView(row);
				if (viewIndex != null)
				{
					if (!AllowMultipleSelection)
						Handler.UnselectAll();
					Handler.SelectRow(viewIndex.Value);
				}
			});
		}

		public void UnselectRow(int row)
		{
			ChangeSelection(() => {
				if (selectedRows.Contains(row))
					selectedRows.Remove(row);

				var viewIndex = DataStoreView.ModelToView(row);
				if (viewIndex != null) // can be null if the row is not in the view because it is filtered out
					Handler.UnselectRow(viewIndex.Value);
			});
		}

		internal void SelectAll()
		{
			ChangeSelection(() => {
				areAllObjectsSelected = true;
				selectedRows.Clear();
				Handler.SelectAll();
			});
		}

		internal void UnselectAll()
		{
			ChangeSelection(() => {
				areAllObjectsSelected = true;
				selectedRows.Clear();
				Handler.UnselectAll();
			});
		}

		public GridViewSelection(GridView gridView, IDataStore dataStore)
		{
			this.gridView = gridView;
			this.selectedRows = new SortedSet<int>();
			this.state = GridViewSelectionState.Normal;
			var collection = dataStore as INotifyCollectionChanged;
			if(collection != null)
				collection.CollectionChanged += OnCollectionChanged;
		}

		void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
			}
			else if (e.Action == NotifyCollectionChangedAction.Move)
			{
			}
			else if (e.Action == NotifyCollectionChangedAction.Remove)
			{
			}
		}
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
		Dictionary<int, int> viewToModel = null;
		Dictionary<int, int> modelToView = null;

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

		public int ViewToModel(int viewIndex)
		{
			var result = viewIndex;

			var temp = 0;
			if (HasSortOrFilter &&
				viewToModel != null &&
				viewToModel.TryGetValue(viewIndex, out temp))
				result = temp;

			return result;
		}

		public int? ModelToView(int modelIndex)
		{
			if (!HasSortOrFilter)
				return modelIndex;

			var temp = 0;
			if (modelToView != null &&
				modelToView.TryGetValue(modelIndex, out temp))
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

		/// <summary>
		/// Used to filter the displayed items.
		/// If SortComparer or Filter is specified, the model items
		/// should implement Equals so that model-to-view
		/// mapping can be done.
		/// </summary>
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
		private void UpdateView()
		{
			if (view != null &&
				model != null)
			{
				var temp = new DataStoreVirtualCollection<object>(model);

				// filter if needed
				var viewItems = (Filter != null) ? temp.Where(Filter).ToList() : temp.ToList();

				// sort if needed
				if (this.comparer != null)
					viewItems.Sort(this.comparer);

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
					viewToModel = new Dictionary<int, int>();
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
						var modelIndex = 0;
						if (o != null &&
							modelIndexes.TryGetValue(o, out modelIndex))
						{
							viewToModel[viewIndex] = modelIndex;
							modelToView[modelIndex] = viewIndex;
						}
						viewIndex++;
					}
				}
			}
		}

		private bool HasSortOrFilter
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
