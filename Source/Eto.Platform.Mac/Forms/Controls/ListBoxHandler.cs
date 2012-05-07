using System;
using System.Reflection;
using Eto.Forms;
using MonoMac.AppKit;
using MonoMac.Foundation;
using System.Collections.Generic;
using System.Linq;
using Eto.Platform.Mac.Forms.Controls;

namespace Eto.Platform.Mac
{
	public class ListBoxHandler : MacView<NSScrollView, ListBox>, IListBox
	{
		NSTableView control;
		NSScrollView scroll;
		CollectionHandler collection;

				
		class DataSource : NSTableViewDataSource
		{
			public ListBoxHandler Handler { get; set; }
			
			public override NSObject GetObjectValue (NSTableView tableView, NSTableColumn tableColumn, int row)
			{
				return new MacImageData (Handler.collection.DataStore [row]);
			}

			public override int GetRowCount (NSTableView tableView)
			{
				return Handler.collection.DataStore != null ? Handler.collection.DataStore.Count : 0;
			}
		}
		
		class Delegate : NSTableViewDelegate
		{
			public ListBoxHandler Handler { get; set; }

			public override bool ShouldSelectRow (NSTableView tableView, int row)
			{
				return true;
			}
			
			public override void SelectionDidChange (NSNotification notification)
			{
				Handler.Widget.OnSelectedIndexChanged (EventArgs.Empty);
			}
			
		}
		
		class EtoListBoxTableView : NSTableView, IMacControl
		{
			object IMacControl.Handler { get { return Handler; } }
			
			public override NSMenu MenuForEvent (NSEvent theEvent)
			{
				if (Handler.ContextMenu != null)
					return Handler.ContextMenu.ControlObject as NSMenu;
				else
					return base.MenuForEvent (theEvent);
			}

			public ListBoxHandler Handler { get; set; }
			
			public override void KeyDown (NSEvent theEvent)
			{
				if (theEvent.KeyCode == (ushort)NSKey.Return) {
					Handler.Widget.OnActivated (EventArgs.Empty);
				} else
					base.KeyDown (theEvent);
			}
		}
		
		public ContextMenu ContextMenu {
			get;
			set;
		}
		
		public override bool Enabled {
			get { return control.Enabled; }
			set { control.Enabled = value; }
		}
		
		public override object EventObject {
			get { return control; }
		}
		
		public ListBoxHandler ()
		{
			collection = new CollectionHandler{ Handler = this };
			control = new EtoListBoxTableView{ Handler = this };
			
			var col = new NSTableColumn ();
			col.ResizingMask = NSTableColumnResizing.Autoresizing;
			col.Editable = false;
			var cell = new MacImageListItemCell ();
			cell.Wraps = false;
			col.DataCell = cell;
			control.AddColumn (col);
			
			control.DataSource = new DataSource{ Handler = this };
			control.HeaderView = null;
			control.DoubleClick += delegate {
				Widget.OnActivated (EventArgs.Empty);
			};
			control.Delegate = new Delegate { Handler = this };
			
			scroll = new NSScrollView ();
			scroll.AutoresizesSubviews = true;
			scroll.DocumentView = control;
			scroll.HasVerticalScroller = true;
			scroll.HasHorizontalScroller = true;
			scroll.AutohidesScrollers = true;
			scroll.BorderType = NSBorderType.BezelBorder;
			Control = scroll;
		}
		
		class CollectionHandler : DataStoreChangedHandler<IListItem, IListStore>
		{
			public ListBoxHandler Handler { get; set; }

			public override int IndexOf (IListItem item)
			{
				return -1; // not needed
			}
			
			public override void AddRange (IEnumerable<IListItem> items)
			{
				Handler.control.ReloadData ();
			}

			public override void AddItem (IListItem item)
			{
				Handler.control.ReloadData ();
			}

			public override void InsertItem (int index, IListItem item)
			{
				Handler.control.ReloadData ();
			}

			public override void RemoveItem (int index)
			{
				Handler.control.ReloadData ();
			}

			public override void RemoveAllItems ()
			{
				Handler.control.ReloadData ();
			}
		}

		public IListStore DataStore {
			get { return collection.DataStore; }
			set {
				if (collection.DataStore != null)
					collection.Unregister ();
				collection.Register (value);
			}
		}

		public int SelectedIndex {
			get	{ return control.SelectedRow; }
			set {
				if (value == -1)
					control.DeselectAll (control);
				else {
					control.SelectRow (value, false);
					control.ScrollRowToVisible (value);
				}
			}
		}

		public override void Focus ()
		{
			if (this.control.Window != null)
				this.control.Window.MakeFirstResponder (this.control);
			else 
				base.Focus();
		}
		
		public override bool HasFocus {
			get {
				return control.Window != null && control.Window.FirstResponder == control;
			}
		}
	}
}
