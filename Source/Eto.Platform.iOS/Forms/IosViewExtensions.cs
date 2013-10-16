using System;
using MonoTouch.UIKit;
using Eto.Forms;
using Eto.Drawing;
using Eto.Platform.Mac.Forms;
using sd = System.Drawing;

namespace Eto.Platform.iOS.Forms
{
	public static class IosViewExtensions
	{
		public static UIViewController GetViewController(this Control control, bool force = true)
		{
			if (control == null)
				return null;
			
			var controller = control.Handler as IIosView;
			if (controller != null)
			{
				return controller.Controller;
			}
			if (force)
			{
				var view = control.GetContainerView();
				if (view != null)
				{
					var viewcontroller = new RotatableViewController
					{
						View = view
					};
					return viewcontroller;
				}
			}
			return null;
		}
	}
}

