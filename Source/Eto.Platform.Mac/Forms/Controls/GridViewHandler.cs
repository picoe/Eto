using System;
using MonoMac.AppKit;
using Eto.Forms;
using System.Collections.Generic;
using MonoMac.Foundation;
using Eto.Platform.Mac.Forms.Menu;
using System.Linq;
using Eto.Platform.Mac.Drawing;
using Eto.Drawing;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class GridViewHandler : GridHandler<NSTableView, GridView>, IGridView
	{
		CollectionHandler collection;

		public class EtoTableView : NSTableView, IMacControl
		{
			public GridViewHandler Handler { get; set; }

			object IMacControl.Handler { get { return Handler; } }
		}
		
		class EtoTableViewDataSource : NSTableViewDataSource
		{
			public GridViewHandler Handler { get; set; }
			
			public override int GetRowCount (NSTableView tableView)
			{
				return (Handler.collection != null && Handler.collection.Collection != null) ? Handler.collection.Collection.Count : 0;
			}

			public override NSObject GetObjectValue (NSTableView tableView, NSTableColumn tableColumn, int row)
			{
				var item = Handler.collection.Collection [row];
				var id = tableColumn.Identifier as EtoDataColumnIdentifier;
				if (id != null) {
					return id.Handler.GetObjectValue (item);
				}
				return null;
			}

			public override void SetObjectValue (NSTableView tableView, NSObject theObject, NSTableColumn tableColumn, int row)
			{
				var item = Handler.collection.Collection [row];
				var id = tableColumn.Identifier as EtoDataColumnIdentifier;
				if (id != null) {
					id.Handler.SetObjectValue (item, theObject);
					
					Handler.Widget.OnEndCellEdit (new GridViewCellArgs ((GridColumn)id.Handler.Widget, row, id.Column, item));
				}
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

			public override Font Font {
				get { return font; }
				set {
					font = value;
					if (font != null)
						Cell.Font = ((FontHandler)font.Handler).Control;
					else
						Cell.Font = null;
				}
			}

			public override Color BackgroundColor {
				get { return CellHandler.GetBackgroundColor (Cell); }
				set { CellHandler.SetBackgroundColor (Cell, value); }
			}

			public override Color ForegroundColor {
				get { return CellHandler.GetForegroundColor (Cell); }
				set { CellHandler.SetForegroundColor (Cell, value); }
			}
		}
		
		class EtoTableDelegate : NSTableViewDelegate
		{
			public GridViewHandler Handler { get; set; }

			public override bool ShouldEditTableColumn (NSTableView tableView, NSTableColumn tableColumn, int row)
			{
				var id = tableColumn.Identifier as EtoDataColumnIdentifier;
				var item = Handler.collection.Collection [row];
				var args = new GridViewCellArgs ((GridColumn)id.Handler.Widget, row, id.Column, item);
				Handler.Widget.OnBeginCellEdit (args);
				return true;
			}
			
			public override void SelectionDidChange (NSNotification notification)
			{
				Handler.Widget.OnSelectionChanged (EventArgs.Empty);
			}

			public override void DidClickTableColumn (NSTableView tableView, NSTableColumn tableColumn)
			{
				var id = tableColumn.Identifier as EtoDataColumnIdentifier;
				var column = Handler.Widget.Columns[id.Column];
				// var column = Handler.Widget.Columns.First (r => object.ReferenceEquals (r.ControlObject, tableColumn));
				Handler.Widget.OnColumnHeaderClick (new GridColumnEventArgs (column));
			}

			public override void WillDisplayCell (NSTableView tableView, NSObject cell, NSTableColumn tableColumn, int row)
			{
				var id = tableColumn.Identifier as EtoDataColumnIdentifier;
				var column = Handler.Widget.Columns[id.Column];
				//var column = Handler.Widget.Columns.First (r => object.ReferenceEquals (r.ControlObject, tableColumn));
				var item = Handler.GetItem (row);
				Handler.Widget.OnCellFormatting(new MacCellFormatArgs(column, item, row, cell as NSCell));

			}
		}

		public GridViewHandler ()
		{
		}

		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case Grid.BeginCellEditEvent:
				// handled by delegate
				/* following should work, but internal delegate to trigger event does not work
				table.ShouldEditTableColumn = (tableView, tableColumn, row) => {
					var id = tableColumn.Identifier as EtoGridColumnIdentifier;
					var item = store.GetItem (row);
					var args = new GridViewCellArgs(id.Handler.Widget, row, id.Handler.Column, item);
					this.Widget.OnBeginCellEdit (args);
					return true;
				};*/
				break;
			case Grid.EndCellEditEvent:
				// handled after object value is set
				break;
			case Grid.SelectionChangedEvent:
				/* handled by delegate, for now
				table.SelectionDidChange += delegate {
					Widget.OnSelectionChanged (EventArgs.Empty);
				};*/
				break;
			case Grid.ColumnHeaderClickEvent:
				/*
				table.DidClickTableColumn += delegate(object sender, NSTableViewTableEventArgs e) {
					var column = Handler.Widget.Columns.First (r => object.ReferenceEquals (r.ControlObject, tableColumn));
					Handler.Widget.OnHeaderClick (new GridColumnEventArgs (column));
				};
				*/
				break;
			case Grid.CellFormattingEvent:
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
		}

		public override void Initialize ()
		{
			Control = new EtoTableView {
				Handler = this,
				FocusRingType = NSFocusRingType.None,
				DataSource = new EtoTableViewDataSource { Handler = this },
				Delegate = new EtoTableDelegate { Handler = this },
				ColumnAutoresizingStyle = NSTableViewColumnAutoresizingStyle.None
			};
			
			base.Initialize ();
		}
		
		class CollectionHandler : DataStoreChangedHandler<IGridItem, IGridStore>
		{
			public GridViewHandler Handler { get; set; }

			public override int IndexOf (IGridItem item)
			{
				return -1; // not needed
			}
			
			public override void AddRange (IEnumerable<IGridItem> items)
			{
				Handler.Control.ReloadData ();
			}

			public override void AddItem (IGridItem item)
			{
				Handler.Control.ReloadData ();
			}

			public override void InsertItem (int index, IGridItem item)
			{
				Handler.Control.ReloadData ();
			}

			public override void RemoveItem (int index)
			{
				Handler.Control.ReloadData ();
			}

			public override void RemoveAllItems ()
			{
				Handler.Control.ReloadData ();
			}
		}

		public IGridStore DataStore {
			get { return collection != null ? collection.Collection : null; }
			set {
				if (collection != null)
					collection.Unregister ();
				collection = new CollectionHandler{ Handler = this };
				collection.Register (value);
			}
		}
		
		public override object GetItem (int row)
		{
			return collection.Collection [row];
		}
	}
}

