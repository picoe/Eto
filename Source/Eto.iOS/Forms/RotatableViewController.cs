using System;
using System.Linq;
using MonoTouch.UIKit;
using Eto.Forms;
using System.Threading.Tasks;

namespace Eto.iOS.Forms
{
	internal class RotatableViewController : UIViewController
	{
		public RotatableViewController()
		{
			AutomaticallyAdjustsScrollViewInsets = true;
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
