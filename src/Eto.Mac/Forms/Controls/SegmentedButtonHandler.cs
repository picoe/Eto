using System;
using Eto.Forms;
using System.Collections.Generic;
using Eto.Drawing;
using System.Linq;

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
	interface ISegmentedItemHandler
	{
		void TriggerClick();
		void TriggerSelectionChanged();
	}

	static class SegmentedItemHandler
	{
		internal static readonly object Text_Key = new object();
		internal static readonly object Image_Key = new object();
		internal static readonly object Width_Key = new object();
		internal static readonly object Selected_Key = new object();
		internal static readonly object Enabled_Key = new object();
		internal static readonly object Visible_Key = new object();
		internal static readonly object ToolTip_Key = new object();
	}

	public abstract class SegmentedItemHandler<TControl, TWidget, TCallback> : WidgetHandler<TControl, TWidget, TCallback>, SegmentedItem.IHandler, ISegmentedItemHandler
		where TWidget: SegmentedItem
		where TCallback: SegmentedItem.ICallback
	{
		SegmentedButtonHandler ParentHandler => Widget.Parent?.Handler as SegmentedButtonHandler;
		NSSegmentedControl SegmentedControl => ParentHandler?.Control;
		int CurrentSegment => Widget.Parent?.Items.IndexOf(Widget) ?? -1;

		public string Text
		{
			get => Widget.Properties.Get<string>(SegmentedItemHandler.Text_Key);
			set
			{
				if (Widget.Properties.TrySet(SegmentedItemHandler.Text_Key, value))
				{
					SegmentedControl?.SetLabel(value ?? string.Empty, CurrentSegment);
				}
			}
		}
		public Image Image
		{
			get => Widget.Properties.Get<Image>(SegmentedItemHandler.Image_Key);
			set
			{
				if (Widget.Properties.TrySet(SegmentedItemHandler.Image_Key, value))
				{
					SegmentedControl?.SetImage(value.ToNS(), CurrentSegment);
				}
			}
		}

		public int Width
		{
			get => Widget.Properties.Get<int>(SegmentedItemHandler.Width_Key) ;
			set
			{
				if (Widget.Properties.TrySet(SegmentedItemHandler.Width_Key, value))
				{
					SegmentedControl?.SetWidth(value, CurrentSegment);
				}
			}
		}

		public bool Selected
		{
			get
			{
				if (ParentHandler?.SelectionMode == SegmentedSelectionMode.None)
					return false;
				return SegmentedControl?.IsSelectedForSegment(CurrentSegment) ?? Widget.Properties.Get<bool>(SegmentedItemHandler.Selected_Key);
			}
			set
			{
				if (value != Selected)
				{
					Widget.Properties.Set(SegmentedItemHandler.Selected_Key, value);
					if (!value && ParentHandler?.SelectionMode == SegmentedSelectionMode.Single)
						SegmentedControl.SelectedSegment = -1;
					else
						SegmentedControl?.SetSelected(value, CurrentSegment);

					if (ParentHandler?.SelectionMode != SegmentedSelectionMode.None)
						Callback.OnSelectedChanged(Widget, EventArgs.Empty);
					ParentHandler?.TriggerSelectionChanged(false);
				}
			}
		}
		public bool Enabled
		{
			get => Widget.Properties.Get<bool>(SegmentedItemHandler.Enabled_Key, true);
			set
			{
				if (Widget.Properties.TrySet(SegmentedItemHandler.Enabled_Key, value, true))
				{
					SegmentedControl?.SetEnabled(value, CurrentSegment);
				}
			}
		}
		public bool Visible { get; set; } = true;
		public string ToolTip
		{
			get => Widget.Properties.Get<string>(SegmentedItemHandler.ToolTip_Key);
			set
			{
				if (Widget.Properties.TrySet(SegmentedItemHandler.ToolTip_Key, value))
				{
					SegmentedControl?.SetToolTip(value, CurrentSegment);
				}
			}
		}

		public void TriggerClick() => Callback.OnClick(Widget, EventArgs.Empty);

		public void TriggerSelectionChanged() => Callback.OnSelectedChanged(Widget, EventArgs.Empty);

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case SegmentedItem.ClickEvent:
					// handled intrinsically
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}

	public class ButtonSegmentedItemHandler : SegmentedItemHandler<object, ButtonSegmentedItem, ButtonSegmentedItem.ICallback>, ButtonSegmentedItem.IHandler
	{

	}

	public class MenuSegmentedItemHandler : SegmentedItemHandler<object, MenuSegmentedItem, MenuSegmentedItem.ICallback>, MenuSegmentedItem.IHandler
	{
		public ContextMenu Menu { get; set; }
		public bool CanSelect { get; set; }
	}


	public class SegmentedButtonHandler : MacView<NSSegmentedControl, SegmentedButton, SegmentedButton.ICallback>, SegmentedButton.IHandler
	{
		int? lastSelected;

		public class EtoSegmentedCell : NSSegmentedCell
		{
			ColorizeView colorize;
			public SegmentedButtonHandler Handler => (ControlView as EtoSegmentedControl)?.Handler;

			public Color? BackgroundColor
			{
				get => colorize?.Color;
				set => ColorizeView.Create(ref colorize, value);
			}

			static IntPtr selMenuDelayTimeForSegment = Selector.GetHandle("_menuDelayTimeForSegment:");

			// private API, but only way to do this..
			[Export("_menuDelayTimeForSegment:")]
			nfloat MenuDelayTimeForSegment(nint segment)
			{
				if (ShowMenu(segment))
					return 0;

				return Messaging.nfloat_objc_msgSendSuper_nint(SuperHandle, selMenuDelayTimeForSegment, segment);
			}

			bool ShowMenu(nint segment)
			{
				if (segment < 0 || GetMenu(segment) == null)
					return false;
				var item = Handler?.Widget.Items[(int)segment] as MenuSegmentedItem;
				return item?.CanSelect == false;
			}

			public override void DrawWithFrame(CGRect cellFrame, NSView inView)
			{
				colorize?.Begin(cellFrame, inView);
				base.DrawWithFrame(cellFrame, inView);
				colorize?.End();
			}

			public override void DrawInteriorWithFrame(CGRect cellFrame, NSView inView)
			{
				if (colorize != null)
				{
					var context = NSGraphicsContext.CurrentContext.GraphicsPort;
					var frame = inView.Frame;
					context.TranslateCTM(0, frame.Height + cellFrame.Y - (frame.Height - cellFrame.Y - cellFrame.Height) + 1);
					context.ScaleCTM(1, -1);
				}
				base.DrawInteriorWithFrame(cellFrame, inView);
			}
		}

		public class EtoSegmentedControl : NSSegmentedControl, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public SegmentedButtonHandler Handler => WeakHandler?.Target as SegmentedButtonHandler;

			public EtoSegmentedControl()
			{
				Cell = new EtoSegmentedCell();
				TrackingMode = NSSegmentSwitchTracking.Momentary;
				Target = this;
				Action = selClicked;
			}

			static readonly Selector selClicked = new Selector("clicked:");

			[Export("clicked:")]
			public void Clicked(NSObject sender) => Handler?.TriggerSegmentClicked();

		}

		private void TriggerSegmentClicked()
		{
			// using SelectedSegment as this is the segment that was clicked last.
			var selected = (int)Control.SelectedSegment;
			if (selected >= 0)
			{
				var item = Widget.Items[selected];
				Callback.OnItemClicked(Widget, new SegmentedItemClickEventArgs(item, selected));
				var itemHandler = item.Handler as ISegmentedItemHandler;
				itemHandler?.TriggerClick();
				var mode = SelectionMode;
				if (mode == SegmentedSelectionMode.Multiple || 
					(mode == SegmentedSelectionMode.Single && selected != lastSelected))
				{
					itemHandler?.TriggerSelectionChanged();
				}
			}

			TriggerSelectionChanged(false);
		}

		protected override bool DefaultUseAlignmentFrame => true;

		protected override NSSegmentedControl CreateControl() => new EtoSegmentedControl();

		static readonly object SelectionMode_Key = new object();

		public SegmentedSelectionMode SelectionMode
		{
			get => Widget.Properties.Get<SegmentedSelectionMode>(SelectionMode_Key);
			set
			{
				if (Widget.Properties.TrySet(SelectionMode_Key, value))
				{
					bool selectionChanged = false;
					switch (value)
					{
						case SegmentedSelectionMode.None:
							selectionChanged = HasSelection;
							Control.UnselectAllSegments();
							Control.TrackingMode = NSSegmentSwitchTracking.Momentary;
							break;
						case SegmentedSelectionMode.Single:
							var count = Control.SegmentCount;
							bool wasSelected = false;
							for (nint i = 0; i < count; i++)
							{
								if (wasSelected)
								{
									selectionChanged |= Control.IsSelectedForSegment(i);
									Control.SetSelected(false, i);
								}
								else if (Control.IsSelectedForSegment(i))
									wasSelected = true;
							}
							Control.TrackingMode = NSSegmentSwitchTracking.SelectOne;
							break;
						case SegmentedSelectionMode.Multiple:
							Control.TrackingMode = NSSegmentSwitchTracking.SelectAny;
							break;
						default:
							throw new NotSupportedException();
					}
					if (selectionChanged)
						TriggerSelectionChanged(true);
				}
			}
		}

		public override NSView ContainerControl => Control;

		public int SelectedIndex
		{
			get
			{
				if (SelectionMode == SegmentedSelectionMode.None)
					return -1;
				return GetSelectedIndex();
			}
			set
			{
				if (SelectionMode == SegmentedSelectionMode.None)
					return;
				if (value != SelectedIndex)
				{
					Control.SelectedSegment = value;
					TriggerSelectionChanged(true);
				}
			}
		}

		private int GetSelectedIndex()
		{
			// NSSegmentedControl.SelectedSegment returns the item that was last changed, but we want the currently selected segment.
			for (int i = 0; i < Control.SegmentCount; i++)
			{
				if (Control.IsSelectedForSegment(i))
					return i;
			}
			return -1;
		}

		public IEnumerable<int> SelectedIndexes
		{
			get
			{
				if (SelectionMode == SegmentedSelectionMode.None)
					yield break;

				for (int i = 0; i < Widget.Items.Count; i++)
				{
					if (Control.IsSelectedForSegment(i))
						yield return i;
				}
			}
			set
			{
				Control.UnselectAllSegments();
				if (SelectionMode == SegmentedSelectionMode.None)
					return;
				if (value != null)
				{
					foreach (var index in value)
					{
						Control.SetSelected(true, index);
					}
				}
				TriggerSelectionChanged(true);
			}
		}

		nint IndexOf(SegmentedItem item)
		{
			if (item == null)
				return -1;
			return Widget.Items.IndexOf(item);
		}

		public void ClearItems()
		{
			var wasSelected = Widget.Items.Any(r => r.Selected);
			Control.SegmentCount = 0;
			if (wasSelected)
				TriggerSelectionChanged(true);
		}

		public void InsertItem(int index, SegmentedItem item)
		{
			var count = ++Control.SegmentCount;

			// need to copy items as we can't insert items
			if (index < count - 1)
			{
				for (nint i = index; i < count; i++)
				{
					var next = i - 1;
					CopyItem(i, next);
				}
			}
			SetItem(index, item);
			if (item.Selected)
				TriggerSelectionChanged(true);
		}

		static readonly IntPtr selSetToolTipForSegment = Selector.GetHandle("setToolTip:forSegment:");
		static readonly IntPtr selSetShowsMenuIndicator = Selector.GetHandle("setShowsMenuIndicator:forSegment:");

		// 10.13+
		static readonly bool supportsTooltip = ObjCExtensions.InstancesRespondToSelector<NSSegmentedCell>(selSetToolTipForSegment);
		static readonly bool supportsMenuIndicator = ObjCExtensions.InstancesRespondToSelector<NSSegmentedControl>(selSetShowsMenuIndicator);

		public void SetItem(int index, SegmentedItem item)
		{
			Control.SetLabel(item.Text ?? string.Empty, index);
			Control.SetImage(item.Image.ToNS(), index);
			Control.SetEnabled(item.Enabled, index);
			Control.SetSelected(item.Selected, index);
			Control.SetWidth(item.Width < 0 ? 0 : item.Width, index);
			var menu = (item as MenuSegmentedItem)?.Menu.ToNS();
			Control.SetMenu(menu, index);
			if (supportsMenuIndicator)
			{
				Control.SetShowsMenuIndicator(menu != null, index);
			}
			if (supportsTooltip)
			{
				Control.SetToolTip(item.ToolTip ?? string.Empty, index);
			}
		}

		public void RemoveItem(int index, SegmentedItem item)
		{
			var wasSelected = item.Selected;
			// no way to remove an item, need to copy all item data then set count.
			for (nint i = Control.SegmentCount - 1; i > index; i--)
			{
				var prev = i - 1;
				CopyItem(i, prev);
			}
			if (wasSelected)
			{
				// when removing the selected item, it selects the previous item by default. ugh.
				if (SelectionMode == SegmentedSelectionMode.Single)
					Control.SelectedSegment = -1;
				TriggerSelectionChanged(true);
			}
		}

		private void CopyItem(nint i, nint prev)
		{
			Control.SetLabel(Control.GetLabel(i), prev);
			Control.SetImage(Control.GetImage(i), prev);
			Control.SetEnabled(Control.IsEnabled(i), prev);
			Control.SetSelected(Control.IsSelectedForSegment(i), prev);
			Control.SetMenu(Control.GetMenu(i), prev);
			Control.SetWidth(Control.GetWidth(i), prev);
			if (supportsMenuIndicator)
			{
				Control.SetShowsMenuIndicator(Control.ShowsMenuIndicator(i), prev);
			}
			if (supportsTooltip)
			{
				Control.SetToolTip(Control.GetToolTip(i), prev);
			}
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case SegmentedButton.ItemClickEvent:
				case SegmentedButton.SelectedIndexesChangedEvent:
					// handled automatically
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		bool HasSelection => GetSelectedIndex() != -1;

		public void SelectAll()
		{
			if (SelectionMode == SegmentedSelectionMode.None)
				return;
			var didSelect = false;
			var count = Control.SegmentCount;
			for (int i = 0; i < count; i++)
			{
				var isSelected = Control.IsSelectedForSegment(i);
				if (!isSelected)
				{
					Control.SetSelected(true, i);
					didSelect = true;
					var item = Widget.Items[i];
					var itemHandler = item.Handler as ISegmentedItemHandler;
					itemHandler?.TriggerSelectionChanged();
				}
			}
			if (didSelect)
				TriggerSelectionChanged(true);
		}

		public void ClearSelection()
		{
			var wasSelected = false;
			var count = Control.SegmentCount;
			for (int i = 0; i < count; i++)
			{
				var isSelected = Control.IsSelectedForSegment(i);
				if (isSelected)
				{
					Control.SetSelected(false, i);
					wasSelected = true;
					var item = Widget.Items[i];
					var itemHandler = item.Handler as ISegmentedItemHandler;
					itemHandler?.TriggerSelectionChanged();
				}
			}

			if (wasSelected)
				TriggerSelectionChanged(true);
		}

		internal void TriggerSelectionChanged(bool force)
		{
			var mode = SelectionMode;
			if (!force && mode == SegmentedSelectionMode.None)
				return;

			var selectedIndex = SelectedIndex;
			if (force || mode == SegmentedSelectionMode.Multiple || lastSelected != selectedIndex)
			{
				Callback.OnSelectedIndexesChanged(Widget, EventArgs.Empty);
				lastSelected = selectedIndex;
			}
		}

		protected override void SetBackgroundColor(Color? color)
		{
			if (Control.Cell is EtoSegmentedCell cell)
			{
				cell.BackgroundColor = color;
				Control.SetNeedsDisplay();
			}
		}
	}
}
