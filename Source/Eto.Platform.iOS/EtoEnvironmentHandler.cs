using System;
using System.Runtime.InteropServices;
using System.Linq;
using MonoTouch.Foundation;

namespace Eto.Platform.iOS
{
	public class EtoEnvironmentHandler : WidgetHandler, IEtoEnvironment
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
			var path = manager.GetUrl(dir, domain, new NSUrl(), false, out error);
			return path.Path;
		}

		#region IWidget implementation
		
		public void Initialize ()
		{
		}

		public IWidget Handler { get; set; }
		
		#endregion
	}
}

