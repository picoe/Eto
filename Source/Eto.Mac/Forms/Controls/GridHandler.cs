using System;
using Eto.Forms;
using System.Collections.Generic;
using Eto.Mac.Forms.Menu;
using System.Linq;
using Eto.Drawing;
using Eto.Mac.Drawing;
using Eto.Mac.Forms.Cells;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#if Mac64
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#if SDCOMPAT
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
#endif
#endif

#if XAMMAC
using nnint = System.Int32;
#elif Mac64
using nnint = System.UInt64;
#else
using nnint = System.UInt32;
#endif

namespace Eto.Mac.Forms.Controls
{
	public interface IGridHandler
	{
		Grid Widget { get; }

		NSTableView Table { get; }

		bool AutoSizeColumns();
	}

	class EtoGridScrollView : NSScrollView, IMacControl
	{
		public IGridHandler Handler { get { return (IGridHandler)WeakHandler.Target; } set { WeakHandler = new WeakReference(value); } }

		public WeakReference WeakHandler { get; set; }

		bool autoSized;

		public override void SetFrameSize(CGSize newSize)
		{
			base.SetFrameSize(newSize);

			if (!autoSized)
			{
				autoSized = Handler.AutoSizeColumns();
			}
		}
	}

	class EtoTableHeaderView : NSTableHeaderView
	{
		WeakReference handler;

		public IGridHandler Handler { get { return (IGridHandler)handler.Target; } set { handler = new WeakReference(value); } }


		public override void MouseDown(NSEvent theEvent)
		{
			if (!Handler.Table.AllowsColumnReordering)
			{
				var point = ConvertPointFromView(theEvent.LocationInWindow, null);

				var col = GetColumn(point);
				if (col >= 0)
				{
					var column = Handler.Widget.Columns[(int)col];
					var rect = Handler.Table.RectForColumn(col);
					if (!column.Sortable && point.X < rect.Right - 4 && point.X > rect.Left + 2)
						return;
				}
			}
			base.MouseDown(theEvent);
		}
	}

	class MacCellFormatArgs : GridCellFormatEventArgs
	{
		public ICellHandler CellHandler { get { return Column.DataCell.Handler as ICellHandler; } }

		public NSView View { get; private set; }

		public MacCellFormatArgs(GridColumn column, object item, int row, NSView view)
			: base(column, item, row)
		{
			View = view;
		}

		public bool FontSet { get; set; }

		Font font;

		public override Font Font
		{
			get { return font ?? (font = CellHandler.GetFont(View)); }
			set
			{ 
				if (!ReferenceEquals(font, value))
				{
					font = value;
					CellHandler.SetFont(View, value);
					FontSet = true;
				}
			}
		}

		public override Color BackgroundColor
		{
			get { return CellHandler.GetBackgroundColor(View); }
			set { CellHandler.SetBackgroundColor(View, value); }
		}

		public override Color ForegroundColor
		{
			get { return CellHandler.GetForegroundColor(View); }
			set { CellHandler.SetForegroundColor(View, value); }
		}
	}

	public abstract class GridHandler<TControl, TWidget, TCallback> : MacControl<TControl, TWidget, TCallback>, Grid.IHandler, IDataViewHandler, IGridHandler
		where TControl: NSTableView
		where TWidget: Grid
		where TCallback: Grid.ICallback
	{
		ColumnCollection columns;
		ContextMenu contextMenu;

		protected int SuppressUpdate { get; set; }

		bool IDataViewHandler.SuppressUpdate => SuppressUpdate > 0;

		protected int SuppressSelectionChanged { get; set; }

		protected bool IsAutoSizingColumns { get; private set; }

		public NSTableView Table
		{
			get { return Control; }
		}

		protected IEnumerable<IDataColumnHandler> ColumnHandlers
		{
			get { return Widget.Columns.Select(r => r.Handler).OfType<IDataColumnHandler>(); }
		}

		public NSScrollView ScrollView { get; private set; }

		public override NSView ContainerControl { get { return ScrollView; } }

		protected virtual void PreUpdateColumn(int index)
		{
		}

		protected virtual void UpdateColumns()
		{
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
				colhandler.Setup(Handler, (int)(Handler.Control.ColumnCount - 1));
				
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
				colhandler.Setup(Handler, index);
				for (int i = index + 1; i < columns.Count; i++)
				{
					var col = columns[i];
					var colHandler = Handler.GetColumn(i);
					colHandler.Setup(Handler, i);
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
					colHandler.Setup(Handler, i);
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
			handler.AutoSizeColumns();
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Grid.CellDoubleClickEvent:
					Control.DoubleClick += (sender, e) =>
					{
						int rowIndex = (int)Control.ClickedRow;
						if (rowIndex >= 0)
						{
							var columnIndex = (int)Control.ClickedColumn;
							var item = GetItem(rowIndex);
							var column = columnIndex == -1 ? null : Widget.Columns[columnIndex];
							var cellArgs = MacConversions.CreateCellEventArgs(column, Control, rowIndex, columnIndex, item);
						}
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
			Control.AllowsColumnReordering = false;
			columns = new ColumnCollection { Handler = this };
			columns.Register(Widget.Columns);

			Control.HeaderView = new EtoTableHeaderView { Handler = this };
			ScrollView.DocumentView = Control;
		}

		public override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);

			if (!Widget.Properties.Get<bool>(ScrolledToRowKey))
				// Yosemite bug: hides first row when DataStore is set before control is visible
				Control.ScrollRowToVisible(0);
			else
				Widget.Properties.Remove(ScrolledToRowKey);

			IsAutoSizingColumns = true;
			foreach (var col in ColumnHandlers)
			{
				col.Resize(true);
			}
			IsAutoSizingColumns = false;
		}

		NSRange autoSizeRange;

		public bool AutoSizeColumns()
		{
			if (Widget.Loaded)
			{
				var rect = Table.VisibleRect();
				var newRange = Table.RowsInRect(rect);
				if (newRange.Length > 0 && (autoSizeRange.Location != newRange.Location || autoSizeRange.Length != newRange.Length))
				{
					IsAutoSizingColumns = true;
					foreach (var col in ColumnHandlers)
					{
						col.AutoSizeColumn(newRange);
					}
					autoSizeRange = newRange;
					IsAutoSizingColumns = false;
					return true;
				}
			}
			return false;
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
				var rows = Control.SelectedRows;
				if (rows != null && rows.Count > 0)
					return rows.Select(r => (int)r);
				return Enumerable.Empty<int>();
			}
			set
			{
				SuppressSelectionChanged++;
				UnselectAll();
				if (value != null)
				{
					var indexes = NSIndexSet.FromArray(value.ToArray());
					Control.SelectRows(indexes, AllowMultipleSelection);
				}
				SuppressSelectionChanged--;
				if (SuppressSelectionChanged == 0)
					Callback.OnSelectionChanged(Widget, EventArgs.Empty);
			}
		}

