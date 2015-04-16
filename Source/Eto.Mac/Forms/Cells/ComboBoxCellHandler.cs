using System;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
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
using CGSize = MonoMac.Foundation.NSSize;
using CGRect = MonoMac.Foundation.NSRect;
using CGPoint = MonoMac.Foundation.NSPoint;
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#endif

namespace Eto.Mac.Forms.Cells
{
	public class ComboBoxCellHandler : CellHandler<NSPopUpButtonCell, ComboBoxCell, ComboBoxCell.ICallback>, ComboBoxCell.IHandler
	{
		CollectionHandler collection;

		public class EtoCell : NSPopUpButtonCell, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public object Handler
			{ 
				get { return WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); } 
			}

			public EtoCell()
			{
			}

			public EtoCell(IntPtr handle) : base(handle)
			{
			}

			public NSColor TextColor { get; set; }

			public bool DrawsBackground { get; set; }

			[Export("copyWithZone:")]
			NSObject CopyWithZone(IntPtr zone)
			{
				var ptr = Messaging.IntPtr_objc_msgSendSuper_IntPtr(SuperHandle, MacCommon.CopyWithZoneHandle, zone);
				return new EtoCell(ptr) { Handler = Handler };
			}

			public override void DrawBorderAndBackground(CGRect cellFrame, NSView controlView)
			{
				if (DrawsBackground)
				{
					var nscontext = NSGraphicsContext.CurrentContext;
					var context = nscontext.GraphicsPort;

					context.SetFillColor(BackgroundColor.CGColor);
					context.FillRect(cellFrame);
				}

				base.DrawBorderAndBackground(cellFrame, controlView);
			}

			public override CGRect DrawTitle(NSAttributedString title, CGRect frame, NSView controlView)
			{
				if (TextColor != null)
				{
					var newtitle = (NSMutableAttributedString)title.MutableCopy();
					var range = new NSRange(0, (int)title.Length);
					newtitle.RemoveAttribute(NSStringAttributeKey.ForegroundColor, range);
					newtitle.AddAttribute(NSStringAttributeKey.ForegroundColor, TextColor, range);
					title = newtitle;
				}
				var rect = base.DrawTitle(title, frame, controlView);
				return rect;
			}
		}

		class CollectionHandler : EnumerableChangedHandler<object>
		{
			public ComboBoxCellHandler Handler { get; set; }
			ILookup<string, int> indexLookupByKey;

			public ILookup<string, int> IndexLookup
			{
				get { return indexLookupByKey ?? (indexLookupByKey = Collection.ToLookup(e => Handler.Widget.ComboKeyBinding.GetValue(e), e => IndexOf(e))); }
			}

			public override void AddItem(object item)
			{
				var menu = Handler.Control.Menu;
				var textBinding = Handler.Widget.ComboTextBinding;
				menu.AddItem(new NSMenuItem(textBinding.GetValue(item)));
				indexLookupByKey = null;
			}

			public override void InsertItem(int index, object item)
			{
				var menu = Handler.Control.Menu;
				var textBinding = Handler.Widget.ComboTextBinding;
				menu.InsertItem(new NSMenuItem(textBinding.GetValue(item)), index);
				indexLookupByKey = null;
			}

			public override void RemoveItem(int index)
			{
				var menu = Handler.Control.Menu;
				menu.RemoveItemAt(index);
				indexLookupByKey = null;
			}

			public override void RemoveAllItems()
			{
				Handler.Control.RemoveAllItems();
				indexLookupByKey = null;
			}
		}

		public ComboBoxCellHandler()
		{
			Control = new EtoCell { Handler = this, ControlSize = NSControlSize.Regular, Bordered = false };
			Control.Title = string.Empty;
		}

		bool editable = true;

		public override bool Editable
		{
			get { return editable; }
			set
			{ 
				editable = value;
				Control.Enabled = value;
			}
		}

		public override void EnabledChanged(bool value)
		{
			base.EnabledChanged(value);
			if (value)
				Control.Enabled = editable;
		}

		public override void SetBackgroundColor(NSCell cell, Color color)
		{
			var c = (EtoCell)cell;
			c.BackgroundColor = color.ToNSUI();
			c.DrawsBackground = color != Colors.Transparent;
		}

		public override Color GetBackgroundColor(NSCell cell)
		{
			var c = (EtoCell)cell;
			return c.BackgroundColor.ToEto();
		}

		public override void SetForegroundColor(NSCell cell, Color color)
		{
			var c = (EtoCell)cell;
			c.TextColor = color.ToNSUI();
		}

		public override Color GetForegroundColor(NSCell cell)
		{
			var c = (EtoCell)cell;
			return c.TextColor.ToEto();
		}

		public IEnumerable<object> DataStore
		{
			get { return collection != null ? collection.Collection : null; }
			set
			{
				if (collection != null)
					collection.Unregister();
				collection = new CollectionHandler { Handler = this };
				collection.Register(value);
			}
		}

		public override void SetObjectValue(object dataItem, NSObject value)
		{
			if (Widget.Binding != null)
			{
				var row = ((NSNumber)value).Int32Value;
				var item = collection.ElementAt(row);
				var itemValue = item != null ? Widget.ComboKeyBinding.GetValue(item) : null;
				Widget.Binding.SetValue(dataItem, itemValue);
			}
		}

		public override NSObject GetObjectValue(object dataItem)
		{
			if (Widget.Binding != null)
			{
				var val = Widget.Binding.GetValue(dataItem);
				var key = Convert.ToString(val);
				var lookup = collection.IndexLookup[key].ToArray();
				var index = lookup.Length > 0 ? lookup[0] : -1;
				return new NSNumber(index);
			}
			return null;
		}

		public override nfloat GetPreferredSize(object value, CGSize cellSize, NSCell cell)
		{
			return 100;
		}
	}
}

