using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Eto
{
	#if XAMMAC2
	[Foundation.Preserve(AllMembers = true)]
	#elif OSX
	[MonoMac.Foundation.Preserve(AllMembers = true)]
	#endif
	static class PlatformDetect
	{
		[DllImport("libc")]
		static extern int uname(IntPtr buf);

		public static string GetUnixType()
		{
			IntPtr buf = IntPtr.Zero;
			string osName = "";
			try
			{
				buf = Marshal.AllocHGlobal(8192);
				if (uname(buf) == 0)
					osName = Marshal.PtrToStringAnsi(buf);
			}
			// Analysis disable once EmptyGeneralCatchClause
			catch
			{
			}
			finally
			{
				if (buf != IntPtr.Zero)
					Marshal.FreeHGlobal(buf);
			}
			return osName;
		}

#if GTK3
		private static Assembly _etoassembly = typeof(Eto.Forms.Button).Assembly;

		public static Assembly GetCallingAssembly()
		{
			var s = new StackTrace();

			for (int i = 0; i < s.FrameCount; i++)
			{
				if (_etoassembly.Equals(s.GetFrame(i).GetMethod().DeclaringType.Assembly))
					return s.GetFrame(i + 1).GetMethod().DeclaringType.Assembly;
			}

			throw new Exception("Failed to get executing assembly.");
		}
#endif
	}
}
