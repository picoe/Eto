using System;
using Eto.Forms;
using MonoMac.AppKit;
using MonoMac.Foundation;
using System.Collections.Generic;
using Eto.Mac.Forms.Controls;
using Eto.Drawing;
using Eto.Mac.Drawing;

namespace Eto.Mac.Forms.Controls
{
	public class EtoScrollView : NSScrollView, IMacControl
	{
		public WeakReference WeakHandler { get; set; }

		public object Handler { get { return WeakHandler.Target; } set { WeakHandler = new WeakReference(value); } }
	}

	public class ListBoxHandler : MacControl<NSTableView, ListBox>, IListBox
	{
		Font font;
		readonly NSScrollView scroll;
		readonly CollectionHandler collection;
		readonly MacImageListItemCell cell;

		public override NSView ContainerControl
		{
			get { return scroll; }
		}

		public NSScrollView Scroll
		{
			get { return scroll; }
		}

		class DataSource : NSTableViewDataSource
		{
			WeakReference handler;
			public ListBoxHandler Handler { get { return (ListBoxHandler)handler.Target; } set { handler = new WeakReference(value); } }

			public override NSObject GetObjectValue(NSTableView tableView, NSTableColumn tableColumn, int row)
			{
				return new MacImageData(Handler.collection.Collection[row]);
			}

			public override int GetRowCount(NSTableView tableView)
			{
				return Handler.collection.Collection == null ? 0 : Handler.collection.Collection.Count;
			}
		}

		class Delegate : NSTableViewDelegate
		{
			WeakReference handler;
			public ListBoxHandler Handler { get { return (ListBoxHandler)handler.Target; } set { handler = new WeakReference(value); } }

			public override bool ShouldSelectRow(NSTableView tableView, int row)
			{
				return true;
			}

			public override void SelectionDidChange(NSNotification notification)
			{
				Handler.Widget.OnSelectedIndexChanged(EventArgs.Empty);
			}
		}

		class EtoListBoxTableView : NSTableView, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public ListBoxHandler Handler
			{ 
				get { return (ListBoxHandler)WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); } 
			}

			public override NSMenu MenuForEvent(NSEvent theEvent)
			{
				if (Handler.ContextMenu != null)
					return Handler.ContextMenu.ControlObject as NSMenu;
				return base.MenuForEvent(theEvent);
			}
		}

		public override void PostKeyDown(KeyEventArgs e)
		{
			if (e.Key == Keys.Enter)
			{
				Widget.OnActivated(EventArgs.Empty);
				e.Handled = true;
			}
		}

		public ContextMenu ContextMenu
		{
			get;
			set;
		}

		public override bool Enabled
		{
			get { return Control.Enabled; }
			set { Control.Enabled = value; }
		}

		public ListBoxHandler()
		{
			collection = new CollectionHandler { Handler = this };
			Control = new EtoListBoxTableView { Handler = this };
			
			var col = new NSTableColumn();
			col.ResizingMask = NSTableColumnResizing.Autoresizing;
			col.Editable = false;
			cell = new MacImageListItemCell();
			cell.Wraps = false;
			col.DataCell = cell;
			Control.AddColumn(col);

			Control.DataSource = new DataSource { Handler = this };
			Control.HeaderView = null;
			Control.DoubleClick += HandleDoubleClick;
			Control.Delegate = new Delegate { Handler = this };
			
			scroll = new EtoScrollView { Handler = this };
			scroll.AutoresizesSubviews = true;
			scroll.DocumentView = Control;
			scroll.HasVerticalScroller = true;
			scroll.HasHorizontalScroller = true;
			scroll.AutohidesScrollers = true;
			scroll.BorderType = NSBorderType.BezelBorder;
		}

		protected override void Initialize()
		{
			base.Initialize();
			HandleEvent(Eto.Forms.Control.KeyDownEvent);
		}

		static void HandleDoubleClick (object sender, EventArgs e)
		{
			var handler = GetHandler(sender) as ListBoxHandler;
			if (handler != null)
				handler.Widget.OnActivated(EventArgs.Empty);
		}

		public override Font Font
		{
			get
			{
				if (font == null)
					font = new Font(Widget.Platform, new FontHandler(Control.Font));
				return font;
			}
			set
			{
				font = value;
				if (font != null)
				{
					var fontHandler = (FontHandler)font.Handler;
					cell.Font = fontHandler.Control;
					Control.RowHeight = fontHandler.LineHeight;
				}
				else
					cell.Font = NSFont.SystemFontOfSize(NSFont.SystemFontSize);
			}
		}

		class CollectionHandler : DataStoreChangedHandler<IListItem, IListStore>
		{
			public ListBoxHandler Handler { get; set; }

			public override int IndexOf(IListItem item)
			{
				return -1; // not needed
			}

			public override void AddRange(IEnumerable<IListItem> items)
			{
				Handler.Control.ReloadData();
			}

			public override void AddItem(IListItem item)
			{
				Handler.Control.ReloadData();
			}

			public override void InsertItem(int index, IListItem item)
			{
				Handler.Control.ReloadData();
			}

			public override void RemoveItem(int index)
			{
				Handler.Control.ReloadData();
			}

			public override void RemoveAllItems()
			{
				Handler.Control.ReloadData();
			}
		}

		public IListStore DataStore
		{
			get { return collection.Collection; }
			set
			{
				if (collection.Collection != null)
					collection.Unregister();
				collection.Register(value);
			}
		}

		public int SelectedIndex
		{
			get { return Control.SelectedRow; }
			set
			{
				if (value == -1)
					Control.DeselectAll(Control);
				else
				{
					Control.SelectRow(value, false);
					Control.ScrollRowToVisible(value);
				}
			}
		}

		public override void Focus()
		{
			if (Control.Window != null)
				Control.Window.MakeFirstResponder(Control);
			else 
				base.Focus();
		}
	}
}
