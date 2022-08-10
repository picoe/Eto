using System;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Linq;

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
		/// Gets a value indicating that the current .NET runtime is .NET Core
		/// </summary>
		public bool IsNetCore { get; private set; }

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
		
		/// <summary>
		/// Initializes a new instance of the OperatingSystemPlatform class
		/// </summary>
		public OperatingSystemPlatform()
		{
#if NETSTANDARD2_0
			if (RuntimeInformation.FrameworkDescription.StartsWith(".NET Core", StringComparison.OrdinalIgnoreCase))
				IsNetCore = true;
#endif
			if (Type.GetType("Mono.Runtime", false) != null || Type.GetType("Mono.Interop.IDispatch", false) != null)
				IsMono = true;

			var winRtType = Type.GetType("Windows.ApplicationModel.DesignMode, Windows, ContentType=WindowsRuntime");
			IsWinRT = winRtType != null;

			if (Environment.NewLine == "\r\n")
				IsWindows = true;
			else
			{
				IsUnix = true;

				if (GetUnixType().ToUpperInvariant() == "DARWIN")
 					IsMac = true;
				else
					IsLinux = true;
			}
		}
	}
}
