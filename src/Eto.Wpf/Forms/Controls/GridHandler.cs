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

		protected override void OnPreviewKeyDown(swi.KeyEventArgs e)
		{
			base.OnPreviewKeyDown(e);
			if (!e.Handled && e.Key == swi.Key.Enter && swi.Keyboard.Modifiers == swi.ModifierKeys.None)
			{
				CommitEdit(); // if needed, commit the editing
				// don't go to next row!
				e.Handled = true;
			}
		}

	}

	class GridDragRowState
	{
		public swc.DataGridRow Row;
		public sw.Thickness BorderThickness;
		public swm.Brush BorderBrush;
		public swm.Brush Background;
		public swm.Brush Foreground;
		public int ChildIndex;

		public GridDragRowState(swc.DataGridRow row, int childIndex)
		{
			Row = row;
			//Item = row.Item;
			BorderThickness = row.BorderThickness;
			BorderBrush = row.BorderBrush;
			Background = row.Background;
			Foreground = row.Foreground;
			ChildIndex = childIndex;
		}

		public bool IsEqual(swc.DataGridRow row, int childIndex)
		{
			if (!ReferenceEquals(row, Row))
				return false;
			if (childIndex != ChildIndex)
				return false;
			return true;
		}

		public void Revert()
		{
			Row.BorderThickness = BorderThickness;
			Row.BorderBrush = BorderBrush;
			Row.Background = Background;
			Row.Foreground = Foreground;
		}
	}

	static class GridHandler
	{
		public static readonly object IsEditing_Key = new object();
		public static readonly object LastDragRow_Key = new object();
		public static readonly object Border_Key = new object();
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
					Control.PreviewMouseDown += (sender, e) => Callback.OnCellClick(Widget, CreateCellMouseArgs(e.OriginalSource, e));
					break;
				case Grid.CellDoubleClickEvent:
					Control.MouseDoubleClick += (sender, e) => Callback.OnCellDoubleClick(Widget, CreateCellMouseArgs(e.OriginalSource, e));
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

		GridCellMouseEventArgs CreateCellMouseArgs(object originalSource, swi.MouseButtonEventArgs ea)
		{
			swc.DataGridCell cell;
			var row = GetRowOfElement(originalSource, out cell);

			int rowIndex = row?.GetIndex() ?? -1;
			var columnIndex = cell?.Column?.DisplayIndex ?? -1;

			var item = row?.Item;
			var column = columnIndex == -1 || columnIndex >= Widget.Columns.Count ? null : Widget.Columns[columnIndex];

			var buttons = ea.GetEtoButtons();
			var modifiers = swi.Keyboard.Modifiers.ToEto();
			var location = ea.GetPosition(ContainerControl).ToEto();
			return new GridCellMouseEventArgs(column, rowIndex, columnIndex, item, buttons, modifiers, location);
		}

		swc.DataGridRow GetRowOfElement(object source, out swc.DataGridCell cell)
		{
			// when clicking on labels, etc this will be a content element
			while (source is sw.FrameworkContentElement)
				source = ((sw.FrameworkContentElement)source).Parent;

			// VisualTreeHelper will throw if not a Visual, we can return null here
			var dep = source as swm.Visual;
			while (dep != null && !(dep is swc.DataGridCell))
				dep = swm.VisualTreeHelper.GetParent(dep) as swm.Visual;

			cell = dep as swc.DataGridCell;
			while (dep != null && !(dep is swc.DataGridRow))
				dep = swm.VisualTreeHelper.GetParent(dep) as swm.Visual;

			return dep as swc.DataGridRow;
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

		public bool CommitEdit() => Control.CommitEdit();

		public bool CancelEdit() => Control.CancelEdit();

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
				var parent = Control.GetParent<sw.Window>();
				if (parent != null)
					return Control.HasFocus(parent);
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
			SetIsEditing(false);
			Callback.OnCellEdited(Widget, new GridViewCellEventArgs(gridColumn, row, dataGridColumn.DisplayIndex, dataItem));
			SetIsEditing(null);
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

		public BorderType Border
		{
			get { return Widget.Properties.Get(GridHandler.Border_Key, BorderType.Bezel); }
			set { Widget.Properties.Set(GridHandler.Border_Key, value, () => Control.SetEtoBorderType(value)); }
		}

		public void ReloadData(IEnumerable<int> rows)
		{
			Control.Items.Refresh();
		}

		swc.DataGridRow GetCurrentRow()
		{
			var selectedIndex = Control.SelectedIndex;
			if (selectedIndex >= 0)
			{
				return Control.ItemContainerGenerator.ContainerFromIndex(selectedIndex) as swc.DataGridRow;
			}
			return null;
		}

		public bool IsEditing => Widget.Properties.Get<bool?>(GridHandler.IsEditing_Key) ?? GetCurrentRow()?.IsEditing == true;

		void SetIsEditing(bool? value) => Widget.Properties.Set(GridHandler.IsEditing_Key, value);

		protected swc.DataGridRow GetDataGridRow(object item) => Control.ItemContainerGenerator.ContainerFromItem(item) as swc.DataGridRow;

		internal GridDragRowState LastDragRow
		{
			get { return Widget.Properties.Get<GridDragRowState>(GridHandler.LastDragRow_Key); }
			set { Widget.Properties.Set(GridHandler.LastDragRow_Key, value); }
		}

		bool IGridHandler.Loaded => Widget.Loaded;
	}
}
