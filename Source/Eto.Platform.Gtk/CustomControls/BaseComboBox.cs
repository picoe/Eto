using System;
using Gtk;

namespace Eto.Platform.GtkSharp.CustomControls
{
	public class BaseComboBox : SizableBin
	{
		Entry entry;
		Button popupButton;
		
		public BaseComboBox ()
		{
			AppPaintable = true;
			Build ();
#if GTK3
			//popupButton.AppPaintable = true;
			//popupButton.Drawn += popup_Drawn;
			SetSizeRequest(150, 30);
			foreach (var cls in PopupButton.StyleContext.ListClasses())
				PopupButton.StyleContext.RemoveClass(cls);
			foreach (var cls in Entry.StyleContext.ListClasses())
			{
				//Console.WriteLine(cls);
				//Entry.StyleContext.RemoveClass(cls);
			}


#endif
			//popupButton.Visible = false;
			//popupButton.NoShowAll = true;

			//entry.AppPaintable = true;
			//popupButton.Drawn += (o, args) => {
			//	args.RetVal = false;
			//};
		}

#if GTK2
		[GLib.ConnectBefore]
		protected override bool OnExposeEvent (Gdk.EventExpose evnt)
		{
			var rect = Allocation;
			
			if (rect.Width > 0 && rect.Height > 0) {
				//Gtk.Style.PaintFlatBox (Entry.Style, this.GdkWindow, Entry.State, Gtk.ShadowType.In, evnt.Area, this, "entry_bg", rect.X, rect.Y, rect.Width, rect.Height);
				Gtk.Style.PaintShadow (Entry.Style, GdkWindow, Entry.State, ShadowType.In, evnt.Area, Entry, "entry", rect.X, rect.Y, rect.Width, rect.Height);
				var arrowWidth = popupButton.Allocation.Width;
				var arrowPos = rect.Right - arrowWidth - 4;
				Gtk.Style.PaintArrow(Entry.Style, GdkWindow, Entry.State, ShadowType.None, evnt.Area, this, "arrow", ArrowType.Down, true, arrowPos, rect.Top, arrowWidth, rect.Height);
				Gtk.Style.PaintVline(Entry.Style, GdkWindow, Entry.State, evnt.Area, this, "line", rect.Top + 4, rect.Bottom - 4, arrowPos - 1);
			}
			return true;
		}
#else
		//[GLib.ConnectBefore]
		protected override bool OnDrawn (Cairo.Context cr)
		{
			bool ret = true;
			var rect = this.Allocation;
			if (rect.Width > 0 && rect.Height > 0) {
				var arrowWidth = popupButton.Allocation.Width;
				var arrowPos = rect.Width - arrowWidth + 4;
				var arrowSize = 10;

				StyleContext.Save ();
                StyleContext.AddClass ("entry");
                StyleContext.RenderBackground (cr, 0, 0, rect.Width, rect.Height);

				ret = base.OnDrawn (cr);

				StyleContext.RenderArrow(cr, Math.PI, arrowPos, (rect.Height - arrowSize) / 2, arrowSize);

				cr.Color = new Cairo.Color(.8, .8, .8);
				cr.Rectangle(arrowPos - 5, 2, 1, rect.Height - 4);
				cr.Fill();

				Entry.StyleContext.RenderFrame (cr, 0, 0, rect.Width, rect.Height);
                StyleContext.Restore ();

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
		
		Gtk.Widget CreateEntry ()
		{
			entry = new Entry
			{
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
#if GTK3
			popupButton = new InvisibleButton();
#else

			popupButton = new Button();
#endif
			popupButton.WidthRequest = 25;
			popupButton.CanFocus = false;
			return popupButton;
		}
        
		protected virtual void Build ()
		{
			var vbox = new VBox();
			var hbox = new HBox ();

#if GTK2
			hbox.PackStart (CreateEntry (), true, true, 2);
			hbox.PackEnd (CreatePopupButton (), true, false, 2);
			vbox.PackStart(hbox, true, true, 4);
#else
			hbox.PackStart (CreateEntry (), true, true, 1);
			hbox.PackEnd (CreatePopupButton (), true, false, 0);
			vbox.PackStart(hbox, true, true, 1);
#endif

			Add(vbox);
		}
		
	}
}

