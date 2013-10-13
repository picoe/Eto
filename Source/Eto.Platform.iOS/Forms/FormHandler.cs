using System;
using MonoTouch.UIKit;
using Eto.Forms;
using System.Linq;

namespace Eto.Platform.iOS.Forms
{
	public class FormHandler : IosWindow<UIWindow, Form>, IForm
	{
		public FormHandler()
		{
			Control = new UIWindow(UIScreen.MainScreen.Bounds);
			Control.AutosizesSubviews = true;
			Control.RootViewController = new RotatableViewController();
			Control.RootViewController.View.AutosizesSubviews = true;
			Control.RootViewController.View.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
		}

		public override UIView ContentControl
		{
			get { return Control.RootViewController.View; }
		}

		public override string Title
		{
			get { return null; }
			set { }
		}

		public override void Close()
		{
 			Control.RemoveFromSuperview();
			/*
			var viewControllers = Controller.NavigationController.ViewControllers.ToList();
			int index = viewControllers.IndexOf(Controller);
			if (index > 1) Controller.NavigationController.PopToViewController(viewControllers[index-1], true);
			*/
		}

		public void Show()
		{
			Control.MakeKeyAndVisible();
			//ApplicationHandler.Instance.Navigation.PushViewController(Controller, true);
		}
	}
}

