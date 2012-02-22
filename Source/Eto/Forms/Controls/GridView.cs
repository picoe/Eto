using System;
using Eto.Collections;
using System.Collections.ObjectModel;

namespace Eto.Forms
{
	public interface IGridStore : IDataStore
	{
		int Count { get; }

		IGridItem GetItem (int index);
	}

	public class GridItemCollection : Collection<IGridItem>, IGridStore
	{
		IGridItem IGridStore.GetItem (int index)
		{
			return this[index];
		}
	}

	public interface IGridView : IControl
	{
		bool ShowHeader { get; set; }

		bool AllowColumnReordering { get; set; }

		void InsertColumn (int index, GridColumn column);

		void RemoveColumn (int index, GridColumn column);

		void ClearColumns ();

		IGridStore DataStore { get; set; }
		
		ContextMenu ContextMenu { get; set; }
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


	public class GridView : Control
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

		#endregion


		public GridColumnCollection Columns { get; private set; }

		public GridView ()
			: this (Generator.Current)
		{
		}

		public GridView (Generator g)
			: base (g, typeof (IGridView), true)
		{
			handler = (IGridView)Handler;
			Columns = new GridColumnCollection { Handler = handler };
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
		
		public ContextMenu ContextMenu
		{
			get { return handler.ContextMenu; }
			set { handler.ContextMenu = value; }
		}
	}
}

