using Eto.Mac.Drawing;
using Eto.Mac.Forms.Cells;



namespace Eto.Mac.Forms.Controls
{
	public interface IDataViewHandler
	{
		bool ShowHeader { get; }

		NSTableView Table { get; }

		object GetItem(int row);

		int RowCount { get; }

		int RowHeight { get; }

		CGRect GetVisibleRect();

		bool Loaded { get; }

		void OnCellFormatting(GridCellFormatEventArgs args);

		void OnCellEditing(GridViewCellEventArgs e);

		void OnCellEdited(GridViewCellEventArgs e);

		Grid Widget { get; }

		bool SuppressUpdate { get; }

		bool IsCancellingEdit { get; }

		void ResetAutoSizedColumns();
		bool AutoSizeColumns(bool force, bool forceNewSize = false);

		int GetColumnDisplayIndex(GridColumn column);
		void SetColumnDisplayIndex(GridColumn column, int index);
	}

	public interface IDataColumnHandler : GridColumn.IHandler
	{
		NSTableColumn Control { get; }
		void Setup(IDataViewHandler handler, int column);
		NSObject GetObjectValue(object dataItem);
		void SetObjectValue(object dataItem, NSObject val);
		new GridColumn Widget { get; }
		IDataViewHandler DataViewHandler { get; }
		ICellHandler DataCellHandler { get; }
		bool AutoSizeColumn(NSRange? rowRange, bool force = false);
		void EnabledChanged(bool value);
		nfloat GetPreferredWidth(NSRange? range = null);
		void SizeToFit();
		void SetupDisplayIndex();
	}

	public class GridColumnHandler : MacObject<NSTableColumn, GridColumn, GridColumn.ICallback>, GridColumn.IHandler, IDataColumnHandler
	{
		Cell dataCell;
		bool autoSize;

		static readonly object Font_Key = new object();
		static readonly object WidthAdjust_Key = new object();
		static readonly object MinWidthAdjust_Key = new object();
		static readonly object MaxWidthAdjust_Key = new object();

		// get the cell spacing so we can set column widths based on actual width
		int IntercellSpacingWidth => (int)(Control.TableView?.IntercellSpacing.Width ?? 0);

		public IDataViewHandler DataViewHandler { get; private set; }

		public ICellHandler DataCellHandler => dataCell?.Handler as ICellHandler;

		public int Column { get; private set; }

		protected override NSTableColumn CreateControl()
		{
			return new NSTableColumn();
		}

		protected override void Initialize()
		{
			Control.ResizingMask = NSTableColumnResizing.UserResizingMask;
			Sortable = false;
			HeaderText = string.Empty;
			Editable = false;
			AutoSize = true;
			Control.Width = Control.MinWidth;
			DataCell = new TextBoxCell();
			base.Initialize();
		}

		public bool AutoSizeColumn(NSRange? rowRange, bool force = false)
		{
			var handler = DataViewHandler;
			if (handler == null)
				return false;

			if (AutoSize)
			{
				var width = GetPreferredWidth(rowRange);
				if (force || width > Control.Width)
				{
					Control.Width = (nfloat)Math.Ceiling(width);
					return true;
				}
			}
			return false;
		}

		public nfloat GetPreferredWidth(NSRange? range = null)
		{
			var handler = DataViewHandler;
			nfloat width = 0;

			if (!AutoSize)
				return Width;

			var outlineView = handler.Table as NSOutlineView;
			bool isOutlineColumn = outlineView != null && Column == 0;
			if (handler.ShowHeader)
				width = (nfloat)Math.Max(Control.HeaderCell.CellSizeForBounds(new CGRect(0, 0, int.MaxValue, int.MaxValue)).Width, width);
			
			if (isOutlineColumn)
				width += (float)outlineView.IndentationPerLevel;

			if (dataCell != null)
			{
				/* If no range specified, size based on visible cells */
				var currentRange = range ?? handler.Table.RowsInRect(handler.GetVisibleRect());

				var cellSize = Control.DataCell.CellSize;
				cellSize.Height = (nfloat)Math.Max(cellSize.Height, handler.RowHeight);
				var cell = DataCellHandler;
				var displayIndex = DisplayIndex;
				var columnRect = isOutlineColumn ? outlineView.RectForColumn(displayIndex) : CGRect.Empty;
				for (int i = (int)currentRange.Location; i < (int)(currentRange.Location + currentRange.Length); i++)
				{
					var item = DataViewHandler.GetItem(i);
					var val = GetObjectValue(item);
					var cellWidth = cell.GetPreferredWidth(val, cellSize, i, item);
					// -1 signifies that it doesn't support getting the preferred width
					if (cellWidth == -1)
						cellWidth = Control.Width;
					else if (isOutlineColumn)
					{
						// gets the proper indent for the current row
						cellWidth += outlineView.GetCellFrame(displayIndex, i).X - columnRect.X;
					}
					width = (nfloat)Math.Max(width, cellWidth);
				}
			}
			width = (nfloat)Math.Max(MinWidth, Math.Min(MaxWidth, width));
			return width;
		}

