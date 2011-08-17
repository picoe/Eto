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
				Gtk.Style.PaintFlatBox (Entry.Style, this.GdkWindow, Entry.State, Gtk.ShadowType.In, evnt.Area, Entry, "entry_bg", rect.X, rect.Y, rect.Width, rect.Height);
				Gtk.Style.PaintShadow (Entry.Style, this.GdkWindow, Entry.State, Gtk.ShadowType.In, evnt.Area, Entry, "entry", rect.X, rect.Y, rect.Width, rect.Height);
			}
			return base.OnExposeEvent (evnt);
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
				WidthRequest = 20,
				CanFocus = true
			};

			popupButton.Add (new Gtk.Arrow (Gtk.ArrowType.Down, Gtk.ShadowType.Out));

			return popupButton;
		}
        
		protected virtual void Build ()
		{
			var hbox3 = new Gtk.HBox ();

			hbox3.PackStart (CreateEntry (), true, true, 5);

			hbox3.PackEnd (CreatePopupButton (), false, false, 0);
			
			this.Add (hbox3);
		}
		
	}
}

