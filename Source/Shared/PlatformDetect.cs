﻿using System;
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
	}
}
