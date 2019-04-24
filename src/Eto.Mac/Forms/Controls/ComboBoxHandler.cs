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
	public class ComboBoxHandler : MacControl<NSComboBox, ComboBox, ComboBox.ICallback>, ComboBox.IHandler
	{
		int lastSelected = -1;
		static readonly object SuppressChangeEvents_Key = new object();
		int SuppressChangeEvents
		{
			get { return Widget.Properties.Get<int>(SuppressChangeEvents_Key); }
			set { Widget.Properties.Set(SuppressChangeEvents_Key, value); }
		}
		CollectionHandler collection;

		public class EtoComboBox : NSComboBox, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public object Handler
			{
				get { return WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); }
			}

			[Export("cellClass")]
			public new static Class CellClass()
			{
				return new Class(typeof(EtoCell));
			}

			public EtoComboBox()
			{
				StringValue = string.Empty;
				Editable = true;
				VisibleItems = 20;
			}
		}

		public class EtoCell : NSComboBoxCell
		{
			[Export("tableView:willDisplayCell:forTableColumn:row:")]
			public void TableWillDisplayCellForTableColumn(NSTableView tableView, NSCell cell, NSTableColumn tableColumn, nint rowIndex)
			{
				// check if anything is null first
				if (ControlView == null || ControlView.Window == null || ControlView.Window.Screen == null || tableView.Window == null)
					return;
				// hack (using public api's) to set size of popup to at least show this item's entire text.
				var size = cell.CellSizeForBounds(ControlView.Window.Screen.VisibleFrame);
				var window = tableView.Window;
				var frame = window.Frame;
				size.Width += frame.Width - tableView.Frame.Width;
				if (frame.Width < size.Width)
				{
					frame.Width = size.Width;
					window.SetFrame(frame, true);
				}
			}
		}

		protected override bool DefaultUseAlignmentFrame => true;

		protected override NSComboBox CreateControl() => new EtoComboBox();

		protected override void Initialize()
		{
			AddObserver(NSComboBox.SelectionDidChangeNotification, SelectionDidChange);
			Control.Changed += HandleChanged;
			base.Initialize();
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
				case DropDown.DropDownOpeningEvent:
					AddObserver(NSComboBox.WillPopUpNotification, HandlePopUp);
					break;
				case DropDown.DropDownClosedEvent:
					AddObserver(NSComboBox.WillDismissNotification, HandleDismiss);
					break;
				case ComboBox.TextChangedEvent:
					// handled automatically
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		static void HandlePopUp(ObserverActionEventArgs e)
		{
			var handler = (ComboBoxHandler)e.Handler;
			handler.Callback.OnDropDownOpening(handler.Widget, EventArgs.Empty);
		}

		static void HandleDismiss(ObserverActionEventArgs e)
		{
			var handler = (ComboBoxHandler)e.Handler;
			handler.Callback.OnDropDownClosed(handler.Widget, EventArgs.Empty);
		}

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			var size = base.GetNaturalSize(availableSize);
			// note: natural height reported by NSComboBox.FittingSize is 25, but should be 26.
			return new SizeF(Math.Max(size.Width, 100), size.Height + 1);
		}

		static void SelectionDidChange(ObserverActionEventArgs e)
		{
			var handler = e.Handler as ComboBoxHandler;
			if (handler != null)
			{
				if (handler.SuppressChangeEvents > 0)
					return;
				Application.Instance.AsyncInvoke(() =>
				{
					handler.Callback.OnSelectedIndexChanged(handler.Widget, EventArgs.Empty);
					handler.Callback.OnTextChanged(handler.Widget, EventArgs.Empty);
				});
			}
		}

		class CollectionHandler : EnumerableChangedHandler<object>
		{
			public ComboBoxHandler Handler { get; set; }

			public override void AddRange(IEnumerable<object> items)
			{
				var oldIndex = Handler.Control.SelectedIndex;
				var binding = Handler.Widget.ItemTextBinding;
				foreach (var item in items.Select(r => binding.GetValue(r)))
				{
					Handler.Control.Add(NSObject.FromObject(item));
				}
				Handler.InvalidateMeasure();
			}

			public override void AddItem(object item)
			{
				var binding = Handler.Widget.ItemTextBinding;
				Handler.Control.Add(NSObject.FromObject(binding.GetValue(item)));
				Handler.InvalidateMeasure();
			}

			public override void InsertItem(int index, object item)
			{
				var binding = Handler.Widget.ItemTextBinding;
				Handler.Control.Insert(NSObject.FromObject(binding.GetValue(item)), index);
				Handler.InvalidateMeasure();
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
				Handler.InvalidateMeasure();
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
				Handler.InvalidateMeasure();
			}
		}

		public IEnumerable<object> DataStore
		{
			get => collection?.Collection;
			set
			{
				collection?.Unregister();
				collection = new CollectionHandler { Handler = this };
				collection.Register(value);
			}
		}

		public int SelectedIndex
		{
			get { return (int)Control.SelectedIndex; }
			set
			{
				lastSelected = SelectedIndex;
				SuppressChangeEvents++;
				var lastText = Text;
				if (value == -1)
				{
					if (lastSelected != -1)
					{
						Control.DeselectItem(Control.SelectedIndex);
						Control.StringValue = string.Empty;
					}
				}
				else
					Control.SelectItem(value);
				if (value != lastSelected)
				{
					Callback.OnSelectedIndexChanged(Widget, EventArgs.Empty);
				}
				if (lastText != Text)
				{
					Callback.OnTextChanged(Widget, EventArgs.Empty);
				}
				SuppressChangeEvents--;
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
			set
			{ 
				if (Text != value)
				{
					Control.StringValue = value ?? string.Empty;
					if (collection != null)
					{
						var binding = Widget.ItemTextBinding;
						var item = collection.Collection.FirstOrDefault(r => binding.GetValue(r) == value);
						var index = item != null ? collection.IndexOf(item) : -1;

						SelectedIndex = index;
					}

				}
			}
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

		public bool ShowBorder
		{
			get { return Control.Bordered; }
			set { Control.Bordered = value; }
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
					int selectedIndex = SelectedIndex;
					// re-add all items
					DataStore = dataStore;
					SelectedIndex = selectedIndex;
				}
			}
		}

		public IIndirectBinding<string> ItemKeyBinding { get; set; }
	}
}
