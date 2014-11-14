using System;
using System.Linq;
using MonoTouch.UIKit;
using Eto.Forms;
using System.Threading.Tasks;
using MonoTouch.ObjCRuntime;

namespace Eto.iOS.Forms
{
	class RotatableViewController : UIViewController
	{

		public RotatableViewController()
		{
			if (this.AutomaticallyAdjustsScrollViewInsetsIsSupported())
				AutomaticallyAdjustsScrollViewInsets = true;
			if (this.ExtendLayoutIncludesOpaqueBarsIsSupported())
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
