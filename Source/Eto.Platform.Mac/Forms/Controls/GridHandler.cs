using System;
using MonoMac.AppKit;
using Eto.Forms;
using System.Collections.Generic;
using MonoMac.Foundation;
using Eto.Platform.Mac.Forms.Menu;
using System.Linq;

namespace Eto.Platform.Mac.Forms.Controls
{
	public interface IGridHandler
	{
		Grid Widget { get; }
		
		NSTableView Table { get; }
	}
	
	class EtoScrollView : NSScrollView
	{
		public IGridHandler Handler { get; set; }

		bool autoSized;
		
		public override void SetFrameSize (System.Drawing.SizeF newSize)
		{
			base.SetFrameSize (newSize);
			if (!autoSized && Handler.Widget.Loaded) {
				var rect = Handler.Table.VisibleRect ();
				if (!rect.IsEmpty) {
					foreach (var col in Handler.Widget.Columns) {
						((GridColumnHandler)col.Handler).Resize ();
					}
					autoSized = true;
				}
			}
		}
	}
	
	class EtoTableHeaderView : NSTableHeaderView
	{
		public IGridHandler Handler { get; set; }
			
		public override void MouseDown (NSEvent theEvent)
		{
			var col = base.GetColumn (base.ConvertPointFromBase (theEvent.LocationInWindow));
			if (col >= 0) {
				var column = Handler.Widget.Columns [col];
				if (!column.Sortable)
					return;
			}
			base.MouseDown (theEvent);
		}
	}
	
	public abstract class GridHandler<T, W> : MacView<T, W>, IGrid, IDataViewHandler, IGridHandler
		where T: NSTableView
		where W: Grid
	{
		ColumnCollection columns;
		ContextMenu contextMenu;
		
		public NSTableView Table {
			get { return Control; }
		}
		
		public NSScrollView ScrollView { get; private set; }
		
		public override NSView ContainerControl { get { return ScrollView; } }
		
		protected virtual void PreUpdateColumn (int index)
		{
		}
		
		protected virtual void UpdateColumns ()
		{
		}
		
		
		class ColumnCollection : EnumerableChangedHandler<GridColumn, GridColumnCollection>
		{
			public GridHandler<T,W> Handler { get; set; }
			
			public override void AddItem (GridColumn item)
			{
				var colhandler = (GridColumnHandler)item.Handler;
				Handler.Control.AddColumn (colhandler.Control);
				colhandler.Setup (Handler.Control.ColumnCount - 1);
				
				Handler.UpdateColumns ();
			}

			public override void InsertItem (int index, GridColumn item)
			{
				var outline = Handler.Control;
				var columns = new List<NSTableColumn> (outline.TableColumns ());
				Handler.PreUpdateColumn (index);
				for (int i = index; i < columns.Count; i++) {
					outline.RemoveColumn (columns [i]);
				}
				var colhandler = (GridColumnHandler)item.Handler;
				columns.Insert (index, colhandler.Control);
				outline.AddColumn (colhandler.Control);
				colhandler.Setup (index);
				for (int i = index + 1; i < columns.Count; i++) {
					var col = columns [i];
					var id = col.Identifier as EtoDataColumnIdentifier;
					id.Handler.Setup (i);
					outline.AddColumn (col);
				}
				Handler.UpdateColumns ();
			}

			public override void RemoveItem (int index)
			{
				var outline = Handler.Control;
				var columns = new List<NSTableColumn> (outline.TableColumns ());
				Handler.PreUpdateColumn (index);
				for (int i = index; i < columns.Count; i++) {
					outline.RemoveColumn (columns [i]);
				}
				columns.RemoveAt (index);
				for (int i = index; i < columns.Count; i++) {
					var col = columns [i];
					var id = col.Identifier as EtoDataColumnIdentifier;
					id.Handler.Setup (i);
					outline.AddColumn (col);
				}
				Handler.UpdateColumns ();
			}

			public override void RemoveAllItems ()
			{
				Handler.PreUpdateColumn (0);
				foreach (var col in Handler.Control.TableColumns ())
					Handler.Control.RemoveColumn (col);
				Handler.UpdateColumns ();
			}

		}

		
		public GridHandler ()
		{
			ScrollView = new EtoScrollView {
				Handler = this,
				HasVerticalScroller = true,
				HasHorizontalScroller = true,
				AutohidesScrollers = true,
				BorderType = NSBorderType.BezelBorder
			};
		}
		
		public override void AttachEvent (string handler)
		{
			switch (handler) {
			default:
				base.AttachEvent (handler);
				break;
			}
		}

		public override void Initialize ()
		{
			base.Initialize ();
			columns = new ColumnCollection { Handler = this };
			columns.Register (Widget.Columns);

			Control.HeaderView = new EtoTableHeaderView { Handler = this };
			ScrollView.DocumentView = Control;
		}
		
		public override void OnLoadComplete (EventArgs e)
		{
			base.OnLoadComplete (e);
			
			int i = 0;
			foreach (var col in this.Widget.Columns) {
				var colHandler = (GridColumnHandler)col.Handler;
				colHandler.Loaded (this, i++);
				colHandler.Resize ();
			}
		}

		public bool ShowHeader {
			get {
				return Control.HeaderView != null;
			}
			set {
				if (value && Control.HeaderView == null) {
					Control.HeaderView = new EtoTableHeaderView { Handler = this };
				} else if (!value && Control.HeaderView != null) {
					Control.HeaderView = null;
				}
			}
		}

		public override bool Enabled {
			get { return Control.Enabled; }
			set { Control.Enabled = value; }
		}
		
		public bool AllowColumnReordering {
			get { return Control.AllowsColumnReordering; }
			set { Control.AllowsColumnReordering = value; }
		}
		
		public ContextMenu ContextMenu {
			get { return contextMenu; }
			set {
				contextMenu = value;
				if (contextMenu != null)
					Control.Menu = ((ContextMenuHandler)contextMenu.Handler).Control;
				else
					Control.Menu = null;
			}
		}

		public bool AllowMultipleSelection {
			get { return Control.AllowsMultipleSelection; }
			set { Control.AllowsMultipleSelection = value; }
		}

		public IEnumerable<int> SelectedRows {
			get { return Control.SelectedRows.Select (r => (int)r); }
		}

		public void SelectAll ()
		{
			Control.SelectAll (Control);
		}

		public void SelectRow (int row)
		{
			Control.SelectRow (row, false);
		}

		public void UnselectRow (int row)
		{
			Control.DeselectRow (row);
		}

		public void UnselectAll ()
		{
			Control.DeselectAll (Control);
		}
		
		public abstract object GetItem (int row);
		
		public virtual int RowCount {
			get { return Control.RowCount; }
		}
		
		Grid IGridHandler.Widget {
			get { return Widget; }
		}
		
		public System.Drawing.RectangleF GetVisibleRect ()
		{
			return ScrollView.VisibleRect ();
		}
	}
}

