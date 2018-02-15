using System;
using System.Linq;
using UIKit;
using Eto.Forms;
using System.Threading.Tasks;
using ObjCRuntime;

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

		public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}
	}
	
}
