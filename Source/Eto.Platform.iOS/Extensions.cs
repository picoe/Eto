using System;
using MonoTouch.UIKit;
using System.Drawing;
using Eto.Forms;
using Eto.Platform.iOS.Forms;

namespace Eto.Platform.iOS
{
	public static class Extensions
	{
		public static void SetFrameSize(this UIView view, SizeF size)
		{
			var frame = view.Frame;
			frame.Size = size;
			view.Frame = frame;
		}
		public static void SetFrameOrigin(this UIView view, PointF location)
		{
			var frame = view.Frame;
			frame.Location = location;
			view.Frame = frame;
		}
		
		public static UIViewController GetViewController(this Control control)
		{
			if (control == null) return null;
			
			var controller = control.Handler as IiosViewController;
			if (controller != null)
			{
				return controller.Controller;
			}
			UIView view = control.ControlObject as UIView;
			if (view != null)
			{
				var viewcontroller = new RotatableViewController();
				viewcontroller.View = view;
				return viewcontroller;
			}
			return null;
		}
	}
}

