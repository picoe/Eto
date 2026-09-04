#if GTK3
using Eto.GtkSharp;
using Eto.Test.UnitTests;
using System.Threading.Tasks;
using g = Gtk;

namespace Eto.Test.Gtk
{
	public class TestInput : ITestInput
	{
		static g.Window GetWindow(Window window) => (g.Window)window.ControlObject;

		// GDK stamps every event with a monotonic timestamp that GTK compares (key repeat, grabs,
		// focus changes), so each synthesized event needs a later one than the last.
		static uint s_time = 1000;

		public Task SendKeyDownAsync(Window window, Keys key) => SendKeyAsync(window, key, Gdk.EventType.KeyPress);

		public Task SendKeyUpAsync(Window window, Keys key) => SendKeyAsync(window, key, Gdk.EventType.KeyRelease);

		/// <summary>
		/// Pushes a synthesized key event through GTK's own dispatch.
		/// </summary>
		/// <remarks>
		/// gtk_propagate_event is what GTK itself uses to deliver a key: handing it the toplevel makes
		/// GtkWindow run its real key handling - accelerators and mnemonics first, then the focus
		/// widget, then the window's own key bindings - so the key takes the same path a physical one
		/// would.  Injecting through the X server instead (XTEST/xdotool) would depend on which window
		/// the display's input focus happens to be on.
		/// <para>
		/// Note gtk_main_do_event, the entry point a real event comes in through, is not usable here:
		/// it silently drops the event once any earlier test in the same process has shown and closed a
		/// window with a ComboBox in it, which makes every later key test fail in a full run but not on
		/// its own.
		/// </para>
		/// </remarks>
		static Task SendKeyAsync(Window window, Keys key, Gdk.EventType type)
		{
			var control = GetWindow(window);
			var gdkWindow = control.Window;
			if (gdkWindow == null)
				throw new InvalidOperationException("The window has to be shown before it can be sent a key");

			var keyval = key.ToGdkKey();
			if (keyval == 0)
				throw new NotSupportedException($"There is no GDK key for {key & Keys.KeyMask}");

			var e = Gdk.EventHelper.New(type);
			// the managed EventKey is a view over the native event allocated above, so setting its
			// properties fills in the event that gets dispatched
			new Gdk.EventKey(e.Handle)
			{
				Window = gdkWindow,
				SendEvent = true,
				Time = s_time += 100,
				KeyValue = (uint)keyval,
				// Eto maps the key from the hardware keycode when there is one (so that e.g. Shift+;
				// reports Semicolon rather than the colon it produces), so a made up event needs the
				// real keycode for the key as well
				HardwareKeycode = GetKeycode(keyval),
				State = key.ToGdkModifier()
			};
			// GDK warns about an event that carries no device, and its input method handling wants one
			Gdk.EventHelper.SetDevice(e, Gdk.Display.Default.DefaultSeat.Keyboard);

			g.Global.PropagateEvent(control, e);
			return Task.CompletedTask;
		}

		static ushort GetKeycode(Gdk.Key keyval)
		{
			var keys = Gdk.Keymap.Default.GetEntriesForKeyval((uint)keyval);
			return keys?.Length > 0 ? (ushort)keys[0].Keycode : (ushort)0;
		}

		/// <summary>
		/// GTK keeps the keyboard focus per toplevel: a window that has it either has a focus widget
		/// inside it, or has none and then handles the keys itself.
		/// </summary>
		public bool IsWindowFocusedItself(Window window)
		{
			var control = GetWindow(window);
			return control.HasToplevelFocus && control.Focus == null;
		}

		/// <remarks>
		/// HasToplevelFocus is the window the window manager gave the keyboard to, whether or not
		/// anything inside it has taken the focus. Note this stays true for a window with no focus
		/// widget, because GTK still delivers its keys to the toplevel.
		/// </remarks>
		public bool IsFocusWithinWindow(Window window) => GetWindow(window).HasToplevelFocus;
	}
}
#endif
