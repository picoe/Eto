using System;
using System.Runtime.InteropServices;
using GLib;

namespace Eto.GtkSharp
{
	static class NativeMethods
	{
		const string libgobject = "libgobject-2.0";


		static class NativeMethodsWindows
		{
			[DllImport(libgobject + "-0.dll", CallingConvention = CallingConvention.Cdecl)]
			public static extern void g_signal_stop_emission_by_name(IntPtr instance, IntPtr name);
		}

		static class NativeMethodsLinux
		{
			[DllImport(libgobject + ".so.0", CallingConvention = CallingConvention.Cdecl)]
			public static extern void g_signal_stop_emission_by_name(IntPtr instance, IntPtr name);
		}

		static class NativeMethodsMac
		{
			[DllImport(libgobject + ".dylib", CallingConvention = CallingConvention.Cdecl)]
			public static extern void g_signal_stop_emission_by_name(IntPtr instance, IntPtr name);
		}

		static readonly Action<IntPtr, IntPtr> stopEmmisionByName;

		static NativeMethods()
		{
			// instead of requiring an accompanying .config file.
			if (EtoEnvironment.Platform.IsLinux)
			{
				stopEmmisionByName = NativeMethodsLinux.g_signal_stop_emission_by_name;
			}
			else if (EtoEnvironment.Platform.IsMac)
			{
				stopEmmisionByName = NativeMethodsMac.g_signal_stop_emission_by_name;
			}
			else
			{
				stopEmmisionByName = NativeMethodsWindows.g_signal_stop_emission_by_name;
			}
		}

		public static void StopEmissionByName(GLib.Object o, string signal)
		{
			IntPtr intPtr = Marshaller.StringToPtrGStrdup(signal);
			stopEmmisionByName(o.Handle, intPtr);
			Marshaller.Free(intPtr);
		}
	}

}

