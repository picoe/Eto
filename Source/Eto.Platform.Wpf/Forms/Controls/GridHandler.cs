using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sw = System.Windows;
using swc = System.Windows.Controls;
using Eto.Forms;
using System.Collections;
using System.Collections.ObjectModel;
using Eto.Platform.Wpf.Forms.Menu;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public abstract class GridHandler<C, W> : WpfControl<C, W>, IGrid, IGridHandler
		where C: swc.DataGrid
		where W: Grid
	{
		ContextMenu contextMenu;

		public GridHandler ()
		{
			Control = (C)new swc.DataGrid {
				HeadersVisibility = swc.DataGridHeadersVisibility.Column,
				AutoGenerateColumns = false,
				CanUserAddRows = false,
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
					Widget.OnSelectionChanged (EventArgs.Empty);
				};
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

		public void SelectAll ()
		{
			Control.SelectAll ();
		}

		public void SelectRow (int row)
		{
			var list = Control.ItemsSource as IList;
			if (list != null)
				Control.SelectedItems.Add (list[row]);
		}

		public void UnselectRow (int row)
		{
			var list = Control.ItemsSource as IList;
			if (list != null)
				Control.SelectedItems.Remove (list[row]);
		}

		public void UnselectAll ()
		{
			Control.UnselectAll ();
		}

		public virtual System.Windows.FrameworkElement SetupCell (IGridColumnHandler column, System.Windows.FrameworkElement defaultContent)
		{
			return defaultContent;
		}
	}
}
