using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface IGrid : IControl
	{
		bool ShowHeader { get; set; }

		int RowHeight { get; set; }

		bool AllowColumnReordering { get; set; }

		bool AllowMultipleSelection { get; set; }

		IEnumerable<int> SelectedRows { get; }

		void SelectRow (int row);

		void UnselectRow (int row);

		void SelectAll ();

		void UnselectAll ();
	}
	
	public class GridColumnEventArgs : EventArgs
	{
		public GridColumn Column { get; private set; }
		
		public GridColumnEventArgs (GridColumn column)
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

		protected GridCellFormatEventArgs (GridColumn column, object item, int row)
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

		public const string BeginCellEditEvent = "Grid.BeginCellEditEvent";

		event EventHandler<GridViewCellArgs> _BeginCellEdit;

		public event EventHandler<GridViewCellArgs> BeginCellEdit {
			add {
				_BeginCellEdit += value;
				HandleEvent (BeginCellEditEvent);
			}
			remove { _BeginCellEdit -= value; }
		}

		public virtual void OnBeginCellEdit (GridViewCellArgs e)
		{
			if (_BeginCellEdit != null)
				_BeginCellEdit(this, this.ViewToModel(e));
		}

		public const string EndCellEditEvent = "Grid.EndCellEditEvent";

		event EventHandler<GridViewCellArgs> _EndCellEdit;

		public event EventHandler<GridViewCellArgs> EndCellEdit {
			add {
				_EndCellEdit += value;
				HandleEvent (EndCellEditEvent);
			}
			remove { _EndCellEdit -= value; }
		}

		public virtual void OnEndCellEdit (GridViewCellArgs e)
		{
			if (_EndCellEdit != null)
				_EndCellEdit (this, this.ViewToModel(e));
		}

		public const string SelectionChangedEvent = "Grid.SelectionChanged";

		event EventHandler<EventArgs> _SelectionChanged;

		public event EventHandler<EventArgs> SelectionChanged {
			add {
				_SelectionChanged += value;
				HandleEvent (SelectionChangedEvent);
			}
			remove { _SelectionChanged -= value; }
		}

		public virtual void OnSelectionChanged (EventArgs e)
		{
			if (_SelectionChanged != null)
				_SelectionChanged (this, e);
		}
		
		public const string ColumnHeaderClickEvent = "Grid.ColumnHeaderClickEvent";

		event EventHandler<GridColumnEventArgs> _ColumnHeaderClick;

		public event EventHandler<GridColumnEventArgs> ColumnHeaderClick {
			add {
				_ColumnHeaderClick += value;
				HandleEvent (ColumnHeaderClickEvent);
			}
			remove { _ColumnHeaderClick -= value; }
		}

		public virtual void OnColumnHeaderClick (GridColumnEventArgs e)
		{
			if (_ColumnHeaderClick != null)
				_ColumnHeaderClick (this, e);
		}

		public const string CellFormattingEvent = "Grid.CellFormattingEvent";

		event EventHandler<GridCellFormatEventArgs> _CellFormatting;

		public event EventHandler<GridCellFormatEventArgs> CellFormatting {
			add {
				_CellFormatting += value;
				HandleEvent (CellFormattingEvent);
			}
			remove { _CellFormatting -= value; }
		}

		public virtual void OnCellFormatting (GridCellFormatEventArgs e)
		{
			if (_CellFormatting != null)
				_CellFormatting (this, e);
		}

		protected virtual GridViewCellArgs ViewToModel(GridViewCellArgs e)
		{
			return e;
		}

		#endregion

		protected Grid (Generator generator, Type type, bool initialize = true)
			: base (generator, type, false)
		{
			Columns = new GridColumnCollection ();
			if (initialize)
				Initialize ();
		}

		public bool ShowHeader {
			get { return Handler.ShowHeader; }
			set { Handler.ShowHeader = value; }
		}

		public bool AllowColumnReordering {
			get { return Handler.AllowColumnReordering; }
			set { Handler.AllowColumnReordering = value; }
		}

		public bool AllowMultipleSelection {
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

		public virtual IEnumerable<int> SelectedRows {
			get { return Handler.SelectedRows; }
		}

		public int RowHeight
		{
			get { return Handler.RowHeight; }
			set { Handler.RowHeight = value; }
		}

		public virtual void SelectRow (int row)
		{
			Handler.SelectRow (row);
		}

		public virtual void SelectAll ()
		{
			Handler.SelectAll ();
		}

		public virtual void UnselectRow(int row)
		{
			Handler.UnselectRow (row);
		}

		public virtual void UnselectAll()
		{
			Handler.UnselectAll ();
		}
	}
}
