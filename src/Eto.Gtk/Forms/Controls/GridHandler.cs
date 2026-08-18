using Eto.GtkSharp.Forms.Cells;
using Eto.GtkSharp.Forms.Menu;

namespace Eto.GtkSharp.Forms.Controls
{
	class GridHandler
	{
		internal static readonly object Border_Key = new object();
		internal static readonly object RowDataColumn_Key = new object();
		internal static readonly object ItemDataColumn_Key = new object();
		internal static readonly object AllowColumnReordering_Key = new object();
		internal static readonly object AllowEmptySelection_Key = new object();
	}

	public abstract class GridHandler<TWidget, TCallback> : GtkControl<Gtk.TreeView, TWidget, TCallback>, Grid.IHandler, ICellDataSource, IGridHandler
		where TWidget : Grid
		where TCallback : Grid.ICallback
	{
		ColumnCollection columns;
		Gtk.TreeViewColumn spacingColumn;
		readonly Dictionary<int, (int column, GLib.GType type)> columnMap = new();

		protected bool SkipSelectedChange { get; set; }

		public Gtk.ScrolledWindow ScrolledWindow { get; private set; }
		public EtoEventBox Box { get; private set; }

		Gtk.TreeView IGridHandler.Tree => Control;

		protected Dictionary<int, (int column, GLib.GType type)> ColumnMap { get { return columnMap; } }

		public override Gtk.Widget ContainerControl => ScrolledWindow;

		public override bool ShouldTranslatePoints => true;


		class EtoScrolledWindow : Gtk.ScrolledWindow
		{
			WeakReference handler;
			public IGtkControl Handler { get => handler?.Target as IGtkControl; set => handler = new WeakReference(value); }

#if GTKCORE			

			protected override void OnGetPreferredWidth(out int minimum_width, out int natural_width)
			{
				base.OnGetPreferredWidth(out minimum_width, out natural_width);

				var h = Handler;
				if (h != null)
				{
					var size = h.UserPreferredSize;
					if (size.Width >= 0)
					{
						natural_width = Math.Min(size.Width, natural_width);
						minimum_width = Math.Min(size.Width, minimum_width);
					}
				}
			}

			protected override void OnGetPreferredHeight(out int minimum_height, out int natural_height)
			{
				base.OnGetPreferredHeight(out minimum_height, out natural_height);
				var h = Handler;

				// GtkTreeView under-reports its height until it has been mapped; make the natural height
				// account for all rows so the grid measures correctly before it is ever shown (both when
				// measured directly and when measured as part of a parent container).
				if (h is GridHandler<TWidget, TCallback> gridHandler)
					natural_height = Math.Max(natural_height, gridHandler.CalculateContentHeight());

				if (h != null)
				{
					var size = h.UserPreferredSize;
					if (size.Height >= 0)
					{
						natural_height = Math.Min(size.Height, natural_height);
						minimum_height = Math.Min(size.Height, minimum_height);
					}
				}
			}
#endif
		}

		protected GridHandler()
		{
			ScrolledWindow = new EtoScrolledWindow
			{
				Handler = this,
				ShadowType = Gtk.ShadowType.In,
#if GTKCORE
				PropagateNaturalHeight = true,
				PropagateNaturalWidth = true
#endif
			};
			// Box = new EtoEventBox();
			// ScrolledWindow.Add(Box);
		}

		protected abstract ITreeModelImplementor CreateModelImplementor();

		protected void UpdateModel()
		{
			SkipSelectedChange = true;
			var selected = SelectedRows;
			Control.Model = new Gtk.TreeModelAdapter(CreateModelImplementor());
			SetSelectedRows(selected);
			SkipSelectedChange = false;
		}
		
		protected (double? hscroll, double? vscroll) SaveScrollState()
		{
			var hscrollbar = ScrolledWindow.HScrollbar as Gtk.Scrollbar;
			var vscrollbar = ScrolledWindow.VScrollbar as Gtk.Scrollbar;
			var hscroll = hscrollbar?.Value;
			var vscroll = vscrollbar?.Value;
			return (hscroll, vscroll);
		}
		
		protected void RestoreScrollState((double? hscroll, double? vscroll) state)
		
		{
			var hscrollbar = ScrolledWindow.HScrollbar as Gtk.Scrollbar;
			var vscrollbar = ScrolledWindow.VScrollbar as Gtk.Scrollbar;
			if (state.hscroll != null)
				hscrollbar.Value = state.hscroll.Value;
			if (state.vscroll != null)
				vscrollbar.Value = state.vscroll.Value;
		}

		protected override void Initialize()
		{
			Control = new Gtk.TreeView();
			// always need this one so the last column doesn't expand
			Control.AppendColumn(spacingColumn = new Gtk.TreeViewColumn());
			Control.ColumnDragFunction = Connector.HandleColumnDrag;

			UpdateModel();
			Control.HeadersVisible = true;
			ScrolledWindow.Child = Control;

			columns = new ColumnCollection { Handler = this };
			columns.Register(Widget.Columns);
			base.Initialize();

			// note: these must go through the (weakly referencing) connector, otherwise the native
			// signal keeps a strong reference to this handler and the control can never be collected.
			Control.Selection.Changed += Connector.HandlePreventEmptySelection;

			Control.QueryTooltip += Connector.HandleQueryTooltip;
			Control.HasTooltip = true;

			HandleEvent(Eto.Forms.Control.MouseDownEvent);			
		}

		void PreventEmptySelection()
		{
			// Prevent unselecting last selected item.
			if (!AllowEmptySelection && AllowMultipleSelection && Control.Selection.CountSelectedRows() == 0)
			{
				Control.GetCursor(out var cursorRow, out _);
				if (cursorRow != null)
				{
					Control.Selection.SelectPath(cursorRow);
				}
			}
		}

		void OnQueryTooltip(object o, Gtk.QueryTooltipArgs args)
		{
			var offset = 0;
			if (Control.HeadersVisible)
			{
				Control.BinWindow.GetPosition(out var bx, out offset);
			}
			var isData = Control.GetPathAtPos((int)args.X, (int)args.Y - offset, out var path, out var col);
			
			if (isData)
			{
				var columnIndex = GetColumnIndex(col);
				if (columnIndex != -1)
				{
					var etoColumn = Widget.Columns[columnIndex];
					if (etoColumn.CellToolTipBinding != null)
					{
						var item = GetItem(path);
						var cellRect = Control.GetCellArea(path, col);
						cellRect.Y += offset;
						args.Tooltip.Text = etoColumn.CellToolTipBinding.GetValue(item);
						args.Tooltip.TipArea = cellRect;
						args.RetVal = true;
					}
				}
			}
		}

		protected new GridConnector Connector => (GridConnector)base.Connector;

		protected override WeakConnector CreateConnector() => new GridConnector();

		protected class GridConnector : GtkControlConnector
		{
			public new GridHandler<TWidget, TCallback> Handler { get { return (GridHandler<TWidget, TCallback>)base.Handler; } }

			public void HandlePreventEmptySelection(object sender, EventArgs e) => Handler?.PreventEmptySelection();

			public void HandleQueryTooltip(object o, Gtk.QueryTooltipArgs args) => Handler?.OnQueryTooltip(o, args);

			[GLib.ConnectBefore]
			public override void HandleButtonPressEvent(object sender, Gtk.ButtonPressEventArgs args)
			{
				base.HandleButtonPressEvent(sender, args);

				if (!Equals(args.RetVal, true))
					OnTreeButtonPress(sender, args);

				// Gtk default behaviout for multiselect treeview is that
				// left and right click act the same, which is problematic
				// when it comes to user selecting multiple items and than
				// right clicking to find only one item remains selected
				// or if ctrl is held the current item gets unselected.
				if (!Equals(args.RetVal, true) && args.Event.Button == 3 && sender is Gtk.TreeView treeview)
				{
					Gtk.TreeViewDropPosition pos;
					Gtk.TreePath path;

					if (treeview.GetDestRowAtPos((int)args.Event.X, (int)args.Event.Y, out path, out pos))
					{
						var height = 0;
						if (treeview.HeadersVisible && treeview.Columns.Length > 0)
							height = treeview.GetCellArea(path, treeview.Columns[0]).Height;

						if (treeview.GetDestRowAtPos((int)args.Event.X, (int)args.Event.Y + height, out path, out pos))
						{
							var paths = treeview.Selection.GetSelectedRows().ToList();

							if (paths.Contains(path))
								args.RetVal = true;
						}
					}
				}
			}

			int[] selectedRows;

			static bool ArraysEqual<T>(T[] a1, T[] a2)
			{
				if (ReferenceEquals(a1, a2))
					return true;

				if (a1 == null || a2 == null)
					return false;

				if (a1.Length != a2.Length)
					return false;

				EqualityComparer<T> comparer = EqualityComparer<T>.Default;
				for (int i = 0; i < a1.Length; i++)
				{
					if (!comparer.Equals(a1[i], a2[i]))
						return false;
				}
				return true;
			}

			public void HandleGridSelectionChanged(object sender, EventArgs e)
			{
				var handler = Handler;
				if (handler == null)
					return;
				if (!handler.SkipSelectedChange)
				{
					var selected = handler.SelectedRows.ToArray();
					if (!ArraysEqual(selectedRows, selected))
					{
						handler.Callback.OnSelectionChanged(handler.Widget, EventArgs.Empty);
						selectedRows = selected;
					}
				}
			}			

			void OnTreeButtonPress(object sender, Gtk.ButtonPressEventArgs e)
			{
				if (e.Event.Type == Gdk.EventType.TwoButtonPress || e.Event.Type == Gdk.EventType.ThreeButtonPress)
					return;
				var h = Handler;
				if (h == null)
					return;

				Gtk.TreePath path;
				Gtk.TreeViewColumn clickedColumn;

				// Get path and column from mouse position
				h.Control.GetPathAtPos((int)e.Event.X, (int)e.Event.Y, out path, out clickedColumn);
				if (path == null || clickedColumn == null)
					return;

				var rowIndex = h.GetRowIndexOfPath(path);
				var columnIndex = h.GetColumnIndex(clickedColumn);
				var item = h.GetItem(path);
				var column = columnIndex == -1 || columnIndex >= h.Widget.Columns.Count ? null : h.Widget.Columns[columnIndex];

				var loc = h.PointFromScreen(new PointF((float)e.Event.XRoot, (float)e.Event.YRoot));

				if (ReferenceEquals(h.Control.ExpanderColumn, clickedColumn))
				{
					var cellArea = h.Control.GetCellArea(path, clickedColumn);
					if (loc.X < cellArea.Left && loc.X >= cellArea.Left - 18) // how do we get the size of the expander?
					{
						// clicked on the expander, don't fire the CellClick event
						return;
					}
				}

				h.Callback.OnCellClick(h.Widget, new GridCellMouseEventArgs(column, rowIndex, columnIndex, item, e.Event.ToEtoMouseButtons(), e.Event.State.ToEtoKey(), loc));
			}

			[GLib.ConnectBefore]
			public virtual void HandleGridColumnsChanged(object sender, EventArgs e)
			{
				var h = Handler;
				if (h == null)
					return;
				h.Callback.OnColumnOrderChanged(h.Widget, new GridColumnEventArgs(h.Widget.Columns.First()));
			}

			public virtual bool HandleColumnDrag(Gtk.TreeView tree_view, Gtk.TreeViewColumn column, Gtk.TreeViewColumn prev_column, Gtk.TreeViewColumn next_column)
			{
				var h = Handler;
				if (h == null)
					return true;
				return prev_column != h.spacingColumn;
			}
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Grid.ColumnHeaderClickEvent:
				case Grid.CellEditingEvent:
				case Grid.CellEditedEvent:
				case Grid.CellFormattingEvent:
				case Grid.RowFormattingEvent:
				case Grid.ColumnWidthChangedEvent:
					SetupColumnEvents();
					break;
				case Grid.CellClickEvent:
					HandleEvent(Eto.Forms.Control.MouseDownEvent);
					break;
				case Grid.CellDoubleClickEvent:
					Control.RowActivated += (sender, e) =>
					{
						var rowIndex = GetRowIndexOfPath(e.Path);
						var columnIndex = GetColumnIndex(e.Column);
						var item = GetItem(e.Path);
						var column = columnIndex == -1 ? null : Widget.Columns[columnIndex];
						Callback.OnCellDoubleClick(Widget, new GridCellMouseEventArgs(column, rowIndex, columnIndex, item, Mouse.Buttons, Keyboard.Modifiers, PointFromScreen(Mouse.Position)));
					};
					break;
				case Grid.SelectionChangedEvent:
					Control.Selection.Changed += Connector.HandleGridSelectionChanged;
					break;
				case Grid.ColumnOrderChangedEvent:
					Control.ColumnsChanged += Connector.HandleGridColumnsChanged;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}


		public override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			UpdateColumns();
		}

