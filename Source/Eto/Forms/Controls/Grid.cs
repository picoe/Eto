using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;

namespace Eto.Forms
{
	/// <summary>
	/// Handler interface for <see cref="Grid"/>
	/// </summary>
	public interface IGrid : IControl
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
		IEnumerable<int> SelectedRows { get; }

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
	}

	public class GridColumnEventArgs : EventArgs
	{
		public GridColumn Column { get; private set; }

		public GridColumnEventArgs(GridColumn column)
		{
			this.Column = column;
		}
	}

	public abstract class GridCellFormatEventArgs : EventArgs
	{
		public GridColumn Column { get; private set; }

		public object Item { get; private set; }

		public int Row { get; private set; }

		public abstract Font Font { get; set; }

		public abstract Color BackgroundColor { get; set; }

		public abstract Color ForegroundColor { get; set; }

		protected GridCellFormatEventArgs(GridColumn column, object item, int row)
		{
			this.Column = column;
			this.Item = item;
			this.Row = row;
		}
	}

	public abstract class Grid : Control
	{
		new IGrid Handler { get { return (IGrid)base.Handler; } }

		public GridColumnCollection Columns { get; private set; }

		#region Events

		public const string CellEditingEvent = "Grid.CellEditingEvent";

		public event EventHandler<GridViewCellArgs> CellEditing
		{
			add { Properties.AddHandlerEvent(CellEditingEvent, value); }
			remove { Properties.RemoveEvent(CellEditingEvent, value); }
		}

		public virtual void OnCellEditing(GridViewCellArgs e)
		{
			Properties.TriggerEvent(CellEditingEvent, this, e);
		}

		public const string CellEditedEvent = "Grid.CellEditedEvent";

		public event EventHandler<GridViewCellArgs> CellEdited
		{
			add { Properties.AddHandlerEvent(CellEditedEvent, value); }
			remove { Properties.RemoveEvent(CellEditedEvent, value); }
		}

		public virtual void OnCellEdited(GridViewCellArgs e)
		{
			Properties.TriggerEvent(CellEditedEvent, this, e);
		}

		public const string SelectionChangedEvent = "Grid.SelectionChanged";

		public event EventHandler<EventArgs> SelectionChanged
		{
			add { Properties.AddHandlerEvent(SelectionChangedEvent, value); }
			remove { Properties.RemoveEvent(SelectionChangedEvent, value); }
		}

		public virtual void OnSelectionChanged(EventArgs e)
		{
			Properties.TriggerEvent(SelectionChangedEvent, this, e);
		}

		public const string ColumnHeaderClickEvent = "Grid.ColumnHeaderClickEvent";

		public event EventHandler<GridColumnEventArgs> ColumnHeaderClick
		{
			add { Properties.AddHandlerEvent(ColumnHeaderClickEvent, value); }
			remove { Properties.RemoveEvent(ColumnHeaderClickEvent, value); }
		}

		public virtual void OnColumnHeaderClick(GridColumnEventArgs e)
		{
			Properties.TriggerEvent(ColumnHeaderClickEvent, this, e);
		}

		public const string CellFormattingEvent = "Grid.CellFormattingEvent";

		public event EventHandler<GridCellFormatEventArgs> CellFormatting
		{
			add { Properties.AddHandlerEvent(CellFormattingEvent, value); }
			remove { Properties.RemoveEvent(CellFormattingEvent, value); }
		}

		public virtual void OnCellFormatting(GridCellFormatEventArgs e)
		{
			Properties.TriggerEvent(CellFormattingEvent, this, e);
		}

		protected virtual GridViewCellArgs ViewToModel(GridViewCellArgs e)
		{
			return e;
		}

		#endregion

		static Grid()
		{
			EventLookup.Register(typeof(Grid), "OnCellEdited", Grid.CellEditedEvent);
			EventLookup.Register(typeof(Grid), "OnCellEditing", Grid.CellEditingEvent);
			EventLookup.Register(typeof(Grid), "OnCellFormatting", Grid.CellFormattingEvent);
			EventLookup.Register(typeof(Grid), "OnSelectionChanged", Grid.SelectionChangedEvent);
			EventLookup.Register(typeof(Grid), "OnColumnHeaderClick", Grid.ColumnHeaderClickEvent);
		}

		protected Grid(Generator generator, Type type, bool initialize = true)
			: base(generator, type, false)
		{
			Columns = new GridColumnCollection();
			if (initialize)
				Initialize();
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

		public abstract IEnumerable<object> SelectedItems { get; }

		/// <summary>
		/// If there is exactly one selected item, returns it, otherwise
		/// returns null.
		/// </summary>
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
		/// Gets the selected rows indexes
		/// </summary>
		/// <value>The selected rows.</value>
		public virtual IEnumerable<int> SelectedRows
		{
			get { return Handler.SelectedRows; }
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
	}
}
