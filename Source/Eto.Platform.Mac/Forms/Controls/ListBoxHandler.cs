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
		List<IListItem> data = new List<IListItem> ();
		
		class DataSource : NSTableViewDataSource
		{
			public ListBoxHandler Handler { get; set; }
			
			public override NSObject GetObjectValue (NSTableView tableView, NSTableColumn tableColumn, int row)
			{
				return new MacImageData(Handler.data [row]);
			}

			public override int GetRowCount (NSTableView tableView)
			{
				return Handler.data.Count;
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
		
		class MyTableView : NSTableView
		{
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
		
		public ListBoxHandler ()
		{
			control = new MyTableView{ Handler = this };
			
			var col = new NSTableColumn ();
			col.ResizingMask = NSTableColumnResizing.Autoresizing;
			col.Editable = false;
			col.DataCell = new MacImageListItemCell ();
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
		
		#region IListControl Members
		
		public void AddRange (IEnumerable<IListItem> collection)
		{
			data.AddRange (collection);
			control.ReloadData ();
		}
		
		public void AddItem (IListItem item)
		{
			data.Add (item);
			control.ReloadData ();
		}

		public void RemoveItem (IListItem item)
		{
			data.Remove (item);
			control.ReloadData ();
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

		public void RemoveAll ()
		{
			data.Clear ();
			control.ReloadData ();
		}

		#endregion

	}
}
