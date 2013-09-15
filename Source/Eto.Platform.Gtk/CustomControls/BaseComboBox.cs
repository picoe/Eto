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

#if GTK2
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
#else
		protected override bool OnDrawn (Cairo.Context cr)
		{
			var rect = this.Allocation;
			if (rect.Width > 0 && rect.Height > 0) {
				entry.StyleContext.RenderFrame (cr, 0, 0, rect.Width, rect.Height);
			}
			return base.OnDrawn (cr);
		}

		protected override void OnAdjustSizeRequest (Orientation orientation, out int minimum_size, out int natural_size)
		{
			base.OnAdjustSizeRequest (orientation, out minimum_size, out natural_size);
			if (orientation == Orientation.Horizontal)
				natural_size = minimum_size = Math.Max (minimum_size, 150);
			else if (orientation == Orientation.Vertical)
				natural_size = minimum_size = Math.Max (minimum_size, 30);
		}

#endif

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

#if GTK3
			entry.MarginLeft = 2;
#endif
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
#if GTK3
			//popupButton.MarginBottom = 4;
			//popupButton.MarginTop = 4;
#endif

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

