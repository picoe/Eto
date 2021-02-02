using Eto.Wpf.Forms.Cells;
using swc = System.Windows.Controls;
using sw = System.Windows;
using swd = System.Windows.Data;
using Eto.Forms;
using System.Globalization;
using System;

namespace Eto.Wpf.Forms.Controls
{
	public interface IGridHandler
	{
		EtoDataGrid Control { get; }
		Grid Widget { get; }
		bool Loaded { get; }
		bool DisableAutoScrollToSelection { get; }
		sw.FrameworkElement SetupCell(IGridColumnHandler column, sw.FrameworkElement defaultContent, swc.DataGridCell cell);
		void FormatCell(IGridColumnHandler column, ICellHandler cell, sw.FrameworkElement element, swc.DataGridCell gridcell, object dataItem);
		void CellEdited(int row, swc.DataGridColumn dataGridColumn, object dataItem);
	}

	public interface IGridColumnHandler : GridColumn.IHandler
	{
		swc.DataGridColumn Control { get; }

		void OnLoad();

		void SetHeaderStyle();
	}


	public class GridColumnHandler : WidgetHandler<swc.DataGridColumn, GridColumn>, IGridColumnHandler, ICellContainerHandler
	{
		Cell dataCell;

		public IGridHandler GridHandler { get; private set; }

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
			set { Control.Header = value; }
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

		static readonly object AutoSize_Key = new object();
		static readonly object Expand_Key = new object();
		static readonly object Width_Key = new object();

		bool IsGridLoaded => GridHandler?.Loaded == true;

		public bool AutoSize
		{
			get
			{
				if (IsGridLoaded)
					return Expand ? Widget.Properties.Get<bool>(AutoSize_Key, true) : Control.Width.IsAuto;
				return Widget.Properties.Get<bool>(AutoSize_Key, true);
			}
			set
			{
				if (value && IsGridLoaded && !Expand)
				{
					Control.Width = new swc.DataGridLength(1, swc.DataGridLengthUnitType.Auto);
					Widget.Properties.Set(AutoSize_Key, value, true);
				}
				else if (Widget.Properties.TrySet(AutoSize_Key, value, true))
					SetWidth();
			}
		}

		public bool Expand
		{
			get => Widget.Properties.Get<bool>(Expand_Key);
			set
			{
				if (Widget.Properties.TrySet(Expand_Key, value))
					SetWidth();
			}
		}

		public int Width
		{
			get
			{
				if (IsGridLoaded)
					return (int)Control.ActualWidth;
				return Widget.Properties.Get(Width_Key, -1);
			}
			set
			{
				if (value == -1)
				{
					Widget.Properties.Set(Width_Key, value, -1);
					AutoSize = true;
					return;
				}

				Widget.Properties.Set(AutoSize_Key, false, true);
				if (IsGridLoaded)
				{
					Control.Width = new swc.DataGridLength(value);
					Widget.Properties.Set(Width_Key, value, -1);
				}
				else if (Widget.Properties.TrySet(Width_Key, value, -1))
					SetWidth();
			}
		}

		void SetWidth()
		{
			if (Expand)
			{
				if (AutoSize && !IsGridLoaded)
					Control.Width = new swc.DataGridLength(1.0, swc.DataGridLengthUnitType.Auto);
				else
					Control.Width = new swc.DataGridLength(1.0, swc.DataGridLengthUnitType.Star);
			}
			else if (AutoSize)
			{
				Control.Width = new swc.DataGridLength(1.0, swc.DataGridLengthUnitType.Auto);
			}
			else
			{
				Control.Width = Width;
			}
		}

		void CopyValues(Cell oldCell)
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
				CopyValues(oldCell);
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

		public void Setup(IGridHandler gridHandler)
		{
			GridHandler = gridHandler;
			SetHeaderStyle();
		}

		public sw.FrameworkElement SetupCell(ICellHandler handler, sw.FrameworkElement defaultContent, swc.DataGridCell cell)
		{
			return GridHandler != null ? GridHandler.SetupCell(this, defaultContent, cell) : defaultContent;
		}

		public void FormatCell(ICellHandler cell, sw.FrameworkElement element, swc.DataGridCell gridcell, object dataItem)
		{
			if (GridHandler != null)
				GridHandler.FormatCell(this, cell, element, gridcell, dataItem);
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

		void IGridColumnHandler.OnLoad()
		{
			if (Expand && AutoSize)
				SetWidth();
		}

		static readonly object HeaderTextAlignment_Key = new object();

		public TextAlignment HeaderTextAlignment
		{
			get => Widget.Properties.Get<TextAlignment>(HeaderTextAlignment_Key);
			set
			{
				if (Widget.Properties.TrySet(HeaderTextAlignment_Key, value))
					SetHeaderStyle();
			}
		}

		public virtual void SetHeaderStyle()
		{
			var alignment = HeaderTextAlignment;
			if (alignment == TextAlignment.Left)
			{
				Control.ClearValue(swc.DataGridColumn.HeaderStyleProperty);
			}
			else if (GridHandler != null)
			{
				var style = new sw.Style();
				style.BasedOn = GridHandler.Control.ColumnHeaderStyle;
				style.TargetType = typeof(swc.Primitives.DataGridColumnHeader);
				style.Setters.Add(new sw.Setter(swc.Primitives.DataGridColumnHeader.HorizontalContentAlignmentProperty, alignment.ToWpf()));
				Control.HeaderStyle = style;
			}
		}

		public int MinWidth { get => (int)Control.MinWidth; set => Control.MinWidth = value; }
		public int MaxWidth
		{
			get => double.IsInfinity(Control.MaxWidth) ? int.MaxValue : (int)Control.MaxWidth;
			set => Control.MaxWidth = value == int.MaxValue ? double.PositiveInfinity : value;
		}

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
