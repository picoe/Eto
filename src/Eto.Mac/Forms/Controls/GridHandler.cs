using Eto.Mac.Forms.Menu;
using Eto.Mac.Drawing;
using Eto.Mac.Forms.Cells;

namespace Eto.Mac.Forms.Controls
{
	public interface IGridHandler : IMacViewHandler
	{
		new Grid Widget { get; }
		NSTableView Table { get; }
		bool AutoSizeColumns(bool force, bool forceNewSize = false);
		void PerformLayout();
		void ResetAutoSizedColumns();
	}

	class EtoGridScrollView : NSScrollView, IMacControl
	{
		public IGridHandler Handler { get { return (IGridHandler)WeakHandler.Target; } set { WeakHandler = new WeakReference(value); } }

		public WeakReference WeakHandler { get; set; }
	}

	static class GridHandler
	{
		public static readonly object ScrolledToRow_Key = new object();
		public static readonly object IsEditing_Key = new object();
		public static readonly object IsMouseDragging_Key = new object();
		public static readonly object ContextMenu_Key = new object();
		public static readonly object IsCancelEdit_Key = new object();
	}

	class EtoTableHeaderView : NSTableHeaderView, IMacControl
	{
		public IGridHandler Handler { get { return (IGridHandler)WeakHandler.Target; } set { WeakHandler = new WeakReference(value); } }

		public WeakReference WeakHandler { get; set; }

		public EtoTableHeaderView()
		{
		}

		public EtoTableHeaderView(IntPtr handle) : base(handle)
		{
		}
		
		public override void MouseDown(NSEvent theEvent)
		{
			var h = Handler;
			if (h == null)
			{
				base.MouseDown(theEvent);
				return;
			}

			var sel = MacView.selMouseDown;
			if (!h.Table.AllowsColumnReordering)
			{
				var point = ConvertPointFromView(theEvent.LocationInWindow, null);

				var col = GetColumn(point);
				if (col >= 0)
				{
					var column = h.Widget.Columns[(int)col];
					var rect = h.Table.RectForColumn(col);
					// don't show any feedback to user when they click
					if (!column.Sortable && point.X < rect.Right - 4 && point.X > rect.Left + 2)
						sel = IntPtr.Zero;
				}
			}
			h.TriggerMouseDown(this, sel, theEvent);
		}
	}
	
	class EtoBackgroundView : NSView
	{
		[Export("backgroundColor")]
		public NSColor BackgroundColor { get; set; }
		
		public EtoBackgroundView()
		{
		}
		
		public EtoBackgroundView(NativeHandle handle) : base(handle)
		{
		}

