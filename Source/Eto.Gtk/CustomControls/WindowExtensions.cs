
namespace Eto.GtkSharp.CustomControls
{
	public static class WindowExtensions
	{
		public static void Grab(this Gtk.Window window)
		{
			window.GrabFocus();
			
			Gtk.Grab.Add(window);

			var mask = Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask;
#if GTK3
			var grabbed = window.Display.DeviceManager.ClientPointer.Grab(window.GetWindow(), Gdk.GrabOwnership.Window, true, mask, null, 0);
			if (grabbed != Gdk.GrabStatus.Success)
			{
				Gtk.Grab.Remove(window);
			}
#else
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
			window.Display.DeviceManager.ClientPointer.Ungrab(0);
#else
			Gdk.Pointer.Ungrab(0);
			Gdk.Keyboard.Ungrab(0);
#endif
		}
		
	}
}

