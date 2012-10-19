using System;
using MonoTouch.UIKit;
using Eto.Forms;

namespace Eto.Platform.iOS.Forms.Controls
{
	internal class RotatableNavigationController : UINavigationController
	{
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return true; 
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.All;
		}
	}
	
	public class NavigationHandler : iosControl<UIView, Navigation>, INavigation, IiosViewController
	{
		public UIViewController Controller { get { return Navigation; } }
		
		public UINavigationController Navigation { get; set; }
		
		class Delegate : UINavigationControllerDelegate
		{
			public override void DidShowViewController (UINavigationController navigationController, UIViewController viewController, bool animated)
			{
				Handler.Widget.OnItemShown (EventArgs.Empty);
			}
			
			public NavigationHandler Handler { get; set; }

		}
		
		public NavigationHandler ()
		{
			Navigation = new RotatableNavigationController {
				Delegate = new Delegate { Handler = this }
			};
		}
		
		public override UIView Control {
			get {
				return Navigation.View;
			}
		}
		
		public void Push (INavigationItem item)
		{
			var view = item.Content.GetViewController ();
			view.NavigationItem.Title = item.Text;
			view.View.Frame = Control.Frame;
			view.View.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
			Navigation.PushViewController (view, true);
		}

		public void Pop ()
		{
			Navigation.PopViewControllerAnimated (true);
		}
	}
}

