using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eto.Forms
{
	public interface IGrid : IControl
	{
		bool ShowHeader { get; set; }

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

	public abstract class Grid : Control
	{
		IGrid handler;

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
				_BeginCellEdit (this, e);
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
				_EndCellEdit (this, e);
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

		#endregion

		protected Grid (Generator generator, Type type, bool initialize = true)
			: base (generator, type, false)
		{
			handler = (IGrid)Handler;
			Columns = new GridColumnCollection ();
			if (initialize)
				Initialize ();
		}

		public bool ShowHeader {
			get { return handler.ShowHeader; }
			set { handler.ShowHeader = value; }
		}

		public bool AllowColumnReordering {
			get { return handler.AllowColumnReordering; }
			set { handler.AllowColumnReordering = value; }
		}

		public bool AllowMultipleSelection {
			get { return handler.AllowMultipleSelection; }
			set { handler.AllowMultipleSelection = value; }
		}

		public abstract IEnumerable<IGridItem> SelectedItems { get; }

		public IEnumerable<int> SelectedRows {
			get { return handler.SelectedRows; }
		}

		public void SelectRow (int row)
		{
			handler.SelectRow (row);
		}

		public void SelectAll ()
		{
			handler.SelectAll ();
		}

		public void UnselectRow (int row)
		{
			handler.UnselectRow (row);
		}

		public void UnselectAll ()
		{
			handler.UnselectAll ();
		}
	}
}
