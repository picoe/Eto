using System;
using System.Runtime.InteropServices;
using System.Linq;
using Foundation;

namespace Eto.iOS
{
	public class EtoEnvironmentHandler : WidgetHandler<Widget>, EtoEnvironment.IHandler
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
	}
}

