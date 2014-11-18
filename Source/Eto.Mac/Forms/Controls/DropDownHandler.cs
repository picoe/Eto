using System;
using SD = System.Drawing;
using Eto.Forms;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using Eto.Drawing;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreImage;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
using MonoMac.CoreImage;
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

namespace Eto.Mac.Forms.Controls
{
	public class DropDownHandler : MacControl<NSPopUpButton, DropDown, DropDown.ICallback>, DropDown.IHandler
	{
		CollectionHandler collection;

		public class EtoPopUpButtonCell : NSPopUpButtonCell
		{
			NSDictionary textAttributes;
			Color? textColor;

			public Color? Color { get; set; }

			public Color? TextColor
			{
				get { return textColor; }
				set
				{
					textColor = value;
					textAttributes = textColor != null ? NSDictionary.FromObjectAndKey(textColor.Value.ToNSUI(), NSAttributedString.ForegroundColorAttributeName) : null;
				}
			}

			public override void DrawBezelWithFrame(CGRect frame, NSView controlView)
			{
				if (Color != null)
				{
					MacEventView.Colourize(controlView, Color.Value, delegate
					{
						base.DrawBezelWithFrame(frame, controlView);
					});
				}
				else
					base.DrawBezelWithFrame(frame, controlView);
			}

			public override CGRect DrawTitle(NSAttributedString title, CGRect frame, NSView controlView)
			{
				if (textAttributes != null)
				{
					var str = new NSMutableAttributedString(title);
					str.AddAttributes(textAttributes, new NSRange(0, title.Length));
					title = str;
				}
				return base.DrawTitle(title, frame, controlView);
			}
		}

		public class EtoPopUpButton : NSPopUpButton, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public object Handler
			{
				get { return WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); }
			}
		}

		public DropDownHandler()
		{
			Control = new EtoPopUpButton { Handler = this, Cell = new EtoPopUpButtonCell() };
			Control.Activated += HandleActivated;
		}

		static void HandleActivated(object sender, EventArgs e)
		{
			var handler = GetHandler(sender) as DropDownHandler;
			handler.Callback.OnSelectedIndexChanged(handler.Widget, EventArgs.Empty);
		}

		class CollectionHandler : EnumerableChangedHandler<object>
		{
			public DropDownHandler Handler { get; set; }

			public override int IndexOf(object item)
			{
				var binding = Handler.Widget.TextBinding;
				return (int)Handler.Control.Menu.IndexOf(binding.GetValue(item));
			}

			public override void AddRange(IEnumerable<object> items)
			{
				var oldIndex = Handler.Control.IndexOfSelectedItem;
				var binding = Handler.Widget.TextBinding;
				foreach (var item in items.Select(r => binding.GetValue(r)))
				{
					Handler.Control.Menu.AddItem(new NSMenuItem(item));
				}
				if (oldIndex == -1)
					Handler.Control.SelectItem(-1);
				Handler.LayoutIfNeeded();
			}

			public override void AddItem(object item)
			{
				var oldIndex = Handler.Control.IndexOfSelectedItem;
				var binding = Handler.Widget.TextBinding;
				Handler.Control.Menu.AddItem(new NSMenuItem(Convert.ToString(binding.GetValue(item))));
				if (oldIndex == -1)
					Handler.Control.SelectItem(-1);
				Handler.LayoutIfNeeded();
			}

			public override void InsertItem(int index, object item)
			{
				var oldIndex = Handler.Control.IndexOfSelectedItem;
				var binding = Handler.Widget.TextBinding;
				Handler.Control.Menu.InsertItem(new NSMenuItem(Convert.ToString(binding.GetValue(item))), index);
				if (oldIndex == -1)
					Handler.Control.SelectItem(-1);
				Handler.LayoutIfNeeded();
			}

			public override void RemoveItem(int index)
			{
				var selected = Handler.SelectedIndex;
				Handler.Control.RemoveItem(index);
				Handler.LayoutIfNeeded();
				if (Handler.Widget.Loaded && selected == index)
				{
					Handler.Control.SelectItem(-1);
					Handler.Callback.OnSelectedIndexChanged(Handler.Widget, EventArgs.Empty);
				}
			}

			public override void RemoveAllItems()
			{
				var change = Handler.SelectedIndex != -1;
				Handler.Control.RemoveAllItems();
				Handler.LayoutIfNeeded();
				if (Handler.Widget.Loaded && change)
				{
					Handler.Control.SelectItem(-1);
					Handler.Callback.OnSelectedIndexChanged(Handler.Widget, EventArgs.Empty);
				}
			}
		}

		public IEnumerable<object> DataStore
		{
			get { return collection == null ? null : collection.Collection; }
			set
			{
				if (collection != null)
					collection.Unregister();
				collection = new CollectionHandler { Handler = this };
				collection.Register(value);
			}
		}

		public int SelectedIndex
		{
			get { return (int)Control.IndexOfSelectedItem; }
			set
			{
				if (value != SelectedIndex)
				{
					Control.SelectItem(value);
					if (Widget.Loaded)
						Callback.OnSelectedIndexChanged(Widget, EventArgs.Empty);
				}
			}
		}

		public override Color BackgroundColor
		{
			get { return ((EtoPopUpButtonCell)Control.Cell).Color ?? Colors.Transparent; }
			set
			{
				if (value != BackgroundColor)
				{
					((EtoPopUpButtonCell)Control.Cell).Color = value;
					Control.SetNeedsDisplay();
				}
			}
		}

		public Color TextColor
		{
			get { return ((EtoPopUpButtonCell)Control.Cell).TextColor ?? NSColor.ControlText.ToEto(); }
			set
			{
				if (value != TextColor)
				{
					((EtoPopUpButtonCell)Control.Cell).TextColor = value;
					Control.SetNeedsDisplay();
				}
			}
		}
	}
}