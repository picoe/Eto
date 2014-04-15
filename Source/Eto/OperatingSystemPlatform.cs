using System;
using System.Runtime.InteropServices;

namespace Eto
{
	/// <summary>
	/// Operating system platform information
	/// </summary>
	/// <remarks>
	/// Access this information from <see cref="EtoEnvironment.Platform"/>
	/// </remarks>
	public sealed class OperatingSystemPlatform
	{
		/// <summary>
		/// Gets a value indicating that the current .NET runtime is mono
		/// </summary>
		public bool IsMono { get; private set; }

		/// <summary>
		/// Gets a value indicating that the current OS is windows system
		/// </summary>
		public bool IsWindows { get; private set; }

		/// <summary>
		/// Gets a value indicating that the current OS is a Windows Runtime (WinRT) system.
		/// </summary>
		public bool IsWinRT { get; private set; }

		/// <summary>
		/// Gets a value indicating that the current OS is a unix-based system
		/// </summary>
		/// <remarks>
		/// This will be true for both Unix (e.g. OS X) and all Linux variants.
		/// </remarks>
		public bool IsUnix { get; private set; }

		/// <summary>
		/// Gets a value indicating that the current OS is a Mac OS X system
		/// </summary>
		public bool IsMac { get; private set; }

		/// <summary>
		/// Gets a value indicating that the current OS is a Linux system
		/// </summary>
		public bool IsLinux { get; private set; }

#if !PCL
		[DllImport("libc")]
		static extern int uname(IntPtr buf);

		static string GetUnixType()
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
#endif
		/// <summary>
		/// Initializes a new instance of the OperatingSystemPlatform class
		/// </summary>
		public OperatingSystemPlatform()
		{
#if WINRT
			// System.Environment.OSVersion does not exist on WinRT so
			// unfortunately we have to rely on a compiler #if directive.
			// Will need to think about a portable way to do this.
			IsWinRT = true;
#else

			if (Type.GetType("Mono.Runtime", false) != null || Type.GetType("Mono.Interop.IDispatch", false) != null)
				IsMono = true;

			switch (System.Environment.OSVersion.Platform)
			{
				case PlatformID.MacOSX:
					IsMac = true;
					IsUnix = true;
					break;
				case PlatformID.Unix:
					IsUnix = true;
					switch (GetUnixType().ToUpperInvariant())
					{
						case "DARWIN":
							IsMac = true;
							break;
						case "LINUX":
							IsLinux = true;
							break;
					}
					break;
				case PlatformID.Win32NT:
				case PlatformID.Win32S:
				case PlatformID.Win32Windows:
				case PlatformID.WinCE:
					IsWindows = true;
					break;
				default:
					// treat everything else as windows
					IsWindows = true;
					break;
			}
#endif
		}
	}
}
