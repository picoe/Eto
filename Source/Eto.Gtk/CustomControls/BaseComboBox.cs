using System;
using Gtk;

namespace Eto.GtkSharp.CustomControls
{
	public class BaseComboBox 
	#if GTK3
		: EventBox
	#else
		: SizableBin
	#endif
	{
		Entry entry;
		Button popupButton;

		public BaseComboBox()
		{
			AppPaintable = true;
			Build();
#if GTK3
			SetSizeRequest(150, 30);
			foreach (var cls in PopupButton.StyleContext.ListClasses())
				PopupButton.StyleContext.RemoveClass(cls);
			entry.StyleContext.RemoveClass("entry");
			StyleContext.AddClass("entry");
#else
			BorderWidth = 1;
#endif
		}
		#if GTK2
		int vpadding;
		static readonly int DefaultEntryHeight = new ComboBoxEntry().SizeRequest().Height;

		protected override void OnSizeRequested(ref Requisition requisition)
		{
			base.OnSizeRequested(ref requisition);
			requisition.Height += vpadding; // for border
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
		#else
		protected override bool OnDrawn(Cairo.Context cr)
		{
			bool ret = true;
			var rect = this.Allocation;
			if (rect.Width > 0 && rect.Height > 0)
			{
				var arrowWidth = popupButton.Allocation.Width;
				var arrowPos = rect.Width - arrowWidth;
				var arrowSize = 10;

				StyleContext.Save();
				StyleContext.State = Entry.StyleContext.State;
				StyleContext.RenderBackground(cr, 0, 0, rect.Width, rect.Height);

				ret = base.OnDrawn(cr);

				StyleContext.RenderArrow(cr, Math.PI, arrowPos, (rect.Height - arrowSize) / 2, arrowSize);

				cr.SetSourceColor(new Cairo.Color(.8, .8, .8));
				cr.Rectangle(arrowPos - 5, 2, 1, rect.Height - 4);
				cr.Fill();

				StyleContext.RenderFrame(cr, 0, 0, rect.Width, rect.Height);
				StyleContext.Restore();

			}
			return ret;
		}

		class InvisibleButton : Gtk.Button
		{
			public InvisibleButton()
			{
				AppPaintable = true;
			}

			protected override bool OnDrawn(Cairo.Context cr)
			{
				return false; //return base.OnDrawn(cr);
			}
		}

		#endif
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
#if GTK3
			popupButton = new InvisibleButton();
#else

			popupButton = new Button();
#endif
			popupButton.WidthRequest = ArrowSize + 6;
			popupButton.CanFocus = false;
			return popupButton;
		}

		void Build()
		{
			var vbox = new VBox();
			var hbox = new HBox();

#if GTK2
			CreateEntry();
			vpadding = (DefaultEntryHeight - entry.SizeRequest().Height) / 2;
			entry.HeightRequest = -1;
			hbox.PackStart(entry, true, true, 4);
			hbox.PackEnd(CreatePopupButton(), false, false, 2);
			vbox.PackStart(hbox, true, true, (uint)vpadding);
#else
			hbox.PackStart(CreateEntry(), true, true, 8);
			hbox.PackEnd(CreatePopupButton(), false, false, 2);
			vbox.PackStart(hbox, true, true, 0);
#endif
			Add(vbox);
		}
	}
}

