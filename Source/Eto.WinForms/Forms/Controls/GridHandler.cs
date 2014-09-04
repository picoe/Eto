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
	}

	public abstract class GridHandler<TWidget, TCallback> : WindowsControl<swf.DataGridView, TWidget, TCallback>, Grid.IHandler, IGridHandler
		where TWidget: Grid
		where TCallback: Grid.ICallback
	{
		ColumnCollection columns;
		bool isFirstSelection = true;
		protected int SupressSelectionChanged { get; set; }
		protected bool clearColumns;

		protected void ResetSelection()
		{
			if (!SelectedRows.Any())
				isFirstSelection = true;
		}

		protected abstract object GetItemAtRow(int row);

		class EtoDataGridView : swf.DataGridView
		{
			public GridHandler<TWidget, TCallback> Handler { get; set; }

			public override sd.Size GetPreferredSize(sd.Size proposedSize)
			{
				var size = base.GetPreferredSize(proposedSize);
				var def = Handler.UserDesiredSize;
				if (def.Width >= 0)
					size.Width = def.Width;
				if (def.Height >= 0)
					size.Height = def.Height;
				else
					size.Height = Math.Min(size.Height, 100);
				return size;
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
			set { Control.BackgroundColor = value.ToSD(); }
		}

		public override void OnUnLoad(EventArgs e)
		{
			base.OnUnLoad(e);
			LeakHelper.UnhookObject(Control);
		}

		bool handledAutoSize;

		void HandleRowPostPaint(object sender, swf.DataGridViewRowPostPaintEventArgs e)
		{
			if (handledAutoSize)
				return;

			handledAutoSize = true;
			int colNum = 0;
			foreach (var col in Widget.Columns)
			{
				var colHandler = col.Handler as GridColumnHandler;
				if (col.AutoSize)
				{
					Control.AutoResizeColumn(colNum, colHandler.Control.InheritedAutoSizeMode);
					var width = col.Width;
					colHandler.Control.AutoSizeMode = swf.DataGridViewAutoSizeColumnMode.None;
					col.Width = width;
				}
				colNum++;
			}
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
					Control.ColumnHeaderMouseClick += (sender, e) => Callback.OnColumnHeaderClick(Widget, new GridColumnEventArgs(Widget.Columns[e.ColumnIndex]));
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
				default:
					base.AttachEvent(id);
					break;
			}
		}

		protected override void Initialize()
		{
			base.Initialize();
			columns = new ColumnCollection { Handler = this };
			columns.Register(Widget.Columns);
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
		}

		public void SelectRow(int row)
		{
			Control.Rows[row].Selected = true;
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
	}
}

