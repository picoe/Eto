using System;
using Gtk;

namespace Eto.Platform.GtkSharp.CustomControls
{
	public class BaseComboBox : SizableBin
	{
		Gtk.Entry entry;
		Gtk.Button popupButton;
		
		public BaseComboBox ()
		{
			this.AppPaintable = true;
			this.Build ();
		}
		
		[GLib.ConnectBefore]
		protected override bool OnExposeEvent (Gdk.EventExpose evnt)
		{
			var rect = Allocation;
			
			if (rect.Width > 0 && rect.Height > 0) {
				//Gtk.Style.PaintFlatBox (Entry.Style, this.GdkWindow, Entry.State, Gtk.ShadowType.In, evnt.Area, this, "entry_bg", rect.X, rect.Y, rect.Width, rect.Height);
				Gtk.Style.PaintShadow (Entry.Style, this.GdkWindow, Entry.State, Gtk.ShadowType.In, evnt.Area, Entry, "entry", rect.X, rect.Y, rect.Width, rect.Height);
				var arrowWidth = popupButton.Allocation.Width;
				var arrowPos = rect.Right - arrowWidth - 4;
				Gtk.Style.PaintArrow(Entry.Style, this.GdkWindow, Entry.State, Gtk.ShadowType.None, evnt.Area, this, "arrow", ArrowType.Down, true, arrowPos, rect.Top, arrowWidth, rect.Height);
				Gtk.Style.PaintVline(Entry.Style, this.GdkWindow, Entry.State, evnt.Area, this, "line", rect.Top + 4, rect.Bottom - 4, arrowPos - 1);
			}
			return true;
		}

		public Entry Entry {
			get {
				return entry;
			}
		}

		public Button PopupButton {
			get {
				return popupButton;
			}
		}
		
		Gtk.Widget CreateEntry ()
		{
			this.entry = new Gtk.Entry {
				CanFocus = true,
				IsEditable = true,
				HasFrame = false
			};

			entry.FocusInEvent += delegate {
				QueueDraw ();
			};
			
			entry.FocusOutEvent += delegate {
				QueueDraw ();
			};
			return entry;
		}
		
		Gtk.Widget CreatePopupButton ()
		{
			this.popupButton = new Gtk.Button {
				WidthRequest = 18,
				CanFocus = false
			};

			return popupButton;
		}
        
		protected virtual void Build ()
		{
			var vbox = new Gtk.VBox();
			var hbox = new Gtk.HBox ();

			hbox.PackStart (CreateEntry (), true, true, 5);

			hbox.PackEnd (CreatePopupButton (), true, false, 1);

			vbox.PackStart(hbox, true, true, 4);
			this.Add (vbox);
		}
		
	}
}

