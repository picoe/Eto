using System;
using MonoMac.AppKit;
using Eto.Forms;
using System.Collections.Generic;
using Eto.Mac.Forms.Menu;
using System.Linq;
using Eto.Drawing;
using Eto.Mac.Drawing;
using MonoMac.ObjCRuntime;
using sd = System.Drawing;

namespace Eto.Mac.Forms.Controls
{
	public interface IGridHandler
	{
		Grid Widget { get; }

		NSTableView Table { get; }
	}

	class EtoGridScrollView : NSScrollView
	{
		WeakReference handler;

		public IGridHandler Handler { get { return (IGridHandler)handler.Target; } set { handler = new WeakReference(value); } }

		bool autoSized;

		public override void SetFrameSize(sd.SizeF newSize)
		{
			base.SetFrameSize(newSize);
			if (!autoSized && Handler.Widget.Loaded)
			{
				var rect = Handler.Table.VisibleRect();
				if (!rect.IsEmpty)
				{
					foreach (var col in Handler.Widget.Columns)
					{
						((GridColumnHandler)col.Handler).Resize();
					}
					autoSized = true;
				}
			}
		}
	}

	class EtoTableHeaderView : NSTableHeaderView
	{
		WeakReference handler;

		public IGridHandler Handler { get { return (IGridHandler)handler.Target; } set { handler = new WeakReference(value); } }

		static readonly Selector selConvertPointFromBacking = new Selector("convertPointFromBacking:");

		public override void MouseDown(NSEvent theEvent)
		{
			var point = theEvent.LocationInWindow;
			if (RespondsToSelector(selConvertPointFromBacking))
				point = ConvertPointFromBacking(point);
			else
				point = ConvertPointFromBase(point);
			var col = GetColumn(point);
			if (col >= 0)
			{
				var column = Handler.Widget.Columns[col];
				if (!column.Sortable)
					return;
			}
			base.MouseDown(theEvent);
		}
	}

	class MacCellFormatArgs : GridCellFormatEventArgs
	{
		Font font;

		public ICellHandler CellHandler { get { return Column.DataCell.Handler as ICellHandler; } }

		public NSCell Cell { get; private set; }

		public MacCellFormatArgs(GridColumn column, object item, int row, NSCell cell)
			: base(column, item, row)
		{
			this.Cell = cell;
		}

		public override Font Font
		{
			get
			{
				return font ?? (font = new Font(new FontHandler(Cell.Font)));
			}
			set
			{
				font = value;
				Cell.Font = font != null ? ((FontHandler)font.Handler).Control : null;
			}
		}

		public override Color BackgroundColor
		{
			get { return CellHandler.GetBackgroundColor(Cell); }
			set { CellHandler.SetBackgroundColor(Cell, value); }
		}

		public override Color ForegroundColor
		{
			get { return CellHandler.GetForegroundColor(Cell); }
			set { CellHandler.SetForegroundColor(Cell, value); }
		}
	}