		public string HeaderText
		{
			get { return Control.HeaderCell.StringValue; }
			set
			{
				if (value != HeaderText)
				{
					Control.HeaderCell.StringValue = value;
					Control.TableView?.HeaderView?.SetNeedsDisplay();
				}
			}
		}

		public bool Resizable
		{
			get
			{
				return Control.ResizingMask.HasFlag(NSTableColumnResizing.UserResizingMask);
			}
			set
			{
				if (value)
					Control.ResizingMask |= NSTableColumnResizing.UserResizingMask;
				else
					Control.ResizingMask &= ~NSTableColumnResizing.UserResizingMask;
			}
		}

		public bool AutoSize
		{
			get => autoSize;
			set
			{
				if (autoSize != value)
				{
					autoSize = value;
					if (autoSize && !IsLoaded)
						Control.Width = Control.MinWidth;
				}
			}
		}

		public bool Sortable { get; set; }

		public bool Editable
		{
			get
			{
				return Control.Editable;
			}
			set
			{
				Control.Editable = value;
				if (dataCell != null)
				{
					var cellHandler = (ICellHandler)dataCell.Handler;
					cellHandler.Editable = value;
				}
				if (IsLoaded)
				{
					Control.TableView?.SetNeedsDisplay();
				}

			}
		}

		bool IsLoaded => DataViewHandler != null && DataViewHandler.Loaded && Control.TableView != null;

		public int Width
		{
			get => (int)Math.Ceiling(Control.Width) + IntercellSpacingWidth;
			set
			{
				AutoSize = value == -1;
				Control.Width = Math.Max(0, value - IntercellSpacingWidth);

				if (IsLoaded)
				{
					Control.TableView?.SizeToFit();
				}
				else if (Control.TableView == null)
				{
					Widget.Properties.Set(WidthAdjust_Key, true);
				}
			}
		}

		public bool Visible
		{
			get { return !Control.Hidden; }
			set { Control.Hidden = !value; }
		}

		public Cell DataCell
		{
			get { return dataCell; }
			set
			{
				dataCell = value;
				if (dataCell != null)
				{
					var editable = Editable;
					var cellHandler = (ICellHandler)dataCell.Handler;
					cellHandler.ColumnHandler = this;
					cellHandler.Editable = editable;
				}
				//else
				//Control.DataCell = null;
			}
		}

		public void Setup(IDataViewHandler handler, int column)
		{
			Column = column;
			Control.Identifier = new NSString(column.ToString());
			DataViewHandler = handler;

			// adjust widths now that we know the cell spacing
			var dividerWidth = IntercellSpacingWidth;
			if (Widget.Properties.Get<bool>(WidthAdjust_Key))
			{
				Control.Width = (nfloat)Math.Max(0, Control.Width - dividerWidth);
				Widget.Properties.Remove(WidthAdjust_Key);
			}
			if (Widget.Properties.Get<bool>(MaxWidthAdjust_Key))
			{
				Control.MaxWidth = (nfloat)Math.Max(0, Control.MaxWidth - dividerWidth);
				Widget.Properties.Remove(MaxWidthAdjust_Key);
			}
			if (Widget.Properties.Get<bool>(MinWidthAdjust_Key))
			{
				Control.MinWidth = (nfloat)Math.Max(0, Control.MinWidth - dividerWidth);
				Widget.Properties.Remove(MinWidthAdjust_Key);
			}
		}

		public NSObject GetObjectValue(object dataItem)
		{
			return ((ICellHandler)dataCell.Handler).GetObjectValue(dataItem);
		}