		void SetupColumnEvents()
		{
			if (!Widget.Loaded)
				return;
			foreach (var col in Widget.Columns.Select(r => r.Handler).OfType<GridColumnHandler>())
			{
				col.SetupEvents();
			}
		}

		public int RowDataColumn => 0;
		public int ItemDataColumn => 1;

		protected virtual void UpdateColumns()
		{
			if (!Widget.Loaded)
				return;
			columnMap.Clear();
			int columnIndex = 0;
			int dataIndex = 2; // skip RowDataColumn and ItemDataColumn
			
			for (int i = 0; i < Widget.Columns.Count; i++)
			{
				var col = Widget.Columns[i];
				var colHandler = (GridColumnHandler)col.Handler;
				colHandler.Control.Reorderable = AllowColumnReordering;
				colHandler.SetupCell(this, this, columnIndex++, ref dataIndex);
				if (i == 0)
					Control.ExpanderColumn = colHandler.Control;
			}
			
			foreach (var col in Widget.Columns.OrderBy(r => r.DisplayIndex))
			{
				((GridColumnHandler)col.Handler).SetDisplayIndex();
			}
			
		}

		class ColumnCollection : EnumerableChangedHandler<GridColumn, GridColumnCollection>
		{
			public GridHandler<TWidget, TCallback> Handler { get; set; }

