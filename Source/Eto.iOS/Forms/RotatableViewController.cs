using System;
using System.Linq;
using MonoTouch.UIKit;
using Eto.Forms;
using System.Threading.Tasks;
using MonoTouch.ObjCRuntime;

namespace Eto.iOS.Forms
{
	internal class RotatableViewController : UIViewController
	{
		static readonly Selector selAutomaticallyAdjustsScrollViewInsets = new Selector("automaticallyAdjustsScrollViewInsets");
		static readonly Selector selExtendLayoutIncludesOpaqueBars = new Selector("extendedLayoutIncludesOpaqueBars");

		public RotatableViewController()
		{
			if (RespondsToSelector(selAutomaticallyAdjustsScrollViewInsets))
				AutomaticallyAdjustsScrollViewInsets = true;
			if (RespondsToSelector(selExtendLayoutIncludesOpaqueBars))
				ExtendedLayoutIncludesOpaqueBars = true;
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
		{
			return UIInterfaceOrientationMask.All;
		}

		[Obsolete]
		public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}
	}
	
}
