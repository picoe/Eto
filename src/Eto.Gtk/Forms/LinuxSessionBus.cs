namespace Eto.GtkSharp.Forms
{
	/// <summary>
	/// Minimal access to the D-Bus session bus, which the GtkSharp bindings don't expose.
	/// </summary>
	static class LinuxSessionBus
	{
		const string libgio = "libgio-2.0.so.0";
		const string libglib = "libglib-2.0.so.0";
		const int G_BUS_TYPE_SESSION = 2;

		[DllImport(libgio, CallingConvention = CallingConvention.Cdecl)]
		static extern IntPtr g_bus_get_sync(int bus_type, IntPtr cancellable, out IntPtr error);

		[DllImport(libgio, CallingConvention = CallingConvention.Cdecl)]
		static extern bool g_dbus_connection_emit_signal(IntPtr connection, string destination_bus_name, string object_path, string interface_name, string signal_name, IntPtr parameters, out IntPtr error);

		[DllImport(libgio, CallingConvention = CallingConvention.Cdecl)]
		static extern IntPtr g_dbus_connection_call_sync(IntPtr connection, string bus_name, string object_path, string interface_name, string method_name, IntPtr parameters, IntPtr reply_type, int flags, int timeout_msec, IntPtr cancellable, out IntPtr error);

		[DllImport(libglib, CallingConvention = CallingConvention.Cdecl)]
		static extern IntPtr g_variant_get_child_value(IntPtr value, UIntPtr index);

		[DllImport(libglib, CallingConvention = CallingConvention.Cdecl)]
		static extern bool g_variant_get_boolean(IntPtr value);

		[DllImport(libglib, CallingConvention = CallingConvention.Cdecl)]
		static extern void g_variant_unref(IntPtr value);

		[DllImport(libglib, CallingConvention = CallingConvention.Cdecl)]
		static extern void g_error_free(IntPtr error);

		static IntPtr connection;
		static bool connectionFailed;

		static IntPtr Connection
		{
			get
			{
				if (connection == IntPtr.Zero && !connectionFailed)
				{
					try
					{
						connection = g_bus_get_sync(G_BUS_TYPE_SESSION, IntPtr.Zero, out var error);
						FreeError(error);
					}
					catch (DllNotFoundException)
					{
					}
					connectionFailed = connection == IntPtr.Zero;
				}
				return connection;
			}
		}

		static void FreeError(IntPtr error)
		{
			if (error != IntPtr.Zero)
				g_error_free(error);
		}

		/// <summary>
		/// Gets whether a service with the specified well-known name is running on the session bus.
		/// </summary>
		public static bool NameHasOwner(string name)
		{
			if (Connection == IntPtr.Zero)
				return false;
			var parameters = GLib.Variant.NewTuple(new[] { new GLib.Variant(name) });
			var result = g_dbus_connection_call_sync(connection, "org.freedesktop.DBus", "/org/freedesktop/DBus", "org.freedesktop.DBus", "NameHasOwner", parameters.Handle, IntPtr.Zero, 0, -1, IntPtr.Zero, out var error);
			FreeError(error);
			if (result == IntPtr.Zero)
				return false;
			var child = g_variant_get_child_value(result, UIntPtr.Zero);
			var hasOwner = g_variant_get_boolean(child);
			g_variant_unref(child);
			g_variant_unref(result);
			return hasOwner;
		}

		/// <summary>
		/// Broadcasts a signal to anyone listening on the session bus.
		/// </summary>
		public static void EmitSignal(string objectPath, string interfaceName, string signalName, GLib.Variant parameters)
		{
			if (Connection == IntPtr.Zero)
				return;
			g_dbus_connection_emit_signal(connection, null, objectPath, interfaceName, signalName, parameters.Handle, out var error);
			FreeError(error);
		}
	}
}
