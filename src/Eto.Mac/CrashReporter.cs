using System;
using System.Runtime.InteropServices;

namespace Eto.Mac
{
	public static class CrashReporter
	{
		[DllImport("libSystem.B.Dylib")]
		static extern IntPtr dlsym(IntPtr handle, string symbol);

		[DllImport("libc")]
		static extern void signal(int sig, IntPtr handler);

		[DllImport("libc")]
		static extern void kill(int pid, int sig);

		[DllImport("libc")]
		static extern int getpid();

		const int SIGSEGV = 11;

		public static void Attach(AppDomain domain = null)
		{
			domain = domain ?? AppDomain.CurrentDomain;
			domain.UnhandledException += CurrentDomain_UnhandledException;
		}

		static IntPtr? s_crashReporter;

		static IntPtr CrashReporterPtr => s_crashReporter
			?? (s_crashReporter = dlsym(new IntPtr(-2), "__crashreporter_info__"))
			?? IntPtr.Zero; // -2 = auto

		public static unsafe void Write(string value)
		{
			var crashReporter = CrashReporterPtr;
			if (crashReporter != IntPtr.Zero)
			{
				var ptr = (IntPtr*)crashReporter;
				if (*ptr != IntPtr.Zero)
				{
					// append to existing string
					var existingMsg = Marshal.PtrToStringAuto(*ptr);
					if (!string.IsNullOrEmpty(existingMsg))
						value = existingMsg
							+ Environment.NewLine
							+ Environment.NewLine
							+ value;
				}
				*ptr = Marshal.StringToHGlobalAuto(value);
			}
		}

		static void CurrentDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e)
		{
			// if we're not debugging, generate native crash reports with .NET exception info included
			Write(e.ExceptionObject?.ToString());
			signal(SIGSEGV, IntPtr.Zero); // replace mono's SIGSEGV handler with the system default
			kill(getpid(), SIGSEGV); // cause a SIGSEGV
		}
	}
}
