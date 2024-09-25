using Eto.Wpf.Forms.Cells;
using Eto.Wpf.Forms.Menu;
using Eto.Wpf.Drawing;
using Eto.Wpf.CustomControls.TreeGridView;

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
				IEditableCollectionView itemsView = Items;
				// IsEditingItem value will be true twice because we commit cell first, then the row.
				// See the remark on this page:
				// https://docs.microsoft.com/en-us/dotnet/api/system.windows.controls.datagrid.commitedit
				if (itemsView.IsAddingNew || itemsView.IsEditingItem)
				{
					CommitEdit();
					// don't go to next row!
					e.Handled = true;
				}
			}
		}

		public EtoDataGrid()
		{
			Loaded += EtoDataGrid_Loaded;
		}

				private void EtoDataGrid_Loaded(object sender, sw.RoutedEventArgs e)
		{
			var scp = this.FindChild<swc.ScrollContentPresenter>();
			if (scp != null) scp.RequestBringIntoView += OnRequestBringIntoView;
		}

		private void OnRequestBringIntoView(object sender, sw.RequestBringIntoViewEventArgs e)
		{
			var h = Handler as IGridHandler;
			if (h == null)
				return;
			e.Handled = h.DisableAutoScrollToSelection;
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
		public static readonly object MultipleSelectionInfo_Key = new object();
		public static readonly object AllowEmptySelection_Key = new object();
		public static readonly object OutsideMouseDown_Key = new object();
	}

	public abstract class GridHandler<TWidget, TCallback> : WpfControl<EtoDataGrid, TWidget, TCallback>, Grid.IHandler, IGridHandler
		where TWidget : Grid
		where TCallback : Grid.ICallback
	{
		DateTime? startDragOverTime;
		const int DragScrollSize = 12;
		const int DragScrollFastSize = 6;
		const double DragScrollDelay = 0.1;
		const double DragScrollFastDelay = 0.01;
		
		ContextMenu contextMenu;
		bool hadFocus;
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
				CanUserReorderColumns = false,
				RowHeaderWidth = 0,
				SelectionMode = swc.DataGridSelectionMode.Single,
				GridLinesVisibility = swc.DataGridGridLinesVisibility.None,
				EnableColumnVirtualization = true,
				EnableRowVirtualization = true,
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
						var gridColumn = GetColumn(e.Column);
						Callback.OnCellEditing(Widget, new GridViewCellEventArgs(gridColumn, row, Widget.Columns.IndexOf(gridColumn), item));
					};
					break;
				case Grid.CellEditedEvent:
					// handled by each cell after value is set with the CellEdited method
					break;
				case Grid.CellClickEvent:
					Control.PreviewMouseDown += (sender, e) => {
						var hitTestResult = swm.VisualTreeHelper.HitTest(Control, e.GetPosition(Control))?.VisualHit;
						if (!TreeTogglePanel.IsOverExpander(hitTestResult))
						{
							var args = CreateCellMouseArgs(e.OriginalSource, e, out var isValid);
							if (isValid)
							{
								Callback.OnCellClick(Widget, args);
								e.Handled = args.Handled;
							}
						}
					};
					break;
				case Grid.CellDoubleClickEvent:
					Control.MouseDoubleClick += (sender, e) => {
						var args = CreateCellMouseArgs(e.OriginalSource, e, out var isValid);
						if (isValid)
							Callback.OnCellDoubleClick(Widget, args);
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
				case Grid.RowFormattingEvent:
					Control.LoadingRow += HandleLoadingRow;
					break;
				case Grid.ColumnOrderChangedEvent:
					Control.ColumnReordered += HandleColumnReordered;
					break;
				case Grid.ColumnWidthChangedEvent:
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		private void HandleLoadingRow(object sender, swc.DataGridRowEventArgs e)
		{
			var row = e.Row;
			if (row == null)
				return;
			row.ClearValue(swc.Control.BackgroundProperty);

			Callback.OnRowFormatting(Widget, new RowFormatEventArgs(row, row.Item, row.GetIndex()));
		}


		private void HandleColumnReordered(object sender, swc.DataGridColumnEventArgs e)
		{
			var column = GetColumn(e.Column);
			Callback.OnColumnOrderChanged(Widget, new GridColumnEventArgs(column));
		}

		GridCellMouseEventArgs CreateCellMouseArgs(object originalSource, swi.MouseButtonEventArgs ea) => CreateCellMouseArgs(originalSource, ea, out _);

		GridCellMouseEventArgs CreateCellMouseArgs(object originalSource, swi.MouseButtonEventArgs ea, out bool isValid)
		{
			var row = GetRowOfElement(originalSource, out var dataGridColumn, out isValid);

			int rowIndex = row?.GetIndex() ?? -1;
			var column = GetColumn(dataGridColumn);
			var columnIndex = column != null ? Widget.Columns.IndexOf(column) : -1;

			var item = row?.Item;

			var buttons = ea.GetEtoButtons();
			var modifiers = swi.Keyboard.Modifiers.ToEto();
			var location = ea.GetPosition(ContainerControl).ToEto();
			return new GridCellMouseEventArgs(column, rowIndex, columnIndex, item, buttons, modifiers, location);
		}

		swc.DataGridRow GetRowOfElement(object source, out swc.DataGridColumn column, out bool isValid)
		{
			// when clicking on labels, etc this will be a content element
			while (source is sw.FrameworkContentElement)
				source = ((sw.FrameworkContentElement)source).Parent;

			// VisualTreeHelper will throw if not a Visual, we can return null here
			column = null;
			var dep = source as swm.Visual;
			while (dep != null)
			{
				if (dep is swc.DataGridCell cell)
				{
					column = cell.Column;
					break;
				}
				if (dep is swc.Primitives.DataGridColumnHeader header)
				{
					column = header.Column;
					break;
				}
				if (ReferenceEquals(dep, Control))
				{
					// found the grid, it's a valid click.. but no row or cell.
					isValid = true;
					return null;
				}
				dep = swm.VisualTreeHelper.GetParent(dep) as swm.Visual;
			}

			while (dep != null && !(dep is swc.DataGridRow))
				dep = swm.VisualTreeHelper.GetParent(dep) as swm.Visual;

			if (dep is swc.DataGridRow row)
			{
				isValid = true;
				return row;
			}

			// we didn't find the DataGrid (e.g. clicking on a drop down menu, etc)
			// so don't fire the event
			isValid = false;
			return null;
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

			// Ensure we override selection behaviour to better support drag/drop:
			// 1. When multi-select is on, don't change selection until mouse up 
			//    when clicking on a selected item
			// 2. When a cell is editable, don't begin editing until mouse up when
			//    the cell is selected.
			HandleEvent(Eto.Forms.Control.MouseDownEvent);
			HandleEvent(Eto.Forms.Control.MouseUpEvent);

			Control.Loaded += Control_Loaded;

			// Listen to changes of the column header style so we can apply column styles appropriately for alignment
			AttachPropertyChanged(swc.DataGrid.ColumnHeaderStyleProperty, Control_ColumnHeaderStyleChanged, Control);
		}

		private void Control_ColumnHeaderStyleChanged(object sender, sw.DependencyPropertyChangedEventArgs e)
		{
			foreach (var col in Widget.Columns)
			{
				if (col.Handler is IGridColumnHandler columnHandler)
				{
					columnHandler.SetHeaderStyle();
				}
			}
		}

		private void Control_Loaded(object sender, sw.RoutedEventArgs e)
		{
			// expanded columns don't get autosized, so we flip to star width after they are auto sized.
			foreach (var col in Widget.Columns)
			{
				if (col.Handler is IGridColumnHandler columnHandler)
				{
					columnHandler.OnLoad();
				}
			}
		}

		protected class ColumnCollection : EnumerableChangedHandler<GridColumn, GridColumnCollection>
		{
			public GridHandler<TWidget, TCallback> Handler { get; set; }

			public override void AddItem(GridColumn item)
			{
				var colhandler = (GridColumnHandler)item.Handler;
				colhandler.Setup(Handler);
				Handler.Control.Columns.Add(colhandler.Control);
			}

			public override void InsertItem(int index, GridColumn item)
			{
				var colhandler = (GridColumnHandler)item.Handler;
				colhandler.Setup(Handler);
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
			set
			{
				Control.SelectionMode = value ? swc.DataGridSelectionMode.Extended : swc.DataGridSelectionMode.Single;
			}
		}

		class SelectionInfo
		{
			public swc.DataGridRow Row { get; set; }
			public swc.DataGridCell Cell { get; set; }
			public int ClickCount { get; set; }

		}

		SelectionInfo MultipleSelectionInfo
		{
			get => Widget.Properties.Get<SelectionInfo>(GridHandler.MultipleSelectionInfo_Key);
			set => Widget.Properties.Set(GridHandler.MultipleSelectionInfo_Key, value);
		}

		protected override void HandleMouseUp(object sender, swi.MouseButtonEventArgs e)
		{
			base.HandleMouseUp(sender, e);

			var hitTestResult = swm.VisualTreeHelper.HitTest(Control, e.GetPosition(Control))?.VisualHit;
			var cell = hitTestResult?.GetVisualParent<swc.DataGridCell>();
			var row = hitTestResult?.GetVisualParent<swc.DataGridRow>();

			var info = MultipleSelectionInfo;
			if (!e.Handled && info != null)
			{
				if (ReferenceEquals(row, info.Row))
				{
					// in multiple selection, only set selection to current row if the mouse hasn't moved to a different row
					bool hadMultipleSelection = Control.SelectedItems.Count > 1;
					Control.SelectedItem = row.Item;
					if (cell != null)
					{
						var args = CreateCellMouseArgs(cell, e);
						if (!TreeTogglePanel.IsOverExpander(hitTestResult))
							Callback.OnCellClick(Widget, args);

						if (!args.Handled)
						{
							if (!hadMultipleSelection && ReferenceEquals(info.Cell, cell))
							{
								// we double clicked to fire this event, so trigger a double click event
								if (info.ClickCount >= 2)
									Callback.OnCellDoubleClick(Widget, args);
								if (!args.Handled)
								{
									if (TreeTogglePanel.IsOverContent(hitTestResult) != false)
									{
										// let the column handler perform something specific if needed
										var columnHandler = args.GridColumn?.Handler as GridColumnHandler;
										columnHandler?.OnMouseUp(args, hitTestResult, cell);

										if (!args.Handled && !cell.Column.IsReadOnly)
										{
											cell.Focus();
											Control.BeginEdit();
										}

										e.Handled = true; // prevent default behaviour
									}
								}
							}
							else
								cell.Focus();
						}
						else
						{
							row.Focus();
							e.Handled = true;
						}
					}
				}

				MultipleSelectionInfo = null;
			}			

			if (!e.Handled && cell != null && TreeTogglePanel.IsOverContent(hitTestResult) != false)
			{
				var args = CreateCellMouseArgs(cell, e);
				// let the column handler perform something specific if needed
				var columnHandler = args.GridColumn?.Handler as GridColumnHandler;
				columnHandler?.OnMouseUp(args, hitTestResult, cell);
				e.Handled = args.Handled;
			}
			
			HandleOutsideMouseUp(sender, e);
		}


		private void HandleOutsideMouseUp(object sender, swi.MouseButtonEventArgs e)
		{
			if (!Widget.Properties.Get<bool>(GridHandler.OutsideMouseDown_Key))
				return;
			Widget.Properties.Set(GridHandler.OutsideMouseDown_Key, false);
			
			if (e.Handled)
				return;
			
			var hitTestResult = swm.VisualTreeHelper.HitTest(Control, e.GetPosition(Control))?.VisualHit;
			if (hitTestResult != null
				&& (
					hitTestResult is swc.ScrollViewer // below rows
					|| swm.VisualTreeHelper.GetParent(hitTestResult) is swc.DataGridRow // right of rows
					)
				)
			{
				CommitEdit();
				if (AllowEmptySelection)
				{
					UnselectAll();
					e.Handled = true;
				}
			}
		}
		
		private void HandleOutsideMouseDown(object sender, swi.MouseButtonEventArgs e)
		{
			if (!e.Handled)
			{
				var hitTestResult = swm.VisualTreeHelper.HitTest(Control, e.GetPosition(Control))?.VisualHit;
				if (hitTestResult != null
					&& (
						hitTestResult is swc.ScrollViewer // below rows
						|| swm.VisualTreeHelper.GetParent(hitTestResult) is swc.DataGridRow // right of rows
						)
					)
				{
					Widget.Properties.Set(GridHandler.OutsideMouseDown_Key, true);
					return;
				}
			}
			
			Widget.Properties.Set(GridHandler.OutsideMouseDown_Key, false);
		}

		protected override void HandleMouseDown(object sender, swi.MouseButtonEventArgs e)
		{
			base.HandleMouseDown(sender, e);
			MultipleSelectionInfo = null;
			if (!e.Handled
				&& e.LeftButton == swi.MouseButtonState.Pressed
				&& swi.Keyboard.Modifiers == swi.ModifierKeys.None)
			{
				// prevent WPF from deselecting other rows until mouse up.
				var hitTestResult = swm.VisualTreeHelper.HitTest(Control, e.GetPosition(Control))?.VisualHit;
				var row = hitTestResult?.GetVisualParent<swc.DataGridRow>();
				var cell = hitTestResult?.GetVisualParent<swc.DataGridCell>();

				if (row != null && row.IsSelected
					&& (
						(cell?.Column.IsReadOnly == false && !cell.IsEditing && cell.IsFocused)
						|| Control.SelectedItems.Count > 1
					)
				)
				{
					MultipleSelectionInfo = new SelectionInfo
					{
						Row = row,
						Cell = cell,
						ClickCount = e.ClickCount
					};
					if (cell != null)
						cell.Focus();
					else
						row.Focus();
					e.Handled = true;
				} 
				else if (cell?.IsEditing == true)
				{
					// allow clicking on the image of an ImageTextCell to commit editing.
					var args = CreateCellMouseArgs(cell, e); 
					var columnHandler = args.GridColumn?.Handler as GridColumnHandler;
					columnHandler?.OnMouseDown(args, hitTestResult, cell);
					e.Handled = args.Handled;

					if (!args.Handled && TreeTogglePanel.IsOverContent(hitTestResult) == false)
					{
						// clicked outside of content area in TreeGridView, so we should commit editing.
						CommitEdit();
						e.Handled = true;
					}
				}
			}
			else if (!e.Handled
				&& !AllowEmptySelection
				&& e.LeftButton == swi.MouseButtonState.Pressed
				&& swi.Keyboard.Modifiers == swi.ModifierKeys.Control)
			{
				// prevent deselecting the last selected item
				var hitTestResult = swm.VisualTreeHelper.HitTest(Control, e.GetPosition(Control))?.VisualHit;
				var row = hitTestResult?.GetVisualParent<swc.DataGridRow>();
				var cell = hitTestResult?.GetVisualParent<swc.DataGridCell>();

				if (row != null && row.IsSelected
					&& cell != null
					&& Control.SelectedItems.Count == 1
					)
				{
					e.Handled = true;
				}
			}
			
			// test if we clicked on an empty area to unselect all when mouse up is called
			HandleOutsideMouseDown(sender, e);
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
							// protect against any incorrect info
							if (row >= list.Count)
								continue;
							var item = list[row];
							if (item != null)
								Control.SelectedItems.Add(item);
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
			CommitEdit();
			//sometimes couldn't focus to cell, so use ScrollIntoView
			Control.ScrollIntoView(Control.Items[row]);
			//set current cell and select its row.
			if (!SelectedRows.Contains(row))
			{
				Control.UnselectAll();
				Control.SelectedIndex = row;
			}
			Control.CurrentCell = new swc.DataGridCellInfo(Control.Items[row], Control.Columns[column]);	
			Control.Focus();
			Control.BeginEdit();
		}

		public bool CommitEdit() => Control.CommitEdit();

		public bool CancelEdit() => Control.CancelEdit();

		public virtual sw.FrameworkElement SetupCell(IGridColumnHandler column, sw.FrameworkElement defaultContent, swc.DataGridCell cell)
		{
			return defaultContent;
		}

		class CellFormatEventArgs : GridCellFormatEventArgs
		{
			public sw.FrameworkElement Element { get; private set; }

			public swc.DataGridCell Cell { get; private set; }
			Font font;

			public CellFormatEventArgs(GridColumn column, swc.DataGridCell gridcell, object item, int row, sw.FrameworkElement element)
				: base(column, item, row)
			{
				this.Element = element;
				this.Cell = gridcell;
			}

			public override Font Font
			{
				get => font ?? (font = Cell.GetEtoFont());
				set
				{
					font = value;
					Cell.SetEtoFont(font);
				}
			}

			public override Color BackgroundColor
			{
				get => Cell.Background.ToEtoColor();
				set => Cell.Background = value.ToWpfBrush();
			}

			public override Color ForegroundColor
			{
				get => Cell.Foreground.ToEtoColor();
				set => Cell.Foreground = value.ToWpfBrush();
			}
		}


		class RowFormatEventArgs : GridRowFormatEventArgs
		{
			swc.DataGridRow _gridrow;
			public RowFormatEventArgs(swc.DataGridRow gridrow, object item, int row) : base(item, row)
			{
				_gridrow = gridrow;
			}

			public override Color BackgroundColor
			{
				get => _gridrow.Background.ToEtoColor();
				set => _gridrow.Background = value.ToWpfBrush();
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
				// we always reset values
				gridcell.ClearValue(swc.Control.ForegroundProperty);
				gridcell.ClearValue(swc.Control.BackgroundProperty);
				gridcell.ClearValue(swc.Control.FontFamilyProperty);
				gridcell.ClearValue(swc.Control.FontStyleProperty);
				gridcell.ClearValue(swc.Control.FontWeightProperty);
				gridcell.ClearValue(swc.Control.FontSizeProperty);
				Callback.OnCellFormatting(Widget, new CellFormatEventArgs(column.Widget as GridColumn, gridcell, dataItem, row, element));
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
			{
				// ensure the saved column still exists in the grid, it could be removed by user logic
				if (CurrentColumn != null && Control.Columns.Contains(CurrentColumn))
					Control.CurrentCell = new swc.DataGridCellInfo(Control.SelectedItem, CurrentColumn);
				else
					Control.CurrentCell = new swc.DataGridCellInfo(Control.SelectedItem, Control.CurrentColumn ?? Control.Columns[0]);
			}
			CurrentColumn = null;
		}

		protected void SaveFocus()
		{
			SaveColumnFocus();
			hadFocus = HasFocus && Widget.ParentWindow?.HasFocus == true;
		}

		protected void RestoreFocus()
		{
			if (hadFocus)
				Focus();

			if (CurrentColumn != null)
				RestoreColumnFocus();
		}
		
		GridColumn GetColumn(swc.DataGridColumn dataGridColumn)
		{
			if (dataGridColumn == null)
				return null;
			var columns = Widget.Columns;
			foreach (var col in columns)
			{
				if (col.Handler is IGridColumnHandler handler && ReferenceEquals(handler.Control, dataGridColumn))
					return col;
			}
			return null;
		}

		public void CellEdited(int row, swc.DataGridColumn dataGridColumn, object dataItem)
		{
			var gridColumn = GetColumn(dataGridColumn);
			var columnIndex = gridColumn != null ? Widget.Columns.IndexOf(gridColumn) : -1;
			SetIsEditing(false);
			Callback.OnCellEdited(Widget, new GridViewCellEventArgs(gridColumn, row, columnIndex, dataItem));
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
			set { if (Widget.Properties.TrySet(GridHandler.Border_Key, value)) Control.SetEtoBorderType(value); }
		}

		public void ReloadData(IEnumerable<int> rows)
		{
			SkipSelectionChanged = true;
			SaveFocus();

			Control.Items.Refresh();
			
			DisableAutoScrollToSelection = true;
			RestoreFocus();
			SkipSelectionChanged = false;
			DisableAutoScrollToSelection = false;
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

		Grid IGridHandler.Widget => Widget;

		public bool AllowEmptySelection
		{
			get => Widget.Properties.Get<bool>(GridHandler.AllowEmptySelection_Key, true);
			set => Widget.Properties.Set(GridHandler.AllowEmptySelection_Key, value, true);
		}
		public bool DisableAutoScrollToSelection { get; set; }

		protected void EnsureSelection()
		{
			if (!AllowEmptySelection 
				&& (Control.SelectedItems?.Count ?? 0) == 0
				&& (Control.ItemsSource as IList)?.Count > 0)
			{
				SelectRow(0);
			}
		}

		public void OnColumnWidthChanged(GridColumnHandler gridColumnHandler)
		{
			Callback.OnColumnWidthChanged(Widget, new GridColumnEventArgs(gridColumnHandler.Widget));
		}
		
		internal (swc.DataGridColumn Column, swc.DataGridRow Row, bool IsHeader) GetCellInfoForElement(sw.DependencyObject element)
		{
			swc.DataGridColumn column = null;
			bool isHeader = false;
			var cell = element.GetVisualParent<swc.DataGridCell>();
			if (cell != null)
				column = cell.Column;
			else
			{
				var header = element.GetVisualParent<swc.Primitives.DataGridColumnHeader>();
				if (header != null)
				{
					column = header.Column;
					isHeader = true;
				}
			}

			var dataGridRow = element.GetVisualParent<swc.DataGridRow>();
			return (column, dataGridRow, isHeader);
		}
		
		internal (GridColumn Column, int ColumnIndex, int RowIndex, GridCellType CellType, object Item) GetCellInfo(PointF location)
		{
			var hitTestResult = swm.VisualTreeHelper.HitTest(Control, location.ToWpf())?.VisualHit;
			var cellInfo = GetCellInfoForElement(hitTestResult);
			var columnIndex = cellInfo.Column != null ? Control.Columns.IndexOf(cellInfo.Column) : -1;
			var column = columnIndex != -1 ? Widget.Columns[columnIndex] : null;
			int rowIndex = -1;
			object item = null;
			if (cellInfo.Row != null)
			{
				rowIndex = cellInfo.Row.GetIndex();
				item = GetItemAtRow(rowIndex);
			}
			var cellType = cellInfo.IsHeader ? GridCellType.ColumnHeader : rowIndex != -1 && columnIndex != -1 ? GridCellType.Data : GridCellType.None;
			return (column, columnIndex, rowIndex, cellType, item);
		}

		swc.ScrollViewer ScrollViewer => Control.FindChild<swc.ScrollViewer>();

		protected override void HandleDragOver(sw.DragEventArgs e, Eto.Forms.DragEventArgs args)
		{
			base.HandleDragOver(e, args);

			var scrollViewer = ScrollViewer;
			if (scrollViewer == null)
				return;
				
			void ScrollWithIncrement(bool fast, double increment)
			{
				var now = DateTime.Now;
				if (startDragOverTime == null)
				{
					startDragOverTime = now;
					return;
				}
				
				var diff = now - startDragOverTime;
				if (diff > TimeSpan.FromSeconds(fast ? DragScrollFastDelay : DragScrollDelay))
				{
					startDragOverTime = now;
					var oldOffset = scrollViewer.VerticalOffset;
					var maxHeight = scrollViewer.ScrollableHeight;
					var newOffset = Math.Min(maxHeight, Math.Max(0, oldOffset + increment));
					if (newOffset != oldOffset)
						scrollViewer.ScrollToVerticalOffset(newOffset);
				}
			}

			var headerPart = ShowHeader ? Control.FindChild<swcp.DataGridColumnHeadersPresenter>("PART_ColumnHeadersPresenter") : null;
			var headerHeight = headerPart?.ActualHeight ?? 0;

			// scroll up or down when the user drags close to the edge
			if (args.Location.Y < headerHeight + DragScrollSize)
			{
				ScrollWithIncrement(args.Location.Y < headerHeight + DragScrollFastSize, -1);
				return;
			}

			var height = Control.ActualHeight;
			if (scrollViewer.ComputedHorizontalScrollBarVisibility == sw.Visibility.Visible)
			{
				var child = scrollViewer.FindChild<swcp.ScrollBar>("PART_HorizontalScrollBar");
				height -= child.ActualHeight;
			}
			
			if (args.Location.Y > height - DragScrollSize)
			{
				ScrollWithIncrement(args.Location.Y > height - DragScrollFastSize, 1);
				return;
			}

			// not in a drag scroll area, reset
			startDragOverTime = null;
		}
	}
}
