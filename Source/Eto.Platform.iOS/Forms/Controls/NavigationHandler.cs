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
	}
	
	public class NavigationHandler : iosControl<UIView, Navigation>, INavigation, IiosViewController
	{
		public UIViewController Controller { get { return Navigation; } }
		
		public UINavigationController Navigation { get; set; }
		
		public NavigationHandler ()
		{
			Navigation = new RotatableNavigationController();
		}
		
		public override UIView Control {
			get {
				return Navigation.View;
			}
		}
		
		#region INavigation implementation
		
		public void Push (Control control)
		{
			var view = control.GetViewController();
			//view.NavigationItem.Title = title;
			Navigation.PushViewController(view, true);
		}

		public void Pop ()
		{
			Navigation.PopViewControllerAnimated(true);
		}
		#endregion

	}
}

