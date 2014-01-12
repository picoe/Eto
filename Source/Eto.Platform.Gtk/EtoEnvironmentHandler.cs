using System;
using Eto;
using System.Reflection;
using System.IO;
using System.Runtime.InteropServices;

namespace Eto.Platform.GtkSharp
{
	public class EtoEnvironmentHandler : WidgetHandler<Widget>, IEtoEnvironment
	{
		static Environment.SpecialFolder Convert (EtoSpecialFolder folder)
		{
			switch (folder) {
			case EtoSpecialFolder.ApplicationSettings:
				return Environment.SpecialFolder.ApplicationData;
			case EtoSpecialFolder.Documents:
				return Environment.SpecialFolder.MyDocuments;
			default:
				throw new NotSupportedException ();
			}
		}

		public string GetFolderPath (EtoSpecialFolder folder)
		{
			switch (folder) {
			case EtoSpecialFolder.ApplicationResources:
				return Path.GetDirectoryName (Assembly.GetEntryAssembly ().Location);
			default:
				return Environment.GetFolderPath (Convert (folder));
			}
		}

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

		public OperatingSystemPlatform GetPlatform()
		{
			var result = new OperatingSystemPlatform();

			if (Type.GetType("Mono.Runtime", false) != null || Type.GetType("Mono.Interop.IDispatch", false) != null)
				result.IsMono = true;

			switch (System.Environment.OSVersion.Platform)
			{
				case PlatformID.MacOSX:
					result.IsMac = true;
					result.IsUnix = true;
					break;
				case PlatformID.Unix:
					result.IsUnix = true;
					switch (GetUnixType().ToUpperInvariant())
					{
						case "DARWIN":
							result.IsMac = true;
							break;
						case "LINUX":
							result.IsLinux = true;
							break;
					}
					break;
				case PlatformID.Win32NT:
				case PlatformID.Win32S:
				case PlatformID.Win32Windows:
				case PlatformID.WinCE:
					result.IsWindows = true;
					break;
				default:
					// treat everything else as windows
					result.IsWindows = true;
					break;
			}
			return result;
		}
	}
}

