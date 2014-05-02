using System;
using System.Linq;
using MonoTouch.UIKit;
using Eto.Forms;

namespace Eto.Platform.iOS.Forms
{
	public class ModalWindowHandler : IosWindow<UIView, ModalWindow>, IModalWindow
	{
		Button button;
		Action completionHandler;

		public ModalWindowHandler()
		{
			Control = new UIView();
			Control.Frame = UIScreen.MainScreen.Bounds;
			Controller.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
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

		public ModalWindowDisplayMode DisplayMode {
			get;
			set;
		}

		public override string Title {
			get { return Controller.Title; }
			set { Controller.Title = value; }
		}

		public override void Close ()
		{
			// Per the SDK docs, DismissViewController on the presented view controller
			// forwards to the presenting view controller which is responsible for dismissing it.
			Controller.DismissViewController(animated: false, completionHandler: () => {
				if (completionHandler != null)
					completionHandler();
			});
		}
		
		public void ShowModal(Action completed, Control parent)
		{
			completionHandler = completed;
			// For iOS we ignore parent and determine the topmost view controller
			// as discussed in http://stackoverflow.com/a/12684721
			// 
			var topController = UIApplication.SharedApplication.KeyWindow.RootViewController;
			while (topController.PresentedViewController != null)
				topController = topController.PresentedViewController;

			topController.PresentViewController(Controller, animated: true, completionHandler: null);
		}
	}
}

