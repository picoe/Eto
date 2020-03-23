using Eto.Wpf.Forms.Cells;
using swc = System.Windows.Controls;
using sw = System.Windows;
using Eto.Forms;

namespace Eto.Wpf.Forms.Controls
{
	public interface IGridHandler
	{
		Grid Widget { get; }
		bool Loaded { get; }
		bool DisableAutoScrollToSelection { get; }
		sw.FrameworkElement SetupCell (IGridColumnHandler column, sw.FrameworkElement defaultContent);
		void FormatCell (IGridColumnHandler column, ICellHandler cell, sw.FrameworkElement element, swc.DataGridCell gridcell, object dataItem);
		void CellEdited(int row, swc.DataGridColumn dataGridColumn, object dataItem);
	}

	public interface IGridColumnHandler : GridColumn.IHandler
	{
		swc.DataGridColumn Control { get; }
	}


	public class GridColumnHandler : WidgetHandler<swc.DataGridColumn, GridColumn>, IGridColumnHandler, ICellContainerHandler
	{
		Cell dataCell;

		public IGridHandler GridHandler { get; set; }

		protected override void Initialize()
		{
			base.Initialize();
			DataCell = new TextBoxCell();
			Editable = false;
			Sortable = false;
		}

		public string HeaderText
		{
			get { return Control.Header as string; }
			set { Control.Header = value;  }
		}

		public bool Resizable
		{
			get { return Control.CanUserResize; }
			set { Control.CanUserResize = value; }
		}

		public bool Sortable
		{
			get { return Control.CanUserSort; }
			set { Control.CanUserSort = value; }
		}

		public bool AutoSize
		{
			get { return Control.Width.IsAuto; }
			set { Control.Width = value ? new swc.DataGridLength(1, swc.DataGridLengthUnitType.Auto) : new swc.DataGridLength(100); }
		}

		public int Width
		{
			get
			{
				if (GridHandler?.Loaded == true)
					return (int)Control.ActualWidth;
				return (int)Control.Width.Value;
			}
			set
			{
				Control.Width = new swc.DataGridLength (value);
			}
		}

		void CopyValues (Cell oldCell)
		{
			if (oldCell == null) return;
			var handler = (ICellHandler)oldCell.Handler;
			var oldControl = handler.Control;
			Control.Header = oldControl.Header;
			Control.CanUserResize = oldControl.CanUserResize;
			Control.CanUserSort = oldControl.CanUserSort;
			Control.Width = oldControl.Width;
			Control.MinWidth = oldControl.MinWidth;
			Control.MaxWidth = oldControl.MaxWidth;
			Control.Visibility = oldControl.Visibility;
			Control.IsReadOnly = oldControl.IsReadOnly;
		}

		public Cell DataCell
		{
			get { return dataCell; }
			set
			{
				var oldCell = dataCell;
				dataCell = value;
				var cellHandler = (ICellHandler)dataCell.Handler;
				cellHandler.ContainerHandler = this;
				Control = cellHandler.Control;
				CopyValues (oldCell);
			}
		}

		public bool Editable
		{
			get { return !Control.IsReadOnly; }
			set { Control.IsReadOnly = !value; }
		}

		public bool Visible
		{
			get { return Control.Visibility == sw.Visibility.Visible; }
			set { Control.Visibility = (value) ? sw.Visibility.Visible : sw.Visibility.Hidden; }
		}

		public void Setup (IGridHandler gridHandler)
		{
			GridHandler = gridHandler;
		}

		public sw.FrameworkElement SetupCell (ICellHandler cell, sw.FrameworkElement defaultContent)
		{
			return GridHandler != null ? GridHandler.SetupCell(this, defaultContent) : defaultContent;
		}

		public void FormatCell (ICellHandler cell, sw.FrameworkElement element, swc.DataGridCell gridcell, object dataItem)
		{
			if (GridHandler != null)
				GridHandler.FormatCell (this, cell, element, gridcell, dataItem);
		}

		internal ICellHandler DataCellHandler => DataCell?.Handler as ICellHandler;

		internal void OnMouseDown(GridCellMouseEventArgs args, sw.DependencyObject hitTestResult, swc.DataGridCell cell)
		{
			DataCellHandler?.OnMouseDown(args, hitTestResult, cell);
		}

		internal void OnMouseUp(GridCellMouseEventArgs args, sw.DependencyObject hitTestResult, swc.DataGridCell cell)
		{
			DataCellHandler?.OnMouseUp(args, hitTestResult, cell);
		}

		swc.DataGridColumn IGridColumnHandler.Control => Control;

		public Grid Grid => GridHandler?.Widget;

		public void CellEdited(ICellHandler cell, sw.FrameworkElement element)
		{
			var dataCell = element.GetVisualParent<swc.DataGridCell>();
			var dataRow = element.GetVisualParent<swc.DataGridRow>();
			// These can sometimes be null, but I'm not exactly sure why
			// It could possibly be if another event occurs to refresh the data before this call?
			// either way, if we aren't part of a row/cell, just don't raise the event.
			if (dataRow == null || dataCell == null)
				return;
			var row = dataRow.GetIndex();
			var dataItem = element.DataContext;
			GridHandler.CellEdited(row, dataCell.Column, dataItem);
		}
	}
}
