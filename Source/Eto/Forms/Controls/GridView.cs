using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

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
	/// Grid view with a data store of a specific type
	/// </summary>
	/// <typeparam name="T">Type of the objects in the grid view's data store</typeparam> 
	public class GridView<T> : GridView, ISelectableControl<T>
		where T: class
	{
		/// <summary>
		/// The data store for the grid.
		/// </summary>
		/// <remarks>
		/// This defines what data to show in the grid. If the source implements <see cref="System.Collections.Specialized.INotifyCollectionChanged"/>, such
		/// as an <see cref="System.Collections.ObjectModel.ObservableCollection{T}"/>, then changes to the collection will be reflected in the grid.
		/// </remarks>
		/// <value>The data store for the grid.</value>
		public new IEnumerable<T> DataStore { get { return (IEnumerable<T>)base.DataStore; } set { base.DataStore = value; } }

		/// <summary>
		/// Gets an enumeration of the currently selected items
		/// </summary>
		/// <value>The selected items.</value>
		public new IEnumerable<T> SelectedItems { get { return base.SelectedItems.Cast<T>(); } }
	}

	/// <summary>
	/// Control to present data in a grid in columns and rows.
	/// </summary>
	/// <see cref="TreeGridView"/>
	[Handler(typeof(GridView.IHandler))]
	public class GridView : Grid, ISelectableControl<object>
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

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
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.GridView"/> class with the specified handler.
		/// </summary>
		/// <param name="handler">Platform handler to use for the implementation of this GridView instance.</param>
		protected GridView(IHandler handler)
			: base(handler)
		{
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
		}

		/// <summary>
		/// The data store for the grid.
		/// </summary>
		/// <remarks>
		/// This defines what data to show in the grid. If the source implements <see cref="System.Collections.Specialized.INotifyCollectionChanged"/>, such
		/// as an <see cref="System.Collections.ObjectModel.ObservableCollection{T}"/>, then changes to the collection will be reflected in the grid.
		/// </remarks>
		/// <value>The data store for the grid.</value>
		public IEnumerable<object> DataStore
		{
			get { return Handler.DataStore; }
			set { Handler.DataStore = value; }
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

		class SelectionPreserverHelper : ISelectionPreserver
		{
			readonly int selectedCount;
			HashSet<object> selected;
			readonly GridView grid;

			public SelectionPreserverHelper(GridView grid)
			{
				this.grid = grid;
				selected = new HashSet<object>(grid.SelectedItems);
				selectedCount = selected.Count;
				grid.supressSelectionChanged = true;
			}

			public IEnumerable<object> SelectedItems
			{
				get { return selected; }
				set { selected = new HashSet<object>(value); }
			}

			public void Dispose()
			{
				if (selected.Count > 0)
				{
					var finalSelected = new List<int>();
					var dataStore = grid.DataStore;
					if (dataStore != null)
					{
						// go through list to find indexes of previously selected items
						int row = 0;
						foreach (var item in dataStore)
						{
							if (selected.Contains(item))
								finalSelected.Add(row);
							row++;
						}
					}
					grid.SelectedRows = finalSelected;
					if (finalSelected.Count != selectedCount)
						grid.OnSelectionChanged(EventArgs.Empty);
				}
				grid.supressSelectionChanged = false;
			}
		}

		/// <summary>
		/// Gets a new selection preserver instance for the grid.
		/// </summary>
		/// <remarks>
		/// This is used to keep the selected items consistent for a grid when changing the <see cref="DataStore"/>
		/// collection dramatically, such as filtering or sorting the collection.  Events such as removing or adding rows
		/// will always keep the selection of existing rows.
		/// </remarks>
		/// <value>A new instance of the selection preserver.</value>
		public ISelectionPreserver SelectionPreserver
		{
			get { return new SelectionPreserverHelper(this); }
		}

		/// <summary>
		/// Gets or sets the comparer to sort the data through code. Obsolete. Use <see cref="FilterCollection{T}.Sort"/> instead.
		/// </summary>
		/// <remarks>
		/// This is used to sort data programatically. If you have data coming from a database, it is usually more
		/// efficient to sort the data on the server. 
		/// </remarks>
		/// <value>The sort comparer.</value>
		[Obsolete("Use FilterCollection.SortComparer instead")]
		public Comparison<object> SortComparer
		{
			get
			{ 
				var filter = DataStore as IFilterableSource<object>;
				return filter != null ? filter.Sort : null;
			}
			set
			{
				var filter = DataStore as IFilterable<object> ?? new FilterCollection<object>(DataStore);
				filter.Sort = value;
			}
		}

		/// <summary>
		/// Gets or sets the filter of the data. Obsolete. Use <see cref="FilterCollection{T}.Filter"/> instead.
		/// </summary>
		/// <remarks>
		/// This is used to filter the data programatically.  If you have data coming from a database, it is usually
		/// more efficient to filter from the server.
		/// </remarks>
		/// <value>The data filter.</value>
		[Obsolete("Use FilterCollection.Filter instead")]
		public Func<object, bool> Filter
		{
			get
			{ 
				var filter = DataStore as IFilterableSource<object>;
				return filter != null ? filter.Filter : null;
			}
			set
			{
				var filter = DataStore as IFilterable<object> ?? new FilterCollection<object>(DataStore);
				filter.Filter = value;
			}
		}

		/// <summary>
		/// Gets an enumeration of the currently selected items
		/// </summary>
		/// <value>The selected items.</value>
		public override IEnumerable<object> SelectedItems
		{
			get { return Handler.SelectedItems; }
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
		protected override object GetCallback()
		{
			return callback;
		}

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
			IEnumerable<object> DataStore { get; set; }

			/// <summary>
			/// Gets or sets a value indicating whether to show a border around each cell.
			/// </summary>
			/// <value><c>true</c> to show a space between cells; otherwise, <c>false</c>.</value>
			bool ShowCellBorders { get; set; }

			/// <summary>
			/// Gets an enumeration of the currently selected items
			/// </summary>
			/// <value>The selected items.</value>
			IEnumerable<object> SelectedItems { get; }
		}
	}
}

