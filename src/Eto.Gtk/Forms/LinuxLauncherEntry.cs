namespace Eto.GtkSharp.Forms
{
	/// <summary>
	/// Shows a count badge on the app's dock icon via the Unity LauncherEntry D-Bus API
	/// (Ubuntu Dock, Dash to Dock, KDE, Plank). Needs an installed .desktop file matching the app.
	/// </summary>
	static class LinuxLauncherEntry
	{
		const string libgio = "libgio-2.0.so.0";
		const string libgobject = "libgobject-2.0.so.0";

		[DllImport(libgio, CallingConvention = CallingConvention.Cdecl)]
		static extern IntPtr g_desktop_app_info_new(string desktop_id);

		[DllImport(libgobject, CallingConvention = CallingConvention.Cdecl)]
		static extern void g_object_unref(IntPtr obj);

		static bool shown;

		static string DesktopId
		{
			get
			{
				// same variable the Unity taskbar progress uses
				var desktopEntry = Environment.GetEnvironmentVariable("DESKTOP_ENTRY");
				if (!string.IsNullOrEmpty(desktopEntry))
					return Path.GetFileName(desktopEntry);

				// the file we were launched from (children inherit it, so check it was us),
				// otherwise the id the shell matches our windows by
				var desktopFile = Environment.GetEnvironmentVariable("GIO_LAUNCHED_DESKTOP_FILE");
				var launchedPid = Environment.GetEnvironmentVariable("GIO_LAUNCHED_DESKTOP_FILE_PID");
				if (!string.IsNullOrEmpty(desktopFile) && launchedPid == Process.GetCurrentProcess().Id.ToString(CultureInfo.InvariantCulture))
					return Path.GetFileName(desktopFile);
				return GLib.Global.ProgramName + ".desktop";
			}
		}

		static bool DesktopFileExists(string desktopId)
		{
			var info = g_desktop_app_info_new(desktopId);
			if (info == IntPtr.Zero)
				return false;
			g_object_unref(info);
			return true;
		}

		static bool TryGetCount(string label, out long count) => long.TryParse(label, NumberStyles.Integer, CultureInfo.CurrentCulture, out count);

		/// <summary>
		/// Gets whether the dock can show <paramref name="label"/>: it must be a whole number, a dock must be running,
		/// and the app must have a .desktop file.
		/// </summary>
		public static bool CanShow(string label)
		{
			// Ubuntu Dock and Dash to Dock own this name; docks that only listen for the signal aren't detected
			return TryGetCount(label, out _)
				&& LinuxSessionBus.NameHasOwner("com.canonical.Unity")
				&& DesktopFileExists(DesktopId);
		}

		/// <summary>
		/// Shows <paramref name="label"/> as the badge when it is a whole number, otherwise hides the badge.
		/// </summary>
		public static void Update(string label)
		{
			var hasCount = TryGetCount(label, out var count);
			if (!hasCount && !shown)
				return;
			shown = hasCount;

			var properties = new Dictionary<string, GLib.Variant>
			{
				{ "count", new GLib.Variant(hasCount ? count : 0L) },
				{ "count-visible", new GLib.Variant(hasCount) }
			};
			var appUri = "application://" + DesktopId;
			var parameters = GLib.Variant.NewTuple(new[] { new GLib.Variant(appUri), new GLib.Variant(properties) });
			// any unique path works; the dock only looks at the app uri
			var path = "/com/canonical/unity/launcherentry/" + ((uint)appUri.GetHashCode()).ToString(CultureInfo.InvariantCulture);
			LinuxSessionBus.EmitSignal(path, "com.canonical.Unity.LauncherEntry", "Update", parameters);
		}
	}
}
