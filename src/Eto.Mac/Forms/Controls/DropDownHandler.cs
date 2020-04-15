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
			ColorizeView colorize;
			public Color? Color
			{
				get => colorize?.Color;
				set => ColorizeView.Create(ref colorize, value);
			}

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
				colorize?.Begin(frame, controlView);
				base.DrawBezelWithFrame(frame, controlView);
				colorize?.End();
			}

			public override CGRect DrawTitle(NSAttributedString title, CGRect frame, NSView controlView)
			{
				var str = new NSMutableAttributedString(title);
				var range = new NSRange(0, str.Length);
				if (textAttributes != null)
				{
					str.AddAttributes(textAttributes, range);
				}

				// enforce the control font if it had been overridden for this item, macOS doesn't support non-standard fonts for its NSPopUpButton.
				if (controlView is NSControl control)
					str.AddAttribute(NSStringAttributeKey.Font, control.Font, range);

				title = str;
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

			public EtoPopUpButton(IntPtr handle)
				: base(handle)
			{
				Cell = new EtoPopUpButtonCell();
			}

			public EtoPopUpButton()
			{
				Cell = new EtoPopUpButtonCell();
			}
		}

		protected override bool DefaultUseAlignmentFrame => true;

		protected override NSPopUpButton CreateControl() => new EtoPopUpButton();

		protected override void Initialize()
		{
			Control.Activated += HandleActivated;

			base.Initialize();
		}

		static void HandleActivated(object sender, EventArgs e)
		{
			var handler = GetHandler(sender) as DropDownHandler;
			handler?.Callback.OnSelectedIndexChanged(handler.Widget, EventArgs.Empty);

			

		}

		class CollectionHandler : EnumerableChangedHandler<object>
		{
			public DropDownHandler Handler { get; set; }

			NSMenuItem CreateItem(object dataItem, int row)
			{
				var h = Handler;

				var item = new NSMenuItem();
				var title = h.Widget.ItemTextBinding?.GetValue(dataItem) ?? string.Empty;
				item.Image = h.Widget.ItemImageBinding?.GetValue(dataItem).ToNS();

				if (h.IsEventHandled(DropDown.FormatItemEvent))
				{
					var args = new DropDownFormatEventArgs(dataItem, row, h.Font);
					h.Callback.OnFormatItem(h.Widget, args);
					if (args.IsFontSet && args.Font != null)
					{
						var attr = new NSMutableAttributedString(title);
						var font = args.Font.ToNS();
						var range = new NSRange(0, attr.Length);
						attr.AddAttribute(NSStringAttributeKey.Font, font, range);
						item.AttributedTitle = attr;
					}
					else
					{
						item.Title = title;
					}
				}
				else
				{
					item.Title = title;
				}

				return item;
			}

			static IntPtr selAddItem_Handle = Selector.GetHandle("addItem:");

			public override void AddRange(IEnumerable<object> items)
			{
				var control = Handler.Control;
				var oldIndex = control.IndexOfSelectedItem;
				NSMenu menu = control.Menu;

				// xamarin.mac 2 goes really slow when adding directly.  See https://github.com/xamarin/xamarin-macios/issues/3488
				// also, this does improve performance normally, so let's keep the hack
				// until Xamarin.Mac supports an NSMenu.AddRange() of some sort
				var itemList = items.ToList();
				for (int i = 0; i < itemList.Count; i++)
				{
					var menuItem = CreateItem(itemList[i], i);
					if (i < itemList.Count - 1)
						Messaging.void_objc_msgSend_IntPtr(menu.Handle, selAddItem_Handle, menuItem.Handle);
					else
						menu.AddItem(menuItem); // use this for the last item so the item array gets referenced internally by XM.
				}

				if (oldIndex == -1)
					control.SelectItem(-1);
				Handler.InvalidateMeasure();
			}

			public override void AddItem(object item)
			{
				var oldIndex = Handler.Control.IndexOfSelectedItem;
				Handler.Control.Menu.AddItem(CreateItem(item, Count));
				if (oldIndex == -1)
					Handler.Control.SelectItem(-1);
				Handler.InvalidateMeasure();
			}

			public override void InsertItem(int index, object item)
			{
				var oldIndex = Handler.Control.IndexOfSelectedItem;
				Handler.Control.Menu.InsertItem(CreateItem(item, index), index);
				if (oldIndex == -1)
					Handler.Control.SelectItem(-1);
				Handler.InvalidateMeasure();
			}

			public override void RemoveItem(int index)
			{
				var selected = Handler.SelectedIndex;
				Handler.Control.RemoveItem(index);
				Handler.InvalidateMeasure();
				if (selected == index)
				{
					Handler.Control.SelectItem(-1);
					Handler.Callback.OnSelectedIndexChanged(Handler.Widget, EventArgs.Empty);
				}
			}

			public override void RemoveAllItems()
			{
				var change = Handler.SelectedIndex != -1;
				Handler.Control.RemoveAllItems();
				Handler.InvalidateMeasure();
				if (change)
				{
					Handler.Control.SelectItem(-1);
					Handler.Callback.OnSelectedIndexChanged(Handler.Widget, EventArgs.Empty);
				}
			}

			public void Recreate()
			{
				Handler.Control.RemoveAllItems();
				InitializeCollection();
				Handler.InvalidateMeasure();
			}
		}

		public IEnumerable<object> DataStore
		{
			get => collection?.Collection;
			set
			{
				var selected = Widget.SelectedValue;
				Control.SelectItem(-1);
				collection?.Unregister();
				collection = new CollectionHandler { Handler = this };
				collection.Register(value);
				if (!ReferenceEquals(selected, null))
				{
					Control.SelectItem(collection.IndexOf(selected));
					Callback.OnSelectedIndexChanged(Widget, EventArgs.Empty);
				}
				InvalidateMeasure();
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
			set
			{
				Control.Bordered = value;
				InvalidateMeasure();
			}
		}

		IIndirectBinding<string> itemTextBinding;
		public IIndirectBinding<string> ItemTextBinding
		{
			get => itemTextBinding;
			set
			{
				itemTextBinding = value;
				var dataStore = DataStore;
				if (dataStore != null)
				{
					// re-add all items
					DataStore = dataStore;
				}
			}
		}

		public IIndirectBinding<string> ItemKeyBinding { get; set; }

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
				case DropDown.FormatItemEvent:
					collection?.Recreate();
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}