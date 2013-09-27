using System;
using MonoTouch.UIKit;
using Eto.Forms;

namespace Eto.Platform.iOS.Forms.Controls
{
	internal class RotatableNavigationController : UINavigationController
	{
		[Obsolete]
		public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
		{
			return true; 
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
		{
			return UIInterfaceOrientationMask.All;
		}
	}

	public class NavigationHandler : iosControl<UIView, Navigation>, INavigation, IiosViewController
	{
		public override UIViewController Controller { get { return Navigation; } }

		public UINavigationController Navigation { get; set; }

		class Delegate : UINavigationControllerDelegate
		{
			public override void DidShowViewController(UINavigationController navigationController, UIViewController viewController, bool animated)
			{
				Handler.Widget.OnItemShown(EventArgs.Empty);
				// need to get the view controllers to reset the references to the popped controllers
				// this is due to how xamarin.ios keeps the controllers in an array
				// and this resets that array
				var controllers = Handler.Navigation.ViewControllers;
				/* for testing garbage collection after a view is popped
				#if DEBUG
				GC.Collect();
				GC.WaitForPendingFinalizers();
				#endif
				/**/
			}

			WeakReference handler;

			public NavigationHandler Handler { get { return (NavigationHandler)handler.Target; } set { handler = new WeakReference(value); } }
		}

		public NavigationHandler()
		{
			Navigation = new UINavigationController
			{
				WeakDelegate = new Delegate { Handler = this }
			};
			Navigation.NavigationBar.Translucent = false;
		}

		public override UIView Control { get { return Navigation.View; } }

		public void Push(INavigationItem item)
		{
			var view = item.Content.GetViewController();
			view.NavigationItem.Title = item.Text;
			view.View.Frame = Control.Frame;
			view.View.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
			Navigation.PushViewController(view, true);
		}

		public void Pop()
		{
			Navigation.PopViewControllerAnimated(true);
		}
	}
}

