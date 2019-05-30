#if GTK2
using System;
using Gtk;

namespace Eto.GtkSharp.CustomControls
{
	public class BaseComboBox : SizableBin
	{
		Entry entry;
		Button popupButton;

		public BaseComboBox()
		{
			AppPaintable = true;
			Build();
			BorderWidth = 1;
		}

		int vpadding;
		static readonly int DefaultEntryHeight = new ComboBoxEntry().SizeRequest().Height;

		protected override void OnSizeRequested(ref Requisition requisition)
		{
			base.OnSizeRequested(ref requisition);
			requisition.Height += vpadding; // for border
		}

		public event EventHandler PopupButtonClicked
		{
			add => PopupButton.Clicked += value;
			remove => PopupButton.Clicked -= value;
		}

		[GLib.ConnectBefore]
		protected override bool OnExposeEvent(Gdk.EventExpose evnt)
		{
			var rect = Allocation;
			
			if (rect.Width > 0 && rect.Height > 0)
			{
				var area = evnt.Area;
				area.Height++;
				if (BorderWidth > 0)
					Gtk.Style.PaintShadow(Entry.Style, GdkWindow, Entry.State, ShadowType.In, area, Entry, "entry", rect.X, rect.Y, rect.Width, rect.Height + 1);
				else
					GdkWindow.DrawRectangle(Entry.Style.BaseGC(Entry.State), true, area);
				var popupWidth = popupButton.Allocation.Width;
				var vline = rect.Right - popupWidth - 2;
				Gtk.Style.PaintVline(Entry.Style, GdkWindow, Entry.State, area, this, "line", rect.Top + 4, rect.Bottom - 4, vline);
				var arrowWidth = popupWidth / 2;
				var arrowPos = vline + (popupWidth - arrowWidth) / 2 + 1;
				Gtk.Style.PaintArrow(Entry.Style, GdkWindow, Entry.State, ShadowType.None, area, this, "arrow", ArrowType.Down, true, arrowPos, rect.Top, arrowWidth, rect.Height);
			}
			return true;
		}
		public Entry Entry { get { return entry; } }

		public Button PopupButton { get { return popupButton; } }

		Gtk.Widget CreateEntry()
		{
			entry = new Entry
			{
				CanFocus = true,
				IsEditable = true,
				HasFrame = false,
			};

			entry.FocusInEvent += delegate
			{
				QueueDraw();
			};
			
			entry.FocusOutEvent += delegate
			{
				QueueDraw();
			};
			return entry;
		}

		static readonly int ArrowSize = Convert.ToInt32(new ComboBox().StyleGetProperty("arrow-size"));

		Gtk.Widget CreatePopupButton()
		{
			popupButton = new Button();
			popupButton.WidthRequest = ArrowSize + 6;
			popupButton.CanFocus = false;
			return popupButton;
		}

		void Build()
		{
			var vbox = new VBox();
			var hbox = new HBox();

			CreateEntry();
			vpadding = (DefaultEntryHeight - entry.SizeRequest().Height) / 2;
			entry.HeightRequest = -1;
			hbox.PackStart(entry, true, true, 4);
			hbox.PackEnd(CreatePopupButton(), false, false, 2);
			vbox.PackStart(hbox, true, true, (uint)vpadding);
			Add(vbox);
		}
	}
}

#endif