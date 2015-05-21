using System;
using System.Runtime.InteropServices;
using GLib;

namespace Eto.GtkSharp
{
	static class NativeMethods
	{
		#if GTK2
		const string ver = "2.0";
		#elif GTK3
		const string ver = "3";
		#endif

		static class NativeMethodsWindows
		{
			#if GTK2
			const string plat = "win32-";
			#elif GTK3
			const string plat = "";
			#endif

			const string ext = "-0.dll";
			const string libgobject = "libgobject-" + ver + ext;
			const string libgtk = "libgtk-" + plat + ver + ext;

			[DllImport(libgobject, CallingConvention = CallingConvention.Cdecl)]
			public static extern void g_signal_stop_emission_by_name(IntPtr instance, IntPtr name);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static bool gtk_clipboard_wait_for_targets(IntPtr cp, out IntPtr atoms, out int number);
		}

		static class NativeMethodsLinux
		{
			#if GTK2
			const string plat = "x11-";
			#elif GTK3
			const string plat = "";
			#endif

			const string ext = ".so.0";
			const string libgobject = "libgobject-" + ver + ext;
			const string libgtk = "libgtk-" + plat + ver + ext;

			[DllImport(libgobject, CallingConvention = CallingConvention.Cdecl)]
			public static extern void g_signal_stop_emission_by_name(IntPtr instance, IntPtr name);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static bool gtk_clipboard_wait_for_targets(IntPtr cp, out IntPtr atoms, out int number);
		}

		static class NativeMethodsMac
		{
			#if GTK2
			const string plat = "quartz-";
			#elif GTK3
			const string plat = "";
			#endif

			const string ext = ".dylib";
			const string libgobject = "libgobject-" + ver + ext;
			const string libgtk = "libgtk-" + plat + ver + ext;

			[DllImport(libgobject, CallingConvention = CallingConvention.Cdecl)]
			public static extern void g_signal_stop_emission_by_name(IntPtr instance, IntPtr name);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static bool gtk_clipboard_wait_for_targets(IntPtr cp, out IntPtr atoms, out int number);
		}

		static readonly Action<IntPtr, IntPtr> stopEmmisionByName;

		public delegate bool ClipboardWaitForTargetsDelegate(IntPtr cp, out IntPtr atoms, out int number);

		static readonly ClipboardWaitForTargetsDelegate clipboardWaitForTargets;

		static NativeMethods()
		{
			// instead of requiring an accompanying .config file.
			if (EtoEnvironment.Platform.IsLinux)
			{
				stopEmmisionByName = NativeMethodsLinux.g_signal_stop_emission_by_name;
				clipboardWaitForTargets = NativeMethodsLinux.gtk_clipboard_wait_for_targets;
			}
			else if (EtoEnvironment.Platform.IsMac)
			{
				stopEmmisionByName = NativeMethodsMac.g_signal_stop_emission_by_name;
				clipboardWaitForTargets = NativeMethodsMac.gtk_clipboard_wait_for_targets;
			}
			else
			{
				stopEmmisionByName = NativeMethodsWindows.g_signal_stop_emission_by_name;
				clipboardWaitForTargets = NativeMethodsWindows.gtk_clipboard_wait_for_targets;
			}
		}

		public static void StopEmissionByName(GLib.Object o, string signal)
		{
			IntPtr intPtr = Marshaller.StringToPtrGStrdup(signal);
			stopEmmisionByName(o.Handle, intPtr);
			Marshaller.Free(intPtr);
		}

		public static bool ClipboardWaitForTargets(IntPtr cp, out Gdk.Atom[] atoms)
		{
			IntPtr atomPtrs;
			int count;
			var success = clipboardWaitForTargets(cp, out atomPtrs, out count);
			if (!success || count <= 0)
			{
				atoms = null;
				return false;
			}

			atoms = new Gdk.Atom[count];
			unsafe
			{
				byte* p = (byte*)atomPtrs.ToPointer();
				for (int i = 0; i < count; i++)
				{
					atoms[i] = new Gdk.Atom(new IntPtr(*p));
					p += IntPtr.Size;
				}
			}
			return true;
		}
	}

}

