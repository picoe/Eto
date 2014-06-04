using System;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Forms
{
	/// <summary>
	/// Event arguments for cell-based events of a <see cref="GridView"/>
	/// </summary>
	[Obsolete("Use GridViewCellEventArgs instead")]
	public class GridViewCellArgs : EventArgs
	{
		/// <summary>
		/// Gets the grid column that triggered the event.
		/// </summary>
		/// <value>The grid column.</value>
		public GridColumn GridColumn { get; private set; }

		/// <summary>
		/// Gets the row that triggered the event, or -1 if no row.
		/// </summary>
		/// <value>The grid row.</value>
		public int Row { get; private set; }

		/// <summary>
		/// Gets the index of the column that triggered the event, or -1 if no column.
		/// </summary>
		/// <value>The column index.</value>
		public int Column { get; private set; }

		/// <summary>
		/// Gets the item of the row that triggered the event, or null if there was no item.
		/// </summary>
		/// <value>The row item.</value>
		public object Item { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.GridViewCellArgs"/> class.
		/// </summary>
		/// <param name="gridColumn">Grid column.</param>
		/// <param name="row">Row.</param>
		/// <param name="column">Column.</param>
		/// <param name="item">Item.</param>
		public GridViewCellArgs(GridColumn gridColumn, int row, int column, object item)
		{
			this.GridColumn = gridColumn;
			this.Row = row;
			this.Column = column;
			this.Item = item;
		}
	}

	#pragma warning disable 612,618

	/// <summary>
	/// Event arguments for cell-based events of a <see cref="GridView"/>
	/// </summary>
	public class GridViewCellEventArgs : GridViewCellArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.GridViewCellEventArgs"/> class.
		/// </summary>
		/// <param name="gridColumn">Grid column that triggered the event.</param>
		/// <param name="row">The row that triggered the event, or -1 if no row.</param>
		/// <param name="column">Column that triggered the event, or -1 if no column.</param>
		/// <param name="item">Item of the row that triggered the event, or null if no item.</param>
		public GridViewCellEventArgs(GridColumn gridColumn, int row, int column, object item)
			: base(gridColumn, row, column, item)
		{
		}
	}

	#pragma warning restore 612,618

	/// <summary>
	/// Control to present data in a grid in columns and rows.
	/// </summary>
	/// <see cref="TreeGridView"/>
	[Handler(typeof(GridView.IHandler))]
	public class GridView : Grid
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		// provides sorting and filtering on the model.
		internal IDataStoreView DataStoreView { get; private set; }
		// manages the selection
		GridViewSelection selection;
		
		/// <summary>
		/// A delegate method to delete an item in response to a user's
		/// request. The method should return true after deleting the
		/// item, or false to indicate the item could not be deleted.
		/// 
		/// Currently supported on iOS only.
		/// </summary>
		public Func<object, bool> DeleteItemHandler { get; set; }

		/// <summary>
		/// A delegate that returns true if an item can be deleted
		/// 
		/// Currently supported on iOS only.
		/// </summary>
		public Func<object, bool> CanDeleteItem { get; set; }

		/// <summary>
		/// The text to display in a Delete item button.
		/// 
		/// Currently supported on iOS only.
		/// </summary>
		public Func<object, string> DeleteConfirmationTitle { get; set; }

		static GridView()
		{
			EventLookup.Register<GridView>(c => c.OnCellClick(null), GridView.CellClickEvent);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.GridView"/> class.
		/// </summary>
		public GridView()
		{
			InitializeGrid();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.GridView"/> class with the specified handler.
		/// </summary>
		/// <param name="handler">Platform handler to use for the implementation of this GridView instance.</param>
		protected GridView(IHandler handler)
			: base(handler)
		{
			InitializeGrid();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.GridView"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		[Obsolete("Use default constructor instead")]
		public GridView(Generator generator)
			: this(generator, typeof(IHandler))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.GridView"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		/// <param name="type">Type.</param>
		/// <param name="initialize">If set to <c>true</c> initialize.</param>
		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected GridView(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
			InitializeGrid();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.GridView"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		/// <param name="handler">Handler.</param>
		/// <param name="initialize">If set to <c>true</c> initialize.</param>
		[Obsolete("Use GridView(IGridView) instead")]
		public GridView(Generator generator, IHandler handler, bool initialize = true)
			: base(generator, handler, initialize)
		{
			InitializeGrid();
		}

		void InitializeGrid()
		{
			// Always attach the SelectionChangedEvent
			// since it is always handled in the GridView.
			HandleEvent(Grid.SelectionChangedEvent);

			// Create a selection so that Filter and SortComparer
			// can be set before DataStore.
			selection = new GridViewSelection(this, null);
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
		/// Gets or sets a value indicating whether to show a border around each cell.
		/// </summary>
		/// <value><c>true</c> to show a space between cells; otherwise, <c>false</c>.</value>
		public bool ShowCellBorders
		{
			get { return Handler.ShowCellBorders; }
			set { Handler.ShowCellBorders = value; }
		}

		#region Events

		/// <summary>
		/// Event identifier for the <see cref="CellClick"/> event.
		/// </summary>
		public const string CellClickEvent = "GridView.CellClick";

		/// <summary>
		/// Occurs when an individual cell is clicked.
		/// </summary>
		public event EventHandler<GridViewCellEventArgs> CellClick
		{
			add { Properties.AddHandlerEvent(CellClickEvent, value); }
			remove { Properties.RemoveEvent(CellClickEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="CellClick"/> event.
		/// </summary>
		/// <param name="e">Grid cell event arguments.</param>
		protected virtual void OnCellClick(GridViewCellEventArgs e)
		{
			Properties.TriggerEvent(CellClickEvent, this, e);
		}

		#endregion

		/// <summary>
		/// Raises the <see cref="Grid.SelectionChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected internal override void OnSelectionChanged(EventArgs e)
		{
			if (selection != null && !selection.SuppressSelectionChanged)
				base.OnSelectionChanged(e);
		}

		Comparison<object> sortComparer;

		/// <summary>
		/// Gets or sets the comparer to sort the data through code.
		/// </summary>
		/// <remarks>
		/// This is used to sort data programatically. If you have data coming from a database, it is usually more
		/// efficient to sort the data on the server. 
		/// </remarks>
		/// <value>The sort comparer.</value>
		public Comparison<object> SortComparer
		{
			get { return sortComparer; }
			set
			{
				using (selection.PreserveSelection())
				{
					sortComparer = value;
					if (DataStoreView != null)
						DataStoreView.SortComparer = value;
				}
			}
		}

		Func<object, bool> filter;

		/// <summary>
		/// Gets or sets the filter of the data.
		/// </summary>
		/// <remarks>
		/// This is used to filter the data programatically.  If you have data coming from a database, it is usually
		/// more efficient to filter from the server.
		/// </remarks>
		/// <value>The data filter.</value>
		public Func<object, bool> Filter
		{
			get { return filter; }
			set
			{
				using (selection.PreserveSelection())
				{
					filter = value;
					if (DataStoreView != null)
						DataStoreView.Filter = value;
				}
			}
		}

		/// <summary>
		/// Gets an enumeration of the currently selected items
		/// </summary>
		/// <value>The selected items.</value>
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

		/// <summary>
		/// Selects all rows
		/// </summary>
		public override void SelectAll()
		{
			if (selection != null)
				selection.SelectAll();
		}

		/// <summary>
		/// Clears the selection
		/// </summary>
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

		/// <summary>
		/// Selects the previous item in the view (not the model.)
		/// This can be used to cursor up/down the view
		/// </summary>
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
					modelRowToSelect = next ? viewRows[0] : viewRows.Last();
			}

			if (modelRowToSelect != null)
			{
				UnselectAll();
				SelectRow(modelRowToSelect.Value);
			}
		}

		/// <summary>
		/// Gets or sets the context menu when right clicking or pressing the menu button on the control.
		/// </summary>
		/// <value>The context menu.</value>
		public ContextMenu ContextMenu
		{
			get { return Handler.ContextMenu; }
			set { Handler.ContextMenu = value; }
		}

		static readonly object callback = new Callback();
		/// <summary>
		/// Gets an instance of an object used to perform callbacks to the widget from handler implementations
		/// </summary>
		/// <returns>The callback instance to use for this widget</returns>
		protected override object GetCallback() { return callback; }

		/// <summary>
		/// Callback interface for the <see cref="GridView"/>
		/// </summary>
		public new interface ICallback : Grid.ICallback
		{
			/// <summary>
			/// Raises the cell click event.
			/// </summary>
			void OnCellClick(GridView widget, GridViewCellEventArgs e);
		}

		/// <summary>
		/// Callback implementation for handlers of the <see cref="GridView"/>.
		/// </summary>
		protected new class Callback : Grid.Callback, ICallback
		{
			/// <summary>
			/// Raises the cell click event.
			/// </summary>
			public void OnCellClick(GridView widget, GridViewCellEventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnCellClick(e));
			}
		}

		/// <summary>
		/// Handler interface for the <see cref="GridView"/>.
		/// </summary>
		public new interface IHandler : Grid.IHandler, IContextMenuHost
		{
			/// <summary>
			/// Gets or sets the data store for the items to show in the grid.
			/// </summary>
			/// <value>The grid's data store.</value>
			IDataStore DataStore { get; set; }

			/// <summary>
			/// Gets or sets a value indicating whether to show a border around each cell.
			/// </summary>
			/// <value><c>true</c> to show a space between cells; otherwise, <c>false</c>.</value>
			bool ShowCellBorders { get; set; }
		}
	}
}