			public override void AddItem(GridColumn item)
			{
				var colhandler = (GridColumnHandler)item.Handler;
#if GTKCORE
				Handler.Control.InsertColumn(colhandler.Control, (int)Handler.Control.NColumns - 1);
#else
				Handler.Control.InsertColumn(colhandler.Control, Count);
#endif
				Handler.UpdateColumns();
			}

			public override void InsertItem(int index, GridColumn item)
			{
				var colhandler = (GridColumnHandler)item.Handler;
				Handler.Control.InsertColumn(colhandler.Control, index);
				Handler.UpdateColumns();
			}

			public override void RemoveItem(int index)
			{
				var colhandler = (GridColumnHandler)Handler.Widget.Columns[index].Handler;
				Handler.Control.RemoveColumn(colhandler.Control);
				Handler.UpdateColumns();
			}

			public override void RemoveAllItems()
			{
				foreach (var col in Handler.Control.Columns)
				{
					Handler.Control.RemoveColumn(col);
				}
				Handler.Control.AppendColumn(Handler.spacingColumn);
				Handler.UpdateColumns();
			}

		}

		public bool ShowHeader
		{
			get { return Control.HeadersVisible; }
			set { Control.HeadersVisible = value; }
		}


		public bool AllowColumnReordering
		{
			get { return Widget.Properties.Get<bool>(GridHandler.AllowColumnReordering_Key, false); }
			set
			{
				Widget.Properties.Set(GridHandler.AllowColumnReordering_Key, value, false);
				UpdateColumns();
			}
		}

