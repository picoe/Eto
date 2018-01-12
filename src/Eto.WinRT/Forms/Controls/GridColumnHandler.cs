#if TODO_XAML
using swc = Windows.UI.Xaml.Controls;
using sw = Windows.UI.Xaml;
using Eto.Forms;

namespace Eto.WinRT.Forms.Controls
{
	public interface IGridHandler
	{
		sw.FrameworkElement SetupCell (IGridColumnHandler column, sw.FrameworkElement defaultContent);
		void FormatCell (IGridColumnHandler column, ICellHandler cell, sw.FrameworkElement element, swc.DataGridCell gridcell, object dataItem);
	}

	public interface IGridColumnHandler : IGridColumn
	{
		swc.DataGridColumn Control { get; }
	}


	public class GridColumnHandler : WidgetHandler<swc.DataGridColumn, GridColumn>, IGridColumnHandler, ICellContainerHandler
	{
		Cell dataCell;

		public IGridHandler GridHandler { get; set; }

		protected override void Initialize ()
		{
			base.Initialize ();
			DataCell = new TextBoxCell (Widget.Generator);
			Editable = false;
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

		swc.DataGridColumn IGridColumnHandler.Control
		{
			get { return Control; }
		}

	}
}
#endif