		public override void DrawRect(CGRect dirtyRect)
		{
			BackgroundColor.SetFill();
			NSBezierPath.FillRect(Bounds);
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

	class GridDragInfo
	{
		public NSDragOperation AllowedOperation { get; set; }
		public NSImage DragImage { get; set; }
		public PointF ImageOffset { get; set; }
		
		public DataObject Data { get; set; }

		public CGPoint GetDragImageOffset()
		{
			var size = DragImage.Size;
			return new CGPoint(size.Width / 2 - ImageOffset.X, ImageOffset.Y - size.Height / 2);
		}
	}

	public abstract class GridHandler<TControl, TWidget, TCallback> : MacControl<TControl, TWidget, TCallback>, Grid.IHandler, IDataViewHandler, IGridHandler
		where TControl : NSTableView
		where TWidget : Grid
		where TCallback : Grid.ICallback
	{
		ColumnCollection columns;
		bool loadComplete;

		public override NSView DragControl => Control;

		public bool AllowEmptySelection
		{
			get => Control.AllowsEmptySelection;
			set => Control.AllowsEmptySelection = value;
		}

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

		protected virtual void UpdateColumns()
		{
			foreach (var col in ColumnHandlers)
			{
				col.SetupDisplayIndex();
			}
		}

		public GridColumnHandler GetColumn(NSTableColumn tableColumn)
		{
			if (tableColumn == null)
				return null;
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
			public GridHandler<TControl, TWidget, TCallback> Handler { get; set; }

			public override void AddItem(GridColumn item)
			{
				var colhandler = (GridColumnHandler)item.Handler;
				Handler.Control.AddColumn(colhandler.Control);
				colhandler.Setup(Handler, (int)(Handler.Control.ColumnCount - 1));

				if (Handler.Loaded)
					Handler.UpdateColumns();
			}

			public override void InsertItem(int index, GridColumn item)
			{
				var outline = Handler.Control;
				var columns = new List<NSTableColumn>(outline.TableColumns());
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
				if (Handler.Loaded)
					Handler.UpdateColumns();
			}

			public override void RemoveItem(int index)
			{
				var outline = Handler.Control;
				var columns = new List<NSTableColumn>(outline.TableColumns());
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
				if (Handler.Loaded)
					Handler.UpdateColumns();
			}

			public override void RemoveAllItems()
			{
				foreach (var col in Handler.Control.TableColumns())
					Handler.Control.RemoveColumn(col);
				if (Handler.Loaded)
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
			var handler = (GridHandler<TControl, TWidget, TCallback>)e.Handler;
			if (handler == null)
				return;
			if (handler.hasAutoSizedColumns == true)
				handler.AutoSizeColumns(false);
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Grid.ColumnWidthChangedEvent:
					// handled in delegates
					break;
				case Eto.Forms.Control.MouseDownEvent:
					AddMethod(MacView.selMouseDown, MacView.TriggerMouseDown_Delegate, "v@:@", typeof(EtoTableHeaderView));
					AddMethod(MacView.selRightMouseDown, MacView.TriggerMouseDown_Delegate, "v@:@", typeof(EtoTableHeaderView));
					AddMethod(MacView.selOtherMouseDown, MacView.TriggerMouseDown_Delegate, "v@:@", typeof(EtoTableHeaderView));
					base.AttachEvent(id);
					break;
				case Eto.Forms.Control.MouseUpEvent:
					AddMethod(MacView.selMouseUp, MacView.TriggerMouseUp_Delegate, "v@:@", typeof(EtoTableHeaderView));
					AddMethod(MacView.selRightMouseUp, MacView.TriggerMouseUp_Delegate, "v@:@", typeof(EtoTableHeaderView));
					AddMethod(MacView.selOtherMouseUp, MacView.TriggerMouseUp_Delegate, "v@:@", typeof(EtoTableHeaderView));
					base.AttachEvent(id);
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		EtoTableHeaderView headerView;

		protected override void Initialize()
		{
			base.Initialize();
			Control.AllowsColumnReordering = false;
			columns = new ColumnCollection { Handler = this };
			columns.Register(Widget.Columns);

			Control.HeaderView = headerView = new EtoTableHeaderView { Handler = this };
			ScrollView.DocumentView = Control;
		}

		public override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			ResetAutoSizedColumns();
			UpdateColumns();
		}

		public override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);

			var row = Widget.Properties.Get<int>(GridHandler.ScrolledToRow_Key, 0);
			// Yosemite bug: hides first row when DataStore is set before control is visible, so we always call this
			Control.ScrollRowToVisible(row);
			loadComplete = true;
		}

		public override void OnUnLoad(EventArgs e)
		{
			base.OnUnLoad(e);
			loadComplete = false;
		}

		NSRange autoSizeRange;

		static Lazy<bool> supportsTableStyle = new Lazy<bool>(() => ObjCExtensions.InstancesRespondToSelector<NSTableView>(Selector.GetHandle("effectiveStyle")));
		
		nfloat GetTableRowInsets()
		{
			// oh I love magic numbers, but there doesn't seem to be any APIs that will return these..
			// https://developer.apple.com/documentation/macos-release-notes/appkit-release-notes-for-macos-12
			if (supportsTableStyle.Value)
			{
				switch (Control.EffectiveStyle)
				{
					case NSTableViewStyle.Inset:
						return 32; 
					case NSTableViewStyle.FullWidth:
						return 12;
					case NSTableViewStyle.SourceList:
						return 32;
					case NSTableViewStyle.Plain:
					default:
						return Control.IntercellSpacing.Width;
				}
			}
			else
			{
				return Control.IntercellSpacing.Width;
			}
		}

		int lastAutoSizeWidth;

		public bool AutoSizeColumns(bool force, bool forceNewSize = false)
		{
			if (!Widget.Loaded)
				return false;
				
			var rect = Table.VisibleRect();
			if (rect.Width <= 0)
				return false;

			bool resizeExpanded = forceNewSize;
			bool changed = false;
			var newRange = rect.IsEmpty ? null : (NSRange?)Table.RowsInRect(rect);

			if (lastAutoSizeWidth != (int)rect.Width)
			{
				resizeExpanded = true;
				lastAutoSizeWidth = (int)rect.Width;
			}
			
			if (force 
				|| newRange == null 
				|| (autoSizeRange.Location != newRange.Value.Location || autoSizeRange.Length != newRange.Value.Length))
			{
				IsAutoSizingColumns = true;
				
				int expandCount = 0;
				nfloat requiredWidth = 0;
				nfloat expandedWidth = 0;
				
				// remove all spacing that isn't part of column widths
				var intercellSpacingWidth = Table.IntercellSpacing.Width;
				rect.Width -= intercellSpacingWidth * (Table.ColumnCount - 1);
				rect.Width -= GetTableRowInsets();

				foreach (var col in ColumnHandlers)
				{
					changed |= col.AutoSizeColumn(newRange, forceNewSize && !col.Expand);

					if (col.Expand)
					{
						expandCount++;
						expandedWidth += col.Control.Width;
					}
					else
					{
						requiredWidth += col.Control.Width;
					}
				}
				
				resizeExpanded |= changed;
				
				if (expandCount > 0 && resizeExpanded)
				{
					var remaining = (nfloat)Math.Max(0, rect.Width - requiredWidth);
					// System.Diagnostics.Debug.WriteLine($"Remaining: {remaining}, Required: {requiredWidth}, Width: {rect.Width}");
					if (remaining > 0)
					{
						var each = remaining / expandCount;
						
						foreach (var col in ColumnHandlers)
						{
							if (col.Expand)
							{
								var existingWidth = col.Control.Width;
								var weightedWidth = expandedWidth > 0 ? existingWidth / expandedWidth * remaining : each;

								changed |= existingWidth != weightedWidth;

								col.Control.Width = weightedWidth;
							}
						}
					}
				}

				if (newRange != null)
					autoSizeRange = newRange.Value;
					
				IsAutoSizingColumns = false;
				
				if (forceNewSize && changed)
				{
					InvalidateMeasure();
				}
				return true;
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
					headerView = new EtoTableHeaderView { Handler = this, Menu = ContextMenu.ToNS() };
					Control.HeaderView = headerView;
				}
				else if (!value && Control.HeaderView != null)
				{
					Control.HeaderView = headerView = null;
				}
			}
		}

