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
				var colHandler = Handler.GetColumn (tableColumn);
				if (colHandler != null) {
					return colHandler.GetObjectValue (item);
				}
				return null;
			}

			public override void SetObjectValue (NSTableView tableView, NSObject theObject, NSTableColumn tableColumn, int row)
			{
				var item = Handler.collection.Collection [row];
				var colHandler = Handler.GetColumn (tableColumn);
				if (colHandler != null) {
					colHandler.SetObjectValue (item, theObject);
					
					Handler.Widget.OnEndCellEdit (new GridViewCellArgs ((GridColumn)colHandler.Widget, row, colHandler.Column, item));
				}
			}
		}

		class EtoTableDelegate : NSTableViewDelegate
		{
			public GridViewHandler Handler { get; set; }

			public override bool ShouldEditTableColumn (NSTableView tableView, NSTableColumn tableColumn, int row)
			{
				var colHandler = Handler.GetColumn (tableColumn);
				var item = Handler.collection.Collection [row];
				var args = new GridViewCellArgs (colHandler.Widget, row, colHandler.Column, item);
				Handler.Widget.OnBeginCellEdit (args);
				return true;
			}
					
			public override void SelectionDidChange (NSNotification notification)
			{
				Handler.Widget.OnSelectionChanged ();

				// Trigger CellClick
				var tableView = Handler.Control;
				var row = tableView.SelectedRow;
				var col = tableView.SelectedColumn;
				if (row >= 0) // && col >= 0) TODO: Fix the column
					Handler.Widget.OnCellClick (
						new GridViewCellArgs (null, // TODO: col is always -1 currently, so this does not work: Handler.GetColumn (tableView.ClickedColumn).Widget,
		                     row, col, Handler.collection.Collection [row]));					
			}

			public override void DidClickTableColumn (NSTableView tableView, NSTableColumn tableColumn)
			{
				var colHandler = Handler.GetColumn (tableColumn);
				Handler.Widget.OnColumnHeaderClick (new GridColumnEventArgs (colHandler.Widget));
			}

			public override void WillDisplayCell (NSTableView tableView, NSObject cell, NSTableColumn tableColumn, int row)
			{
				var colHandler = Handler.GetColumn (tableColumn);
				var item = Handler.GetItem (row);
				Handler.OnCellFormatting(colHandler.Widget, item, row, cell as NSCell);

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

		protected override void Initialize ()
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
		
		class CollectionHandler : DataStoreChangedHandler<object, IDataStore>
		{
			public GridViewHandler Handler { get; set; }

			public override int IndexOf (object item)
			{
				return -1; // not needed
			}
			
			public override void AddRange (IEnumerable<object> items)
			{
				Handler.Control.ReloadData ();
			}

			public override void AddItem (object item)
			{
				Handler.Control.ReloadData ();
			}

			public override void InsertItem (int index, object item)
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

		public IDataStore DataStore {
			get { return collection != null ? collection.Collection : null; }
			set {
				if (collection != null)
					collection.Unregister ();
				collection = new CollectionHandler{ Handler = this };
				collection.Register (value);
				if (Widget.Loaded)
					ResizeAllColumns ();
			}
		}
		
		public override object GetItem (int row)
		{
			return collection.Collection [row];
		}
	}
}

