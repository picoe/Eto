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

		#if PCL

		static string GetUnixType()
		{
			// need at least one of these platforms if we're detecting the unix type (mac/linux) in PCL
			var detectType = Type.GetType("Eto.PlatformDetect, Eto.XamMac")
			                 ?? Type.GetType("Eto.PlatformDetect, Eto.Mac")
			                 ?? Type.GetType("Eto.PlatformDetect, Eto.Gtk2")
			                 ?? Type.GetType("Eto.PlatformDetect, Eto.Gtk3");
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
			if (Type.GetType("Mono.Runtime", false) != null || Type.GetType("Mono.Interop.IDispatch", false) != null)
				IsMono = true;

			var winRtType = Type.GetType("Windows.ApplicationModel.DesignMode, Windows, ContentType=WindowsRuntime");
			IsWinRT = winRtType != null;

			#if PCL

			var windowsType = Type.GetType("System.ComponentModel.DesignerProperties, PresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
			IsWindows = windowsType != null || IsWinRT;

			// get Environment.OSVersion.Platform using reflection
			int platform = -1;
			var envType = typeof(Environment);
			var osVersionProp = envType.GetRuntimeProperty("OSVersion");
			if (osVersionProp != null)
			{
				var osVal = osVersionProp.GetValue(null);
				if (osVal != null)
				{
					var platProp = osVal.GetType().GetRuntimeProperty("Platform");
					platform = (int)platProp.GetValue(osVal);
				}
			}

			#else
			var platform = (int)Environment.OSVersion.Platform;
			#endif

			switch (platform)
			{
				case 6: // PlatformID.MacOSX:
					IsMac = true;
					IsUnix = true;
					break;
				case 4: // PlatformID.Unix:
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
				case 0: // PlatformID.Win32S:
				case 1: // PlatformID.Win32Windows:
				case 2: // PlatformID.Win32NT:
				case 3: // PlatformID.WinCE:
				case 5: // PlatformID.Xbox:
					IsWindows = true;
					break;
				default:
					// treat everything else as windows
					IsWindows = true;
					break;
			}
		}
	}
}
