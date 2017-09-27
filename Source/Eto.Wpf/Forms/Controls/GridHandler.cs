using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Wpf.Forms.Cells;
using sw = System.Windows;
using swc = System.Windows.Controls;
using swm = System.Windows.Media;
using swi = System.Windows.Input;
using Eto.Forms;
using System.Collections;
using Eto.Wpf.Forms.Menu;
using Eto.Drawing;
using Eto.Wpf.Drawing;

namespace Eto.Wpf.Forms.Controls
{
	public class EtoDataGrid : swc.DataGrid
	{
		public IWpfFrameworkElement Handler { get; set; }

		public new void BeginUpdateSelectedItems()
		{
			base.BeginUpdateSelectedItems();
		}

		public new void EndUpdateSelectedItems()
		{
			base.EndUpdateSelectedItems();
		}

		protected override sw.Size MeasureOverride(sw.Size constraint)
		{
			return Handler?.MeasureOverride(constraint, base.MeasureOverride) ?? base.MeasureOverride(constraint);
		}
	}


	public abstract class GridHandler<TWidget, TCallback> : WpfControl<EtoDataGrid, TWidget, TCallback>, Grid.IHandler, IGridHandler
		where TWidget : Grid
		where TCallback : Grid.ICallback
	{
		ContextMenu contextMenu;
		bool hasFocus;
		protected bool SkipSelectionChanged { get; set; }
		protected swc.DataGridColumn CurrentColumn { get; set; }

		protected override sw.Size DefaultSize => new sw.Size(100, 100);
		public override bool UseMousePreview => true;
		public override bool UseKeyPreview => true;

		protected GridHandler()
		{
			Control = new EtoDataGrid
			{
				Handler = this,
				HeadersVisibility = swc.DataGridHeadersVisibility.Column,
				AutoGenerateColumns = false,
				CanUserDeleteRows = false,
				CanUserResizeRows = false,
				CanUserAddRows = false,
				RowHeaderWidth = 0,
				SelectionMode = swc.DataGridSelectionMode.Single,
				GridLinesVisibility = swc.DataGridGridLinesVisibility.None,
				Background = sw.SystemColors.WindowBrush
			};
		}

		protected ColumnCollection Columns { get; private set; }

		protected abstract object GetItemAtRow(int row);

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Grid.ColumnHeaderClickEvent:
					Control.Sorting += (sender, e) =>
					{
						var column = Widget.Columns.First(r => object.ReferenceEquals(r.ControlObject, e.Column));
						Callback.OnColumnHeaderClick(Widget, new GridColumnEventArgs(column));
						e.Handled = true;
					};
					break;
				case Grid.CellEditingEvent:
					Control.PreparingCellForEdit += (sender, e) =>
					{
						var row = e.Row.GetIndex();
						var item = GetItemAtRow(row);
						var gridColumn = Widget.Columns[e.Column.DisplayIndex];
						Callback.OnCellEditing(Widget, new GridViewCellEventArgs(gridColumn, row, e.Column.DisplayIndex, item));
					};
					break;
				case Grid.CellEditedEvent:
					// handled by each cell after value is set with the CellEdited method
					break;
				case Grid.CellClickEvent:
					Control.PreviewMouseLeftButtonDown += (sender, e) =>
					{
						var dep = e.OriginalSource as sw.DependencyObject;
						while (dep != null && !(dep is swc.DataGridCell))
							dep = swm.VisualTreeHelper.GetParent(dep);

						var cell = dep as swc.DataGridCell;
						while (dep != null && !(dep is swc.DataGridRow))
							dep = swm.VisualTreeHelper.GetParent(dep);

						var row = dep as swc.DataGridRow;

						int rowIndex;
						if (row != null && (rowIndex = row.GetIndex()) >= 0)
						{
							var columnIndex = cell.Column == null ? -1 : cell.Column.DisplayIndex;

							var item = Control.Items[rowIndex];
							var column = columnIndex == -1 || columnIndex >= Widget.Columns.Count ? null : Widget.Columns[columnIndex];
							Callback.OnCellClick(Widget, new GridViewCellEventArgs(column, rowIndex, columnIndex, item));
						}
					};
					break;
				case Grid.CellDoubleClickEvent:
					Control.MouseDoubleClick += (sender, e) =>
					{
						int rowIndex;
						if ((rowIndex = Control.SelectedIndex) >= 0)
						{
							var columnIndex = Control.CurrentColumn == null ? -1 : Control.CurrentColumn.DisplayIndex;
							var item = Control.SelectedItem;
							var column = columnIndex == -1 || columnIndex >= Widget.Columns.Count ? null : Widget.Columns[columnIndex];
							Callback.OnCellDoubleClick(Widget, new GridViewCellEventArgs(column, rowIndex, columnIndex, item));
						}
					};
					break;
				case Grid.SelectionChangedEvent:
					Control.SelectedCellsChanged += (sender, e) =>
					{
						if (!SkipSelectionChanged)
							Callback.OnSelectionChanged(Widget, EventArgs.Empty);
					};
					break;
				case Grid.CellFormattingEvent:
					// handled by FormatCell method
					break;
				default:
					base.AttachEvent(id);
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

		protected override void Initialize()
		{
			base.Initialize();
			Columns = new ColumnCollection { Handler = this };
			Columns.Register(Widget.Columns);

			// prevent DataGrid from burying these keys and propegate up to window.
			// from: http://gonetdotnet.blogspot.ca/2014/04/solved-how-to-disable-default-keyboard.html
			Control.InputBindings.Add(new swi.KeyBinding(swi.ApplicationCommands.NotACommand, swi.Key.C, swi.ModifierKeys.Control));
			Control.InputBindings.Add(new swi.KeyBinding(swi.ApplicationCommands.NotACommand, swi.Key.Delete, swi.ModifierKeys.None));
		}

		protected class ColumnCollection : EnumerableChangedHandler<GridColumn, GridColumnCollection>
		{
			public GridHandler<TWidget, TCallback> Handler { get; set; }

			public override void AddItem(GridColumn item)
			{
				var colhandler = (GridColumnHandler)item.Handler;
				colhandler.GridHandler = Handler;
				Handler.Control.Columns.Add(colhandler.Control);
			}

			public override void InsertItem(int index, GridColumn item)
			{
				var colhandler = (GridColumnHandler)item.Handler;
				colhandler.GridHandler = Handler;
				Handler.Control.Columns.Insert(index, colhandler.Control);
			}

			public override void RemoveItem(int index)
			{
				Handler.Control.Columns.RemoveAt(index);
			}

			public override void RemoveAllItems()
			{
				Handler.Control.Columns.Clear();
			}
		}

		public ContextMenu ContextMenu
		{
			get { return contextMenu; }
			set
			{
				contextMenu = value;
				Control.ContextMenu = contextMenu != null ? ((ContextMenuHandler)contextMenu.Handler).Control : null;
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
				if (list != null)
				{
					foreach (object item in Control.SelectedItems)
					{
						yield return list.IndexOf(item);
					}
				}
			}
			set
			{
				if (AllowMultipleSelection)
				{
					var list = Control.ItemsSource as IList;
					if (list != null)
					{
						Control.BeginUpdateSelectedItems();

						Control.SelectedItems.Clear();
						foreach (int row in value)
						{
							Control.SelectedItems.Add(list[row]);
						}

						Control.EndUpdateSelectedItems();
					}
				}
				else if (value == null)
					UnselectAll();
				else
				{
					UnselectAll();
					SelectRow(value.FirstOrDefault());
				}
			}
		}

		public int RowHeight
		{
			get { return (int)Control.RowHeight; }
			set { Control.RowHeight = value; }
		}

		public void SelectAll()
		{
			Control.SelectAll();
		}

		public void SelectRow(int row)
		{
			var list = Control.ItemsSource as IList;
			if (list != null)
			{
				if (AllowMultipleSelection)
					Control.SelectedItems.Add(list[row]);
				else
				{
					SaveColumnFocus();
					Control.SelectedIndex = row;
					RestoreColumnFocus();
				}
			}
		}

		public void UnselectRow(int row)
		{
			var list = Control.ItemsSource as IList;
			if (list != null)
			{
				if (AllowMultipleSelection)
					Control.SelectedItems.Remove(list[row]);
				else
					Control.UnselectAll();
			}
		}

		public void UnselectAll()
		{
			Control.UnselectAll();
		}

		public void BeginEdit(int row, int column)
		{
			Control.UnselectAll();
			//sometimes couldn't focus to cell, so use ScrollIntoView
			Control.ScrollIntoView(Control.Items[row]);
			//set current cell
			Control.CurrentCell = new swc.DataGridCellInfo(Control.Items[row], Control.Columns[column]);	
			Control.Focus();
			Control.BeginEdit();
		}

		public virtual sw.FrameworkElement SetupCell(IGridColumnHandler column, sw.FrameworkElement defaultContent)
		{
			return defaultContent;
		}

		class FormatEventArgs : GridCellFormatEventArgs
		{
			public sw.FrameworkElement Element { get; private set; }

			public swc.DataGridCell Cell { get; private set; }
			Font font;

			public FormatEventArgs(GridColumn column, swc.DataGridCell gridcell, object item, int row, sw.FrameworkElement element)
				: base(column, item, row)
			{
				this.Element = element;
				this.Cell = gridcell;
			}

			public override Font Font
			{
				get
				{
					return font ?? (font = Cell.GetEtoFont());
				}
				set
				{
					font = value;
					Cell.SetEtoFont(font);
				}
			}

			public override Color BackgroundColor
			{
				get
				{
					return Cell.Background.ToEtoColor();
				}
				set
				{
					Cell.Background = value.ToWpfBrush(Cell.Background);
				}
			}

			public override Color ForegroundColor
			{
				get
				{
					return Cell.Foreground.ToEtoColor();
				}
				set
				{
					Cell.Foreground = value.ToWpfBrush(Cell.Foreground);
				}
			}
		}

		public override bool HasFocus
		{
			get
			{
				if (Widget.ParentWindow != null)
					return Control.HasFocus((sw.DependencyObject)Widget.ParentWindow.ControlObject);
				return base.HasFocus;
			}
		}

		public override void Focus()
		{
			SaveColumnFocus();
			base.Focus();
			RestoreColumnFocus();
		}

		public override void Invalidate(bool invalidateChildren)
		{
			SaveColumnFocus();
			Control.Items.Refresh();
			RestoreColumnFocus();
			base.Invalidate(invalidateChildren);
		}

		public override void Invalidate(Rectangle rect, bool invalidateChildren)
		{
			SaveColumnFocus();
			Control.Items.Refresh();
			RestoreColumnFocus();
			base.Invalidate(rect, invalidateChildren);
		}

		public virtual void FormatCell(IGridColumnHandler column, ICellHandler cell, sw.FrameworkElement element, swc.DataGridCell gridcell, object dataItem)
		{
			if (IsEventHandled(Grid.CellFormattingEvent))
			{
				var row = Control.Items.IndexOf(dataItem);
				Callback.OnCellFormatting(Widget, new FormatEventArgs(column.Widget as GridColumn, gridcell, dataItem, row, element));
			}
		}

		protected void SaveColumnFocus()
		{
			CurrentColumn = Control.CurrentColumn;
		}

		protected void RestoreColumnFocus()
		{
			Control.CurrentColumn = null;
			if (Control.Columns.Count > 0)
				Control.CurrentCell = new swc.DataGridCellInfo(Control.SelectedItem, CurrentColumn ?? Control.CurrentColumn ?? Control.Columns[0]);
			CurrentColumn = null;
		}

		protected void SaveFocus()
		{
			SaveColumnFocus();
			hasFocus = HasFocus;
		}

		protected void RestoreFocus()
		{
			if (hasFocus)
			{
				Focus();
				RestoreColumnFocus();
			}
		}

		public void CellEdited(int row, swc.DataGridColumn dataGridColumn, object dataItem)
		{
			var gridColumn = Widget.Columns[dataGridColumn.DisplayIndex];
			Callback.OnCellEdited(Widget, new GridViewCellEventArgs(gridColumn, row, dataGridColumn.DisplayIndex, dataItem));
		}

		public void ScrollToRow(int row)
		{
			Control.ScrollIntoView(Control.Items[row]);
		}

		public GridLines GridLines
		{
			get
			{
				switch (Control.GridLinesVisibility)
				{
					case System.Windows.Controls.DataGridGridLinesVisibility.All:
						return Eto.Forms.GridLines.Both;
					case System.Windows.Controls.DataGridGridLinesVisibility.Horizontal:
						return Eto.Forms.GridLines.Horizontal;
					case System.Windows.Controls.DataGridGridLinesVisibility.None:
						return Eto.Forms.GridLines.None;
					case System.Windows.Controls.DataGridGridLinesVisibility.Vertical:
						return Eto.Forms.GridLines.Vertical;
					default:
						throw new NotSupportedException();
				}
			}
			set
			{
				switch (value)
				{
					case GridLines.None:
						Control.GridLinesVisibility = swc.DataGridGridLinesVisibility.None;
						break;
					case GridLines.Horizontal:
						Control.GridLinesVisibility = swc.DataGridGridLinesVisibility.Horizontal;
						break;
					case GridLines.Vertical:
						Control.GridLinesVisibility = swc.DataGridGridLinesVisibility.Vertical;
						break;
					case GridLines.Both:
						Control.GridLinesVisibility = swc.DataGridGridLinesVisibility.All;
						break;
					default:
						break;
				}
			}
		}

		static object Border_Key = new object();

		public BorderType Border
		{
			get { return Widget.Properties.Get(Border_Key, BorderType.Bezel); }
			set { Widget.Properties.Set(Border, value, () => Control.SetEtoBorderType(value)); }
		}

		public void ReloadData(IEnumerable<int> rows)
		{
			Control.Items.Refresh();
		}
	}
}
