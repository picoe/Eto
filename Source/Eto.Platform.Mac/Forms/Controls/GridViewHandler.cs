using System;
using MonoMac.AppKit;
using Eto.Forms;
using System.Collections.Generic;
using MonoMac.Foundation;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class GridViewHandler : MacView<NSScrollView, GridView>, IGridView
	{
		IGridStore store;
		NSTableView table;
		
		class EtoTableViewDataSource : NSTableViewDataSource
		{
			public GridViewHandler Handler { get; set; }
			
			public override int GetRowCount (NSTableView tableView)
			{
				return (Handler.store != null) ? Handler.store.Count : 0;
			}

			public override MonoMac.Foundation.NSObject GetObjectValue (NSTableView tableView, NSTableColumn tableColumn, int row)
			{
				var item = Handler.store.GetItem (row);
				var id = tableColumn.Identifier as EtoGridColumnIdentifier;
				if (item != null && id != null) {
					var val = item.GetValue (id.Column);
					return id.Handler.GetObjectValue (val);
				}
				return null;
			}

			public override void SetObjectValue (NSTableView tableView, NSObject theObject, NSTableColumn tableColumn, int row)
			{
				var item = Handler.store.GetItem (row);
				var id = tableColumn.Identifier as EtoGridColumnIdentifier;
				if (item != null && id != null) {
					var val = id.Handler.SetObjectValue (theObject);
					item.SetValue (id.Column, val);
				}
			}
			
			
		}
		
		public GridViewHandler ()
		{
			Control = new NSScrollView ();
			Control.HasVerticalScroller = true;
			Control.HasHorizontalScroller = true;
			Control.AutohidesScrollers = true;
			Control.BorderType = NSBorderType.BezelBorder;
			
			
			table = new NSTableView ();
			table.FocusRingType = NSFocusRingType.None;
			table.DataSource = new EtoTableViewDataSource{ Handler = this };
			Control.DocumentView = table;
		}

		public void InsertColumn (int index, GridColumn column)
		{
			var colhandler = ((GridColumnHandler)column.Handler);
			if (index == -1 || index == table.ColumnCount) {
				colhandler.Setup (index);
				table.AddColumn (colhandler.Control);
			} else {
				var columns = new List<NSTableColumn> (table.TableColumns ());
				for (int i = 0; i < index; i++) {
					table.RemoveColumn (columns [i]);
				}
				columns.Insert (index, ((GridColumnHandler)column.Handler).Control);
				for (int i = index; i < columns.Count; i++) {
					var col = columns [i];
					var id = col.Identifier as EtoGridColumnIdentifier;
					if (id != null)
						id.Handler.Setup (i);
					table.AddColumn (col);
				}
			}
		}
		
		public override void OnLoadComplete (EventArgs e)
		{
			base.OnLoadComplete (e);
			
			foreach (var col in this.Widget.Columns) {
				((GridColumnHandler)col.Handler).Loaded (this);
			}
		}

		public void RemoveColumn (int index, GridColumn column)
		{
			table.RemoveColumn (((GridColumnHandler)column.Handler).Control);
		}

		public void ClearColumns ()
		{
			foreach (var col in table.TableColumns ())
				table.RemoveColumn (col);
		}

		public bool ShowHeader {
			get {
				return table.HeaderView != null;
			}
			set {
				if (value && table.HeaderView == null) {
					table.HeaderView = new NSTableHeaderView ();
				} else if (!value && table.HeaderView != null) {
					table.HeaderView = null;
				}
			}
		}

		public IGridStore DataStore {
			get { return store; }
			set {
				store = value;
				table.ReloadData ();
			}
		}
		
		public override bool Enabled {
			get { return table.Enabled; }
			set { table.Enabled = value; }
		}
		
		public bool AllowColumnReordering {
			get { return table.AllowsColumnReordering; }
			set { table.AllowsColumnReordering = value; }
		}
	}
}

