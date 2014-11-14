using System;
using Eto.Forms;
using System.Collections.Generic;
using Eto.Mac.Forms.Controls;
using Eto.Drawing;
using Eto.Mac.Drawing;
using System.Collections;
using System.Linq;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using nnint = System.Int32;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#if Mac64
using CGSize = MonoMac.Foundation.NSSize;
using CGRect = MonoMac.Foundation.NSRect;
using CGPoint = MonoMac.Foundation.NSPoint;
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
using nnint = System.UInt64;
#else
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
using nnint = System.Int32;
#endif
#endif

namespace Eto.Mac.Forms.Controls
{
	public class EtoScrollView : NSScrollView, IMacControl
	{
		public WeakReference WeakHandler { get; set; }

		public object Handler { get { return WeakHandler.Target; } set { WeakHandler = new WeakReference(value); } }
	}

	public class ListBoxHandler : MacControl<NSTableView, ListBox, ListBox.ICallback>, ListBox.IHandler
	{
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

			public override NSObject GetObjectValue(NSTableView tableView, NSTableColumn tableColumn, nint row)
			{
				var w = Handler.Widget;
				var item = Handler.collection.ElementAt((int)row);
				return new MacImageData
				{
					Text = new NSString(Convert.ToString(w.TextBinding.GetValue(item))),
					Image = w.ImageBinding != null ? ((Image)w.ImageBinding.GetValue(item)).ToNS() : null
				};
			}

			public override nint GetRowCount(NSTableView tableView)
			{
				return Handler.collection.Collection == null ? 0 : Handler.collection.Collection.Count();
			}
		}

		class Delegate : NSTableViewDelegate
		{
			WeakReference handler;

			public ListBoxHandler Handler { get { return (ListBoxHandler)handler.Target; } set { handler = new WeakReference(value); } }

			public override bool ShouldSelectRow(NSTableView tableView, nint row)
			{
				return true;
			}

			public override void SelectionDidChange(NSNotification notification)
			{
				Handler.Callback.OnSelectedIndexChanged(Handler.Widget, EventArgs.Empty);
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

		public override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);

			if (!e.Handled && e.Key == Keys.Enter)
			{
				Callback.OnActivated(Widget, EventArgs.Empty);
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

		static void HandleDoubleClick(object sender, EventArgs e)
		{
			var handler = GetHandler(sender) as ListBoxHandler;
			if (handler != null)
				handler.Callback.OnActivated(handler.Widget, EventArgs.Empty);
		}

		public override Font Font
		{
			get
			{
				if (font == null)
					font = new Font(new FontHandler(Control.Font));
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

		class CollectionHandler : EnumerableChangedHandler<object>
		{
			public ListBoxHandler Handler { get; set; }

			public override void AddRange(IEnumerable<object> items)
			{
				Handler.Control.ReloadData();
			}

			public override void AddItem(object item)
			{
				Handler.Control.ReloadData();
			}

			public override void InsertItem(int index, object item)
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

		public IEnumerable<object> DataStore
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
			get { return (int)Control.SelectedRow; }
			set
			{
				if (value == -1)
					Control.DeselectAll(Control);
				else
				{
					Control.SelectRow((nnint)value, false);
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

		public override Color BackgroundColor
		{
			get { return Control.BackgroundColor.ToEto(); }
			set { Control.BackgroundColor = value.ToNSUI(); }
		}

		public Color TextColor
		{
			get { return cell.TextColor.ToEto(); }
			set
			{ 
				cell.TextColor = value.ToNSUI();
				Control.SetNeedsDisplay();
			}
		}
	}
}