		public BorderType Border
		{
			get { return ScrollView.BorderType.ToEto(); }
			set
			{
				ScrollView.BorderType = value.ToNS();
				LayoutIfNeeded();
			}
		}

		public void SelectAll()
		{
			Control.SelectAll(Control);
		}

		public void SelectRow(int row)
		{
			Control.SelectRow((nnint)row, AllowMultipleSelection);
		}

		public void UnselectRow(int row)
		{
			Control.DeselectRow(row);
		}

		public void UnselectAll()
		{
			Control.DeselectAll(Control);
		}

		public void BeginEdit(int row, int column)
		{
			Control.SelectRow((nnint)row, false);
			Control.EditColumn((nint)column, (nint)row, new NSEvent(), true);
		}

		public bool CommitEdit() => SetFocusToControl();

		public bool CancelEdit()
		{
			SuppressUpdate++;
			var ret = SetFocusToControl();
			SuppressUpdate--;
			return ret;
		}

		bool SetFocusToControl()
		{
			var firstResponder = Control.Window?.FirstResponder as NSView;
			while (firstResponder != null)
			{
				if (firstResponder == Control)
				{
					Control.Window.MakeFirstResponder(Control);
					return true;
				}
				firstResponder = firstResponder.Superview;
			}
			return true; // always true for now, no way to suppress cancelling or committing edit.
		}

		public int RowHeight
		{
			get { return (int)Control.RowHeight; }
			set { Control.RowHeight = value; }
		}

		public abstract object GetItem(int row);

		public virtual int RowCount
		{
			get { return (int)Control.RowCount; }
		}

		public override bool Enabled
		{
			get { return base.Enabled; }
			set
			{
				base.Enabled = value;
				foreach (var ctl in ColumnHandlers)
				{
					ctl.EnabledChanged(value);
				}
			}
		}

		Grid IGridHandler.Widget
		{
			get { return Widget; }
		}

		public CGRect GetVisibleRect()
		{
			var rect = ScrollView.VisibleRect();
			var loc = ScrollView.ContentView.Bounds.Location;
			return new CGRect(rect.X + loc.X, rect.Y + loc.Y, rect.Width, rect.Height);
		}

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			var width = Widget.Columns.Sum(r => r.Width);
			if (width == 0)
				width = 100;
			var height = RowHeight * 4;
			return new Size(width, height);
		}

		public void OnCellFormatting(GridCellFormatEventArgs args)
		{
			Callback.OnCellFormatting(Widget, args);
		}

		static readonly object ScrolledToRowKey = new object();

		public void ScrollToRow(int row)
		{
			Control.ScrollRowToVisible(row);
			if (!Widget.Loaded)
				Widget.Properties[ScrolledToRowKey] = true;
		}

		public bool Loaded
		{
			get { return Widget.Loaded; }
		}

		public GridLines GridLines
		{
			get
			{
				var lines = GridLines.None;
				if (Control.GridStyleMask.HasFlag(NSTableViewGridStyle.SolidHorizontalLine))
					lines |= GridLines.Horizontal;
				if (Control.GridStyleMask.HasFlag(NSTableViewGridStyle.SolidVerticalLine))
					lines |= GridLines.Vertical;
				return lines;
			}
			set
			{
				var mask = NSTableViewGridStyle.None;
				if (value.HasFlag(GridLines.Horizontal))
					mask |= NSTableViewGridStyle.SolidHorizontalLine;
				if (value.HasFlag(GridLines.Vertical))
					mask |= NSTableViewGridStyle.SolidVerticalLine;
				Control.GridStyleMask = mask;
			}
		}

		Grid.ICallback IDataViewHandler.Callback
		{
			get { return Callback; }
		}

		Grid IDataViewHandler.Widget
		{
			get { return Widget; }
		}
	}
}

