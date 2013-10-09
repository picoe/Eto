using System;
using MonoTouch.UIKit;
using Eto.Forms;
using Eto.Drawing;
using Eto.Platform.Mac.Forms;
using sd = System.Drawing;

namespace Eto.Platform.iOS.Forms
{
	public static class iosViewExtensions
	{
		public static void AddSubview (this Control control, Control subView, bool useRoot = false)
		{
			var parentController = control.GetViewController (false);
			if (parentController != null) {
				parentController.AddChildViewController (subView.GetViewController ());
				return;
			}
			if (useRoot) {
				var window = control.GetContainerView () as UIWindow;
				if (window != null) {
					window.RootViewController = subView.GetViewController ();
					return;
				}
			}
			var parentView = control.GetContentView ();
			if (parentView != null) {
				parentView.AddSubview (subView.GetContainerView ());
				return;
			}

			throw new EtoException("Coult not add subview to parent");
		}

		public static UIViewController GetViewController (this Control control, bool force = true)
		{
			if (control == null)
				return null;
			
			var controller = control.Handler as IiosViewController;
			if (controller != null) {
				return controller.Controller;
			}
			if (force) {
				var view = control.GetContainerView ();
				if (view != null) {
					var viewcontroller = new RotatableViewController {
					Control = control, 
					View = view
				};
					return viewcontroller;
				}
			}
			return null;
		}
	}
}

