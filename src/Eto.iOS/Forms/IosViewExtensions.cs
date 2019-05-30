using System;
using UIKit;
using Eto.Forms;
using Eto.Drawing;
using Eto.Mac.Forms;
using sd = System.Drawing;

namespace Eto.iOS.Forms
{
	public static class IosViewExtensions
	{
		public static UIViewController GetViewController(this Widget control, bool force = true)
		{
			if (control == null)
				return null;
			
			var iosView = control.Handler as IIosViewControllerSource;
			if (iosView != null)
			{
				var controller = iosView.Controller;
				if (controller != null)
					return controller;
			}
			if (force)
			{
				var view = control.GetContainerView();
				if (view != null)
				{
					var viewcontroller = new RotatableViewController { View = view };
					if (iosView != null)
						iosView.Controller = viewcontroller;
					return viewcontroller;
				}
			}
			return null;
		}

		public static void AddChild(this IIosView parent, Widget control)
		{
			var parentViewController = parent.Controller;
			if (parentViewController != null)
			{
				// wire up view controllers, if both have one
				var childController = control.GetViewController(false);
				if (childController != null)
					parentViewController.AddChildViewController(childController);
			}

			parent.ContentControl.AddSubview(control.GetContainerView());
		}
		public static void RemoveChild(this IIosView parent, Widget control)
		{
			var parentViewController = parent.Controller;
			if (parentViewController != null)
			{
				// wire up view controllers, if both have one
				var childController = control.GetViewController(false);
				if (childController != null)
					childController.RemoveFromParentViewController();
			}

			control.GetContainerView().RemoveFromSuperview();
		}
	}
}

