using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Eto.Platform.iOS.Forms;
using Eto.Forms;

namespace Eto.Platform.iOS
{
	[MonoTouch.Foundation.Register("EtoAppDelegate")]
	public class EtoAppDelegate : UIApplicationDelegate
	{
		public EtoAppDelegate ()
		{
		}

		public override bool FinishedLaunching (UIApplication application, NSDictionary launcOptions)
		{
			ApplicationHandler.Instance.Initialize(this);
			return true;
		}
	}
}

