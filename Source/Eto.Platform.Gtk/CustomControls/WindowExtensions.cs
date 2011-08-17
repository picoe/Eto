using System;
using Gtk;

namespace Eto.Platform.GtkSharp.CustomControls
{
	public static class WindowExtensions
	{
		public static void Grab (this Gtk.Window window)
		{
			window.GrabFocus ();
			
			Gtk.Grab.Add (window);

			Gdk.GrabStatus grabbed = Gdk.Pointer.Grab (window.GdkWindow, true, Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask, null, null, 0);

			if (grabbed == Gdk.GrabStatus.Success) {
				grabbed = Gdk.Keyboard.Grab (window.GdkWindow, true, 0);

				if (grabbed != Gdk.GrabStatus.Success) {
					Gtk.Grab.Remove (window);
					window.Destroy ();
				}
			} else {
				Gtk.Grab.Remove (window);
			}
		}
		
		public static void RemoveGrab (this Gtk.Window window)
		{
			Gtk.Grab.Remove (window);
			Gdk.Pointer.Ungrab (0);
			Gdk.Keyboard.Ungrab (0);
		}
		
	}
}

