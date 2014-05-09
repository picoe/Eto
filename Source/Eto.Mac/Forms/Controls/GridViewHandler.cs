using System;
using MonoMac.AppKit;
using Eto.Forms;
using System.Collections.Generic;
using MonoMac.Foundation;
using Eto.Drawing;
using sd = System.Drawing;

namespace Eto.Mac.Forms.Controls
{
	public class GridViewHandler : GridHandler<NSTableView, GridView, GridView.ICallback>, IGridView
	{
		CollectionHandler collection;

		public class EtoTableView : NSTableView, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public GridViewHandler Handler
			{ 
				get { return (GridViewHandler)WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); } 
			}
			
			/// <summary>
			/// The area to the right and below the rows is not filled with the background
			/// color. This fixes that. See http://orangejuiceliberationfront.com/themeing-nstableview/
			/// </summary>
			public override void DrawBackground(sd.RectangleF clipRect)
			{
				var backgroundColor = Handler.BackgroundColor;
				if (backgroundColor != Colors.Transparent) {
					backgroundColor.ToNSUI ().Set ();
					NSGraphics.RectFill (clipRect);
				} else
					base.DrawBackground (clipRect);
			}
		}
		
		class EtoTableViewDataSource : NSTableViewDataSource
		{
			WeakReference handler;
			public GridViewHandler Handler { get { return (GridViewHandler)(handler != null ? handler.Target : null); } set { handler = new WeakReference(value); } }
			
			public override int GetRowCount (NSTableView tableView)
			{
				return (Handler.collection != null && Handler.collection.Collection != null) ? Handler.collection.Collection.Count : 0;
			}

			public override NSObject GetObjectValue (NSTableView tableView, NSTableColumn tableColumn, int row)
			{
				var item = Handler.collection.Collection [row];
				var colHandler = Handler.GetColumn (tableColumn);
				return colHandler == null ? null : colHandler.GetObjectValue(item);
			}

			public override void SetObjectValue (NSTableView tableView, NSObject theObject, NSTableColumn tableColumn, int row)
			{
				var item = Handler.collection.Collection [row];
				var colHandler = Handler.GetColumn (tableColumn);
				if (colHandler != null) {
					colHandler.SetObjectValue (item, theObject);
					
					Handler.Callback.OnCellEdited(Handler.Widget, new GridViewCellArgs(colHandler.Widget, row, colHandler.Column, item));
				}
			}
		}

		class EtoTableDelegate : NSTableViewDelegate
		{
			WeakReference handler;
			public GridViewHandler Handler { get { return (GridViewHandler)(handler != null ? handler.Target : null); } set { handler = new WeakReference(value); } }

			public override bool ShouldEditTableColumn (NSTableView tableView, NSTableColumn tableColumn, int row)
			{
				var colHandler = Handler.GetColumn (tableColumn);
				var item = Handler.collection.Collection [row];
				var args = new GridViewCellArgs (colHandler.Widget, row, colHandler.Column, item);
				Handler.Callback.OnCellEditing(Handler.Widget, args);
				return true;
			}
					
			public override void SelectionDidChange (NSNotification notification)
			{
				Handler.Callback.OnSelectionChanged(Handler.Widget, EventArgs.Empty);

				// Trigger CellClick
				var tableView = Handler.Control;
				var row = tableView.SelectedRow;
				var col = tableView.SelectedColumn;
				if (row >= 0) // && col >= 0) TODO: Fix the column
					Handler.Callback.OnCellClick (Handler.Widget,
						new GridViewCellArgs (null, // TODO: col is always -1 currently, so this does not work: Handler.GetColumn (tableView.ClickedColumn).Widget,
		                     row, col, Handler.collection.Collection [row]));					
			}

			public override void DidClickTableColumn (NSTableView tableView, NSTableColumn tableColumn)
			{
				var colHandler = Handler.GetColumn (tableColumn);
				Handler.Callback.OnColumnHeaderClick(Handler.Widget, new GridColumnEventArgs (colHandler.Widget));
			}

			public override void WillDisplayCell (NSTableView tableView, NSObject cell, NSTableColumn tableColumn, int row)
			{
				var colHandler = Handler.GetColumn (tableColumn);
				var item = Handler.GetItem (row);
				Handler.OnCellFormatting(colHandler.Widget, item, row, cell as NSCell);

			}
		}

		public bool ShowCellBorders
		{
			get { return Control.IntercellSpacing.Width > 0 || Control.IntercellSpacing.Height > 0; }
			set { Control.IntercellSpacing = value ? new sd.SizeF(1, 1) : sd.SizeF.Empty; } 
		}

		public override void AttachEvent (string id)
		{
			switch (id) {
			case Grid.CellEditingEvent:
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
			case Grid.CellEditedEvent:
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
				base.AttachEvent (id);
				break;
			}
		}

		public GridViewHandler()
		{
			Control = new EtoTableView
			{
				Handler = this,
				FocusRingType = NSFocusRingType.None,
				DataSource = new EtoTableViewDataSource { Handler = this },
				Delegate = new EtoTableDelegate { Handler = this },
				ColumnAutoresizingStyle = NSTableViewColumnAutoresizingStyle.None
			};
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

