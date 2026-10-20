
namespace Eto.GtkSharp.CustomControls
{
	public static class WindowExtensions
	{
		public static void Grab(this Gtk.Window window)
		{
			window.GrabFocus();
			
			Gtk.Grab.Add(window);

#if GTK3
			var grabbed = window.Display.DefaultSeat.Grab(window.GetWindow(), Gdk.SeatCapabilities.Pointer, true, null, null, null);
			if (grabbed != Gdk.GrabStatus.Success)
			{
				Gtk.Grab.Remove(window);
			}
#else
			var mask = Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask;
			var grabbed = Gdk.Pointer.Grab(window.GdkWindow, true, mask, null, null, 0);
			if (grabbed == Gdk.GrabStatus.Success)
			{
				grabbed = Gdk.Keyboard.Grab(window.GdkWindow, true, 0);

				if (grabbed != Gdk.GrabStatus.Success)
				{
					Gtk.Grab.Remove(window);
					window.Destroy();
				}
			}
			else
			{
				Gtk.Grab.Remove(window);
			}
#endif
		}

		public static void RemoveGrab(this Gtk.Window window)
		{
			Gtk.Grab.Remove(window);
#if GTK3
			window.Display.DefaultSeat.Ungrab();
#else
			Gdk.Pointer.Ungrab(0);
			Gdk.Keyboard.Ungrab(0);
#endif
		}
		
	}
}

