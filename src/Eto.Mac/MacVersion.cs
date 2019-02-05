using System;
using System.Runtime.InteropServices;
#if XAMMAC2
using Foundation;
using ObjCRuntime;
#else
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
#endif

namespace Eto.Mac
{
	static class MacVersion
	{
		static int s_major;
		static int s_minor;
		static int s_patch;

		public static bool IsAtLeast(int major, int minor, int patch = 0)
		{
			// check major
			if (s_major < major)
				return false;
			if (s_major > major)
				return true;

			// major equals, check minor
			if (s_minor < minor)
				return false;
			if (s_minor > minor)
				return true;

			// minor equals, check patch
			return s_patch >= patch;
		}

		[DllImport("/System/Library/Frameworks/CoreServices.framework/CoreServices")]
		internal static extern short Gestalt(int selector, ref int response);

		static MacVersion()
		{
#if XAMMAC
			// TODO: Can't use IsOperatingSystemAtLeastVersion in monomac yet, it's not mapped.
			if (NSProcessInfo.ProcessInfo.RespondsToSelector(new Selector("operatingSystemVersion")))
			{
				var operatingSystemVersion = NSProcessInfo.ProcessInfo.OperatingSystemVersion;
				s_major = (int)operatingSystemVersion.Major;
				s_minor = (int)operatingSystemVersion.Minor;
				s_patch = (int)operatingSystemVersion.PatchVersion;
			}
			else
#endif
			{
				const int gestaltSystemVersionMajor = 0x73797331;
				const int gestaltSystemVersionMinor = 0x73797332;
				const int gestaltSystemVersionPatch = 0x73797333;

				Gestalt(gestaltSystemVersionMajor, ref s_major);
				Gestalt(gestaltSystemVersionMinor, ref s_minor);
				Gestalt(gestaltSystemVersionPatch, ref s_patch);
			}
		}
	}
}
