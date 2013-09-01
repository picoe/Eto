using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Eto.Forms
{
#if OBSOLETE
	public interface IGridStore : IDataStore
	{
	}
#endif

	public partial interface IGridView : IGrid
	{
		IDataStore DataStore { get; set; }
	}

	public class GridViewCellArgs : EventArgs
	{
		public GridColumn GridColumn { get; private set; }

		public int Row { get; private set; }

		public int Column { get; private set; }

		public object Item { get; private set; }

		public GridViewCellArgs (GridColumn gridColumn, int row, int column, object item)
		{
			this.GridColumn = gridColumn;
			this.Row = row;
			this.Column = column;
			this.Item = item;
		}
	}

	public partial class GridView : Grid
	{
		public new IGridView Handler { get { return base.Handler as IGridView; } }
		internal IDataStoreView DataStoreView { get; private set; } // provides sorting and filtering on the model.
		GridViewSelection selection; // manages the selection

#if MOBILE
		/// <summary>
		/// A delegate method to delete an item in response to a user's
		/// request. The method should return true after deleting the
		/// item, or false to indicate the item could not be deleted.
		/// </summary>
		public Func<object, bool> DeleteItemHandler { get; set; }

		/// <summary>
		/// A delegate that returns true if an item can be edited
		/// </summary>
		public Func<object, bool> CanDeleteItem { get; set; }

		/// <summary>
		/// The text to display in a Delete item button.
		/// </summary>
		public string DeleteConfirmationTitle { get; set; }
#endif

		public GridView ()
			: this (Generator.Current)
		{
		}

		public GridView (Generator g)
			: this (g, typeof (IGridView))
		{
		}
		
		protected GridView (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
		}

		/// <summary>
		/// The model data store.
		/// Setting this creates a DataStoreView, and the handler's
		/// DataStore is set to the view collection of the DataStoreView.
		/// </summary>
		public IDataStore DataStore {
			get { return DataStoreView != null ? DataStoreView.Model : null; }
			set
			{
				// initialize the selection
				this.selection = new GridViewSelection(this, value);

				// Create a data store view wrapping the model
				DataStoreView = value != null ? new DataStoreView { Model = value } : null;

				// Initialize the sort comparer and filter since a new view has been created.
				this.SortComparer = this.sortComparer;
				this.Filter = this.filter;				

				// Set the handler's data store to the sorted and filtered view.
				Handler.DataStore = DataStoreView != null ? DataStoreView.View : null;
			}
		}

		public override void OnSelectionChanged()
		{
			if (this.selection != null &&
				!this.selection.SuppressSelectionChanged)
				base.OnSelectionChanged();
		}

		private Comparison<object> sortComparer;
		public Comparison<object> SortComparer
		{
			get { return sortComparer; }
			set
			{
				sortComparer = value;
				if (DataStoreView != null)
					DataStoreView.SortComparer = value;
			}
		}

		Func<object, bool> filter;
		public Func<object, bool> Filter
		{
			get { return filter; }
			set
			{
				filter = value;
				if (DataStoreView != null) DataStoreView.Filter = value;
			}
		}

		public override IEnumerable<object> SelectedItems
		{
			get
			{
				if (DataStore == null)
					yield break;
				if (SelectedRows != null)
					foreach (var row in SelectedRows)
						yield return DataStore[row];
			}
		}

		/// <summary>
		/// Does view to model mapping of the selected row indexes.
		/// </summary>
		public override IEnumerable<int> SelectedRows
		{
			get { return this.selection != null ? this.selection.SelectedRows : new List<int>(); }
		}

		/// <summary>
		/// Selects the view row of the specified model row index
		/// </summary>
		public override void SelectRow(int row)
		{
			if (this.selection != null)
				this.selection.SelectRow(row);
		}

		/// <summary>
		/// Unselects the view row of the specified model row index
		/// </summary>
		public override void UnselectRow(int row)
		{
			if (this.selection != null)
				this.selection.UnselectRow(row);
		}

		public override void SelectAll()
		{
			if (this.selection != null)
				this.selection.SelectAll();
		}

		public override void UnselectAll()
		{
			if (this.selection != null)
				this.selection.UnselectAll();
		}
	}
}

