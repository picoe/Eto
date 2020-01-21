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
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#if Mac64
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#if SDCOMPAT
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
#endif
#endif

#if XAMMAC
using nnint = System.Int32;
#elif Mac64
using nnint = System.UInt64;
#else
using nnint = System.UInt32;
#endif


namespace Eto.Mac.Forms.Controls
{
	public class EtoScrollView : NSScrollView, IMacControl
	{
		public WeakReference WeakHandler { get; set; }

		public object Handler { get { return WeakHandler.Target; } set { WeakHandler = new WeakReference(value); } }

		NSBorderType? _borderType;

		public override NSBorderType BorderType
		{
			get { return _borderType ?? base.BorderType; }
			set
			{
				base.BorderType = value;
				_borderType = value;
			}
		}

		public override void SetFrameSize(CGSize newSize)
		{
			if (_borderType != null)
			{
				// when we're below 2,2, turn off the border so we don't get constraint warnings
				if (newSize.Width < 2 || newSize.Height < 2)
					base.BorderType = NSBorderType.NoBorder;
				else
					base.BorderType = _borderType.Value;
			}
			base.SetFrameSize(newSize);


			var h = Handler as IMacViewHandler;
			if (h == null)
				return;

			h.OnSizeChanged(EventArgs.Empty);
			h.Callback.OnSizeChanged(h.Widget, EventArgs.Empty);
		}
	}

	public class ListBoxHandler : MacControl<NSTableView, ListBox, ListBox.ICallback>, ListBox.IHandler
	{
		NSScrollView scroll;
		CollectionHandler collection;
		MacImageListItemCell cell;

		public override NSView ContainerControl
		{
			get { return scroll; }
		}

		public NSScrollView Scroll
		{
			get { return scroll; }
		}

		public class EtoDataSource : NSTableViewDataSource
		{
			WeakReference handler;

			public ListBoxHandler Handler { get { return (ListBoxHandler)handler.Target; } set { handler = new WeakReference(value); } }

			public override NSObject GetObjectValue(NSTableView tableView, NSTableColumn tableColumn, nint row)
			{
				var w = Handler.Widget;
				var item = Handler.collection.ElementAt((int)row);
				return new MacImageData
				{
					Text = new NSString(Convert.ToString(w.ItemTextBinding.GetValue(item))),
					Image = w.ItemImageBinding != null ? ((Image)w.ItemImageBinding.GetValue(item)).ToNS() : null
				};
			}

			public override nint GetRowCount(NSTableView tableView)
			{
				return Handler.collection.Collection == null ? 0 : Handler.collection.Collection.Count();
			}
		}

		public class EtoDelegate : NSTableViewDelegate
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

			public override nfloat GetRowHeight(NSTableView tableView, nint row)
			{
				return Handler.Control.GetCell(0, row).CellSize.Height;
			}
		}

		public class EtoListBoxTableView : NSTableView, IMacControl
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

			public EtoListBoxTableView()
			{
				HeaderView = null;
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

		protected override NSTableView CreateControl()
		{
			return new EtoListBoxTableView();
		}
			
		protected override void Initialize()
		{
			collection = new CollectionHandler { Handler = this };
			var col = new NSTableColumn();
			col.ResizingMask = NSTableColumnResizing.Autoresizing;
			col.Editable = false;
			cell = new MacImageListItemCell();
			cell.VerticalAlignment = VerticalAlignment.Center;
			cell.Wraps = false;
			col.DataCell = cell;
			Control.AddColumn(col);

			Control.DoubleClick += HandleDoubleClick;
			Control.DataSource = new EtoDataSource { Handler = this };
			Control.Delegate = new EtoDelegate { Handler = this };

			scroll = new EtoScrollView { Handler = this };
			scroll.AutoresizesSubviews = true;
			scroll.DocumentView = Control;
			scroll.HasVerticalScroller = true;
			scroll.HasHorizontalScroller = true;
			scroll.AutohidesScrollers = true;
			scroll.BorderType = NSBorderType.BezelBorder;

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
			get { return base.Font; }
			set
			{
				Widget.Properties.Set(MacControl.Font_Key, value, () =>
				{
					if (value != null)
					{
						var fontHandler = (FontHandler)value.Handler;
						cell.Font = fontHandler.Control;
					}
					else
						cell.Font = NSFont.SystemFontOfSize(NSFont.SystemFontSize);
					Control.ReloadData();
				});
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

		IIndirectBinding<string> itemTextBinding;
		public IIndirectBinding<string> ItemTextBinding
		{
			get => itemTextBinding;
			set
			{
				itemTextBinding = value;
				Control.ReloadData();
			}
		}
		public IIndirectBinding<string> ItemKeyBinding { get; set; }
	}
}