		public int NumberOfColumns
		{
			get
			{
				return Widget.Columns.Count;
			}
		}

		public abstract object GetItem(Gtk.TreePath path);

		public int GetColumnIndex(Gtk.TreeViewColumn item)
		{
			for (int i = 0; i < Widget.Columns.Count; i++)
			{
				GridColumn col = Widget.Columns[i];
				if (col.Handler is GridColumnHandler handler && ReferenceEquals(handler.Control, item))
				{
					return i;
				}
			}
			return -1;
		}

		public virtual int GetRowIndexOfPath(Gtk.TreePath path)
		{
			int rowIndex = 0;
			if (path.Indices.Length > 0)
			{
				for (int i = 0; i < path.Depth; i++)
					rowIndex += path.Indices[i] + 1;

				rowIndex--;
			}
			else
				rowIndex = -1;

			return rowIndex;
		}

		public abstract Gtk.TreeIter GetIterAtRow(int row);

		public abstract Gtk.TreePath GetPathAtRow(int row);

		public void SetColumnMap(int dataIndex, int column, GLib.GType type)
		{
			columnMap[dataIndex] = (column, type);
		}

		public void EndCellEditing(Gtk.TreePath path, int column)
		{
			var row = path.Indices.Length > 0 ? path.Indices[0] : -1;
			var item = GetItem(path);
			Callback.OnCellEdited(Widget, new GridViewCellEventArgs(Widget.Columns[column], row, column, item));
		}

