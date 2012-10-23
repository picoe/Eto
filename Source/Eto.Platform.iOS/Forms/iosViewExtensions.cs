using System;
using MonoTouch.UIKit;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.iOS.Forms
{
	public static class MacViewExtensions
	{
		public static UIView GetContainerView (this Control control)
		{
			if (control == null)
				return null;
			var viewHandler = control.Handler as IiosView;
			if (viewHandler != null)
				return viewHandler.ContainerControl;
			return control.ControlObject as UIView;
		}

		public static UIView GetContentView (this Control control)
		{
			if (control == null)
				return null;
			var containerHandler = control.Handler as IiosContainer;
			if (containerHandler != null)
				return containerHandler.ContentControl;
			return control.ControlObject as UIView;
		}

		public static void AddSubview(this Control control, Control subView, bool useRoot = false)
		{
			var parentController = control.GetViewController (false);
			if (parentController != null) {
				parentController.AddChildViewController(subView.GetViewController());
				return;
			}
			var parentView = control.GetContentView ();
			parentView.AddSubview (subView.GetContainerView ());
		}

		public static Size GetPreferredSize(this Control control)
		{
			if (control == null)
				return Size.Empty;
			var mh = control.Handler as IiosView;
			if (mh != null) {
				return mh.GetPreferredSize ();
			}
			
			var c = control.ControlObject as UIView;
			if (c != null) {
				return c.SizeThatFits(UIView.UILayoutFittingCompressedSize).ToEtoSize ();
			}
			return Size.Empty;
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

