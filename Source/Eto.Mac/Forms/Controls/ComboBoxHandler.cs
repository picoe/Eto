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
	public class ComboBoxHandler : MacControl<NSComboBox, ComboBox, ComboBox.ICallback>, ComboBox.IHandler
	{
		int lastSelected = -1;
		CollectionHandler collection;

		public class EtoComboBox : NSComboBox, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public object Handler
			{
				get { return WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); }
			}
		}

		public ComboBoxHandler()
		{
			Control = new EtoComboBox
			{ 
				Handler = this, 
				StringValue = string.Empty,
				Editable = true,
				VisibleItems = 20
			};
			AddObserver(NSComboBox.SelectionIsChangingNotification, SelectionDidChange);
			Control.Changed += HandleChanged;
		}

		static void HandleChanged(object sender, EventArgs e)
		{
			var handler = GetHandler(sender) as ComboBoxHandler;
			if (handler != null)
			{
				handler.Callback.OnTextChanged(handler.Widget, EventArgs.Empty);
				Application.Instance.AsyncInvoke(() =>
				{
					if (handler.SelectedIndex != handler.lastSelected)
					{
						handler.Callback.OnSelectedIndexChanged(handler.Widget, EventArgs.Empty);
						handler.lastSelected = handler.SelectedIndex;
					}
				});
			}
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case ComboBox.TextChangedEvent:
					// handled automatically
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			var size = base.GetNaturalSize(availableSize);
			return new SizeF(Math.Max(size.Width, 100), size.Height);
		}

		static void SelectionDidChange(ObserverActionEventArgs e)
		{
			var handler = e.Handler as ComboBoxHandler;
			if (handler != null)
			{
				Application.Instance.AsyncInvoke(() =>
				{
					handler.Callback.OnSelectedIndexChanged(handler.Widget, EventArgs.Empty);
					Application.Instance.AsyncInvoke(() => handler.Callback.OnTextChanged(handler.Widget, EventArgs.Empty));
				});
			}
		}

		class CollectionHandler : EnumerableChangedHandler<object>
		{
			public ComboBoxHandler Handler { get; set; }

			public override int IndexOf(object item)
			{
				var binding = Handler.Widget.TextBinding;
				return (int)Handler.Control.IndexOf(NSObject.FromObject(binding.GetValue(item)));
			}

			public override void AddRange(IEnumerable<object> items)
			{
				var oldIndex = Handler.Control.SelectedIndex;
				var binding = Handler.Widget.TextBinding;
				foreach (var item in items.Select(r => binding.GetValue(r)))
				{
					Handler.Control.Add(NSObject.FromObject(item));
				}
				Handler.LayoutIfNeeded();
			}

			public override void AddItem(object item)
			{
				var binding = Handler.Widget.TextBinding;
				Handler.Control.Add(NSObject.FromObject(binding.GetValue(item)));
				Handler.LayoutIfNeeded();
			}

			public override void InsertItem(int index, object item)
			{
				var binding = Handler.Widget.TextBinding;
				Handler.Control.Insert(NSObject.FromObject(binding.GetValue(item)), index);
				Handler.LayoutIfNeeded();
			}

			public override void RemoveItem(int index)
			{
				var selected = Handler.SelectedIndex;
				Handler.Control.RemoveAt(index);
				if (selected == index)
				{
					Handler.Control.StringValue = string.Empty;
					Handler.SelectedIndex = -1;
					// when removing the last item, we don't get a change event here
					if (index == Count)
						Handler.Callback.OnSelectedIndexChanged(Handler.Widget, EventArgs.Empty);
				}
				Handler.LayoutIfNeeded();
			}

			public override void RemoveAllItems()
			{
				var change = Handler.SelectedIndex != -1;
				Handler.Control.RemoveAll();
				if (change)
				{
					Handler.Control.StringValue = string.Empty;
					Handler.SelectedIndex = -1;
				}
				Handler.LayoutIfNeeded();
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
			get { return (int)Control.SelectedIndex; }
			set
			{
				var selectedIndex = SelectedIndex;
				var lastText = Text;
				if (value == -1)
				{
					if (selectedIndex != -1)
					{
						Control.DeselectItem(Control.SelectedIndex);
						Control.StringValue = string.Empty;
					}
				}
				else
					Control.SelectItem(value);
				if (value != selectedIndex)
				{
					Callback.OnSelectedIndexChanged(Widget, EventArgs.Empty);
				}
				if (lastText != Text)
				{
					Callback.OnTextChanged(Widget, EventArgs.Empty);
				}
			}
		}

		public override Color BackgroundColor
		{
			get { return ((NSComboBoxCell)Control.Cell).BackgroundColor.ToEto(); }
			set
			{
				if (value != BackgroundColor)
				{
					((NSComboBoxCell)Control.Cell).BackgroundColor = value.ToNSUI();
					Control.SetNeedsDisplay();
				}
			}
		}

		public Color TextColor
		{
			get { return ((NSComboBoxCell)Control.Cell).TextColor.ToEto(); }
			set
			{
				if (value != TextColor)
				{
					((NSComboBoxCell)Control.Cell).TextColor = value.ToNSUI();
					Control.SetNeedsDisplay();
				}
			}
		}

		public string Text
		{
			get { return Control.StringValue; }
			set { Control.StringValue = value ?? string.Empty; }
		}

		public bool ReadOnly
		{
			get { return !Control.Editable; }
			set
			{ 
				Control.Editable = !value;
				if (Control.Window != null)
					Control.Window.MakeFirstResponder(null); // allows editing if currently focussed, so remove focus
			}
		}

		public bool AutoComplete
		{
			get { return Control.Completes; }
			set { Control.Completes = value; }
		}
	}
}
