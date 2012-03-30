using System;
using Eto.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Eto.Forms
{
	public interface IGridStore : IDataStore<IGridItem>
	{
	}

	public class GridItemCollection : DataStoreCollection<IGridItem>, IGridStore
	{
	}

	public partial interface IGridView : IControl
	{
		bool ShowHeader { get; set; }

		bool AllowColumnReordering { get; set; }

		void InsertColumn (int index, GridColumn column);

		void RemoveColumn (int index, GridColumn column);

		void ClearColumns ();

		IGridStore DataStore { get; set; }

		bool AllowMultipleSelection { get; set; }

		IEnumerable<int> SelectedRows { get; }

		void SelectRow (int row);

		void UnselectRow (int row);

		void SelectAll ();

		void UnselectAll ();
	}

	public class GridViewCellArgs : EventArgs
	{
		public GridColumn GridColumn { get; private set; }

		public int Row { get; private set; }

		public int Column { get; private set; }

		public IGridItem Item { get; private set; }

		public GridViewCellArgs (GridColumn gridColumn, int row, int column, IGridItem item)
		{
			this.GridColumn = gridColumn;
			this.Row = row;
			this.Column = column;
			this.Item = item;
		}
	}


	public partial class GridView : Control
	{
		IGridView handler;

		#region Events

		public const string BeginCellEditEvent = "GridView.BeginCellEditEvent";

		event EventHandler<GridViewCellArgs> beginCellEdit;

		public event EventHandler<GridViewCellArgs> BeginCellEdit
		{
			add
			{
				beginCellEdit += value;
				HandleEvent (BeginCellEditEvent);
			}
			remove { beginCellEdit -= value; }
		}
		public virtual void OnBeginCellEdit (GridViewCellArgs e)
		{
			if (beginCellEdit != null) beginCellEdit (this, e);
		}

		public const string EndCellEditEvent = "GridView.EndCellEditEvent";

		event EventHandler<GridViewCellArgs> endCellEdit;

		public event EventHandler<GridViewCellArgs> EndCellEdit
		{
			add
			{
				endCellEdit += value;
				HandleEvent (EndCellEditEvent);
			}
			remove { endCellEdit -= value; }
		}

		public virtual void OnEndCellEdit (GridViewCellArgs e)
		{
			if (endCellEdit != null) endCellEdit (this, e);
		}

		public const string SelectionChangedEvent = "GridView.SelectionChanged";

		event EventHandler<EventArgs> selectionChanged;

		public event EventHandler<EventArgs> SelectionChanged
		{
			add
			{
				selectionChanged += value;
				HandleEvent (SelectionChangedEvent);
			}
			remove { selectionChanged -= value; }
		}

		public virtual void OnSelectionChanged (EventArgs e)
		{
			if (selectionChanged != null) selectionChanged (this, e);
		}

		#endregion


		public GridColumnCollection Columns { get; private set; }

		public GridView ()
			: this (Generator.Current)
		{
		}

		public GridView (Generator g)
			: base (g, typeof (IGridView), false)
		{
			handler = (IGridView)Handler;
			Columns = new GridColumnCollection { Handler = handler };
			Initialize ();
		}

		public bool ShowHeader
		{
			get { return handler.ShowHeader; }
			set { handler.ShowHeader = value; }
		}

		public bool AllowColumnReordering
		{
			get { return handler.AllowColumnReordering; }
			set { handler.AllowColumnReordering = value; }
		}

		public IGridStore DataStore
		{
			get { return handler.DataStore; }
			set { handler.DataStore = value; }
		}

		public bool AllowMultipleSelection
		{
			get { return handler.AllowMultipleSelection; }
			set { handler.AllowMultipleSelection = value; }
		}

		public IEnumerable<IGridItem> SelectedItems
		{
			get
			{
				if (DataStore == null) yield break;
				foreach (var row in SelectedRows) {
					yield return DataStore[row];
				}
			}
		}

		public IEnumerable<int> SelectedRows
		{
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

