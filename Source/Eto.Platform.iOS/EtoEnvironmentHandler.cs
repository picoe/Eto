using System;
using System.Runtime.InteropServices;
using System.Linq;
using MonoTouch.Foundation;

namespace Eto.Platform.iOS
{
	public class EtoEnvironmentHandler : WidgetHandler<Widget>, IEtoEnvironment
	{
		void Convert (EtoSpecialFolder folder, out NSSearchPathDirectory dir, out NSSearchPathDomain domain)
		{
			switch (folder) {
			case EtoSpecialFolder.ApplicationSettings:
				dir = NSSearchPathDirectory.ApplicationSupportDirectory;
				domain = NSSearchPathDomain.User;
				break;
			case EtoSpecialFolder.Documents:
				dir = NSSearchPathDirectory.DocumentDirectory;
				domain = NSSearchPathDomain.User;
				break;
			default:
				throw new NotSupportedException ();
			}
		}

		public string GetFolderPath (EtoSpecialFolder folder)
		{
			NSSearchPathDirectory dir;
			NSSearchPathDomain domain;
			Convert (folder, out dir, out domain);
			
			var manager = new NSFileManager();
			NSError error;
			var path = manager.GetUrl(dir, domain, null, false, out error);
			return path.Path;
		}

		public OperatingSystemPlatform GetPlatform()
		{
			return new OperatingSystemPlatform
			{
				IsUnix = true,
				IsMono = true,
				IsIos = true,
			};
		}
	}
}

