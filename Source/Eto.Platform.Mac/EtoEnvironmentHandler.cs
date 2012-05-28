using System;
using System.Runtime.InteropServices;
using MonoMac.AppKit;
using MonoMac.Foundation;
using System.Linq;

namespace Eto.Platform.Mac
{
	public class EtoEnvironmentHandler : IEtoEnvironment
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
			switch (folder) {
			case EtoSpecialFolder.ApplicationResources:
				return NSBundle.MainBundle.ResourcePath;
				break;
			default:
				NSSearchPathDirectory dir;
				NSSearchPathDomain domain;
				Convert (folder, out dir, out domain);
				return NSSearchPath.GetDirectories (dir, domain, true).FirstOrDefault ();
			}
		}

		#region IWidget implementation
		
		public void Initialize ()
		{
		}

		public Widget Widget { get; set; }
		
		#endregion
	}
}