	public abstract class GridHandler<TControl, TWidget, TCallback> : MacControl<TControl, TWidget, TCallback>, Grid.IHandler, IDataViewHandler, IGridHandler
		where TControl: NSTableView
		where TWidget: Grid
		where TCallback: Grid.ICallback
	{
		ColumnCollection columns;
		ContextMenu contextMenu;

		public NSTableView Table
		{
			get { return Control; }
		}

		public NSScrollView ScrollView { get; private set; }

		public override NSView ContainerControl { get { return ScrollView; } }

		protected virtual void PreUpdateColumn(int index)
		{
		}

		protected virtual void UpdateColumns()
		{
		}

		protected void UpdateColumnSizes()
		{
			if (Widget.Loaded)
			{
				var rect = Table.VisibleRect();
				if (!rect.IsEmpty)
				{
					foreach (var col in Widget.Columns)
					{
						((GridColumnHandler)col.Handler).Resize();
					}
				}
			}
		}

		public GridColumnHandler GetColumn(NSTableColumn tableColumn)
		{
			var str = tableColumn.Identifier;
			if (!string.IsNullOrEmpty(str))
			{
				int col;
				if (int.TryParse(str, out col))
				{
					return GetColumn(col);
				}
			}
			return null;
		}

		public GridColumnHandler GetColumn(int column)
		{
			return Widget.Columns[column].Handler as GridColumnHandler;
			//return Widget.Columns.Select (r => r.Handler as GridColumnHandler).First (r => r.Column == column);
		}

		class ColumnCollection : EnumerableChangedHandler<GridColumn, GridColumnCollection>
		{
			public GridHandler<TControl,TWidget,TCallback> Handler { get; set; }

			public override void AddItem(GridColumn item)
			{
				var colhandler = (GridColumnHandler)item.Handler;
				Handler.Control.AddColumn(colhandler.Control);
				colhandler.Setup(Handler.Control.ColumnCount - 1);
				
				Handler.UpdateColumns();
			}

			public override void InsertItem(int index, GridColumn item)
			{
				var outline = Handler.Control;
				var columns = new List<NSTableColumn>(outline.TableColumns());
				Handler.PreUpdateColumn(index);
				for (int i = index; i < columns.Count; i++)
				{
					outline.RemoveColumn(columns[i]);
				}
				var colhandler = (GridColumnHandler)item.Handler;
				columns.Insert(index, colhandler.Control);
				outline.AddColumn(colhandler.Control);
				colhandler.Setup(index);
				for (int i = index + 1; i < columns.Count; i++)
				{
					var col = columns[i];
					var colHandler = Handler.GetColumn(i);
					colHandler.Setup(i);
					outline.AddColumn(col);
				}
				Handler.UpdateColumns();
			}

			public override void RemoveItem(int index)
			{
				var outline = Handler.Control;
				var columns = new List<NSTableColumn>(outline.TableColumns());
				Handler.PreUpdateColumn(index);
				for (int i = index; i < columns.Count; i++)
				{
					outline.RemoveColumn(columns[i]);
				}
				columns.RemoveAt(index);
				for (int i = index; i < columns.Count; i++)
				{
					var col = columns[i];
					var colHandler = Handler.GetColumn(i);
					colHandler.Setup(i);
					outline.AddColumn(col);
				}
				Handler.UpdateColumns();
			}

			public override void RemoveAllItems()
			{
				Handler.PreUpdateColumn(0);
				foreach (var col in Handler.Control.TableColumns ())
					Handler.Control.RemoveColumn(col);
				Handler.UpdateColumns();
			}
		}

		protected GridHandler()
		{
			ScrollView = new EtoGridScrollView
			{
				Handler = this,
				HasVerticalScroller = true,
				HasHorizontalScroller = true,
				AutohidesScrollers = true,
				BorderType = NSBorderType.BezelBorder,
			};
			ScrollView.ContentView.PostsBoundsChangedNotifications = true;
			this.AddObserver(NSView.BoundsChangedNotification, HandleScrolled, ScrollView.ContentView);
		}

		static void HandleScrolled(ObserverActionEventArgs e)
		{
			var handler = (GridHandler<TControl,TWidget,TCallback>)e.Handler;
			handler.UpdateColumnSizes();
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
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

			Control.HeaderView = new EtoTableHeaderView { Handler = this };
			ScrollView.DocumentView = Control;
		}

		public override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			
			int i = 0;
			foreach (var col in Widget.Columns)
			{
				var colHandler = (GridColumnHandler)col.Handler;
				colHandler.Loaded(this, i++);
				colHandler.Resize(true);
			}
		}

		public void ResizeAllColumns()
		{
			foreach (var col in Widget.Columns.Select (r => r.Handler as GridColumnHandler))
			{
				col.Resize();
			}
		}

		public bool ShowHeader
		{
			get
			{
				return Control.HeaderView != null;
			}
			set
			{
				if (value && Control.HeaderView == null)
				{
					Control.HeaderView = new EtoTableHeaderView { Handler = this };
				}
				else if (!value && Control.HeaderView != null)
				{
					Control.HeaderView = null;
				}
			}
		}

		public bool AllowColumnReordering
		{
			get { return Control.AllowsColumnReordering; }
			set { Control.AllowsColumnReordering = value; }
		}

		public ContextMenu ContextMenu
		{
			get { return contextMenu; }
			set
			{
				contextMenu = value;
				Control.Menu = contextMenu != null ? ((ContextMenuHandler)contextMenu.Handler).Control : null;
			}
		}

		public bool AllowMultipleSelection
		{
			get { return Control.AllowsMultipleSelection; }
			set { Control.AllowsMultipleSelection = value; }
		}

		public IEnumerable<int> SelectedRows
		{
			get
			{ 
				if (Control.SelectedRows != null && Control.SelectedRows.Count > 0)
					return Control.SelectedRows.Select(r => (int)r);
				return Enumerable.Empty<int>();
			}
		}

		public void SelectAll()
		{
			Control.SelectAll(Control);
		}

		public void SelectRow(int row)
		{
			Control.SelectRow(row, false);
		}

		public void UnselectRow(int row)
		{
			Control.DeselectRow(row);
		}

		public void UnselectAll()
		{
			Control.DeselectAll(Control);
		}

		public int RowHeight
		{
			get { return (int)Control.RowHeight; }
			set { Control.RowHeight = value; }
		}

		public abstract object GetItem(int row);

		public virtual int RowCount
		{
			get { return Control.RowCount; }
		}

		Grid IGridHandler.Widget
		{
			get { return Widget; }
		}

		public sd.RectangleF GetVisibleRect()
		{
			var rect = ScrollView.VisibleRect();
			var loc = ScrollView.ContentView.Bounds.Location;
			rect.Offset(loc);
			return rect;
		}

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			var width = Widget.Columns.Sum(r => r.Width);
			if (width == 0)
				width = 100;
			var height = RowHeight * 4;
			return new Size(width, height);
		}

		public void OnCellFormatting(GridColumn column, object item, int row, NSCell cell)
		{
			Callback.OnCellFormatting(Widget, new MacCellFormatArgs(column, item, row, cell));
		}
	}
}

