using System;
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

namespace Eto.Mac.Forms.Controls
{
	public class DropDownHandler : MacControl<NSPopUpButton, DropDown, DropDown.ICallback>, DropDown.IHandler
	{
		class MenuDelegate : NSMenuDelegate
		{
			WeakReference handler;

			public DropDownHandler Handler
			{
				get { return handler?.Target as DropDownHandler; }
				set { handler = new WeakReference(value); }
			}

			public override void MenuWillHighlightItem(NSMenu menu, NSMenuItem item)
			{
			}

			public override void MenuWillOpen(NSMenu menu)
			{
				Handler.Callback.OnDropDownOpening(Handler.Widget, EventArgs.Empty);
			}

			public override void MenuDidClose(NSMenu menu)
			{
				Handler.Callback.OnDropDownClosed(Handler.Widget, EventArgs.Empty);
			}
		}


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
					textAttributes = textColor != null ? NSDictionary.FromObjectAndKey(textColor.Value.ToNSUI(), NSStringAttributeKey.ForegroundColor) : null;
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

			public EtoPopUpButton()
			{
				Cell = new EtoPopUpButtonCell();
			}
		}

		protected override NSPopUpButton CreateControl()
		{
			return new EtoPopUpButton();
		}

		protected override void Initialize()
		{
			Control.Activated += HandleActivated;

			base.Initialize();
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
				var binding = Handler.Widget.ItemTextBinding;
				return (int)Handler.Control.Menu.IndexOf(binding.GetValue(item));
			}

			NSMenuItem CreateItem(object dataItem)
			{
				var item = new NSMenuItem(Handler.Widget.ItemTextBinding?.GetValue(dataItem) ?? string.Empty);
				item.Image = Handler.Widget.ItemImageBinding?.GetValue(dataItem).ToNS();
				return item;
			}

			public override void AddRange(IEnumerable<object> items)
			{
				var oldIndex = Handler.Control.IndexOfSelectedItem;
				foreach (var item in items)
				{
					Handler.Control.Menu.AddItem(CreateItem(item));
				}
				if (oldIndex == -1)
					Handler.Control.SelectItem(-1);
				Handler.LayoutIfNeeded();
			}

			public override void AddItem(object item)
			{
				var oldIndex = Handler.Control.IndexOfSelectedItem;
				Handler.Control.Menu.AddItem(CreateItem(item));
				if (oldIndex == -1)
					Handler.Control.SelectItem(-1);
				Handler.LayoutIfNeeded();
			}

			public override void InsertItem(int index, object item)
			{
				var oldIndex = Handler.Control.IndexOfSelectedItem;
				Handler.Control.Menu.InsertItem(CreateItem(item), index);
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

		public bool ShowBorder
		{
			get { return Control.Bordered; }
			set { Control.Bordered = value; }
		}

		void EnsureDelegate()
		{
			if (Control.Menu.Delegate == null)
				Control.Menu.Delegate = new MenuDelegate { Handler = this };
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case DropDown.DropDownOpeningEvent:
				case DropDown.DropDownClosedEvent:
					EnsureDelegate();
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}