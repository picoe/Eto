using System;
using System.Runtime.InteropServices;
using GLib;
using System.Reflection;

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

#pragma warning disable 0649

		static class Impl
		{
			public static readonly Action<IntPtr, IntPtr> g_signal_stop_emission_by_name;

			public delegate bool ClipboardWaitForTargetsDelegate(IntPtr cp, out IntPtr atoms, out int number);

			public static readonly ClipboardWaitForTargetsDelegate gtk_clipboard_wait_for_targets;
		}

#pragma warning restore 0649

		static NativeMethods()
		{
			var fields = typeof(Impl).GetFields();
			Type platformType;
			if (EtoEnvironment.Platform.IsLinux)
				platformType = typeof(NativeMethodsLinux);
			else if (EtoEnvironment.Platform.IsMac)
				platformType = typeof(NativeMethodsMac);
			else
				platformType = typeof(NativeMethodsWindows);
			
			// instead of requiring an accompanying .config file.
			foreach (var field in fields)
			{
				var method = platformType.GetMethod(field.Name);
				if (method != null)
					field.SetValue(null, Delegate.CreateDelegate(field.FieldType, method));
			}
		}

		public static void StopEmissionByName(GLib.Object o, string signal)
		{
			if (Impl.g_signal_stop_emission_by_name == null)
				return;
			IntPtr intPtr = Marshaller.StringToPtrGStrdup(signal);
			Impl.g_signal_stop_emission_by_name(o.Handle, intPtr);
			Marshaller.Free(intPtr);
		}

		public static bool ClipboardWaitForTargets(IntPtr cp, out Gdk.Atom[] atoms)
		{
			if (Impl.gtk_clipboard_wait_for_targets == null)
			{
				atoms = null;
				return false;
			}
			IntPtr atomPtrs;
			int count;
			var success = Impl.gtk_clipboard_wait_for_targets(cp, out atomPtrs, out count);
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

