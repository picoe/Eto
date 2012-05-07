using System;
using MonoMac.AppKit;
using Eto.Forms;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;

namespace Eto.Platform.Mac.Forms.Controls
{
	public interface IDataViewHandler
	{
		bool ShowHeader { get; }
		
		NSTableView Table { get; }
		
		object GetItem (int row);
		
		int RowCount { get; }
		
		System.Drawing.RectangleF GetVisibleRect ();
	}
	
	public interface IDataColumnHandler
	{
		void Setup (int column);
		
		NSObject GetObjectValue (object dataItem);
		
		void SetObjectValue (object dataItem, NSObject val);
		
		GridColumn Widget { get; }
	}
	
	public class EtoDataColumnIdentifier : NSObject
	{
		public int Column { get; set; }

		public IDataColumnHandler Handler { get; set; }
	}

	public class GridColumnHandler : MacObject<NSTableColumn, GridColumn>, IGridColumn, IDataColumnHandler
	{
		Cell dataCell;
		
		public IDataViewHandler DataViewHandler { get; private set; }
		
		public int Column { get; private set; }
		
		public GridColumnHandler ()
		{
			Control = new NSTableColumn ();
			Control.ResizingMask = NSTableColumnResizing.None;
			Sortable = false;
			HeaderText = string.Empty;
			Editable = false;
			AutoSize = true;
		}
		
		public override void Initialize ()
		{
			base.Initialize ();
			this.DataCell = new TextBoxCell (Widget.Generator);
		}
		
		public void Loaded (IDataViewHandler handler, int column)
		{
			this.Column = column;
			this.DataViewHandler = handler;
		}
		
		public void Resize ()
		{
			var handler = this.DataViewHandler;
			if (this.AutoSize && handler != null) {
				Control.SizeToFit ();
				float width = Control.DataCell.CellSize.Width;
				var outlineView = handler.Table as NSOutlineView;
				if (handler.ShowHeader)
					width = Math.Max (Control.HeaderCell.CellSize.Width, width);
					
				if (dataCell != null) {
					var rect = handler.GetVisibleRect ();
					var range = handler.Table.RowsInRect (rect);
					//var range = new NSRange(0, handler.RowCount);
					var cellSize = Control.DataCell.CellSize;
					var dataCellHandler = ((ICellHandler)dataCell.Handler);
					for (int i = range.Location; i < range.Location + range.Length; i++) {
						//var dataCellRow = Control.DataCellForRow(i);
						//var cellWidth = dataCellRow.CellSize.Width;
						var cellWidth = GetRowWidth (dataCellHandler, i, cellSize) + 4;
						if (outlineView != null && Column == 0)
							cellWidth += (outlineView.LevelForRow (i) + 1) * outlineView.IndentationPerLevel;
						width = Math.Max (width, cellWidth);
					}
				}
				Control.Width = width;
			}
		}
		
		protected virtual float GetRowWidth (ICellHandler cell, int row, System.Drawing.SizeF cellSize)
		{
			var val = GetObjectValue (DataViewHandler.GetItem (row));
			return cell.GetPreferredSize (val, cellSize);
		}
		
		public string HeaderText {
			get { return Control.HeaderCell.StringValue; }
			set { Control.HeaderCell.StringValue = value; }
		}
		
		public bool Resizable {
			get {
				return Control.ResizingMask.HasFlag (NSTableColumnResizing.UserResizingMask);
			}
			set {
				if (value)
					Control.ResizingMask |= NSTableColumnResizing.UserResizingMask;
				else
					Control.ResizingMask &= ~NSTableColumnResizing.UserResizingMask;
			}
		}
		
		public bool AutoSize {
			get;
			set;
		}

		public bool Sortable { get; set; }
		
		public bool Editable {
			get {
				return Control.Editable;
			}
			set {
				Control.Editable = value;
				if (Control.DataCell != null) {
					Control.DataCell.Enabled = Control.DataCell.Editable = value;
				}
			}
		}
		
		public int Width {
			get { return (int)Control.Width; }
			set { Control.Width = value; }
		}
		
		public bool Visible {
			get { return !Control.Hidden; }
			set { Control.Hidden = !value; }
		}
		
		public Cell DataCell {
			get { return dataCell; }
			set {
				dataCell = value;
				if (dataCell != null) {
					var editable = this.Editable;
					var cellHandler = (ICellHandler)dataCell.Handler;
					cellHandler.ColumnHandler = this;
					Control.DataCell = cellHandler.Control;
					Control.DataCell.Enabled = Control.DataCell.Editable = editable;
				} else
					Control.DataCell = null;
			}
		}
		
		public void Setup (int column)
		{
			this.Column = column;
			Control.Identifier = new EtoDataColumnIdentifier{ Handler = this, Column = column };
		}
		
		public NSObject GetObjectValue (object dataItem)
		{
			return ((ICellHandler)dataCell.Handler).GetObjectValue (dataItem);
		}
		
		public void SetObjectValue (object dataItem, NSObject val)
		{
			((ICellHandler)dataCell.Handler).SetObjectValue (dataItem, val);
		}

		GridColumn IDataColumnHandler.Widget {
			get { return (GridColumn)Widget; }
		}
	}
}

