using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sw = System.Windows;
using swc = System.Windows.Controls;
using swm = System.Windows.Media;
using swi = System.Windows.Input;
using Eto.Forms;
using System.Collections;
using System.Collections.ObjectModel;
using Eto.Platform.Wpf.Forms.Menu;
using Eto.Drawing;
using Eto.Platform.Wpf.Drawing;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public abstract class GridHandler<C, W> : WpfControl<C, W>, IGrid, IGridHandler
		where C: swc.DataGrid
		where W: Grid
	{
		ContextMenu contextMenu;
		bool hasFocus;
		protected bool SkipSelectionChanged { get; set; }
		protected swc.DataGridColumn CurrentColumn { get; set; }

		public GridHandler ()
		{
			Control = (C)new swc.DataGrid {
				HeadersVisibility = swc.DataGridHeadersVisibility.Column,
				AutoGenerateColumns = false,
				CanUserAddRows = false,
				RowHeaderWidth = 0,
				SelectionMode = swc.DataGridSelectionMode.Single
			};
		}

		protected ColumnCollection Columns { get; private set; }

		protected abstract IGridItem GetItemAtRow (int row);

		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case Grid.ColumnHeaderClickEvent:
				Control.Sorting += (sender, e) => {
					var column = Widget.Columns.First (r => object.ReferenceEquals (r.ControlObject, e.Column));
					Widget.OnColumnHeaderClick(new GridColumnEventArgs(column));
					e.Handled = true;
				};
				break;
			case Grid.BeginCellEditEvent:
				Control.PreparingCellForEdit += (sender, e) => {
					var row = e.Row.GetIndex ();
					var item = GetItemAtRow (row);
					var gridColumn = Widget.Columns[e.Column.DisplayIndex];
					Widget.OnBeginCellEdit (new GridViewCellArgs (gridColumn, row, e.Column.DisplayIndex, item));
				};
				break;
			case Grid.EndCellEditEvent:
				Control.CellEditEnding += (sender, e) => {
					var row = e.Row.GetIndex ();
					var item = GetItemAtRow(row);
					var gridColumn = Widget.Columns[e.Column.DisplayIndex];
					Widget.OnEndCellEdit (new GridViewCellArgs (gridColumn, row, e.Column.DisplayIndex, item));
				};
				break;
			case Grid.SelectionChangedEvent:
				Control.SelectedCellsChanged += (sender, e) => {
					if (!SkipSelectionChanged)
						Widget.OnSelectionChanged (EventArgs.Empty);
				};
				break;
			case Grid.CellFormattingEvent:
				// handled by FormatCell method
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
		}

		public bool ShowHeader
		{
			get { return Control.HeadersVisibility.HasFlag(swc.DataGridHeadersVisibility.Column); }
			set
			{
				Control.HeadersVisibility = value ? swc.DataGridHeadersVisibility.Column : swc.DataGridHeadersVisibility.None;
			}
		}

		public bool AllowColumnReordering
		{
			get { return Control.CanUserReorderColumns; }
			set { Control.CanUserReorderColumns = value; }
		}

		public override void Initialize ()
		{
			base.Initialize ();
			Columns = new ColumnCollection { Handler = this };
			Columns.Register (Widget.Columns);
		}

		protected class ColumnCollection : EnumerableChangedHandler<GridColumn, GridColumnCollection>
		{
			public GridHandler<C,W> Handler { get; set; }

			public override void AddItem (GridColumn item)
			{
				var colhandler = (GridColumnHandler)item.Handler;
				colhandler.GridHandler = Handler;
				Handler.Control.Columns.Add (colhandler.Control);
			}

			public override void InsertItem (int index, GridColumn item)
			{
				var colhandler = (GridColumnHandler)item.Handler;
				colhandler.GridHandler = Handler;
				Handler.Control.Columns.Insert (index, colhandler.Control);
			}

			public override void RemoveItem (int index)
			{
				Handler.Control.Columns.RemoveAt (index);
			}

			public override void RemoveAllItems ()
			{
				Handler.Control.Columns.Clear ();
			}
		}

		public ContextMenu ContextMenu
		{
			get { return contextMenu; }
			set
			{
				contextMenu = value;
				if (contextMenu != null)
					Control.ContextMenu = ((ContextMenuHandler)contextMenu.Handler).Control;
				else
					Control.ContextMenu = null;
			}
		}


		public bool AllowMultipleSelection
		{
			get { return Control.SelectionMode == swc.DataGridSelectionMode.Extended; }
			set { Control.SelectionMode = value ? swc.DataGridSelectionMode.Extended : swc.DataGridSelectionMode.Single; }
		}

		public IEnumerable<int> SelectedRows
		{
			get
			{
				var list = Control.ItemsSource as IList;
				if (list != null) {
					foreach (var item in Control.SelectedItems.OfType<IGridItem> ()) {
						yield return list.IndexOf (item);
					}
				}
			}
		}

		public int RowHeight
		{
			get { return (int)Control.RowHeight; }
			set { Control.RowHeight = value; }
		}

		public void SelectAll ()
		{
			Control.SelectAll ();
		}

		public void SelectRow (int row)
		{
			var list = Control.ItemsSource as IList;
			if (list != null) {
				if (this.AllowMultipleSelection)
					Control.SelectedItems.Add (list[row]);
				else
				{
					SaveColumnFocus ();
					Control.SelectedIndex = row;
					RestoreColumnFocus ();
				}
			}
		}

		public void UnselectRow (int row)
		{
			var list = Control.ItemsSource as IList;
			if (list != null) {
				if (this.AllowMultipleSelection)
					Control.SelectedItems.Remove (list[row]);
				else
					Control.UnselectAll ();
			}
		}

		public void UnselectAll ()
		{
			Control.UnselectAll ();
		}

		public virtual System.Windows.FrameworkElement SetupCell (IGridColumnHandler column, System.Windows.FrameworkElement defaultContent)
		{
			return defaultContent;
		}

		class FormatEventArgs : GridCellFormatEventArgs
		{
			public sw.FrameworkElement Element { get; private set; }

			public swc.DataGridCell Cell { get; private set; }
			Font font;

			public FormatEventArgs (GridColumn column, swc.DataGridCell gridcell, object item, int row, sw.FrameworkElement element)
				: base(column, item, row)
			{
				this.Element = element;
				this.Cell = gridcell;
			}

			public override Eto.Drawing.Font Font
			{
				get { return font; }
				set
				{
					font = value;
					FontHandler.Apply (Cell, font);
				}
			}

			public override Eto.Drawing.Color BackgroundColor
			{
				get
				{
					var brush = Cell.Background as swm.SolidColorBrush;
					if (brush != null) return brush.Color.ToEto ();
					else return Colors.White;
				}
				set
				{
					Cell.Background = new swm.SolidColorBrush (value.ToWpf ());
				}
			}

			public override Eto.Drawing.Color ForegroundColor
			{
				get
				{
					var brush = Cell.Foreground as swm.SolidColorBrush;
					if (brush != null) return brush.Color.ToEto ();
					else return Colors.Black;
				}
				set
				{
					Cell.Foreground = new swm.SolidColorBrush (value.ToWpf ());
				}
			}
		}

		public override bool HasFocus
		{
			get
			{
				if (Widget.ParentWindow != null)
					return Control.HasFocus((sw.DependencyObject)Widget.ParentWindow.ControlObject);
				else
					return base.HasFocus;
			}
		}

		public override void Focus ()
		{
			SaveColumnFocus ();
			base.Focus ();
			RestoreColumnFocus ();
		}

		public override void Invalidate ()
		{
			SaveColumnFocus ();
			Control.Items.Refresh ();
			RestoreColumnFocus ();
			base.Invalidate ();
		}

		public override void Invalidate (Rectangle rect)
		{
			SaveColumnFocus ();
			Control.Items.Refresh ();
			RestoreColumnFocus ();
			base.Invalidate (rect);
		}

		public virtual void FormatCell (IGridColumnHandler column, ICellHandler cell, sw.FrameworkElement element, swc.DataGridCell gridcell, object dataItem)
		{
			if (IsEventHandled (Grid.CellFormattingEvent)) {
				var row = Control.Items.IndexOf (dataItem);
				Widget.OnCellFormatting (new FormatEventArgs (column.Widget as GridColumn, gridcell, dataItem, row, element));
			}
		}

		protected void SaveColumnFocus ()
		{
			CurrentColumn = Control.CurrentColumn;
		}

		protected void RestoreColumnFocus ()
		{
			Control.CurrentColumn = null;
			Control.CurrentCell = new swc.DataGridCellInfo (Control.SelectedItem, CurrentColumn ?? Control.CurrentColumn ?? Control.Columns[0]);
			CurrentColumn = null;
		}

		protected void SaveFocus ()
		{
			SaveColumnFocus ();
			hasFocus = this.HasFocus;
		}

		protected void RestoreFocus ()
		{
			if (hasFocus)
			{
				this.Focus ();
				RestoreColumnFocus ();
			}
		}

	}
}
