using System;
using System.Linq;
using MonoTouch.UIKit;
using Eto.Forms;

namespace Eto.Platform.iOS.Forms
{
	internal class RotatableViewController : UIViewController
	{
		public RotatableViewController()
		{
			AutomaticallyAdjustsScrollViewInsets = true;
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.All;
		}

		[Obsolete]
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}
	}

	public class DialogHandler : IosWindow<UIView, Dialog>, IDialog
	{
		Button button;

		public DialogHandler ()
		{
			Control = Controller.View;
		}
		
		public Button AbortButton { get; set; }
		
		public Button DefaultButton {
			get {
				return button;	
			}
			set {
				button = value;
				// TODO: implement?
			}
		}

		public DialogDisplayMode DisplayMode {
			get;
			set;
		}

		public override string Title {
			get { return Controller.Title; }
			set { Controller.Title = value; }
		}

		public override void Close ()
		{
			var viewControllers = Controller.NavigationController.ViewControllers.ToList ();
			int index = viewControllers.IndexOf (Controller);
			if (index > 1)
				Controller.NavigationController.PopToViewController (viewControllers [index - 1], true);
		}
		
		public DialogResult ShowDialog (Control parent)
		{
			var controller = parent.Handler as IIosView;
			if (controller != null) {
				var nav = controller.Controller.NavigationController;
				if (nav != null) {
					nav.PushViewController (Controller, true);
				}
			}
			return Widget.DialogResult;
		}
	}
}

