using System;
using Eto.Forms;
using System.Collections.Generic;
using Eto.Mac.Forms.Controls;
using Eto.Drawing;
using Eto.Mac.Drawing;
using System.Collections;
using System.Linq;

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

		public override NSView ContainerControl => scroll;

		public NSScrollView Scroll => scroll;

		public override NSView TextInputControl => Control;

		public class EtoDataSource : NSTableViewDataSource
		{
			WeakReference handler;

			public ListBoxHandler Handler { get { return (ListBoxHandler)handler.Target; } set { handler = new WeakReference(value); } }

			public override NSObject GetObjectValue(NSTableView tableView, NSTableColumn tableColumn, nint row)
			{
				var h = Handler;
				if (h == null)
					return null;
				var w = h.Widget;
				var item = h.collection.ElementAt((int)row);
				return new MacImageData
				{
					Text = new NSString(Convert.ToString(w.ItemTextBinding.GetValue(item))),
					Image = w.ItemImageBinding != null ? ((Image)w.ItemImageBinding.GetValue(item)).ToNS() : null
				};
			}

			public override nint GetRowCount(NSTableView tableView)
			{
				var h = Handler;
				if (h == null)
					return 0;
				return h.collection.Collection == null ? 0 : h.collection.Collection.Count();
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
				var h = Handler;
				if (h == null)
					return;
				h.Callback.OnSelectedIndexChanged(h.Widget, EventArgs.Empty);
			}

			public override nfloat GetRowHeight(NSTableView tableView, nint row)
			{
				var h = Handler;
				if (h == null)
					return tableView.RowHeight;
				return h.Control.GetCell(0, row).CellSize.Height;
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
				var h = Handler;
				if (h?.ContextMenu != null)
					return h.ContextMenu.ControlObject as NSMenu;
				return base.MenuForEvent(theEvent);
			}

			public EtoListBoxTableView()
			{
				HeaderView = null;
			}

			public EtoListBoxTableView(IntPtr handle) : base(handle)
			{
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

		public ContextMenu ContextMenu { get; set; }

		protected override NSTableView CreateControl() => new EtoListBoxTableView();
			
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
			scroll.DrawsBackground = false;
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
				if (Widget.Properties.TrySet(MacControl.Font_Key, value))
				{
					if (value != null)
					{
						var fontHandler = (FontHandler)value.Handler;
						cell.Font = fontHandler.Control;
					}
					else
						cell.Font = NSFont.SystemFontOfSize(NSFont.SystemFontSize);
					Control.ReloadData();
					InvalidateMeasure();
				};
			}
		}

		class CollectionHandler : EnumerableChangedHandler<object>
		{
			public ListBoxHandler Handler { get; set; }

			protected override void InitializeCollection()
			{
				Handler.Control.ReloadData();
				Handler.InvalidateMeasure();
			}

			public override void AddRange(IEnumerable<object> items)
			{
				Handler.Control.ReloadData();
				Handler.InvalidateMeasure();
			}

			public override void AddItem(object item)
			{
				Handler.Control.ReloadData();
				Handler.InvalidateMeasure();
			}

			public override void InsertItem(int index, object item)
			{
				Handler.Control.ReloadData();
				Handler.InvalidateMeasure();
			}

			public override void RemoveItem(int index)
			{
				Handler.Control.ReloadData();
				Handler.InvalidateMeasure();
			}

			public override void RemoveAllItems()
			{
				Handler.Control.ReloadData();
				Handler.InvalidateMeasure();
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
					Control.SelectRow((nint)value, false);
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

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			bool isInfinity = float.IsPositiveInfinity(availableSize.Width) && float.IsPositiveInfinity(availableSize.Height);

			if (isInfinity)
			{
				var naturalSizeInfinity = NaturalSizeInfinity;
				if (naturalSizeInfinity != null)
					return naturalSizeInfinity.Value;
			}
			else
			{
				var naturalAvailableSize = availableSize.TruncateInfinity();
				var naturalSize = NaturalSize;
				if (naturalSize != null && NaturalAvailableSize == naturalAvailableSize)
					return naturalSize.Value;
				NaturalAvailableSize = naturalAvailableSize;
			}

			var intercellSpacing = Control.IntercellSpacing;
			var count = Control.RowCount;
			var height = (int)((Control.RowHeight + intercellSpacing.Height) * count);

			// we need to go through each item to calculate its preferred size
			var size = new CGSize(0, height);
			var cell = new MacImageTextView();
			var font = Font.ToNS();
			if (font != null)
				cell.TextField.Font = font;
			var dataSource = Control.DataSource;
			var column = Control.TableColumns()[0];
			for (nint i = 0; i < count; i++)
			{
				var data = dataSource.GetObjectValue(Control, column, i) as MacImageData;
				if (data != null)
				{
					cell.TextField.ObjectValue = data.Text;
					cell.Image = data.Image;
					size.Width = (nfloat)Math.Max(size.Width, cell.FittingSize.Width);
				}
			}

			var frameSize = scroll.FrameSizeForContentSize(size, false, false);

			var etoFrameSize = frameSize.ToEto();

			if (isInfinity)
				NaturalSizeInfinity = etoFrameSize;
			else
				NaturalSize = etoFrameSize;

			return etoFrameSize;
		}

	}
}
