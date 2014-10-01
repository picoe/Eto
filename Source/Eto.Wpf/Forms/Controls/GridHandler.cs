using System;
using System.Collections.Generic;
using System.Linq;
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
		public new void BeginUpdateSelectedItems()
		{
			base.BeginUpdateSelectedItems();
		}
		public new void EndUpdateSelectedItems()
		{
			base.EndUpdateSelectedItems();
		}
	}


	public abstract class GridHandler<TControl, TWidget, TCallback> : WpfControl<TControl, TWidget, TCallback>, Grid.IHandler, IGridHandler
		where TControl : EtoDataGrid
		where TWidget : Grid
		where TCallback : Grid.ICallback
	{
		ContextMenu contextMenu;
		bool hasFocus;
		protected bool SkipSelectionChanged { get; set; }
		protected swc.DataGridColumn CurrentColumn { get; set; }

		protected GridHandler()
		{
			Control = (TControl)new EtoDataGrid
			{
				HeadersVisibility = swc.DataGridHeadersVisibility.Column,
				AutoGenerateColumns = false,
				CanUserAddRows = false,
				RowHeaderWidth = 0,
				SelectionMode = swc.DataGridSelectionMode.Single
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
					Control.CellEditEnding += (sender, e) =>
					{
						var row = e.Row.GetIndex();
						var item = GetItemAtRow(row);
						var gridColumn = Widget.Columns[e.Column.DisplayIndex];
						Callback.OnCellEdited(Widget, new GridViewCellEventArgs(gridColumn, row, e.Column.DisplayIndex, item));
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
		}

		protected class ColumnCollection : EnumerableChangedHandler<GridColumn, GridColumnCollection>
		{
			public GridHandler<TControl, TWidget, TCallback> Handler { get; set; }

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
					if (font == null)
						font = new Font(new FontHandler(Cell));
					return font;
				}
				set
				{
					font = value;
					FontHandler.Apply(Cell, null, font);
				}
			}

			public override Color BackgroundColor
			{
				get
				{
					var brush = Cell.Background as swm.SolidColorBrush;
					return brush != null ? brush.Color.ToEto() : Colors.White;
				}
				set
				{
					Cell.Background = new swm.SolidColorBrush(value.ToWpf());
				}
			}

			public override Color ForegroundColor
			{
				get
				{
					var brush = Cell.Foreground as swm.SolidColorBrush;
					return brush != null ? brush.Color.ToEto() : Colors.Black;
				}
				set
				{
					Cell.Foreground = new swm.SolidColorBrush(value.ToWpf());
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

		public override void Invalidate()
		{
			SaveColumnFocus();
			Control.Items.Refresh();
			RestoreColumnFocus();
			base.Invalidate();
		}

		public override void Invalidate(Rectangle rect)
		{
			SaveColumnFocus();
			Control.Items.Refresh();
			RestoreColumnFocus();
			base.Invalidate(rect);
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

	}
}
