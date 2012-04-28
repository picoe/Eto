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

	public abstract class Grid : Control
	{
		IGrid handler;

		public GridColumnCollection Columns { get; private set; }

		#region Events

		public const string BeginCellEditEvent = "BaseGrid.BeginCellEditEvent";

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
			if (beginCellEdit != null)
				beginCellEdit (this, e);
		}

		public const string EndCellEditEvent = "BaseGrid.EndCellEditEvent";

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
			if (endCellEdit != null)
				endCellEdit (this, e);
		}

		public const string SelectionChangedEvent = "BaseGrid.SelectionChanged";

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
			if (selectionChanged != null)
				selectionChanged (this, e);
		}

		#endregion

		protected Grid (Generator generator, Type type, bool initialize)
			: base (generator, type, false)
		{
			handler = (IGrid)Handler;
			Columns = new GridColumnCollection ();
			if (initialize)
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

		public bool AllowMultipleSelection
		{
			get { return handler.AllowMultipleSelection; }
			set { handler.AllowMultipleSelection = value; }
		}

		public abstract IEnumerable<IGridItem> SelectedItems { get; }

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
