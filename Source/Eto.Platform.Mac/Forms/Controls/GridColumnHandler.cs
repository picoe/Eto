using System;
using MonoMac.AppKit;
using Eto.Forms;
using MonoMac.Foundation;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class EtoGridColumnIdentifier : NSObject
	{
		public int Column { get; set; }

		public GridColumnHandler Handler { get; set; }
	}

	public class GridColumnHandler : WidgetHandler<NSTableColumn, GridColumn>, IGridColumn
	{
		Cell dataCell;
		
		public GridColumnHandler ()
		{
			Control = new NSTableColumn ();
			Control.ResizingMask = NSTableColumnResizing.UserResizingMask;
			
			Sortable = false;
			AutoSize = true;
			HeaderText = string.Empty;
			Editable = false;
		}
		
		public override void Initialize ()
		{
			base.Initialize ();
			this.DataCell = new TextCell (Widget.Generator);
		}
		
		internal void Loaded (GridViewHandler gridHandler, int column)
		{
			if (this.AutoSize) {
				Control.SizeToFit ();
				float width = Control.DataCell.CellSize.Width;
				if (gridHandler.ShowHeader)
					width = Math.Max (Control.HeaderCell.CellSize.Width, width);
					
				if (dataCell != null) {
					var rect = gridHandler.Table.VisibleRect ();
					var range = gridHandler.Table.RowsInRect (rect);
					var cellSize = Control.DataCell.CellSize;
					var dataCellHandler = ((ICellHandler)dataCell.Handler);
					for (int i = range.Location; i < range.Location + range.Length; i++) {
						var item = gridHandler.DataStore.GetItem (i);
						var val = item.GetValue (column);
						var cellWidth = dataCellHandler.GetPreferredSize (val, cellSize);
						width = Math.Max (width, cellWidth);
					}
				}
				Control.Width = width;
			}
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

		public bool Sortable {
			get {
				return Control.SortDescriptorPrototype != null;
			}
			set {
				if (value) {
					var descriptor = Control.SortDescriptorPrototype;
					if (descriptor == null) {
						descriptor = new NSSortDescriptor (Guid.NewGuid ().ToString (), true);
						Control.SortDescriptorPrototype = descriptor;
					}
				} else {
					Control.SortDescriptorPrototype = null;
				}
			}
		}
		
		public bool Editable {
			get {
				return Control.Editable;
			}
			set {
				Control.Editable = value;
				if (Control.DataCell != null) {
					Control.DataCell.Editable = value;
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
					Control.DataCell = ((ICellHandler)dataCell.Handler).Control;
					Control.DataCell.Editable = editable;
				} else
					Control.DataCell = null;
			}
		}
		
		public void Setup (int column)
		{
			Control.Identifier = new EtoGridColumnIdentifier{ Handler = this, Column = column };
		}
		
		public NSObject GetObjectValue (object val)
		{
			return ((ICellHandler)dataCell.Handler).GetObjectValue (val);
		}
		
		public object SetObjectValue (NSObject val)
		{
			return ((ICellHandler)dataCell.Handler).SetObjectValue (val);
		}
	}
}

