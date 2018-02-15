using System;
using Foundation;
using UIKit;
using Eto.iOS.Forms;
using Eto.Forms;

namespace Eto.iOS
{
	[Foundation.Register("EtoAppDelegate")]
	public class EtoAppDelegate : UIApplicationDelegate
	{
		public EtoAppDelegate()
		{
		}

		public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
		{
			ApplicationHandler.Instance.Initialize(this);
			return true;
		}
	}
}