		public void BeginCellEditing(Gtk.TreePath path, int column)
		{
			var row = path.Indices.Length > 0 ? path.Indices[0] : -1;
			var item = GetItem(path);
			Callback.OnCellEditing(Widget, new GridViewCellEventArgs(Widget.Columns[column], row, column, item));
		}

		public void ColumnClicked(GridColumnHandler column)
		{
			Callback.OnColumnHeaderClick(Widget, new GridColumnEventArgs(column.Widget));
		}

		public bool AllowMultipleSelection
		{
			get { return Control.Selection.Mode == Gtk.SelectionMode.Multiple; }
			set => SetSelectionMode(AllowEmptySelection, value);
		}

		public virtual IEnumerable<int> SelectedRows
		{
			get
			{
				return Control.Selection.GetSelectedRows().Select(r => r.Indices[0]);
			}
			set
			{
				SkipSelectedChange = true;
				SetSelectedRows(value);
				SkipSelectedChange = false;
				Callback.OnSelectionChanged(Widget, EventArgs.Empty);
			}
		}

		protected abstract void SetSelectedRows(IEnumerable<int> value);

		public int RowHeight
		{
			get;
			set;
		}

		public void SelectAll()
		{
			Control.Selection.SelectAll();
		}

		public void SelectRow(int row)
		{
			Control.Selection.SelectIter(GetIterAtRow(row));
		}

		public void UnselectRow(int row)
		{
			Control.Selection.UnselectIter(GetIterAtRow(row));
		}

		public void UnselectAll()
		{
			Control.Selection.UnselectAll();
		}

		public void BeginEdit(int row, int column)
		{
			var nameColumn = Control.Columns[column];
			if (nameColumn.Cells.Length == 0)
				return;
			var cellRenderer = nameColumn.Cells[0];
			var path = Control.Model.GetPath(GetIterAtRow(row));
			Control.Model.IterNChildren();
			Control.SetCursorOnCell(path, nameColumn, cellRenderer, true);
		}

		public bool CommitEdit()
		{
			Gtk.TreePath path;
			Gtk.TreeViewColumn column;
			Control.GetCursor(out path, out column);
			if (path == null || column == null)
				return true;

			// This is a hack, but it works to commit editing.  Is there a better way?
			if (Control.FocusChild?.HasFocus == true)
				Control.ChildFocus(Gtk.DirectionType.TabForward);
			return true;
		}

		public bool CancelEdit()
		{
			Gtk.TreePath path;
			Gtk.TreeViewColumn column;
			Control.GetCursor(out path, out column);
			if (path == null || column == null)
				return true;

			// This is a hack, but it works to abort editing.  Is there a better way?
			if (Control.FocusChild?.HasFocus == true)
				Control.GrabFocus();
			return true;
		}

		public void OnCellFormatting(GridCellFormatEventArgs args)
		{
			// var tooltipBinding = args.Column?.CellToolTipBinding;
			// if (tooltipBinding != null && args is GtkGridCellFormatEventArgs macargs && macargs.View != null)
			// 	macargs.View.ToolTip = tooltipBinding.GetValue(args.Item) ?? string.Empty;
			
			Callback.OnCellFormatting(Widget, args);
		}

		public void OnRowFormatting(GridRowFormatEventArgs args) => Callback.OnRowFormatting(Widget, args);

