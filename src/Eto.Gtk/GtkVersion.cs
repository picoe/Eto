using System;
namespace Eto.GtkSharp
{
	static class GtkVersion
	{
		static int s_major;
		static int s_minor;
		static int s_micro;

		public static bool IsAtLeast(int major, int minor, int micro = 0)
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
			return s_micro >= micro;
		}

		static GtkVersion()
		{
			s_major = (int)NativeMethods.gtk_get_major_version();
			s_minor = (int)NativeMethods.gtk_get_minor_version();
			s_micro = (int)NativeMethods.gtk_get_micro_version();
		}
	}
}