		public bool AllowColumnReordering
		{
			get { return Control.AllowsColumnReordering; }
			set { Control.AllowsColumnReordering = value; }
		}

		public virtual ContextMenu ContextMenu
		{
			get { return Widget.Properties.Get<ContextMenu>(GridHandler.ContextMenu_Key); }
			set
			{
				Widget.Properties.Set(GridHandler.ContextMenu_Key, value);
				Control.Menu = value.ToNS();
				if (Control.HeaderView != null)
					Control.HeaderView.Menu = value.ToNS();
			}
		}

		public bool AllowMultipleSelection
		{
			get { return Control.AllowsMultipleSelection; }
			set { Control.AllowsMultipleSelection = value; }
		}

		public virtual IEnumerable<int> SelectedRows
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
					var rows = value.ToArray();
					if (rows.Length > 0)
					{
						var indexes = NSIndexSet.FromArray(rows);
						Control.SelectRows(indexes, AllowMultipleSelection);
						ScrollToRow(rows[0]);
					}
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
				InvalidateMeasure();
			}
		}

		public void SelectAll()
		{
			Control.SelectAll(Control);
		}

		public void SelectRow(int row)
		{
			Control.SelectRow(row, AllowMultipleSelection);
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
			if (!Control.IsRowSelected(row))
			{
				Control.SelectRow((nint)row, false);
			}
			Control.EditColumn((nint)column, (nint)row, new NSEvent(), true);
		}

		public bool CommitEdit() => SetFocusToControl();

		public bool CancelEdit()
		{
			if (IsEditing)
			{
				SuppressUpdate++;
				IsCancellingEdit = true;
				var ret = SetFocusToControl();
				IsCancellingEdit = false;
				SuppressUpdate--;
				return ret;
			}
			return true;
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

		public virtual int RowCount => (int)Control.RowCount;

		protected override bool ControlEnabled
		{
			get => base.ControlEnabled;
			set
			{
				base.ControlEnabled = value;
				foreach (var ctl in ColumnHandlers)
				{
					ctl.EnabledChanged(value);
				}
			}
		}

		Grid IGridHandler.Widget => Widget;

		public CGRect GetVisibleRect()
		{
			var rect = ScrollView.VisibleRect();
			var loc = ScrollView.ContentView.Bounds.Location;
			return new CGRect(rect.X + loc.X, rect.Y + loc.Y, rect.Width, rect.Height);
		}

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			var rowCount = RowCount;
			var range = new NSRange(0, rowCount);
			var loaded = loadComplete;
			var size = new SizeF();
			size.Width = (float)ColumnHandlers.Sum(r => loaded ? r.Control.Width : r.GetPreferredWidth(range));
			if (size.Width == 0)
				size.Width = 100;

			if (Border != BorderType.None)
				size += ScrollView.FrameSizeForContentSize(new CGSize(0, 0), false, false).ToEto();

			var intercellSpacing = Control.IntercellSpacing;
			size.Width += (Control.ColumnCount - 1) * (float)intercellSpacing.Width;
			size.Width += (float)GetTableRowInsets();

			var contentHeight = (float)((RowHeight + intercellSpacing.Height) * rowCount);
			if (ScrollView.HasVerticalScroller && ScrollView.VerticalScroller.ScrollerStyle == NSScrollerStyle.Legacy)
			{
				if (!float.IsNaN(availableSize.Height) && contentHeight > availableSize.Height)
					size.Width += (float)NSScroller.GetScrollerWidth(ScrollView.VerticalScroller.ControlSize, ScrollView.VerticalScroller.ScrollerStyle);
			}

			size.Height += contentHeight;

			if (ShowHeader)
				size.Height += 2 + (int)Control.HeaderView.Frame.Height;
			return size;
		}

		public void OnCellFormatting(GridCellFormatEventArgs args)
		{
			var tooltipBinding = args.Column?.CellToolTipBinding;
			if (tooltipBinding != null && args is MacCellFormatArgs macargs && macargs.View != null)
				macargs.View.ToolTip = tooltipBinding.GetValue(args.Item) ?? string.Empty;
			
			Callback.OnCellFormatting(Widget, args);
		}

		public void ScrollToRow(int row)
		{
			if (!Widget.Loaded)
				Widget.Properties[GridHandler.ScrolledToRow_Key] = row;
			else
				Control.ScrollRowToVisible(row);
		}

		public bool Loaded => Widget.Loaded;

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

		void IDataViewHandler.OnCellEditing(GridViewCellEventArgs e)
		{
			Callback.OnCellEditing(Widget, e);
			SetIsEditing(true);
		}

		public bool IsCancellingEdit
		{
			get => Widget.Properties.Get<bool>(GridHandler.IsCancelEdit_Key);
			set => Widget.Properties.Set(GridHandler.IsCancelEdit_Key, value);
		}

		void IDataViewHandler.OnCellEdited(GridViewCellEventArgs e)
		{
			SetIsEditing(false);
			if (e.Item != null && !IsCancellingEdit)
				Callback.OnCellEdited(Widget, e);

			// reload this entire row
			if (e.Row >= 0)
			{
				Control.ReloadData(NSIndexSet.FromIndex((nint)e.Row), NSIndexSet.FromNSRange(new NSRange(0, Control.ColumnCount)));
			}

			if (e.GridColumn.AutoSize)
			{
				AutoSizeColumns(true);
			}
		}

		Grid IDataViewHandler.Widget => Widget;

		protected void SetIsEditing(bool value) => Widget.Properties.Set(GridHandler.IsEditing_Key, value, false);

		bool? hasAutoSizedColumns;
		public void ResetAutoSizedColumns()
		{
			if (hasAutoSizedColumns != null)
				hasAutoSizedColumns = false;
		}

		void EnsureAutoSizedColumns()
		{
			AutoSizeColumns(true, hasAutoSizedColumns == null);
			hasAutoSizedColumns = true;
		}

		public void PerformLayout()
		{
			EnsureAutoSizedColumns();
		}

		public bool IsEditing => Widget.Properties.Get(GridHandler.IsEditing_Key, Control.EditedRow != -1 && Control.EditedColumn != -1);

		protected bool IsMouseDragging
		{
			get { return Widget.Properties.Get(GridHandler.IsMouseDragging_Key, false); }
			set { Widget.Properties.Set(GridHandler.IsMouseDragging_Key, value, false); }
		}

		static readonly object DragPasteboard_Key = new object();

		protected NSPasteboard DragPasteboard
		{
			get { return Widget.Properties.Get<NSPasteboard>(DragPasteboard_Key); }
			set { Widget.Properties.Set(DragPasteboard_Key, value); }
		}

		internal GridDragInfo DragInfo { get; set; }

		public override void DoDragDrop(DataObject data, DragEffects allowedAction, Image image, PointF origin)
		{
			if (DragPasteboard != null)
			{
				var handler = data.Handler as IDataObjectHandler;
				handler?.Apply(DragPasteboard);
				SetupDragPasteboard(DragPasteboard);
				DragInfo = new GridDragInfo
				{
					AllowedOperation = allowedAction.ToNS(),
					DragImage = image.ToNS(),
					ImageOffset = origin,
					Data = data
				};
			}
			else
			{
				base.DoDragDrop(data, allowedAction, image, origin);
			}
		}

		static readonly object DidSetAutoSizeColumn_Key = new object();

		internal bool DidSetAutoSizeColumn
		{
			get => Widget.Properties.Get<bool>(DidSetAutoSizeColumn_Key);
			set => Widget.Properties.Set(DidSetAutoSizeColumn_Key, value);
		}

		protected void ColumnDidResize(NSNotification notification)
		{
			var column = notification.UserInfo["NSTableColumn"] as NSTableColumn;
			var colHandler = GetColumn(column);
			if (!IsAutoSizingColumns && Widget.Loaded && hasAutoSizedColumns == true)
			{
				// when the user resizes the column, don't autosize anymore when data/scroll changes
				if (column != null)
				{
					if (!DidSetAutoSizeColumn)
						colHandler.AutoSize = false;
					InvalidateMeasure();
				}
			}
			if (colHandler != null)
				Callback.OnColumnWidthChanged(Widget, new GridColumnEventArgs(colHandler.Widget));
		}
		
		protected virtual bool HandleMouseEvent(NSEvent theEvent)
		{
			var args = MacConversions.GetMouseEvent(this, theEvent, false);
			if (theEvent.ClickCount >= 2)
			{
				Callback.OnMouseDoubleClick(Widget, args);
				if (args.Handled)
					return false;
			}
			else
			{
				Callback.OnMouseDown(Widget, args);
				if (args.Handled)
					return true;
			}

			var point = Control.ConvertPointFromView(theEvent.LocationInWindow, null);

			var rowIndex = (int)Control.GetRow(point);
			if (rowIndex >= 0)
			{
				var columnIndex = (int)Control.GetColumn(point);
				var item = GetItem(rowIndex);
				var column = columnIndex == -1 || columnIndex > Widget.Columns.Count ? null : Widget.Columns[columnIndex];
				var cellArgs = MacConversions.CreateCellMouseEventArgs(column, ContainerControl, rowIndex, columnIndex, item, theEvent);
				if (theEvent.ClickCount >= 2)
					Callback.OnCellDoubleClick(Widget, cellArgs);
				else
					Callback.OnCellClick(Widget, cellArgs);
				
				return cellArgs.Handled;
			}
			return false;
		}
		
		protected virtual bool ValidateProposedFirstResponder(NSResponder responder, NSEvent forEvent, bool valid)
		{
			if (valid || responder == null || forEvent == null)
				return valid;
				
			if (responder is NSView view)
			{
				// forward events for controls in custom cells, as long as it can't be a first responder, like a text box.
				var parentView = view;
				while (parentView != null && !(parentView is NSTableRowView))
				{
					if (parentView is CustomCellHandler.EtoCustomCellView)
					{
						return !view.AcceptsFirstResponder();
					}
					parentView = parentView.Superview;
				};
			}
			
			return false;
		}

		public int GetColumnDisplayIndex(GridColumn column)
		{
			if (column.Handler is GridColumnHandler handler)
				return (int)Control.FindColumn(new NSString(handler.Control.Identifier));
			return -1;
		}

		public void SetColumnDisplayIndex(GridColumn column, int index)
		{
			if (column.Handler is GridColumnHandler handler)
			{
				var fromIndex = Control.FindColumn(new NSString(handler.Control.Identifier));
				if (fromIndex != index)
					Control.MoveColumn(fromIndex, index);
			}
		}
		
		internal int DisplayIndexToColumnIndex(int displayIndex)
		{
			var col = Widget.Columns.FirstOrDefault(r => r.DisplayIndex == displayIndex);
			if (col == null)
				return -1;
			return Widget.Columns.IndexOf(col);
		}
		
		protected override void SetBackgroundColor(Color? color)
		{
			var bg = color?.ToNSUI() ?? NSColor.ControlBackground;
			Control.BackgroundColor = bg;
			if (!UseNSBoxBackgroundColor)
			{
				var clip = ScrollView.Subviews.OfType<NSClipView>().FirstOrDefault(r => r.DocumentView == headerView);
				var banner = clip?.Subviews.FirstOrDefault(r =>  r.Class.Name == "NSBannerView");
				var effectView = banner?.Subviews.OfType<NSVisualEffectView>().FirstOrDefault();
				if (effectView == null || banner == null)
					return;

				// inject a view above the effectView with our desired background color
				var backgroundView = banner.Subviews.OfType<EtoBackgroundView>().FirstOrDefault();
				if (backgroundView == null)
				{
					backgroundView = new EtoBackgroundView
					{
						Frame = banner.Bounds,
						AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable,
						TranslatesAutoresizingMaskIntoConstraints = true
					};
					banner.AddSubview(backgroundView, NSWindowOrderingMode.Above, effectView);
				}
				
				backgroundView.BackgroundColor = bg;
				backgroundView.SetNeedsDisplay();
			}
		}

		class RowFormatEventArgs : GridRowFormatEventArgs
		{
			NSTableRowView _rowView;
			public RowFormatEventArgs(NSTableRowView rowView, object item, int row) : base(item, row)
			{
				_rowView = rowView;
			}

			public override Color BackgroundColor
			{
				get => _rowView.BackgroundColor.ToEto();
				set => _rowView.BackgroundColor = value.ToNSUI();
			}
		}

		protected virtual void OnDidAddRowView(NSTableRowView rowView, nint row)
		{
			var item = GetItem((int)row);
			Callback.OnRowFormatting(Widget, new RowFormatEventArgs(rowView, item, (int)row));
		}
	}
}