		public void ScrollToRow(int row)
		{
			var path = this.GetPathAtRow(row);
			var column = Control.Columns.First();
			Control.ScrollToCell(path, column, false, 0, 0);
		}

		public GridLines GridLines
		{
			get
			{
				switch (Control.EnableGridLines)
				{
					case Gtk.TreeViewGridLines.None:
						return GridLines.None;
					case Gtk.TreeViewGridLines.Horizontal:
						return GridLines.Horizontal;
					case Gtk.TreeViewGridLines.Vertical:
						return GridLines.Vertical;
					case Gtk.TreeViewGridLines.Both:
						return GridLines.Both;
					default:
						throw new NotSupportedException();
				}
			}
			set
			{
				switch (value)
				{
					case GridLines.None:
						Control.EnableGridLines = Gtk.TreeViewGridLines.None;
						break;
					case GridLines.Horizontal:
						Control.EnableGridLines = Gtk.TreeViewGridLines.Horizontal;
						break;
					case GridLines.Vertical:
						Control.EnableGridLines = Gtk.TreeViewGridLines.Vertical;
						break;
					case GridLines.Both:
						Control.EnableGridLines = Gtk.TreeViewGridLines.Both;
						break;
					default:
						throw new NotSupportedException();
				}
			}
		}

		public BorderType Border
		{
			get { return Widget.Properties.Get(GridHandler.Border_Key, BorderType.Bezel); }
			set { Widget.Properties.Set(GridHandler.Border_Key, value, () => ScrolledWindow.ShadowType = value.ToGtk(), BorderType.Bezel); }
		}

		public bool IsEditing
		{
			get
			{
				Gtk.TreePath path;
				Gtk.TreeViewColumn focus_column;
				Control.GetCursor(out path, out focus_column);

#if GTK2
				var cells = focus_column?.CellRenderers;
#elif GTK3
				var cells = focus_column?.Cells;
#endif
				return cells?.OfType<IEtoCellRenderer>().Any(r => r.Editing) ?? false;
			}
		}

		protected abstract bool HasRows { get; }

		public bool AllowEmptySelection
		{
			get => Widget.Properties.Get<bool>(GridHandler.AllowEmptySelection_Key, true);
			set
			{
				Widget.Properties.TrySet(GridHandler.AllowEmptySelection_Key, value, true);
				SetSelectionMode(value, AllowMultipleSelection);
			}
		}

		private void SetSelectionMode(bool allowEmptySelection, bool allowMultipleSelection)
		{
			var currentMode = Control.Selection.Mode;
			var newMode =
				(allowEmptySelection, allowMultipleSelection) switch
				{
					(true, true) => Gtk.SelectionMode.Multiple,
					(true, false) => Gtk.SelectionMode.Single,
					(false, true) => Gtk.SelectionMode.Multiple, // Handled by Selection Changed event in Initialize().
					(false, false) => Gtk.SelectionMode.Browse
				};

			if (newMode != currentMode)
			{
				// Workaround to not lose the ability to select after switching to Multi, unselecting everything,
				// then switching back to Single or Browse. Cause unknown but reproduced in GtkSharp.
				if (currentMode == Gtk.SelectionMode.Multiple
				    && newMode is Gtk.SelectionMode.Single or Gtk.SelectionMode.Browse)
				{
					Control.GetCursor(out var cursorRowPath, out _);
					var mustReselectCursor = cursorRowPath != null && Control.Selection.PathIsSelected(cursorRowPath);

					Control.Selection.Mode = Gtk.SelectionMode.None;
					Control.Selection.Mode = newMode;

					if (mustReselectCursor)
					{
						Control.Selection.SelectPath(cursorRowPath);
					}
				}
				else
				{
					Control.Selection.Mode = newMode;
				}
			}
		}

		protected void EnsureSelection()
		{
			if (!AllowEmptySelection && Control.Selection.CountSelectedRows() == 0)
			{
				SelectRow(0);
			}
		}

		public int GetColumnDisplayIndex(GridColumnHandler column)
		{
			var columns = Control.Columns;
			return Array.IndexOf(columns, column.Control);
		}

