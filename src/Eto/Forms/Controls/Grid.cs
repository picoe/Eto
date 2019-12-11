using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Eto.Drawing;

namespace Eto.Forms
{
	/// <summary>
	/// Event arguments for <see cref="Grid"/> events relating to a specific column
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class GridColumnEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the column that originated the event
		/// </summary>
		/// <value>The column.</value>
		public GridColumn Column { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.GridColumnEventArgs"/> class.
		/// </summary>
		/// <param name="column">Column that originated the event</param>
		public GridColumnEventArgs(GridColumn column)
		{
			this.Column = column;
		}
	}

	/// <summary>
	/// Event arguments for cell-based events of a <see cref="Grid"/> triggered by the mouse.
	/// </summary>
	public class GridCellMouseEventArgs : MouseEventArgs
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
		/// Initializes a new instance of the GridCellMouseEventArgs class.
		/// </summary>
		/// <param name="gridColumn">Grid column that triggered the event.</param>
		/// <param name="row">The row that triggered the event, or -1 if no row.</param>
		/// <param name="column">Column that triggered the event, or -1 if no column.</param>
		/// <param name="item">Item of the row that triggered the event, or null if no item.</param>
		/// <param name="buttons">Mouse buttons that are pressed during the event</param>
		/// <param name="modifiers">Key modifiers currently pressed</param>
		/// <param name="location">Location of the mouse cursor in the grid</param>
		/// <param name="delta">Delta of the scroll wheel.</param>
		/// <param name="pressure">Pressure of a stylus or touch, if applicable. 1.0f for full pressure or not supported</param>
		public GridCellMouseEventArgs(GridColumn gridColumn, int row, int column, object item, MouseButtons buttons, Keys modifiers, PointF location, SizeF? delta = null, float pressure = 1.0f)
			: base(buttons, modifiers, location, delta, pressure)
		{
			this.GridColumn = gridColumn;
			this.Row = row;
			this.Column = column;
			this.Item = item;
		}
	}

	/// <summary>
	/// Enumeration for the type of grid lines to show around each column/row in a <see cref="Grid"/>
	/// </summary>
	[Flags]
	public enum GridLines
	{
		/// <summary>
		/// No grid lines shown
		/// </summary>
		None = 0,
		/// <summary>
		/// A horizontal line is shown between each row
		/// </summary>
		Horizontal = 1 << 0,
		/// <summary>
		/// A vertical line is shown between each column
		/// </summary>
		Vertical = 1 << 1,
		/// <summary>
		/// Shows both vertical and horizontal lines between each column/row
		/// </summary>
		Both = Horizontal | Vertical
	}

	/// <summary>
	/// Event arguments to format a cell in a <see cref="Grid"/>
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public abstract class GridCellFormatEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the column to format
		/// </summary>
		/// <value>The column to format</value>
		public GridColumn Column { get; private set; }

		/// <summary>
		/// Gets the item that is associated with the row being formatted
		/// </summary>
		/// <value>The item.</value>
		public object Item { get; private set; }

		/// <summary>
		/// Gets the row number in the data source
		/// </summary>
		/// <value>The row.</value>
		public int Row { get; private set; }

		/// <summary>
		/// Gets or sets the font to use for the cell, or null to use the default font
		/// </summary>
		/// <value>The font.</value>
		public abstract Font Font { get; set; }

		/// <summary>
		/// Gets or sets the background color for the cell
		/// </summary>
		/// <value>The color of the background.</value>
		public abstract Color BackgroundColor { get; set; }

		/// <summary>
		/// Gets or sets the foreground color for the cell contents
		/// </summary>
		/// <value>The color of the foreground.</value>
		public abstract Color ForegroundColor { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.GridCellFormatEventArgs"/> class.
		/// </summary>
		/// <param name="column">Column to format</param>
		/// <param name="item">Item for the row being formatted</param>
		/// <param name="row">Row number being formatted</param>
		protected GridCellFormatEventArgs(GridColumn column, object item, int row)
		{
			Column = column;
			Item = item;
			Row = row;
		}
	}

	/// <summary>
	/// Base grid control to display items in columns and rows
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[ContentProperty("Columns")]
	public abstract class Grid : Control, ISelectable<object>
	{
		GridColumnCollection columns;
		internal bool supressSelectionChanged;

		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Gets the collection of columns to display in the grid
		/// </summary>
		/// <value>The column collection</value>
		public GridColumnCollection Columns { get { return columns ?? (columns = new GridColumnCollection()); } }

		#region Events

		/// <summary>
		/// Event identifier for handlers when attaching the <see cref="Grid.CellEditing"/> event
		/// </summary>
		public const string CellEditingEvent = "Grid.CellEditingEvent";

		/// <summary>
		/// Occurs before a cell is being edited to allow canceling based on application logic
		/// </summary>
		public event EventHandler<GridViewCellEventArgs> CellEditing
		{
			add { Properties.AddHandlerEvent(CellEditingEvent, value); }
			remove { Properties.RemoveEvent(CellEditingEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="CellEditing"/> event
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnCellEditing(GridViewCellEventArgs e)
		{
			Properties.TriggerEvent(CellEditingEvent, this, e);
		}

		/// <summary>
		/// Event identifier for handlers when attaching the <see cref="Grid.CellEdited"/> event
		/// </summary>
		public const string CellEditedEvent = "Grid.CellEditedEvent";

		/// <summary>
		/// Occurs after a cell has been edited
		/// </summary>
		public event EventHandler<GridViewCellEventArgs> CellEdited
		{
			add { Properties.AddHandlerEvent(CellEditedEvent, value); }
			remove { Properties.RemoveEvent(CellEditedEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="Grid.CellEdited"/> event
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnCellEdited(GridViewCellEventArgs e)
		{
			Properties.TriggerEvent(CellEditedEvent, this, e);
		}

		/// <summary>
		/// Event identifier for the <see cref="CellClick"/> event.
		/// </summary>
		public const string CellClickEvent = "Grid.CellClick";

		/// <summary>
		/// Occurs when an individual cell is clicked.
		/// </summary>
		public event EventHandler<GridCellMouseEventArgs> CellClick
		{
			add { Properties.AddHandlerEvent(CellClickEvent, value); }
			remove { Properties.RemoveEvent(CellClickEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="CellClick"/> event.
		/// </summary>
		/// <param name="e">Grid cell event arguments.</param>
		protected virtual void OnCellClick(GridCellMouseEventArgs e)
		{
			Properties.TriggerEvent(CellClickEvent, this, e);
		}

		/// <summary>
		/// Event identifier for the <see cref="CellDoubleClick"/> event.
		/// </summary>
		public const string CellDoubleClickEvent = "Grid.CellDoubleClick";

		/// <summary>
		/// Occurs when an individual cell is double clicked.
		/// </summary>
		public event EventHandler<GridCellMouseEventArgs> CellDoubleClick
		{
			add { Properties.AddHandlerEvent(CellDoubleClickEvent, value); }
			remove { Properties.RemoveEvent(CellDoubleClickEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="CellDoubleClick"/> event.
		/// </summary>
		/// <param name="e">Grid cell event arguments.</param>
		protected virtual void OnCellDoubleClick(GridCellMouseEventArgs e)
		{
			Properties.TriggerEvent(CellDoubleClickEvent, this, e);
		}

		/// <summary>
		/// Event identifier for handlers when attaching the <see cref="Grid.SelectionChanged"/> event
		/// </summary>
		public const string SelectionChangedEvent = "Grid.SelectionChanged";

		/// <summary>
		/// Occurs when the user has changed the selection in the grid
		/// </summary>
		public event EventHandler<EventArgs> SelectionChanged
		{
			add { Properties.AddHandlerEvent(SelectionChangedEvent, value); }
			remove { Properties.RemoveEvent(SelectionChangedEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="Grid.SelectionChanged"/> event
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected internal virtual void OnSelectionChanged(EventArgs e)
		{
			Properties.TriggerEvent(SelectionChangedEvent, this, e);
		}

		/// <summary>
		/// Event identifier for handlers when attaching the <see cref="Grid.ColumnHeaderClick"/> event
		/// </summary>
		public const string ColumnHeaderClickEvent = "Grid.ColumnHeaderClickEvent";

		/// <summary>
		/// Occurs when the column header has been clicked by the user
		/// </summary>
		public event EventHandler<GridColumnEventArgs> ColumnHeaderClick
		{
			add { Properties.AddHandlerEvent(ColumnHeaderClickEvent, value); }
			remove { Properties.RemoveEvent(ColumnHeaderClickEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="Grid.ColumnHeaderClick"/> event
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnColumnHeaderClick(GridColumnEventArgs e)
		{
			Properties.TriggerEvent(ColumnHeaderClickEvent, this, e);
		}

		/// <summary>
		/// Event identifier for handlers when attaching the <see cref="Grid.CellFormatting"/> event
		/// </summary>
		public const string CellFormattingEvent = "Grid.CellFormattingEvent";

		/// <summary>
		/// Occurs when each cell is being formatted for font and color
		/// </summary>
		public event EventHandler<GridCellFormatEventArgs> CellFormatting
		{
			add { Properties.AddHandlerEvent(CellFormattingEvent, value); }
			remove { Properties.RemoveEvent(CellFormattingEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="Grid.CellFormatting"/> event
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnCellFormatting(GridCellFormatEventArgs e)
		{
			Properties.TriggerEvent(CellFormattingEvent, this, e);
		}

		/// <summary>
		/// Occurs when the <see cref="SelectedItems"/> is changed.
		/// </summary>
		public event EventHandler<EventArgs> SelectedItemsChanged
		{
			add { SelectionChanged += value; }
			remove { SelectionChanged -= value; }
		}

		/// <summary>
		/// Occurs when the <see cref="SelectedRows"/> is changed.
		/// </summary>
		public event EventHandler<EventArgs> SelectedRowsChanged
		{
			add { SelectionChanged += value; }
			remove { SelectionChanged -= value; }
		}

		#endregion

		static Grid()
		{
			EventLookup.Register<Grid>(c => c.OnCellEdited(null), Grid.CellEditedEvent);
			EventLookup.Register<Grid>(c => c.OnCellEditing(null), Grid.CellEditingEvent);
			EventLookup.Register<Grid>(c => c.OnCellFormatting(null), Grid.CellFormattingEvent);
			EventLookup.Register<Grid>(c => c.OnCellClick(null), Grid.CellClickEvent);
			EventLookup.Register<Grid>(c => c.OnCellDoubleClick(null), Grid.CellDoubleClickEvent);
			EventLookup.Register<Grid>(c => c.OnSelectionChanged(null), Grid.SelectionChangedEvent);
			EventLookup.Register<Grid>(c => c.OnColumnHeaderClick(null), Grid.ColumnHeaderClickEvent);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Grid"/> class.
		/// </summary>
		protected Grid()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Grid"/> class with the specified handler
		/// </summary>
		/// <param name="handler">Handler implementation for the control</param>
		protected Grid(IHandler handler)
			: base(handler)
		{
		}

		/// <summary>
		/// Gets or sets a value indicating that the header should be shown
		/// </summary>
		/// <value><c>true</c> to show header; otherwise, <c>false</c>.</value>
		public bool ShowHeader
		{
			get { return Handler.ShowHeader; }
			set { Handler.ShowHeader = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the user can re-order columns
		/// </summary>
		/// <value><c>true</c> to allow column reordering; otherwise, <c>false</c>.</value>
		public bool AllowColumnReordering
		{
			get { return Handler.AllowColumnReordering; }
			set { Handler.AllowColumnReordering = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the user can select multiple rows
		/// </summary>
		/// <value><c>true</c> to allow multiple row selection; otherwise, <c>false</c>.</value>
		public bool AllowMultipleSelection
		{
			get { return Handler.AllowMultipleSelection; }
			set { Handler.AllowMultipleSelection = value; }
		}

		/// <summary>
		/// Gets an enumeration of the currently selected items
		/// </summary>
		/// <value>The selected items.</value>
		public abstract IEnumerable<object> SelectedItems { get; }

		/// <summary>
		/// If there is exactly one selected item, returns it, otherwise
		/// returns null.
		/// </summary>
		/// <remarks>
		/// Typically, you would use <see cref="SelectedItems"/> when <see cref="AllowMultipleSelection"/> is <c>true</c>.
		/// </remarks>
		/// <seealso cref="SelectedItems"/>
		public object SelectedItem
		{
			get
			{
				var selectedItems = SelectedItems;
				if (selectedItems != null && selectedItems.Count() == 1)
					return SelectedItems.FirstOrDefault();
				return null;
			}
		}

		/// <summary>
		/// Gets a binding object to bind to the <see cref="SelectedItem"/> property.
		/// </summary>
		/// <value>The selected item binding.</value>
		public BindableBinding<Grid, object> SelectedItemBinding
		{
			get
			{
				return new BindableBinding<Grid, object>(this, 
					g => g.SelectedItem,
					null,
					(g, eh) => g.SelectionChanged += eh,
					(g, eh) => g.SelectionChanged -= eh
				);
			}
		}

		/// <summary>
		/// Gets or sets the selected rows indexes
		/// </summary>
		/// <value>The selected rows.</value>
		public virtual IEnumerable<int> SelectedRows
		{
			get { return Handler.SelectedRows; }
			set { Handler.SelectedRows = value; }
		}

		/// <summary>
		/// Gets or sets the selected row, or -1 for none.
		/// </summary>
		/// <remarks>
		/// When <see cref="AllowMultipleSelection"/> is true and you want all selected rows, use <see cref="SelectedRow"/>.
		/// </remarks>
		/// <value>The selected row.</value>
		public int SelectedRow
		{
			get
			{
				var selected = SelectedRows;
				if (!selected.Any())
					return -1;
				return selected.First();
			}
			set
			{
				if (value < 0)
					SelectedRows = Enumerable.Empty<int>();
				else
					SelectedRows = new[] { value };
			}
		}

		/// <summary>
		/// Gets or sets the height for each row in the grid
		/// </summary>
		/// <value>The height of the row.</value>
		public int RowHeight
		{
			get { return Handler.RowHeight; }
			set { Handler.RowHeight = value; }
		}

		/// <summary>
		/// Gets or sets the style of grid lines to show between columns and rows
		/// </summary>
		/// <value>The grid line style.</value>
		public GridLines GridLines
		{
			get { return Handler.GridLines; }
			set { Handler.GridLines = value; }
		}

		/// <summary>
		/// Gets or sets the border type
		/// </summary>
		/// <value>The border.</value>
		public BorderType Border
		{
			get { return Handler.Border; }
			set { Handler.Border = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating that the user can clear the selection.
		/// </summary>
		/// <remarks>
		/// When true, the user can deselect the item by cmd/ctrl+click the last selected row, or
		/// by clicking on the empty space in the Grid. Some platforms may have empty space to the
		/// right, and some only have space at the bottom.
		/// When false, this setting will make it so the user cannot deselect the last selected item, and
		/// the control will initially select the first item when setting the DataStore property.
		/// This does not affect the ability to clear the selection programmatically.
		/// </remarks>
		[DefaultValue(true)]
		public bool AllowEmptySelection
		{
			get => Handler.AllowEmptySelection;
			set => Handler.AllowEmptySelection = value;
		}

		/// <summary>
		/// Selects the row to the specified <paramref name="row"/>, clearing other selections
		/// </summary>
		/// <param name="row">Row to select</param>
		public virtual void SelectRow(int row)
		{
			Handler.SelectRow(row);
		}

		/// <summary>
		/// Selects all rows
		/// </summary>
		public virtual void SelectAll()
		{
			Handler.SelectAll();
		}

		/// <summary>
		/// Unselects the specified <paramref name="row"/>
		/// </summary>
		/// <param name="row">Row to unselect</param>
		public virtual void UnselectRow(int row)
		{
			Handler.UnselectRow(row);
		}

		/// <summary>
		/// Clears the selection
		/// </summary>
		public virtual void UnselectAll()
		{
			Handler.UnselectAll();
		}

		/// <summary>
		/// Begin to edit one cell
		/// </summary>
		/// <param name="row">Row to edit</param>
		/// <param name="column">Column to edit</param>
		public void BeginEdit(int row, int column)
		{
			Handler.BeginEdit(row, column);
		}

		/// <summary>
		/// Commits a current edit operation and sets the current value to the model.
		/// </summary>
		/// <returns><c>true</c>, if edit was commited or if there was no current edit operation, <c>false</c> if the commit was cancelled..</returns>
		public bool CommitEdit() => Handler.CommitEdit();

		/// <summary>
		/// Cancels the current edit operation and reverts the cell value to the value in the model.
		/// </summary>
		/// <returns><c>true</c>, if edit was canceled or there was no current edit operation, <c>false</c> if the cancel was aborted.</returns>
		public bool CancelEdit() => Handler.CancelEdit();

		/// <summary>
		/// Gets a value indicating that the current cell is in edit mode. 
		/// </summary>
		/// <value><c>true</c> if the current cell is in edit mode; otherwise, <c>false</c>.</value>
		public bool IsEditing => Handler.IsEditing;

		/// <summary>
		/// Scrolls to show the specified row in the view
		/// </summary>
		/// <param name="row">Row to scroll to.</param>
		public void ScrollToRow(int row)
		{
			Handler.ScrollToRow(row);
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
		/// Callback interface for instances of <see cref="Grid"/>
		/// </summary>
		public new interface ICallback : Control.ICallback
		{
			/// <summary>
			/// Raises the cell editing event.
			/// </summary>
			void OnCellEditing(Grid widget, GridViewCellEventArgs e);

			/// <summary>
			/// Raises the cell edited event.
			/// </summary>
			void OnCellEdited(Grid widget, GridViewCellEventArgs e);

			/// <summary>
			/// Raises the cell click event.
			/// </summary>
			void OnCellClick(Grid widget, GridCellMouseEventArgs e);

			/// <summary>
			/// Raises the cell double click event.
			/// </summary>
			void OnCellDoubleClick(Grid widget, GridCellMouseEventArgs e);

			/// <summary>
			/// Raises the selection changed event.
			/// </summary>
			void OnSelectionChanged(Grid widget, EventArgs e);

			/// <summary>
			/// Raises the column header click event.
			/// </summary>
			void OnColumnHeaderClick(Grid widget, GridColumnEventArgs e);

			/// <summary>
			/// Raises the cell formatting event.
			/// </summary>
			void OnCellFormatting(Grid widget, GridCellFormatEventArgs e);
		}

		/// <summary>
		/// Callbacks for instances of <see cref="Grid"/>
		/// </summary>
		protected new class Callback : Control.Callback, ICallback
		{
			/// <summary>
			/// Raises the cell editing event.
			/// </summary>
			public void OnCellEditing(Grid widget, GridViewCellEventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnCellEditing(e);
			}

			/// <summary>
			/// Raises the cell edited event.
			/// </summary>
			public void OnCellEdited(Grid widget, GridViewCellEventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnCellEdited(e);
			}

			/// <summary>
			/// Raises the cell click event.
			/// </summary>
			public void OnCellClick(Grid widget, GridCellMouseEventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnCellClick(e);
			}

			/// <summary>
			/// Raises the cell double click event.
			/// </summary>
			public void OnCellDoubleClick(Grid widget, GridCellMouseEventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnCellDoubleClick(e);
			}

			/// <summary>
			/// Raises the selection changed event.
			/// </summary>
			public void OnSelectionChanged(Grid widget, EventArgs e)
			{
				using (widget.Platform.Context)
				{
					if (!widget.supressSelectionChanged)
						widget.OnSelectionChanged(e);
				}
			}

			/// <summary>
			/// Raises the column header click event.
			/// </summary>
			public void OnColumnHeaderClick(Grid widget, GridColumnEventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnColumnHeaderClick(e);
			}

			/// <summary>
			/// Raises the cell formatting event.
			/// </summary>
			public void OnCellFormatting(Grid widget, GridCellFormatEventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnCellFormatting(e);
			}
		}

		#region Handler

		/// <summary>
		/// Handler interface for the <see cref="Grid"/> control
		/// </summary>
		/// <copyright>(c) 2014 by Curtis Wensley</copyright>
		/// <license type="BSD-3">See LICENSE for full terms</license>
		public new interface IHandler : Control.IHandler
		{
			/// <summary>
			/// Gets or sets a value indicating that the header should be shown
			/// </summary>
			/// <value><c>true</c> to show header; otherwise, <c>false</c>.</value>
			bool ShowHeader { get; set; }

			/// <summary>
			/// Gets or sets the height for each row in the grid
			/// </summary>
			/// <value>The height of the row.</value>
			int RowHeight { get; set; }

			/// <summary>
			/// Gets or sets a value indicating whether the user can re-order columns
			/// </summary>
			/// <value><c>true</c> to allow column reordering; otherwise, <c>false</c>.</value>
			bool AllowColumnReordering { get; set; }

			/// <summary>
			/// Gets or sets a value indicating whether the user can select multiple rows
			/// </summary>
			/// <value><c>true</c> to allow multiple row selection; otherwise, <c>false</c>.</value>
			bool AllowMultipleSelection { get; set; }

			/// <summary>
			/// Gets the selected rows indexes
			/// </summary>
			/// <value>The selected rows.</value>
			IEnumerable<int> SelectedRows { get; set; }

			/// <summary>
			/// Gets or sets the style of grid lines to show between columns and rows
			/// </summary>
			/// <value>The grid line style.</value>
			GridLines GridLines { get; set; }

			/// <summary>
			/// Gets or sets the border type
			/// </summary>
			/// <value>The border.</value>
			BorderType Border { get; set; }

			/// <summary>
			/// Selects the row to the specified <paramref name="row"/>, clearing other selections
			/// </summary>
			/// <param name="row">Row to select</param>
			void SelectRow(int row);

			/// <summary>
			/// Unselects the specified <paramref name="row"/>
			/// </summary>
			/// <param name="row">Row to unselect</param>
			void UnselectRow(int row);

			/// <summary>
			/// Selects all rows
			/// </summary>
			void SelectAll();

			/// <summary>
			/// Clears the selection
			/// </summary>
			void UnselectAll();

			/// <summary>
			/// Begin to edit one cell
			/// </summary>
			/// <param name="row">Row to edit</param>
			/// <param name="column">Column to edit</param>
			void BeginEdit(int row, int column);

			/// <summary>
			/// Commits a current edit operation and sets the current value to the model.
			/// </summary>
			/// <returns><c>true</c>, if edit was commited or there was no current edit operation, <c>false</c> if the commit was cancelled..</returns>
			bool CommitEdit();

			/// <summary>
			/// Cancels the current edit operation and reverts the cell value to the value in the model.
			/// </summary>
			/// <returns><c>true</c>, if edit was canceled or there was no current edit operation, <c>false</c> if the cancel was aborted.</returns>
			bool CancelEdit();

			/// <summary>
			/// Gets a value indicating that the current cell is in edit mode. 
			/// </summary>
			/// <value><c>true</c> if the current cell is in edit mode; otherwise, <c>false</c>.</value>
			bool IsEditing { get; }

			/// <summary>
			/// Gets or sets a value indicating that the user can clear the selection.
			/// </summary>
			/// <remarks>
			/// When true, the user can deselect the item by cmd/ctrl+click the last selected row, or
			/// by clicking on the empty space in the Grid. Some platforms may have empty space to the
			/// right, and some only have space at the bottom.
			/// When false, this setting will make it so the user cannot deselect the last selected item, and
			/// the control will initially select the first item when setting the DataStore property.
			/// This does not affect the ability to clear the selection programmatically.
			/// </remarks>
			bool AllowEmptySelection { get; set; }

			/// <summary>
			/// Scrolls to show the specified row in the view
			/// </summary>
			/// <param name="row">Row to scroll to.</param>
			void ScrollToRow(int row);
		}

		#endregion
	}
}
