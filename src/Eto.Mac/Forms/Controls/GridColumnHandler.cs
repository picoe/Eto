using System;
using Eto.Forms;
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
	}

	public interface IDataColumnHandler
	{
		void Setup(IDataViewHandler handler, int column);

		NSObject GetObjectValue(object dataItem);

		void SetObjectValue(object dataItem, NSObject val);

		GridColumn Widget { get; }

		IDataViewHandler DataViewHandler { get; }

		void AutoSizeColumn(NSRange rowRange, bool force = false);

		void EnabledChanged(bool value);

		nfloat GetPreferredWidth(NSRange? range = null);
	}

	public class GridColumnHandler : MacObject<NSTableColumn, GridColumn, GridColumn.ICallback>, GridColumn.IHandler, IDataColumnHandler
	{
		Cell dataCell;
		Font font;

		public IDataViewHandler DataViewHandler { get; private set; }

		public ICellHandler DataCellHandler
		{
			get
			{
				return dataCell != null ? dataCell.Handler as ICellHandler : null;
			}
		}

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
			DataCell = new TextBoxCell();
			base.Initialize();
		}

		public void AutoSizeColumn(NSRange rowRange, bool force = false)
		{
			var handler = DataViewHandler;
			if (AutoSize && handler != null)
			{
				var width = GetPreferredWidth(rowRange);
				if (force || width > Control.Width)
					Control.Width = (nfloat)Math.Ceiling(width);
			}
		}

		public nfloat GetPreferredWidth(NSRange? range = null)
		{
			var handler = DataViewHandler;
			nfloat width = 0;
			var outlineView = handler.Table as NSOutlineView;
			if (handler.ShowHeader)
				width = (nfloat)Math.Max(Control.HeaderCell.CellSizeForBounds(new CGRect(0, 0, int.MaxValue, int.MaxValue)).Width, width);

			if (dataCell != null)
			{
				/* If no range specified, size based on visible cells */
				var currentRange = range ?? handler.Table.RowsInRect(handler.GetVisibleRect());

				var cellSize = Control.DataCell.CellSize;
				cellSize.Height = (nfloat)Math.Max(cellSize.Height, handler.RowHeight);
				var cell = DataCellHandler;
				for (int i = (int)currentRange.Location; i < (int)(currentRange.Location + currentRange.Length); i++)
				{
					var item = DataViewHandler.GetItem(i);
					var val = GetObjectValue(item);
					var cellWidth = cell.GetPreferredWidth(val, cellSize, i, item);
					// -1 signifies that it doesn't support getting the preferred width
					if (cellWidth == -1)
						cellWidth = Control.Width;
					else if (outlineView != null && Column == 0)
					{
						cellWidth += (float)((outlineView.LevelForRow((nint)i) + 1) * outlineView.IndentationPerLevel);
					}
					width = (nfloat)Math.Max(width, cellWidth);
				}
			}
			return width;
		}

		public string HeaderText
		{
			get { return Control.HeaderCell.StringValue; }
			set { Control.HeaderCell.StringValue = value; }
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
			get;
			set;
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
				var table = Control.TableView;
				if (DataViewHandler != null && DataViewHandler.Loaded && table != null)
				{
					table.SetNeedsDisplay();
				}

			}
		}

		public int Width
		{
			get { return (int)Math.Ceiling(Control.Width) + 3; }
			set { Control.Width = Math.Max(0, value - 3); }
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
		}

		public NSObject GetObjectValue(object dataItem)
		{
			return ((ICellHandler)dataCell.Handler).GetObjectValue(dataItem);
		}

		public void SetObjectValue(object dataItem, NSObject val)
		{
			((ICellHandler)dataCell.Handler).SetObjectValue(dataItem, val);
		}

		GridColumn IDataColumnHandler.Widget
		{
			get { return Widget; }
		}

		public Font Font
		{
			get
			{
				if (font == null)
					font = new Font(new FontHandler(Control.DataCell.Font));
				return font;
			}
			set
			{
				font = value;
				if (font != null)
				{
					var fontHandler = (FontHandler)font.Handler;
					Control.DataCell.Font = fontHandler.Control;
				}
				else
					Control.DataCell.Font = null;
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
	}
}

