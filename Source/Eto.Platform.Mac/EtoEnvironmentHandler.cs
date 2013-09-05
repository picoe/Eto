using System;
using System.Runtime.InteropServices;
using MonoMac.AppKit;
using MonoMac.Foundation;
using System.Linq;
using Eto.Forms;

namespace Eto.Platform.Mac
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
			switch (folder) {
			case EtoSpecialFolder.ApplicationResources:
				return NSBundle.MainBundle.ResourcePath;
			case EtoSpecialFolder.ApplicationSettings:
				Convert (folder, out dir, out domain);
				var path = NSSearchPath.GetDirectories (dir, domain, true).FirstOrDefault ();
				path = System.IO.Path.Combine (path, NSBundle.MainBundle.BundleIdentifier);
				if (!System.IO.Directory.Exists (path))
					System.IO.Directory.CreateDirectory (path);
				return path;
			default:
				Convert (folder, out dir, out domain);
				return NSSearchPath.GetDirectories (dir, domain, true).FirstOrDefault ();
			}
		}
	}
}

