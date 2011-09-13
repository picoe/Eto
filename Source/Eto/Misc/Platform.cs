using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Eto.Misc
{
	public static class Platform
	{
		public static readonly bool IsMono;
		public static readonly bool IsWindows;
		public static readonly bool IsUnix;
		public static readonly bool IsMac;
		public static readonly bool IsLinux;

		[DllImport( "libc" )]
		static extern int uname (IntPtr buf);
		
		static string GetUnixType ()
		{
			IntPtr buf = IntPtr.Zero;
			string osName = "";			
			try {
				buf = Marshal.AllocHGlobal (8192);
				if (uname (buf) == 0)
					osName = Marshal.PtrToStringAnsi (buf);
			} catch {
			} finally {
				if (buf != IntPtr.Zero)
					Marshal.FreeHGlobal (buf);
			}
			return osName;
 
		}
		static Platform ()
		{
			if (Type.GetType ("Mono.Runtime", false) != null)
				IsMono = true;
            
			switch (Environment.OSVersion.Platform)
			{
			case PlatformID.MacOSX:
				IsMac = true;
				IsUnix = true;
				break;
			case PlatformID.Unix:
				IsUnix = true;
				switch (GetUnixType ().ToLowerInvariant ()) {
				case "darwin": IsMac = true; break;
				case "linux": IsLinux = true; break;
				}
				break;
			default:
			case PlatformID.Win32NT:
			case PlatformID.Win32S:
			case PlatformID.Win32Windows:
			case PlatformID.WinCE:
				IsWindows = true;
				break;
			}
		}
	}
}
