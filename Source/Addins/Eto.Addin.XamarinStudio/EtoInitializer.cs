using System;
using System.Linq;

namespace Eto.Addin.XamarinStudio
{
	public static class EtoInitializer
	{
		static readonly string AddinPlatform = Eto.Platforms.Gtk2;

		public static void Initialize()
		{
			if (Platform.Instance == null)
			{
				new Eto.Forms.Application(AddinPlatform).Attach();
			}

			if (EtoEnvironment.Platform.IsMac)
			{
				// hack for OS X el capitan. mcs moved from /usr/bin to /usr/local/bin and is not on the path when XS is running
				// this should be removed when mono/XS is fixed.
				var path = Environment.GetEnvironmentVariable("PATH");
				var paths = path.Split(':');
				if (!paths.Contains("/usr/local/bin", StringComparer.Ordinal))
				{
					path += ":/usr/local/bin";
					Environment.SetEnvironmentVariable("PATH", path);
				}
			}
		}
	}
}