		public void SetObjectValue(object dataItem, NSObject val)
		{
			((ICellHandler)dataCell.Handler).SetObjectValue(dataItem, val);
		}

		GridColumn IDataColumnHandler.Widget => Widget;

		public Font Font
		{
			get
			{
				var font = Widget.Properties.Get<Font>(Font_Key);
				if (font == null)
				{
					font = new Font(new FontHandler(Control.DataCell.Font));
					Widget.Properties.Set(Font_Key, font);
				}
				return font;
			}
			set
			{
				if (Widget.Properties.TrySet(Font_Key, value))
				{
					Control.DataCell.Font = FontHandler.GetControl(value);
				}
			}
		}

		public bool Expand
		{
			get => Control.ResizingMask.HasFlag(NSTableColumnResizing.Autoresizing);
			set
			{
				if (value)
					Control.ResizingMask |= NSTableColumnResizing.Autoresizing;
				else
					Control.ResizingMask &= ~NSTableColumnResizing.Autoresizing;
			}
		}
		public TextAlignment HeaderTextAlignment
		{
			get => Control.HeaderCell.Alignment.ToEto();
			set => Control.HeaderCell.Alignment = value.ToNS();
		}
		public int MinWidth
		{
			get => (int)Control.MinWidth + IntercellSpacingWidth;
			set
			{
				if (value == MinWidth)
					return;

				var width = Math.Max(0, value - IntercellSpacingWidth);
				if (Control.Width < width)
				{
					// need to set width for things to update correctly..
					var autoSize = AutoSize;
					Control.Width = width;
					AutoSize = autoSize;
				}

				Control.MinWidth = width;
				if (IsLoaded)
				{
					DataViewHandler?.ResetAutoSizedColumns();
					Control.TableView.NeedsLayout = true;
				}
				else if (Control.TableView == null)
				{
					Widget.Properties.Set(MinWidthAdjust_Key, true);
				}
			}
		}

		public int MaxWidth
		{
			get
			{
				var width = Control.MaxWidth + IntercellSpacingWidth;
				if (width > int.MaxValue)
					return int.MaxValue;
				return (int)width;
			}
			set
			{
				if (value == MaxWidth)
					return;

				var width = Math.Max(0, value - IntercellSpacingWidth);
				if (Control.Width > width)
				{
					// need to set width for things to update correctly..
					var autoSize = AutoSize;
					Control.Width = width;
					AutoSize = autoSize;
				}
				Control.MaxWidth = width;
				if (IsLoaded)
				{
					DataViewHandler?.ResetAutoSizedColumns();
					Control.TableView.NeedsLayout = true;
				}
				else if (Control.TableView == null)
				{
					Widget.Properties.Set(MaxWidthAdjust_Key, true);
				}
			}
		}

		public void EnabledChanged(bool value)
		{
			if (dataCell != null)
			{
				var cellHandler = (ICellHandler)dataCell.Handler;
				cellHandler.EnabledChanged(value);
			}
		}

		public void SizeToFit() => Control.SizeToFit();


		static readonly object DisplayIndex_Key = new object();
		public int DisplayIndex
		{
			get => DataViewHandler?.GetColumnDisplayIndex(Widget) ?? Widget.Properties.Get<int?>(DisplayIndex_Key) ?? -1;
			set
			{
				Widget.Properties.Set<int?>(DisplayIndex_Key, value);
				DataViewHandler?.SetColumnDisplayIndex(Widget, value);
			}
		}

		public string HeaderToolTip
		{
			get => Control.HeaderToolTip;
			set => Control.HeaderToolTip = value ?? string.Empty;
		}

		static readonly object CellToolTipBinding_Key = new object();

		public IIndirectBinding<string> CellToolTipBinding
		{
			get => Widget.Properties.Get<IIndirectBinding<string>>(CellToolTipBinding_Key);
			set => Widget.Properties.Set(CellToolTipBinding_Key, value);
		}

		public void SetupDisplayIndex()
		{
			var displayIndex = Widget.Properties.Get<int?>(DisplayIndex_Key) ?? -1;
			if (displayIndex >= 0)
			{
				DataViewHandler?.SetColumnDisplayIndex(Widget, displayIndex);
				Widget.Properties.Remove(DisplayIndex_Key);
			}
		}
	}
}

