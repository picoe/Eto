using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Eto.Forms
{
	/// <summary>
	/// Manages the selection of a grid view, and maintains it
	/// under four types of operations:
	/// 
	/// 1) The user changes the selection in the control. In this case the selection indexes are recalculated.
	/// 2) The application programmatically changes the selection using SelectRow(), SelectAll(), etc.
	///    In this case the internal selection is updated and SelectionChanged is fired.
	/// 3) A sort or filter is applied to the GridView. 
	/// 4) The DataStore changes, i.e. items are added, removed or modified.
	/// <copyright>(c) 2013 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
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
		readonly GridView gridView;

		IDataStore DataStore { get { return gridView == null ? null : gridView.DataStore; } }

		IDataStoreView DataStoreView { get { return gridView == null ? null : gridView.DataStoreView; } }

		IGridView Handler { get { return gridView == null ? null : gridView.Handler; } }

		bool AllowMultipleSelection { get { return gridView != null && gridView.AllowMultipleSelection; } }

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
				if (state == GridViewSelectionState.SelectionChanged)
				{
					state = GridViewSelectionState.Normal;
					return false;
				}
				return state == GridViewSelectionState.SelectionChanging;
			}
		}

		readonly SortedSet<int> selectedRows;

		/// <summary>
		/// The indexes of the selected objects in the model
		/// </summary>
		public IEnumerable<int> SelectedRows
		{
			get
			{
				// If all objects are selected, return the range [0, count-1]
				if (areAllObjectsSelected && DataStore != null)
					return Enumerable.Range(0, DataStore.Count);
				return selectedRows ?? Enumerable.Empty<int>();
			}
		}

		/// <summary>
		/// A GridView's GridViewSelection maintains the set of 
		/// selected items.
		/// 
		/// When sorting or filtering is applied, 
		/// it is necessary to recompute the view selection, since the view
		/// only tracks items by index.
		/// 
		/// This class contains the logic to map selected items to their
		/// view indexes.
		/// 
		/// Preserving the selection currently takes O(nlogn) if there
		/// is a non-empty selection. This is very slow in the case
		/// when multiple insertions and deletions occur together.
		/// </summary>
		class SelectionPreserver : IDisposable
		{
			GridViewSelection s;
			HashSet<object> previousSelectedItems;

			public SelectionPreserver(GridViewSelection s)
			{
				this.s = s;
				previousSelectedItems = new HashSet<object>();
				foreach (var i in s.SelectedRows)
					previousSelectedItems.Add(s.DataStoreView.Model[i]);
			}

			public void Dispose()
			{
				if (previousSelectedItems.Count > 0) // optimization
				{
					// Determine which of the originally selected objects still
					// exist and which have been removed.
					var selectedItems = new HashSet<object>();
					var removedSelectedItems = new HashSet<object>();
					var selectedRows = new SortedSet<int>();
					var model = s.DataStoreView.Model;
					// O(nlogn)
					for (var i = 0; i < model.Count; ++i)
					{
						var item = model[i];
						if (previousSelectedItems.Contains(item))
						{
							selectedItems.Add(item);
							selectedRows.Add(i);
						}
					}

					foreach (var item in previousSelectedItems)
						if (!selectedItems.Contains(item))
							removedSelectedItems.Add(item);

					s.ResetSelection(selectedRows, selectedItems, removedSelectedItems);
				}
			}
		}

		/// <summary>
		/// Resets the selection to the specified model indexes.
		/// Called by
		/// a) SelectionPreserver.Dispose() during sort and filter operations
		/// b) OnCollectionChanged when the model collection is changed.
		/// </summary>
		private void ResetSelection(SortedSet<int> newSelectedRows, 
			HashSet<object> selectedItems = null, 
			HashSet<object> removedSelectedItems = null)
		{
			state = GridViewSelectionState.SelectionChanging; // causes selection events to be suppressed.

			// Set the selection model indexes
			selectedRows.Clear();
			foreach (var i in newSelectedRows)
				selectedRows.Add(i);

			// Create the set of selected items if it wasn't supplied
			if (selectedItems == null)
			{
				selectedItems = new HashSet<object>();
				foreach (var i in selectedRows)
					selectedItems.Add(DataStoreView.Model[i]);
			}

			// Calculate the view indexes
			var view = DataStoreView.View;
			var selectedRowViewIndexes = new SortedSet<int>();
			for (var i = 0; i < view.Count; ++i) // O(nlogn)
				if (selectedItems.Contains(view[i]))
					selectedRowViewIndexes.Add(i);

			// Reselect the rows in the handler.
			Handler.UnselectAll();
			foreach (var i in selectedRowViewIndexes)
				Handler.SelectRow(i);

			state = GridViewSelectionState.Normal; // start firing selection changed events again.

			// TODO: should we fire SelectionChanged events if
			// selected items were removed? This is an edge case;
			// implement only if needed.
		}

		void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			var model = DataStoreView.Model;

			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				// The set of selected items does not change, 
				// but the row indexes of affected items are incremented.
				var selectedItems = new HashSet<object>();
				var newSelectedRows = new SortedSet<int>();
				foreach (var i in selectedRows)
				{
					// The tricky part - if the index is greater than or equal to the location
					// where the insertions were done, add the count of objects inserted.
					var temp = i;
					if (i >= e.NewStartingIndex)
						temp += e.NewItems.Count;

					newSelectedRows.Add(temp);
					selectedItems.Add(model[temp]);
				}

				ResetSelection(newSelectedRows, selectedItems);
			}
			else if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				throw new NotImplementedException();
			}
			else if (e.Action == NotifyCollectionChangedAction.Move)
			{
				throw new NotImplementedException();
			}
			else if (e.Action == NotifyCollectionChangedAction.Replace)
			{
				throw new NotImplementedException();
			}
			else if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Wrap code that should preserve the selection with 
		/// using(PreserveSelection()).
		/// This takes a snapshot of the selection and restores
		/// it during dispose.
		/// </summary>
		public IDisposable PreserveSelection()
		{
			return new SelectionPreserver(this);
		}

		void ChangeSelection(Action a)
		{
			state = GridViewSelectionState.SelectionChanging;
			a(); // Causes GridView.OnSelectionChanged to trigger which calls SuppressSelectionChanged which returns true.
			state = GridViewSelectionState.SelectionChanged;
			gridView.OnSelectionChanged(EventArgs.Empty); // Calls SuppressSelectionChanged which returns false.
			state = GridViewSelectionState.Normal; // This should already be done in SuppressSelectionChanged but repeated for robustness
		}

		/// <summary>
		/// Programmatically selects a row.
		/// </summary>
		/// <param name="row">Index of the row to select in the model</param>
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

		/// <summary>
		/// Unselects the specified row
		/// </summary>
		/// <param name="row">Index of the row to unselect in the model</param>
		public void UnselectRow(int row)
		{
			ChangeSelection(() => {
				areAllObjectsSelected = false;
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
				areAllObjectsSelected = false;
				selectedRows.Clear();
				Handler.UnselectAll();
			});
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.GridViewSelection"/> class.
		/// </summary>
		/// <param name="gridView">Grid view associated with the selection object</param>
		/// <param name="dataStore">Data store to iterate</param>
		public GridViewSelection(GridView gridView, IDataStore dataStore)
		{
			this.gridView = gridView;
			this.selectedRows = new SortedSet<int>();
			this.state = GridViewSelectionState.Normal;
			var collection = dataStore as INotifyCollectionChanged;
			if (collection != null)
				collection.CollectionChanged += OnCollectionChanged;
		}
	}
}
