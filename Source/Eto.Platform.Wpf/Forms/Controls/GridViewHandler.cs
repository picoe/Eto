using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using Eto.Forms;
using System.Collections;
using System.Collections.ObjectModel;
using Eto.Platform.Wpf.Forms.Menu;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class GridViewHandler : WpfControl<swc.DataGrid, GridView>, IGridView
	{
		IGridStore store;
		ContextMenu contextMenu;

		public GridViewHandler ()
		{
			Control = new swc.DataGrid {
				HeadersVisibility = swc.DataGridHeadersVisibility.Column,
				AutoGenerateColumns = false,
				CanUserAddRows = false,
				SelectionMode = swc.DataGridSelectionMode.Single
			};
		}

		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case GridView.BeginCellEditEvent:
				Control.PreparingCellForEdit += (sender, e) => {
					var row = e.Row.GetIndex ();
					var item = store[row];
					var gridColumn = Widget.Columns[e.Column.DisplayIndex];
					Widget.OnBeginCellEdit (new GridViewCellArgs (gridColumn, row, e.Column.DisplayIndex, item));
				};
				break;
			case GridView.EndCellEditEvent:
				Control.CellEditEnding += (sender, e) => {
					var row = e.Row.GetIndex ();
					var item = store[row];
					var gridColumn = Widget.Columns[e.Column.DisplayIndex];
					Widget.OnEndCellEdit (new GridViewCellArgs (gridColumn, row, e.Column.DisplayIndex, item));
				};
				break;
			case GridView.SelectionChangedEvent:
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

		public void InsertColumn (int index, GridColumn column)
		{
			var colHandler = (GridColumnHandler)column.Handler;
			if (index >= 0 && Control.Columns.Count > 0)
				Control.Columns.Insert (index, colHandler.Control);
			else
				Control.Columns.Add (colHandler.Control);
			for (int i = index; i < Widget.Columns.Count; i++) {
				var childCol = (GridColumnHandler)Widget.Columns[i].Handler;
				childCol.Bind (i);
			}
		}

		public void RemoveColumn (int index, GridColumn column)
		{
			var colHandler = (GridColumnHandler)column.Handler;
			Control.Columns.Remove (colHandler.Control);
		}

		public void ClearColumns ()
		{
			Control.Columns.Clear ();
		}

		public IGridStore DataStore
		{
			get { return store; }
			set
			{
				store = value;
				// must use observable collection for editing
				if (store is ObservableCollection<IGridItem>)
					Control.ItemsSource = store as ObservableCollection<IGridItem>;
				else
					Control.ItemsSource = new ObservableCollection<IGridItem> (GridItemCollection.EnumerateDataStore (store));
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
	}
}
