using System;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Forms
{
	public partial interface IGridView : IGrid
	{
		IDataStore DataStore { get; set; }

		bool ShowCellBorders { get; set; }
	}

	public class GridViewCellArgs : EventArgs
	{
		public GridColumn GridColumn { get; private set; }

		public int Row { get; private set; }

		public int Column { get; private set; }

		public object Item { get; private set; }

		public GridViewCellArgs(GridColumn gridColumn, int row, int column, object item)
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

		internal IDataStoreView DataStoreView { get; private set; }
		// provides sorting and filtering on the model.
		GridViewSelection selection;
		// manages the selection
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

		static GridView()
		{
			EventLookup.Register(typeof(GridView), "OnCellClick", GridView.CellClickEvent);
		}

		public GridView()
			: this((Generator)null)
		{
		}

		public GridView(Generator generator)
			: this(generator, typeof(IGridView))
		{
		}

		protected GridView(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
			// Always attach the SelectionChangedEvent
			// since it is always handled in the GridView.
			HandleEvent(Grid.SelectionChangedEvent);
		}

		/// <summary>
		/// The model data store.
		/// Setting this creates a DataStoreView, and the handler's
		/// DataStore is set to the view collection of the DataStoreView.
		/// </summary>
		public IDataStore DataStore
		{
			get { return DataStoreView == null ? null : DataStoreView.Model; }
			set
			{
				// initialize the selection
				selection = new GridViewSelection(this, value);

				// Create a data store view wrapping the model
				DataStoreView = value == null ? null : new DataStoreView { Model = value };

				// Initialize the sort comparer and filter since a new view has been created.
				SortComparer = sortComparer;
				Filter = filter;				

				// Set the handler's data store to the sorted and filtered view.
				Handler.DataStore = DataStoreView == null ? null : DataStoreView.View;
			}
		}

		/// <summary>
		/// If true, there is a 1-pixel space between cells.
		/// If false, there is no space between cells.
		/// </summary>
		public bool ShowCellBorders
		{
			get { return Handler.ShowCellBorders; }
			set { Handler.ShowCellBorders = value; }
		}

		#region Events

		public const string CellClickEvent = "GridView.CellClick";

		public event EventHandler<GridViewCellArgs> CellClick
		{
			add { Properties.AddHandlerEvent(CellClickEvent, value); }
			remove { Properties.RemoveEvent(CellClickEvent, value); }
		}

		public virtual void OnCellClick(GridViewCellArgs e)
		{
			Properties.TriggerEvent(CellClickEvent, this, e);
		}

		#endregion

		protected override GridViewCellArgs ViewToModel(GridViewCellArgs e)
		{
			return new GridViewCellArgs(
				e.GridColumn,
				DataStoreView.ViewToModel(e.Row),
				e.Column,
				e.Item);
		}

		public override void OnSelectionChanged(EventArgs e)
		{
			if (selection != null && !selection.SuppressSelectionChanged)
				base.OnSelectionChanged(e);
		}

		Comparison<object> sortComparer;

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
				if (DataStoreView != null)
					DataStoreView.Filter = value;
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
			get { return selection != null ? selection.SelectedRows : new List<int>(); }
		}

		/// <summary>
		/// The model indexes of the displayed rows.
		/// E.g. ViewRows[5] is the index in the data store of
		/// the 6th displayed item.
		/// </summary>		
		public IEnumerable<int> ViewRows
		{
			get { return DataStoreView.ViewRows; }
		}

		/// <summary>
		/// Selects the view row of the specified model row index
		/// </summary>
		public override void SelectRow(int row)
		{
			if (selection != null)
				selection.SelectRow(row);
		}

		/// <summary>
		/// Unselects the view row of the specified model row index
		/// </summary>
		public override void UnselectRow(int row)
		{
			if (selection != null)
				selection.UnselectRow(row);
		}

		public override void SelectAll()
		{
			if (selection != null)
				selection.SelectAll();
		}

		public override void UnselectAll()
		{
			if (selection != null)
				selection.UnselectAll();
		}

		/// <summary>
		/// Selects the next item in the view (not the model.)
		/// This can be used to cursor up/down the view
		/// </summary>
		public void SelectNextViewRow()
		{
			SelectNextViewRow(next: true);
		}

		public void SelectPreviousViewRow()
		{
			SelectNextViewRow(next: false);
		}

		void SelectNextViewRow(bool next)
		{
			var increment = next ? 1 : -1;
			int? modelRowToSelect = null; // If there are no selected rows, this is the default

			var rows = SelectedRows.ToArray();
			if (DataStoreView != null && rows.Length > 0)
			{
				// Get the last (or first, if moving back) selected view row.
				// This handles multiselection.
				int? currentRowViewIndex = null;
				foreach (var x in rows)
				{
					var temp = DataStoreView.ModelToView(x);
					if (temp != null &&
					    (currentRowViewIndex == null || Math.Sign(temp.Value - currentRowViewIndex.Value) == Math.Sign(increment)))
						currentRowViewIndex = temp;
				}

				if (currentRowViewIndex != null)
				{
					var newRow = currentRowViewIndex.Value + increment; // view index
					if (newRow >= 0 &&
					    DataStore.Count > newRow)
						modelRowToSelect = DataStoreView.ViewToModel(newRow);
				}
			}

			if (modelRowToSelect == null)
			{
				var viewRows = ViewRows.ToArray();
				if (viewRows.Length > 0)
					modelRowToSelect = next ? viewRows.First() : viewRows.Last();
			}

			if (modelRowToSelect != null)
			{
				UnselectAll();
				SelectRow(modelRowToSelect.Value);
			}
		}
	}
}

