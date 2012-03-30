using System;
using System.Linq;
using MonoTouch.UIKit;
using Eto.Forms;

namespace Eto.Platform.iOS.Forms
{
	internal class RotatableViewController : UIViewController
	{
		public object Control { get; set; }
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}
		
		protected override void Dispose (bool disposing)
		{
			var c = Control as IDisposable;
			if (c != null) {
				c.Dispose ();
				c = null;
			}
			
			base.Dispose (disposing);
		}
	}

	public class DialogHandler : iosWindow<UIView, Dialog>, IDialog, IiosViewController
	{
		Button button;

		public UIViewController Controller { get; set; }

		public DialogHandler ()
		{
			Controller = new RotatableViewController ();
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

		public override string Text {
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
			var controller = parent.Handler as IiosViewController;
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