		public void SetColumnDisplayIndex(GridColumnHandler column, int index)
		{
			var columns = Control.Columns;
			var currentIndex = Array.IndexOf(columns, column.Control);
			if (index != currentIndex)
				Control.MoveColumnAfter(column.Control, columns[index]);
		}

		public void ColumnWidthChanged(GridColumnHandler h)
		{
			Callback.OnColumnWidthChanged(Widget, new GridColumnEventArgs(h.Widget));
		}

		public abstract int GetRowOfItem(object item);

		// Number of rows currently displayed (all rows for a flat grid, expanded rows for a tree).
		protected abstract int NumberOfRows { get; }

		// Height of a single row, including the RowHeight floor that the cell renderers apply plus
		// the inter-row spacing GtkTreeView adds between rows.
		int GetContentRowHeight()
		{
			// Ask each cell renderer how tall it wants to be. While the tree is mapped this is exact,
			// but while it is unmapped the renderers report 0 (they only measure once they've been
			// rendered on a real window), so fall back to the font line height below.
			int cellHeight = 0;
			foreach (Gtk.TreeViewColumn column in Control.Columns)
			{
				if (!column.Visible)
					continue;
				column.CellGetSize(Gdk.Rectangle.Zero, out _, out _, out _, out var height);
				cellHeight = Math.Max(cellHeight, height);
			}
			if (cellHeight == 0)
				cellHeight = GetFontRowHeight();

			int rowHeight = Math.Max(RowHeight, cellHeight);
			if (rowHeight > 0)
				rowHeight += RowSeparatorHeight;
			return rowHeight;
		}

		// The height a text cell renderer needs for a single line, derived from the tree's font.
		// Unlike the cell renderers, a Pango layout reports a stable line height regardless of whether
		// the tree is mapped, so this lets us estimate the row height before the tree is ever shown.
		int GetFontRowHeight()
		{
			// include ascender/descender glyphs so we get the full line height
			using (var layout = Control.CreatePangoLayout("Xygj"))
			{
				layout.GetPixelSize(out _, out var lineHeight);
				return lineHeight + CellVerticalPadding;
			}
		}

		// GtkCellRendererText's default vertical padding (ypad = 2, top and bottom).
		const int CellVerticalPadding = 4;

		// GtkTreeView's default "vertical-separator" style property, the spacing added between rows.
		// The exact allocated height wins via Math.Max once the tree is mapped; this only refines
		// the estimate used while unmapped.
		const int RowSeparatorHeight = 2;

		// Vertical space taken by the ScrolledWindow's frame (the ShadowType.In border drawn around
		// the tree). The style context reports this regardless of mapped state.
		int GetFrameHeight()
		{
			if (ScrolledWindow.ShadowType == Gtk.ShadowType.None)
				return 0;
			var border = ScrolledWindow.StyleContext.GetBorder(Gtk.StateFlags.Normal);
			var height = border.Top + border.Bottom;
			return height > 0 ? height : DefaultFrameHeight;
		}

		// Fallback border thickness (1px top + 1px bottom) when the theme doesn't report a frame border.
		const int DefaultFrameHeight = 2;

		// The natural height needed to display every row plus the header and frame. GtkTreeView derives
		// its height from lazily-validated rows, which only happens while it is mapped -- so a native
		// measurement taken while unmapped (or after the data store changed while unmapped) reports a
		// stale minimal height. This computes the height explicitly from the row count and per-row
		// height so it is correct regardless of realized/mapped state, and feeds the ScrolledWindow's
		// native height request (see EtoScrolledWindow) so containers measure it correctly too.
		internal int CalculateContentHeight()
		{
			var rowCount = NumberOfRows;
			var rowHeight = GetContentRowHeight();
			if (rowCount <= 0 || rowHeight <= 0)
				return 0;
			var height = rowCount * rowHeight;
			if (ShowHeader)
				height += GetHeaderHeight();
			height += GetFrameHeight();
			return height;
		}

		int GetHeaderHeight()
		{
			int max = 0;
			foreach (Gtk.TreeViewColumn column in Control.Columns)
			{
				if (column.Button != null)
				{
					column.Button.GetPreferredHeight(out _, out var nat);
					max = Math.Max(max, nat);
				}
			}
			return max;
		}
	}
}

