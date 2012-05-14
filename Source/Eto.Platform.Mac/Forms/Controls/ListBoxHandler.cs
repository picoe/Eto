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
	public class ListBoxHandler : MacControl<NSTableView, ListBox>, IListBox
	{
		NSScrollView scroll;
		CollectionHandler collection;

		public override NSView ContainerControl
		{
			get { return scroll; }
		}

				
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
			get { return Control.Enabled; }
			set { Control.Enabled = value; }
		}
		
		public ListBoxHandler ()
		{
			collection = new CollectionHandler{ Handler = this };
			Control = new EtoListBoxTableView { Handler = this };
			
			var col = new NSTableColumn ();
			col.ResizingMask = NSTableColumnResizing.Autoresizing;
			col.Editable = false;
			var cell = new MacImageListItemCell ();
			cell.Wraps = false;
			col.DataCell = cell;
			Control.AddColumn (col);

			Control.DataSource = new DataSource { Handler = this };
			Control.HeaderView = null;
			Control.DoubleClick += delegate {
				Widget.OnActivated (EventArgs.Empty);
			};
			Control.Delegate = new Delegate { Handler = this };
			
			scroll = new NSScrollView ();
			scroll.AutoresizesSubviews = true;
			scroll.DocumentView = Control;
			scroll.HasVerticalScroller = true;
			scroll.HasHorizontalScroller = true;
			scroll.AutohidesScrollers = true;
			scroll.BorderType = NSBorderType.BezelBorder;
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
				Handler.Control.ReloadData ();
			}

			public override void AddItem (IListItem item)
			{
				Handler.Control.ReloadData ();
			}

			public override void InsertItem (int index, IListItem item)
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

		public IListStore DataStore {
			get { return collection.DataStore; }
			set {
				if (collection.DataStore != null)
					collection.Unregister ();
				collection.Register (value);
			}
		}

		public int SelectedIndex {
			get { return Control.SelectedRow; }
			set {
				if (value == -1)
					Control.DeselectAll (Control);
				else {
					Control.SelectRow (value, false);
					Control.ScrollRowToVisible (value);
				}
			}
		}

		public override void Focus ()
		{
			if (this.Control.Window != null)
				this.Control.Window.MakeFirstResponder (this.Control);
			else 
				base.Focus();
		}
		
		public override bool HasFocus {
			get {
				return Control.Window != null && Control.Window.FirstResponder == Control;
			}
		}
	}
}
