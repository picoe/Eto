using System;
using MonoMac.AppKit;
using MonoMac.Foundation;
using Eto.Forms;
namespace Eto.Platform.Mac
{
	//[MonoMac.Foundation.Register("AppDelegate")]
	public class AppDelegate : NSApplicationDelegate
	{
		
		public AppDelegate ()
		{
		}
		
		public override void DidFinishLaunching (NSNotification notification)
		{
			var handler = Application.Instance.Handler as ApplicationHandler;
			if (handler != null) handler.Initialize(this);
		}
	}
}

