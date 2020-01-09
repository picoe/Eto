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

		#if NETSTANDARD

		static string GetUnixType()
		{
			// need at least one of these platforms if we're detecting the unix type (mac/linux) in PCL
			var detectType = Type.GetType("Eto.PlatformDetect, Eto.XamMac2", false)
			                 ?? Type.GetType("Eto.PlatformDetect, Eto.Mac64", false)
			                 ?? Type.GetType("Eto.PlatformDetect, Eto.XamMac", false)
			                 ?? Type.GetType("Eto.PlatformDetect, Eto.Mac", false)
			                 ?? Type.GetType("Eto.PlatformDetect, Eto.Gtk", false)
			                 ?? Type.GetType("Eto.PlatformDetect, Eto.Gtk2", false)
			                 ?? Type.GetType("Eto.PlatformDetect, Eto.Gtk3", false);
			if (detectType != null)
			{
				var getUnixTypeMethod = detectType.GetRuntimeMethod("GetUnixType", new Type[] { });
				if (getUnixTypeMethod != null)
					return (string)getUnixTypeMethod.Invoke(null, null);
			}
			return string.Empty;
		}

		#else
		
		static string GetUnixType()
		{
			return PlatformDetect.GetUnixType();
		}

		#endif

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
