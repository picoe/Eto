using Eto.GtkSharp;
using Eto.GtkSharp.Drawing;
using Eto.GtkSharp.Forms.Menu;

namespace Eto.GtkSharp.Forms
{
	public class LinuxTrayIndicatorHandler : WidgetHandler<GLib.Object, TrayIndicator, TrayIndicator.ICallback>, TrayIndicator.IHandler
	{
		const string libappindicator = "libappindicator3.so.1";
		const string libgio = "libgio-2.0.so.0";
		const string libglib = "libglib-2.0.so.0";
		const string libgobject = "libgobject-2.0.so.0";
		Image image;
		string imagePath;
		ContextMenu menu;

		[DllImport(libappindicator, CallingConvention = CallingConvention.Cdecl)]
		public extern static IntPtr app_indicator_new(string id, string icon_name, int category);

		[DllImport(libappindicator, CallingConvention = CallingConvention.Cdecl)]
		public extern static void app_indicator_set_icon(IntPtr self, string icon_name);

		[DllImport(libappindicator, CallingConvention = CallingConvention.Cdecl)]
		public extern static void app_indicator_set_menu(IntPtr self, IntPtr menu);

		[DllImport(libappindicator, CallingConvention = CallingConvention.Cdecl)]
		public extern static string app_indicator_get_title(IntPtr self);

		[DllImport(libappindicator, CallingConvention = CallingConvention.Cdecl)]
		public extern static void app_indicator_set_title(IntPtr self, string title);

		[DllImport(libappindicator, CallingConvention = CallingConvention.Cdecl)]
		public extern static int app_indicator_get_status(IntPtr self);

		[DllImport(libappindicator, CallingConvention = CallingConvention.Cdecl)]
		public extern static void app_indicator_set_status(IntPtr self, int status);

		[DllImport(libappindicator, CallingConvention = CallingConvention.Cdecl)]
		public extern static void app_indicator_dispose(IntPtr gobject);

		[DllImport(libgio, CallingConvention = CallingConvention.Cdecl)]
		static extern IntPtr g_bus_get_sync(GLib.BusType busType, IntPtr cancellable, IntPtr error);

		[DllImport(libgio, CallingConvention = CallingConvention.Cdecl)]
		static extern IntPtr g_dbus_connection_call_sync(IntPtr connection, string busName, string objectPath,
			string interfaceName, string methodName, IntPtr parameters, IntPtr replyType, GLib.DBusCallFlags flags,
			int timeoutMs, IntPtr cancellable, ref IntPtr error);

		[DllImport(libgio, CallingConvention = CallingConvention.Cdecl)]
		static extern int g_dbus_error_quark();

		[DllImport(libglib, CallingConvention = CallingConvention.Cdecl)]
		static extern void g_variant_unref(IntPtr value);

		[DllImport(libgobject, CallingConvention = CallingConvention.Cdecl)]
		static extern void g_object_unref(IntPtr obj);

		static uint Id = 0;

		// The standard name libappindicator waits for, then the one Cinnamon's watcher is activatable under.
		static readonly string[] WatcherNames = { "org.kde.StatusNotifierWatcher", "org.x.StatusNotifierWatcher" };

		public string Title
		{
			get { return app_indicator_get_title(Control.Handle); }
			set { app_indicator_set_title(Control.Handle, value); }
		}

		public bool Visible
		{
			get { return app_indicator_get_status(Control.Handle) == 1; }
			set
			{
				// A watcher is only needed once an icon is wanted, and asking on each Show restarts one that has died since.
				if (value)
					Task.Run(EnsureStatusNotifierWatcher);
				app_indicator_set_status(Control.Handle, value ? 1 : 0);
			}
		}

		public LinuxTrayIndicatorHandler()
		{
#if NET
			DllImportResolverManager.Add((name, assembly, path) =>
			{
				if (name == libappindicator)
				{
					IntPtr result = IntPtr.Zero;
					if (!NativeLibrary.TryLoad("libayatana-appindicator3.so.1", assembly, path, out result))
					{
						return IntPtr.Zero;
					}
					return result;
				}
				return IntPtr.Zero;
			});
#endif

			Control = GLib.Object.GetObject(app_indicator_new(Assembly.GetExecutingAssembly().FullName + Id, "", 0));
			app_indicator_set_menu(Control.Handle, (new Gtk.Menu()).Handle);

			Id++;
		}

		/// <summary>
		/// Asks the bus to start the StatusNotifier watcher if it is activatable but not running. Any call to an
		/// activatable name starts its service, and Peer.Ping takes no arguments. Nothing waits for the result:
		/// libappindicator watches the standard name and registers when it appears.
		/// </summary>
		static void EnsureStatusNotifierWatcher()
		{
			var connection = IntPtr.Zero;
			try
			{
				connection = g_bus_get_sync(GLib.BusType.Session, IntPtr.Zero, IntPtr.Zero);
				if (connection == IntPtr.Zero)
					return;

				foreach (var busName in WatcherNames)
				{
					var error = IntPtr.Zero;
					var reply = g_dbus_connection_call_sync(connection, busName, "/StatusNotifierWatcher",
						"org.freedesktop.DBus.Peer", "Ping", IntPtr.Zero, IntPtr.Zero, GLib.DBusCallFlags.None, 5000, IntPtr.Zero, ref error);
					if (reply != IntPtr.Zero)
					{
						g_variant_unref(reply);
						return;
					}
					// Only a name with no service at all moves on to the next; a running but unresponsive watcher is left alone.
					if (error == IntPtr.Zero)
						return;
					var failure = new GLib.GException(error);
					if (failure.Domain != g_dbus_error_quark() || failure.Code != (int)GLib.DBusError.ServiceUnknown)
						return;
				}
			}
			catch
			{
				// Watcher activation is optional; retain libappindicator's existing fallback.
			}
			finally
			{
				if (connection != IntPtr.Zero)
					g_object_unref(connection);
			}
		}

		void RemoveTempImage()
		{
			if (!string.IsNullOrEmpty(imagePath))
			{
				ApplicationHandler.TempFiles.Remove(imagePath);
				File.Delete(imagePath);
				imagePath = null;
			}
		}

		public Image Image
		{
			get { return image; }
			set
			{
				RemoveTempImage();
				imagePath = Path.GetTempFileName();

				image = value;
				image.ToGdk()?.Save(imagePath, "png");

				app_indicator_set_icon(Control.Handle, imagePath);
				ApplicationHandler.TempFiles.Add(imagePath);
			}
		}

		public ContextMenu Menu
		{
			get { return menu; }
			set
			{
				if (menu != null)
				{
					var handler = menu.Handler as ContextMenuHandler;
					if (handler != null)
					{
						handler.Changed -= ContextMenu_Changed;
					}
				}
				menu = value;
				if (menu == null)
					app_indicator_set_menu(Control.Handle, (new Gtk.Menu()).Handle);
				else
				{
					app_indicator_set_menu(Control.Handle, menu.ToGtk().Handle);
					var handler = menu.Handler as ContextMenuHandler;
					if (handler != null)
					{
						// need to re-set the when it has changed.. I guess.
						handler.Changed += ContextMenu_Changed;
					}
				}
			}
		}

		void ContextMenu_Changed(object sender, EventArgs e)
		{
			app_indicator_set_menu(Control.Handle, menu.ToGtk().Handle);
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TrayIndicator.ActivatedEvent:
					// Appindicator only has a context menu.
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (Control != null)
			{
				Visible = false;
				Control.Dispose();
				Control = null;
			}
			RemoveTempImage();
			base.Dispose(disposing);
		}
	}
}
