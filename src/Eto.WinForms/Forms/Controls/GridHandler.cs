using System;
using swf = System.Windows.Forms;
using Eto.Forms;
using System.Linq;
using System.Collections.Generic;
using sd = System.Drawing;
using Eto.Drawing;
using Eto.WinForms.Drawing;
using System.Diagnostics;

namespace Eto.WinForms.Forms.Controls
{
	public interface IGridHandler
	{
		void Paint(GridColumnHandler column, sd.Graphics graphics, sd.Rectangle clipBounds, sd.Rectangle cellBounds, int rowIndex, swf.DataGridViewElementStates cellState, object value, object formattedValue, string errorText, swf.DataGridViewCellStyle cellStyle, swf.DataGridViewAdvancedBorderStyle advancedBorderStyle, ref swf.DataGridViewPaintParts paintParts);

		int GetRowOffset(GridColumnHandler column, int rowIndex);

		bool CellMouseClick(GridColumnHandler column, swf.MouseEventArgs e, int rowIndex);
		object GetItemAtRow(int row);

		Grid Grid { get; }		
	}

	public abstract class GridHandler<TWidget, TCallback> : WindowsControl<swf.DataGridView, TWidget, TCallback>, Grid.IHandler, IGridHandler
		where TWidget : Grid
		where TCallback : Grid.ICallback
	{
		ColumnCollection columns;
		bool isFirstSelection = true;
		protected int SupressSelectionChanged { get; set; }
		protected bool clearColumns;

		protected void ResetSelection()
		{
			if (!SelectedRows.Any() && AllowEmptySelection)
				isFirstSelection = true;
		}

		public abstract object GetItemAtRow(int row);

		Grid IGridHandler.Grid => Widget;

		class EtoDataGridView : swf.DataGridView
		{
			public GridHandler<TWidget, TCallback> Handler { get; set; }

			public EtoDataGridView() { DoubleBuffered = true; }

			sd.Size GetSizeWithData(sd.Size proposedSize)
			{
				int width = RowHeadersWidth;
				int height = 0;
				if (ColumnHeadersVisible)
					height += ColumnHeadersHeight;

				if (BorderStyle == swf.BorderStyle.Fixed3D)
				{
					width += 4;
					height += 4;
				}
				else if (BorderStyle == swf.BorderStyle.FixedSingle)
				{
					width += 2;
					height += 2;
				}

				height += RowCount * (RowTemplate.Height + RowTemplate.DividerHeight);

				for (int i = 0; i < ColumnCount; i++)
				{
					var col = Columns[i];
					if (col.Visible)
					{
						width += col.GetPreferredWidth(swf.DataGridViewAutoSizeColumnMode.DisplayedCells, true);
					}
				}

				if (proposedSize.Height > 0 && proposedSize.Height < height)
					width += swf.SystemInformation.VerticalScrollBarWidth;

				return new sd.Size(width, height);
			}

			public override sd.Size GetPreferredSize(sd.Size proposedSize)
			{
				var size = GetSizeWithData(proposedSize);

				var def = Handler.UserPreferredSize;
				if (def.Width >= 0)
					size.Width = def.Width;
				if (def.Height >= 0)
					size.Height = def.Height;

				return size;
			}

			protected override bool ProcessDialogKey(swf.Keys keyData)
			{
				if (IsCurrentCellInEditMode && keyData == swf.Keys.Enter && EndEdit())
				{
					// prevent going to next row after committing with enter key
					return true;
				}
				return base.ProcessDialogKey(keyData);
			}

			protected override void OnKeyDown(swf.KeyEventArgs e)
			{
				if (e.KeyData == swf.Keys.Enter)
				{
					// don't go to next row when pressing enter
					e.SuppressKeyPress = true;
				}
				base.OnKeyDown(e);
			}
		}

		protected GridHandler()
		{
			Control = new EtoDataGridView
			{
				Handler = this,
				VirtualMode = true,
				MultiSelect = false,
				SelectionMode = swf.DataGridViewSelectionMode.FullRowSelect,
				CellBorderStyle = swf.DataGridViewCellBorderStyle.None,
				RowHeadersVisible = false,
				AllowUserToAddRows = false,
				AllowUserToResizeRows = false,
				AutoSize = true,
				AutoSizeColumnsMode = swf.DataGridViewAutoSizeColumnsMode.DisplayedCells,
				ColumnHeadersHeightSizeMode = swf.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
			};
			Control.CellValueNeeded += (sender, e) =>
			{
				var item = GetItemAtRow(e.RowIndex);
				if (Widget.Columns.Count > e.ColumnIndex)
				{
					var col = Widget.Columns[e.ColumnIndex].Handler as GridColumnHandler;
					if (item != null && col != null)
						e.Value = col.GetCellValue(item);
				}
			};

			Control.CellValuePushed += (sender, e) =>
			{
				var item = GetItemAtRow(e.RowIndex);
				if (Widget.Columns.Count > e.ColumnIndex)
				{
					var col = Widget.Columns[e.ColumnIndex].Handler as GridColumnHandler;
					if (item != null && col != null)
						col.SetCellValue(item, e.Value);
				}
			};
			Control.RowPostPaint += HandleRowPostPaint;

			Control.SelectionChanged += HandleFirstSelection;
			Control.DataError += HandleDataError;
		}

		private void Widget_MouseDown(object sender, MouseEventArgs e)
		{
			if (!e.Handled && e.Buttons == MouseButtons.Primary)
			{
				var hitTest = Control.HitTest((int)e.Location.X, (int)e.Location.Y);
				if (AllowEmptySelection)
				{
					if (hitTest.Type == swf.DataGridViewHitTestType.None)
					{
						if (IsEditing)
							CancelEdit();
						UnselectAll();
					}
				}
				else if (e.Modifiers == Keys.Control
					&& hitTest.RowIndex >= 0
					&& Control.SelectedRows.Count == 1
					&& Control.SelectedRows[0].Index == hitTest.RowIndex)
				{
					// don't allow user to deselect all items
					e.Handled = true;
				}
			}
		}

		void HandleDataError(object sender, swf.DataGridViewDataErrorEventArgs e)
		{
			// ignore errors to prevent ugly popup when clearing data
			Debug.WriteLine("Data Error: {0}", e.Exception);
		}

		void HandleFirstSelection(object sender, EventArgs e)
		{
			// don't select the first row on selection
			if (Widget.Loaded && isFirstSelection)
			{
				Control.ClearSelection();
				isFirstSelection = false;
			}
			else if (SupressSelectionChanged == 0)
				Callback.OnSelectionChanged(Widget, EventArgs.Empty);
		}

		/// <summary>
		/// Unlike other controls, DataGridView's background color is implemented
		/// via the BackgroundColor property, not the BackColor property.
		/// </summary>
		public override Color BackgroundColor
		{
			get { return Control.BackgroundColor.ToEto(); }
			set { BackgroundColorSet = true; Control.BackgroundColor = value.ToSD(); }
		}

		public override void OnUnLoad(EventArgs e)
		{
			base.OnUnLoad(e);
			LeakHelper.UnhookObject(Control);
		}

		static readonly object DisableAutoSizeToggle_Key = new object();

		int DisableAutoSizeToggle
		{
			get => Widget.Properties.Get<int>(DisableAutoSizeToggle_Key);
			set => Widget.Properties.Set(DisableAutoSizeToggle_Key, value);
		}
		
		bool handledAutoSize;

		void HandleRowPostPaint(object sender, swf.DataGridViewRowPostPaintEventArgs e)
		{
			if (handledAutoSize)
				return;

			handledAutoSize = true;
			DisableAutoSizeToggle++;
			int colNum = 0;
			foreach (var col in Widget.Columns)
			{
				var colHandler = col.Handler as GridColumnHandler;
				if (col.AutoSize)
				{
					Control.AutoResizeColumn(colNum, colHandler.Control.InheritedAutoSizeMode);
					var width = colHandler.Control.Width;
					colHandler.Control.Width = width;
					colHandler.Control.AutoSizeMode = swf.DataGridViewAutoSizeColumnMode.None;
				}
				colNum++;
			}
			DisableAutoSizeToggle--;
		}

		class FormattingArgs : GridCellFormatEventArgs
		{
			public swf.DataGridViewCellFormattingEventArgs Args { get; private set; }

			public FormattingArgs(swf.DataGridViewCellFormattingEventArgs args, GridColumn column, object item, int row)
				: base(column, item, row)
			{
				this.Args = args;
			}

			Font font;

			public override Font Font
			{
				get
				{
					return font ?? (font = new Font(new FontHandler(Args.CellStyle.Font)));
				}
				set
				{
					font = value;
					Args.CellStyle.Font = font.ToSD();
				}
			}

			public override Color BackgroundColor
			{
				get { return Args.CellStyle.BackColor.ToEto(); }
				set { Args.CellStyle.BackColor = value.ToSD(); }
			}

			public override Color ForegroundColor
			{
				get { return Args.CellStyle.ForeColor.ToEto(); }
				set { Args.CellStyle.ForeColor = value.ToSD(); }
			}
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Grid.ColumnHeaderClickEvent:
					Control.ColumnHeaderMouseClick += (sender, e) =>
					{
						if (e.ColumnIndex >= 0 && columns.Collection[e.ColumnIndex].Sortable)
							Callback.OnColumnHeaderClick(Widget, new GridColumnEventArgs(Widget.Columns[e.ColumnIndex]));
					};
					break;
				case Grid.CellEditingEvent:
					Control.CellBeginEdit += (sender, e) =>
					{
						var item = GetItemAtRow(e.RowIndex);
						var column = Widget.Columns[e.ColumnIndex];
						Callback.OnCellEditing(Widget, new GridViewCellEventArgs(column, e.RowIndex, e.ColumnIndex, item));
					};
					break;
				case Grid.CellEditedEvent:
					Control.CellEndEdit += (sender, e) =>
					{
						var item = GetItemAtRow(e.RowIndex);
						var column = Widget.Columns[e.ColumnIndex];
						Callback.OnCellEdited(Widget, new GridViewCellEventArgs(column, e.RowIndex, e.ColumnIndex, item));
					};
					break;
				case Grid.CellClickEvent:
					Control.CellMouseClick += (sender, e) =>
					{
						var item = GetItemAtRow(e.RowIndex);
						var column = Widget.Columns[e.ColumnIndex];
						var location = PointFromScreen(Mouse.Position); // e.Location is relative to the cell. ugh.
						Callback.OnCellClick(Widget, new GridCellMouseEventArgs(column, e.RowIndex, e.ColumnIndex, item, e.Button.ToEto(), swf.Control.ModifierKeys.ToEto(), location, e.ToEtoDelta()));
					};
					break;
				case Grid.CellDoubleClickEvent:
					Control.CellMouseDoubleClick += (sender, e) =>
					{
						if (e.RowIndex > -1)
						{
							var item = GetItemAtRow(e.RowIndex);
							var column = Widget.Columns[e.ColumnIndex];
							var location = PointFromScreen(Mouse.Position); // e.Location is relative to the cell. ugh.
							Callback.OnCellDoubleClick(Widget, new GridCellMouseEventArgs(column, e.RowIndex, e.ColumnIndex, item, e.Button.ToEto(), swf.Control.ModifierKeys.ToEto(), location, e.ToEtoDelta()));
						}
					};
					break;
				case Grid.SelectionChangedEvent:
					// handled automatically
					break;
				case Grid.CellFormattingEvent:
					Control.CellFormatting += (sender, e) =>
					{
						var column = Widget.Columns[e.ColumnIndex];
						var item = GetItemAtRow(e.RowIndex);
						Callback.OnCellFormatting(Widget, new FormattingArgs(e, column, item, e.RowIndex));
					};
					break;
				case Grid.ColumnOrderChangedEvent:
					Control.ColumnDisplayIndexChanged += HandleColumnDisplayIndexChanged;
					Control.MouseUp += HandleColumnOrderChangedOnMouseUp;
					break;
				case Grid.ColumnWidthChangedEvent:
					Control.ColumnWidthChanged += HandleColumnWidthChanged;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		private void HandleColumnWidthChanged(object sender, swf.DataGridViewColumnEventArgs e)
		{
			var column = Widget.Columns[e.Column.Index];
			if (handledAutoSize && DisableAutoSizeToggle == 0 && column.Handler is GridColumnHandler handler)
			{
				// turn off autosize, user (most likely?) resized this column
				handler.UpdateAutoSize(false);
			}
			Callback.OnColumnWidthChanged(Widget, new GridColumnEventArgs(column));
		}

		static readonly object ColumnOrderChanged_Key = new object();
		int? ColumnOrderChangedIndex
		{
			get => Widget.Properties.Get<int?>(ColumnOrderChanged_Key);
			set => Widget.Properties.Set(ColumnOrderChanged_Key, value);
		}

		private void HandleColumnOrderChangedOnMouseUp(object sender, swf.MouseEventArgs e)
		{
			if (ColumnOrderChangedIndex != null)
			{
				var column = Widget.Columns[ColumnOrderChangedIndex.Value];
				Callback.OnColumnOrderChanged(Widget, new GridColumnEventArgs(column));
				ColumnOrderChangedIndex = null;
			}
		}

		private void HandleColumnDisplayIndexChanged(object sender, swf.DataGridViewColumnEventArgs e)
		{
			// only store the first one (which is the one being dragged)
			// we fire the event on the mouse up so it only happens once.
			if (ColumnOrderChangedIndex == null)
				ColumnOrderChangedIndex = e.Column.Index;
		}

		protected override void Initialize()
		{
			base.Initialize();
			columns = new ColumnCollection { Handler = this };
			columns.Register(Widget.Columns);
			Widget.MouseDown += Widget_MouseDown;
			Control.ColumnDividerDoubleClick += Control_ColumnDividerDoubleClick;
		}

		private void Control_ColumnDividerDoubleClick(object sender, swf.DataGridViewColumnDividerDoubleClickEventArgs e)
		{
			if (e.Button == swf.MouseButtons.Left)
			{
				DisableAutoSizeToggle++;
				if (Widget.Columns[e.ColumnIndex].Handler is GridColumnHandler handler)
				{
					handler.UpdateAutoSize(true);
				}
				Control.AutoResizeColumn(e.ColumnIndex, swf.DataGridViewAutoSizeColumnMode.DisplayedCells);
				DisableAutoSizeToggle--;
				e.Handled = true;			
			}
		}

		class ColumnCollection : EnumerableChangedHandler<GridColumn, GridColumnCollection>
		{
			public GridHandler<TWidget, TCallback> Handler { get; set; }

			public override void AddItem(GridColumn item)
			{
				var colhandler = (GridColumnHandler)item.Handler;
				colhandler.Setup(Handler);
				Handler.Control.Columns.Add(colhandler.Control);
				if (Handler.clearColumns)
				{
					Handler.Control.Columns.RemoveAt(0);
					Handler.clearColumns = false;
				}
			}

			public override void InsertItem(int index, GridColumn item)
			{
				var colhandler = (GridColumnHandler)item.Handler;
				colhandler.Setup(Handler);
				Handler.Control.Columns.Insert(index, colhandler.Control);
				if (Handler.clearColumns && Handler.Control.Columns.Count == 2 && index == 0)
				{
					Handler.Control.Columns.RemoveAt(Handler.Control.Columns.Count - 1);
					Handler.clearColumns = false;
				}
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

		public bool ShowHeader
		{
			get { return Control.ColumnHeadersVisible; }
			set { Control.ColumnHeadersVisible = value; }
		}

		public bool AllowColumnReordering
		{
			get { return Control.AllowUserToOrderColumns; }
			set { Control.AllowUserToOrderColumns = value; }
		}

		public bool AllowMultipleSelection
		{
			get { return Control.MultiSelect; }
			set { Control.MultiSelect = value; }
		}

		public IEnumerable<int> SelectedRows
		{
			get { return Control.SelectedRows.OfType<swf.DataGridViewRow>().Where(r => r.Index >= 0).Select(r => r.Index); }
			set
			{
				SupressSelectionChanged++;
				UnselectAll();
				foreach (var row in value)
				{
					SelectRow(row);
				}
				SupressSelectionChanged--;
				if (SupressSelectionChanged == 0)
					Callback.OnSelectionChanged(Widget, EventArgs.Empty);
			}
		}

		public int RowHeight
		{
			get { return Control.RowTemplate.Height; }
			set
			{
				Control.RowTemplate.Height = value;
				foreach (swf.DataGridViewRow row in Control.Rows)
				{
					row.Height = value;
				}
			}
		}

		public void SelectAll()
		{
			Control.SelectAll();
			isFirstSelection = false;
		}

		public void SelectRow(int row)
		{
			if (!AllowMultipleSelection)
				Control.CurrentCell = Control.Rows[row].Cells[0];
			else
				Control.Rows[row].Selected = true;
			isFirstSelection = false;
		}

		public void UnselectRow(int row)
		{
			Control.Rows[row].Selected = false;
		}

		public void UnselectAll()
		{
			Control.ClearSelection();
		}

		public void BeginEdit(int row, int column)
		{
			Control.CurrentCell = Control.Rows[row].Cells[column];
			Control.BeginEdit(true);
		}

		public bool CommitEdit() => Control.EndEdit();

		public bool CancelEdit() => Control.CancelEdit();

		public virtual void Paint(GridColumnHandler column, System.Drawing.Graphics graphics, System.Drawing.Rectangle clipBounds, System.Drawing.Rectangle cellBounds, int rowIndex, swf.DataGridViewElementStates cellState, object value, object formattedValue, string errorText, swf.DataGridViewCellStyle cellStyle, swf.DataGridViewAdvancedBorderStyle advancedBorderStyle, ref swf.DataGridViewPaintParts paintParts)
		{
		}

		public virtual int GetRowOffset(GridColumnHandler column, int rowIndex)
		{
			return 0;
		}

		public virtual bool CellMouseClick(GridColumnHandler column, swf.MouseEventArgs e, int rowIndex)
		{
			return false;
		}

		static readonly Win32.WM[] intrinsicEvents = {
														 Win32.WM.LBUTTONDOWN, Win32.WM.LBUTTONUP, Win32.WM.LBUTTONDBLCLK,
														 Win32.WM.RBUTTONDOWN, Win32.WM.RBUTTONUP, Win32.WM.RBUTTONDBLCLK
													 };
		public override bool ShouldBubbleEvent(swf.Message msg)
		{
			return !intrinsicEvents.Contains((Win32.WM)msg.Msg) && base.ShouldBubbleEvent(msg);
		}

		public void ScrollToRow(int row)
		{
			var displayedCount = Control.DisplayedRowCount(false);
			var idx = Control.FirstDisplayedScrollingRowIndex;
			if (row < idx)
			{
				Control.FirstDisplayedScrollingRowIndex = row;
			}
			else if (row >= idx + displayedCount)
			{
				Control.FirstDisplayedScrollingRowIndex = Math.Max(0, row - displayedCount + 1);
			}
		}

		public GridLines GridLines
		{
			get
			{
				switch (Control.CellBorderStyle)
				{
					case System.Windows.Forms.DataGridViewCellBorderStyle.Single:
						return GridLines.Both;
					case System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal:
						return GridLines.Horizontal;
					case System.Windows.Forms.DataGridViewCellBorderStyle.SingleVertical:
						return GridLines.Vertical;
					default:
						return GridLines.None;
				}
			}
			set
			{
				switch (value)
				{
					case GridLines.None:
						Control.CellBorderStyle = swf.DataGridViewCellBorderStyle.None;
						break;
					case GridLines.Horizontal:
						Control.CellBorderStyle = swf.DataGridViewCellBorderStyle.SingleHorizontal;
						break;
					case GridLines.Vertical:
						Control.CellBorderStyle = swf.DataGridViewCellBorderStyle.SingleVertical;
						break;
					case GridLines.Both:
						Control.CellBorderStyle = swf.DataGridViewCellBorderStyle.Single;
						break;
					default:
						throw new NotSupportedException();
				}
			}
		}

		public BorderType Border
		{
			get { return Control.BorderStyle.ToEto(); }
			set { Control.BorderStyle = value.ToSWF(); }
		}

		public void ReloadData(IEnumerable<int> rows)
		{
			Control.Refresh();
		}

		public bool IsEditing => Control.IsCurrentCellInEditMode;

		public bool AllowEmptySelection { get; set; } = true;

		protected void EnsureSelection()
		{
			if (!AllowEmptySelection && Control.RowCount > 0 && Control.SelectedRows.Count == 0)
			{
				SelectRow(0);
			}
		}
	}
}

